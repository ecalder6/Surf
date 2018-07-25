namespace SurferBot.Dialogs
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
            var welcomeMessage = context.MakeMessage();
            var message = $"Welcome to Travel Surfer!";
            welcomeMessage.Text = message;
            welcomeMessage.Speak = message;
            await context.PostAsync(welcomeMessage);

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
                .Message("Looking for best credit cards to use to fly to {Destination}...")
                .AddRemainingFields()
                .OnCompletion(processflightsSearch)
                .Build();
        }

        private async Task ResumeAfterFlightsFormDialog(IDialogContext context, IAwaitable<FlightsQuery> result)
        {
            try
            {
                var searchQuery = await result;
                var cards = await GetCards(searchQuery);

                var response = context.MakeMessage();
                var responseMessage = $"I found the {cards.Count()} best cards to use for your travel dates. " +
                    $"They are sorted by a recommendation score I have computed for you.";
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
                        Subtitle = string.Format("Weighted Score: {0}\nGet {1} points/miles bonus by spending ${2} within {3} days.", 
                        card.RecommendationWeight, card.Bonus, card.MinimumSpend, card.DaysForMinSpend),
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=" + HttpUtility.UrlEncode(string.Format("{0} {1}", card.Issuer, card.Name))
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
        private async Task<IEnumerable<Card>> GetCards(FlightsQuery searchQuery)
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

            // Where the recommendation engine comes into play.
            // Filter for the recommended credit cards.
            List<Card> recommendedCreditCards = GenerateRecommendations(creditCards);

            return creditCards;
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