using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Rule
    {
        public int NumberNumeric { get; set; }
        public string Number { get; set; }
        public char RightSide { get; set; }
        public List<char> LeftSide { get; set; }


        //For FC
        public bool Flag1 { get; set; }
        public bool Flag2 { get; set; }
        public override string ToString()
        {
            return Number;
        }

        public string ToStringFull()
        {
            return string.Join(",", LeftSide) + "->" + RightSide;
        }
    }
}
