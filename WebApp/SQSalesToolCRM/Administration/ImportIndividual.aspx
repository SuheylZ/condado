<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ImportIndividual.aspx.cs" Inherits="Admin_ImportIndividual" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>

<%@ Register src="../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register src="../UserControls/UserPermissions.ascx" tagname="UserPermissions" tagprefix="uc2" %>
<%@ Register src="../UserControls/UserDetail.ascx" tagname="UserDetail" tagprefix="uc3" %>
<%@ Register src="../UserControls/StatusLabel.ascx" tagname="StatusLabel" tagprefix="uc4" %>
<%@ Register src="~/UserControls/SelectionLists.ascx" TagName="SelectionControl" TagPrefix="sc" %>

<asp:Content ID="cntHeader1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript">
    function showMenu(e, contextMenu) {
        $telerik.cancelRawEvent(e);

        if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
            contextMenu.show(e);
        }
    }
</script>
<script type="text/javascript" id="telerikClientEvents2">
//<![CDATA[
   
    var validFilesTypes = ["csv"];

    function CheckExtension(file) {
        /*global document: false */
        var filePath = file.value;
        var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
        var isValidFile = false;

        for (var i = 0; i < validFilesTypes.length; i++) {
            if (ext == validFilesTypes[i]) {
                isValidFile = true;
                break;
            }
        }

        if (!isValidFile) {
            file.value = null;
            alert("Invalid File. Valid extensions are:\n\n" + validFilesTypes.join(", "));
        }

        return isValidFile;
    }
//]]>
</script>

    <style type="text/css">
        .mGrid th,
        .mGrid td {
            padding: 2px 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">

    <asp:UpdatePanel runat="server" ID="updPanel">
        <ContentTemplate>
                
            <uc4:StatusLabel ID="ctlStatus" runat="server" />
            <asp:HiddenField runat="server" ID="hdID" />
            <asp:Panel ID="pnlUserList" runat="server">
                <fieldset class="condado">
                    <legend>Import Individuals for Account Access</legend>
                    <div id="divToolbar" class="Toolbar">
                            Select file to import data
                            <asp:FileUpload ID="FileUpload1" runat="server" onchange="return CheckExtension(this);" />
                            <br />
                            <asp:Button runat="server" ID="btnParse" Text="Parse File" OnClick="btnParse_Click" />
                            <asp:Button runat="server" ID="btnImport" Text="Import Data" OnClick="btnImport_Click" />
                            <br />
                            <asp:Label runat="server" ID="lblError"></asp:Label>
                            <br />
                    </div>
                    <div id="divGrid" style="width: 1450px; overflow: scroll;">
            
                        <asp:GridView ID="grdImport" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt"
                            CssClass="mGrid" Width="100%" GridLines="None" ShowHeaderWhenEmpty="true" >
                            <AlternatingRowStyle CssClass="alt" />
                            <Columns>
                                <asp:BoundField DataField="campaign_id" HeaderText="campaign_id" />
                                <asp:BoundField DataField="status_id" HeaderText="status_id" />

                                <asp:BoundField DataField="primary_firstname" HeaderText="primary_firstname" />
                                <asp:BoundField DataField="primary_lastname" HeaderText="primary_lastname" />
                                <asp:BoundField DataField="primary_phone" HeaderText="primary_phone" />
                                <asp:BoundField DataField="primary_last_four_SSN" HeaderText="primary_last_four_SSN" />
                                <asp:BoundField DataField="primary_birthday" HeaderText="primary_birthday" />

                                <asp:BoundField DataField="primary_drug_id_1" HeaderText="primary_drug_id_1" />
                                <asp:BoundField DataField="primary_drug_dosage_id_1" HeaderText="primary_drug_dosage_id_1" />
                                <asp:BoundField DataField="primary_drug_package_id_1" HeaderText="primary_drug_package_id_1" />
                                <asp:BoundField DataField="primary_amount_30_days_1" HeaderText="primary_amount_30_days_1" />

                                <asp:BoundField DataField="primary_drug_id_2" HeaderText="primary_drug_id_2" />
                                <asp:BoundField DataField="primary_drug_dosage_id_2" HeaderText="primary_drug_dosage_id_2" />
                                <asp:BoundField DataField="primary_drug_package_id_2" HeaderText="primary_drug_package_id_2" />
                                <asp:BoundField DataField="primary_amount_30_days_2" HeaderText="primary_amount_30_days_2" />

                                <asp:BoundField DataField="primary_drug_id_3" HeaderText="primary_drug_id_3" />
                                <asp:BoundField DataField="primary_drug_dosage_id_3" HeaderText="primary_drug_dosage_id_3" />
                                <asp:BoundField DataField="primary_drug_package_id_3" HeaderText="primary_drug_package_id_3" />
                                <asp:BoundField DataField="primary_amount_30_days_3" HeaderText="primary_amount_30_days_3" />

                                <asp:BoundField DataField="primary_drug_id_4" HeaderText="primary_drug_id_4" />
                                <asp:BoundField DataField="primary_drug_dosage_id_4" HeaderText="primary_drug_dosage_id_4" />
                                <asp:BoundField DataField="primary_drug_package_id_4" HeaderText="primary_drug_package_id_4" />
                                <asp:BoundField DataField="primary_amount_30_days_4" HeaderText="primary_amount_30_days_4" />

                                <asp:BoundField DataField="primary_drug_id_5" HeaderText="primary_drug_id_5" />
                                <asp:BoundField DataField="primary_drug_dosage_id_5" HeaderText="primary_drug_dosage_id_5" />
                                <asp:BoundField DataField="primary_drug_package_id_5" HeaderText="primary_drug_package_id_5" />
                                <asp:BoundField DataField="primary_amount_30_days_5" HeaderText="primary_amount_30_days_5" />

                                <asp:BoundField DataField="primary_drug_id_6" HeaderText="primary_drug_id_6" />
                                <asp:BoundField DataField="primary_drug_dosage_id_6" HeaderText="primary_drug_dosage_id_6" />
                                <asp:BoundField DataField="primary_drug_package_id_6" HeaderText="primary_drug_package_id_6" />
                                <asp:BoundField DataField="primary_amount_30_days_6" HeaderText="primary_amount_30_days_6" />

                                <asp:BoundField DataField="primary_drug_id_7" HeaderText="primary_drug_id_7" />
                                <asp:BoundField DataField="primary_drug_dosage_id_7" HeaderText="primary_drug_dosage_id_7" />
                                <asp:BoundField DataField="primary_drug_package_id_7" HeaderText="primary_drug_package_id_7" />
                                <asp:BoundField DataField="primary_amount_30_days_7" HeaderText="primary_amount_30_days_7" />

                                <asp:BoundField DataField="primary_drug_id_8" HeaderText="primary_drug_id_8" />
                                <asp:BoundField DataField="primary_drug_dosage_id_8" HeaderText="primary_drug_dosage_id_8" />
                                <asp:BoundField DataField="primary_drug_package_id_8" HeaderText="primary_drug_package_id_8" />
                                <asp:BoundField DataField="primary_amount_30_days_8" HeaderText="primary_amount_30_days_8" />

                                <asp:BoundField DataField="primary_drug_id_9" HeaderText="primary_drug_id_9" />
                                <asp:BoundField DataField="primary_drug_dosage_id_9" HeaderText="primary_drug_dosage_id_9" />
                                <asp:BoundField DataField="primary_drug_package_id_9" HeaderText="primary_drug_package_id_9" />
                                <asp:BoundField DataField="primary_amount_30_days_9" HeaderText="primary_amount_30_days_9" />

                                <asp:BoundField DataField="primary_drug_id_10" HeaderText="primary_drug_id_10" />
                                <asp:BoundField DataField="primary_drug_dosage_id_10" HeaderText="primary_drug_dosage_id_10" />
                                <asp:BoundField DataField="primary_drug_package_id_10" HeaderText="primary_drug_package_id_10" />
                                <asp:BoundField DataField="primary_amount_30_days_10" HeaderText="primary_amount_30_days_10" />

                                <asp:BoundField DataField="primary_drug_id_11" HeaderText="primary_drug_id_11" />
                                <asp:BoundField DataField="primary_drug_dosage_id_11" HeaderText="primary_drug_dosage_id_11" />
                                <asp:BoundField DataField="primary_drug_package_id_11" HeaderText="primary_drug_package_id_11" />
                                <asp:BoundField DataField="primary_amount_30_days_11" HeaderText="primary_amount_30_days_11" />

                                <asp:BoundField DataField="primary_drug_id_12" HeaderText="primary_drug_id_12" />
                                <asp:BoundField DataField="primary_drug_dosage_id_12" HeaderText="primary_drug_dosage_id_12" />
                                <asp:BoundField DataField="primary_drug_package_id_12" HeaderText="primary_drug_package_id_12" />
                                <asp:BoundField DataField="primary_amount_30_days_12" HeaderText="primary_amount_30_days_12" />

                                <asp:BoundField DataField="primary_drug_id_13" HeaderText="primary_drug_id_13" />
                                <asp:BoundField DataField="primary_drug_dosage_id_13" HeaderText="primary_drug_dosage_id_13" />
                                <asp:BoundField DataField="primary_drug_package_id_13" HeaderText="primary_drug_package_id_13" />
                                <asp:BoundField DataField="primary_amount_30_days_13" HeaderText="primary_amount_30_days_13" />

                                <asp:BoundField DataField="primary_drug_id_14" HeaderText="primary_drug_id_14" />
                                <asp:BoundField DataField="primary_drug_dosage_id_14" HeaderText="primary_drug_dosage_id_14" />
                                <asp:BoundField DataField="primary_drug_package_id_14" HeaderText="primary_drug_package_id_14" />
                                <asp:BoundField DataField="primary_amount_30_days_14" HeaderText="primary_amount_30_days_14" />

                                <asp:BoundField DataField="primary_drug_id_15" HeaderText="primary_drug_id_15" />
                                <asp:BoundField DataField="primary_drug_dosage_id_15" HeaderText="primary_drug_dosage_id_15" />
                                <asp:BoundField DataField="primary_drug_package_id_15" HeaderText="primary_drug_package_id_15" />
                                <asp:BoundField DataField="primary_amount_30_days_15" HeaderText="primary_amount_30_days_15" />

                                <asp:BoundField DataField="primary_drug_id_16" HeaderText="primary_drug_id_16" />
                                <asp:BoundField DataField="primary_drug_dosage_id_16" HeaderText="primary_drug_dosage_id_16" />
                                <asp:BoundField DataField="primary_drug_package_id_16" HeaderText="primary_drug_package_id_16" />
                                <asp:BoundField DataField="primary_amount_30_days_16" HeaderText="primary_amount_30_days_16" />

                                <asp:BoundField DataField="primary_drug_id_17" HeaderText="primary_drug_id_17" />
                                <asp:BoundField DataField="primary_drug_dosage_id_17" HeaderText="primary_drug_dosage_id_17" />
                                <asp:BoundField DataField="primary_drug_package_id_17" HeaderText="primary_drug_package_id_17" />
                                <asp:BoundField DataField="primary_amount_30_days_17" HeaderText="primary_amount_30_days_17" />

                                <asp:BoundField DataField="primary_drug_id_18" HeaderText="primary_drug_id_18" />
                                <asp:BoundField DataField="primary_drug_dosage_id_18" HeaderText="primary_drug_dosage_id_18" />
                                <asp:BoundField DataField="primary_drug_package_id_18" HeaderText="primary_drug_package_id_18" />
                                <asp:BoundField DataField="primary_amount_30_days_18" HeaderText="primary_amount_30_days_18" />

                                <asp:BoundField DataField="primary_drug_id_19" HeaderText="primary_drug_id_19" />
                                <asp:BoundField DataField="primary_drug_dosage_id_19" HeaderText="primary_drug_dosage_id_19" />
                                <asp:BoundField DataField="primary_drug_package_id_19" HeaderText="primary_drug_package_id_19" />
                                <asp:BoundField DataField="primary_amount_30_days_19" HeaderText="primary_amount_30_days_19" />

                                <asp:BoundField DataField="primary_drug_id_20" HeaderText="primary_drug_id_20" />
                                <asp:BoundField DataField="primary_drug_dosage_id_20" HeaderText="primary_drug_dosage_id_20" />
                                <asp:BoundField DataField="primary_drug_package_id_20" HeaderText="primary_drug_package_id_20" />
                                <asp:BoundField DataField="primary_amount_30_days_20" HeaderText="primary_amount_30_days_20" />



                                <asp:BoundField DataField="dependent_firstname" HeaderText="dependent_firstname" />
                                <asp:BoundField DataField="dependent_lastname" HeaderText="dependent_lastname" />
                                <asp:BoundField DataField="dependent_phone" HeaderText="dependent_phone" />
                                <asp:BoundField DataField="dependent_last_four_SSN" HeaderText="dependent_last_four_SSN" />
                                <asp:BoundField DataField="dependent_birthday" HeaderText="dependent_birthday" />
                                
                                <asp:BoundField DataField="dependent_drug_id_1" HeaderText="dependent_drug_id_1" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_1" HeaderText="dependent_drug_dosage_id_1" />
                                <asp:BoundField DataField="dependent_drug_package_id_1" HeaderText="dependent_drug_package_id_1" />
                                <asp:BoundField DataField="dependent_amount_30_days_1" HeaderText="dependent_amount_30_days_1" />

                                <asp:BoundField DataField="dependent_drug_id_2" HeaderText="dependent_drug_id_2" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_2" HeaderText="dependent_drug_dosage_id_2" />
                                <asp:BoundField DataField="dependent_drug_package_id_2" HeaderText="dependent_drug_package_id_2" />
                                <asp:BoundField DataField="dependent_amount_30_days_2" HeaderText="dependent_amount_30_days_2" />

                                <asp:BoundField DataField="dependent_drug_id_3" HeaderText="dependent_drug_id_3" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_3" HeaderText="dependent_drug_dosage_id_3" />
                                <asp:BoundField DataField="dependent_drug_package_id_3" HeaderText="dependent_drug_package_id_3" />
                                <asp:BoundField DataField="dependent_amount_30_days_3" HeaderText="dependent_amount_30_days_3" />

                                <asp:BoundField DataField="dependent_drug_id_4" HeaderText="dependent_drug_id_4" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_4" HeaderText="dependent_drug_dosage_id_4" />
                                <asp:BoundField DataField="dependent_drug_package_id_4" HeaderText="dependent_drug_package_id_4" />
                                <asp:BoundField DataField="dependent_amount_30_days_4" HeaderText="dependent_amount_30_days_4" />

                                <asp:BoundField DataField="dependent_drug_id_5" HeaderText="dependent_drug_id_5" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_5" HeaderText="dependent_drug_dosage_id_5" />
                                <asp:BoundField DataField="dependent_drug_package_id_5" HeaderText="dependent_drug_package_id_5" />
                                <asp:BoundField DataField="dependent_amount_30_days_5" HeaderText="dependent_amount_30_days_5" />

                                <asp:BoundField DataField="dependent_drug_id_6" HeaderText="dependent_drug_id_6" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_6" HeaderText="dependent_drug_dosage_id_6" />
                                <asp:BoundField DataField="dependent_drug_package_id_6" HeaderText="dependent_drug_package_id_6" />
                                <asp:BoundField DataField="dependent_amount_30_days_6" HeaderText="dependent_amount_30_days_6" />

                                <asp:BoundField DataField="dependent_drug_id_7" HeaderText="dependent_drug_id_7" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_7" HeaderText="dependent_drug_dosage_id_7" />
                                <asp:BoundField DataField="dependent_drug_package_id_7" HeaderText="dependent_drug_package_id_7" />
                                <asp:BoundField DataField="dependent_amount_30_days_7" HeaderText="dependent_amount_30_days_7" />

                                <asp:BoundField DataField="dependent_drug_id_8" HeaderText="dependent_drug_id_8" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_8" HeaderText="dependent_drug_dosage_id_8" />
                                <asp:BoundField DataField="dependent_drug_package_id_8" HeaderText="dependent_drug_package_id_8" />
                                <asp:BoundField DataField="dependent_amount_30_days_8" HeaderText="dependent_amount_30_days_8" />

                                <asp:BoundField DataField="dependent_drug_id_9" HeaderText="dependent_drug_id_9" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_9" HeaderText="dependent_drug_dosage_id_9" />
                                <asp:BoundField DataField="dependent_drug_package_id_9" HeaderText="dependent_drug_package_id_9" />
                                <asp:BoundField DataField="dependent_amount_30_days_9" HeaderText="dependent_amount_30_days_9" />

                                <asp:BoundField DataField="dependent_drug_id_10" HeaderText="dependent_drug_id_10" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_10" HeaderText="dependent_drug_dosage_id_10" />
                                <asp:BoundField DataField="dependent_drug_package_id_10" HeaderText="dependent_drug_package_id_10" />
                                <asp:BoundField DataField="dependent_amount_30_days_10" HeaderText="dependent_amount_30_days_10" />

                                <asp:BoundField DataField="dependent_drug_id_11" HeaderText="dependent_drug_id_11" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_11" HeaderText="dependent_drug_dosage_id_11" />
                                <asp:BoundField DataField="dependent_drug_package_id_11" HeaderText="dependent_drug_package_id_11" />
                                <asp:BoundField DataField="dependent_amount_30_days_11" HeaderText="dependent_amount_30_days_11" />

                                <asp:BoundField DataField="dependent_drug_id_12" HeaderText="dependent_drug_id_12" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_12" HeaderText="dependent_drug_dosage_id_12" />
                                <asp:BoundField DataField="dependent_drug_package_id_12" HeaderText="dependent_drug_package_id_12" />
                                <asp:BoundField DataField="dependent_amount_30_days_12" HeaderText="dependent_amount_30_days_12" />

                                <asp:BoundField DataField="dependent_drug_id_13" HeaderText="dependent_drug_id_13" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_13" HeaderText="dependent_drug_dosage_id_13" />
                                <asp:BoundField DataField="dependent_drug_package_id_13" HeaderText="dependent_drug_package_id_13" />
                                <asp:BoundField DataField="dependent_amount_30_days_13" HeaderText="dependent_amount_30_days_13" />

                                <asp:BoundField DataField="dependent_drug_id_14" HeaderText="dependent_drug_id_14" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_14" HeaderText="dependent_drug_dosage_id_14" />
                                <asp:BoundField DataField="dependent_drug_package_id_14" HeaderText="dependent_drug_package_id_14" />
                                <asp:BoundField DataField="dependent_amount_30_days_14" HeaderText="dependent_amount_30_days_14" />

                                <asp:BoundField DataField="dependent_drug_id_15" HeaderText="dependent_drug_id_15" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_15" HeaderText="dependent_drug_dosage_id_15" />
                                <asp:BoundField DataField="dependent_drug_package_id_15" HeaderText="dependent_drug_package_id_15" />
                                <asp:BoundField DataField="dependent_amount_30_days_15" HeaderText="dependent_amount_30_days_15" />

                                <asp:BoundField DataField="dependent_drug_id_16" HeaderText="dependent_drug_id_16" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_16" HeaderText="dependent_drug_dosage_id_16" />
                                <asp:BoundField DataField="dependent_drug_package_id_16" HeaderText="dependent_drug_package_id_16" />
                                <asp:BoundField DataField="dependent_amount_30_days_16" HeaderText="dependent_amount_30_days_16" />

                                <asp:BoundField DataField="dependent_drug_id_17" HeaderText="dependent_drug_id_17" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_17" HeaderText="dependent_drug_dosage_id_17" />
                                <asp:BoundField DataField="dependent_drug_package_id_17" HeaderText="dependent_drug_package_id_17" />
                                <asp:BoundField DataField="dependent_amount_30_days_17" HeaderText="dependent_amount_30_days_17" />

                                <asp:BoundField DataField="dependent_drug_id_18" HeaderText="dependent_drug_id_18" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_18" HeaderText="dependent_drug_dosage_id_18" />
                                <asp:BoundField DataField="dependent_drug_package_id_18" HeaderText="dependent_drug_package_id_18" />
                                <asp:BoundField DataField="dependent_amount_30_days_18" HeaderText="dependent_amount_30_days_18" />

                                <asp:BoundField DataField="dependent_drug_id_19" HeaderText="dependent_drug_id_19" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_19" HeaderText="dependent_drug_dosage_id_19" />
                                <asp:BoundField DataField="dependent_drug_package_id_19" HeaderText="dependent_drug_package_id_19" />
                                <asp:BoundField DataField="dependent_amount_30_days_19" HeaderText="dependent_amount_30_days_19" />

                                <asp:BoundField DataField="dependent_drug_id_20" HeaderText="dependent_drug_id_20" />
                                <asp:BoundField DataField="dependent_drug_dosage_id_20" HeaderText="dependent_drug_dosage_id_20" />
                                <asp:BoundField DataField="dependent_drug_package_id_20" HeaderText="dependent_drug_package_id_20" />
                                <asp:BoundField DataField="dependent_amount_30_days_20" HeaderText="dependent_amount_30_days_20" />

                            </Columns>
                            <EmptyDataTemplate>
                                There are no data to display.
                            </EmptyDataTemplate>
                            <HeaderStyle CssClass="gridHeader" />
                        </asp:GridView>
                    </div>
                </fieldset>
            </asp:Panel>
               
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnParse" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
