using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ChessWPF.Data
{
    public static class DataAccess
    {
        private static string ConnString =>
            ConfigurationManager.ConnectionStrings["ChessDb"].ConnectionString;

        public static bool RegisterUser(string username, string email, string password)
        {
            // Hash password
            string hash = ComputeSHA256(password);
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            string sql = "INSERT INTO users (username, email, password_hash) VALUES (@u,@e,@p)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", hash);
            try
            {
                return cmd.ExecuteNonQuery() == 1;
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                // duplicate key
                return false;
            }
        }

        public static bool AuthenticateUser(string email, string password)
        {
            string hash = ComputeSHA256(password);
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            string sql = "SELECT COUNT(*) FROM users WHERE email=@e AND password_hash=@p";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", hash);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        private static string ComputeSHA256(string input)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
