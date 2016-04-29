namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Logger;
    using System.IO;

    public static class AdminsAjaxPage
    {
        private const string adminRole = "admin";

        private static readonly IDictionary<string, Func<HttpRequestBase, AjaxResponse>> _Queries
            = new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>();

        static AdminsAjaxPage()
        {
            _Queries.Add("clickSaveTestBtn", ClickSaveTestBtn);
            _Queries.Add("listQuestionsByTestId", ListQuestionsByTestId);
            _Queries.Add("saveQuestionAndAnswers", SaveQuestionAndAnswers);
            _Queries.Add("getQuestionAndAnswers", GetQuestionAndAnswers);
            _Queries.Add("removeTest", RemoveTest);
            _Queries.Add("removeQuestion", RemoveQuestion);
            _Queries.Add("getTestForPreview", GetTestForPreview);
            _Queries.Add("listEmployeesByDep", ListEmployeesByDep);
            _Queries.Add("saveEmployee", SaveEmployee);
            _Queries.Add("removeEmployee", RemoveEmployee);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

        private static AjaxResponse ClickSaveTestBtn(HttpRequestBase request)
        {
            string testName = null;
            int testId = -1;
            int employeeId = -1;
            int depId = -1;
            var methodName = nameof(ClickSaveTestBtn);

            try
            {
                testName = request["testname"];
                testId = Convert.ToInt32(request["testid"]);
                employeeId = Convert.ToInt32(request["employeeid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                depId = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId).Dep_Id;
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            Test test = (testId > 0) ? new Test(testId, testName, depId) : new Test(testName, depId);

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
            int questionType = -1;
            string text = null;
            dynamic answers = null;
            byte[] image = null;

            try
            {
                testId = Convert.ToInt32(request["testid"]);
                questionId = Convert.ToInt32(request["questionid"]);
                questionType = Convert.ToInt32(request["questiontype"]);
                text = request["text"];
                answers = Json.Decode(request["answers"]);
                var file = request.Files["image"];

                if (file != null)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        image = binaryReader.ReadBytes(file.ContentLength);
                    }
                }
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                questionId = LogicProvider.QuestionLogic.InsertQuestion(new Question(questionId, text, testId, questionType));
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

            if (image != null)
            {
                try
                {
                    LogicProvider.QuestionLogic.SetImage(questionId, image);
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
            //string text = null;
            Question question = null;

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
                question = LogicProvider.QuestionLogic.GetQuestionById(questionId);
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

            dynamic questionAndAnswers = new { text = question.Name, questionType = question.Type, answers = answers };
       
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

        private static AjaxResponse GetTestForPreview(HttpRequestBase request)
        {
            var methodName = nameof(GetTestForPreview);
            int testId = -1;
            object result;

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
                result = UsersAjaxPage.GetTestWithQuestions(testId, mixed: false);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse ListEmployeesByDep(HttpRequestBase request)
        {
            var methodName = nameof(ListEmployeesByDep);
            int requestOwnerId = -1;
            ICollection<Employee> employees;

            try
            {
                requestOwnerId = Convert.ToInt32(request["requestowner"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            try
            {
                var requestOwner = LogicProvider.EmployeeLogic.GetEmployeeById(requestOwnerId);
                employees = LogicProvider.EmployeeLogic.ListEmployeesByDepId(requestOwner.Dep_Id);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, employees);
        }

        private static AjaxResponse SaveEmployee(HttpRequestBase request)
        {
            var methodName = nameof(SaveEmployee);
            int requestOwnerId = -1;
            int employeeId = -1;
            string firstName;
            string laststName;
            string password;

            try
            {
                requestOwnerId = Convert.ToInt32(request["requestowner"]);
                employeeId = Convert.ToInt32(request["employeeid"]);
                firstName = request["firstname"];
                laststName = request["lastname"];
                password = request["password"];
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }
            
            Employee employee = null;
            int result = -1;

            try
            {
                if (employeeId == -1)
                {
                    var requestOwner = LogicProvider.EmployeeLogic.GetEmployeeById(requestOwnerId);
                    employee = new Employee(requestOwner.Dep_Id, firstName, laststName, null, enabled: true);
                    result = LogicProvider.EmployeeLogic.AddEmployee(employee, password);
                }
                else
                {
                    employee = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId);
                    employee.FirstName = firstName;
                    employee.LastName = laststName;
                    result = LogicProvider.EmployeeLogic.InsertEmployee(employee);

                    if (password != "" && result > 0)
                    {
                        LogicProvider.EmployeeLogic.ChangePassword(employeeId, "IamGod", password, godMode: true);
                    }
                }
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse RemoveEmployee(HttpRequestBase request)
        {
            var methodName = nameof(RemoveEmployee);
            int employeeId = -1;

            try
            {
                employeeId = Convert.ToInt32(request["employeeid"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            bool result = false;

            try
            {
                result = LogicProvider.EmployeeLogic.RemoveEmployee(employeeId);
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