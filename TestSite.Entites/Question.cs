namespace TestSite.Entites
{
    public class Question
    {
        public Question(int id, string name, int testId, int type, bool forRating)
        {
            this.Id = id;
            this.Name = name;
            this.TestId = testId;
            this.Type = type;
            this.ForRating = forRating;
        }

        public Question(string name, int testId, int type, bool forRating)
            : this(-1, name, testId, type, forRating)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int TestId { get; set; }

        public int Type { get; set; }

        public bool ForRating { get; set; }
    }
}
