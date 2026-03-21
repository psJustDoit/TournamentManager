using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Windows;
using TournamentManager.Models;

namespace TournamentManager.Db
{
    public static class DbRepository
    {
        private static readonly string _connectionString = "Data Source=database.db";

        public static int AddOffice(string officeName, string? officeAddress, int? countryId)
        {
            int result = 0;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    // Enable foreign keys just in case
                    new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                    string query = @"INSERT INTO Offices (Name, Address, CountryId) VALUES (@name, @address, @countryId)";

                    using (var cmd = new SqliteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", officeName);
                        cmd.Parameters.AddWithValue("@address", officeAddress ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@countryId", countryId);

                        result = cmd.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        public static int EditOffice(Office officeToEdit)
        {
            int result = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string query = $"UPDATE Offices SET Name = @name, " +
                    $"Address = @address, " +
                    $"CountryId = @countryId WHERE Id = @id;";

                using var cmd = new SqliteCommand(query, connection);

                cmd.Parameters.AddWithValue("@name", officeToEdit.Name);
                cmd.Parameters.AddWithValue("@address", officeToEdit.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@countryId", officeToEdit.CountryId);
                cmd.Parameters.AddWithValue("@id", officeToEdit.Id);

                result = cmd.ExecuteNonQuery();

                connection.Close();
            }

            return result;
        }

        public static List<Country> GetCountries()
        {
            var countries = new List<Country>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string query = "SELECT Id, Name FROM Countries ORDER BY Name ASC";

                using var cmd = new SqliteCommand(query, connection);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add(new Country
                    {
                        CountryId = reader.GetInt32(0),
                        CountryName = reader.GetString(1),
                    });
                }

                connection.Close();
            }

            return countries;
        }

        public static List<Office> GetOfficesPerCountry(int countryId)
        {
            var offices = new List<Office>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string queryOffices = $"SELECT Id, Name, Address, CountryId FROM Offices WHERE CountryId = {countryId}";

                var cmd = new SqliteCommand(queryOffices, connection);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    offices.Add(new Office
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Address = reader.GetString(2),
                        CountryId = reader.GetInt32(3),
                    });
                }

                cmd = new SqliteCommand(queryOffices, connection);
                reader = cmd.ExecuteReader();

                cmd.Dispose();
                reader.Close();

                connection.Close();
            }

            return offices;
        }

        public static int DeleteOffice(int officeId)
        {
            int result = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string queryDelete = $"DELETE FROM Offices WHERE Id = @officeId;";

                var cmd = new SqliteCommand(queryDelete, connection);
                cmd.Parameters.AddWithValue("@officeId", officeId);

                result = cmd.ExecuteNonQuery();       

                cmd.Dispose();

                connection.Close();
            }

            return result;
        }

        public static ObservableCollection<Country> GetCountriesWithExistingOffices()
        {
            var countriesDict = new Dictionary<int, Country>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string queryCountries = "SELECT c.Id AS CountryId, c.Name AS CountryName, o.Id AS OfficeId, o.Name AS OfficeName, o.Address AS OfficeAddress " +
                    "FROM Countries c INNER JOIN Offices o " +
                    "ON c.Id = o.CountryId " +
                    "ORDER BY c.Name ASC, o.Name ASC";

                var cmd = new SqliteCommand(queryCountries, connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int countryId = reader.GetInt32(reader.GetOrdinal("CountryId"));

                    if (!countriesDict.ContainsKey(countryId))
                    {
                        countriesDict[countryId] = new Country
                        {
                            CountryId = countryId,
                            CountryName = reader.GetString(reader.GetOrdinal("CountryName")),
                            Offices = new ObservableCollection<Office>()
                        };
                    }

                    countriesDict[countryId].Offices.Add(new Office
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("OfficeId")),
                        Name = reader.GetString(reader.GetOrdinal("OfficeName")),
                        Address = reader.IsDBNull(reader.GetOrdinal("OfficeAddress")) ? null : reader.GetString(reader.GetOrdinal("OfficeAddress")),
                        CountryId = countryId
                    });
                }

                var countries = new ObservableCollection<Country>(countriesDict.Values);

                cmd.Dispose();
                reader.Close();

                connection.Close();

                return countries;
            }
        }

        public static List<Office> GetAllOffices()
        {
            var offices = new List<Office>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string queryOffices = $"SELECT * FROM Offices";

                var cmd = new SqliteCommand(queryOffices, connection);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    offices.Add(new Office
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                        CountryId = reader.GetInt32(reader.GetOrdinal("CountryId")),
                    });
                }

                cmd = new SqliteCommand(queryOffices, connection);
                reader = cmd.ExecuteReader();

                cmd.Dispose();
                reader.Close();

                connection.Close();
            }

            return offices;
        }
    }
}
