namespace TestSite.DAL.Sqlite
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SQLite;
    using Contract;
    using Entites;

    public class DepSqliteStore : IDepStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Sqlite"].ConnectionString;

        public bool InsertDep(Dep dep)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command;

                if (dep.Id > 0)
                {
                    command = new SQLiteCommand("UPDATE dep SET name=:name WHERE id=:id", connection);
                    command.Parameters.AddWithValue(":id", dep.Id);
                    command.Parameters.AddWithValue(":name", dep.Name);
                }
                else
                {
                    command = new SQLiteCommand("INSERT INTO dep (name) VALUES (:name)", connection);
                    command.Parameters.AddWithValue(":name", dep.Name);
                }

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        public ICollection<Dep> ListAllDeps()
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                var command = new SQLiteCommand("SELECT id, name FROM dep", connection);

                List<Dep> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<Dep>(reader.StepCount);
                }

                while (reader.Read())
                {
                    result.Add(this.RowToDep(reader));
                }

                return result;
            }
        }

        public bool RemoveDep(int depId)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command;

                command = new SQLiteCommand("DELETE FROM dep WHERE id=:id", connection);
                command.Parameters.AddWithValue(":id", depId);

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0;
            }
        }

        private Dep RowToDep(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);

            return new Dep(id, name);
        }
    }
}
