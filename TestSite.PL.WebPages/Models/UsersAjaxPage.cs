namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;
    
    public static class UsersAjaxPage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, AjaxResponse>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>();

        private static Random rand = new Random();

        static UsersAjaxPage()
        {
            _Queries.Add("getRandomTest", GetRandomTest);
            
            _Queries.Add("checkMyAnswers", CheckMyAnswers);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse GetRandomTest(HttpRequestBase request)
        {
            var methodName = nameof(GetRandomTest);
            int randomTestId = -1;
            object result;

            try
            {
                // TODO Затем по отделам
                var tests = LogicProvider.TestLogic.ListAllTests();
                randomTestId = tests.ElementAt(rand.Next(tests.Count)).Id;
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                result = GetTestWithQuestions(randomTestId, mixed:true);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse CheckMyAnswers(HttpRequestBase request)
        {
            var methodName = nameof(CheckMyAnswers);
            int testId = -1;
            dynamic answers = null;

            try
            {
                testId = Convert.ToInt32(request["testid"]);
                answers = Json.Decode(request["answers"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            ICollection<Tuple<int, int>> correctAnswers = null;

            try
            {
                correctAnswers = LogicProvider.TestLogic.ListCorrectAnswers(testId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (correctAnswers == null)
            {
                return SendError(new InvalidOperationException("Не известная ошибка"), methodName);
            }

            try
            {
                int t = Enumerable.Count(answers);
                if (t != correctAnswers.Count)
                {
                    throw new InvalidOperationException("Не известная ошибка");
                }
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Tuple<int, int> item;

            foreach (var answer in answers)
            {
                item = correctAnswers.First(a => a.Item1 == answer.questionId);
                if (item == null)
                {
                    return SendError(new InvalidOperationException("Не известная ошибка"), methodName);
                }
                else
                {
                    if (item.Item2 != answer.answerId)
                    {
                        return new AjaxResponse(null, false);
                    }
                }
            }

            return new AjaxResponse(null, true);
        }

        private static AjaxResponse SendError(Exception ex, string sender = null)
        {
            if (sender == null)
            {
                Logger.Log.Error(ex.Message);
            }
            else
            {
                Logger.Log.Error(sender, ex);
            }

            return new AjaxResponse(ex.Message);
        }

        private static AjaxResponse SendError(string message, string logMessage, string sender = null)
        {
            if (sender == null)
            {
                Logger.Log.Error(logMessage);
            }
            else
            {
                Logger.Log.Error(sender, new Exception(logMessage));
            }

            return new AjaxResponse(message);
        }

        private static ICollection<T> Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        // Вызывать только в try catch
        internal static object GetTestWithQuestions(int testId, bool mixed)
        {
            Test test = LogicProvider.TestLogic.GetTestById(testId);

            ICollection<Question> questions = LogicProvider.QuestionLogic.ListQuestionsByTestId(test.Id);

            if (mixed)
            {
                questions = Shuffle(questions.ToList());
            }
            
            IList result = new ArrayList();

            if (questions != null)
            {
                foreach (var question in questions)
                {
                    var answers = LogicProvider.AnswerLogic.ListAnswersByQuestionId(question.Id);
                    if (mixed)
                    {
                        // Скрываем правильные ответы и перемешиваем
                        answers = Shuffle(answers.Select(a => { a.Correct = false; return a; }).ToList());
                    }

                    //result.Add(new { id = question.Id, text = question.Name, answers = answers });
                    result.Add(new { question, answers = answers });
                }
            }

            return new { test = test, questions = result };
        }
    }
}