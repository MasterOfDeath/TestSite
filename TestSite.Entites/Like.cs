namespace TestSite.Entites
{
    using System;

    public class Like
    {
        public Like(int id, int photoId, int userId, DateTime date)
        {
            this.Id = id;
            this.PhotoId = photoId;
            this.UserId = userId;
            this.Date = date;
        }

        public Like(int photoId, int userId, DateTime date)
            : this(0, photoId, userId, date)
        {
        }

        public int Id { get; set; }

        public int PhotoId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }
    }
}
