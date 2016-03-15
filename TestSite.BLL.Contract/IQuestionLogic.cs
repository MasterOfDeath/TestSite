namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IQuestionLogic
    {
        int InsertQuestion(Question question);

        bool RemoveQuestion(int questionId);

        ICollection<Question> ListQuestionsByTestId(int testId);
    }
}
