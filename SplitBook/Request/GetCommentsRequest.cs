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
    class GetCommentsRequest : RestBaseRequest
    {
        public static String getCommentsURL = "get_comments";
        private int expenseId;

        public GetCommentsRequest(int expenseId)
            : base()
        {
            this.expenseId = expenseId;
        }

        public async void getComments(Action<List<Comment>> Callback)
        {
            var request = new RestRequest(getCommentsURL);
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            IRestResponse response = await client.Execute(request);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    Callback(null);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
                Newtonsoft.Json.Linq.JToken testToken = root["comments"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Comment> comments = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Comment>>(testToken.ToString(), settings);
                Callback(comments);
            }
            catch (Exception e)
            {
                Callback(null);
            }
        }
    }
}
