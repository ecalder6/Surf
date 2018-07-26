namespace AwardInfoJson
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class AwardInfo
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("from")]
        public string[] From { get; set; }

        [JsonProperty("params")]
        public string Params { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }

        [JsonProperty("to")]
        public string[] To { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("may_need_more")]
        public bool MayNeedMore { get; set; }

        [JsonProperty("awards")]
        public Award[] Awards { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("miles")]
        public long Miles { get; set; }

        [JsonProperty("from")]
        public string[] From { get; set; }

        [JsonProperty("one_way")]
        public long OneWay { get; set; }

        [JsonProperty("transfer")]
        public Transfer Transfer { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("stops")]
        public long[] Stops { get; set; }

        [JsonProperty("to")]
        public string[] To { get; set; }

        [JsonProperty("operate_airlines")]
        public OperateAirline[] OperateAirlines { get; set; }

        [JsonProperty("all_levels")]
        public AllLevel[] AllLevels { get; set; }

        [JsonProperty("airline")]
        public ResultAirline Airline { get; set; }

        [JsonProperty("cabin")]
        public string Cabin { get; set; }
    }

    public partial class ResultAirline
    {
        [JsonProperty("award_website_url")]
        public string AwardWebsiteUrl { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fixed_value")]
        public bool FixedValue { get; set; }

        [JsonProperty("award_search_website_login_required")]
        public bool AwardSearchWebsiteLoginRequired { get; set; }

        [JsonProperty("frequent_flyer_program")]
        public string FrequentFlyerProgram { get; set; }

        [JsonProperty("recommended_award_search_websites")]
        public AirlineRecommendedAwardSearchWebsite[] RecommendedAwardSearchWebsites { get; set; }

        [JsonProperty("award_phone_ticketing_fee_tips")]
        public string AwardPhoneTicketingFeeTips { get; set; }

        [JsonProperty("miles_name")]
        public string MilesName { get; set; }

        [JsonProperty("award_phone_number")]
        public string AwardPhoneNumber { get; set; }
    }

    public partial class AirlineRecommendedAwardSearchWebsite
    {
        [JsonProperty("recommended_award_search_websites")]
        public RecommendedAwardSearchWebsiteRecommendedAwardSearchWebsite[] RecommendedAwardSearchWebsites { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("airlines")]
        public AirlineElement[] Airlines { get; set; }
    }

    public partial class AirlineElement
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class RecommendedAwardSearchWebsiteRecommendedAwardSearchWebsite
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("login_required")]
        public bool LoginRequired { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("frequent_flyer_program")]
        public string FrequentFlyerProgram { get; set; }
    }

    public partial class AllLevel
    {
        [JsonProperty("info")]
        public object Info { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }
    }

    public partial class Award
    {
        [JsonProperty("routes")]
        public Routes Routes { get; set; }

        [JsonProperty("distance_type")]
        public long DistanceType { get; set; }

        [JsonProperty("operate_airlines")]
        public OperateAirline[] OperateAirlines { get; set; }

        [JsonProperty("may_need_more")]
        public bool MayNeedMore { get; set; }

        [JsonProperty("stops")]
        public long[] Stops { get; set; }

        [JsonProperty("award_chart", NullValueHandling = NullValueHandling.Ignore)]
        public AwardChart[] AwardChart { get; set; }
    }

    public partial class AwardChart
    {
        [JsonProperty("max_distance")]
        public long MaxDistance { get; set; }

        [JsonProperty("miles")]
        public long Miles { get; set; }

        [JsonProperty("min_distance")]
        public long MinDistance { get; set; }
    }

    public partial class OperateAirline
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("airlines")]
        public AirlineElement[] Airlines { get; set; }
    }

    public partial class Routes
    {
        [JsonProperty("return")]
        public Outbound Return { get; set; }

        [JsonProperty("outbound")]
        public Outbound Outbound { get; set; }
    }

    public partial class Outbound
    {
        [JsonProperty("one_stop")]
        public OneStop[] OneStop { get; set; }

        [JsonProperty("non_stop")]
        public NonStop[] NonStop { get; set; }

        [JsonProperty("two_stop")]
        public object[] TwoStop { get; set; }
    }

    public partial class NonStop
    {
        [JsonProperty("distance")]
        public long Distance { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("min_fee")]
        public long MinFee { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("flights")]
        public string[] Flights { get; set; }

        [JsonProperty("miles")]
        public long Miles { get; set; }
    }

    public partial class OneStop
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("miles")]
        public long Miles { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("distance")]
        public long Distance { get; set; }

        [JsonProperty("to_flights")]
        public string[] ToFlights { get; set; }

        [JsonProperty("from_flights")]
        public string[] FromFlights { get; set; }

        [JsonProperty("min_fee")]
        public long MinFee { get; set; }

        [JsonProperty("stop")]
        public string Stop { get; set; }
    }

    public partial class Transfer
    {
        [JsonProperty("SPG")]
        public Mr Spg { get; set; }

        [JsonProperty("UR")]
        public Mr Ur { get; set; }

        [JsonProperty("TYP")]
        public Mr Typ { get; set; }

        [JsonProperty("MR")]
        public Mr Mr { get; set; }
    }

    public partial class Mr
    {
        [JsonProperty("tips")]
        public string Tips { get; set; }

        [JsonProperty("ratio")]
        public double Ratio { get; set; }
    }

    public partial class AwardInfo
    {
        public static AwardInfo FromJson(string json) => JsonConvert.DeserializeObject<AwardInfo>(json, AwardInfoJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AwardInfo self) => JsonConvert.SerializeObject(self, AwardInfoJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                stringConverter.Singleton,
                PurpleFromConverter.Singleton,
                CabinConverter.Singleton,
                FromElementConverter.Singleton,
                ToConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class stringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (string)untypedValue;
            serializer.Serialize(writer, value);
        }

        public static readonly stringConverter Singleton = new stringConverter();
    }

    internal class PurpleFromConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (string)untypedValue;
            serializer.Serialize(writer, value);
        }

        public static readonly PurpleFromConverter Singleton = new PurpleFromConverter();
    }

    internal class CabinConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (string)untypedValue;
            serializer.Serialize(writer, value);
        }

        public static readonly CabinConverter Singleton = new CabinConverter();
    }

    internal class FromElementConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (string)untypedValue;
            serializer.Serialize(writer, value);
        }

        public static readonly FromElementConverter Singleton = new FromElementConverter();
    }

    internal class ToConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (string)untypedValue;
            serializer.Serialize(writer, value);
        }

        public static readonly ToConverter Singleton = new ToConverter();
    }
}

namespace FlightQuoteJson
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class FlightQuote
    {
        [JsonProperty("Quotes")]
        public Quote[] Quotes { get; set; }

        [JsonProperty("Places")]
        public FlightPlace[] Places { get; set; }

        [JsonProperty("Carriers")]
        public Carrier[] Carriers { get; set; }

        [JsonProperty("Currencies")]
        public Currency[] Currencies { get; set; }
    }

    public partial class Carrier
    {
        [JsonProperty("CarrierId")]
        public long CarrierId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }

    public partial class Currency
    {
        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ThousandsSeparator")]
        public string ThousandsSeparator { get; set; }

        [JsonProperty("DecimalSeparator")]
        public string DecimalSeparator { get; set; }

        [JsonProperty("SymbolOnLeft")]
        public bool SymbolOnLeft { get; set; }

        [JsonProperty("SpaceBetweenAmountAndSymbol")]
        public bool SpaceBetweenAmountAndSymbol { get; set; }

        [JsonProperty("RoundingCoefficient")]
        public long RoundingCoefficient { get; set; }

        [JsonProperty("DecimalDigits")]
        public long DecimalDigits { get; set; }
    }

    public partial class FlightPlace
    {
        [JsonProperty("PlaceId")]
        public long PlaceId { get; set; }

        [JsonProperty("IataCode")]
        public string IataCode { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("SkyscannerCode")]
        public string SkyscannerCode { get; set; }

        [JsonProperty("CityName")]
        public string CityName { get; set; }

        [JsonProperty("CityId")]
        public string CityId { get; set; }

        [JsonProperty("CountryName")]
        public string CountryName { get; set; }
    }

    public partial class Quote
    {
        [JsonProperty("QuoteId")]
        public long QuoteId { get; set; }

        [JsonProperty("MinPrice")]
        public long MinPrice { get; set; }

        [JsonProperty("Direct")]
        public bool Direct { get; set; }

        [JsonProperty("OutboundLeg")]
        public BoundLeg OutboundLeg { get; set; }

        [JsonProperty("QuoteDateTime")]
        public DateTimeOffset QuoteDateTime { get; set; }

        [JsonProperty("InboundLeg", NullValueHandling = NullValueHandling.Ignore)]
        public BoundLeg InboundLeg { get; set; }
    }

    public partial class BoundLeg
    {
        [JsonProperty("CarrierIds")]
        public long[] CarrierIds { get; set; }

        [JsonProperty("OriginId")]
        public long OriginId { get; set; }

        [JsonProperty("DestinationId")]
        public long DestinationId { get; set; }

        [JsonProperty("DepartureDate")]
        public DateTimeOffset DepartureDate { get; set; }
    }

    public partial class FlightQuote
    {
        public static FlightQuote FromJson(string json) => JsonConvert.DeserializeObject<FlightQuote>(json, FlightQuoteJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this FlightQuote self) => JsonConvert.SerializeObject(self, FlightQuoteJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

namespace PlaceInfoJson
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PlaceInfo
    {
        [JsonProperty("Places")]
        public Location[] Places { get; set; }
    }

    [Serializable]
    public partial class Location
    {
        [JsonProperty("PlaceId")]
        public string PlaceId { get; set; }

        [JsonProperty("PlaceName")]
        public string PlaceName { get; set; }

        [JsonProperty("CountryId")]
        public string CountryId { get; set; }

        [JsonProperty("RegionId")]
        public string RegionId { get; set; }

        [JsonProperty("CityId")]
        public string CityId { get; set; }

        [JsonProperty("CountryName")]
        public string CountryName { get; set; }
    }

    public partial class PlaceInfo
    {
        public static PlaceInfo FromJson(string json) => JsonConvert.DeserializeObject<PlaceInfo>(json, PlaceInfoJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this PlaceInfo self) => JsonConvert.SerializeObject(self, PlaceInfoJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                RegionIdConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class RegionIdConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(string);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (string)untypedValue;
            serializer.Serialize(writer, value);
        }

        public static readonly RegionIdConverter Singleton = new RegionIdConverter();
    }
}

namespace AirportInfoJson
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class AirportInfo
    {
        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("iata")]
        public string Iata { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("elevation")]
        public long Elevation { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("tz")]
        public string Tz { get; set; }
    }

    public partial class AirportInfo
    {
        public static Dictionary<string, AirportInfo> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, AirportInfo>>(json, AirportInfoJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Dictionary<string, AirportInfo> self) => JsonConvert.SerializeObject(self, AirportInfoJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}