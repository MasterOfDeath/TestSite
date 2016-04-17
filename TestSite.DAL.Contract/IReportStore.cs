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

        ICollection<Report> ListReportsByDate(DateTime start, DateTime end);
    }
}
