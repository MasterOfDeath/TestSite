namespace TestSite.DAL.Sqlite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class EmployeeSqliteStore : IEmployeeStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public Employee GetEmployeeById(int employeeId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var select = "SELECT * FROM employee WHERE id=:id";

                using (var command = new SQLiteCommand(select, connection))
                {
                    command.Parameters.AddWithValue(":id", employeeId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }

                        reader.Read();
                        var result = this.RowToEmployee(reader);

                        return result;
                    }
                }
            }
        }

        public int InsertEmployee(Employee employee)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command;
                int result = -1;

                connection.Open();

                var update = "UPDATE employee SET " +
                             "dep_id=:dep_id, " +
                             "first_name=:first_name, " +
                             "last_name=:last_name, " +
                             "hash=:hash, " +
                             "enabled=:enabled, " +
                             "role_id=:role_id " +
                             "WHERE id=:id";

                var insert = "INSERT INTO employee " +
                             "(dep_id, first_name, last_name, hash, enabled, role_id) " +
                             "VALUES (:dep_id, :first_name, :last_name, :hash, :enabled, :role_id); " +
                             "SELECT last_insert_rowid() AS id;";

                if (employee.Id > 0)
                {
                    using (command = new SQLiteCommand(update, connection))
                    {
                        command.Parameters.AddWithValue(":id", employee.Id);
                        command.Parameters.AddWithValue(":dep_id", employee.Dep_Id);
                        command.Parameters.AddWithValue(":first_name", employee.FirstName);
                        command.Parameters.AddWithValue(":last_name", employee.LastName);
                        command.Parameters.AddWithValue(":hash", employee.Hash);
                        command.Parameters.AddWithValue(":enabled", employee.Enabled);
                        command.Parameters.AddWithValue(":role_id", employee.Role_Id);

                        result = (command.ExecuteNonQuery() > 0) ? employee.Id : -1;
                    }
                }
                else
                {
                    using (command = new SQLiteCommand(insert, connection))
                    {
                        command.Parameters.AddWithValue(":dep_id", employee.Dep_Id);
                        command.Parameters.AddWithValue(":first_name", employee.FirstName);
                        command.Parameters.AddWithValue(":last_name", employee.LastName);
                        command.Parameters.AddWithValue(":hash", employee.Hash);
                        command.Parameters.AddWithValue(":enabled", employee.Enabled);
                        command.Parameters.AddWithValue(":role_id", employee.Role_Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = reader.GetInt32(0); ;
                            }
                            else
                            {
                                result = -1;
                            }
                        }
                    }
                }

                return result;
            }
        }

        public ICollection<Employee> ListAllEmployees()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                var select = "SELECT * FROM employee";
                connection.Open();

                using (var command = new SQLiteCommand(select, connection))
                {
                    List<Employee> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Employee>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToEmployee(reader));
                        }

                        return result;
                    }  
                } 
            }
        }

        public ICollection<Employee> ListEmployeesByDepId(int depId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var select = "SELECT * FROM employee WHERE dep_id=:depId";

                using (var command = new SQLiteCommand(select, connection))
                {
                    command.Parameters.AddWithValue(":depId", depId);
                    List<Employee> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Employee>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToEmployee(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public ICollection<Employee> ListEmployeesByRoleName(string roleName)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var select = "SELECT employee.* FROM employee_role " +
                             "JOIN employee " +
                             "ON employee.id = employee_role.user_id " +
                             "JOIN role " +
                             "ON role.id = employee_role.role_id " +
                             "WHERE role.name = :roleName";

                using (var command = new SQLiteCommand(select, connection))
                {
                    command.Parameters.AddWithValue(":roleName", roleName);
                    List<Employee> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Employee>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToEmployee(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public bool RemoveEmployee(int employeeId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var delete = "PRAGMA foreign_keys = ON; DELETE FROM employee WHERE id=:id";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue(":id", employeeId);
                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }   
            }
        }

        public ICollection<string> ListRolesForUserByUserId(int employeeId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                var select = "SELECT employee.id, role.name " +
                             "FROM employee " +
                             "JOIN role ON role.id = employee.role_id " +
                             "WHERE employee.id = :employeeId";

                using (var command = new SQLiteCommand(select, connection))
                {
                    List<string> result = null;
                    command.Parameters.AddWithValue(":employeeId", employeeId);

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

        private Employee RowToEmployee(SQLiteDataReader reader)
        {
            var id = (int)(long)reader["id"];
            var dep_id = (int)(long)reader["dep_id"];
            var first_name = (string)reader["first_name"];
            var last_name = (string)reader["last_name"];
            var hash = (byte[])reader["hash"];
            var enabled = Convert.ToBoolean((long)reader["enabled"]);
            var role_id = (int)(long)reader["role_id"];

            return new Employee(id, dep_id, first_name, last_name, hash, enabled, role_id);
        }
    }
}
