namespace Ajinomoto.Arc.Common.Helpers
{
    public class AppSettings
    {
        public string AjinomotoDomain { get; set; }
        public int DropdownMaxItem { get; set; }
        public string Secret { get; set; }
        public string StoredFilesPath { get; set; }
        public string SharingFolder { get; set; }
        public bool UsingTemporaryEmail { get; set; }
        public string TemporaryEmailTo { get; set; }
        public string EmailCc { get; set; }
        public string WebUrl { get; set; }
    }
}
