using System.Net.Mail;

namespace Ajinomoto.Arc.Common.Helpers
{
    public static class AppHelper
    {
        public static bool TryParseEmail(string email, out MailAddress? addr)
        {
            try
            {
                addr = new MailAddress(email);

                return true;
            }
            catch
            {
                addr = null;
                return false;
            }
        }
    }
}
