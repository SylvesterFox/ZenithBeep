using Microsoft.Extensions.Configuration;


namespace ReworkZenithBeep.Data
{
    public static class DataConfig
    {
        public static string? InitDataConfig()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("DataStringToken.json")
                .Build();


            return config.GetConnectionString("PostgreSqlConnection");
        }
    }
}
