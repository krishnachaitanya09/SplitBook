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
            var request = new RestRequest(createFriendURL, Method.POST);

            request.AddParameter("user_email", email, ParameterType.GetOrPost);
            request.AddParameter("user_first_name", firstName, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(lastName))
            {
                request.AddParameter("user_last_name", lastName, ParameterType.GetOrPost);
            }

            IRestResponse response = await client.Execute(request);
            try
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
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
