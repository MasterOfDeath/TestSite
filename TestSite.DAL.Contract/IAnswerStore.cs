namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IAnswerStore
    {
        bool InsertAnswer(Answer answer);

        bool RemoveAnswer(int answerId);

        ICollection<Answer> ListAllAnswers();
    }
}
