namespace TestSite.DAL.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class RoleSqliteStore : IRoleStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public bool GiveRole(int userId, string roleName)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                var query = "INSERT INTO employee_role (user_id, role_id) " +
                            "VALUES (:userId, (SELECT id FROM role WHERE name = :roleName))";

                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue(":userId", userId);
                    command.Parameters.AddWithValue(":roleName", roleName);

                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }
            }
        }

        public ICollection<string> ListAllRoles()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM role", connection))
                {
                    List<string> result = null;

                    using (var reader = command.ExecuteReader())
                    {
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
            }
        }

        public ICollection<string> ListRolesForUserByUserId(int userId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                var select = "SELECT role.name " +
                             "FROM role JOIN employee_role " +
                             "ON role.id = employee_role.role_id " +
                             "WHERE user_id = :userId";

                using (var command = new SQLiteCommand(select, connection))
                {
                    List<string> result = null;
                    command.Parameters.AddWithValue(":userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
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
            }
        }

        public bool PullOffRole(int userId, string roleName)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                var query = "DELETE FROM employee_role " +
                            "WHERE user_id = :userId AND role_id = (SELECT id FROM role WHERE name = :roleName)";

                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue(":userId", userId);
                    command.Parameters.AddWithValue(":roleName", roleName);

                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }  
            }
        }
    }
}
