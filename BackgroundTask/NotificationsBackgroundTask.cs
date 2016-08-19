using BackgroundTasks.Model;
using BackgroundTasks.Request;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.System.Profile;
using Windows.UI.Notifications;


namespace BackgroundTasks
{
    public sealed class NotificationsBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            GetNotifications request = new GetNotifications();
            IList<Notifications> notifications = await request.getNotifications();
            if (notifications.Count > 0)
            {
                BuildNotifications(notifications);
            }
            deferral.Complete();
        }


        private TileContent GenerateTileContent(Notifications notification)
        {
            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = GenerateTileBinding(notification),
                    TileWide = GenerateTileBinding(notification),
                    TileLarge = GenerateTileBinding(notification)
                }
            };
        }

        private TileBinding GenerateTileBinding(Notifications notification)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    TextStacking = TileTextStacking.Center,

                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                new AdaptiveSubgroup()
                                 {
                                    HintWeight = 1,
                                    Children =
                                    {
                                        new AdaptiveImage() { Source = notification.image_url }
                                    }
                                 },
                                new AdaptiveSubgroup()
                                {
                                    HintWeight = 7,
                                    Children =
                                    {
                                        new AdaptiveText() { Text = notification.content, HintWrap = true },
                                        new AdaptiveText() { Text = notification.created_at, HintStyle = AdaptiveTextStyle.CaptionSubtle }
                                    }
                                }
                            }
                        }
                    }
                },
                Branding = TileBranding.Name
            };
        }

        private ToastContent GenerateToastContent(Notifications notification)
        {
            return new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                           new AdaptiveGroup()
                        {
                            Children =
                            {
                                new AdaptiveSubgroup()
                                 {
                                    HintWeight = 1,
                                    Children =
                                    {
                                        new AdaptiveImage() { Source = notification.image_url }
                                    }
                                 },
                                new AdaptiveSubgroup()
                                {
                                    HintWeight = 7,
                                    Children =
                                    {
                                        new AdaptiveText() { Text = notification.content, HintWrap = true },
                                        new AdaptiveText() { Text = notification.created_at, HintStyle = AdaptiveTextStyle.CaptionSubtle }
                                    }
                                }
                            }
                        }
                        }
                    }
                }
            };
        }

        private void BuildNotifications(IList<Notifications> notifications)
        {
            var badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            badgeUpdater.Clear();
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.EnableNotificationQueue(true);
            tileUpdater.Clear();
            BadgeNumericContent badgeContent = new BadgeNumericContent((uint)notifications.Count);
            badgeUpdater.Update(new BadgeNotification(badgeContent.GetXml()));
            ToastNotificationManager.ConfigureNotificationMirroring(NotificationMirroring.Allowed);

            // Keep track of the number feed items that get tile notifications.
            int itemCount = 0;

            // Create a tile notification for each feed item.
            foreach (var notification in notifications)
            {
                // Create a new tile notification.
                tileUpdater.Update(new TileNotification(GenerateTileContent(notification).GetXml()));
                ToastNotification toastNotification = new ToastNotification(GenerateToastContent(notification).GetXml());
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
                // Don't create more than 5 notifications.
                if (itemCount++ > 5) break;
            }
        }
    }
}
