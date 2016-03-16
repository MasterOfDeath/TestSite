namespace TestSite.DAL.Sqlite
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class TestSqliteStore : ITestStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public bool InsertTest(Test test)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command;
                connection.Open();
                int result;

                if (test.Id > 0)
                {
                    using (command = new SQLiteCommand("UPDATE test SET name=:name WHERE id=:id", connection))
                    {
                        command.Parameters.AddWithValue(":id", test.Id);
                        command.Parameters.AddWithValue(":name", test.Name);
                        result = command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (command = new SQLiteCommand("INSERT INTO test (name) VALUES (:name)", connection))
                    {
                        command.Parameters.AddWithValue(":name", test.Name);
                        result = command.ExecuteNonQuery();
                    }
                }

                return result > 0;
            }
        }

        public ICollection<Test> ListAllTests()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM test", connection))
                {
                    List<Test> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Test>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToTest(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public bool RemoveTest(int testId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var delete = "PRAGMA foreign_keys = ON; DELETE FROM test WHERE id=:id;";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue(":id", testId);
                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }                    
            }
        }

        private Test RowToTest(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);

            return new Test(id, name);
        }
    }
}
