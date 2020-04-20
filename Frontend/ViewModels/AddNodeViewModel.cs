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

        [Required(ErrorMessage = "{0} yra privalomas.")]
        [Display(Name = "Pavadinimas")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} yra privalomas.")]
        [Display(Name = "Adresas")]
        public string Endpoint { get; set; }

        [Required(ErrorMessage = "{0} yra privaloma.")]
        [Display(Name = "Ping-pong adresas")]
        public string HealthEndpoint { get; set; }

        [Required(ErrorMessage = "{0} yra privaloma.")]
        [Display(Name = "Produkcinė taisyklė")]
        public string Rule { get; set; }

        [Required(ErrorMessage = "{0} yra privaloma.")]
        [Display(Name = "Antecedentų tipai")]
        public List<DataTypeEnum> InputDataType { get; set; }

        [Required(ErrorMessage = "{0} yra privaloma.")]
        [Display(Name = "Konsekvento tipas")]
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
