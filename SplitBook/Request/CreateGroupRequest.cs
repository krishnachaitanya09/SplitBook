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
    class CreateGroupRequest : RestBaseRequest
    {
        public static String createGroupURL = "create_group";
        Group groupToAdd;

        public CreateGroupRequest(Group group)
            : base()
        {
            this.groupToAdd = group;
        }

        public async Task CreateGroup(Action<Group> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            List<KeyValuePair<string, string>> postContent = new List<KeyValuePair<string, string>>();
            postContent.Add(new KeyValuePair<string, string>("name", groupToAdd.name));

            int count = 0;
            foreach (var user in groupToAdd.members)
            {
                string idKey = String.Format("users__{0}__user_id", count);
                postContent.Add(new KeyValuePair<string, string>(idKey, Convert.ToString(user.id, System.Globalization.CultureInfo.InvariantCulture)));

                count++;
            }
            HttpContent httpContent = new FormUrlEncodedContent(postContent);
            try
            {
                HttpResponseMessage response = await client.PostAsync(createGroupURL, httpContent);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["group"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                Group group = Newtonsoft.Json.JsonConvert.DeserializeObject<Group>(testToken.ToString(), settings);
                if (group != null)
                {
                    if (group.id != 0)
                        CallbackOnSuccess(group);
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
