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

    [Serializable]
    public class FlightsDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome to Travel Surfer!");

            var flightsFormDialog = FormDialog.FromForm(this.BuildFlightsForm, FormOptions.PromptInStart);

            context.Call(flightsFormDialog, this.ResumeAfterFlightsFormDialog);
        }

        private IForm<FlightsQuery> BuildFlightsForm()
        {
            OnCompletionAsyncDelegate<FlightsQuery> processflightsSearch = async (context, state) =>
            {
                await context.PostAsync($"Searching for free round trip flights from {state.Origin} to {state.Destination} departing {state.DepartDate.ToString("MM/dd/YYYY")} and returning {state.ReturnDate.ToString("MM/dd/YYYY")}...");
            };

            return new FormBuilder<FlightsQuery>()
                .Field(nameof(FlightsQuery.Destination))
                // CHANGE this
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

                var flights = await GetflightsAsync(searchQuery);

                await context.PostAsync($"I found in total {flights.Count()} flights for your dates:");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var flight in flights)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = flight.Name,
                        Subtitle = $"{flight.Rating} starts. {flight.NumberOfReviews} reviews. From ${flight.PriceStarting} per night.",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = flight.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=flights+in+" + HttpUtility.UrlEncode(flight.Location)
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
        private async Task<IEnumerable<CreditCard>> GetflightsAsync(FlightsQuery searchQuery)
        {
            var cards = new List<CreditCard>();

            // Filling the credit cards results manually just for demo purposes
            /// TODO: Populate from the JSON
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                CreditCard card = new CreditCard()
                {
                    Name = $"{searchQuery.Destination} Credit Card {i}",
                    Location = searchQuery.Destination,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"https://placeholdit.imgix.net/~text?txtsize=35&txt=Flight{i}&w=500&h=260"
                };

                cards.Add(card);
            }

            cards.Sort((card1, card2) => card1.PriceStarting.CompareTo(card2.PriceStarting));

            return cards;
        }
    }
}