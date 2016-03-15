namespace TestSite.Entites
{
    using System;

    public class Album
    {
        public Album(int id, string name, DateTime date, int userId)
        {
            this.Id = id;
            this.Name = name;
            this.Date = date;
            this.UserId = userId;
        }

        public Album(string name, DateTime date, int userId)
            : this(0, name, date, userId)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; }
    }
}
