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
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command;

                if (answer.Id > 0)
                {
                    command = new SQLiteCommand("UPDATE answer SET name=:name, correct=:correct WHERE id=:id", connection);
                    command.Parameters.AddWithValue(":id", answer.Id);
                    command.Parameters.AddWithValue(":name", answer.Name);
                    command.Parameters.AddWithValue(":correct", answer.Correct);
                }
                else
                {
                    command = new SQLiteCommand("INSERT INTO answer (name, correct) VALUES (:name, :correct)", connection);
                    command.Parameters.AddWithValue(":name", answer.Name);
                    command.Parameters.AddWithValue(":correct", answer.Correct);
                }

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        public ICollection<Answer> ListAllAnswers()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                var command = new SQLiteCommand("SELECT id, name, correct FROM answer", connection);

                List<Answer> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

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

        public bool RemoveAnswer(int answerId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command;

                command = new SQLiteCommand("DELETE FROM answer WHERE id=:id", connection);
                command.Parameters.AddWithValue(":id", answerId);

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        private Answer RowToAnswer(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            var correct = reader.GetInt32(2) > 0;

            return new Answer(id, name, correct);
        }
    }
}
