using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using mPower.Framework;
using Newtonsoft.Json;

namespace mPower.Documents.ExternalServices.Janrain
{
    public class JanrainService
    {
        private readonly string _baseApiUrl;

        private string _apiKey;

        public JanrainService(MPowerSettings settings)
        {
            _baseApiUrl = settings.JanrainApiBaseUrl;
        }

        public void Initialize(string applicationApiKey)
        {
            _apiKey = applicationApiKey;
        }

        public IJanrainResponse GetAuthInfo(string token)
        {
            if (_apiKey == null)
                throw new ArgumentException("Initialize service application api key before use it");

            var parameters = new NameValueCollection { { "token", token }, { "apiKey", _apiKey }, { "format", "json" } };

            var queryString = ToQueryString(parameters);

            var request = (HttpWebRequest)WebRequest.Create(_baseApiUrl + "auth_info");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = Encoding.ASCII.GetBytes(queryString);
            request.ContentLength = bytes.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            string responseText;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var answerReader = new StreamReader(response.GetResponseStream()))
                {
                    responseText = answerReader.ReadToEnd();
                }
            }

            var janrain = JsonConvert.DeserializeObject<BaseJanrainResponse>(responseText);
            if (!janrain.stat.Equals("ok"))
            {
                throw new ArgumentException(String.Format("Invalid response from janrain: {0}", responseText));
            }

            IJanrainResponse result = null;

            switch (janrain.profile.providerName)
            {
                case "Google":
                    {
                        var response = JsonConvert.DeserializeObject<JanrainGoogleResponse>(responseText);
                        result = response;
                        result.Provider = JanrainProviderType.Google;
                        result.Identifier = response.profile.identifier;
                    }
                    break;
                case "Facebook":
                    {
                        var response = JsonConvert.DeserializeObject<JanrainFacebookResponse>(responseText);
                        result = response;
                        result.Provider = JanrainProviderType.Facebook;
                        result.Identifier = response.profile.identifier;
                    }
                    break;
                case "Twitter":
                    {
                        var response = JsonConvert.DeserializeObject<JanrainTwitterResponse>(responseText);
                        result = response;
                        result.Provider = JanrainProviderType.Twitter;
                        result.Identifier = response.profile.identifier;
                    }
                    break;
                case "Windows Live":
                    {
                        var response = JsonConvert.DeserializeObject<JanrainWindowsLiveResponse>(responseText);
                        result = response;
                        result.Provider = JanrainProviderType.WindowsLive;
                        result.Identifier = response.profile.identifier;
                    }
                    break;

            }

            return result;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            return string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key])));
        }
    }
}
