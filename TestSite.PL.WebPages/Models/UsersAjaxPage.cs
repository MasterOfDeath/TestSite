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
            _Queries.Add("getRandomMixedTest", GetRandomMixedTest);
            _Queries.Add("getMixedTest", GetMixedTest);
            _Queries.Add("checkMyAnswers", CheckMyAnswers);
            _Queries.Add("listTestsForEmployee", ListTestsForEmployee);
        }

        public static IDictionary<string, Func<HttpRequestBase, AjaxResponse>> Queries
        {
            get { return _Queries; }
        }

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
                return SendError(ex, methodName);
            }

            try
            {
                result = GetTestWithQuestions(testId, mixed: true);
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
            dynamic results = null;

            try
            {
                testId = Convert.ToInt32(request["testid"]);
                results = Json.Decode(request["results"]);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            ICollection<Answer> correctAnswers = null;
            ICollection<string> wrongQuestions = new List<string>();
            Question question = null;
            bool correct = true;

            try
            {
                foreach (var result in results)
                {
                    correctAnswers = LogicProvider.AnswerLogic.ListCorrectAnswers((int)result.questionId);

                    if (correctAnswers.Count != Enumerable.Count(result.answers))
                    {
                        correct = false;
                        question = LogicProvider.QuestionLogic.GetQuestionById((int)result.questionId);
                        wrongQuestions.Add(question.Name);
                        continue;
                    }

                    foreach (var answer in result.answers)
                    {
                        if (!correctAnswers.Select(a => a.Id).Contains((int)answer))
                        {
                            correct = false;
                            question = LogicProvider.QuestionLogic.GetQuestionById((int)result.questionId);
                            wrongQuestions.Add(question.Name);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
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

            try
            {
                tests = LogicProvider.TestLogic.ListTestsByDepId(depId);
            }
            catch (Exception ex)
            {
                return SendError(ex, methodName);
            }

            return new AjaxResponse(null, tests);
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