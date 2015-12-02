using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitWisely.Model
{
    public class AmountSplit
    {
        const String EQUALLY = "equally";
        const String UNEQUALLY = "unequally";

        public const int TYPE_SPLIT_EQUALLY = 0;
        public const int TYPE_SPLIT_UNEQUALLY = 1;

        [Unique]
        public int id { get; set; }
        public String typeString { get; set; }

        public static AmountSplit EqualSplit = new AmountSplit() { id = TYPE_SPLIT_EQUALLY, typeString = EQUALLY };
        public static AmountSplit UnequalSplit = new AmountSplit() { id = TYPE_SPLIT_UNEQUALLY, typeString = UNEQUALLY };

        public static List<AmountSplit> GetAmountSplitTypes()
        {
            List<AmountSplit> splitList = new List<AmountSplit>();
            splitList.Add(EqualSplit);
            splitList.Add(UnequalSplit);
            return splitList;
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

        public override string ToString()
        {
            return typeString;
        }
    }
}
