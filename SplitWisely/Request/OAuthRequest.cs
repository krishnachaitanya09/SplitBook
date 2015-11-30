using RestSharp.Portable;
using RestSharp.Portable.Authenticators;
using RestSharp.Portable.HttpClient;
using SplitWisely.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SplitWisely.Request
{
    class OAuthRequest
    {
        public static String reuqestTokenURL = "get_request_token";
        public static String accessTokenURL = "get_access_token";
        private string _oAuthToken, _oAuthTokenSecret, _oAuthVerifier;

        public async Task<bool> GetRequestToken()
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
                await WebAuthenticate();
                return true;
            }
            return false;
        }

        public async Task GetAccessToken()
        {
            var client = new RestClient(Constants.SPLITWISE_API_URL);
            client.Authenticator = OAuth1Authenticator.ForAccessToken(Constants.consumerKey, Constants.consumerSecret, _oAuthToken, _oAuthTokenSecret, _oAuthVerifier);
            var request = new RestRequest(OAuthRequest.accessTokenURL, Method.POST);
            IRestResponse response = await client.Execute(request);
            if (response.IsSuccess)
            {
                string accessToken = Helpers.GetQueryParameter(Encoding.UTF8.GetString(response.RawBytes), "oauth_token");
                string accessTokenSecret = Helpers.GetQueryParameter(Encoding.UTF8.GetString(response.RawBytes), "oauth_token_secret");
                Helpers.AccessToken = accessToken;
                Helpers.AccessTokenSecret = accessTokenSecret;
                App.accessToken = Helpers.AccessToken;
                App.accessTokenSecret = Helpers.AccessTokenSecret;
            }
        }

        public async Task WebAuthenticate()
        {
            WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                             WebAuthenticationOptions.None, new Uri(Constants.SPLITWISE_AUTHORIZE_URL + "?oauth_token=" + _oAuthToken),
                                             new Uri(Constants.OAUTH_CALLBACK));
            if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                _oAuthVerifier = Helpers.GetQueryParameter(WebAuthenticationResult.ResponseData, "oauth_verifier");
                await GetAccessToken();
                //OutputToken(WebAuthenticationResult.ResponseData.ToString());
            }
            else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                //OutputToken("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
            }
            else
            {
                //OutputToken("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
            }
        }
    }
}

