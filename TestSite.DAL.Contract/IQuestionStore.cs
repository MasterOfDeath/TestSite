namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IQuestionStore
    {
        int InsertQuestion(Question question);

        bool RemoveQuestion(int questionId);

        ICollection<Question> ListAllQuestions();

        ICollection<Question> ListQuestionsByTestId(int testId);

        Question GetQuestionById(int questionId);

        bool SetImage(int questionId, byte[] image);

        byte[] GetImage(int questionId);
    }
}
