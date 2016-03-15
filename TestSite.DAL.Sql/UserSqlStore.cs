namespace TestSite.DAL.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Contract;
    using Entites;

    public class UserSqlStore : IUserStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;

        public bool AddUser(User user)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "User_AddUser";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@first_name", user.FirstName);
                command.Parameters.AddWithValue("@last_name", user.LastName);
                command.Parameters.AddWithValue("@user_name", user.UserName);
                command.Parameters.AddWithValue("@hash", user.Hash);
                command.Parameters.AddWithValue("@enabled", user.Enabled);
                command.Parameters.AddWithValue("@tariff_id", user.Tariff_Id);

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user.Id = (int)(decimal)reader["newId"];
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public User GetUserById(int userId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "User_GetUserById";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", userId);

                User user = null;

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = this.RowToUser(reader);
                }

                return user;
            }
        }

        public User GetUserByUserName(string userName)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "User_GetUserByUserName";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@user_name", userName);

                User user = null;

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user = this.RowToUser(reader);
                }

                return user;
            }
        }

        public bool InsertUser(User user)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "User_InsertUser";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@first_name", user.FirstName);
                command.Parameters.AddWithValue("@last_name", user.LastName);
                command.Parameters.AddWithValue("@user_name", user.UserName);
                command.Parameters.AddWithValue("@hash", user.Hash);
                command.Parameters.AddWithValue("@enabled", user.Enabled);
                command.Parameters.AddWithValue("@tariff_id", user.Tariff_Id);

                connection.Open();
                var reader = command.ExecuteNonQuery();

                return reader > 0;
            }
        }

        public ICollection<User> ListAllUsers()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "User_ListAllUsers";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                List<User> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<User>();
                }

                while (reader.Read())
                {
                    result.Add(this.RowToUser(reader));
                }

                return result;
            }
        }

        public ICollection<User> ListUsersByRoleName(string roleName)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "User_ListUserByRoleName";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@role_name", roleName);

                List<User> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<User>();
                }

                while (reader.Read())
                {
                    result.Add(this.RowToUser(reader));
                }

                return result;
            }
        }

        public bool RemoveUser(int userId)
        {
            throw new NotImplementedException();
        }

        private User RowToUser(SqlDataReader reader)
        {
            var id = (int)reader["id"];
            var firstName = (string)reader["first_name"];
            var lastName = (string)reader["last_name"];
            var userName = (string)reader["user_name"];
            var hash = (byte[])reader["hash"];
            var enabled = (bool)reader["enabled"];
            var tariff_id = (int)reader["tariff_id"]; 

            return new User(id, firstName, lastName, userName, hash, tariff_id, enabled);
        }
    }
}
