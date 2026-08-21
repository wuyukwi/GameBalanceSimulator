namespace GameBalanceSimulator.Core.Reporting;

public interface IReportExporter
{
    string Format { get; }

    string Extension { get; }

    string Export(ReportData data);
}
