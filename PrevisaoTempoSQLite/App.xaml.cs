using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using PrevisaoTempoSQLite.Helpers;

namespace PrevisaoTempoSQLite
{
    public partial class App : Application
    {
        public static string apiKey {  get; private set; }

        static DbHelper db;

        public static DbHelper Database
        {
            get
            {
                if(db == null)
                {
                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db-sqlite-previsao.db3");

                    db = new DbHelper(path);
                }

                return db;
            }
        }

        public App()
        {
            InitializeComponent();


            //MainPage = new AppShell();
            MainPage = new NavigationPage(new Views.MainPage());

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .Build();

            apiKey = configuration["API_KEY"];

        }

    }
}
