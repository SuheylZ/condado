<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ManageFilters.ascx.cs"
    Inherits="UserControls_ManageFilters" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>

<fieldset id="fldSetGrid" class="condado">
<asp:HiddenField ID="hdnTaqWorkflow" runat="server" />
<asp:HiddenField ID="hdnIsCustomReport" runat="server" />
<asp:HiddenField ID="hdnIsDuplicateCheck" runat="server" />
<asp:HiddenField ID="hdnCustomReportBaseDataID" runat="server" />
    <legend id="lblTitle" runat="server">Send Conditions Note:</legend>
    <ul>
        <li id="Header" runat="server" visible="false">
            <uc1:StatusLabel ID="ctrlStatusLabel" runat="server" Visible="False" />
        </li>
        <li>
            <table>
                <tr>
                    <td style="width: 30%;">
                        Columns
                    </td>
                    <td style="width: 35%;">
                        Operator
                    </td>
                    <td style="width: 30%;">
                        Value
                    </td>
                    <td style="width: 10%;">
                        Options
                    </td>
                </tr>
                <tr>
                    <td>
                      <telerik:RadComboBox ID="ddlFieldsColumn" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlFieldsColumn_SelectedIndexChanged"
                            CausesValidation="False" MaxLength="100" DataTextField="Name" DataValueField="Key" Width="250" 
                            AppendDataBoundItems="True">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="---Select Column---" Value="-1" />
                                <telerik:RadComboBoxItem runat="server" Text="Numeric" Value="0" />
                                <telerik:RadComboBoxItem runat="server" Text="Text" Value="1" />
                                <telerik:RadComboBoxItem runat="server" Text="Date" Value="2" />
                                <telerik:RadComboBoxItem runat="server" Text="Table" Value="3" />
                                <telerik:RadComboBoxItem runat="server" Text="CheckBox" Value="4" />
                                <telerik:RadComboBoxItem runat="server" Text="DateTime" Value="5" />
                            </Items>
                        </telerik:RadComboBox>     
                    </td>
                    <td style="width: 30%;">
                        <asp:DropDownList ID="ddlOperators" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlOperators_SelectedIndexChanged"
                            Width="200px" Visible="false">
                            <asp:ListItem Value="0">Equal to</asp:ListItem>
                            <asp:ListItem Value="1">Not Equal to</asp:ListItem>
                            <asp:ListItem Value="2">Less than</asp:ListItem>
                            <asp:ListItem Value="3">Less than or equal to</asp:ListItem>
                            <asp:ListItem Value="4">Greater than</asp:ListItem>
                            <asp:ListItem Value="5">Greater than or equal to</asp:ListItem>
                            <asp:ListItem Value="6">Contains</asp:ListItem>
                            <asp:ListItem Value="7">Does not contains</asp:ListItem>
                            <asp:ListItem Value="8">Within</asp:ListItem>
                            <asp:ListItem Value="9">Not Within</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <div id="divNumericValue" runat="server" visible="false">
                            <telerik:RadNumericTextBox ID="txtValueNumeric" runat="server" Width="100px" MinValue="0"
                                MaxValue="999999999">
                                <NumberFormat GroupSeparator="" DecimalDigits="0" />
                            </telerik:RadNumericTextBox>
                        </div>
                        <div id="divTextValue" runat="server" visible="false">
                            <asp:TextBox ID="txtValueText" runat="server" Width="100px"></asp:TextBox>
                        </div>
                        <div id="divCheckBox" runat="server"  visible="false">
                            <asp:CheckBox ID="chkValue" runat="server" />
                        </div>
                        <div id="divDate" runat="server" visible="false">
                            <telerik:RadDatePicker ID="tlDateOnlyValue" runat="server" >
                                <Calendar ID="tlDateOnlyCalendar" runat="server">
                                </Calendar>
                            </telerik:RadDatePicker>
                        </div>
                        <div id="divDateTime" runat="server" visible="false">
                            <telerik:RadDateTimePicker ID="tlDateInputValue" runat="server" 
                                Width="200px">
                                <Calendar ID="tlCalendar" runat="server">
                                </Calendar>
                            </telerik:RadDateTimePicker>
                        </div>
                        <div id="divLookupTableValue" runat="server" visible="false">
                            <asp:DropDownList ID="ddlLookupTableValue" runat="server" DataTextField="Title" DataValueField="Key">
                            </asp:DropDownList>
                        </div>
                        <div id="divLookupTableMultiSelect" runat="server" visible="false">
                            <asp:ListBox ID="lstBoxLookupMultitext" runat="server" SelectionMode="Multiple" DataTextField="Title"
                                DataValueField="Key"></asp:ListBox>
                        </div>
                        <div id="divDateWithInorNotValue" runat="server" visible="false">
                            <table>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="rdBtnCriteriaWithinOrNotFirst" runat="server" ValidationGroup="Criteria"
                                            GroupName="DateWithin" Checked="True" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCriteria" runat="server" Width="70px">
                                            <asp:ListItem Value="0">Today</asp:ListItem>
                                            <asp:ListItem Value="1">Since Monday</asp:ListItem>
                                            <asp:ListItem Value="2">This calendar month</asp:ListItem>
                                            <asp:ListItem Value="3">This calendar year</asp:ListItem>
                                            <asp:ListItem Value="4">In past</asp:ListItem>
                                            <asp:ListItem Value="5">In future</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rdCriteriaWithinOrNotSecond" runat="server" ValidationGroup="Criteria"
                                            GroupName="DateWithin" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlLastNext" runat="server" Width="50px">
                                            <asp:ListItem Value="0">Last</asp:ListItem>
                                            <asp:ListItem Value="1">Next</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtDayHourMin" runat="server" Width="40px" MinValue="0"
                                            MaxValue="999999999">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlDayHourMin" runat="server" Width="70px">
                                            <asp:ListItem Value="0">Days</asp:ListItem>
                                            <asp:ListItem Value="1">Hours</asp:ListItem>
                                            <asp:ListItem Value="2">Minutes</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td class="tableTDCenterMiddle">
                        <br />
                        <asp:Button ID="btnFilterAdd" runat="server" OnClick="btnFilterAdd_Click" Text="Add Filter"
                            CausesValidation="False" Enabled= "false" />
                        <asp:Button ID="btnUpdateFilter" runat="server" Text="Update Filter" Visible="False"
                            OnClick="btnUpdateFilter_Click" />
                        <br />
                        <br />
                        <asp:Button ID="btnCancelFilter" runat="server" Text="Cancel" Visible="False" OnClick="btnCancelFilter_Click" />
                    </td>
                </tr>
            </table>
        </li>
    </ul>
</fieldset>
<fieldset class="condado">
    <legend>Current Filters: </legend>
    <asp:GridView ID="grdTemplateFilters" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        CssClass="mGrid" GridLines="None" Width="100%" DataKeyNames="Key" 
        OnRowCommand="grdEmailFilters_RowCommand">
        <AlternatingRowStyle CssClass="alt" />
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Key" Visible="False" />
            <asp:TemplateField Visible ="false">
                <ItemTemplate>
                    <asp:Label ID="lblSRNO" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="15%" />
            </asp:TemplateField>
             <asp:BoundField DataField="OrderNumber" HeaderText="">                
                <ItemStyle HorizontalAlign="Center" Width="3%"/>
            </asp:BoundField>
            <asp:BoundField DataField="FilterText" HeaderText="Current Filters">
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Options">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                        Text="Edit"></asp:LinkButton>
                    |
                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                        Text="Delete" OnClientClick="if(confirm('Are you sure want to delete filter?')== true) true; else return false;"></asp:LinkButton>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="10%" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            No record found.
        </EmptyDataTemplate>
        <HeaderStyle CssClass="gridHeader" />
        <PagerSettings Position="Top" />
        <PagerStyle VerticalAlign="Bottom" />
    </asp:GridView>
    <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
    <asp:HiddenField ID="hdnFieldSelectedDataType" runat="server" />
    <asp:HiddenField ID="hdnFieldEditFilterKey" runat="server" />
    <br />
    <asp:HiddenField ID="hdnFieldAddedBy" runat="server" />
    <asp:HiddenField ID="hdnFieldChangedBy" runat="server" />
    <asp:HiddenField ID="hdnFieldParentkey" runat="server" />
    <asp:HiddenField ID="hdnFieldParentType" runat="server" />
</fieldset>
