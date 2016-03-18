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
            Test test = null;

            Random rand = new Random();

            try
            {
                // TODO Затем по отделам
                var tests = LogicProvider.TestLogic.ListAllTests();
                var randomIndex = rand.Next(tests.Count);
                test = tests.ElementAt(randomIndex);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            ICollection<Question> questions = null;

            try
            {
                questions = LogicProvider.QuestionLogic.ListQuestionsByTestId(test.Id);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            IList result = new ArrayList();

            if (questions != null)
            {
                foreach (var question in questions)
                {
                    try
                    {
                        var answers = LogicProvider.AnswerLogic.ListAnswersByQuestionId(question.Id);
                        // Скрываем правильные ответы
                        answers.Select(a => { a.Correct = false; return a; }).ToList();
                        result.Add(new { id = question.Id, text = question.Name, answers = answers });
                    }
                    catch (Exception ex)
                    {
                        return SendError(ex, methodName);
                    }
                }
            }
       
            return new AjaxResponse(null, new { test = test, questions = result });
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
    }
}