using AsyncOAuth;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http.Headers;

namespace SplitBook.Request
{
    public abstract class RestBaseRequest
    {
        protected HttpClient client;

        public RestBaseRequest()
        {
            client = OAuthUtility.CreateOAuthClient(Constants.consumerKey, Constants.consumerSecret, new AccessToken(App.accessToken, App.accessTokenSecret));
            client.BaseAddress = new Uri(Constants.SPLITWISE_API_URL);
        }
    }
}
