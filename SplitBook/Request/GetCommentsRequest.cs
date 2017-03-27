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
    class GetCommentsRequest : RestBaseRequest
    {
        public static String getCommentsURL = "get_comments";
        private int expenseId;

        public GetCommentsRequest(int expenseId)
            : base()
        {
            this.expenseId = expenseId;
        }

        public async Task GetComments(Action<List<Comment>> Callback)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(getCommentsURL + "?expense_id=" + expenseId);
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    Callback(null);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
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
