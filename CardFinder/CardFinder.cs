namespace Surf
{
    using Newtonsoft.Json;
    using AwardInfoJson;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using FlightQuoteJson;
    using PlaceInfoJson;
    using AirportInfoJson;

    class CardFinder
    {
        private static readonly HttpClient _client = new HttpClient();
        private const string _baseAddress = "https://www.awardhacker.com/award-charts/";
        public Dictionary<string, SortedList<float, List<Card>>> RewardToCards = new Dictionary<string, SortedList<float, List<Card>>>();
        public Dictionary<string, string> AirlineToCode = new Dictionary<string, string>();
        public Dictionary<string, string> CodeToAirline = new Dictionary<string, string>();
        public Dictionary<string, Node> Graph = new Dictionary<string, Node>();
        public Dictionary<string, List<AirportInfo>> CityToAirports = new Dictionary<string, List<AirportInfo>>();
        public HashSet<string> AirportCodes = new HashSet<string>();

        public async Task<List<Card>> GetAwardCreditCards(List<string> fromAirports, List<string> toAirports)
        {
            _client.DefaultRequestHeaders.Clear();
            string address = _baseAddress;
            address += GenerateAwardURL(fromAirports, 'f');
            address += GenerateAwardURL(toAirports, 't');
            address += "&o=0&c=y&s=1&p=0&n=10&v=2";
            string response = await _client.GetStringAsync(address);
            var awardInfo = JsonConvert.DeserializeObject<AwardInfo>(response);
            HashSet<Card> cards = new HashSet<Card>();
            foreach (Result result in awardInfo.Results)
            {
                long miles = result.Miles;
                string code = result.Airline.Code;
                foreach (Card card in FindRewardsCards(miles, code))
                {
                    cards.Add(card);
                }
            }
            return cards.ToList();
        }

        public List<Card> GetCashBackCreditCards(float price)
        {
            List<Card> result = new List<Card>();
            Node cashNode = Graph["Cash"];
            AddCardsToResult(result, RewardToCards["Cash"], cashNode, price, 1);
            return result;
        }

        public async Task<float> GetFlightPriceAsync(Location origin, Location destination, DateTime departureDate, DateTime returnDate)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-Mashape-Key", "BJR61WRk64mshsNl6JvNqCPIPJV6p1f4wvgjsn4SNJAEHXWdQe");
            _client.DefaultRequestHeaders.Add("X-Mashape-Host", "skyscanner-skyscanner-flight-search-v1.p.mashape.com");
            string address = string.Format("https://skyscanner-skyscanner-flight-search-v1.p.mashape.com/apiservices/browsequotes/v1.0/US/USD/en-US/{0}/{1}/{2}/{3}",
                origin.PlaceId, destination.PlaceId, departureDate.ToString("yyyy'-'MM'-'dd"), returnDate.ToString("yyyy'-'MM'-'dd"));
            Console.WriteLine(address);
            string response = string.Empty;
            try
            {
                response = await _client.GetStringAsync(address);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
            var flightQuotes = JsonConvert.DeserializeObject<FlightQuote>(response);
            SortedList<float, int> minPrices = new SortedList<float, int>();
            foreach (Quote quote in flightQuotes.Quotes)
            {
                if (!minPrices.TryGetValue(quote.MinPrice, out int count))
                {
                    minPrices[quote.MinPrice] = 0;
                }
                minPrices[quote.MinPrice] += 1;
            }
            int curIndex = 0;
            int med = minPrices.Count / 2;
            foreach (int count in minPrices.Values)
            {
                if (curIndex >= med)
                {
                    break;
                }
                curIndex += count;
            }
            return curIndex < flightQuotes.Quotes.Length ? minPrices.Keys[curIndex] : -1;
        }

        public async Task<Location> GetPlace(string input)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-Mashape-Key", "BJR61WRk64mshsNl6JvNqCPIPJV6p1f4wvgjsn4SNJAEHXWdQe");
            _client.DefaultRequestHeaders.Add("X-Mashape-Host", "skyscanner-skyscanner-flight-search-v1.p.mashape.com");
            string address = string.Format("https://skyscanner-skyscanner-flight-search-v1.p.mashape.com/apiservices/autosuggest/v1.0/US/USD/en-US/?query={0}", input);
            string response = await _client.GetStringAsync(address);
            var placeInfo = JsonConvert.DeserializeObject<PlaceInfo>(response);
            Location place = placeInfo.Places.First();
            if (place.PlaceName == "L.A")
            {
                place.PlaceName = "Los Angeles";
            }
            else if (place.PlaceName == "NYC")
            {
                place.PlaceName = "New York City";
            }
            return place;
        }

        private List<Card> FindRewardsCards(long miles, string code)
        {
            List<Card> result = new List<Card>();
            if (!Graph.TryGetValue(code, out Node start))
            {
                Console.WriteLine(code + " DOES NOT EXIST IN GRAPH!");
                return result;
            }
            Console.WriteLine("Finding cards for " + code);

            Queue<(Node node, float mult)> toVisit = new Queue<(Node node, float mult)>();
            HashSet<string> visited = new HashSet<string>();
            toVisit.Enqueue((node: start, mult: 1));
            while (toVisit.Count > 0)
            {
                (Node node, float mult) = toVisit.Dequeue();
                Console.WriteLine("Looking at " + code + " cards");
                if (visited.Contains(node.Code))
                {
                    continue;
                }
                visited.Add(node.Code);

                SortedList<float, List<Card>> sortedList;
                if (RewardToCards.TryGetValue(node.Code, out sortedList) || (CodeToAirline.TryGetValue(node.Code, out string airlineName) && RewardToCards.TryGetValue(airlineName, out sortedList)))
                {
                    AddCardsToResult(result, sortedList, node, miles, mult);
                }

                foreach (KeyValuePair<string, float> entry in node.FromNodes)
                {
                    string toCode = entry.Key;
                    float multiplier = entry.Value;
                    if (!visited.Contains(toCode))
                    {
                        toVisit.Enqueue((node: Graph[toCode], mult: mult * multiplier));
                    }
                }
            }

            return result;
        }

        private void AddCardsToResult(List<Card> result, SortedList<float, List<Card>> sortedList, Node node, float miles, float mult)
        {
            List<float> keys = sortedList.Keys.ToList();
            int index = keys.BinarySearch(miles * mult);
            if (index >= 0 || ~index < keys.Count)
            {
                if (index < 0)
                {
                    index = ~index;
                }
                for (int i = index; i < keys.Count; i++)
                {
                    result.AddRange(sortedList[keys[i]]);
                }
            }
        }

        public List<Card> FindCashCards(float price)
        {
            List<Card> result = new List<Card>();
            Node cashNode = Graph["Cash"];
            AddCardsToResult(result, RewardToCards["Cash"], cashNode, price, 1);
            return result;
        }

        public void PopulateAirports()
        {
            _client.DefaultRequestHeaders.Clear();
            string address = "https://raw.githubusercontent.com/ecalder6/Surf/master/data/airports.json";
            string response = _client.GetStringAsync(address).Result;
            var airprotsInfo = AirportInfo.FromJson(response);
            Console.WriteLine(airprotsInfo.Keys.Count);
            foreach (AirportInfo info in airprotsInfo.Values)
            {
                if (!CityToAirports.TryGetValue(info.City, out List<AirportInfo> airportList))
                {
                    airportList = new List<AirportInfo>();
                    CityToAirports.Add(info.City, airportList);
                }
                airportList.Add(info);
                AirportCodes.Add(info.Iata);
            }
        }

        public void PopulateCards()
        {
            Graph.Add("Cash", new Node("Cash", "Cash"));

            _client.DefaultRequestHeaders.Clear();
            string address = "https://raw.githubusercontent.com/ecalder6/Surf/master/data/cards.tsv";
            string response = _client.GetStringAsync(address).Result;
            foreach (string line in response.Split('\n'))
            {
                string[] tokens = line.Split('\t');
                Card card = new Card(
                    issuer: tokens[0].Trim(),
                    name: tokens[1].Trim(),
                    rewardProgram: tokens[2].Trim(),
                    isPersonal: string.Equals(tokens[3].Trim(), "P"),
                    daysForMinSpend: tokens[4].Trim().Length == 0 ? 0 : int.Parse(new string(tokens[4].TakeWhile(char.IsDigit).ToArray())),
                    annualFee: tokens[5].Trim().Length == 0 ? 0 : float.Parse(new string(tokens[5].TakeWhile(char.IsDigit).ToArray())),
                    isCurrentBestOffer: string.Equals(tokens[6], "Y"),
                    bonus: float.Parse(new string(tokens[7].TakeWhile(char.IsDigit).ToArray())),
                    minSpend: tokens[9].Trim().Length == 0 || tokens[9].Trim().Equals("N/A") || tokens[9].Trim().Equals("Unknown") || tokens[9].Trim().Equals("?") ? 0 : float.Parse(new string(tokens[9].TakeWhile(char.IsDigit).ToArray())),
                    isFirstFeeWaived: tokens[10].Trim().Equals("Y") ? true : false,
                    recommendationWeight: 0);

                if (!RewardToCards.TryGetValue(card.RewardProgram, out SortedList<float, List<Card>> PointsToCards))
                {
                    PointsToCards = new SortedList<float, List<Card>>();
                    RewardToCards.Add(card.RewardProgram, PointsToCards);
                }
                if (!PointsToCards.TryGetValue(card.Bonus, out List<Card> cards))
                {
                    cards = new List<Card>();
                    PointsToCards.Add(card.Bonus, cards);
                }
                if (!Graph.TryGetValue(card.RewardProgram, out _))
                {
                    if (!AirlineToCode.TryGetValue(card.RewardProgram, out string code) || !Graph.TryGetValue(code, out _))
                    {
                        Graph.Add(card.RewardProgram, new Node(card.RewardProgram, card.RewardProgram));
                    }

                }
                cards.Add(card);
            }
        }

        public void PopulateAirlines()
        {
            _client.DefaultRequestHeaders.Clear();
            string address = "https://raw.githubusercontent.com/ecalder6/Surf/master/data/codes.csv";
            string response = _client.GetStringAsync(address).Result;
            foreach (string line in response.Split('\n'))
            {
                string[] tokens = line.Split(',');
                AirlineToCode.Add(tokens[1], tokens[0]);
                CodeToAirline.Add(tokens[0], tokens[1]);
                Graph.Add(tokens[0], new Node(tokens[0], tokens[1]));
            }

            address = "https://raw.githubusercontent.com/ecalder6/Surf/master/data/transfer.tsv";
            response = _client.GetStringAsync(address).Result;
            Graph.Add("DC", new Node("DC", "Diner's Club"));
            Graph.Add("SPG", new Node("SPG", "Starwood Preferred Guest"));
            Graph.Add("MR", new Node("MR", "American Express Membership Rewards"));
            Graph.Add("UR", new Node("UR", "Chase Ultimate Rewards"));
            Graph.Add("TY", new Node("TY", "Citi ThankYou"));

            foreach (string line in response.Split('\n'))
            {
                string[] tokens = line.Split('\t');
                string airline = tokens[0];
                float dc = ConvertToRatio(tokens[1]);
                float spg = ConvertToRatio(tokens[2]);
                float mr = ConvertToRatio(tokens[3]);
                float ur = ConvertToRatio(tokens[4]);
                float ty = ConvertToRatio(tokens[5]);
                if (!AirlineToCode.TryGetValue(airline, out string code))
                {
                    Console.WriteLine(airline + " DOES NOT HAVE CODE!");
                    return;
                }
                AddEdge(dc, "DC", code);
                AddEdge(spg, "SPG", code);
                AddEdge(mr, "MR", code);
                AddEdge(ur, "UR", code);
                AddEdge(ty, "TY", code);
            }
        }

        public void AddAirports(List<string> airports, Location place, string input)
        {
            if (AirportCodes.Contains(input) || !CityToAirports.TryGetValue(place.PlaceName, out List<AirportInfo> fromAirportInfo))
            {
                airports.Add(input);
            }
            else
            {
                airports.AddRange(fromAirportInfo.Select(a => a.Iata));
            }
        }

        private void AddEdge(float ratio, string reward, string code)
        {
            if (ratio == 0)
            {
                return;
            }
            Node rewardNode = Graph[reward];
            Node airlineNode = Graph[code];
            airlineNode.FromNodes.Add(reward, 1 / ratio);
            rewardNode.ToNodes.Add(code, ratio);
        }

        private float ConvertToRatio(string token)
        {
            if (token == null || token.Length == 0)
            {
                return 0;
            }
            if (token.Equals("Y"))
            {
                return 1;
            }

            try
            {
                return float.Parse(token);
            }
            catch
            {
                Console.WriteLine(token);
                return 0;
            }
        }

        private string GenerateAwardURL(List<string> airports, char property)
        {
            string address = string.Empty;
            bool first = property == 'f';
            foreach (string airport in airports)
            {
                if (first)
                {
                    address += string.Format("?{0}={1}", property, airport);
                    first = false;
                }
                else
                {
                    address += string.Format("&{0}={1}", property, airport);
                }
            }
            return address;
        }
    }
}