using Newtonsoft.Json;

using SplitBook.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
    class AddExpenseRequest : RestBaseRequest
    {
        public static String addExpenseURL = "create_expense";
        public static String updateExpenseURL = "update_expense/";
        Expense paymentExpense;

        public AddExpenseRequest(Expense expense)
            : base()
        {
            this.paymentExpense = expense;
        }

        public async void addExpense(Action<bool> CallbackOnSuccess, Action<System.Net.HttpStatusCode> CallbackOnFailure)
        {
            List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
            if (paymentExpense.payment)
                content.Add(new KeyValuePair<string, string>("payment", "true"));
            else
                content.Add(new KeyValuePair<string, string>("payment", "false"));

            content.Add(new KeyValuePair<string, string>("cost", Convert.ToString(Convert.ToDouble(paymentExpense.cost), System.Globalization.CultureInfo.InvariantCulture)));
            content.Add(new KeyValuePair<string, string>("description", paymentExpense.description));

            if (!String.IsNullOrEmpty(paymentExpense.currency_code))
                content.Add(new KeyValuePair<string, string>("currency_code", paymentExpense.currency_code));

            if (!String.IsNullOrEmpty(paymentExpense.creation_method))
            {
                content.Add(new KeyValuePair<string, string>("creation_method", paymentExpense.creation_method));
            }

            if (!String.IsNullOrEmpty(paymentExpense.details) && !paymentExpense.details.Equals(Expense.DEFAULT_DETAILS))
            {
                content.Add(new KeyValuePair<string, string>("details", paymentExpense.details));
            }

            if (!String.IsNullOrEmpty(paymentExpense.date))
            {
                content.Add(new KeyValuePair<string, string>("date", paymentExpense.date));
            }

            if (paymentExpense.group_id != 0)
            {
                content.Add(new KeyValuePair<string, string>("group_id", Convert.ToString(paymentExpense.group_id, System.Globalization.CultureInfo.InvariantCulture)));
            }

            int count = 0;
            foreach (var user in paymentExpense.users)
            {
                string idKey = String.Format("users__array_{0}__user_id", count);
                string paidKey = String.Format("users__array_{0}__paid_share", count);
                string owedKey = String.Format("users__array_{0}__owed_share", count);
                content.Add(new KeyValuePair<string, string>(idKey, Convert.ToString(user.user_id, System.Globalization.CultureInfo.InvariantCulture)));
                content.Add(new KeyValuePair<string, string>(paidKey, Convert.ToString(Convert.ToDouble(user.paid_share), System.Globalization.CultureInfo.InvariantCulture)));
                content.Add(new KeyValuePair<string, string>(owedKey, Convert.ToString(Convert.ToDouble(user.owed_share), System.Globalization.CultureInfo.InvariantCulture)));

                count++;
            }

            HttpContent httpContent = new FormUrlEncodedContent(content);
            try
            {
                HttpResponseMessage response = await client.PostAsync(addExpenseURL, httpContent);
                Newtonsoft.Json.Linq.JToken root = Newtonsoft.Json.Linq.JObject.Parse(await response.Content.ReadAsStringAsync());
                Newtonsoft.Json.Linq.JToken testToken = root["expenses"];
                JsonSerializerSettings settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                List<Expense> expenseList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Expense>>(testToken.ToString(), settings);
                if (expenseList != null && expenseList.Count != 0)
                {
                    Expense payment = expenseList[0];
                    if (payment.id != 0)
                    {
                        if (paymentExpense.receiptFile != null)
                        {
                            try
                            {
                                using (var form = new MultipartFormDataContent())
                                {
                                    using (IRandomAccessStream fileStream = await paymentExpense.receiptFile.OpenAsync(FileAccessMode.Read))
                                    {
                                        DataReader dataReader = new DataReader(fileStream.GetInputStreamAt(0));
                                        var bytes = new byte[fileStream.Size];
                                        await dataReader.LoadAsync((uint)fileStream.Size);
                                        dataReader.ReadBytes(bytes);
                                        var fileContent = new ByteArrayContent(bytes);
                                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                        {
                                            Name = "\"receipt\"",
                                            FileName = "\"" + paymentExpense.receiptFile.Name + "\""
                                        }; // the extra quotes are key here
                                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(paymentExpense.receiptFile.ContentType);
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
            catch (Exception e)
            {
                CallbackOnFailure(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
