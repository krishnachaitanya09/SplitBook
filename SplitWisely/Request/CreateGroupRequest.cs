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
    class CreateGroupRequest : RestBaseRequest
    {
        public static String createGroupURL = "create_group";
        Group groupToAdd;

        public CreateGroupRequest(Group group)
            : base()
        {
            this.groupToAdd = group;
        }

        public async void createGroup(Action<Group> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            var request = new RestRequest(createGroupURL, Method.POST);

            request.AddParameter("name", groupToAdd.name, ParameterType.GetOrPost);

            int count = 0;
            foreach (var user in groupToAdd.members)
            {
                string idKey = String.Format("users__{0}__user_id", count);
                request.AddParameter(idKey, user.id, ParameterType.GetOrPost);

                count++;
            }


            IRestResponse response = await client.Execute(request);
            try
            {
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(Encoding.UTF8.GetString(response.RawBytes));
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
