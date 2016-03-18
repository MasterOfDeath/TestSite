namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;
    
    public static class AdminsAjaxPage
    {
        private static readonly IDictionary<string, Func<HttpRequestBase, AjaxResponse>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>();

        static AdminsAjaxPage()
        {
            _Queries.Add("clickSaveTestBtn", ClickSaveTestBtn);
            _Queries.Add("listAllTests", ListAllTests);
            _Queries.Add("listQuestionsByTestId", ListQuestionsByTestId);
            _Queries.Add("saveQuestionAndAnswers", SaveQuestionAndAnswers);
            _Queries.Add("getQuestionAndAnswers", GetQuestionAndAnswers);
            _Queries.Add("removeTest", RemoveTest);
            _Queries.Add("removeQuestion", RemoveQuestion);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse ClickSaveTestBtn(HttpRequestBase request)
        {
            string testName = null;
            int testId = -1;
            var methodName = nameof(ClickSaveTestBtn);

            try
            {
                testName = request["testname"];
                testId = Convert.ToInt32(request["testid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Test test = (testId > 0) ? new Test(testId, testName) : new Test(testName);

            try
            {
                LogicProvider.TestLogic.InsertTest(test);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }
            
            return new AjaxResponse(null, true);
        }

        private static AjaxResponse ListAllTests(HttpRequestBase request)
        {
            var methodName = nameof(ListAllTests);
            ICollection<Test> tests;

            try
            {
                tests = LogicProvider.TestLogic.ListAllTests();
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, tests);
        }

        private static AjaxResponse ListQuestionsByTestId(HttpRequestBase request)
        {
            var methodName = nameof(ListQuestionsByTestId);
            int testId = -1;
            ICollection<Question> questions;

            try
            {
                testId = Convert.ToInt32(request["testid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                questions = LogicProvider.QuestionLogic.ListQuestionsByTestId(testId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, questions);
        }

        private static AjaxResponse SaveQuestionAndAnswers(HttpRequestBase request)
        {
            var methodName = nameof(SaveQuestionAndAnswers);
            int testId = -1;
            int questionId = -1;
            string text = null;
            dynamic answers = null;

            try
            {
                testId = Convert.ToInt32(request["testid"]);
                questionId = Convert.ToInt32(request["questionid"]);
                text = request["text"];
                answers = Json.Decode(request["answers"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                questionId = LogicProvider.QuestionLogic.InsertQuestion(new Question(questionId,text, testId));
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            if (questionId < 0)
            {
                return SendError(new InvalidOperationException("Не известная ошибка"), methodName);
            }

            foreach (var answer in answers)
            {
                try
                {
                    if (answer.action == "insert" || answer.action == "update")
                    {
                        LogicProvider.AnswerLogic.InsertAnswer(
                            new Answer(answer.answerId, answer.text, answer.correct, questionId));
                    }

                    if (answer.action == "delete")
                    {
                        LogicProvider.AnswerLogic.RemoveAnswer(answer.answerId);
                    }
                }
                catch (Exception ex)
                {
                    return SendError(ex, methodName);
                }
            }

            return new AjaxResponse(null, questionId);
        }

        private static AjaxResponse GetQuestionAndAnswers(HttpRequestBase request)
        {
            var methodName = nameof(GetQuestionAndAnswers);
            int questionId = -1;
            string text = null;

            try
            {
                questionId = Convert.ToInt32(request["questionid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                text = LogicProvider.QuestionLogic.GetQuestionById(questionId).Name;
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            ICollection<Answer> answers = null;

            try
            {
                answers = LogicProvider.AnswerLogic.ListAnswersByQuestionId(questionId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            dynamic questionAndAnswers = new { text = text, answers = answers };
       
            return new AjaxResponse(null, questionAndAnswers);
        }

        private static AjaxResponse RemoveTest(HttpRequestBase request)
        {
            int testId = -1;
            var methodName = nameof(RemoveTest);

            try
            {
                testId = Convert.ToInt32(request["testid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                LogicProvider.TestLogic.RemoveTest(testId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, true);
        }

        private static AjaxResponse RemoveQuestion(HttpRequestBase request)
        {
            int questionId = -1;
            var methodName = nameof(RemoveQuestion);

            try
            {
                questionId = Convert.ToInt32(request["questionid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                LogicProvider.QuestionLogic.RemoveQuestion(questionId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
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