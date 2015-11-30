using Newtonsoft.Json;
using RestSharp;
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
    class UpdateUserRequest : RestBaseRequest
    {
        public static String updateUserURL = "update_user/{id}";
        User updatedUser;

        public UpdateUserRequest(User user)
            : base()
        {
            this.updatedUser = user;
        }

        public async void updateUser(Action<User, HttpStatusCode> CallbackOnSuccess)
        {
            var request = new RestRequest(updateUserURL, Method.POST);
            request.AddUrlSegment("id", updatedUser.id.ToString());

            request.AddParameter("first_name", updatedUser.first_name, ParameterType.GetOrPost);
            request.AddParameter("last_name", updatedUser.last_name, ParameterType.GetOrPost);
            request.AddParameter("email", updatedUser.email, ParameterType.GetOrPost);

            if (!String.IsNullOrEmpty(updatedUser.default_currency))
                request.AddParameter("default_currency", updatedUser.default_currency, ParameterType.GetOrPost);

            IRestResponse response = await client.Execute(request);
            try
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NotModified)
                {
                    CallbackOnSuccess(null, response.StatusCode);
                    return;
                }
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
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
