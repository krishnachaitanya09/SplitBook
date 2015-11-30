using Newtonsoft.Json;
using RestSharp;
using RestSharp.Portable;
using SplitWisely.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Request
{
    class GetUserRequest : RestBaseRequest
    {
        public static String getUserURL = "get_user/{id}";

        public GetUserRequest()
            : base()
        {
        }

        public async void getCurrentUser(int userId)
        {
            var request = new RestRequest(getUserURL);
            request.AddUrlSegment("id", userId.ToString());
            IRestResponse response = await client.Execute(request);
            Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
            Newtonsoft.Json.Linq.JToken testToken = root["user"];
            JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            User currentUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(testToken.ToString(), settings);
        }
    }
}
