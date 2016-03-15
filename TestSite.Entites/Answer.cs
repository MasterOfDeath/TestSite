namespace TestSite.Entites
{
    public class Answer
    {
        public Answer(int id, string name, bool correct)
        {
            this.Id = id;
            this.Name = name;
            this.Correct = correct;
        }

        public Answer(string name, bool correct)
            : this(-1, name, correct)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Correct { get; set; }
    }
}
