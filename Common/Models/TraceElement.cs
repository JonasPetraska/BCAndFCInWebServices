using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class TraceElement
    {
        public TraceElement()
        {
            NewGoals = new List<char>();
            Facts = new List<char>();
        }

        public TraceElement(Rule rule, List<char> newGoals, char goal) : this()
        {
            Type = TraceElementTypeEnum.NewGoal;
            Rule = rule;
            NewGoals.AddRange(newGoals);
            Goal = goal;
        }

        public TraceElement(Node node) : this()
        {
            Type = TraceElementTypeEnum.NewGoal;
            Node = node;
        }

        public TraceElement(char newFact, List<char> facts, char goal) : this()
        {
            Type = TraceElementTypeEnum.NewFact;
            Fact = newFact;
            Facts.AddRange(facts);
            Goal = goal;
        }

        public TraceElement(char fact, char goal, bool earlierDerived = false) : this()
        {
            Type = earlierDerived ? TraceElementTypeEnum.EarlierDerivedFact : TraceElementTypeEnum.Fact;
            Fact = fact;
            Goal = goal;
        }

        //FC specific constructors
        public TraceElement(int iterationNumber, TraceElementTypeEnum type, Rule rule) : this()
        {
            IterationNumber = iterationNumber;
            Type = type;
            Rule = rule;
        }

        public TraceElement(int iterationNumber, TraceElementTypeEnum type, Rule rule, List<char> facts) : this()
        {
            IterationNumber = iterationNumber;
            Type = type;
            Rule = rule;
            Facts.AddRange(facts);
        }


        public List<char> NewGoals { get; set; }
        public char Goal { get; set; }
        public char Fact { get; set; }
        public List<char> Facts { get; set; }
        public TraceElementTypeEnum Type { get; set; }
        public Rule Rule { get; set; }
        public Node Node { get; set; }

        //FC Specific trace elements
        public int IterationNumber { get; set; }
        public List<char> MissingFacts { get; set; }


    }
}
