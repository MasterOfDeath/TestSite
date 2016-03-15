namespace TestSite.Entites
{
    using System;
    
    public class Photo
    {
        public Photo(int id, string name, int albumId, int size, string mime, DateTime date, int userId)
        {
            this.Id = id;
            this.Name = name;
            this.AlbumId = albumId;
            this.Size = size;
            this.Mime = mime;
            this.Date = date;
            this.UserId = userId;
        }

        public Photo(string name, int albumId, int size, string mime, DateTime date, int userId)
            : this(0, name, albumId, size, mime, date, userId)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int AlbumId { get; set; }

        public int Size { get; set; }

        public string Mime { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }
    }
}
