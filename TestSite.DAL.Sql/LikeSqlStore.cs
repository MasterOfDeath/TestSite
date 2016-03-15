namespace TestSite.DAL.Sql
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using DAL.Contract;
    using Entites;

    public class LikeSqlStore : ILikeStore
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["MSSql"].ConnectionString;

        public bool AddLike(Like like)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Like_AddLike";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@photoId", like.PhotoId);
                command.Parameters.AddWithValue("@userId", like.UserId);
                command.Parameters.AddWithValue("@date", like.Date);

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    like.Id = (int)(decimal)reader["newId"];
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        public Like GetLikeByUserIdAndPhotoId(int userId, int phoroId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Like_GetLikeByUserIdAndPhotoId";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@photoId", phoroId);

                Like like = null;

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    like = this.RowToLike(reader);
                }

                return like;
            }
        }

        public int GetLikesCount(int photoId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var storeProcedure = "Like_CountLikes";

                var command = new SqlCommand(storeProcedure, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@photoId", photoId);

                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return (int)reader["count"];
                }
                else
                {
                    return -1;
                }
            }
        }

        private Like RowToLike(SqlDataReader reader)
        {
            var id = (int)reader["id"];
            var photoId = (int)reader["photo_id"];
            var userId = (int)reader["user_id"];
            var date = (DateTime)reader["date"];

            return new Like(id, photoId, userId, date);
        }
    }
}
