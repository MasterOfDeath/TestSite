namespace TestSite.BLL.Contract
{
    using System;
    using System.Collections.Generic;
    using Entites;

    public interface IReportLogic
    {
        int InsertReport(Report report);

        bool RemoveReport(int reportId);

        ICollection<Report> ListReportsByDep(int depId, DateTime start, DateTime end);

        ICollection<Report> ListReportsByEmployee(int employeeId, DateTime start, DateTime end);
    }
}
