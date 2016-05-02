namespace TestSite.PL.WebPages.Models
{
    using BLL.Contract;

    public static class LogicProvider
    {
        public static IEmployeeLogic EmployeeLogic { get; } = new BLL.Main.EmployeeMainLogic();

        //public static IRoleLogic RoleLogic { get; } = new BLL.Main.RoleMainLogic();

        public static IDepLogic DepLogic { get; } = new BLL.Main.DepMainLogic();

        public static ITestLogic TestLogic { get; } = new BLL.Main.TestMainLogic();

        public static IQuestionLogic QuestionLogic { get; } = new BLL.Main.QuestionMainLogic();

        public static IAnswerLogic AnswerLogic { get; } = new BLL.Main.AnswerMainLogic();

        public static IReportLogic ReportLogic { get; } = new BLL.Main.ReportMainLogic();
    }
}