namespace TestSite.Entites
{
    public class Question
    {
        public Question(int id, string name, int testId)
        {
            this.Id = id;
            this.Name = name;
            this.TestId = testId;
        }

        public Question(string name, int testId)
            : this(-1, name, testId)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int TestId { get; set; }
    }
}
