namespace TestSite.BLL.Main
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Entites;

    public class ReportMainLogic : IReportLogic
    {
        public int InsertReport(Report report)
        {
            int result = -1;

            try
            {
                this.IsValidReport(report);

                result = Stores.ReportStore.InsertReport(report);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public ICollection<Report> ListReportsByDate(DateTime start, DateTime end)
        {
            ICollection<Report> result = null;

            if (start == null && end == null)
            {
                throw new ArgumentException($"{nameof(start)} и {nameof(end)} не могут быть отрицательными");
            }

            try
            {
                result = Stores.ReportStore.ListReportsByDate(start, end);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool RemoveReport(int reportId)
        {
            if (reportId < -1)
            {
                throw new ArgumentException($"{nameof(reportId)} не может быть отрицательным");
            }

            try
            {
                Stores.ReportStore.RemoveReport(reportId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        private bool IsValidReport(Report report)
        {
            if (report == null)
            {
                throw new ArgumentException($"{nameof(report)} не может быть null");
            }

            if (report.Id < -1)
            {
                throw new ArgumentException($"{nameof(report.Id)} не может быть отрицательным");
            }

            if (report.TestId < 1)
            {
                throw new ArgumentException($"{nameof(report.TestId)} не может быть отрицательным");
            }

            if (report.EmployeeId < 1)
            {
                throw new ArgumentException($"{nameof(report.EmployeeId)} не может быть отрицательным");
            }

            if (report.Date > DateTime.Now)
            {
                throw new ArgumentException($"Дата не может быть в будущем");
            }

            if (report.ErrCount < 0)
            {
                throw new ArgumentException($"{nameof(report.ErrCount)} не может быть отрицательным");
            }

            if (report.ErrPercent < 0)
            {
                throw new ArgumentException($"{nameof(report.ErrPercent)} не может быть отрицательным");
            }

            return true;
        }
    }
}
