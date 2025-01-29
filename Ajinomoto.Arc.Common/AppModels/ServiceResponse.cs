namespace Ajinomoto.Arc.Common.AppModels
{
    public class ServiceResponse<T>
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public string Code { get; set; }
        public T Data { get; set; }
    }
}
