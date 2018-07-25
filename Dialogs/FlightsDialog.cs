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
            await context.PostAsync("Welcome to Travel Surfer!");

            var flightsFormDialog = FormDialog.FromForm(BuildFlightsForm, FormOptions.PromptInStart);

            context.Call(flightsFormDialog, ResumeAfterFlightsFormDialog);
        }

        private IForm<FlightsQuery> BuildFlightsForm()
        {
            OnCompletionAsyncDelegate<FlightsQuery> processflightsSearch = async (context, state) =>
            {
                await context.PostAsync($"Searching for free round trip flights from {state.Origin} to {state.Destination} departing {state.DepartDate.ToString("MM/dd/YYYY")} and returning {state.ReturnDate.ToString("MM/dd/YYYY")}...");
            };

            return new FormBuilder<FlightsQuery>()
                .Field(nameof(FlightsQuery.Destination))
                // TODO Look into pjhrasing for this
                .Message("Looking for free flights to {Destination}...")
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

                await context.PostAsync($"I found in total {cards.Count()} flights for your dates:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var card in cards)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        // Todo: images?
                        Title = string.Format("{0} by {1}", card.Name, card.Issuer),
                        Subtitle = string.Format("Get {0} points/miles bonus by spending ${1} within {2} days.", card.Bonus, card.MinimumSpend, card.DaysForMinSpend),
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
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the FlightsDialog";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
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
                if (aCard.MinimumSpend <= 25000)
                    weight += 10;
                else if (aCard.MinimumSpend <= 10000)
                    weight += 5;
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