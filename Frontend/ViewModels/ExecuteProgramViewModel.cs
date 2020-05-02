using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public class ExecuteProgramViewModel
    {
        public ExecuteProgramViewModel()
        {
            NodeInputValues = new List<ProgramInputWithValue>();
        }

        public int ProgramId { get; set; }

        [Display(Name = "Vykdymo plano pavadinimas")]
        public string ProgramName { get; set; }

        [Display(Name = "Tiekėjai")]
        public List<Node> Nodes { get; set; }

        //[Display(Name = "Antecedentų reikšmės")]
        //public List<string> NodeInputValues { get; set; }

        [Display(Name = "Antecedentų reikšmės")]
        public List<ProgramInputWithValue> NodeInputValues { get; set; }

        [Display(Name = "Rezultatas")]
        public string Result { get; set; }

        [Display(Name = "Klaida")]
        public string Error { get; set; }

        public DataTypeEnum OutputDataType => Nodes == null ? DataTypeEnum.None : Nodes.Last().OutputDataType;
    }
}
