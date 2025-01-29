
using Ajinomoto.Arc.Business.Interfaces;
using Ajinomoto.Arc.Common.AppModels;
using Ajinomoto.Arc.Common.Constants;
using Ajinomoto.Arc.Common.DtoModels;
using Serilog;

namespace Ajinomoto.Arc.Business.Modules
{
    public class AdminService : IAdminService
    {
        private readonly IMailService _mailService;

        public AdminService(IMailService mailService)
        {
            _mailService = mailService;
        }

        public async Task<ResultBase> EmailTesting()
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var result = new ResultBase
                    {
                        Success = false,
                        Message = ""
                    };

                    var mailRequest = new MailRequest
                    {
                        ToEmail = new List<string>(),
                        Cc = new List<string>(),
                        Subject = EmailConstants.S_EMAIL_TESTING_SUBJECT
                    };

                    mailRequest.ToEmail.Add(EmailConstants.S_EMAIL_TESTING_DUMMY_EMAIL);

                    mailRequest.Body = EmailConstants.S_EMAIL_TESTING_BODY;

                    await _mailService.SendEmailAsync(mailRequest);
                    Log.Logger.Information($"Method: SendReminder(), mailRequest: {mailRequest}");

                    result.Message = EmailConstants.S_EMAIL_TESTING_SUCCESS_MESSAGE;
                    result.Success = true;

                    return result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Method: EmailTesting(), " +
                        $"Message: {ex.Message}");
                    throw;
                }
            }).ConfigureAwait(false);
        }
    }
}
