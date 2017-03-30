using Newtonsoft.Json;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SplitBook.Request
{
    public class GetNotificationsRequest : RestBaseRequest
    {
        public static String getNotificationsURL = "get_notifications";

        public GetNotificationsRequest()
            : base()
        {
        }

        public async Task GetNotifications(Action<List<Notifications>> CallbackOnSuccess)
        {
            try
            {
                string lastUpdated = Helpers.LastUpdatedTime;
                string url = getNotificationsURL + "?limit=0";
                if (lastUpdated != null)
                    url += "&updated_after=" + lastUpdated;

                HttpResponseMessage response = await client.GetAsync(url);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["notifications"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Notifications> notifications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Notifications>>(testToken.ToString(), settings);
                CallbackOnSuccess(notifications);
            }
            catch (Exception)
            {
                CallbackOnSuccess(null);
            }
        }
    }
}
