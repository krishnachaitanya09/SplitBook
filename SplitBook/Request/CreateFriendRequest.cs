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
    class CreateFriendRequest : RestBaseRequest
    {
        public static String createFriendURL = "create_friend";
        string email, firstName, lastName;

        public CreateFriendRequest(string email, string firstName, string lastName)
            : base()
        {
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public async void createFriend(Action<User> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            List<KeyValuePair<string, string>> postContent = new List<KeyValuePair<string, string>>();
            postContent.Add(new KeyValuePair<string, string>("user_email", email));
            postContent.Add(new KeyValuePair<string, string>("user_first_name", firstName));

            if (!String.IsNullOrEmpty(lastName))
            {
                postContent.Add(new KeyValuePair<string, string>("user_last_name", lastName));
            }
            HttpContent httpContent = new FormUrlEncodedContent(postContent);
            try
            {
                HttpResponseMessage response = await client.PostAsync(createFriendURL, httpContent);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["friend"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
                if (user != null)
                {
                    if (user.id != 0)                        
                        CallbackOnSuccess(user);
                    else
                        CallbackOnFailure(response.StatusCode);
                }
                else
                    CallbackOnFailure(response.StatusCode);
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
