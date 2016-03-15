namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface ITestStore
    {
        bool InsertTest(Test test);

        bool RemoveTest(int testId);

        ICollection<Test> ListAllTests();
    }
}
