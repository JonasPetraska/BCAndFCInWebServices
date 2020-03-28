using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public class ExecuteProgramViewModel
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public List<Node> Nodes { get; set; }
        public List<string> NodeInputValues { get; set; }


        public string Result { get; set; }
        public DataTypeEnum OutputDataType => Nodes.Last().OutputDataType;
    }
}
