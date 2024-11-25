using MAUIAppTempoAgora.Helpers;
using System.Diagnostics;

namespace MAUIAppTempoAgora
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();


            if (_db != null)
            {
                Debug.WriteLine("Ja foi criado jao");
            }
            else
            {
                Debug.WriteLine("Não foi criado jao");
            }

        }

        static SQLiteDatabaseHelper _db;

        public static SQLiteDatabaseHelper Db
        {
            get
            {
                if (_db == null)
                {
                    string path = Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData
                            ), "banco_sqlite_tempo.db3"
                    );

                    _db = new SQLiteDatabaseHelper(path);
                }
                
                return _db;
            }

        }
    }
}
