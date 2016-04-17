namespace TestSite.BLL.Contract
{
    using System;
    using System.Collections.Generic;
    using Entites;

    public interface IReportLogic
    {
        int InsertReport(Report report);

        bool RemoveReport(int reportId);

        ICollection<Report> ListReportsByDate(DateTime start, DateTime end);
    }
}
