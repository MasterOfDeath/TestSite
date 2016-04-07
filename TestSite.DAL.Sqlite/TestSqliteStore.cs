namespace TestSite.DAL.Sqlite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class TestSqliteStore : ITestStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public Test GetTestById(int testId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM test WHERE id=:testId", connection))
                {
                    command.Parameters.AddWithValue(":testId", testId);

                    using (var reader = command.ExecuteReader())
                    {
                        Test result = null;
                        reader.Read();

                        if (reader.HasRows)
                        {
                            result = this.RowToTest(reader);
                        }

                        return result;
                    }
                }
            }
        }

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

        public ICollection<Tuple<int, int>> ListCorrectAnswers(int testId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                var select = "SELECT question.id AS q_id, answer.id AS a_id " +
                             "FROM question JOIN answer " +
                             "ON question.id = answer.question_id " +
                             "WHERE test_id = :testId AND correct = 1";

                using (var command = new SQLiteCommand(select, connection))
                {
                    command.Parameters.AddWithValue(":testId", testId);
                    List<Tuple<int, int>> result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Tuple<int, int>>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(new Tuple<int, int>(reader.GetInt32(0), reader.GetInt32(1)));
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
