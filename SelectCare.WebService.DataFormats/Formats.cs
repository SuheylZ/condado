using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Summary description for Data
/// </summary>



[Serializable()]
//[XmlRoot("IndividualData")]
//[XmlType("IndividualData")]
public class IndividualData
{
    public string SourceCode;
    public string Title;
    public string FirstName;
    public string MiddleName;
    public string LastName;
    public string Suffix;
    public string Gender;
    public string State;
    public string Birthdate;
    //MH:26 March 2014
    public string indv_key;
    public string AccountKey;
};


[Serializable()]
public class AccountHistoryData
{
    //public long? AccountId { get; set; }
    public DateTime? AddedOn;
    public string Comment;
    public string Entry;
    //public byte? EntryType { get; set; }
    public string FullName;
    //public long Key { get; set; }
    //public Guid? UserKey { get; set; }
};



//public void X(){
//Arc.ArcServerClass Service = new Arc.ArcServerClass();
//Service.NewCase(L.SourceCode, "", L.Account.PrimaryIndividual.FirstName, "", L.Account.PrimaryIndividual.LastName, "", L
//    .Account.PrimaryIndividual.Gender, USStates.
//    Where(x => x.Id == L.Account.PrimaryIndividual.StateID).FirstOrDefault().FullName, 
//    L.Account.PrimaryIndividual.Birthday.Value.ToShortDateString());
//}

//}