using Newtonsoft.Json;
using RestSharp.Portable;
using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Request
{
    class CreateCommentRequest : RestBaseRequest
    {
        public static String createCommentURL = "create_comment";
        private int expenseId;
        private string content;

        public CreateCommentRequest(int expenseId, string content)
            : base()
        {
            this.expenseId = expenseId;
            this.content = content;
        }

        public async void postComment(Action<List<Comment>> Callback)
        {
            var request = new RestRequest(createCommentURL);
            request.AddParameter("expense_id", expenseId, ParameterType.GetOrPost);
            request.AddParameter("content", content, ParameterType.GetOrPost);
            IRestResponse response = await client.Execute(request);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    Callback(null);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
                Newtonsoft.Json.Linq.JToken testToken = root["comment"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                Comment comment = Newtonsoft.Json.JsonConvert.DeserializeObject<Comment>(testToken.ToString(), settings);
                List<Comment> comments = new List<Comment>();
                comments.Add(comment);
                Callback(comments);
            }
            catch (Exception e)
            {
                Callback(null);
            }
        }
    }
}
