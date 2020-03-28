using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    public class ResponseModel
    {
        public ResponseModel() { }

        public ResponseModel(RequestModel reqModel)
        {
            MethodType = reqModel.MethodType;
            Goal = reqModel.Goal;
            Facts = new List<char>(reqModel.Facts);
            Nodes = new List<Node>(reqModel.Nodes);
            Rules = new List<Rule>();
            Production = new List<Rule>();
            Trace = new List<TraceElement>();

            var reqModelTempRules = reqModel.Rules;

            //Normalize rules if backward method
            if (MethodType.ToLower() == "backward")
            {
                var ruleWithGoal = reqModelTempRules.FirstOrDefault(x => x.RightSide == reqModel.Goal);
                if (ruleWithGoal != null)
                {
                    var indexOfRuleWithGoal = reqModelTempRules.IndexOf(ruleWithGoal);
                    var rules = reqModel.Rules;
                    var firstRule = rules[0];
                    rules[0] = ruleWithGoal;
                    rules[indexOfRuleWithGoal] = firstRule;
                    reqModelTempRules = rules;
                }

                Rules.AddRange(reqModelTempRules);
            }
            else
            {
                Rules.AddRange(reqModel.Rules);
            }
        }

        public string MethodType { get; set; }
        public char Goal { get; set; }
        public List<char> Facts { get; set; }
        public List<Rule> Rules { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Rule> Production { get; set; }
        public List<TraceElement> Trace { get; set; }
        public FCBCResultEnum Result { get; set; }

        //FC
        public List<Iteration> FCTrace { get; set; }

        public void GenerateIterationsFromTrace()
        {
            if(MethodType.ToLower() == "forward")
            {
                var groups = Trace.GroupBy(x => x.IterationNumber, (key, g) => new Iteration()
                {
                    Trace = g.ToList(),
                    Number = key
                });

                FCTrace = groups.ToList();
                Trace = new List<TraceElement>();
            }
        }

        public void OrderNodesByProduction()
        {
            var orderedList = new List<Node>();

            foreach(var prod in Production)
            {
                var node = Nodes.FirstOrDefault(x => x.Rule.ToStringFull().Equals(prod.ToStringFull()));
                if(node != null)
                    orderedList.Add(node);
            }

            Nodes = orderedList;
        }
    }
}
