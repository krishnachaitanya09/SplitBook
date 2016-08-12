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
    class GetFriendsRequest : RestBaseRequest
    {
        public static String getFriendsURL = "get_friends";

        public GetFriendsRequest()
            : base()
        {
        }

        public async void getAllFriends(Action<List<User>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(getFriendsURL);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["friends"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<User> friends = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(testToken.ToString(), settings);
                CallbackOnSuccess(friends);
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
