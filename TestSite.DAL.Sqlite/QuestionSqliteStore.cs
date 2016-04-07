namespace TestSite.DAL.Sqlite
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class QuestionSqliteStore : IQuestionStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public byte[] GetImage(int questionId)
        {
            byte[] result;

            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand("SELECT * FROM question_image", connection))
                {
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();

                        try
                        {
                            result = (byte[])reader.GetValue(1);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        return result;
                    }
                }
            }
        }

        public Question GetQuestionById(int questionId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand("SELECT * FROM question WHERE id=:id", connection))
                {
                    command.Parameters.AddWithValue(":id", questionId);
                    Question result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = this.RowToQuestion(reader);
                        }
                        else
                        {
                            result = null;
                        }


                        return result;
                    }
                }
            }
        }

        public int InsertQuestion(Question question)
        {
            int result = -1;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var insert = "INSERT INTO question (name, test_id, type) VALUES (:name, :testId, :type); " + 
                             "SELECT last_insert_rowid() AS id;";
                var update = "UPDATE question SET name=:name, test_id=:testId, type=:type WHERE id=:id";

                if (question.Id > 0)
                {
                    using (var command = new SQLiteCommand(update, connection))
                    {
                        command.Parameters.AddWithValue(":id", question.Id);
                        command.Parameters.AddWithValue(":name", question.Name);
                        command.Parameters.AddWithValue(":testId", question.TestId);
                        command.Parameters.AddWithValue(":type", question.Type);
                        result = (command.ExecuteNonQuery() > 0) ? question.Id : -1;
                    }
                }
                else
                {
                    using (var command = new SQLiteCommand(insert, connection))
                    {
                        command.Parameters.AddWithValue(":name", question.Name);
                        command.Parameters.AddWithValue(":testId", question.TestId);
                        command.Parameters.AddWithValue(":type", question.Type);

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
            }

            return result;
        }

        public ICollection<Question> ListAllQuestions()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand("SELECT * FROM question", connection))
                {
                    List<Question> result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Question>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToQuestion(reader));
                        }

                        return result;
                    }
                } 
            }
        }

        public ICollection<Question> ListQuestionsByTestId(int testId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteCommand("SELECT * FROM question WHERE test_id=:testId", connection))
                {
                    command.Parameters.AddWithValue(":testId", testId);
                    List<Question> result = null;

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Question>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToQuestion(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public bool RemoveQuestion(int questionId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var delete = "PRAGMA foreign_keys = ON; DELETE FROM question WHERE id=:id;";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue(":id", questionId);
                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }  
            }
        }

        public bool SetImage(int questionId, byte[] image)
        {
            int result;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var update = "UPDATE question_image SET data=:image WHERE question_id=:questionId";
                var insert = "INSERT INTO question_image (data, question_id) VALUES (:image, :questionId)";

                using (var command = new SQLiteCommand(update, connection))
                {
                    command.Parameters.AddWithValue(":questionId", questionId);
                    command.Parameters.AddWithValue(":image", image);
                    result = command.ExecuteNonQuery();
                }

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    using (var command = new SQLiteCommand(insert, connection))
                    {
                        command.Parameters.AddWithValue(":questionId", questionId);
                        command.Parameters.AddWithValue(":image", image);
                        result = command.ExecuteNonQuery();
                    }

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        private Question RowToQuestion(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var testId = reader.GetInt32(2);
            var type = reader.GetInt32(3);

            return new Question(id, name, testId, type);
        }
    }
}
