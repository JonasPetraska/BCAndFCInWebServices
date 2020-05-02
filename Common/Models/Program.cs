using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Program
    {
        public Program()
        {
            Inputs = new List<ProgramInput>();
            Nodes = new List<Node>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<Node> Nodes { get; set; }
        public List<ProgramInput> Inputs { get; set; }
    }
}
