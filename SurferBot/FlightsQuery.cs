namespace SurferBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class FlightsQuery
    {
        [Prompt("Please enter your origin")]
        public string Origin { get; set; }

        [Prompt("Please enter your destination")]
        public string Destination { get; set; }
        
        [Prompt("What date do you want to leave?")]
        public DateTime DepartDate { get; set; }

        [Prompt("When do you want to return?")]
        public DateTime ReturnDate { get; set; }
    }
}