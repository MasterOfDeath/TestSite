namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;
    using Variables;

    public static class AdminsAjaxPage
    {
        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> AdminsQueries { get; } = 
            new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>
            {
                ["clickSaveTestBtn"] = ClickSaveTestBtn,
                ["listQuestionsByTestId"] = ListQuestionsByTestId,
                ["saveQuestionAndAnswers"] = SaveQuestionAndAnswers,
                ["getQuestionAndAnswers"] = GetQuestionAndAnswers,
                ["removeTest"] = RemoveTest,
                ["removeQuestion"] = RemoveQuestion,
                ["getTestForPreview"] = GetTestForPreview,
                ["saveEmployeeByDepFromOwner"] = SaveEmployeeByDepFromOwner,
                ["removeEmployee"] = RemoveEmployee,
                ["removeReport"] = RemoveReport,
            };

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> SuperadminsQueries { get; } =
            new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>
            {
                ["insertDep"] = InsertDep,
                ["removeDep"] = RemoveDep,
                ["saveEmployeeByDep"] = SaveEmployeeByDep,
                ["removeEmployee"] = RemoveEmployee,
            };

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> InspectorsQueries { get; } =
            new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>()
            {
                ["listQuestionsByTestId"] = ListQuestionsByTestId,
                ["getQuestionAndAnswers"] = GetQuestionAndAnswers,
                ["getTestForPreview"] = GetTestForPreview,
            };

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
                return Common.SendError(ex, methodName);
            }

            try
            {
                depId = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId).Dep_Id;
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            Test test = (testId > 0) ? new Test(testId, testName, depId) : new Test(testName, depId);

            try
            {
                LogicProvider.TestLogic.InsertTest(test);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
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
                return Common.SendError(ex, methodName);
            }

            try
            {
                questions = LogicProvider.QuestionLogic.ListQuestionsByTestId(testId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
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
                return Common.SendError(ex, methodName);
            }

            try
            {
                questionId = LogicProvider.QuestionLogic.InsertQuestion(new Question(questionId, text, testId, questionType));
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            if (questionId < 0)
            {
                return Common.SendError(new InvalidOperationException("Не известная ошибка"), methodName);
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
                    return Common.SendError(ex, methodName);
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
                    return Common.SendError(ex, methodName);
                }
            }

            return new AjaxResponse(null, questionId);
        }

        private static AjaxResponse GetQuestionAndAnswers(HttpRequestBase request)
        {
            var methodName = nameof(GetQuestionAndAnswers);
            int questionId = -1;
            Question question = null;

            try
            {
                questionId = Convert.ToInt32(request["questionid"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            try
            {
                question = LogicProvider.QuestionLogic.GetQuestionById(questionId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            ICollection<Answer> answers = null;

            try
            {
                answers = LogicProvider.AnswerLogic.ListAnswersByQuestionId(questionId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
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
                return Common.SendError(ex, methodName);
            }

            try
            {
                LogicProvider.TestLogic.RemoveTest(testId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
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
                return Common.SendError(ex, methodName);
            }

            try
            {
                LogicProvider.QuestionLogic.RemoveQuestion(questionId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
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
                return Common.SendError(ex, methodName);
            }

            try
            {
                result = UsersAjaxPage.GetTestWithQuestions(testId, mixed: false);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse SaveEmployeeByDepFromOwner(HttpRequestBase request)
        {
            var methodName = nameof(SaveEmployeeByDepFromOwner);
            int requestOwnerId = -1;

            try
            {
                requestOwnerId = Convert.ToInt32(request["requestowner"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            int result = -1;

            try
            {
                var depId = LogicProvider.EmployeeLogic.GetEmployeeById(requestOwnerId).Dep_Id;
                result = SaveEmployee(request, depId, Variables.UserRole.Id);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse SaveEmployeeByDep(HttpRequestBase request)
        {
            var methodName = nameof(SaveEmployeeByDep);
            int depId = -1;
            int roleId = -1;

            try
            {
                depId = Convert.ToInt32(request["depid"]);
                roleId = Convert.ToInt32(request["roleid"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            int result = -1;

            try
            {
                result = SaveEmployee(request, depId, roleId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
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
                return Common.SendError(ex, methodName);
            }

            bool result = false;

            try
            {
                result = LogicProvider.EmployeeLogic.RemoveEmployee(employeeId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse InsertDep(HttpRequestBase request)
        {
            var methodName = nameof(InsertDep);
            int employeeId = -1;
            int depId = -1;
            string depName = null;

            try
            {
                employeeId = Convert.ToInt32(request["employeeid"]);
                depId = Convert.ToInt32(request["depid"]);
                depName = request["depname"];
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            bool result = false;

            try
            {
                result = LogicProvider.DepLogic.InsertDep(new Dep(depId, depName));
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse RemoveDep(HttpRequestBase request)
        {
            var methodName = nameof(RemoveDep);
            int depId = -1;

            try
            {
                depId = Convert.ToInt32(request["depid"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            bool result = false;

            try
            {
                result = LogicProvider.DepLogic.RemoveDep(depId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse RemoveReport(HttpRequestBase request)
        {
            var methodName = nameof(RemoveReport);
            int reportId = -1;

            try
            {
                reportId = Convert.ToInt32(request["reportid"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            bool result = false;

            try
            {
                result = LogicProvider.ReportLogic.RemoveReport(reportId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        // Вызывать только в try catch
        private static int SaveEmployee(HttpRequestBase request, int depId, int roleId)
        {
            int employeeId = -1;
            string firstName;
            string laststName;
            string password;

            employeeId = Convert.ToInt32(request["employeeid"]);
            firstName = request["firstname"];
            laststName = request["lastname"];
            password = request["password"];

            Employee employee = null;
            int result = -1;

            if (employeeId == -1)
            {
                employee = new Employee(depId, firstName, laststName, null, true, roleId);
                result = LogicProvider.EmployeeLogic.AddEmployee(employee, password);
            }
            else
            {
                employee = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId);
                employee.FirstName = firstName;
                employee.LastName = laststName;
                employee.Role_Id = roleId;
                result = LogicProvider.EmployeeLogic.InsertEmployee(employee);

                if (password != "" && result > 0)
                {
                    LogicProvider.EmployeeLogic.ChangePassword(employeeId, "IamGod", password, godMode: true);
                }
            }

            return result;
        }
    }
}