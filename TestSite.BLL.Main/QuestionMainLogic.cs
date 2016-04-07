namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Entites;

    public class QuestionMainLogic : IQuestionLogic
    {
        public byte[] GetImage(int questionId)
        {
            byte[] result = null;

            if (questionId < -1)
            {
                throw new ArgumentException($"{nameof(questionId)} не может быть отрицательным");
            }

            try
            {
                result = Stores.QuestionStore.GetImage(questionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public Question GetQuestionById(int questionId)
        {
            Question result = null;

            if (questionId < -1)
            {
                throw new ArgumentException($"{nameof(questionId)} не может быть отрицательным");
            }

            try
            {
                result = Stores.QuestionStore.GetQuestionById(questionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public int InsertQuestion(Question question)
        {
            int result = -1;

            try
            {
                this.IsValidQuestion(question);

                result = Stores.QuestionStore.InsertQuestion(question);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Question> ListQuestionsByTestId(int testId)
        {
            ICollection<Question> result = null;

            if (testId < -1)
            {
                throw new ArgumentException($"{nameof(testId)} не может быть отрицательным");
            }

            try
            {
                result = Stores.QuestionStore.ListQuestionsByTestId(testId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveQuestion(int questionId)
        {
            try
            {
                Stores.QuestionStore.RemoveQuestion(questionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public bool SetImage(int questionId, byte[] image)
        {
            bool result = false;

            if (questionId < -1)
            {
                throw new ArgumentException($"{nameof(questionId)} не может быть отрицательным");
            }

            if (image == null)
            {
                throw new ArgumentException($"{nameof(image)} не может быть пустым");
            }

            try
            {
                result = Stores.QuestionStore.SetImage(questionId, image);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private bool IsValidQuestion(Question question)
        {
            if (question == null)
            {
                throw new ArgumentException($"{nameof(question)} не может быть null");
            }

            if (string.IsNullOrWhiteSpace(question.Name))
            {
                throw new ArgumentException($"{question.Name} не может быть пустым или состоять из пробелов");
            }

            if (question.Id < -1)
            {
                throw new ArgumentException($"{nameof(question.Id)} не может быть отрицательным");
            }

            return true;
        }
    }
}
