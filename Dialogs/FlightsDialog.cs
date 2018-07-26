namespace Surf.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;
    using PlaceInfoJson;

    [Serializable]
    public class FlightsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var flightsFormDialog = FormDialog.FromForm(BuildFlightsForm, FormOptions.PromptInStart);

            context.Call(flightsFormDialog, ResumeAfterFlightsFormDialog);
        }
        
        private IForm<FlightsQuery> BuildFlightsForm()
        {
            OnCompletionAsyncDelegate<FlightsQuery> processflightsSearch = async (context, state) =>
            {
                var searchMessage = context.MakeMessage();
                var message = $"Searching for best credit cards to use on round trip flights from {state.Origin} to {state.Destination} " +
                $"departing {state.DepartDate.ToString("MMMM dd, yyyy")} and returning {state.ReturnDate.ToString("MMMM dd, yyyy")}...";
                searchMessage.Text = message;
                searchMessage.Speak = message;
                await context.PostAsync(searchMessage);
            };

            return new FormBuilder<FlightsQuery>()
                .Field(nameof(FlightsQuery.Destination))
                .Field(nameof(FlightsQuery.Origin))
                .Field(nameof(FlightsQuery.DepartDate))
                .Field(nameof(FlightsQuery.ReturnDate),
                    validate: async (state, response) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = response };
                        if (state.DepartDate > (DateTime)response)
                        {
                            result.IsValid = false;
                            result.Feedback = "Return date can't be before departure date";
                        }
                        return result;
                    })
                .AddRemainingFields()
                .OnCompletion(processflightsSearch)
                .Build();
        }

        private async Task ResumeAfterFlightsFormDialog(IDialogContext context, IAwaitable<FlightsQuery> result)
        {
            try
            {
                var searchQuery = await result;
                (IEnumerable<Card> cards, float price) = await GetCards(searchQuery);

                var response = context.MakeMessage();
                var responseMessage = $"I found {cards.Count()} credit cards to make this trip free.";
                if (price > 0)
                {
                    responseMessage += $" Signing up for one of these cards will save you approximately ${price}!";
                }
                response.Text = responseMessage;
                response.Speak = responseMessage;
                await context.PostAsync(response);

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var card in cards)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        // Todo: images?
                        Title = string.Format("{0} by {1}", card.Name, card.Issuer),
                        Subtitle = string.Format("Get {1} points/miles bonus by spending ${2} within {3} months.", 
                        card.RecommendationWeight, card.Bonus, card.MinimumSpend, card.DaysForMinSpend / 30),
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=" + HttpUtility.UrlEncode(string.Format("{0} {1} credit card", card.Issuer, card.Name))
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                var response = context.MakeMessage();
                await context.PostAsync(response);

                if (ex.InnerException == null)
                {
                    var cancelationMessage = "You have canceled the operation. Quitting from the FlightsDialog...";
                    response.Text = cancelationMessage;
                    response.Speak = cancelationMessage;
                }
                else
                {
                    var errorSpeak = "Something went wrong in FlightsDialog. Please find the exception text below for guidance:";
                    var errorText = $"{errorSpeak}\n{ex}";
                    response.Text = errorText;
                    response.Speak = errorSpeak;
                }

                await context.PostAsync(response);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        /// TODO: Populate the credit cards from the JSON
        private async Task<(IEnumerable<Card> cards, float price)> GetCards(FlightsQuery searchQuery)
        {
            CardFinder cardFinder = new CardFinder();
            cardFinder.PopulateAirports();
            cardFinder.PopulateAirlines();
            cardFinder.PopulateCards();
            DateTime departureDate = searchQuery.DepartDate;
            DateTime returnDate = searchQuery.ReturnDate;
            Location origin = await cardFinder.GetPlace(searchQuery.Origin);
            Location destination = await cardFinder.GetPlace(searchQuery.Destination);
            float minPrice = await cardFinder.GetFlightPriceAsync(origin, destination, departureDate, returnDate);

            List<string> fromAirports = new List<string>();
            List<string> toAirports = new List<string>();
            cardFinder.AddAirports(fromAirports, origin, searchQuery.Origin);
            cardFinder.AddAirports(toAirports, destination, searchQuery.Destination);
            List<Card> creditCards = await cardFinder.GetAwardCreditCards(fromAirports, toAirports);
            if (minPrice > 0)
            {
                creditCards.AddRange(cardFinder.GetCashBackCreditCards(minPrice));
            }

            // Where the recommendation engine comes into play.
            // Filter for the recommended credit cards.
            List<Card> recommendedCreditCards = GenerateRecommendations(creditCards);

            return (cards: recommendedCreditCards, price: minPrice);
        }

        private List<Card> GenerateRecommendations(List<Card> cards)
        {
            List<Card> intermediateListOfCards = new List<Card>();
            Console.WriteLine("============List Of Cards=================");
            foreach (Card aCard in cards)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", 
                    aCard.Name, aCard.RecommendationWeight, aCard.Bonus, aCard.DaysForMinSpend, 
                    aCard.IsCurrentBestOffer, aCard.MinimumSpend, aCard.IsPersonal);
                int weight = 0;
                if (aCard.AnnualFee == 0 ? true : false)
                    weight += 10;
                else if (aCard.AnnualFee > 0 && aCard.IsFirstAnnualFeeWaived == true)
                    weight += 2;
                if (aCard.Bonus > 50000)
                    weight += 10;
                else if (aCard.Bonus > 25000)
                    weight += 5;
                if (aCard.DaysForMinSpend > 90)
                    weight += 10;
                else if (aCard.DaysForMinSpend > 30)
                    weight += 5;
                if (aCard.IsCurrentBestOffer == true)
                    weight += 15;
                if (aCard.MinimumSpend == 0)
                    weight += 10;
                else if (aCard.MinimumSpend <= 1000)
                    weight += 6;
                else if (aCard.MinimumSpend <= 3000)
                    weight += 2;
                if (aCard.IsPersonal == true) // this is to be confirmed if any preference needs to be applied if this is a personal card since the requester criteria data is unavailable
                    weight += 10;
                aCard.RecommendationWeight = weight;
                intermediateListOfCards.Add(aCard);
            }
            Console.WriteLine("-------------------------------------------");
            List<Card> SortedList = intermediateListOfCards.OrderByDescending(o => o.RecommendationWeight).ToList();
            Console.WriteLine("============List Of Cards Sorted by Recommendation =================");
            foreach (Card aCard in SortedList)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                    aCard.Name.ToString(), aCard.RecommendationWeight.ToString(), aCard.Bonus.ToString(), aCard.DaysForMinSpend.ToString(), aCard.IsCurrentBestOffer.ToString(), aCard.MinimumSpend.ToString(), aCard.IsPersonal.ToString(), aCard.AnnualFee.ToString(), aCard.IsFirstAnnualFeeWaived.ToString());
            }
            Console.WriteLine("-------------------------------------------");

            return SortedList;
        }
    }
}