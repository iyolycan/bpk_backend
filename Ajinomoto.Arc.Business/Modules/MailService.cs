using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Ajinomoto.Arc.Common.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ajinomoto.Arc.Business.Modules
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly AppSettings _appSettings;

        public MailService(IOptions<MailSettings> mailSettings, IOptions<AppSettings> appSettings)
        {
            _mailSettings = mailSettings.Value;
            _appSettings = appSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            Console.WriteLine("cek email starting 1");
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);

            foreach (var item in mailRequest.ToEmail)
            {
                email.To.Add(MailboxAddress.Parse(item));
            }

            foreach (var item in mailRequest.Cc)
            {
                email.Cc.Add(MailboxAddress.Parse(item));
            }

            email.Subject = mailRequest.Subject;

            // this to prevent sending to production email
            if (_appSettings.UsingTemporaryEmail)
            {
                var tempTo = string.Join(", ", email.To.Select(x => x.ToString()).ToArray());
                var tempCc = string.Join(", ", email.Cc.Select(x => x.ToString()).ToArray());
                var tempBcc = string.Join(", ", email.Bcc.Select(x => x.ToString()).ToArray());

                mailRequest.Body = string.Format(EmailConstants.S_TEMPORARY_EMAIL_USED,
                    tempTo, tempCc, tempBcc)
                    + mailRequest.Body;

                email.To.Clear();
                email.Cc.Clear();
                email.Bcc.Clear();

                email.To.Add(MailboxAddress.Parse(_appSettings.TemporaryEmailTo));
                var emailCc = _appSettings.EmailCc.Split(",").ToList();
                foreach (var item in emailCc)
                {
                    email.Cc.Add(MailboxAddress.Parse(item));
                }
            }

            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            Console.WriteLine("cek email starting");
            if (_mailSettings.IsUsingGmailSmtp)
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                Console.WriteLine("cek email: " + _mailSettings.Mail);
                Console.WriteLine("cek email: " + _mailSettings.Mail);
            }
            else
            {
                Console.WriteLine("cek email 2: " + _mailSettings.Mail);
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.None);
            }

            try
            {
                await smtp.SendAsync(email);
                Console.WriteLine("Email sent successfully to: " + string.Join(", ", mailRequest.ToEmail));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send email: " + ex.Message);
            }
            smtp.Disconnect(true);
        }
            // code here
        public async Task SendEmailAsyncInvoice(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);

            foreach (var item in mailRequest.ToEmail)
            {
                email.To.Add(MailboxAddress.Parse(item));
            }

            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }

                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            try
            {
                await smtp.SendAsync(email);
                Console.WriteLine("Invoice email sent successfully to: " + string.Join(", ", mailRequest.ToEmail));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to send invoice email: " + ex.Message);
            }
            smtp.Disconnect(true);
        }
    }
}
