using AsyncOAuth;
using BackgroundTasks.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BackgroundTasks.Request
{
    public sealed class GetNotifications
    {
        private static String getNotificationsURL = "get_notifications?limit=5";

        public GetNotifications()
            : base()
        {
        }

        public IAsyncOperation<IList<Notifications>> getNotifications()
        {
            return getNotificationsAsync().AsAsyncOperation();
        }

        private async Task<IList<Notifications>> getNotificationsAsync()
        {
            try
            {
                OAuthUtility.ComputeHash = (key, buffer) =>
                {
                    var crypt = Windows.Security.Cryptography.Core.MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
                    var keyBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(key);
                    var cryptKey = crypt.CreateKey(keyBuffer);

                    var dataBuffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(buffer);
                    var signBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(cryptKey, dataBuffer);

                    byte[] value;
                    Windows.Security.Cryptography.CryptographicBuffer.CopyToByteArray(signBuffer, out value);
                    return value;
                };

                HttpClient client = OAuthUtility.CreateOAuthClient("etGuasDJQxqFTfpVFsWdeunzraxAtfi3cCNxwcOL", "S3cIJuC7IC2FaNj6EjGThZKREt0zXnTcVODUmBLJ", new AccessToken(Helpers.AccessToken, Helpers.AccessTokenSecret));
                client.BaseAddress = new Uri("https://secure.splitwise.com/api/v3.0/");
                getNotificationsURL = getNotificationsURL + "&updated_after=" + Helpers.NotificationsLastUpdated ?? DateTime.UtcNow.ToString("u");
                HttpResponseMessage response = await client.GetAsync(getNotificationsURL);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["notifications"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Notifications> notifications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Notifications>>(testToken.ToString(), settings);
                Helpers.NotificationsLastUpdated = DateTime.UtcNow.ToString("u");
                return notifications;
            }
            catch (Exception)
            {
                return new List<Notifications>();
            }
        }
    }
}
