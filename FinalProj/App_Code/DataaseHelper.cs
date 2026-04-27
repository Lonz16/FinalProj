using CanteenProject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

public static class DatabaseHelper
{
    public static string GetConnectionString()
    {
        return ConfigurationManager.ConnectionStrings["BorrowBoxDB"].ConnectionString;
    }

    public static List<User> GetAllUsers()
    {
        List<User> users = new List<User>();
        string connectionString = GetConnectionString();

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                User user = new User();
                user.UserID = Convert.ToInt32(reader["UserID"]);
                user.FullName = reader["FullName"].ToString();
                user.Email = reader["Email"].ToString();
                user.Role = reader["Role"].ToString();
                users.Add(user);
            }
        }
        return users;
    }
}