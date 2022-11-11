using ClientSever.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ClientSever.DTO
{
    public class EmailSender
    {
        
        public async Task SendMailPdf(Account account)
        {
            var email = "";
            if(account != null)
            {
                email = account.Email;
            } else
            {
                email = "ducyb782001@gmail.com";
            }
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Trung Duc Backend", "noreply@test.com"));
            message.To.Add(new MailboxAddress("Email address", email));
            message.Subject = "Invoice Detail in E-shop";
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = @"
    <html>
        <body>
            <h1>Invoice Detail in E-shop</h1>
            <p>Hi! User" + account.Customer.ContactName + @"</p>
            <p>Below are you InvoiceDetail, you can download it to see detail.</p>
            <p>Thanks! :)</p>
        </body>
    </html>";
            bodyBuilder.Attachments.Add("C:\\Users\\trung duc\\Documents\\Semester 8\\PRN231\\InvoiceDetail.pdf");
            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync("ducybtest@gmail.com", "rwhryjpuqsqczpoz");
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
        public async Task ForgetPassword(string token, Account account)
        {
            var accountEmail = account.Email;
            var tokenLink = token;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Trung Duc Backend", "noreply@test.com"));
            message.To.Add(new MailboxAddress("Email address", accountEmail));
            message.Subject = "Invoice Detail File";
            var bodyBuilder = new BodyBuilder();
            var link = "http://localhost:5045/Welcome/ResetPassword/" +token;
            bodyBuilder.HtmlBody = @"
    <html>
        <body>
            <h1>Change password in E-shop</h1>
            <p>Hi! User" +account.Customer.ContactName +@"</p>
            <p>You have been clicked on forget password so we send you new password. You can click 
<strong>
<a href=`"+link
            +@"`>Here</a> 
</strong> 
to change your password.</p>
            <p>Thanks! :)</p>
        </body>
    </html>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync("ducybtest@gmail.com", "rwhryjpuqsqczpoz");
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
        public async Task SendEmail()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test From Address", "noreply@test.com"));
            message.To.Add(new MailboxAddress("Test Recipient", "ducybtest@gmail.com"));
            message.Subject = "Test Email Subject";
            var bodyBuilder = new BodyBuilder();

            bodyBuilder.TextBody = @"
    Hi!

    This is just a test email so feel free to ignore it!

    Thanks! :)";

            bodyBuilder.HtmlBody = @"
    <html>
        <body>
            <h1>Test Email</h1>
            <p>Hi!</p>
            <p>This is just a <strong>test email</strong> so feel free to ignore it!</p>
            <p>Thanks! :)</p>
        </body>
    </html>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.gmail.com", 587,MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync("ducybtest@gmail.com", "rwhryjpuqsqczpoz");
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
        }
    }
}
