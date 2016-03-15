﻿namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Entites;

    public class TestMainLogic : ITestLogic
    {
        public bool InsertTest(Test test)
        {
            var result = false;

            try
            {
                this.IsValidTest(test);

                result = Stores.TestStore.InsertTest(test);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Test> ListAllTests()
        {
            ICollection<Test> result = null;

            try
            {
                result = Stores.TestStore.ListAllTests();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveTest(int testId)
        {
            try
            {
                Stores.TestStore.RemoveTest(testId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private bool IsValidTest(Test test)
        {
            if (test == null)
            {
                throw new ArgumentException($"{nameof(test)} не может быть null");
            }

            if (string.IsNullOrWhiteSpace(test?.Name))
            {
                throw new ArgumentException("Название отдела не может быть пустым или состоять из пробелов");
            }

            if (test.Id < -1)
            {
                throw new ArgumentException($"Не легальный номер отдела");
            }

            return true;
        }
    }
}
