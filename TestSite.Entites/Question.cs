namespace TestSite.Entites
{
    public class Question
    {
        public Question(int id, string name, int testId, int type)
        {
            this.Id = id;
            this.Name = name;
            this.TestId = testId;
            this.Type = type;
        }

        public Question(string name, int testId, int type)
            : this(-1, name, testId, type)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int TestId { get; set; }

        public int Type { get; set; }
    }
}
