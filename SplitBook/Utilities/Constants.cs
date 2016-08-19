using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SplitBook.Utilities
{
    class Constants
    {
        public static string APP_VERSION = "app_version";
        public static String DATABASE_NAME = "splitbook.v1.sqlite";
        public static string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, DATABASE_NAME);

        public static String SPLITWISE_API_URL = "https://secure.splitwise.com/api/v3.0/";
        public static String SPLITWISE_AUTHORIZE_URL = "https://secure.splitwise.com/authorize";
        public static String OAUTH_CALLBACK = "http://techcryptic.com";
        public static String consumerKey = "etGuasDJQxqFTfpVFsWdeunzraxAtfi3cCNxwcOL";
        public static String consumerSecret = "S3cIJuC7IC2FaNj6EjGThZKREt0zXnTcVODUmBLJ";

        public static String ACCESS_TOKEN_TAG = "access_token";
        public static String ACCESS_TOKEN_SECRET_TAG = "access_token_secret";

        public static String LAST_UPDATED_TIME = "last_update";
        public static String CURRENT_USER_ID = "current_user_id";

        public static String DEBT_SIMPLIFICATION_DO_NOT_SHOW = "debt_simplification_do_not_show";

        public static string SELECTED_USER = "selected_user";
        public static string SELECTED_EXPENSE = "selected_expense";
        public static string SELECTED_GROUP = "selected_group";
        public static string PAYMENT_USER = "payment_user";
        public static string ADD_EXPENSE = "add_expense";

        public static string PAYMENT_TYPE = "payment_type";
        public static int PAYMENT_TO = 1000;
        public static int PAYMENT_FROM = 1001;
        public static string PAYMENT_GROUP = "payment_group";

        public static string NEW_USER = "new_user";
        public static string NEW_GROUP = "new_group";
    }
}
