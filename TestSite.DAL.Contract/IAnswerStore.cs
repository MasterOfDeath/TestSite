namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IAnswerStore
    {
        bool InsertAnswer(Answer answer);

        bool RemoveAnswer(int answerId);

        ICollection<Answer> ListAllAnswers();

        ICollection<Answer> ListAnswersByQuestionId(int questionId);

        ICollection<Answer> ListCorrectAnswers(int questionId);
    }
}
