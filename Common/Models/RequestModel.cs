using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class RequestModel
    {
        public string MethodType { get; set; }
        public char Goal { get; set; }
        public List<char> Facts { get; set; }
        public List<Rule> Rules { get; set; }
        public List<Node> Nodes { get; set; }

        public void AssignNumbersToRules()
        {
            //foreach(var rule in Rules)
            //{
            //    var ruleInNodes = Nodes.FirstOrDefault(x => x.Rule.ToStringFull() == rule.ToStringFull());
            //    rule.Number = ruleInNodes.Name;
            //}

            var i = 1;
            foreach (var rule in Rules)
            {
                rule.Number = "R" + i;
                rule.NumberNumeric = i;
                i++;
            }
        }
    }
}
