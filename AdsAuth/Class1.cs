using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AdsAuth
{
    public class Class1
    {
        private string _connectionString;
        public Class1(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Ad> GetAds()
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Ads";
            connection.Open();
            List<Ad> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId=(int)reader["UserId"]
                });
            }

            return ads;
        }
        public List<Ad> GetAdsById(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Ads WHERE UserId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Ad> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId = (int)reader["UserId"]
                });
            }

            return ads;
        }
        public int AddAd(Ad ad)
        {
            SqlConnection conn = new(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO Ads (Title, PhoneNumber, Description, UserId) " +
                $"VALUES (@title, @phone, @description, @userid) " +
                "Select Scope_Identity()";
            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@phone", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@userid", ad.UserId);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }
        public void Delete(int id)
        {
            SqlConnection connection = new(_connectionString);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Ads WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }
        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
    }
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public int UserId { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
    //index, newad
}
