namespace Surf
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class FlightsQuery
    {
        // TODO see if we need something like this to get this read
        // speak: SSMLHelper.Speak()
        [Prompt("Please tell me which city or airport you are leaving from?")]
        public string Origin { get; set; }

        [Prompt("Please tell me which city or airport you are headed to?")]
        public string Destination { get; set; }
        
        [Prompt("What date do you want to leave?")]
        public DateTime DepartDate { get; set; }

        [Prompt("What date do you want to return?")]
        public DateTime ReturnDate { get; set; }
    }
}