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
            _Queries.Add("saveQuestion", SaveQuestion);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse ClickSaveTestBtn(HttpRequestBase request)
        {
            string testName = null;
            var methodName = nameof(ClickSaveTestBtn);

            try
            {
                testName = request["testname"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                LogicProvider2.TestLogic.InsertTest(new Test(testName));
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
                tests = LogicProvider2.TestLogic.ListAllTests();
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
                questions = LogicProvider2.QuestionLogic.ListQuestionsByTestId(testId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, questions);
        }

        private static AjaxResponse SaveQuestion(HttpRequestBase request)
        {
            var methodName = nameof(SaveQuestion);
            int testId = -1;
            string text = null;

            try
            {
                testId = Convert.ToInt32(request["testid"]);
                text = request["text"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            int result = -1;

            try
            {
                result = LogicProvider2.QuestionLogic.InsertQuestion(new Question(text, testId));
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
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