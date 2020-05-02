using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class ProgramInputWithValue : ProgramInput
    {
        public ProgramInputWithValue() { }
        public ProgramInputWithValue(ProgramInput input)
        {
            Letter = input.Letter;
            NodeId = input.NodeId;
            Type = input.Type;
        }

        public string Value { get; set; }
    }
}
