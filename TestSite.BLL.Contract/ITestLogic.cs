namespace TestSite.BLL.Contract
{
    using System;
    using System.Collections.Generic;
    using Entites;

    public interface ITestLogic
    {
        bool InsertTest(Test test);

        bool RemoveTest(int testId);

        ICollection<Test> ListAllTests();

        ICollection<Tuple<int, int>> ListCorrectAnswers(int testId);
    }
}
