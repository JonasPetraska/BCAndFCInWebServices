using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Enums;
using Common.Models;

namespace Common.Comparers
{
    public class BackwardsChainingTraceElementComparer : IEqualityComparer<TraceElement>
    {
        public bool Equals(TraceElement x, TraceElement y)
        {
            var equals = true;

            if (x.Type != TraceElementTypeEnum.NewGoal || y.Type != TraceElementTypeEnum.NewGoal )
                equals = false;

            if (x.Rule?.Number != y.Rule?.Number)
                equals = false;

            if (x.Rule?.ToStringFull() != y.Rule?.ToStringFull())
                equals = false;

            if (string.Join(",", x.NewGoals) != string.Join(",", y.NewGoals))
                equals = false;

            return equals;
        }

        public int GetHashCode(TraceElement obj)
        {
            int hash = 17;
            hash = hash * 23 + (obj.Type).GetHashCode();
            hash = hash * 23 + (obj?.Rule?.Number ?? "").GetHashCode();
            hash = hash * 23 + (obj?.Rule?.ToStringFull() ?? "").GetHashCode();
            hash = hash * 23 + (string.Join(",", obj.NewGoals)).GetHashCode();
            return hash;
        }
    }
}
