namespace TestSite.DAL.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using Contract;

    public class RoleSqlStore : IRoleStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;

        public bool GiveRole(int userId, string roleName)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Role_GiveRole";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@roleName", roleName);

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        public ICollection<string> ListAllRoles()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Role_ListAllRoles";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                List<string> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<string>();
                }

                while (reader.Read())
                {
                    result.Add((string)reader["name"]);
                }

                return result;
            }
        }

        public ICollection<string> ListRolesForUser(string userName)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Role_ListRolesByUserName";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@user_name", userName);

                List<string> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<string>();
                }

                while (reader.Read())
                {
                    result.Add((string)reader["name"]);
                }

                return result;
            }
        }

        public ICollection<string> ListRolesForUserByUserId(int userId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Role_ListRolesByUserId";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@userId", userId);

                List<string> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<string>();
                }

                while (reader.Read())
                {
                    result.Add((string)reader["name"]);
                }

                return result;
            }
        }

        public bool PullOffRole(int userId, string roleName)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Role_PullOffRole";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@roleName", roleName);

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0;
            }
        }
    }
}
