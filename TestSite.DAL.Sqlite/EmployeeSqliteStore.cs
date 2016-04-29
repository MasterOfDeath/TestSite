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
                             "enabled=:enabled " +
                             "WHERE id=:id";

                var insert = "INSERT INTO employee " +
                             "(dep_id, first_name, last_name, hash, enabled) " +
                             "VALUES (:dep_id, :first_name, :last_name, :hash, :enabled); " +
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

        private Employee RowToEmployee(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var dep_id = reader.GetInt32(1);
            var first_name = reader.GetString(2);
            var last_name = reader.GetString(3);
            var hash = (byte[])reader.GetValue(4);
            var enabled = reader.GetBoolean(5);

            return new Employee(id, dep_id, first_name, last_name, hash, enabled);
        }
    }
}
