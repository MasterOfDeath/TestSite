namespace TestSite.Entites
{
    using System;

    public class Report
    {
        public Report(int id, int employeeId, int testId, DateTime date, int errCount, int errPercent)
        {
            this.Id = id;
            this.EmployeeId = employeeId;
            this.TestId = testId;
            this.Date = date;
            this.ErrCount = errCount;
            this.ErrPercent = errPercent;
        }

        public Report(int employeeId, int testId, DateTime date, int errCount, int errPercent)
            : this(-1, employeeId, testId, date, errCount, errPercent)
        {
        }

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int TestId { get; set; }

        public DateTime Date { get; set; }

        public int ErrCount { get; set; }

        public int ErrPercent { get; set; }
    }
}
