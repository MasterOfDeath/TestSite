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

        public Dep GetDepById(int depId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand("SELECT * FROM dep WHERE id=:depId", connection))
                {
                    command.Parameters.AddWithValue(":depId", depId);

                    using (var reader = command.ExecuteReader())
                    {
                        Dep result = null;
                        reader.Read();

                        if (reader.HasRows)
                        {
                            result = this.RowToDep(reader);
                        }

                        return result;
                    }
                }
            }
        }

        public bool InsertDep(Dep dep)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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
                var command = new SQLiteCommand("SELECT id, name FROM dep ORDER BY name", connection);

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
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();

                var delete = "PRAGMA foreign_keys = ON; DELETE FROM dep WHERE id=:id;";

                using (var command = new SQLiteCommand(delete, connection))
                {
                    command.Parameters.AddWithValue(":id", depId);
                    var result = command.ExecuteNonQuery();

                    return result > 0;
                }
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
