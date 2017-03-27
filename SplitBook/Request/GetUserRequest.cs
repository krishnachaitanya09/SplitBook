using Newtonsoft.Json;

using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Request
{
    class GetUserRequest : RestBaseRequest
    {
        public static String getUserURL = "get_user/";

        public GetUserRequest()
            : base()
        {
        }

        public async Task GetCurrentUser(int userId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(getUserURL + userId.ToString());
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["user"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                User currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
            }
            catch (Exception)
            {

            }
        }
    }
}
