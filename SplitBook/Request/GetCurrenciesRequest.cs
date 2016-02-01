using Newtonsoft.Json;
using RestSharp;
using RestSharp.Portable;
using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Request
{
    class GetCurrenciesRequest : RestBaseRequest
    {
        public static String getCurrenciesURL = "get_currencies";

        public GetCurrenciesRequest()
            : base()
        {
        }

        public async void getSupportedCurrencies(Action<List<Currency>> CallbackOnSuccess)
        {
            var request = new RestRequest(getCurrenciesURL);
            IRestResponse response = await client.Execute(request);
            try
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
                Newtonsoft.Json.Linq.JToken testToken = root["currencies"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Currency> supportedCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Currency>>(testToken.ToString(), settings);
                CallbackOnSuccess(supportedCurrencies);
            }
            catch (Exception e)
            {
                CallbackOnSuccess(null);
            }
        }
    }
}
