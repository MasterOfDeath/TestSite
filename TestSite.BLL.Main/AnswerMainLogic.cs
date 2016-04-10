namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Entites;

    public class AnswerMainLogic : IAnswerLogic
    {
        public bool InsertAnswer(Answer answer)
        {
            bool result = false;

            try
            {
                this.IsValidAnswer(answer);

                result = Stores.AnswerStore.InsertAnswer(answer);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Answer> ListAnswersByQuestionId(int questionId)
        {
            ICollection<Answer> result = null;

            if (questionId < -1)
            {
                throw new ArgumentException($"{nameof(questionId)} не может быть отрицательным");
            }

            try
            {
                result = Stores.AnswerStore.ListAnswersByQuestionId(questionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Answer> ListCorrectAnswers(int questionId)
        {
            ICollection<Answer> result = null;

            if (questionId < -1)
            {
                throw new ArgumentException($"{nameof(questionId)} не может быть отрицательным");
            }

            try
            {
                result = Stores.AnswerStore.ListCorrectAnswers(questionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveAnswer(int answerId)
        {
            try
            {
                Stores.AnswerStore.RemoveAnswer(answerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private bool IsValidAnswer(Answer answer)
        {
            if (answer == null)
            {
                throw new ArgumentException($"{nameof(answer)} не может быть null");
            }

            if (string.IsNullOrWhiteSpace(answer.Name))
            {
                throw new ArgumentException($"{answer.Name} не может быть пустым или состоять из пробелов");
            }

            if (answer.Id < -1)
            {
                throw new ArgumentException($"{nameof(answer.Id)} не может быть отрицательным");
            }

            return true;
        }
    }
}
