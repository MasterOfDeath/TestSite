namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IQuestionStore
    {
        bool InsertQuestion(Question question);

        bool RemoveQuestion(int questionId);

        ICollection<Question> ListAllQuestions();

        ICollection<Question> ListQuestionsByTestId(int testId);
    }
}
