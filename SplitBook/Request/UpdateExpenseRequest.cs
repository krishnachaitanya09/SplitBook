using Newtonsoft.Json;

using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SplitBook.Request
{
    class UpdateExpenseRequest : RestBaseRequest
    {
        public static String updateExpenseURL = "update_expense/";
        Expense updatedExpense;

        public UpdateExpenseRequest(Expense expense)
            : base()
        {
            this.updatedExpense = expense;
        }

        public async Task UpdateExpense(Action<bool> CallbackOnSuccess, Action<HttpStatusCode> CallbackOnFailure)
        {
            List<KeyValuePair<string, string>> postContent = new List<KeyValuePair<string, string>>();

            if (updatedExpense.payment)
                postContent.Add(new KeyValuePair<string, string>("payment", "true"));
            else
                postContent.Add(new KeyValuePair<string, string>("payment", "false"));

            postContent.Add(new KeyValuePair<string, string>("cost", Convert.ToString(Convert.ToDouble(updatedExpense.cost), System.Globalization.CultureInfo.InvariantCulture)));
            postContent.Add(new KeyValuePair<string, string>("description", updatedExpense.description));

            if (!String.IsNullOrEmpty(updatedExpense.currency_code))
                postContent.Add(new KeyValuePair<string, string>("currency_code", updatedExpense.currency_code));

            if (!String.IsNullOrEmpty(updatedExpense.creation_method))
            {
                postContent.Add(new KeyValuePair<string, string>("creation_method", updatedExpense.creation_method));
            }

            if (!String.IsNullOrEmpty(updatedExpense.details) && !updatedExpense.details.Equals(Expense.DEFAULT_DETAILS))
            {
                postContent.Add(new KeyValuePair<string, string>("details", updatedExpense.details));
            }

            if (!String.IsNullOrEmpty(updatedExpense.date))
            {
                postContent.Add(new KeyValuePair<string, string>("date", updatedExpense.date));
            }

            if (updatedExpense.group_id != 0)
            {
                postContent.Add(new KeyValuePair<string, string>("group_id", Convert.ToString(updatedExpense.group_id, System.Globalization.CultureInfo.InvariantCulture)));
            }

            int count = 0;
            foreach (var user in updatedExpense.users)
            {
                string idKey = String.Format("users__array_{0}__user_id", count);
                string paidKey = String.Format("users__array_{0}__paid_share", count);
                string owedKey = String.Format("users__array_{0}__owed_share", count);
                postContent.Add(new KeyValuePair<string, string>(idKey, Convert.ToString(user.user_id, System.Globalization.CultureInfo.InvariantCulture)));
                postContent.Add(new KeyValuePair<string, string>(paidKey, Convert.ToString(Convert.ToDouble(user.paid_share), System.Globalization.CultureInfo.InvariantCulture)));
                postContent.Add(new KeyValuePair<string, string>(owedKey, Convert.ToString(Convert.ToDouble(user.owed_share), System.Globalization.CultureInfo.InvariantCulture)));

                count++;
            }
            HttpContent httpContent = new FormUrlEncodedContent(postContent);
            try
            {
                HttpResponseMessage response = await client.PostAsync(updateExpenseURL + updatedExpense.id.ToString(), httpContent);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Expense> expenseList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Expense>>(testToken.ToString(), settings);
                if (expenseList != null)
                {
                    Expense payment = expenseList[0];
                    if (payment.id != 0)
                    {
                        if (updatedExpense.receiptFile != null)
                        {
                            try
                            {
                                using (var form = new MultipartFormDataContent())
                                {
                                    using (IRandomAccessStream fileStream = await updatedExpense.receiptFile.OpenAsync(FileAccessMode.Read))
                                    {
                                        DataReader dataReader = new DataReader(fileStream.GetInputStreamAt(0));
                                        var bytes = new byte[fileStream.Size];
                                        await dataReader.LoadAsync((uint)fileStream.Size);
                                        dataReader.ReadBytes(bytes);
                                        var fileContent = new ByteArrayContent(bytes);
                                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                        {
                                            Name = "\"receipt\"",
                                            FileName = "\"" + updatedExpense.receiptFile.Name + "\""
                                        }; // the extra quotes are key here
                                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(updatedExpense.receiptFile.ContentType);
                                        form.Add(fileContent);
                                        response = await client.PostAsync(updateExpenseURL + payment.id, form);
                                    }
                                }
                            }
                            catch { }
                        }
                        CallbackOnSuccess(true);
                    }
                    else
                        CallbackOnFailure(response.StatusCode);
                }
                else
                    CallbackOnFailure(response.StatusCode);
            }
            catch (Exception)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
