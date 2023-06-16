using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace EmailSenderHandlowcy;

public class AppSettings
{
    public static void Load()
    {
        var config = GetConfig();
        
        SQL.Server = config.GetSection("Settings:SQL:Server").Value;
        SQL.Baza = config.GetSection("Settings:SQL:Baza").Value;
        SQL.User = config.GetSection("Settings:SQL:User").Value;
        SQL.Pass = config.GetSection("Settings:SQL:Pass").Value;

        #if DEBUG
            SQL.ConnString = $"DATA SOURCE={SQL.Server};INITIAL CATALOG={SQL.Baza};User Id={SQL.User};Password={SQL.Pass};MultipleActiveResultSets=true";
        #else
            SQL.ConnString = $"DATA SOURCE={SQL.Server};INITIAL CATALOG={SQL.Baza};Integrated Security=true;Connection Timeout=300";
        #endif
        
        Email.Smtp = config.GetSection("Settings:EMAIL:Smtp").Value;
        Email.User = config.GetSection("Settings:EMAIL:User").Value;
        Email.Pass = config.GetSection("Settings:EMAIL:Pass").Value;
        Email.Port = Int32.Parse(config.GetSection("Settings:EMAIL:Port").Value);

    }
    
    private static IConfiguration GetConfig()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        return builder.Build();
    }

    public class SQL
    {
        public static string ConnString { get; set; }
        public static string Server { get; set; }
        public static string Baza { get; set; }
        public static string User { get; set; }
        public static string Pass { get; set; }
    }

    public class Email
    {
        public static string Smtp { get; set; }
        public static string User { get; set; }
        public static string Pass { get; set; }
        public static int Port { get; set; }
    }
}