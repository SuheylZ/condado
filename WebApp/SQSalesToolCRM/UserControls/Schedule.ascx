<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Schedule.ascx.cs" Inherits="UserControls_Schedule" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>

    <%--<ul>
        <li>
            <asp:Label ID="lblSchedule" runat="server" AssociatedControlID="lblScheduleText"
                Text="Schedule:"></asp:Label>
            <asp:Label ID="lblScheduleText" runat="server"></asp:Label>
        </li>
    </ul>--%>

    <fieldset class="condado">
    <legend>Schedule:</legend>
   
    <table class="standardTable" >
        <tr  class= "tableHeader">
            <td>
                Day
            </td>
            <td>
                Shift 1<br />
                <table>
                <tr>
                    <td class="tableTDW102">Start Time </td>
                    <td>End Time </td>
                </tr>
                </table>
                
            </td>
            <td>
                Shift 2<br />
                <table>
                <tr>
                    <td class="tableTDW102">Start Time </td>
                    <td>End Time </td>
                </tr>
                </table>
            </td>
            <td>
                Shift 3<br />
                <table>
                <tr>
                    <td class="tableTDW102">Start Time </td>
                    <td>End Time </td>
                </tr>
                </table>
            </td>
            <td>
                Options
            </td>
        </tr>
        <tr>
            <td>
                Sunday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlSundayS1StartTime" runat="server" Width="100px" Culture="en-US">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlSundayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlSundayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlSundayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlSundayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlSundayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkSundayClear" runat="server" CausesValidation="False" OnClick="hlnkSundayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkSundayCopy" runat="server" CausesValidation="False" OnClick="hlnkSundayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                Monday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlMondayS1StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlMondayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlMondayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlMondayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlMondayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlMondayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkMondayClear" runat="server" CausesValidation="False" OnClick="hlnkMondayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkMondayCopy" runat="server" CausesValidation="False" OnClick="hlnkMondayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                Tuesday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlTuesdayS1StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlTuesdayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlTuesdayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlTuesdayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlTuesdayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlTuesdayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkTuesdayClear" runat="server" CausesValidation="False" OnClick="hlnkTuesdayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkTuesdayCopy" runat="server" CausesValidation="False" OnClick="hlnkTuesdayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                Wednesday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlWednesdayS1StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlWednesdayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlWednesdayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlWednesdayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlWednesdayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlWednesdayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkWednesdayClear" runat="server" CausesValidation="False" OnClick="hlnkWednesdayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkWednesdayCopy" runat="server" CausesValidation="False" OnClick="hlnkWednesdayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                Thursday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlThursdayS1StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlThursdayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlThursdayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlThursdayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlThursdayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlThursdayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkThursdayClear" runat="server" CausesValidation="False" OnClick="hlnkThursdayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkThursdayCopy" runat="server" CausesValidation="False" OnClick="hlnkThursdayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                Friday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlFridayS1StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlFridayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlFridayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlFridayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlFridayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlFridayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkFridayClear" runat="server" CausesValidation="False" OnClick="hlnkFridayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkFridayCopy" runat="server" CausesValidation="False" OnClick="hlnkFridayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                Saturday
            </td>
            <td>
                <telerik:RadTimePicker ID="tlSaturdayS1StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlSaturdayS1EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlSaturdayS2StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlSaturdayS2EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <telerik:RadTimePicker ID="tlSaturdayS3StartTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
                &nbsp;<telerik:RadTimePicker ID="tlSaturdayS3EndTime" runat="server" Width="100px">
                    <TimeView CellSpacing="-1" Interval="00:30:00" Columns="4">
                    </TimeView>
                </telerik:RadTimePicker>
            </td>
            <td>
                <asp:LinkButton ID="hlnkSaturdayClear" runat="server" CausesValidation="False" OnClick="hlnkSaturdayClear_Click">Clear</asp:LinkButton>
                &nbsp;|
                <asp:LinkButton ID="hlnkSaturdayCopy" runat="server" CausesValidation="False" OnClick="hlnkSaturdayCopy_Click">Copy To All</asp:LinkButton>
            </td>
        </tr>
    </table>
     </fieldset>
