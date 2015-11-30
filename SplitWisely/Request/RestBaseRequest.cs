using RestSharp.Portable.Authenticators;
using RestSharp.Portable.HttpClient;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Request
{
    abstract class RestBaseRequest
    {
        protected RestClient client;

        public RestBaseRequest()
        {
            client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(Constants.consumerKey, Constants.consumerSecret, App.accessToken, App.accessTokenSecret);
        }
    }
}
