using MdNotesServer.Infrastructure.MailSending.Options;
using Microsoft.Extensions.Options;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using MdNotesServer.Infrastructure.MailSending.Messages;
using Microsoft.Extensions.Logging;

namespace MdNotesServer.Infrastructure.MailSending
{
    public class EmailSender
    {
        private EmailSendingOptions _options;

        private ISmtpClient _smtpClient;

        private ILogger _logger;

        private bool _sendend = false;

        public EmailSender(IOptions<EmailSendingOptions> options, ISmtpClient smtpClient, ILogger logger)
        {
            _options = options.Value;

            _smtpClient = smtpClient;

            _smtpClient.MessageSent += (s, e) => { _sendend = true; };

            _logger = logger;
        }

        public async Task<bool> SendAsync(string from, string to, Mail message)
        {
            await _smtpClient.AuthenticateAsync(_options.UserName, _options.Password);

            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(from, from));

            mimeMessage.To.Add(new MailboxAddress(to, to));

            mimeMessage.Subject = message.Header;

            mimeMessage.Body = new TextPart("plain") { Text = message.Body };

            _logger.LogInformation($"Mail to {to} sendent - {_sendend}");

            return _sendend;
        }
    }
}
