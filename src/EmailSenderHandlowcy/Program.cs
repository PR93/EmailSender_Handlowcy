using EmailSenderHandlowcy;
using EmailSenderHandlowcy.Service;
using ENSOFT_Utils;

AppSettings.Load();

if (!Tools.CheckConnection(AppSettings.SQL.ConnString))
{
    Tools.WriteLog("[ERROR] Błąd połączenia SQL", true, true, ConsoleColor.Red);
    
    return;
}

RepoService repoService = new RepoService();

if (!repoService.CheckExist())
{
    Tools.WriteLog("[LOG] Brak danych do wysłania", true, true, ConsoleColor.Gray);
    return;
}

var doks = await repoService.GetData();

EmailService emailService = new EmailService(doks);
emailService.SendEmails();

repoService.DeleteDataFromSQL();

#if DEBUG
    Console.ReadKey();
#endif