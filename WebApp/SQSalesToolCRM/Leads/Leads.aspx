<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="Leads.aspx.cs" Inherits="Leads_Leads" EnableEventValidation="false" ValidateRequest="false" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="~/Leads/UserControls/IndividualsInformation.ascx" TagName="IndvInfo"
    TagPrefix="IndvInfo1" %>
<%@ Register Src="~/Leads/UserControls/HomeInformation.ascx" TagName="HomeInfo" TagPrefix="HomeInfo1" %>
<%@ Register Src="~/Leads/UserControls/VehicleInfo.ascx" TagName="VehicleInfo" TagPrefix="VehicleInfo1" %>
<%@ Register Src="~/Leads/UserControls/applicationInformation.ascx" TagName="AppInfo"
    TagPrefix="AppInfo1" %>
<%@ Register Src="~/Leads/UserControls/mapdpInformation.ascx" TagName="MapdpInfo"
    TagPrefix="MapdpInfo1" %>
<%@ Register Src="~/Leads/UserControls/dentalVisionInformation.ascx" TagName="DentInfo"
    TagPrefix="DentInfo1" %>
<%@ Register Src="~/Leads/UserControls/AutoHomePolicy.ascx" TagName="AutoHomePolicy"
    TagPrefix="AutoHomePolicy1" %>
<%@ Register Src="~/Leads/UserControls/MedicalSupplement.ascx" TagName="PolicyInfo"
    TagPrefix="PolicyInfo1" %>
<%@ Register Src="~/Leads/UserControls/carrierIssuesInformation.ascx" TagName="CarrierIssueInfo"
    TagPrefix="CarrierIssueInfo1" %>
<%@ Register Src="~/Leads/UserControls/driverInformation.ascx" TagName="DriverInfo"
    TagPrefix="DriverInfo1" %>
<%@ Register Src="~/Leads/UserControls/LeadsMarketing.ascx" TagName="LeadsMarketing"
    TagPrefix="LeadsMarketing1" %>
<%@ Register Src="UserControls/AccountAttachments.ascx" TagName="AccountAttachments"
    TagPrefix="uc2" %>
<%@ Register Src="UserControls/AutoHomeQuote.ascx" TagName="AutoHomeQuote" TagPrefix="uc3" %>
<%@ Register Src="UserControls/EventCalendarAddEdit.ascx" TagName="EventCalendarAddEdit"
    TagPrefix="ec" %>

<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc4" %>
<%@ Register Src="../UserControls/AlertConsent.ascx" TagName="AlertConsent" TagPrefix="uc5" %>
<%@ Register Src="UserControls/IndividualBox.ascx" TagName="IndividualBox" TagPrefix="uc6" %>
<%@ Register Src="../UserControls/EmailSender.ascx" TagName="EmailSender" TagPrefix="uc7" %>
<%@ Register Src="~/Leads/UserControls/ArcCases.ascx" TagPrefix="ArcCases" TagName="ArcCases" %>
<%@ Register Src="~/Leads/UserControls/ArcHistory.ascx" TagPrefix="ArcHistory" TagName="ArcHistory" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/Leads/UserControls/IndividualsAddEdit.ascx" TagName="IndividualsAddEdit"
    TagPrefix="uc" %>
<%@ Register Src="~/UserControls/RadWindowControl.ascx" TagName="RadWindowControl"
    TagPrefix="RadWindowControl" %>
<%@ Register Assembly="PhoneBarUserControl" Namespace="PhoneBarUserControl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>--%>
    <script type="text/javascript" src="../Scripts/json2.js"></script>
    <style type="text/css">
        .RadWindow.rwInactiveWindow {
            opacity: 1 !important;
            filter: none !important;
        }

        a.rwIcon {
            background-image: none !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server" style="background-color: White; overflow-y: visible; width: 100%; height: 100%">
    <script type="text/javascript" src="../Scripts/leads.js"></script>
    <asp:HiddenField ID="hdnNextAccountReportClicked" runat="server" />
    <asp:HiddenField ID="hdnShowTimerAlert" runat="server" />
    
     

    <script type="text/javascript">        
        
        initArcHub();
        
        function OpenArcCase()
        {   
            var CurrentArcReferenceID = document.getElementById('<%= hdnCurrentArcReferenceID.ClientID %>');            
            openArc(CurrentArcReferenceID.value, GetAccountId(), '<%= CurrentAccount==null? null:CurrentAccount.PrimaryIndividualId %>' );
        }
        function getGuid() { return '<%= CurrentUser.Key.ToString() %>' }
        function IsArcNewImplementationToUse()
        {
            var IsNewImplementation = document.getElementById('<%= hdnUseArcNewImplementation.ClientID %>').value;
            if (IsNewImplementation == "False") return false;
            else return true;
        }
        function GetArcHubPath() {
            return "<%= hdnArcHubPath.Value %>/signalr";            
        }
    </script>
    <script type="text/javascript">
        $(function () {
            $('#leadheader-slider').click(function () {

                if ($("#leadheader").height() > 23) {
                    $("#leadheader").animate({ "height": "23px" }, "slow");
                    $(this).text('More');
                } else {
                    $("#leadheader").animate({ "height": "71px" }, "slow");
                    $(this).text('Less');
                }
                return false;
            });
        });


        function validateDropdownStatus(e) {
            $('#<%=btnApplyAction.ClientID%>').attr('disabled', 'true');
            if ($(e).val() == "") {
                return false;
            }
            setTimeout('__doPostBack(\'ctl00$MainContent$ddActions\',\'\')', 0);
            return true;
        }
        function validateActionStatus() {
            if (!Page_ClientValidate("indp")) return false;
            var val = $('#MainContent_ddActions').val();
            if (val == "") {
                radalert("Action can not empty", 340, 100, "Apply Action", '', '');
                return false;
            }
            //return true;
        }
        loger = new Logger();
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var senderButton = "";
        prm.add_beginRequest(function (s, e) {
            sender = e.get_postBackElement().id;
            if (sender == '<%=ddActions.ClientID%>' || sender === ('<%=tmrDisableAction.ClientID%>')) {
                $('#pnlLoader').hide();
            }
            else
                $('#pnlLoader').show();
        });



        prm.add_endRequest(function (s, e) {

            if (senderButton != "" && senderButton == "MainContent_btnNewArcCall") alert(senderButton + $('#<% =lblAccountID.ClientID%>').text());
            //$(function () {
            //    $('#<%=ddActions.ClientID%>')
            //        .on("change", getActionTimer('#<%=btnApplyAction.ClientID%>'));
            //});
            $(function () {
                $('#<%=ddActions.ClientID%>').on("change", function (e) {
                    $('#<%=btnApplyAction.ClientID%>').attr('disabled', 'disabled');
                });
            });

            $(function () {
                $('#leadheader-slider').click(function () {
                    if ($("#leadheader").height() > 23) {
                        $(this).text('More');
                        $("#leadheader").animate({ "height": "23px" }, "slow");

                    } else {
                        $(this).text('Less');
                        $("#leadheader").animate({ "height": "66px" }, "slow");
                    }
                    return false;
                });
            });
        });
        function GridCreated(sender, args) {
            var scrollArea = sender.GridDataDiv;
            var parent = $get("gridContainer");
            var gridHeader = sender.GridHeaderDiv;
            scrollArea.style.height = parent.clientHeight -
              gridHeader.clientHeight + "px";
        }
        function GetAccountId() {
            return $('#<% =lblAccountID.ClientID%>').text();
        }
        function SetAccountId(id) {
            //alert(id);
            $('#<% =lblAccountID.ClientID%>').text(id);
        }

        function GetSelectCareServiceURL(func) {
            return '<%= Request.Url.ToString().Replace(Request.RawUrl.ToString(), "")%>' + '/Services/SelectCare.asmx/' + func;
        }

        function GetStringifyDataForArc() {
            var parentActId = "<%=Request.ReadQueryStringAs<long?>("parentaccountid")%>";
            var birthdateCtl = $find('<%= diDOB.ClientID%>');
            var appdateCtl = $find('<%= diDOB2.ClientID%>');
            var fname1 = $('#<%=txtFName.ClientID%>').val(),
                lname1 = $('#<%=txtLName.ClientID%>').val(),
                //MH:30 April
                mName = $('#<%=txtMiddleName.ClientID%>').val(),
                dayPhone = $find('<%=txtDayTimePhNo.ClientID%>').get_value(),
                evenPhone = $find('<%=txtEvePhNo.ClientID%>').get_value(),
                cellPhone = $find('<%=txtCellPhNo.ClientID%>').get_value(),
                email = $('#<%=txtEmailIndv.ClientID%>').val(),
                emailOptOut = $('#<%=chkEmailOptOutPrimary.ClientID%>').is(':checked'),
                address1 = $('#<%=txtAddress1Primary.ClientID%>').val(),
                address2 = $('#<%=txtAddress2Primary.ClientID%>').val(),
                city = $('#<%=txtCityPrimary.ClientID%>').val(),
                state = $('#<%=ddlStatePrimary.ClientID%>').val(),
                zipCode = $('#<%=txtZipCodePrimary.ClientID%>').val(),
                appDate = appdateCtl.get_selectedDate(),
                gender1 = $('#<%=ddlGenderP.ClientID%>').val(),
                consent = $('#<%=ddlConsent.ClientID%>').val(),
                state1 = $('#<%=ddlApplicationState.ClientID%>').val(),
                dob1 = birthdateCtl.get_selectedDate();

        var obj = JSON.stringify({
            id: GetAccountId(),
            fname: fname1,
            lname: lname1,
            mName: mName,
            dayPhone: dayPhone,
            evenPhone: evenPhone,
            cellPhone: cellPhone,
            email: email,
            emailOptOut: emailOptOut,
            address1: address1,
            address2: address2,
            city: city,
            state: state,
            zipCode: zipCode,
            appDate: appDate,
            gender: gender1,
            appState: state1,
            consent: consent,
            dob: dob1,
            parentActId: parentActId
        });
            //MH:06 March 2014
        if (loger != null && loger != undefined)
            loger.info("Collecting ArcCase data from leads page " + obj);
        return obj;
    }

    //SR to make the modal windows center.
    function pageLoad() {
        $(window).resize(function () {
            var oWindow = GetRadWindowClientID('Individual');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('CarrierIssueInfo');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('LeadMarketing');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('MedicalSupplement');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('MAPInfo');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('DentalVision');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('Homes');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('DriverInfo');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('VehicleInfo');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('AutoHomePolicy');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
            oWindow = GetRadWindowClientID('Quotes');
            if (oWindow != null && oWindow.isVisible()) {
                oWindow.center();
                return;
            }
        });
    }
    function GetRadWindowClientID(windowName) {
        switch (windowName) {
            case "Individual":
                return $find("<%=IndvInfo1.IndividualWindowClientID %>");
            case "CarrierIssueInfo":
                return $find("<%=CarrierIssueInfo1.CarrierRadWindowClientID %>");
            case "LeadMarketing":
                return $find("<%=LeadsMarketing1.LeadsMarketingRadWindowClientID %>");
                case "MedicalSupplement":
                    return $find("<%=PolicyInfo1.MedicalSupplementRadWindowClientID %>");
                case "MAPInfo":
                    return $find("<%=mapdpInfo1.MAPInformationRadWindowClientID%>");
                case "DentalVision":
                    return $find("<%= dentInfo1.DentalVisionRadWindowClientID%>");
                case "Homes":
                    return $find('<%= HomeInfo1.HomeRadWindowClientID %>');
                case "DriverInfo":
                    return $find('<%= DriverInfo1.DriverInfoRadWindowClientID %>');
                case "VehicleInfo":
                    return $find('<%= VehicleInfo1.VehicleInfoRadWindowClientID %>');
                case "AutoHomePolicy":
                    return $find('<%= AutoHomePolicy1.AutoHomeRadWindowClientID %>');
                case "Quotes":
                    return $find('<%= AutoHomeQuote2.QuoteRadWindowClientID %>');

                default:
                    return;
            }
        }

        // SZ [May 5, 2014] Added for action timer attributes.
        function getActionTimerData() {
            var data = {
                actionId: $('#<%=ddActions.SelectedValue%>').val(),
            accountId: $('#<% =lblAccountID.ClientID%>').text(),
            userId: $('#<%CurrentUser.Key.ToString();%>')
        };
        return JSON.stringify(data);
    }

       
    </script>
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hdInsuranceType" runat="server" />
            <!-- SZ [Apr 18, 2013] do not remove hdnAccountId field at any cost. it fixes the issue of session loss-->

            <asp:HiddenField ID="hdnAccountId" runat="server" />
            <asp:HiddenField ID="hdnCurrentArcReferenceID" runat="server" />
            <asp:HiddenField ID="hdnUseArcNewImplementation" runat="server" />
            <asp:HiddenField ID="hdnArcHubPath" runat="server" />
            <uc5:AlertConsent ID="dlgAlert" runat="server" />
            <uc6:IndividualBox ID="IndividualBox1" runat="server" />

            <telerik:RadWindow ID="dlgEventCalendar" runat="server" Width="750" Height="650"
                Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                IconUrl="../Images/alert1.ico" Title="Calendar Event - Lead Occurrence" VisibleOnPageLoad="false"
                OnClientClose="OnClientClose">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Updatepanel5" runat="server">
                        <ContentTemplate>
                            <ec:EventCalendarAddEdit ID="EventCalendarAddEdit1" runat="server" />
                            <div class="buttons" style="text-align: right">
                                <!--YA[April 18, 2013] Added close button server side event as using client side closedlg will not work properly  -->
                                <asp:Button ID="btnCloseEventCalendar" runat="server" CausesValidation="false" OnClick="btnCloseEventCalendar_Click"
                                    Text="Close" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
            <asp:HiddenField ID="accountIden" runat="server" Value="0" />
            <div style="position: relative;">
                <div style="margin-top: 0px;">
                    <table style="border: 1px solid #C0C0C0; border-collapse: separate">
                        <tr id="t">
                            <td>
                                <div id="leadheader">
                                    <table>
                                        <tr>
                                            <td style="width: 22%"><span class="lead_headerXLable">Source</span>
                                                <asp:DropDownList Width="50%" ID="ddCampaigns" runat="server" DataTextField="Title" DataValueField="ID"
                                                    AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddCampaigns_SelectedIndexChanged" />
                                                <asp:Button ID="btnMore" runat="server" Text="More" CssClass="buttonstyle" CausesValidation="false"
                                                    OnClick="btnMore_Click1" /></td>

                                            <td style="width: 17%"><span class="lead_headerXLable">Status</span>
                                                <asp:DropDownList ID="ddlStatus" runat="server" Width="130px" DataTextField="Title"
                                                    DataValueField="Id" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged" AutoPostBack="true" /></td>

                                            <td style="width: 17%"><span class="lead_headerXLable">Sub Status</span>
                                                <asp:DropDownList ID="ddlSubStatus1" runat="server" Width="130px" DataTextField="Title"
                                                    DataValueField="Id" AutoPostBack="false" /></td>

                                            <td style="width: 18%"><span class="lead_headerSLable" style="width: 85px">Assigned User</span>
                                                <asp:DropDownList ID="ddUsers" runat="server" DataTextField="FullName" DataValueField="Key"
                                                    Width="128px">
                                                    <asp:ListItem Value="-1">-- Unassigned --</asp:ListItem>
                                                </asp:DropDownList></td>

                                            <td style="width: 14%"><span class="lead_headerSLable">CSR</span>
                                                <asp:DropDownList ID="ddCSR" runat="server" DataTextField="FullName" DataValueField="Key"
                                                    Width="128px">
                                                    <asp:ListItem Value="-1">-- Unassigned --</asp:ListItem>
                                                </asp:DropDownList></td>

                                            <td style="width: 15%"><span class="lead_headerSLable">TA</span>
                                                <asp:DropDownList ID="ddTA" runat="server" DataTextField="FullName" DataValueField="Key"
                                                    Width="128px">
                                                    <asp:ListItem Value="-1">-- Unassigned --</asp:ListItem>
                                                </asp:DropDownList></td>


                                        </tr>
                                        <!--2nd row-->
                                        <tr>
                                            <td class="lead_header_column">
                                                <span class="lead_headerXLable">Account:</span>
                                                <asp:Label CssClass="lead_headerXValue" ID="lblAccountID" runat="server" Text="" />
                                            </td>
                                            <td class="lead_header_column">
                                                <span class="lead_headerXLable">Created:</span>
                                                <asp:Label ID="lblDateCreatedText" runat="server" CssClass="lead_headerXValue" Visible="true"></asp:Label>
                                            </td>
                                            <td class="lead_header_column">
                                                <span class="lead_headerXLable">Last Action:</span>
                                                <asp:Label ID="lblLastActionDate" runat="server" CssClass="lead_headerXValue" Visible="true"></asp:Label>
                                            </td>

                                            <td style="width: 18%"><span class="lead_headerSLable" style="width: 85px">Original User</span>
                                                <asp:DropDownList ID="ddOU" runat="server" DataTextField="FullName" DataValueField="Key" Enabled="false"
                                                    Width="128px">
                                                    <asp:ListItem Value="-1">-- Unassigned --</asp:ListItem>
                                                </asp:DropDownList></td>

                                            <td><span class="lead_headerSLable">AP</span>
                                                <asp:DropDownList runat="server" ID="ddAltProduct" DataTextField="FullName" DataValueField="Key" Width="128px" /></td>

                                            <td><span class="lead_headerSLable">OB</span>
                                                <asp:DropDownList runat="server" ID="ddOP" DataTextField="FullName" DataValueField="Key" Width="128px" /></td>

                                        </tr>
                                        <!--3rd row-->
                                        <tr>
                                            <td></td>
                                            <td>
                                                <span class="lead_headerXLable">Modified:</span>
                                                <asp:Label ID="lblModifiedDateText" runat="server" Width="110px" Visible="true"></asp:Label>
                                            </td>
                                            <td>
                                                <span class="lead_headerXLable">Last Call:</span>
                                                <asp:Label ID="lblLastCallDate" runat="server" Width="110px" Visible="true"></asp:Label>
                                                <asp:Label ID="lblFirstAssignedUser" runat="server" Width="110px" Visible="False"></asp:Label>
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td><span class="lead_headerSLable" title="External Agent">EA</span>
                                                <asp:TextBox ID="txtExternalAgent" runat="server" Style="padding: 0px" Width="125px" class="editable" /></td>
                                        </tr>


                                    </table>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <td valign="top">
                                <span id="leadheader-slider">More</span>
                                <!-- Lead and Individual -->
                                <table>
                                    <tr>
                                        <td style="vertical-align: top; width: auto;">
                                            <!-- Lead Panel -->
                                            <asp:Timer runat="server" ID="tmrDisableAction" Enabled="false" />
                                            <asp:Panel ID="pnlLead" runat="server" Style="margin-right: 5px">

                                                <fieldset class="leads leads-fieldset">
                                                    <legend>Actions & History</legend>
                                                    <ul>
                                                        <li>
                                                            <asp:Label ID="lblActions" runat="server" Text="Action" /><br />
                                                            <asp:DropDownList ID="ddActions" runat="server" DataTextField="Name" DataValueField="Value" onChange="return validateDropdownStatus(this)"
                                                                Width="210px" AutoPostBack="true" />


                                                            <asp:Button ID="btnApplyAction" runat="server" CausesValidation="true" Text="Apply" Width="75px" ValidationGroup="indp" OnClick="btnApplyAction_Click" CssClass="buttonstyle" OnClientClick="return validateActionStatus()" />
                                                            <asp:Button ID="btnNextLead" runat="server" CausesValidation="false" Text="Next" Width="75px" Class="buttonstyle" OnClientClick="disableUI();" />
                                                            <asp:Button ID="btnNextLeadfromReport" runat="server" CausesValidation="false" Text="Next(R)" Width="75px" Class="buttonstyle" Visible="false" OnClientClick="disableUI();" />
                                                            <asp:Button ID="btnAddEvent1" runat="server" CausesValidation="false" Text="Events" Width="75px" OnClick="btnAddEvent1_Click" CssClass="buttonstyle" />
                                                            <asp:Button ID="btnEmailSender" runat="server" CausesValidation="false" Text="Email" Width="75px" Class="buttonstyle" />
                                                            <asp:Button ID="btnNewArcCall" runat="server" CausesValidation="true" Text="New Arc" Width="75px" CssClass="buttonstyle" OnClientClick="return newArcCall(0);" UseSubmitBehavior="false" ValidationGroup="indpArc" Visible="false" />
                                                            <asp:Button ID="btnOpenCaseCall" runat="server" CausesValidation="true" Text="Open Case" Width="75px" CssClass="buttonstyle" OnClientClick="OpenArcCase(); return false;" UseSubmitBehavior="false" ValidationGroup="indpArc" Visible="false" CommandArgument="test" />
                                                            <asp:Button ID="btnQuote" runat="server" CausesValidation="false" Text="Quote" Width="75px" CssClass="buttonstyle" OnClick="btnQuote_Click" Visible="false" />
                                                            <asp:Button ID="btnSave" runat="server" CausesValidation="true" Text="Save" Width="75px" Class="buttonstyle" ValidationGroup="indp" />
                                                            <br />
                                                            <br />

                                                            <asp:Label ID="lblNotes" runat="server" Text="Action Notes" />
                                                            <asp:Label ID="lblErrorMsg" runat="server" CssClass="Error" Text="Action Notes required." Visible="false" />
                                                            <br />
                                                            <asp:TextBox ID="tbNotes" runat="server" OnTextChanged="tbNotes_TextChanged" Rows="5" TextMode="MultiLine" Width="100%" />

                                                        </li>
                                                        <li>Account History &nbsp;&nbsp;<asp:RadioButton ID="rbtnAction" runat="server" GroupName="actionbtn"
                                                            AutoPostBack="true" Checked="true" />Action &nbsp;&nbsp;<asp:RadioButton ID="rbtnCalls"
                                                                runat="server" GroupName="actionbtn" AutoPostBack="true" />Calls &nbsp;&nbsp;<asp:RadioButton
                                                                    ID="rbtnLog" runat="server" GroupName="actionbtn" AutoPostBack="true" />Log
                                                            &nbsp;&nbsp;<asp:RadioButton
                                                                ID="rbtnPolicyStatus" runat="server" GroupName="actionbtn" AutoPostBack="true" />Policy Status
                                                            &nbsp;&nbsp;<asp:RadioButton ID="rbtnAll" runat="server" GroupName="actionbtn" AutoPostBack="true" />All
                                                             <telerik:RadGrid ID="grdAccountHistory" runat="server" Height="200px" Width="100%"
                                                                 BorderWidth="1px" AutoGenerateColumns="False" CssClass="mGrid" Skin="" EnableTheming="False"
                                                                 ViewStateMode="Enabled" CellSpacing="0" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt"
                                                                 onfocus="this.blur();" GridLines="None">
                                                                 <MasterTableView Width="100%">
                                                                     <NoRecordsTemplate>
                                                                         No History to display at the moment
                                                                     </NoRecordsTemplate>
                                                                     <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                                                     </RowIndicatorColumn>
                                                                     <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                                                     </ExpandCollapseColumn>
                                                                     <Columns>
                                                                         <telerik:GridBoundColumn DataField="Entry" HeaderText="Entry" HtmlEncode="false" />
                                                                         <telerik:GridBoundColumn DataField="Comment" HeaderText="Comment" UniqueName="Comment" />
                                                                         <telerik:GridBoundColumn DataField="FullName" HeaderText="User" UniqueName="FullName" />
                                                                         <telerik:GridDateTimeColumn DataField="AddedOn" HeaderText="Date" UniqueName="AddedOn" DataType="System.DateTime" DataFormatString="{0:MMM dd, yyyy [h:mm tt]}" />
                                                                         <telerik:GridBoundColumn DataField="pvtitle" HeaderText="PV Rule" UniqueName="pvrule" DataType="System.String" ItemStyle-Width="100px" />
                                                                         <telerik:GridBoundColumn DataField="UserTypes" HeaderText="User Type(s)" UniqueName="usertypes" DataType="System.String" ItemStyle-Width="100px" />
                                                                         <telerik:GridBoundColumn DataField="DNIS" HeaderText="DNIS" UniqueName="DNIS" DataType="System.String" ItemStyle-Width="100px" />
                                                                         <telerik:GridBoundColumn DataField="TalkTime" HeaderText="Talk Time" UniqueName="TalkTime" DataType="System.String" ItemStyle-Width="100px" />
                                                                         <telerik:GridBoundColumn DataField="ContactId" HeaderText="Contact ID" UniqueName="ContactId" DataType="System.String" ItemStyle-Width="100px" />
                                                                     </Columns>
                                                                 </MasterTableView>
                                                                 <ClientSettings>
                                                                     <Scrolling AllowScroll="True" UseStaticHeaders="true"></Scrolling>
                                                                 </ClientSettings>
                                                                 <HeaderStyle CssClass="gridHeader" />
                                                                 <FilterMenu EnableImageSprites="False">
                                                                 </FilterMenu>
                                                             </telerik:RadGrid>
                                                        </li>
                                                    </ul>
                                                </fieldset>
                                            </asp:Panel>
                                        </td>
                                        <td style="vertical-align: top; width: 576px;">
                                            <!-- Primary Individual -->
                                            <asp:Panel ID="pnlPrimary" runat="server" Width="575px">
                                                <fieldset class="condado leads-fieldset">
                                                    <legend runat="server" id="primaryIndividualLegend">Primary Individual Information</legend>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <ul>
                                                                    <li>
                                                                        <asp:Label ID="lblIndividual" runat="server" Text="Individual" AssociatedControlID="ddlIndividual" />
                                                                        <asp:DropDownList ID="ddlIndividual" runat="server" Width="130px" AutoPostBack="True" DataTextField="FullName" DataValueField="AccountId">
                                                                        </asp:DropDownList>
                                                                    </li>
                                                                    <li>

                                                                        <asp:Label ID="lblFName" runat="server" Text="First Name" AssociatedControlID="txtFName" />
                                                                        <asp:TextBox ID="txtFName" runat="server" CausesValidation="True" ValidationGroup="indp"
                                                                            Width="128px" />
                                                                        <asp:RequiredFieldValidator ID="vldFName" runat="server" ControlToValidate="txtFName"
                                                                            Display="None" ErrorMessage="First name is required" ValidationGroup="indp" />
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vldc1" runat="server" TargetControlID="vldFName" />
                                                                        <!--Arc-->
                                                                        <asp:RequiredFieldValidator ID="vldFNameArc" runat="server" ControlToValidate="txtFName"
                                                                            Display="None" ErrorMessage="First name is required" ValidationGroup="indpArc" />
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vldcFNameArc" runat="server" TargetControlID="vldFNameArc" />
                                                                    </li>
                                                                    <li runat="server" id="liMiddleInitial">
                                                                        <asp:Label ID="lblMiddle" runat="server" Text="Middle Name" AssociatedControlID="txtMiddleName" />
                                                                        <asp:TextBox ID="txtMiddleName" runat="server" CausesValidation="False" Width="128px" MaxLength="1" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="lblLastName" runat="server" Text="Last Name" AssociatedControlID="txtLName" />
                                                                        <asp:TextBox ID="txtLName" runat="server" CausesValidation="True" ValidationGroup="indp"
                                                                            Width="128px" />
                                                                        <asp:RequiredFieldValidator ID="vldLastName" runat="server" ControlToValidate="txtLName"
                                                                            Display="None" ErrorMessage="Last name is required" ValidationGroup="indp" />
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server"
                                                                            TargetControlID="vldLastName" />
                                                                        <!--Arc-->
                                                                        <asp:RequiredFieldValidator ID="vldLastNameArc" runat="server" ControlToValidate="txtLName"
                                                                            Display="None" ErrorMessage="Last name is required" ValidationGroup="indpArc" />
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vldcLastNameArc" runat="server"
                                                                            TargetControlID="vldLastNameArc" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label4" runat="server" Text="Day Phone" AssociatedControlID="txtDayTimePhNo" />
                                                                        <telerik:RadMaskedTextBox ID="txtDayTimePhNo" runat="server" Mask="(###) ###-####"
                                                                            Width="130px" AutoPostBack="True" />
                                                                        <a id="lnkDayPhone" class="PhoneBarCheck" runat="server">Dial</a> </li>
                                                                    <li>
                                                                        <asp:Label ID="Label5" runat="server" Text="Evening Phone" AssociatedControlID="txtEvePhNo" />
                                                                        <telerik:RadMaskedTextBox ID="txtEvePhNo" runat="server" Mask="(###) ###-####" Width="130px"
                                                                            AutoPostBack="True" />
                                                                        <a id="lnkEvePhNo" class="PhoneBarCheck" runat="server">Dial</a> </li>
                                                                    <li>
                                                                        <asp:Label ID="lblCellPhone" runat="server" Text="Cell Phone" AssociatedControlID="txtCellPhNo" />
                                                                        <telerik:RadMaskedTextBox ID="txtCellPhNo" runat="server" Mask="(###) ###-####" Width="130px"
                                                                            AutoPostBack="True" />
                                                                        <a id="lnkCellPhone" class="PhoneBarCheck" runat="server">Dial</a> </li>
                                                                    <!--wm-->
                                                                    <li>
                                                                        <asp:Label ID="Label1" runat="server" Text="Email" AssociatedControlID="txtEmailIndv" />
                                                                        <asp:TextBox ID="txtEmailIndv" runat="server" CausesValidation="False" Width="128px" />
                                                                    </li>
                                                                    <!--wm-->
                                                                    <li style="clear: both">
                                                                        <%--<asp:Label ID="Label10" runat="server" Text="Email Opt Out" AssociatedControlID="chkEmailOptOutPrimary" />--%>
                                                                        <asp:CheckBox Style="display: block" ID="chkEmailOptOutPrimary" runat="server" CausesValidation="False" Text="Email Opt Out" TextAlign="Left" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label6" runat="server" Text="Birth Date" AssociatedControlID="diDOB" />
                                                                        <telerik:RadDatePicker ID="diDOB" runat="server" Style="display: inline-block;" ValidationGroup="indp" Width="156px">
                                                                            <Calendar runat="server" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False"
                                                                                ViewSelectorText="x" />
                                                                            <DateInput runat="server" DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="40%" />
                                                                        </telerik:RadDatePicker>
                                                                        <asp:RequiredFieldValidator ID="vldReqdiDOB" runat="server" ControlToValidate="diDOB"
                                                                            Display="None" ErrorMessage="Date of Birth is required" ValidationGroup="indp" />
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender5" runat="server"
                                                                            TargetControlID="vldReqdiDOB" />
                                                                        <!--Arc-->
                                                                        <asp:RequiredFieldValidator ID="vldReqdiDOBArc" runat="server" ControlToValidate="diDOB"
                                                                            Display="None" ErrorMessage="Date of Birth is required" ValidationGroup="indpArc" />
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vldcDOBArc" runat="server"
                                                                            TargetControlID="vldReqdiDOBArc" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="lblGender" runat="server" Text="Gender" AssociatedControlID="ddlGenderP" />
                                                                        <asp:DropDownList ID="ddlGenderP" runat="server" Width="130px" ValidationGroup="indp">
                                                                            <asp:ListItem></asp:ListItem>
                                                                            <asp:ListItem>Male</asp:ListItem>
                                                                            <asp:ListItem>Female</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="vldReqGender" Display="None" runat="server"
                                                                            ValidationGroup="indp" ErrorMessage="Please select gender." ControlToValidate="ddlGenderP"
                                                                            InitialValue=""> </asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server"
                                                                            TargetControlID="vldReqGender" />
                                                                        <!--Arc-->
                                                                        <asp:RequiredFieldValidator ID="vldReqGenderArc" Display="None" runat="server"
                                                                            ValidationGroup="indpArc" ErrorMessage="Please select gender." ControlToValidate="ddlGenderP"
                                                                            InitialValue=""> </asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="vlcCGenderARc" runat="server"
                                                                            TargetControlID="vldReqGenderArc" />
                                                                    </li>

                                                                </ul>
                                                            </td>
                                                            <td>
                                                                <ul>

                                                                    <li>
                                                                        <asp:Label ID="Label2" runat="server" Text="Address 1" AssociatedControlID="txtAddress1Primary" />
                                                                        <asp:TextBox ID="txtAddress1Primary" runat="server" CausesValidation="False" Width="128px" />

                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label3" runat="server" Text="Address 2" AssociatedControlID="txtAddress2Primary" />
                                                                        <asp:TextBox ID="txtAddress2Primary" runat="server" CausesValidation="False" Width="128px" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label8" runat="server" Text="City" AssociatedControlID="txtCityPrimary" />
                                                                        <asp:TextBox ID="txtCityPrimary" runat="server" CausesValidation="False" Width="128px" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label9" runat="server" Text="State" AssociatedControlID="ddlStatePrimary" />
                                                                        <asp:DropDownList ID="ddlStatePrimary" runat="server" Width="130px" DataTextField="FullName"
                                                                            DataValueField="Id" ValidationGroup="indp" />
                                                                        <asp:RequiredFieldValidator ID="vldReqStatePrimary" Display="None" runat="server"
                                                                            ValidationGroup="indp" ErrorMessage="Please select a State." ControlToValidate="ddlStatePrimary"
                                                                            InitialValue="-1"> </asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                                            TargetControlID="vldReqStatePrimary" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label7" runat="server" Text="Zip Code" AssociatedControlID="txtZipCodePrimary" />
                                                                        <asp:TextBox ID="txtZipCodePrimary" runat="server" CausesValidation="False" Width="128px" />
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="lblConsent" runat="server" AssociatedControlID="ddlConsent" Text="Consent" />
                                                                        <asp:DropDownList ID="ddlConsent" runat="server" Enabled="false" Width="130px">
                                                                            <asp:ListItem Value="0" Text="" />
                                                                            <asp:ListItem Value="1" Text="Yes" />
                                                                            <asp:ListItem Value="2" Text="No" />
                                                                            <asp:ListItem Value="3" Text="Not Applicable" />
                                                                        </asp:DropDownList>
                                                                    </li>
                                                                    <li>
                                                                        <asp:Label ID="Label10" runat="server" Text="AP Date" AssociatedControlID="diDOB2" />
                                                                        <telerik:RadDatePicker ID="diDOB2" runat="server" Style="display: inline-block;" Width="156px">
                                                                            <Calendar ID="Calendar2" runat="server" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False"
                                                                                ViewSelectorText="x" />
                                                                            <DateInput ID="DateInput2" runat="server" DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy" LabelWidth="40%" />
                                                                        </telerik:RadDatePicker>

                                                                    </li>
                                                                    <li runat="server" id="liApplicationState">
                                                                        <asp:Label ID="lblAppState" runat="server" Text="Application State" AssociatedControlID="ddlApplicationState" />
                                                                        <asp:DropDownList ID="ddlApplicationState" runat="server" Width="130px" DataTextField="FullName"
                                                                            DataValueField="Id" ValidationGroup="indp" />
                                                                        <asp:RequiredFieldValidator ID="vldReqAppState" Display="None" runat="server"
                                                                            ValidationGroup="indp" ErrorMessage="Please select an application State." ControlToValidate="ddlApplicationState"
                                                                            InitialValue="-1"> </asp:RequiredFieldValidator>
                                                                        <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender7" runat="server"
                                                                            TargetControlID="vldReqAppState" />
                                                                    </li>
                                                                </ul>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <fieldset class="onboard" runat="server" id="fset_onboard">
                                                                    <legend>Onboarding</legend>

                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_dental">
                                                                        <asp:Label runat="server" Text="Dental" ID="lbl_indv_ob_dental"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_dental" />
                                                                    </div>
                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_annuity">
                                                                        <asp:Label runat="server" Text="Annuity" ID="lbl_indv_ob_annuity"></asp:Label>
                                                                        <asp:CheckBox runat="server" ID="chk_indv_ob_annuity" />
                                                                    </div>
                                                                    <div class="onboard-field" id="field_indv_ob_billing" runat="server">
                                                                        <asp:Label runat="server" Text="Billing" ID="lbl_indv_ob_billing"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_billing" />
                                                                    </div>
                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_auto_home_life">
                                                                        <asp:Label runat="server" Text="Auto/Home and Life" ID="lbl_indv_ob_auto_home_life"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_auto_home_life" />
                                                                    </div>
                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_inspection">
                                                                        <asp:Label runat="server" Text="Inspection" ID="lbl_indv_ob_inspection"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_inspection" />
                                                                    </div>
                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_app_esign">
                                                                        <asp:Label runat="server" Text="Application/eSign" ID="lbl_indv_ob_app_esign"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_app_esign" />
                                                                    </div>
                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_cs_prep">
                                                                        <asp:Label runat="server" Text="CS Prep " ID="lbl_indv_ob_cs_prep"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_cs_prep" />
                                                                    </div>
                                                                    <div class="onboard-field" runat="server" id="field_indv_ob_auto_home">
                                                                        <asp:Label runat="server" Text="Auto and Home" ID="lbl_indv_ob_auto_home"></asp:Label><asp:CheckBox runat="server" ID="chk_indv_ob_auto_home" />
                                                                    </div>
                                                                </fieldset>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </fieldset>
                                            </asp:Panel>
                                        </td>



                                        <%--
<!-- Spouce Individual -->    <td style="vertical-align: top; width: 282px;" runat="server" id="spouseSection">
                                              SZ [May 19, 2014] Code has been commented out as no longer required.
                                              @YA: details required.
                                                
                                              <asp:Panel ID="pnlSpouce" runat="server" Width="290px">
                                                <fieldset class="condado" style="height: 550px;">
                                                    <legend>Spouse Information</legend>
                                                    <ul>
                                                        <li>
                                                            <asp:Label ID="lblSpouce" runat="server" Text="Spouse" AssociatedControlID="ddlSpouse" />
                                                            <asp:DropDownList ID="ddlSpouse" runat="server" Width="130px" AutoPostBack="True">
                                                                <asp:ListItem Value="N" Text="No" />
                                                                <asp:ListItem Value="Y" Text="Yes" />
                                                            </asp:DropDownList>
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblSpouseFirstName" runat="server" Text="First Name" AssociatedControlID="txtSpouseFName"
                                                                Visible="false" />
                                                            <asp:TextBox ID="txtSpouseFName" runat="server" CausesValidation="True" ValidationGroup="indp"
                                                                Width="128px" Visible="false" />
                                                            <asp:RequiredFieldValidator ID="valSpouceFirst" runat="server" ControlToValidate="txtSpouseFName"
                                                                Display="None" ErrorMessage="Spouce first name is required" ValidationGroup="indp" />
                                                            <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server"
                                                                TargetControlID="valSpouceFirst" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblSpouseLastName" runat="server" Text="Last Name" AssociatedControlID="tbSpouseLastName"
                                                                Visible="false" />
                                                            <asp:TextBox ID="tbSpouseLastName" runat="server" CausesValidation="True" ValidationGroup="indp"
                                                                Width="128px" Visible="false" />
                                                            <asp:RequiredFieldValidator ID="valSpouceLast" runat="server" ControlToValidate="tbSpouseLastName"
                                                                Display="None" ErrorMessage="Spouce last name is required" ValidationGroup="indp" />
                                                            <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" runat="server"
                                                                TargetControlID="valSpouceLast" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblEmailSpouse" runat="server" Text="Email" AssociatedControlID="txtEmailSpouse"
                                                                Visible="false" />
                                                            <asp:TextBox ID="txtEmailSpouse" runat="server" CausesValidation="False" Width="128px"
                                                                Visible="false" />
                                                        </li>
                                                        <li style="clear: both">
                                                            <%--<asp:Label ID="lblEmailOptOutSpouse" runat="server" Text="Email Opt Out" AssociatedControlID="chkEmailOptOutSpuse"
                                                                Visible="false" />
                                                            <asp:CheckBox Style="display: block" ID="chkEmailOptOutSpuse" runat="server" CausesValidation="False" Text="Email Opt Out" TextAlign="Left"
                                                                Visible="false" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblSpouseDateOfBirth" runat="server" Text="Birth Date" AssociatedControlID="rdisecondDob"
                                                                Visible="false" />
                                                            <telerik:RadDatePicker ID="rdisecondDob" runat="server" Visible="False" SkipMinMaxDateValidationOnServer="true" Style="display: inline-block;" Width="156px">
                                                                <Calendar ID="Calendar1" runat="server" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False"
                                                                    ViewSelectorText="x" />
                                                                <DateInput ID="DateInput1" runat="server" DateFormat="M/d/yyyy" DisplayDateFormat="M/d/yyyy"
                                                                    LabelWidth="40%" />
                                                            </telerik:RadDatePicker>
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblGenderS" runat="server" Text="Gender" AssociatedControlID="ddlGenderS" />
                                                            <asp:DropDownList ID="ddlGenderS" runat="server" Width="130px">
                                                                <asp:ListItem></asp:ListItem>
                                                                <asp:ListItem>Male</asp:ListItem>
                                                                <asp:ListItem>Female</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblAddress1Secondary" runat="server" Text="Address 1" AssociatedControlID="txtAddress1Secondary" />
                                                            <asp:TextBox ID="txtAddress1Secondary" runat="server" CausesValidation="False" Width="128px" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblAddress2Secondary" runat="server" Text="Address 2" AssociatedControlID="txtAddress2Secondary" />
                                                            <asp:TextBox ID="txtAddress2Secondary" runat="server" CausesValidation="False" Width="128px" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblCitySecondary" runat="server" Text="City" AssociatedControlID="txtCitySecondary" />
                                                            <asp:TextBox ID="txtCitySecondary" runat="server" CausesValidation="False" Width="128px" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblStateSecondary" runat="server" Text="State" AssociatedControlID="ddlStateSecondary" />
                                                            <asp:DropDownList ID="ddlStateSecondary" runat="server" Width="130px" DataTextField="FullName"
                                                                DataValueField="Id" />
                                                        </li>
                                                        <li>
                                                            <asp:Label ID="lblZipCodeSecondary" runat="server" Text="Zip Code" AssociatedControlID="txtZipCodeSecondary" />
                                                            <asp:TextBox ID="txtZipCodeSecondary" runat="server" CausesValidation="False" Width="128px" />
                                                        </li>
                                                    </ul>
                                                </fieldset>
                                            </asp:Panel>  </td>--%>

                                        <td style="vertical-align: top;">
                                            <!-- Buttons -->
                                            <br />
                                            <table>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                        <tr>
                            <td valign="top">
                                <!---Child Tab  --->
                                <br />
                                <div id="divtabs" runat="server" style="border-style: none; min-height: 500px;">
                                    <telerik:RadTabStrip ID="tlkLeadsTabs" runat="server" MultiPageID="tlkLeadsPages"
                                        Font-Size="XX-Small" SelectedIndex="0" Skin="WebBlue" CausesValidation="False"
                                        OnTabClick="tlkLeadsTabs_TabClick">
                                        <Tabs>
                                            <telerik:RadTab runat="server" Text="Individuals" PageViewID="pgIndividuals"
                                                Selected="True" />
                                            <telerik:RadTab runat="server" Text="Arc Cases" PageViewID="pgArcCases" />
                                            <telerik:RadTab runat="server" Text="Arc History" PageViewID="pgArcHistory" />
                                            <telerik:RadTab runat="server" Text="Homes" PageViewID="pgHomeInfo" />
                                            <telerik:RadTab runat="server" Text="Drivers" PageViewID="pgDrivers" />
                                            <telerik:RadTab runat="server" Text="Vehicles" PageViewID="pgVehicleInfo" />
                                            <telerik:RadTab runat="server" Text="Policies" PageViewID="pgPolicies" />
                                            <telerik:RadTab runat="server" Text="Quotes" PageViewID="pgQuotes" />
                                            <telerik:RadTab runat="server" Text="Carrier Issue Tracking" PageViewID="pgCarrierIssues" />
                                            <telerik:RadTab runat="server" Text="Medicare Supplement" PageViewID="pgMedicareSupplement" />
                                            <telerik:RadTab runat="server" Text="Application Tracking" PageViewID="pgApplicationTracking" />
                                            <telerik:RadTab runat="server" Text="MA &amp; PDP" PageViewID="pgMAPDP" />
                                            <telerik:RadTab runat="server" Text="Dental &amp; Vision" PageViewID="pgDentalVision" />
                                            <telerik:RadTab runat="server" Text="Leads &amp; Marketing" PageViewID="pgLeadsMarketing" />
                                            <%--<telerik:RadTab runat="server" Text="Screener" PageViewID="pgScreener" 
                                            Visible="false" />--%>
                                            <telerik:RadTab runat="server" Text="Attachments" PageViewID="pgAttachments" />
                                            <telerik:RadTab runat="server" Text="Notes" PageViewID="pgNotes" />
                                            <telerik:RadTab runat="server" Text="Life Information" PageViewID="pgLifeInfo" />
                                        </Tabs>
                                    </telerik:RadTabStrip>
                                    <telerik:RadMultiPage ID="tlkLeadsPages" runat="server" SelectedIndex="0" Width="100%"
                                        Height="100%">
                                        <telerik:RadPageView ID="pgIndividuals" runat="server" Selected="true">
                                            <fieldset id="fldSetIndv" class="condado" style="border-style: none">
                                                <IndvInfo1:IndvInfo ID="IndvInfo1" runat="server" OnDialClicked="ClickedToDial" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgArcCases" runat="server">
                                            <fieldset id="Fieldset11" class="condado" style="border-style: none">
                                                <ArcCases:ArcCases runat="server" ID="ArcCases" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgArcHistory" runat="server">
                                            <fieldset id="Fieldset12" class="condado" style="border-style: none">
                                                <ArcHistory:ArcHistory runat="server" ID="ArcHistory" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgApplicationTracking" runat="server">
                                            <fieldset id="fldsetAppInfo" class="condado">
                                                <AppInfo1:AppInfo ID="AppInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgMAPDP" runat="server">
                                            <fieldset id="fldsetMAPDPInfo" class="condado">
                                                <MapdpInfo1:MapdpInfo ID="mapdpInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgDentalVision" runat="server">
                                            <fieldset id="fldsetDental" class="condado">
                                                <DentInfo1:DentInfo ID="dentInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <%--<telerik:RadPageView ID="pgScreener" runat="server">
                                        <fieldset id="fldsetScrInfo" class="condado">
                                            <uc3:AutoHomeQuote ID="AutoHomeQuote1" runat="server" />
                                        </fieldset>
                                    </telerik:RadPageView>--%>
                                        <telerik:RadPageView ID="pgMedicareSupplement" runat="server">
                                            <fieldset id="Fieldset7" class="condado">
                                                <PolicyInfo1:PolicyInfo ID="PolicyInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgHomeInfo" runat="server">
                                            <fieldset id="Fieldset1" class="condado" style="border-style: none">
                                                <HomeInfo1:HomeInfo ID="HomeInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgVehicleInfo" runat="server">
                                            <fieldset id="Fieldset2" class="condado" style="border-style: none">
                                                <VehicleInfo1:VehicleInfo ID="VehicleInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgCarrierIssues" runat="server">
                                            <fieldset id="Fieldset3" class="condado" style="border-style: none">
                                                <CarrierIssueInfo1:CarrierIssueInfo ID="CarrierIssueInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgDrivers" runat="server">
                                            <fieldset id="Fieldset4" class="condado" style="border-style: none">
                                                <DriverInfo1:DriverInfo ID="DriverInfo1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgLeadsMarketing" runat="server">
                                            <fieldset id="fldsetMarInfo" class="condado">
                                                <LeadsMarketing1:LeadsMarketing ID="LeadsMarketing1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgAttachments" runat="server">
                                            <fieldset id="Fieldset5" class="condado" style="border-style: none">
                                                <uc2:AccountAttachments ID="AccountAttachments1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>

                                        <telerik:RadPageView ID="pgPolicies" runat="server">
                                            <fieldset id="Fieldset6" class="condado" style="border-style: none">
                                                <AutoHomePolicy1:AutoHomePolicy ID="AutoHomePolicy1" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgQuotes" runat="server">
                                            <fieldset id="Fieldset8" class="condado">
                                                <uc3:AutoHomeQuote ID="AutoHomeQuote2" runat="server" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgNotes" runat="server">
                                            <fieldset id="Fieldset9" class="condado">
                                                <asp:TextBox ID="txtNotes" TextMode="MultiLine" runat="server" Rows="5" Width="1000px"
                                                    Height="350px" />
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="pgLifeInfo" runat="server">
                                            <fieldset id="Fieldset10" class="condado">
                                                <asp:TextBox ID="txtLifeInfo" TextMode="MultiLine" runat="server" Rows="5" Width="1000px"
                                                    Height="350px" />
                                            </fieldset>
                                        </telerik:RadPageView>

                                    </telerik:RadMultiPage>
                                </div>
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
            <asp:HiddenField ID="hdCampID" runat="server" />
            <asp:HiddenField ID="hdnCurrentOutPulseIDDayPhone" Value="" runat="server" />
            <asp:HiddenField ID="hdnCurrentOutPulseIDEveningPhone" Value="" runat="server" />
            <asp:HiddenField ID="hdnCurrentOutPulseIDCellPhone" Value="" runat="server" />
            <asp:UpdatePanel ID="Updatepanel1" runat="server">
                <ContentTemplate>
                    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ReloadOnShow="True">
                        <Windows>
                            <telerik:RadWindow ID="dlgConfirmBox" Height="200px" Width="300px" Modal="True" runat="server"
                                Style="display: none;" VisibleStatusbar="False" Title="Confirmation" Behaviors="Move,Close"
                                InitialBehaviors="None">
                                <ContentTemplate>
                                    <table style="width: 100%; height: 100%">
                                        <tr>
                                            <td>
                                                <div id="divConfirmMessage" align="center" style="text-align: center">
                                                    <br />
                                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 50px;">
                                                <div class="buttons" align="center" style="height: 40px">
                                                    <asp:Button ID="btnYes" runat="server" CausesValidation="true" OnClick="btnYes_Click"
                                                        ValidationGroup="indp" Text="Yes" OnClientClick="closeDlg();return true;" Width="80px"
                                                        Height="30px" />
                                                    &nbsp;
                                                    <asp:Button ID="btnNo" runat="server" CausesValidation="false" Text="No" OnClientClick="closeDlg();return false;"
                                                        Width="80px" Height="30px" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </telerik:RadWindow>
                        </Windows>
                    </telerik:RadWindowManager>
                    <telerik:RadWindowManager ID="RadWindowManager2" runat="server" ReloadOnShow="True">
                        <Windows>
                            <telerik:RadWindow ID="CamDesc" runat="server" Behaviors="Move" InitialBehaviors="None"
                                Skin="WebBlue" Left="" NavigateUrl="CampaignDescription.aspx" Style="display: none;"
                                Title="Campaign Description" Top="" Height="245px" Width="400px" Modal="True"
                                VisibleStatusbar="False">
                            </telerik:RadWindow>
                        </Windows>
                        <Windows>
                            <telerik:RadWindow ID="ApplyAction" runat="server" Behaviors="Move" InitialBehaviors="None"
                                Left="" NavigateUrl="ApplyAction.aspx" Style="display: none;" Title="Apply Action"
                                Top="" Modal="True" VisibleStatusbar="False">
                            </telerik:RadWindow>
                        </Windows>
                    </telerik:RadWindowManager>
                </ContentTemplate>
            </asp:UpdatePanel>
            <telerik:RadWindow ID="dlgReassignUser" runat="server" Width="450" Height="175" Behaviors="Move"
                Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false" IconUrl="../Images/alert1.ico"
                VisibleOnPageLoad="false" Title="Reassign User">
                <ContentTemplate>
                    <p>
                        <asp:Label ID="lblMessageReassignUsers" runat="server" Text=""></asp:Label>
                        <asp:HiddenField runat="server" ID="hdCurrentAccountID" />
                        <asp:HiddenField runat="server" ID="hdCurrentLeadID" />
                    </p>
                    <div class="buttons" align="center" style="height: 40px">
                        <asp:Button ID="btnTakerOwnership" runat="server" Text="Take Ownership" CausesValidation="false"
                            OnClick="btnTakerOwnership_Click" Height="30px" />
                        &nbsp;
                        <asp:Button ID="btnContinue" runat="server" CausesValidation="false" Text="Continue"
                            OnClick="btnContinue_Click" Width="80px" Height="30px" />
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="radWindowManualEmail" runat="server" Behaviors="Move" Modal="true" Width="900px" Height="900px"
                Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                VisibleOnPageLoad="false" Title="Email Sender">
                <ContentTemplate>
                    <asp:UpdatePanel ID="updatePanelManualEmail" runat="server">
                        <ContentTemplate>
                            <uc7:EmailSender ID="ctlEmailSender" runat="server" />
                            <div class="buttons" align="center">
                                <asp:Button ID="btnQueueEmail" runat="server" CausesValidation="false" Text="Queue Email" OnClick="btnQueueEmail_Click" />
                                <asp:Button ID="btnCloseManualEmail" runat="server" CausesValidation="false" Text="Close" OnClick="btnCloseManualEmail_Click" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnQueueEmail" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnCloseManualEmail" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
            <asp:HiddenField ID="hdnFieldTimerIntervals" runat="server" />
            <asp:HiddenField ID="hdnFieldTimerAlertID" runat="server" />
            <asp:HiddenField ID="hdnFieldGALBaseTime" runat="server" />
            <telerik:RadWindow ID="dlgCampaignAlert" runat="server" Width="450" Height="350"
                Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                IconUrl="../Images/alert1.ico" VisibleOnPageLoad="false" Title="Campaign Alerts">
                <ContentTemplate>
                    <asp:HiddenField ID="hdnFieldCampaignID" runat="server" />
                    <asp:HiddenField ID="hdnFieldPrimaryStatus" runat="server" />
                    <asp:UpdatePanel runat="server" ID="updatePanelInner">
                        <ContentTemplate>
                            <div class="buttons">
                                <asp:Label ID="lblCampaignName" runat="server" Text="" />
                                <uc1:PagingBar ID="ctlPagerCampaignAlerts" runat="server" NewButtonTitle="" OnIndexChanged="EvtCampaignAlert_PageNumberChanged" />
                            </div>
                            <br />
                            <asp:FormView ID="frmViewCampaignAlert" runat="server" DataKeyNames="Id" GridLines="None"
                                Height="144px">
                                <ItemTemplate>
                                    <fieldset class="condadocompare">
                                        <ul>
                                            <li>
                                                <asp:Label ID="lblName" runat="server" Text="Name:" Font-Bold="True" />
                                                &nbsp;<asp:Label ID="lblInnerAlertName" runat="server" Text='<%# Bind("Name") %>' />
                                            </li>
                                            <li>
                                                <asp:Label ID="Label11" runat="server" Text="Message:" Font-Bold="True" />
                                                &nbsp;<asp:Label ID="lblInnerMessage" runat="server" Text='<%# Bind("Message") %>' />
                                            </li>
                                            <li runat="server" visible="false">
                                                <asp:Label ID="lblAlertType" runat="server" Text='<%# Bind("AlertType") %>' />
                                            </li>
                                            <li>
                                                <br />
                                                <asp:Button ID="btnShowMoreDetail" runat="server" CausesValidation="false" Text="Read More"
                                                    OnClick="btnReadMore_Click" />
                                                <br />
                                            </li>
                                            <li>
                                                <asp:Label ID="lblDetail" runat="server" Text="Detail:" Font-Bold="True" Visible="false" />
                                                &nbsp;<asp:Label ID="lblInnerDetailMessage" runat="server" Text='<%# Bind("DetailMessage") %>'
                                                    Visible="false" />
                                            </li>
                                        </ul>
                                    </fieldset>
                                </ItemTemplate>
                            </asp:FormView>
                            <div class="buttons" align="center" style="height: 40px">
                                <asp:Button ID="btnCloseDialogCampaignAlert" runat="server" CausesValidation="false"
                                    Text="Close" OnClick="btnCloseCampaignAlert_Click" Width="80px" Height="30px" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
            <telerik:RadWindow ID="dlgCampaignDescription" runat="server" Width="500" Height="425"
                Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
                IconUrl="../Images/alert1.ico" VisibleOnPageLoad="false" Title="Campaign Description">
                <ContentTemplate>
                    <asp:UpdatePanel runat="server" ID="updatePanelCampaignDescription">
                        <ContentTemplate>
                            <div>
                                <table>
                                    <tr>
                                        <td>
                                            <div id="div1" align="center" style="text-align: center">
                                                <br />
                                                <h2>
                                                    <asp:Label ID="lblCampaignDescriptionLabel" runat="server" AssociatedControlID="lblCampaignDescription"
                                                        Text="Campaign Description"></asp:Label></h2>
                                                <br />
                                                <asp:Label ID="lblCampaignDescription" Text="No description found." runat="server"></asp:Label>
                                                <br />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 50px;"></td>
                                    </tr>
                                </table>
                            </div>
                            <div class="buttons" align="center" style="height: 40px">
                                <asp:Button ID="btnCloseCampaignDescription" CausesValidation="false" runat="server"
                                    OnClick="btnCloseCampaignDescription_Click" Text="Close" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
            <div id="divTimerAlert" runat="server">
            </div>
            <div id="divErroMsg" runat="server">
            </div>
            <telerik:RadWindow ID="rdWAccountExist" runat="server" Width="500" Height="125" Behaviors="Move"
                Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false" Style="display: none;">
                <ContentTemplate>
                    <div id="div2">
                        <br />
                        &nbsp;&nbsp; The account you are looking for does not exist.
                        <br />
                        <br />
                        <div style="text-align: right;">
                            <asp:Button ID="btnAdd" CssClass="button" runat="server" Text="Add New" OnClick="Evt_Add_Clicked" />&nbsp;&nbsp;
                            <asp:Button ID="brnLead" CssClass="button" runat="server" Text="View Leads" OnClick="Evt_ViewLead_Clicked" />&nbsp;&nbsp;
                            <asp:Button ID="btnPrioritizationView" CssClass="button" runat="server" Text="Prioritization View"
                                OnClick="Evt_PrioritizationView_Clicked" />&nbsp;
                        </div>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
            <asp:Timer runat="server" ID="timerAlert" OnTick="timerAlert_Tick" Enabled="false">
            </asp:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>


    <script type="text/javascript">
        Sys.Application.add_load(bindEvents);
        var myDlg = null;

        function showDlg(v) {

            switch (v) {

                case "EventCalendar":
                    myDlg = $find('<%=dlgEventCalendar.ClientID %>');
                    break;

                case "Individual":
                    alert('dlgIndividual not found');
                    //myDlg = $find('% = dlgIndividual.ClientID %>');
                    break;
            }

            if (myDlg == null) {
                return false;
            }

            myDlg.show();
            myDlg.center();

            return false;
        }

        function closeDlg1() {

            $('#dirtyFlag').val('0');

            if (myDlg != null) {
                myDlg.close();
                myDlg = null;
            }

            return false;
        }

        function OpenConfirmationBox() {


            if (document.getElementById("<%= accountIden.ClientID %>").value == "0") {

                $("#<%= lblMessage.ClientID %>").text("In order to complete this action, you must first create the account.  Would you like to create an account?");

                var windows = $find('<%=RadWindowManager1.ClientID %>').get_windows();

                myDlg = windows[0];
                myDlg.show();
                myDlg.center();

                return false;

            }
            else {
                return true;

            }
        }

        function closeDlg() {
            if (myDlg != null) {
                myDlg.close();
                myDlg = null;
            }
        }

        function closeDlg2() {
            if (myDlg != null) {
                myDlg.close();
                myDlg = null;
                __doPostBack("btnYes", "valYes");
            }
        }


        function OpenPositionedWindow() {
            var yz = document.getElementById("<%= ddCampaigns.ClientID%>");
            var strCamp = yz.options[yz.selectedIndex].value;

            var windows = $find('<%=RadWindowManager2.ClientID %>').get_windows();

            var myWin = windows[0];
            var oWnd = window.radopen('CampaignDescription.aspx?CampaignId=' + strCamp, "CamDesc");
            oWnd.Center();
        }

        function myfun() {
            confirm("Are you sure you want to continue?");
        }

        function returnValFun() {
            if (document.getElementById("<%= accountIden.ClientID %>").value == "0") {
                return false;
            } else {

                return true;
            }
        }

        function OnClientClose(sender, eventArgs) {
            //window.location.reload();
            window.location = "Leads.aspx";
        }
    </script>
</asp:Content>
