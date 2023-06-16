using System.Collections.Immutable;
using System.Data.SqlClient;
using Dapper;
using EmailSenderHandlowcy.Models;
using ENSOFT_Utils;

namespace EmailSenderHandlowcy.Service;

public class RepoService
{
    public async Task<IDictionary<User, List<Dok>>> GetData()
    {
        IDictionary<User, List<Dok>> emailData = new Dictionary<User, List<Dok>>();

        IEnumerable<DocumentsToSent> documentsToSent;

        using (SqlConnection connection  = new SqlConnection(AppSettings.SQL.ConnString))
        {
            documentsToSent = await connection.QueryAsync<DocumentsToSent>($@"select es.*, ope.Ope_Ident as OpeIdent, case 
	                                                                                when Ope_UserOutlook = '' then 'brak'
	                                                                                else Ope_UserOutlook
                                                                                end as Email from cdn.ENSOFT_EmailSender_Handlowcy_Data es
                                                                                inner join cdn.OpeKarty ope on ope.Ope_GIDNumer=es.OpeNumer");
        }

        if (documentsToSent == null)
            return null;
        
        documentsToSent.Select(x => new User()
        {
            OpeIdent = x.OpeIdent,
            OpeNumer = x.OpeNumer,
            Email = x.Email
        }).DistinctBy(y=>y.OpeNumer).ToList().ForEach(f =>
        {
            List<Dok> doks = new List<Dok>();

            doks = documentsToSent.Where(x => x.OpeNumer == f.OpeNumer).ToList().Select(y =>
                new Dok()
                {
                    Id = y.Id,
                    DataRealizacji = y.DataRealizacji,
                    ZaNNumer = y.ZaNNumer
                }).ToList();
            
            if(doks.Count == 0)
                return;
            
            emailData.Add(f, doks);
        });

        return emailData;
    }

    public string GetDokName(int numer)
    {
        string score = "";
        using (SqlConnection connection  = new SqlConnection(AppSettings.SQL.ConnString))
        {
            score = connection.Query<string>($"select cdn.nazwaobiektu(960, {numer}, 0, 2)").First();
        }

        return score;
    }

    public int DeleteDataFromSQL()
    {
        int affectedRows = 0;
            
        using (var connection= new SqlConnection(AppSettings.SQL.ConnString))
        {
            affectedRows = connection.Execute($@"delete from cdn.ENSOFT_EmailSender_Handlowcy_Data");
        }
            
        if (affectedRows == 0)
            Tools.WriteLog("[ERROR] Błąd podczas usuwania danych z SQL!", true, true, ConsoleColor.Red);

        return affectedRows;
    }
    
    public bool CheckExist()
    {
        int ilosc = 0;
            
        using (var connection= new SqlConnection(AppSettings.SQL.ConnString))
        {
            ilosc = connection.ExecuteScalar<int>("select count(*) from cdn.ENSOFT_EmailSender_Handlowcy_Data");
        }

        if (ilosc == 0)
            return false;


        return true;
    }
}