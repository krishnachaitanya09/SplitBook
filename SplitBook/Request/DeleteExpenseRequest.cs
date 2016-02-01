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
    class DeleteExpenseRequest : RestBaseRequest
    {
        public static String deleteExpenseURL = "delete_expense/{id}";
        private int expenseId;

        public DeleteExpenseRequest(int id)
            : base()
        {
            expenseId = id;
        }

        public async void deleteExpense(Action<bool> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(deleteExpenseURL, Method.POST);
            request.AddUrlSegment("id", expenseId.ToString());
            IRestResponse response = await client.Execute(request);
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                DeleteExpense result = Newtonsoft.Json.JsonConvert.DeserializeObject<DeleteExpense>(Encoding.UTF8.GetString(response.RawBytes), settings);
                if (result.success)
                    CallbackOnSuccess(result.success);
                else
                    CallbackOnFailure(response.StatusCode);
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }

        private class DeleteExpense
        {
            public Boolean success { get; set; }
            public Object error { get; set; }
        }
    }
}
