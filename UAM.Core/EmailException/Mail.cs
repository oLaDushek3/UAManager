using System.ComponentModel.DataAnnotations;
using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using UAM.Core.Entities;

namespace UAM.Core.EmailException;

public class Mail
{
    public static async Task ReadEmails()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var login = "supuamtest@gmail.com";
        var password = "mxnb ggra bloc dfhk";

        using (var client = new ImapClient())
        {
            await client.ConnectAsync("imap.gmail.com", 993);
            await client.AuthenticateAsync(login, password);

            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);
            var uids = inbox.Search (SearchQuery.NotSeen);
            
            if (uids.Count == 0)
            {
                Console.WriteLine("Сообщения не найдены");
                return;
            }
            var unread = uids.First();

            var message = await inbox.GetMessageAsync(uids.First());
            
            if (message.Subject == "Bug")
            {
                string? email = null;

                var emailTry = message.TextBody.Split('\n')[0];
                if (new EmailAddressAttribute().IsValid(emailTry))
                {
                    email = emailTry;
                }

                await using UaVersionsContext uaVersionsContext = new UaVersionsContext();

                var work = new Problem() { ProblemText = message.TextBody, Email = email, StatusId = 1 };
                uaVersionsContext.Problems.Add(work);
                await uaVersionsContext.SaveChangesAsync();

                await inbox.AddFlagsAsync(unread, MessageFlags.Seen, true);

                if (email != null)
                {
                    await SendEmailAsync(email, "Задача зарегистрирована");
                }

                Console.WriteLine(string.Concat(Enumerable.Repeat("-", 64)));
                Console.WriteLine("Получено сообщение");
                Console.WriteLine(message.Subject);
                Console.WriteLine(message.TextBody);
                Console.WriteLine(string.Concat(Enumerable.Repeat("-", 64)));
                
            }
            else
            {
                await inbox.AddFlagsAsync(unread, MessageFlags.Seen, true);
            }

            await client.DisconnectAsync(true);
        }
    }

    public static async Task SendEmailAsync(string email, string text)
    {
        var login = "supuamtest@gmail.com";
        var password = "mxnb ggra bloc dfhk";

        using var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress("Support", login));
        emailMessage.To.Add(new MailboxAddress("User", email));
        emailMessage.Subject = "Bug";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
        {
            Text = text
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