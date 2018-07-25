namespace Surf
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class FlightsQuery
    {
        [Prompt("Which city or airport are you leaving from?")]
        public string Origin { get; set; }

        [Prompt("Where would you like to go?")]
        public string Destination { get; set; }
        
        [Prompt("When would you like to go?")]
        public DateTime DepartDate { get; set; }

        [Prompt("When would you like to come back?")]
        public DateTime ReturnDate { get; set; }
    }
}