namespace Ajinomoto.Arc.Common.DtoModels
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
