namespace TestSite.DAL.Contract
{
    using System;
    using System.Collections.Generic;
    using TestSite.Entites;

    public interface IReportStore
    {
        int InsertReport(Report report);

        bool RemoveReport(int reportId);

        ICollection<Report> ListAllReports();

        ICollection<Report> ListReportsByDep(int depId, DateTime start, DateTime end);

        ICollection<Report> ListReportsByEmployee(int employeeId, DateTime start, DateTime end);
    }
}
