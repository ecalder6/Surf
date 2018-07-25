namespace Surf
{
    using System;

    [Serializable]
    /// TODO populate correctly
    public class CreditCard
    {
        public string Name { get; set; }

        public int Rating { get; set; }

        public int NumberOfReviews { get; set; }

        public int PriceStarting { get; set; }

        public string Image { get;  set; }

        public string Location { get;  set; }
    }
}