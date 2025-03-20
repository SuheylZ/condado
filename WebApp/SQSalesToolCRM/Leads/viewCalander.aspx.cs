using System;

public partial class Leads_viewCalander : AccountBasePage
{
    /// <summary>
    /// Developed By : Imran H
    /// Description: Displays event secheduler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        uc_calandar1.BtnHandler += GetAccoutnId;


    }
    void GetAccoutnId(string selectedAccoutnId)
    {
        if (selectedAccoutnId != string.Empty)
            AccountID = long.Parse(selectedAccoutnId);
    }
}