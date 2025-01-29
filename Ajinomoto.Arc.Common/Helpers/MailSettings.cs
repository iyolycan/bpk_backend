namespace Ajinomoto.Arc.Common.Helpers
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsUsingGmailSmtp { get; set; }
    }
}
