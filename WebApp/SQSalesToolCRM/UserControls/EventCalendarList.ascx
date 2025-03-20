<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EventCalendarList.ascx.cs"
    Inherits="Leads_UserControls_EventCalendarList" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>

<style type="text/css">
    .hiddencol
    {
        display: none;
    }
</style>

<asp:HiddenField ID="hdSortColumn" runat="server" />
<asp:HiddenField ID="hdSortDirection" runat="server" />
<asp:HiddenField ID="hdnDisplayPagingBar" runat="server" />
<asp:HiddenField ID="hdnCurrentEventID" runat="server" />
<asp:HiddenField ID="hdnShowOptionButtons" runat="server" />
<asp:HiddenField ID="hdnShowCurrentUserEvents" runat="server" />
<asp:HiddenField runat="server" ID="hdnEventType" Value="-1"/>
<asp:HiddenField runat="server" ID="hdnEventTypeFilter" Value="false"/>
<div class="condado">
    <div id="mainDv">
        <div runat="server" id="divGrid" visible="true" class="Toolbar">
            <div id="fldSetGrid" class="condado">
                <table>
                    <tr>
                        <td nowrap="nowrap">
                            <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                OnIndexChanged="Evt_PageNumberChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblMessageGrid" runat="server" CssClass="LabelMessage"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-top: 0px">
                            <asp:Panel ID="Panel1" runat="server" style="width: 300px!important">
                              
                            </asp:Panel>
                            <asp:Panel ID="pnlCalendar" ScrollBars="Vertical"  runat="server" Height="100%"  Width="100%">
                           <%--BorderColor="#5D7B9D" BorderWidth="2px"--%>
                            <%--AutoGenerateSelectButton="True"  OnSelectedIndexChanged="grdEventCalendar_SelectedIndexChanged"--%>
                            <asp:GridView ID="grdEventCalendar"  runat="server"  DataKeyNames="ID" CssClass="grdEventCalendar"
                                OnPageIndexChanging="grdEventCalendar_PageIndexChanging" OnSorting="grdEventCalendar_Sorting"
                                OnRowCommand="grdEventCalendar_RowCommand" AutoGenerateColumns="false" AllowSorting="True"
                                GridLines="None" PageSize="10" AlternatingRowStyle-CssClass="alt"  OnRowCreated="grdEventCalendar_RowCreated"
                                BorderStyle="Solid" CellPadding="3" Font-Names="Verdana" 
                                Font-Size="10pt" OnRowDataBound="grdEventCalendar_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="AccountID" HeaderText="Account ID" SortExpression="AccountID"
                                        ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                    <%--<asp:BoundField DataField="" HeaderText ="rowindex"--%>
                                    <asp:TemplateField HeaderText="Date" SortExpression="PolicyType">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDate" runat="server" Text='<%# Bind("Date") %>' CssClass="eventcalandar_date">
                                            </asp:Label>
                                            <div id="dv1" runat="server" style="position: absolute">
                                            </div>
                                            <ajax:PopupControlExtender ID="PopupControlExtender1" runat="server" PopupControlID="Panel1" OffsetX="-120"
                                                TargetControlID="dv1" DynamicContextKey='<%# Eval("ID") %>' DynamicControlID="Panel1"
                                                DynamicServiceMethod="GetDynamicContent" DynamicServicePath="~/ServiceMethods.aspx" Position="Bottom">
                                            </ajax:PopupControlExtender>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Title" SortExpression="Title">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTitlet" runat="server" Text='<%# Bind("Title") %>' CssClass="eventcalandar_title">
                                            </asp:Label>                                           
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <%--   <asp:TemplateField HeaderText="UserName" SortExpression="UserName">
            <ItemTemplate>
                <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>'>
                </asp:Label>
            </ItemTemplate>
            <HeaderStyle HorizontalAlign="Left" />
        </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Status" SortExpression="Status">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblStatus" runat="server" Text='<%# Bind("Status") %>'>
                                            </asp:Label>--%>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%# GetEventStatus((int)(DataBinder.Eval(Container.DataItem, "Status"))) %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    
                                    
                                     <asp:TemplateField >
                                        <ItemTemplate>
                                           <asp:ImageButton ID="lnkSelect" runat="server" CausesValidation="false" CommandName="SelectX" ImageUrl="~/Images/rowSelect.png" ToolTip="Select Account" ></asp:ImageButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>


                                                                        <%--Enabled='<%1# Bind("IsCompleted") %>'--%>
                                    <%--    <asp:TemplateField HeaderText="Options" >
            <ItemTemplate>

                <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX" Text="E"></asp:LinkButton>
                <asp:LinkButton ID="lnkComplete" runat="server" CausesValidation="false" CommandName="CompleteX" Text="C" 
                OnClientClick="if(confirm('Are you sure to change the status to Completed?')== true){ return true;} else { return false;}"></asp:LinkButton>
                <asp:LinkButton ID="lnkOutlook" runat="server" CausesValidation="false" CommandName="OutlookX" Text="O" Enabled="false" ></asp:LinkButton>
                <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX" Text="D" 
                    OnClientClick="if (!confirmDelete()) return false;"></asp:LinkButton>
            </ItemTemplate>
            <HeaderStyle HorizontalAlign="Left" />
        </asp:TemplateField>--%>
                                </Columns>
                                <EmptyDataTemplate>
                                    There are no events to display.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" />
                            </asp:GridView>
                                 </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>

<%-- Attempted to convert the onmouseover to jQuery mouseenter to reduce ajax posts --%>
<script type="text/javascript" >

    $(document).ready(function () {
        //console.log("ready!");

        $("table.grdEventCalendar tr").mouseenter( function () {
            //console.info("##### MOUSE ENTER CALLED - JQUERY");
            if (this.onmouseout == undefined || this.onmouseout == "") {
                return;
            }

            var code = this.onmouseout.toString();
            var start = code.indexOf("('") + 2;
            var end = code.indexOf("')");
            var id = code.substr(start, end - start);
            $find(id).showPopup();
            
            
        });

        $("table.grdEventCalendar tr").mouseleave(function () {
            //console.info("##### MOUSE Leave CALLED - JQUERY");
            if (this.onmouseout == undefined || this.onmouseout == "") {
                return;
            }

            var code = this.onmouseout.toString();
            var start = code.indexOf("('") + 2;
            var end = code.indexOf("')");
            var id = code.substr(start, end - start);
            $find(id).hidePopup();


        });
    });
    
</script>