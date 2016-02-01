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
    class CurrentUserRequest : RestBaseRequest
    {
        public static String currentUserURL = "get_current_user";

        public CurrentUserRequest()
            : base()
        {
        }

        public async void getCurrentUser(Action<User> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(currentUserURL);
            IRestResponse response = await client.Execute(request);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    CallbackOnFailure(response.StatusCode);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
                Newtonsoft.Json.Linq.JToken testToken = root["user"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                User currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
                CallbackOnSuccess(currentUser);
            }

            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
