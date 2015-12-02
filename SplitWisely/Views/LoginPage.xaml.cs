using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RestSharp.Portable.Authenticators;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System.Collections.Specialized;
using SplitWisely.Request;
using SplitWisely.Utilities;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SplitWisely.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private OAuthRequest authorize;
        public LoginPage()
        {
            this.InitializeComponent();
            authorize = new OAuthRequest();
        }


        private void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            authorize.GetRequestToken(RequestTokenReceived);
        }

        private async void RequestTokenReceived(Uri uri)
        {
            string requestToken;
            if ((requestToken = await SplitwiseAuthenticationBroker.AuthenticateAsync(uri)) != null)
            {
                authorize.GetAccessToken(requestToken, AccessTokenReceived);
            }

        }

        private void AccessTokenReceived(string accessToken, string accessTokenSecret)
        {
            Helpers.AccessToken = accessToken;
            Helpers.AccessTokenSecret = accessTokenSecret;
            App.accessToken = Helpers.AccessToken;
            App.accessTokenSecret = Helpers.AccessTokenSecret;
            this.Frame.Navigate(typeof(MainPage), true);
        }
    }
}
