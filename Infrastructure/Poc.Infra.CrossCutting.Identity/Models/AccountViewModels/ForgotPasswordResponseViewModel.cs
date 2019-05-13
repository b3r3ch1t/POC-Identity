namespace Poc.Infra.CrossCutting.Identity.Models.AccountViewModels
{
    public class ForgotPasswordResponseViewModel
    {
        public string FromName => "Human Data Income";
        public string From => "noreply@humandataincome.com";
        public string To { get; set; }
        public string Subject { get; set; }
        public string Type => "ForgotPasswordResponse";
        public string Code { get; set; }
    }
}
