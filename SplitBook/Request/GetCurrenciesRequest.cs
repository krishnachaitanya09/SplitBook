using Newtonsoft.Json;
using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public async Task GetSupportedCurrencies(Action<List<Currency>> CallbackOnSuccess)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(getCurrenciesURL);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["currencies"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Currency> supportedCurrencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Currency>>(testToken.ToString(), settings);
                CallbackOnSuccess(supportedCurrencies);
            }
            catch (Exception)
            {
                CallbackOnSuccess(null);
            }
        }
    }
}
