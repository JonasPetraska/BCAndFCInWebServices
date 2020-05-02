using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class ProgramInput
    {
        public DataTypeEnum Type { get; set; }
        public int NodeId { get; set; }
        public char Letter { get; set; }
    }
}
