namespace TestSite.PL.WebPages.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Helpers;
    using Entites;

    public static class UsersAjaxPage
    {
        private static Random rand = new Random();

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries { get; } =
            new Dictionary<string, Func<HttpRequestBase, AjaxResponse>>()
            {
                ["getRandomMixedTest"] = GetRandomMixedTest,
                ["getMixedTest"] = GetMixedTest,
                ["checkMyAnswers"] = CheckMyAnswers,
                ["listTestsForEmployee"] = ListTestsForEmployee,
                ["changePassword"] = ChangePassword,
                ["getReportByDep"] = GetReport,
                ["getReportByEmployee"] = GetReport,
            };


        private static AjaxResponse GetRandomMixedTest(HttpRequestBase request)
        {
            var methodName = nameof(GetRandomMixedTest);
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
                return Common.SendError(ex, methodName);
            }

            try
            {
                result = GetTestWithQuestions(randomTestId, mixed:true);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse GetMixedTest(HttpRequestBase request)
        {
            var methodName = nameof(GetMixedTest);
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
                result = GetTestWithQuestions(testId, mixed: true);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse CheckMyAnswers(HttpRequestBase request)
        {
            var methodName = nameof(CheckMyAnswers);
            int employeeId = -1;
            int testId = -1;
            bool rating = false;
            dynamic results = null;

            try
            {
                employeeId = Convert.ToInt32(request["employeeid"]);
                testId = Convert.ToInt32(request["testid"]);
                rating = Convert.ToBoolean(request["rating"]);
                results = Json.Decode(request["results"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            ICollection<Answer> correctAnswers = null;
            ICollection<string> wrongQuestions = new List<string>();
            Question question = null;
            bool correct = true;

            try
            {
                foreach (var result in results)
                {
                    int questionId = (int)result.questionId;
                    correctAnswers = LogicProvider.AnswerLogic.ListCorrectAnswers(questionId);

                    if (correctAnswers.Count != Enumerable.Count(result.answers))
                    {
                        correct = false;
                        question = LogicProvider.QuestionLogic.GetQuestionById(questionId);
                        wrongQuestions.Add(question.Name);
                        continue;
                    }

                    foreach (var answer in result.answers)
                    {
                        if (!correctAnswers.Select(a => a.Id).Contains((int)answer))
                        {
                            correct = false;
                            question = LogicProvider.QuestionLogic.GetQuestionById(questionId);
                            wrongQuestions.Add(question.Name);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            if (rating)
            {
                try
                {
                    var wrongsCount = wrongQuestions.Count;
                    var wrongsPercent = wrongQuestions.Count * 100 / Enumerable.Count(results);
                    var report = new Report(employeeId, testId, DateTime.Now, wrongsCount, wrongsPercent);
                    LogicProvider.ReportLogic.InsertReport(report);
                }
                catch (Exception ex)
                {
                    return Common.SendError(ex, methodName);
                }
            }

            return new AjaxResponse(null, new { correct, wrongQuestions });
        }

        private static AjaxResponse ListTestsForEmployee(HttpRequestBase request)
        {
            var methodName = nameof(ListTestsForEmployee);
            ICollection<Test> tests;
            int employeeId = -1;
            int depId = -1;

            try
            {
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

            try
            {
                tests = LogicProvider.TestLogic.ListTestsByDepId(depId);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, tests);
        }

        private static AjaxResponse ChangePassword(HttpRequestBase request)
        {
            var methodName = nameof(ChangePassword);
            int employeeId = -1;
            string oldPassword = "";
            string newPassword = "";

            try
            {
                employeeId = Convert.ToInt32(request["employeeid"]);
                oldPassword = request["oldpassword"];
                newPassword = request["newpassword"];
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            var result = false;

            try
            {
                result = LogicProvider.EmployeeLogic.ChangePassword(employeeId, oldPassword, newPassword, godMode: false);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
        }

        private static AjaxResponse GetReport(HttpRequestBase request)
        {
            var methodName = nameof(GetReport);
            string queryName;
            int employeeId = -1;
            DateTime dateStart;
            DateTime dateEnd;

            try
            {
                queryName = request["queryName"];
                employeeId = Convert.ToInt32(request["employeeid"]);
                dateStart = Convert.ToDateTime(request["datestart"]);
                dateEnd = Convert.ToDateTime(request["dateend"]);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            List<Dictionary<string, object>> result = null;

            try
            {
                var employee = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId);
                dateEnd = dateEnd.AddHours(23 - dateEnd.Hour);
                ICollection<Report> reports = null;

                if (queryName == "getReportByEmployee")
                {
                    reports = LogicProvider.ReportLogic.ListReportsByEmployee(employee.Id, dateStart, dateEnd);
                }

                if (queryName == "getReportByDep")
                {
                    reports = LogicProvider.ReportLogic.ListReportsByDep(employee.Dep_Id, dateStart, dateEnd);
                }

                if (reports == null)
                {
                    return new AjaxResponse("Данные отсутствуют", null);
                }

                result = GetReportData(reports);
            }
            catch (Exception ex)
            {
                return Common.SendError(ex, methodName);
            }

            return new AjaxResponse(null, result);
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

                    result.Add(new { question, answers = answers });
                }
            }

            return new { test = test, questions = result };
        }

        // Вызывать только в try catch
        internal static List<Dictionary<string, object>> GetReportData(ICollection<Report> reports)
        {
            if (reports == null)
            {
                return null;
            }

            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>(reports.Count);

            var employeeFIOs = new Dictionary<int, string>();
            var testNames = new Dictionary<int, string>();

            result = reports.Select(
                    r => new Dictionary<string, object>()
                    {
                        { "Id", r.Id },
                        { "Employee", GetEmployeeFIO(r.EmployeeId, employeeFIOs) },
                        { "Test", GetTestName(r.TestId, testNames) },
                        { "Date", r.Date.ToString("dd.MM.yyyy HH:mm") },
                        { "ErrCount", r.ErrCount },
                        { "ErrPercent", r.ErrPercent }
                    }
                ).ToList();

            employeeFIOs.Clear();
            testNames.Clear();

            return result;
        }

        private static string GetEmployeeFIO(int employeeId, IDictionary<int, string> cache)
        {
            if (!cache.ContainsKey(employeeId))
            {
                var employee = LogicProvider.EmployeeLogic.GetEmployeeById(employeeId);
                cache.Add(employeeId, employee.LastName + " " + employee.FirstName);
            }

            return cache[employeeId];
        }

        private static string GetTestName(int testId, IDictionary<int, string> cache)
        {
            if (!cache.ContainsKey(testId))
            {
                var test = LogicProvider.TestLogic.GetTestById(testId);
                cache.Add(testId, test.Name);
            }

            return cache[testId];
        }
    }
}