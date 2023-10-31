namespace MdNotesServer.Infrastructure.MailSending.Messages.Templates
{
    public class ConfirmAccountMail : Mail
    {
        public ConfirmAccountMail(string confiramtionLink)
        {
            Header = "Email confirmation";
            Body = $"For confirm your account follow this link {confiramtionLink}";
        }
    }
}
