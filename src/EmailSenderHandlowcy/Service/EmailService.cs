using System.Text;
using EmailSenderHandlowcy.Builder;
using EmailSenderHandlowcy.Models;
using ENSOFT_Utils;

namespace EmailSenderHandlowcy.Service;

public class EmailService
{
    private readonly IDictionary<User, List<Dok>> _doks;
    public EmailService(IDictionary<User,List<Dok>> doks)
    {
        _doks = doks;
    }

    public async void SendEmails()
    {
        foreach (var dok in _doks)
        {
            if (dok.Key.Email != "brak")
            {
                RepoService repo = new RepoService();
            
                var mailBody = new StringBuilder();
            
                mailBody.Append($@"Przyszłe realizacje zamówień:<br><br>");
            
                mailBody.Append($@"<table cellpadding=6 border=1><tbody><tr><td align=center valign=center>Numer Zamówienia</td><td  align=center valign=center>Data Realizacji</td></tr>");
            
                dok.Value.ForEach(x =>
                {
                    var nazwaDok = repo.GetDokName(x.ZaNNumer);
                    var dataRealizacji = ENSOFT_Utils.Tools.ClarionToDate(x.DataRealizacji);
                
                    mailBody.Append($@"<tr><td  align=center valign=center>{nazwaDok}</td><td  align=center valign=center>{dataRealizacji}</td></tr>");
                });
            
                mailBody.Append("</tbody></table>");

                mailBody.Append("<br><br>Mail wygenerowany automatycznie.");
            
                EmailBuilder emailBuilder = new EmailBuilder();
            
                var score = emailBuilder
                    .DodajTytul("Zaplanowane wysyłki (AUTOMAT)")
                    .DodajAdresata(dok.Key.Email)
                    .DodajTresc(mailBody.ToString())
                    .Wyslij();

                if (score.Stan == 0)
                    Tools.WriteLog($"[OK] Email do {dok.Key.Email} poprawnie wysłany!", true, true, ConsoleColor.Green);
                else
                    Tools.WriteLog($"[ERROR] Błąd wysyłania wiadomości do {dok.Key.Email}", true, true, ConsoleColor.Red);
            }
            else
            {
                Tools.WriteLog($"[ERROR] Błąd wysyłania wiadomości do {dok.Key.OpeIdent} (brak adresu email)", true, true, ConsoleColor.Red);   
            }
        }
    }
}