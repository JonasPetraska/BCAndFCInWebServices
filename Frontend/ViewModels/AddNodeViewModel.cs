using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.ViewModels
{
    public class AddNodeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Endpoint { get; set; }

        [Required]
        [Display(Name = "Health Endpoint")]
        public string HealthEndpoint { get; set; }

        [Required]
        public string Rule { get; set; }

        [Required]
        [Display(Name = "Input data types")]
        public List<DataTypeEnum> InputDataType { get; set; }

        [Required]
        [Display(Name = "Output data type")]
        public DataTypeEnum OutputDataType { get; set; }

        public Task<HttpRequestResult<bool>> ValidateEndpoint()
        {
            return ((Node)this).ValidateEndpoint();
        }

        public HttpRequestResult<bool> ValidateRule()
        {
            return Node.ValidateRule(Rule);
        }

        public static explicit operator Node(AddNodeViewModel viewModel)
        {
            return new Node()
            {
                Name = viewModel.Name,
                Id = viewModel.Id,
                Endpoint = viewModel.Endpoint,
                HealthEndPoint = viewModel.HealthEndpoint,
                InputDataType = new List<DataTypeEnum>(viewModel.InputDataType),
                OutputDataType = viewModel.OutputDataType
            };
        }

    }
}
