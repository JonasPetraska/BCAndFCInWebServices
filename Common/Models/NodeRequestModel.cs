using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Models
{
    public class NodeRequestModel
    {
        [Required]
        public List<string> InputData { get; set; }
    }
}
