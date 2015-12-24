using Newtonsoft.Json;
using RestSharp;
using RestSharp.Portable;
using SplitWisely.Model;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Request
{
    class GetExpensesRequest : RestBaseRequest
    {
        public static String getExpensesURL = "get_expenses";

        public GetExpensesRequest()
            : base()
        {
        }

        public async void getAllExpenses(Action<List<Expense>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(getExpensesURL);
            request.AddParameter("limit", 0, ParameterType.GetOrPost);
            //request.AddParameter("updated_after", Helpers.getLastUpdatedTime(), ParameterType.GetOrPost);

            try
            {
                IRestResponse response = await client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    CallbackOnFailure(response.StatusCode);
                    return;
                }
                var x = Encoding.UTF8.GetString(response.RawBytes);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Expense> expenses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Expense>>(testToken.ToString(), settings);
                CallbackOnSuccess(expenses);
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
