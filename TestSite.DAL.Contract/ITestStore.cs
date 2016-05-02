namespace TestSite.DAL.Contract
{
    using System.Collections.Generic;
    using Entites;

    public interface ITestStore
    {
        bool InsertTest(Test test);

        bool RemoveTest(int testId);

        Test GetTestById(int testId);

        ICollection<Test> ListAllTests();

        ICollection<Test> ListTestsByDepId(int depId);
    }
}
