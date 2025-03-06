namespace Ajinomoto.Arc.Common.Constants
{
    public class EmailConstants
    {
        public const string S_TEMPORARY_EMAIL_USED = @"
--------------------------------------<br>
<i><b>UsingTemporaryEmail</b></i> setting is active.<br>
Should be sent to: {0}<br>
Cc: {1}<br>
Bcc: {2}<br>
--------------------------------------<br>
<br>
";

        public const string S_BPK_NOT_CREATED_REMINDER_SUBJECT = "BPK Not Created Alert Email";

        public const string S_BPK_NOT_CREATED_REMINDER_BODY = @"
Dear All,<br>
<br>
This Email is sent automatically from BPK Web Application to be auto BPK Not Created Reminder.<br>
<br>
<b>Execution Time: {0}</b><br>
<b>As of Date    : {1}</b><br>
<br>
Please kindly check BPK Not Created in BPK Web Application<br>
<br>
<table border=1 cellpadding=0 cellspacing=0>
  <tr>
    <th style=""padding: 5px;"">No</th>
    <th style=""padding: 5px;"">Tanggal</th>
    <th style=""padding: 5px;"">Nominal</th>
    <th style=""padding: 5px;"">Nama Customer</th>
    <th style=""padding: 5px;"">Area</th>
    <th style=""padding: 5px;"">Cabang</th>
    <th style=""padding: 5px;"">Doc. Interface</th>
    <th style=""padding: 5px;"">Doc. Clearing</th>
  </tr>
{2}
";

        public const string S_BPK_NOT_CREATED_REMINDER_CONTENT = @"
  <tr>
    <td style=""padding: 5px;"">{0}</td>
    <td style=""padding: 5px;"">{1}</td>
    <td style=""padding: 5px;"">{2}</td>
    <td style=""padding: 5px;"">{3}</td>
    <td style=""padding: 5px;"">{4}</td>
    <td style=""padding: 5px;"">{5}</td>
    <td style=""padding: 5px;"">{6}</td>
    <td style=""padding: 5px;"">{7}</td>
  </tr>
";

        public const string S_RESET_PASSWORD_EMAIL_SUBJECT = "BPK Web Application password reset confirmation";
        public const string S_RESET_PASSWORD_EMAIL_BODY = @"
Hi {0},<br>
<br>
There was recently a request to change the password on your account. if you<br>
requested this password change, please click the link below to set a new password<br>
within 30 Minutes:<br>
<br>
But first make sure, email coming from ajinomoto domain<br>
email is end with <b>@asv.ajinomoto.com</b><br>
<br>
{1}<br>
<br>
If you don't want to change your password, just ignore this message.<br>
<br>
Thank you.<br>
Admin<br>
";

        public const string S_EMAIL_TESTING_SUBJECT = "This is only for testing";
        public const string S_EMAIL_TESTING_BODY = "test.";
        public const string S_EMAIL_TESTING_DUMMY_EMAIL = "test@example.com";
        public const string S_EMAIL_TESTING_SUCCESS_MESSAGE = "email successfully sent.";

    }
    public class EmailConstantsInvoice
    {
        public const string S_TEMPORARY_EMAIL_USED = @"
--------------------------------------<br>
<i><b>UsingTemporaryEmail</b></i> setting is active.<br>
Should be sent to: {0}<br>
Cc: {1}<br>
Bcc: {2}<br>
--------------------------------------<br>
<br>
";

        public const string S_BPK_NOT_CREATED_REMINDER_SUBJECT = "BPK Not Created Alert Email";

        public const string S_BPK_NOT_CREATED_REMINDER_BODY = @"
Dear All,<br>
<br>
This Email is sent automatically from BPK Web Application to be auto BPK Not Created Reminder.<br>
<br>
<b>Execution Time: {0}</b><br>
<b>As of Date    : {1}</b><br>
<br>
Please kindly check BPK Not Created in BPK Web Application<br>
<br>
<table border=1 cellpadding=0 cellspacing=0>
  <tr>
    <th style=""padding: 5px;"">No</th>
    <th style=""padding: 5px;"">Tanggal</th>
    <th style=""padding: 5px;"">Nominal</th>
    <th style=""padding: 5px;"">Nama Customer</th>
    <th style=""padding: 5px;"">Area</th>
    <th style=""padding: 5px;"">Cabang</th>
    <th style=""padding: 5px;"">Doc. Interface</th>
    <th style=""padding: 5px;"">Doc. Clearing</th>
  </tr>
{2}
";

        public const string S_BPK_NOT_CREATED_REMINDER_CONTENT = @"
  <tr>
    <td style=""padding: 5px;"">{0}</td>
    <td style=""padding: 5px;"">{1}</td>
    <td style=""padding: 5px;"">{2}</td>
    <td style=""padding: 5px;"">{3}</td>
    <td style=""padding: 5px;"">{4}</td>
    <td style=""padding: 5px;"">{5}</td>
    <td style=""padding: 5px;"">{6}</td>
    <td style=""padding: 5px;"">{7}</td>
  </tr>
";

        public const string S_RESET_PASSWORD_EMAIL_SUBJECT = "BPK Web Application password reset confirmation";
        public const string S_RESET_PASSWORD_EMAIL_BODY = @"
Hi {0},<br>
<br>
There was recently a request to change the password on your account. if you<br>
requested this password change, please click the link below to set a new password<br>
within 30 Minutes:<br>
<br>
But first make sure, email coming from ajinomoto domain<br>
email is end with <b>@asv.ajinomoto.com</b><br>
<br>
{1}<br>
<br>
If you don't want to change your password, just ignore this message.<br>
<br>
Thank you.<br>
Admin<br>
";

        public const string S_EMAIL_TESTING_SUBJECT = "This is only for testing";
        public const string S_EMAIL_TESTING_BODY = "test.";
        public const string S_EMAIL_TESTING_DUMMY_EMAIL = "test@example.com";
        public const string S_EMAIL_TESTING_SUCCESS_MESSAGE = "email successfully sent.";

    }
}
