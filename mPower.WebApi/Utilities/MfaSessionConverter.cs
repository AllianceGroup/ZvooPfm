using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mPower.Aggregation.Contract.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mPower.WebApi.Utilities
{
    public class MfaSessionConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var sessionJObject = JObject.Load(reader);

            var session = new MfaSession
            {
                Token = sessionJObject["Token"].Value<string>(),
                Questions = sessionJObject["Questions"].Value<JArray>().Select(u => JTokenToAggragationQuetion(u.Value<JObject>())).ToArray()
            };

            return session;

        }

        public AggregationQuestion JTokenToAggragationQuetion(JObject questionJObject)
        {
            switch (questionJObject["Type"].Value<string>())
            {
                case "text":
                    return JsonConvert.DeserializeObject<TextBasedQuestion>(questionJObject.ToString());
                case "imageCaptcha":
                    return JsonConvert.DeserializeObject<ImageCaptchaQuestion>(questionJObject.ToString());
                case "multipleTextOptions":
                    return JsonConvert.DeserializeObject<MultipleTextOptionsQuestion>(questionJObject.ToString());
                case "imageChoice1":
                    return JsonConvert.DeserializeObject<ImageChoiceQuestionStyleOne>(questionJObject.ToString());
                case "imageChoice2":
                    return JsonConvert.DeserializeObject<ImageChoiceQuestionStyleTwo>(questionJObject.ToString());
                default:
                    return null;
            }
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(MfaSession));
        }
    }
}
