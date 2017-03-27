using AsyncOAuth;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static String requestTokenURL = "get_request_token";
        public static String accessTokenURL = "get_access_token";
        private TokenResponse<RequestToken> tokenResponse;
        private OAuthAuthorizer authorizer;

        public OAuthRequest()
        {
            authorizer = new OAuthAuthorizer(Constants.consumerKey, Constants.consumerSecret);
        }

        public async Task GetRequestToken(Action<Uri> CallbackOnSuccess, Action<Exception> CallbackOnFailure)
        {
            try
            {
                tokenResponse = await authorizer.GetRequestToken(Constants.SPLITWISE_API_URL + requestTokenURL);
                String authorizeUrl = authorizer.BuildAuthorizeUrl(Constants.SPLITWISE_AUTHORIZE_URL, tokenResponse.Token);
                CallbackOnSuccess(new Uri(authorizeUrl));
            }
            catch (Exception ex)
            {
                CallbackOnFailure(ex);
            }
        }

        public async Task GetAccessToken(string uri, Action<String, String> CallbackOnSuccess, Action<Exception> CallbackOnFailure)
        {
            try
            {
                string oauth_veririfer = Helpers.GetQueryParameter(uri, "oauth_verifier");
                TokenResponse<AccessToken> accessTokenResponse = await authorizer.GetAccessToken(Constants.SPLITWISE_API_URL + accessTokenURL, tokenResponse.Token, oauth_veririfer);
                CallbackOnSuccess(accessTokenResponse.Token.Key, accessTokenResponse.Token.Secret);
            }
            catch (Exception ex)
            {
                CallbackOnFailure(ex);
            }
        }
    }
}