namespace TestSite.BLL.Main
{
    using DAL.Contract;
    using DAL.Sql;
    using DAL.Sqlite;

    internal static class Stores
    {
        public static IDepStore DepStore { get; } = new DepSqliteStore();

        public static IEmployeeStore EmployeeStore { get; } = new EmployeeSqliteStore();

        public static IRoleStore RoleStore { get; } = new RoleSqliteStore();

        public static ITestStore TestStore { get; } = new TestSqliteStore();

        public static IQuestionStore QuestionStore { get; } = new QuestionSqliteStore();

        public static IAnswerStore AnswerStore { get; } = new AnswerSqliteStore();

        public static IReportStore ReportStore { get; } = new ReportSqliteStore();
    }
}
