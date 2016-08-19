﻿using SplitBook.Add_Expense_Pages;
using SplitBook.Controller;
using SplitBook.Model;
using SplitBook.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Data.Pdf;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace SplitBook.Views
{
    public sealed partial class ExpenseDetail : Page
    {
        Expense selectedExpense;
        BackgroundWorker deleteExpenseBackgroundWorker;
        BackgroundWorker commentLoadingBackgroundWorker;
        BackgroundWorker addCommentBackgroundWorker;

        ObservableCollection<CustomCommentView> comments = new ObservableCollection<CustomCommentView>();

        public ExpenseDetail()
        {
            this.InitializeComponent();
            BackButton.Click += BackButton_Click;

            selectedExpense = (Application.Current as App).SELECTED_EXPENSE;
            selectedExpense.displayType = Expense.DISPLAY_FOR_ALL_USER;
            this.DataContext = selectedExpense;

            deleteExpenseBackgroundWorker = new BackgroundWorker();
            deleteExpenseBackgroundWorker.WorkerSupportsCancellation = true;
            deleteExpenseBackgroundWorker.DoWork += new DoWorkEventHandler(deleteExpenseBackgroundWorker_DoWork);

            commentLoadingBackgroundWorker = new BackgroundWorker();
            commentLoadingBackgroundWorker.WorkerSupportsCancellation = true;
            commentLoadingBackgroundWorker.DoWork += new DoWorkEventHandler(commentLoadingBackgroundWorker_DoWork);

            if (!commentLoadingBackgroundWorker.IsBusy)
            {
                commentLoadingBackgroundWorker.RunWorkerAsync();
            }

            addCommentBackgroundWorker = new BackgroundWorker();
            addCommentBackgroundWorker.WorkerSupportsCancellation = true;
            addCommentBackgroundWorker.DoWork += new DoWorkEventHandler(addCommentBackgroundWorker_DoWork);

            this.conversationView.ItemsSource = this.comments;
            this.conversationView.Loaded += (o, e) => ScrollToBottom();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BackButton.Visibility = this.Frame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            save.IsEnabled = selectedExpense.receipt.original != null;
            LoadReceipt();
            GoogleAnalytics.EasyTracker.GetTracker().SendView("ExpenseDetailsPage");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private async void btnDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog messageDialog = new MessageDialog("Are you sure?", "Delete Expense");
            messageDialog.Commands.Add(new UICommand { Label = "yes", Id = 0 });
            messageDialog.Commands.Add(new UICommand { Label = "no", Id = 1 });
            IUICommand result = await messageDialog.ShowAsync();

            switch ((int)result.Id)
            {
                case 0:
                    if (deleteExpenseBackgroundWorker.IsBusy != true)
                    {
                        busyIndicator.IsActive = true;
                        deleteExpenseBackgroundWorker.RunWorkerAsync();
                    }
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }

        private void deleteExpenseBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ModifyDatabase modify = new ModifyDatabase(_deleteExpenseCompleted);
            modify.deleteExpense(selectedExpense.id);
        }

        private async void _deleteExpenseCompleted(bool success, HttpStatusCode errorCode)
        {
            if (success)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    busyIndicator.IsActive = false;
                    this.Frame.Navigate(typeof(FriendsPage));
                    MainPage.Current.FetchData();
                });
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    busyIndicator.IsActive = false;
                    if (errorCode == HttpStatusCode.Unauthorized)
                    {
                        Helpers.logout();
                        (Application.Current as App).rootFrame.Navigate(typeof(LoginPage));
                        MainPage.Current.Frame.Navigate(typeof(LoginPage));
                    }
                    else
                    {
                        MessageDialog messageDialog = new MessageDialog("Unable to delete expense", "Error");
                        await messageDialog.ShowAsync();
                    }
                });
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //currently you cannot edit all expenses.
            if (canEditExpense())
            {
                (Application.Current as App).ADD_EXPENSE = selectedExpense;
                this.Frame.Navigate(typeof(EditExpense));
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog("This expense has an unknown user (not a friend). You cannnot edit such an expense in this version. This facility will be added in a future update", "Sorry");
                await messageDialog.ShowAsync();
            }
        }

        private bool canEditExpense()
        {
            foreach (var item in selectedExpense.users)
            {
                //user is null when the user is not a friend and hence does not exist in the DB.
                if (item.user == null)
                    return false;
            }
            return true;
        }

        private void commentLoadingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CommentDatabase commentsObj = new CommentDatabase(_CommentsReceived);
            commentsObj.getComments(selectedExpense.id);
        }

        private async void _CommentsReceived(List<Comment> commentList)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (commentList != null && commentList.Count != 0)
                {
                    if (commentList.Count > 1)
                    {
                        comments.Clear();
                    }
                    foreach (var comment in commentList)
                    {
                        if (String.IsNullOrEmpty(comment.deleted_at))
                        {
                            DateTime createdDate = DateTime.Parse(comment.created_at, System.Globalization.CultureInfo.InvariantCulture);
                            comments.Add(new CustomCommentView(comment.content, createdDate, comment.user.name));
                        }
                    }
                    ScrollToBottom();
                }
                busyIndicator.IsActive = false;
            });
        }

        private void ScrollToBottom()
        {
            var selectedIndex = conversationView.Items.Count - 1;
            if (selectedIndex < 0)
                return;
            conversationView.SelectedIndex = selectedIndex;
            conversationView.UpdateLayout();
            conversationView.ScrollIntoView(conversationView.SelectedItem);
        }

        private void addCommentBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string content = (e.Argument as CustomCommentView).Text;
            CommentDatabase commentsObj = new CommentDatabase(_CommentsReceived);
            commentsObj.addComment(selectedExpense.id, content);
        }

        private void SendButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(commentBox.Text))
            {
                return;
            }

            //send this message to the api via the add comment background worker
            if (!addCommentBackgroundWorker.IsBusy)
            {
                this.Focus(FocusState.Programmatic);
                busyIndicator.IsActive = true;
                addCommentBackgroundWorker.RunWorkerAsync(new CustomCommentView(commentBox.Text.Trim(), DateTime.Now, App.currentUser.name));
                commentBox.Text = "";
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!commentLoadingBackgroundWorker.IsBusy)
            {
                commentLoadingBackgroundWorker.RunWorkerAsync();
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 1:
                    refresh.Visibility = Visibility.Collapsed;
                    save.Visibility = Visibility.Visible;
                    break;
                case 3:
                    save.Visibility = Visibility.Collapsed;
                    refresh.Visibility = Visibility.Visible;
                    break;
                default:
                    save.Visibility = Visibility.Collapsed;
                    refresh.Visibility = Visibility.Collapsed;
                    break;

            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var profilePic = sender as Image;
            BitmapImage pic = new BitmapImage(new Uri("ms-appx:///Assets/Images/profilePhoto.png"));
            profilePic.Source = pic;
        }

        private void receiptPivot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReceiptImage.MaxWidth = e.NewSize.Width;
            ReceiptImage.MaxHeight = e.NewSize.Height;
        }

        private async void LoadReceipt()
        {
            try
            {
                BitmapImage image = new BitmapImage();
                if (selectedExpense.receipt.original != null)
                {
                    Uri receiptUri = new Uri(selectedExpense.receipt.original);
                    if (Path.HasExtension(receiptUri.AbsolutePath))
                    {
                        string extension = Path.GetExtension(receiptUri.AbsolutePath);
                        if (extension.Equals(".pdf"))
                        {
                            HttpClient httpClient = new HttpClient();
                            Stream fileStream = await httpClient.GetStreamAsync(receiptUri);
                            MemoryStream memStream = new MemoryStream();
                            await fileStream.CopyToAsync(memStream);
                            PdfDocument pdfDocument = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());
                            using (PdfPage page = pdfDocument.GetPage(0))
                            {
                                var stream = new InMemoryRandomAccessStream();
                                await page.RenderToStreamAsync(stream);
                                await image.SetSourceAsync(stream);
                            }
                        }
                        else
                        {
                            image.UriSource = receiptUri;
                        }
                    }
                }
                ReceiptImage.Source = image;
            }
            catch (Exception ex)
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileSavePicker fileSavePicker = new FileSavePicker();
                fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                string extension = Path.GetExtension(new Uri(selectedExpense.receipt.original).AbsolutePath);
                string extensionDescription;
                ExtensionMappings.TryGetValue(extension, out extensionDescription);
                fileSavePicker.FileTypeChoices.Add(extensionDescription, new List<string>() { extension });
                fileSavePicker.SuggestedFileName = "Receipt";
                StorageFile destinationFile = await fileSavePicker.PickSaveFileAsync();
                if (destinationFile == null)
                {
                    // The user cancelled the picking operation
                    return;
                }

                using (Stream stream = await destinationFile.OpenStreamForWriteAsync())
                {
                    HttpClient httpClient = new HttpClient();
                    Stream fileStream = await httpClient.GetStreamAsync(selectedExpense.receipt.original);
                    await fileStream.CopyToAsync(stream);
                    await stream.FlushAsync();
                }
            }
            catch (Exception ex)
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendException(ex.Message, false);
            }

        }

        private static IDictionary<string, string> ExtensionMappings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
            {".bmp", "BMP"},
            {".jpg", "JPG"},
            {".jpeg", "JPEG"},
            {".png", "PNG"},
            {".pdf", "PDF Document"}
        };
    }

    public class CustomCommentView
    {
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Text { get; set; }

        public CustomCommentView(string text, DateTime timeStamp, string userName)
        {
            this.Name = userName;
            this.Text = text;
            this.TimeStamp = timeStamp;
        }

        public string FormattedTimeStamp
        {
            get
            {
                return this.TimeStamp.ToString();
            }
        }
    }
}
