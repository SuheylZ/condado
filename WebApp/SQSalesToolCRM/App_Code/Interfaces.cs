using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public enum StatisticsMetricsType { Unknown = 0, Sales = 1, Leads = 2, CallCenter = 3 };
public enum PhoneType
{
	Unknown = 0, Cell = 1, Landline = 2, Skype = 3
}
public enum PhoneActionOn
{
	Unknown =0, Cell =1, Landline =2, Skype =3, Cell_Landline=4, Cell_Unknown=5, Landline_Unknown =6
}

public enum CallType // SZ [Aug 27, 2013] Added for the call type paramter, better than using integers
{
	Unknown = 0,
	Inbound = 1,
	Outbound = 2,
	Manual = 3
}
public enum TRType
{
	Unknown = 0,
	Basic = 1,  //SZ [Apr 28, 2014] Assigned User
    Xfer = 2, //SZ [Apr 28, 2014] Transfer Agent
    CSR = 3, //SZ [Apr 28, 2014] CSR User
    AP = 4,  //SZ [Apr 28, 2014] Alternative Product User
    OB = 5 //SZ [Apr 28, 2014] Onboard User
};


public interface IIndividualNotification
{
	void IndividualChanged(IIndividual handle);
}
public interface IIndividual
{

	void Notify(IIndividualNotification client);
	IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals> Individuals { get; }
	void UpdateIndividuals(bool bFireEvent = true);
}
public interface IStatisticsMetricsHandler
{
	void MetricsChanged(StatisticsMetricsType current);
}
public interface IStatisticsMetricsNotifier
{
	void Register(IStatisticsMetricsHandler handler); 
}

// SZ [Aug 28, 2013] has been added to set the writting agent to the default
public interface IWrittingAgentSet 
{
	void SetAgent(System.Guid agentId);
}


public interface ICalenderNotification
{
	void EventsUpdated();
}
public interface ICalenderNotifier
{
	void Register(ICalenderNotification observer);
}


public class BooleanArgs : EventArgs { public bool Choice { get; set; } public BooleanArgs(bool val) { Choice = val; } };
public class LongArgs : EventArgs { public long Id { get; set; } public LongArgs(long id) { Id = id; } };
public class CharArgs : EventArgs { public char Value { get; set; } public CharArgs(char ch) { Value = ch; } };
public class TCPAConsentArgs : EventArgs { public TCPAConsentType Value { get; set; } public TCPAConsentArgs(TCPAConsentType type) { Value = type; } };


public interface ITCPAProvider
{
	int Register(ITCPAClient obj);
	void InvokeTCPA(object sender, int clientId);
	bool IsTCPAEnabled{ get; }
}

public interface ITCPAClient
{
	void ProcessConsent(string controlId, TCPAConsentType choice);
}

public interface IDialogBox
{
	Telerik.Web.UI.RadWindow GetWindow();
}
	