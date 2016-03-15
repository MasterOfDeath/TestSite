namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IQuestionLogic
    {
        bool InsertQuestion(Question question);

        bool RemoveQuestion(int questionId);

        ICollection<Question> ListQuestionsByTestId(int testId);
    }
}
