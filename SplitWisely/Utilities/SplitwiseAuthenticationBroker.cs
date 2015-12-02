using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace SplitWisely.Utilities
{
    public static class SplitwiseAuthenticationBroker
    {
        public static Task<string> AuthenticateAsync(Uri uri)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<string>();

            var button = new Button()
            {
                Content = "Close",
                Margin = new Thickness(0, 30, 30, 30),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var w = new WebView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(30.0),               
            };

            var panel = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            
            panel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            panel.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Grid.SetRow(w, 0);
            Grid.SetRow(button, 1);
            panel.Children.Add(w);
            panel.Children.Add(button);

            var b = new Border
            {
                Background =
                new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height,
                Child = panel
            };

            var p = new Popup
            {
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height,
                Child = b,
                HorizontalOffset = 0.0,
                VerticalOffset = 0.0,
            };

            Window.Current.SizeChanged += (s, e) =>
            {
                p.Width = e.Size.Width;
                p.Height = e.Size.Height;
                b.Width = e.Size.Width;
                b.Height = e.Size.Height;
            };

            w.Source = uri;

            button.Click += (sender, e) =>
            {
                p.IsOpen = false;
            };

            w.NavigationStarting += (sender, args) =>
            {
                if (args.Uri != null)
                {
                    if (args.Uri.OriginalString.Contains("oauth_verifier"))
                    {
                        var arguments = args.Uri.AbsoluteUri.Split('?');
                        if (arguments.Length < 1)
                            return;
                        tcs.SetResult(arguments[1]);
                        p.IsOpen = false;
                    }
                    if (args.Uri.OriginalString.Contains("error=access_denied"))
                    {
                        tcs.SetResult(null);
                        p.IsOpen = false;
                    }
                }
            };

            p.IsOpen = true;
            return tcs.Task;
        }
    }
}
