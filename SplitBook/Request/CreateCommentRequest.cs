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

        public async Task PostComment(Action<List<Comment>> Callback)
        {
            List<KeyValuePair<string, string>> postContent = new List<KeyValuePair<string, string>>();
            postContent.Add(new KeyValuePair<string, string>("expense_id", Convert.ToString(expenseId, System.Globalization.CultureInfo.InvariantCulture)));
            postContent.Add(new KeyValuePair<string, string>("content", content));
            HttpContent httpContent = new FormUrlEncodedContent(postContent);
            try
            {
                HttpResponseMessage response = await client.PostAsync(createCommentURL, httpContent);
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    Callback(null);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
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
