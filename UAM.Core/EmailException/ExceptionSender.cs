using System.Reflection;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace UAM.Core.EmailException;

public class ExceptionSender
{
    public static async Task SendEmailAsync(string message, string email)
    {
        var info = $"Версия ос: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                   $"Ядер процессора: {Environment.ProcessorCount}\n" +
                   $"64 битная система: {Environment.Is64BitProcess}\n";

        var gcMemoryInfo = GC.GetGCMemoryInfo();
        var installedMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
        var physicalMemory = installedMemory / 1048576.0;
        info += $"Ram: {physicalMemory}MB\n";

        var sendTo = "supuamtest@gmail.com";

        var login = "supuamtest@gmail.com";
        var password = "mxnb ggra bloc dfhk";

        using var emailMessage = new MimeMessage();

        message = email + "\n" + message;

        emailMessage.From.Add(new MailboxAddress("BugReporter", login));
        emailMessage.To.Add(new MailboxAddress("Support", sendTo));
        emailMessage.Subject = "Bug";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
        {
            Text = message + "\n" + info
        };
        
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587);
            await client.AuthenticateAsync(login, password);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }
    }
}