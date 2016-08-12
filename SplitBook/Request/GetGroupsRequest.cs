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
    class GetGroupsRequest : RestBaseRequest
    {
        public static String getGroupsURL = "get_groups";

        public GetGroupsRequest()
            : base()
        {
        }

        public async void getAllGroups(Action<List<Group>> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(getGroupsURL);

                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["groups"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Group> groups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Group>>(testToken.ToString(), settings);
                CallbackOnSuccess(groups);
            }
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
