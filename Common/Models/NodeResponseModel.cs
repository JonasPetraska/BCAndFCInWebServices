using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Models
{
    public class NodeResponseModel
    {
        [Required]
        public string OutputData { get; set; }
    }
}
