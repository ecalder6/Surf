using System.Collections.Generic;

namespace SurferBot
{
    class Card
    {
        public string Issuer { get; set; }
        public string Name { get; set; }
        public string RewardProgram { get; set; }
        public bool IsPersonal { get; set; }
        public int DaysForMinSpend { get; set; }
        public float AnnualFee { get; set; }
        public bool IsFirstAnnualFeeWaived { get; set; }
        public bool IsCurrentBestOffer { get; set; }
        public float Bonus { get; set; }
        public float MinimumSpend { get; set; }
        public int RecommendationWeight { get; set; }

        public Card(string issuer, string name, string rewardProgram,
            bool isPersonal, int daysForMinSpend, float annualFee,
            bool isCurrentBestOffer, float bonus, bool isFirstFeeWaived,
            float minSpend, int recommendationWeight)
        {
            Issuer = issuer;
            Name = name;
            RewardProgram = rewardProgram;
            IsPersonal = isPersonal;
            DaysForMinSpend = daysForMinSpend;
            AnnualFee = annualFee;
            IsCurrentBestOffer = isCurrentBestOffer;
            Bonus = bonus;
            IsFirstAnnualFeeWaived = isFirstFeeWaived;
            MinimumSpend = minSpend;
            RecommendationWeight = recommendationWeight;
        }

        public override string ToString()
        {
            return Name + ", Issuer: " + Issuer + ", Bonus: " + Bonus;
        }
    }

    class Node
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Dictionary<string, float> ToNodes { get; set; }
        public Dictionary<string, float> FromNodes { get; set; }

        public Node(string code, string name)
        {
            Code = code;
            Name = name;
            ToNodes = new Dictionary<string, float>();
            FromNodes = new Dictionary<string, float>();
        }
    }
}
