using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Iteration
    {
        public int Number { get; set; }
        public List<TraceElement> Trace { get; set; }
    }
}
