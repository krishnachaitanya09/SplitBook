using SQLite;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    class ExpenseType
    {
        static String SPLIT = "Split this bill";
        static String FRIEND = "{0} owes you the full total";
        static String YOU = "You owe {0} the full total";

        public const int TYPE_SPLIT_BILL = 0;
        public const int TYPE_FRIEND_OWES = 1;
        public const int TYPE_YOU_OWE = 2;

        [Unique]
        public int id { get; set; }
        public String typeString { get; set; }

        public static List<ExpenseType> GetAllExpenseTypeList(String FriendFirstName)
        {
            List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();

            ExpenseTypeList.Add(new ExpenseType() { id = TYPE_SPLIT_BILL, typeString = SPLIT });
            ExpenseTypeList.Add(new ExpenseType() { id = TYPE_FRIEND_OWES, typeString = String.Format(FRIEND, FriendFirstName) });
            ExpenseTypeList.Add(new ExpenseType() { id = TYPE_YOU_OWE, typeString = String.Format(YOU, FriendFirstName) });

            return ExpenseTypeList;
        }

        public static List<ExpenseType> GetOnlySplitExpenseTypeList()
        {
            List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();

            ExpenseTypeList.Add(new ExpenseType() { id = TYPE_SPLIT_BILL, typeString = SPLIT });

            return ExpenseTypeList;
        }

        public override string ToString()
        {
            return typeString;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
                return false;

            AmountSplit p = obj as AmountSplit;
            if ((System.Object)p == null)
            {
                return false;
            }

            return p.id == id;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}
