using System.Net.Mail;

namespace IdentityApp.Helper
{
    public class EmailConfirmation
    {
        public static void SendEmail(string link, string email)
        {
            MailMessage mail = new MailMessage();

            SmtpClient smtpClient = new SmtpClient();

            mail.From = new MailAddress("fatihsaridag26@gmail.com");
            mail.To.Add(email);

            mail.Subject = $"www.sifresifirlama.com::Email doğrulama";
            mail.Body = "<h2>Email doğrulamak için lütfen aşağıdaki linke tıklayınız.</h2><hr/>";
            mail.Body += $"<a href='{link}'>Email  doğruama linki</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.EnableSsl = true;

            smtpClient.Credentials = new System.Net.NetworkCredential("fatihsaridag26@gmail.com", "vsziafjpokcmwwac");

            smtpClient.Send(mail);

        }


    }
}
