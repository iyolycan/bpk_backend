namespace Ajinomoto.Arc.Common.DtoModels
{
    public class BasicListRequest
    {
        public int Limit { get; set; }
        public int Page { get; set; }
        public string Filter { get; set; }
    }
}
