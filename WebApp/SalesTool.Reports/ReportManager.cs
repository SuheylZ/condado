using System;
using System.Linq;
using System.Collections.Generic;


namespace SalesTool.Reports
{
    public class ReportManager
    {
        Guid _userId = Guid.Empty;
        Dictionary<string, object> _params = new Dictionary<string, object>();
        int _type = 0;

        public ReportManager(int AppType, Guid userId)
        {
            _userId = userId;
            _type = AppType;
        }
        public Telerik.Reporting.Report GetReport(int value)
        {
            //  1	Scorecard
            //  2	Stack Rankings
            //  3	CPA Report
            //  4	Pipeline Report
            //  5	Incentive Tracking Report
            //  6	Quota Tracking Report
            //  7	Commision Dashboard
            //  8	Lead Volume Report
            //  9	Goal Report
            //  10	Case Specialist Dashboard
            //  11	Submission/Enrolled Report
            //  13	Premium Report
            //  14	Carrier Report
            //  15	Fill Form Speed
            //  16	Falloff Report
            //  17	Prioritized List Report


            Telerik.Reporting.Report report = null;

            switch (value)
            {
                case 1: report = new SalesTool.Reports.Scorecard(); break;
                case 2: report = new SalesTool.Reports.StackRankings(); break;
                case 3: report = new SalesTool.Reports.CPAByAgent(); break;
                case 4: report = new SalesTool.Reports.Pipeline(); break;
                case 5: report = new SalesTool.Reports.IncentiveTracking(); break;
                case 6: report = new SalesTool.Reports.QuotaTracking(); break;

                case 7: report = new SalesTool.Reports.Commision(); break;
                case 8: report = new SalesTool.Reports.LeadVolume(); break;
                case 9: report = new SalesTool.Reports.Goal(); break;
                case 10: report = new SalesTool.Reports.CaseSpecialist(); break;
                case 11: report = new SalesTool.Reports.SubmitEnrollment(); break;
                case 13: report = new SalesTool.Reports.Premium(); break;
                case 14: report = new SalesTool.Reports.CarrierMix(); break;
                case 15: report = new SalesTool.Reports.FillFormSpeed(); break;
                case 16: report = new SalesTool.Reports.Falloff(); break;
                case 17: report = new SalesTool.Reports.PrioritizedListReport(); break;
                
                default: report = new SalesTool.Reports.Sample(); break;
            }

            if (Guid.Empty != _userId) 
                report.ReportParameters["userkey"].Value = _userId.ToString();
            else
                report.ReportParameters["userkey"].Value = DBNull.Value;
            if(report.ReportParameters.Contains("type"))
                report.ReportParameters["type"].Value=_type;

            if (_params.Count > 0)
                foreach (string key in _params.Keys)
                {
                    if(report.ReportParameters.Contains(key))
                        report.ReportParameters[key].Value = (_params[key] == null) ? DBNull.Value : _params[key];
                }

            return report;
        }
        public void Add(string paramName, object paramValue=null)
        {
            _params.Add(paramName, paramValue);
        }
    }
}
