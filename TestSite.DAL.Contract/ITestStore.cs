namespace TestSite.DAL.Contract
{
    using System;
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface ITestStore
    {
        bool InsertTest(Test test);

        bool RemoveTest(int testId);

        Test GetTestById(int testId);

        ICollection<Test> ListAllTests();

        ICollection<Test> ListTestsByDepId(int depId);

        //ICollection<Tuple<int, int>> ListCorrectAnswers(int testId);
    }
}
