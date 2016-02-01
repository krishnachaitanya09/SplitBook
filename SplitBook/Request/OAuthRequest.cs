using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.HttpClient;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace SplitBook.Request
{
    class OAuthRequest
    {
        public static String reuqestTokenURL = "get_request_token";
        public static String accessTokenURL = "get_access_token";
        public static string _oAuthToken, _oAuthTokenSecret;

        public async void GetRequestToken(Action<Uri> CallbackOnSuccess, Action<Exception> CallbackOnFailure)
        {
            try
            {
                var client = new RestClient(Constants.SPLITWISE_API_URL);
                client.Authenticator = OAuth1Authenticator.ForRequestToken(Constants.consumerKey, Constants.consumerSecret, Constants.OAUTH_CALLBACK);
                var request = new RestRequest(OAuthRequest.reuqestTokenURL, Method.POST);
                IRestResponse response = await client.Execute(request);
                if (response.IsSuccess)
                {
                    _oAuthToken = Helpers.GetQueryParameter(Encoding.UTF8.GetString(response.RawBytes), "oauth_token");
                    _oAuthTokenSecret = Helpers.GetQueryParameter(Encoding.UTF8.GetString(response.RawBytes), "oauth_token_secret");
                    String authorizeUrl = Constants.SPLITWISE_AUTHORIZE_URL + "?oauth_token=" + _oAuthToken;
                    CallbackOnSuccess(new Uri(authorizeUrl));
                }
            }
            catch (Exception ex)
            {
                CallbackOnFailure(ex);
            }
        }

        public async void GetAccessToken(string uri, Action<String, String> CallbackOnSuccess, Action<Exception> CallbackOnFailure)
        {
            try
            {
                string oauth_veririfer = Helpers.GetQueryParameter(uri, "oauth_verifier");
                var client = new RestClient(Constants.SPLITWISE_API_URL);
                client.Authenticator = OAuth1Authenticator.ForAccessToken(Constants.consumerKey, Constants.consumerSecret, _oAuthToken, _oAuthTokenSecret, oauth_veririfer);
                var request = new RestRequest(OAuthRequest.accessTokenURL, Method.POST);
                IRestResponse response = await client.Execute(request);
                if (response.IsSuccess)
                {
                    string accessToken = Helpers.GetQueryParameter(Encoding.UTF8.GetString(response.RawBytes), "oauth_token");
                    string accessTokenSecret = Helpers.GetQueryParameter(Encoding.UTF8.GetString(response.RawBytes), "oauth_token_secret");
                    CallbackOnSuccess(accessToken, accessTokenSecret);
                }
            }
            catch (Exception ex)
            {
                CallbackOnFailure(ex);
            }
        }
    }
}

