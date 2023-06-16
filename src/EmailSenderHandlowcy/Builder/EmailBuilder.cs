using System.Net;
using System.Net.Mail;
using EmailSenderHandlowcy.Models;
using ENSOFT_Utils;

namespace EmailSenderHandlowcy.Builder;

public class EmailBuilder
{
    private Email _email = new Email() {Stan = -99};

    public Email Wyslij()
    {
        using (SmtpClient client = new SmtpClient(AppSettings.Email.Smtp, AppSettings.Email.Port))
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("x"),
                Subject = _email.Tytul,
                Body = _email.Tresc,
                IsBodyHtml = true,
            };

             
            #if DEBUG
                mailMessage.To.Add("y");
            #else
                mailMessage.To.Add(_email.DoKogo);
            #endif
            
            
            try
            {
                client.Send(mailMessage);

                _email.Stan = 0;
            }
            catch(Exception ex)
            {
                _email.Stan = -1;
                Tools.WriteLog(ex.Message, true, true, ConsoleColor.Red);
            }
                

            mailMessage.Dispose();
        }
        
        
        return _email;
    }

    public EmailBuilder DodajTresc(string wartosc)
    {
        _email.Tresc = wartosc;

        return this;
    }

    public EmailBuilder DodajTytul(string wartosc)
    {
        _email.Tytul = wartosc;

        return this;
    }

    public EmailBuilder DodajAdresata(string email)
    {
        _email.DoKogo = email;

        return this;
    }
}