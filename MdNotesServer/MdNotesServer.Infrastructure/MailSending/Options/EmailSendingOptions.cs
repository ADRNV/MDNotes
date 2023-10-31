using Microsoft.Extensions.Options;

namespace MdNotesServer.Infrastructure.MailSending.Options
{
    public class EmailSendingOptions : IOptions<EmailSendingOptions>
    {
        public EmailSendingOptions Value => new EmailSendingOptions(UserName, Password);

        public EmailSendingOptions(string userName, string password)
        {
            UserName = userName;

            Password = password;
        }

        public string UserName { get; set; }

        public string Password { get; set; }



    }
}
