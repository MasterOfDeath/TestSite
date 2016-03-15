namespace TestSite.BLL.Main
{
    using DAL.Contract;
    using DAL.Sql;
    using DAL.Sqlite;

    internal static class Stores
    {
        public static IUserStore UserStore { get; } = new UserSqlStore();

        public static IPhotoStore PhotoStore { get; } = new PhotoSqlStore();

        public static IAlbumStore AlbumStore { get; } = new AlbumSqlStore();

        public static ILikeStore LikeStore { get; } = new LikeSqlStore();

        public static IDepStore DepStore { get; } = new DepSqliteStore();

        public static IEmployeeStore EmployeeStore { get; } = new EmployeeSqliteStore();

        public static IRoleStore RoleStore { get; } = new RoleSqliteStore();

        public static ITestStore TestStore { get; } = new TestSqliteStore();

        public static IQuestionStore QuestionStore { get; } = new QuestionSqliteStore();
    }
}
