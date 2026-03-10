using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    public static class DbRepository
    {
        private static readonly string _connectionString = "Data Source=database.db";

        public static int AddOffice(string officeName, string officeAddress, int? countryId)
        {
            int result = 0;
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    // Enable foreign keys just in case
                    new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                    string query = @"INSERT INTO Poslovnice (Name, Address, CountryId)
                                VALUES (@name, @address, @countryId)";

                    using (var cmd = new SqliteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", officeName);
                        cmd.Parameters.AddWithValue("@address", officeAddress);
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

        public static int EditOffice(PoslovniceViewModel officeToEdit)
        {
            int result = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string query = $"UPDATE Poslovnice SET Name = @name, " +
                    $"Address = @address, " +
                    $"CountryId = @countryId WHERE Id = @id;";

                using var cmd = new SqliteCommand(query, connection);

                cmd.Parameters.AddWithValue("@name", officeToEdit.Name);
                cmd.Parameters.AddWithValue("@address", officeToEdit.Address);
                cmd.Parameters.AddWithValue("@countryId", officeToEdit.CountryId);
                cmd.Parameters.AddWithValue("@id", officeToEdit.Id);

                result = cmd.ExecuteNonQuery();

                connection.Close();
            }

            return result;
        }

        public static List<DrzaveViewModel> GetCountries()
        {
            var countries = new List<DrzaveViewModel>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string query = "SELECT Id, Name FROM Drzave ORDER BY Name ASC";

                using var cmd = new SqliteCommand(query, connection);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    countries.Add(new DrzaveViewModel
                    {
                        CountryId = reader.GetInt32(0),
                        CountryName = reader.GetString(1),
                    });
                }

                connection.Close();
            }

            return countries;
        }

        public static List<PoslovniceViewModel> GetOfficesPerCountry(int countryId)
        {
            var offices = new List<PoslovniceViewModel>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string queryOffices = $"SELECT Id, Name, Address, CountryId FROM Poslovnice WHERE CountryId = {countryId}";

                var cmd = new SqliteCommand(queryOffices, connection);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    offices.Add(new PoslovniceViewModel
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

                string queryDelete = $"DELETE FROM Poslovnice WHERE Id = @officeId;";

                var cmd = new SqliteCommand(queryDelete, connection);
                cmd.Parameters.AddWithValue("@officeId", officeId);

                result = cmd.ExecuteNonQuery();       

                cmd.Dispose();

                connection.Close();
            }

            return result;
        }

        public static ObservableCollection<DrzaveViewModel> GetCountriesWithExistingOffices()
        {
            var countriesDict = new Dictionary<int, DrzaveViewModel>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Enable foreign keys just in case
                new SqliteCommand("PRAGMA foreign_keys = ON;", connection).ExecuteNonQuery();

                string queryCountries = "SELECT d.Id AS CountryId, d.Name AS CountryName, p.Id AS OfficeId, p.Name AS OfficeName, p.Address AS OfficeAddress " +
                    "FROM Drzave d INNER JOIN Poslovnice p " +
                    "ON d.Id = p.CountryId " +
                    "ORDER BY d.Name ASC, p.Name ASC";

                var cmd = new SqliteCommand(queryCountries, connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int countryId = reader.GetInt32(reader.GetOrdinal("CountryId"));

                    if (!countriesDict.ContainsKey(countryId))
                    {
                        countriesDict[countryId] = new DrzaveViewModel
                        {
                            CountryId = countryId,
                            CountryName = reader.GetString(reader.GetOrdinal("CountryName")),
                            Poslovnice = new ObservableCollection<PoslovniceViewModel>()
                        };
                    }

                    countriesDict[countryId].Poslovnice.Add(new PoslovniceViewModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("OfficeId")),
                        Name = reader.GetString(reader.GetOrdinal("OfficeName")),
                        Address = reader.GetString(reader.GetOrdinal("OfficeAddress")),
                        CountryId = countryId
                    });
                }

                var countries = new ObservableCollection<DrzaveViewModel>(countriesDict.Values);

                cmd.Dispose();
                reader.Close();

                connection.Close();

                return countries;
            }
        }
    }
}
