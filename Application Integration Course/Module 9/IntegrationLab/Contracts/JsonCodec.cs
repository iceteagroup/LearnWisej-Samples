using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// The wire format of the contract: camelCase JSON, case-insensitive on the way in.
    /// Used by the postback handler (both directions) and to normalize dictionaries that
    /// arrive either from System.Text.Json (JsonElement values) or from a Wisej widget
    /// event (Wisej.Core.DynamicObject values) into plain CLR primitives.
    /// </summary>
    public static class JsonCodec
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public static string Serialize(object value) => JsonSerializer.Serialize(value, Options);

        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new DataContractException(400, "The request body is empty; a JSON object was expected.");
            try
            {
                var result = JsonSerializer.Deserialize<T>(json, Options);
                if (result == null)
                    throw new DataContractException(400, "The request body is not a JSON object.");
                return result;
            }
            catch (JsonException ex)
            {
                throw new DataContractException(400, "Malformed JSON body: " + ex.Message);
            }
        }

        /// <summary>Replaces JsonElement / DynamicObject values with string, double, bool or null.</summary>
        public static Dictionary<string, object> Normalize(IDictionary<string, object> source)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (source == null) return result;
            foreach (var pair in source)
                result[pair.Key] = NormalizeValue(pair.Value);
            return result;
        }

        /// <summary>Turns a Wisej.Core.DynamicObject (widget event payload) into a plain dictionary.</summary>
        public static Dictionary<string, object> ToDictionary(object value)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (value is Wisej.Core.DynamicObject dyn)
            {
                foreach (var member in dyn)
                    result[member.Name] = NormalizeValue(member.Value);
            }
            else if (value is IDictionary<string, object> dict)
            {
                foreach (var pair in dict)
                    result[pair.Key] = NormalizeValue(pair.Value);
            }
            return result;
        }

        public static object NormalizeValue(object value)
        {
            switch (value)
            {
                case null:
                    return null;
                case JsonElement el:
                    switch (el.ValueKind)
                    {
                        case JsonValueKind.String: return el.GetString();
                        case JsonValueKind.Number: return el.GetDouble();
                        case JsonValueKind.True: return true;
                        case JsonValueKind.False: return false;
                        case JsonValueKind.Null: return null;
                        default: return el.GetRawText();
                    }
                case Wisej.Core.DynamicObject dyn:
                    return ToDictionary(dyn);
                case string s:
                    return s;
                case bool b:
                    return b;
                case int or long or short or byte or float or double or decimal:
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                default:
                    return value.ToString();
            }
        }

        /// <summary>Compact {"a":1,"b":"x"} rendering of a dictionary for the trace.</summary>
        public static string DictionaryToTrace(IReadOnlyDictionary<string, object> dict)
        {
            if (dict == null || dict.Count == 0) return "{}";
            var parts = new List<string>();
            foreach (var pair in dict)
            {
                string v = pair.Value switch
                {
                    null => "null",
                    string s => "\"" + s + "\"",
                    bool b => b ? "true" : "false",
                    double d => d.ToString(CultureInfo.InvariantCulture),
                    _ => pair.Value.ToString()
                };
                parts.Add(pair.Key + ":" + v);
            }
            return "{" + string.Join(",", parts) + "}";
        }
    }
}
