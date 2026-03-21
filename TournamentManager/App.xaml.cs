using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string connectionString = "Data Source=database.db";
        public static ServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Alter database code
            //AlterDatabase();

            // Database setup
            SetupDatabase();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register ViewModels
            services.AddSingleton<TournamentViewModel>();
            services.AddSingleton<RoundHistoryViewModel>();

            // Register pages
            services.AddSingleton<MainWindow>();
            services.AddTransient<Offices>();
            services.AddTransient<Teams>();
            services.AddTransient<Tournament>();
            services.AddTransient<RoundHistory>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public void AlterDatabase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();// creates the DB file if missing

                // Enable foreign key enforcement
                using (var cmd = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Query to alter the db
                string changeOffices = "DROP TABLE Offices;";
                string changeRounds = "DROP TABLE Rounds";
                string changeTeams = "DROP TABLE Teams";

                using (var cmd = new SqliteCommand(changeOffices, connection))
                { cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand(changeRounds, connection))
                { cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand(changeTeams, connection))
                { cmd.ExecuteNonQuery(); }
            }
        }

        private void SeedCountries(SqliteConnection connection)
        {
            string check = "SELECT COUNT(Id) FROM Countries;";
            long count = (long)new SqliteCommand(check, connection).ExecuteScalar();

            if (count == 0)
            {
                string insert = @"
                    INSERT INTO Countries (Id, Name) VALUES
                    ('1', 'Afghanistan'), ('2', 'Albania'), ('3', 'Algeria'), ('4', 'Andorra'), ('5', 'Angola'), ('6', 'Antigua and Barbuda'), ('7', 'Argentina'), 
                    ('8', 'Armenia'), ('9', 'Australia'),('10', 'Austria'), 
                    ('11', 'Azerbaijan'), ('12', 'Bahamas'), ('13', 'Bahrain'),('14', 'Bangladesh'), ('15', 'Barbados'), ('16', 'Belarus'), ('17', 'Belgium'), ('18', 'Belize'), ('19', 'Benin'), ('20', 'Bhutan'), 
                    ('21', 'Bolivia'), ('22', 'Bosnia and Herzegovina'), ('23', 'Botswana'), ('24', 'Brazil'), ('25', 'Brunei'), ('26', 'Bulgaria'), ('27', 'Burkina Faso'), 
                    ('28', 'Burundi'), ('29', 'Cabo Verde'), ('30', 'Cambodia'), ('31', 'Cameroon'), ('32', 'Canada'), ('33', 'Central African Republic'), ('34', 'Chad'), ('35', 'Chile'), 
                    ('36', 'China'), ('37', 'Colombia'), ('38', 'Comoros'), ('39', 'Democratic Republic of the Congo'), ('40', 'Costa Rica'), ('41', 'Croatia'), 
                    ('42', 'Cuba'), ('43', 'Cyprus'), ('44', 'Czechia'), ('45', 'Denmark'), ('46', 'Djibouti'), ('47', 'Dominica'), ('48', 'Dominican Republic'), ('49', 'Ecuador'), 
                    ('50', 'Egypt'), ('51', 'El Salvador'), ('52', 'Equatorial Guinea'), ('53', 'Eritrea'), ('54', 'Estonia'), ('55', 'Eswatini'), ('56', 'Ethiopia'), ('57', 'Fiji'), 
                    ('58', 'Finland'), ('59', 'France'), ('60', 'Gabon'), ('61', 'Gambia'), ('62', 'Georgia'), ('63', 'Germany'), ('64', 'Ghana'), ('65', 'Greece'), ('66', 'Grenada'), 
                    ('67', 'Guatemala'), ('68', 'Guinea'), ('69', 'Guinea-Bissau'), ('70', 'Guyana'), ('71', 'Haiti'), ('72', 'Honduras'), ('73', 'Hungary'), ('74', 'Iceland'), ('75', 'India'),
                    ('76', 'Indonesia'), ('77', 'Iran'), ('78', 'Iraq'), ('79', 'Ireland'), ('80', 'Israel'), ('81', 'Italy'), ('82', 'Jamaica'), ('83', 'Japan'), ('84', 'Jordan'), 
                    ('85', 'Kazakhstan'), ('86', 'Kenya'), ('87', 'Kiribati'), ('88', 'Kuwait'), ('89', 'Kyrgyzstan'), ('90', 'Laos'), ('91', 'Latvia'), ('92', 'Lebanon'), ('93', 'Lesotho'), 
                    ('94', 'Liberia'), ('95', 'Libya'), ('96', 'Liechtenstein'), ('97', 'Lithuania'), ('98', 'Luxembourg'), ('99', 'Madagascar'), ('100', 'Malawi'), ('101', 'Malaysia'), 
                    ('102', 'Maldives'), ('103', 'Mali'), ('104', 'Malta'), ('105', 'Marshall Islands'), ('106', 'Mauritania'), ('107', 'Mauritius'), ('108', 'Mexico'), ('109', 'Micronesia'),
                    ('110', 'Moldova'), ('111', 'Monaco'), ('112', 'Mongolia'), ('113', 'Montenegro'), ('114', 'Morocco'), ('115', 'Mozambique'), ('116', 'Myanmar'), ('117', 'Namibia'), 
                    ('118', 'Nauru'), ('119', 'Nepal'), ('120', 'Netherlands'), ('121', 'New Zealand'), ('122', 'Nicaragua'), ('123', 'Niger'), ('124', 'Nigeria'), ('125', 'North Korea'), 
                    ('126', 'North Macedonia'), ('127', 'Norway'), ('128', 'Oman'), ('129', 'Pakistan'), ('130', 'Palau'), ('131', 'Palestine'), ('132', 'Panama'), ('133', 'Papua New Guinea'), 
                    ('134', 'Paraguay'), ('135', 'Peru'), ('136', 'Philippines'), ('137', 'Poland'), ('138', 'Portugal'), ('139', 'Qatar'), ('140', 'Romania'), ('141', 'Russia'), 
                    ('142', 'Rwanda'), ('143', 'Saint Kitts and Nevis'), ('144', 'Saint Lucia'), ('145', 'Saint Vincent and the Grenadines'), ('146', 'Samoa'), ('147', 'San Marino'), 
                    ('148', 'Sao Tome and Principe'), ('149', 'Saudi Arabia'), ('150', 'Senegal'), ('151', 'Serbia'), ('152', 'Seychelles'), ('153', 'Sierra Leone'), ('154', 'Singapore'), 
                    ('155', 'Slovakia'), ('156', 'Slovenia'), ('157', 'Solomon Islands'), ('158', 'Somalia'), ('159', 'South Africa'), ('160', 'South Korea'), ('161', 'South Sudan'), 
                    ('162', 'Spain'), ('163', 'Sri Lanka'), ('164', 'Sudan'), ('165', 'Suriname'), ('166', 'Sweden'), ('167', 'Switzerland'), ('168', 'Syria'), ('169', 'Taiwan'), 
                    ('170', 'Tajikistan'), ('171', 'Tanzania'), ('172', 'Thailand'), ('173', 'Timor-Leste'), ('174', 'Togo'), ('175', 'Tonga'), ('176', 'Trinidad and Tobago'), ('177', 'Tunisia'), 
                    ('178', 'Turkey'), ('179', 'Turkmenistan'), ('180', 'Tuvalu'), ('181', 'Uganda'), ('182', 'Ukraine'), ('183', 'United Arab Emirates'), ('184', 'United Kingdom'), 
                    ('185', 'United States'), ('186', 'Uruguay'), ('187', 'Uzbekistan'), ('188', 'Vanuatu'), ('190', 'Vatican City'), ('191', 'Venezuela'), ('192', 'Vietnam'), ('193', 'Yemen'), 
                    ('194', 'Zambia'), ('195', 'Zimbabwe');";
                new SqliteCommand(insert, connection).ExecuteNonQuery();
            }
        }

        private void SetupDatabase()
        {          
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();// creates the DB file if missing

                // Enable foreign key enforcement
                using (var cmd = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    cmd.ExecuteNonQuery();
                }

                string createOfficesTable = @"
                    CREATE TABLE IF NOT EXISTS Offices (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Address TEXT,
                        CountryId INTEGER,
                        FOREIGN KEY(CountryId) REFERENCES Countries(Id)
                    );";

                string createCountriesTable = @"
                    CREATE TABLE IF NOT EXISTS Countries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL
                    );";

                string createTeamsTable = @"
                    CREATE TABLE IF NOT EXISTS Teams (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        DateCreated TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        OfficeId INTEGER NOT NULL,
                        FOREIGN KEY(OfficeId) REFERENCES Offices(Id)
                    );";

                string createTournamentsHistoryTable = @"
                    CREATE TABLE IF NOT EXISTS TournamentsHistory (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GameName INTEGER,
                        StartDate TEXT NOT NULL,
                        EndDate TEXT NOT NULL
                    );";

                string createLogsTable = @"
                    CREATE TABLE IF NOT EXISTS Logs (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DateTimeOccurred TEXT,
                        ClassOccurred TEXT
                        GameName TEXT,
                        TournamentId INTEGER,
                        LogMessage TEXT NOT NULL
                    );";

                using (var cmd = new SqliteCommand(createOfficesTable, connection))
                { cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand(createCountriesTable, connection))
                { cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand(createTeamsTable, connection))
                { cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand(createTournamentsHistoryTable, connection))
                { cmd.ExecuteNonQuery(); }

                using (var cmd = new SqliteCommand(createLogsTable, connection))
                { cmd.ExecuteNonQuery(); }

                SeedCountries(connection);

            }
        }
    }

}
