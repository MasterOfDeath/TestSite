namespace TestSite.DAL.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using Contract;
    using Entites;

    public class AlbumSqlStore : IAlbumStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;

        public bool AddAlbum(Album album)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Album_AddAlbum";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                command.Parameters.AddWithValue("@name", album.Name);
                var dateParameter = new SqlParameter("@date", SqlDbType.DateTime);
                dateParameter.Value = album.Date;
                command.Parameters.Add(dateParameter);
                command.Parameters.AddWithValue("@userId", album.UserId);

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    album.Id = (int)(decimal)reader["newId"];
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Album GetAlbumById(int albumId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Album_GetAlbumById";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", albumId);

                Album album = null;

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    album = this.RowToAlbum(reader);
                }

                return album;
            }
        }

        public bool InsertAlbum(Album album)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Album_InsertAlbum";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", album.Id);
                command.Parameters.AddWithValue("@name", album.Name);
                var dateParameter = new SqlParameter("@date", SqlDbType.DateTime);
                dateParameter.Value = album.Date;
                command.Parameters.Add(dateParameter);
                command.Parameters.AddWithValue("@userId", album.UserId);

                connection.Open();
                var reader = command.ExecuteNonQuery();

                return reader > 0;
            }
        }

        public ICollection<Album> ListAlbumsByUserId(int userId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Album_ListAlbumsByUserId";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@userId", userId);

                List<Album> result = null;

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    result = new List<Album>();
                }

                while (reader.Read())
                {
                    result.Add(this.RowToAlbum(reader));
                }

                return result;
            }
        }

        public bool RemoveAlbum(int albumId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Album_RemoveAlbum";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@id", albumId);

                connection.Open();
                var reader = command.ExecuteNonQuery();

                return reader > 0;
            }
        }

        private Album RowToAlbum(SqlDataReader reader)
        {
            var id = (int)reader["id"];
            var name = (string)reader["name"];
            var date = (DateTime)reader["date"];
            var userId = (int)reader["user_id"];

            return new Album(id, name, date, userId);
        }
    }
}
