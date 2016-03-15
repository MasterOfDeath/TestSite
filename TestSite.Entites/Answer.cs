namespace TestSite.Entites
{
    public class Answer
    {
        public Answer(int id, string name, bool correct, int questionId)
        {
            this.Id = id;
            this.Name = name;
            this.Correct = correct;
            this.QuestionId = questionId;
        }

        public Answer(string name, bool correct, int questionId)
            : this(-1, name, correct, questionId)
        {
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Correct { get; set; }

        public int QuestionId { get; set; }
    }
}
