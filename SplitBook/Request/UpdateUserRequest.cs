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
    class UpdateUserRequest : RestBaseRequest
    {
        public static String updateUserURL = "update_user/";
        User updatedUser;

        public UpdateUserRequest(User user)
            : base()
        {
            this.updatedUser = user;
        }

        public async Task UpdateUser(Action<User, HttpStatusCode> CallbackOnSuccess)
        {
            List<KeyValuePair<string, string>> postContent = new List<KeyValuePair<string, string>>();

            postContent.Add(new KeyValuePair<string, string>("first_name", updatedUser.first_name));
            postContent.Add(new KeyValuePair<string, string>("last_name", updatedUser.last_name));
            postContent.Add(new KeyValuePair<string, string>("email", updatedUser.email));

            if (!String.IsNullOrEmpty(updatedUser.default_currency))
                postContent.Add(new KeyValuePair<string, string>("default_currency", updatedUser.default_currency));

            HttpContent httpContent = new FormUrlEncodedContent(postContent);
            try
            {
                HttpResponseMessage response = await client.PostAsync(updateUserURL + updatedUser.id.ToString(), httpContent);
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    CallbackOnSuccess(null, response.StatusCode);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["user"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                User currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
                CallbackOnSuccess(currentUser, HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                CallbackOnSuccess(null, HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
