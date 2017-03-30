using Newtonsoft.Json;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Request
{
    class GetExpensesRequest : RestBaseRequest
    {
        public static String getExpensesURL = "get_expenses";

        public GetExpensesRequest()
            : base()
        {
        }

        public async Task GetAllExpenses(Action<List<Expense>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            //request.AddParameter("updated_after", Helpers.getLastUpdatedTime(), ParameterType.GetOrPost);
            try
            {
                string lastUpdated = Helpers.LastUpdatedTime;
                string url = getExpensesURL + "?limit=0";
                if (lastUpdated != null)
                    url += "&updated_after=" + lastUpdated;
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    CallbackOnFailure(response.StatusCode);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Expense> expenses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Expense>>(testToken.ToString(), settings);
                CallbackOnSuccess(expenses);
            }
            catch (Exception)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
