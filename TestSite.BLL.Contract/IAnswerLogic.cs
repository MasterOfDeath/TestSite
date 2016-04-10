namespace TestSite.BLL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface IAnswerLogic
    {
        bool InsertAnswer(Answer answer);

        bool RemoveAnswer(int answerId);

        ICollection<Answer> ListAnswersByQuestionId(int questionId);

        ICollection<Answer> ListCorrectAnswers(int questionId);
    }
}
