<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChangeStatus.ascx.cs" Inherits="Leads_UserControls_ChangeStatus" %>


<fieldset style="margin:10px">
    <div id="sldOuter">
        <div>
            <table width="100%">
                <tr>
                    <td style="vertical-align:top">Status</td>
                    <td>
                        <telerik:RadComboBox ID="ddlStatus" OnClientSelectedIndexChanged="OnClientSelectedIndexChanged" runat="server" DataTextField="Status" DataValueField="ID" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true"
                            Height="200" Width="200" DropDownWidth="250">
                        </telerik:RadComboBox> 
                         <asp:RequiredFieldValidator ID="vldYear" runat="server"  InitialValue="" Display="Dynamic"
                              ValidationGroup="StatusValidationGroup"  ErrorMessage="*" ForeColor="Red" ControlToValidate="ddlStatus"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td  style="vertical-align:top">Include Sub Status?</td>
                    <td>
                        <asp:CheckBox ID="chkInclueSubStatus" onclick="javascript:return IncludeSubStatus(this);" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td  style="vertical-align:top">Sub Status</td>
                    <td>
                        <telerik:RadComboBox Enabled="false" ID="ddlSubStatus" runat="server" DataTextField="Status" DataValueField="ID"
                            Height="200" Width="200" DropDownWidth="250">
                        </telerik:RadComboBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</fieldset>
