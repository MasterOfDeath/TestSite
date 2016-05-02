namespace TestSite.DAL.Sqlite
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class AnswerSqliteStore : IAnswerStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public bool InsertAnswer(Answer answer)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                int result = -1;

                if (answer.Id > 0)
                {
                    using (var command = new SQLiteCommand("UPDATE answer SET name=:name, correct=:correct, question_id=:questionId WHERE id=:id", connection))
                    {
                        command.Parameters.AddWithValue(":id", answer.Id);
                        command.Parameters.AddWithValue(":name", answer.Name);
                        command.Parameters.AddWithValue(":correct", answer.Correct);
                        command.Parameters.AddWithValue(":questionId", answer.QuestionId);
                        result = command.ExecuteNonQuery();
                    } 
                }
                else
                {
                    using (var command = new SQLiteCommand("INSERT INTO answer (name, correct, question_id) VALUES (:name, :correct, :questionId)", connection))
                    {
                        command.Parameters.AddWithValue(":name", answer.Name);
                        command.Parameters.AddWithValue(":correct", answer.Correct);
                        command.Parameters.AddWithValue(":questionId", answer.QuestionId);
                        result = command.ExecuteNonQuery();
                    }    
                }
                
                return result > 0;
            }
        }

        public ICollection<Answer> ListAllAnswers()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM answer", connection))
                {
                    List<Answer> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Answer>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToAnswer(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public ICollection<Answer> ListAnswersByQuestionId(int questionId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM answer WHERE question_id=:questionId", connection))
                {
                    command.Parameters.AddWithValue(":questionId", questionId);
                    List<Answer> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Answer>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToAnswer(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public ICollection<Answer> ListCorrectAnswers(int questionId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM answer WHERE question_id=:questionId AND correct = 1", connection))
                {
                    command.Parameters.AddWithValue(":questionId", questionId);
                    List<Answer> result = null;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            result = new List<Answer>(reader.StepCount);
                        }

                        while (reader.Read())
                        {
                            result.Add(this.RowToAnswer(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public bool RemoveAnswer(int answerId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("DELETE FROM answer WHERE id=:id", connection))
                {
                    command.Parameters.AddWithValue(":id", answerId);
                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }
            }
        }

        private Answer RowToAnswer(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var correct = reader.GetInt32(2) > 0;
            var questionId = reader.GetInt32(3);

            return new Answer(id, name, correct, questionId);
        }
    }
}
