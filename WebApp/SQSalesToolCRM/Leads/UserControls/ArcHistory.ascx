<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArcHistory.ascx.cs" Inherits="Leads_UserControls_ArcHistory" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>

<div id="fldSetGrid" class="condado" style="clear: both;">

                            <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                OnIndexChanged="Evt_PageNumberChanged" />
                            <br />
     <br />
                            <telerik:RadGrid ID="grdArcHistory" runat="server" Skin="" CssClass="mGrid" Width="100%"
                                CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                                AllowSorting="true" AutoGenerateColumns="False" 
                                OnSortCommand="grdArcHistory_SortGrid" SelectedItemStyle-CssClass="gridHeader">
                                <AlternatingItemStyle CssClass="alt" />                                
                                <MasterTableView DataKeyNames="">
                                    <NoRecordsTemplate>
                                        No record found.
                                    </NoRecordsTemplate>
                                    <Columns>                                                                                
                                        <telerik:GridBoundColumn DataField="FullName" FilterControlAltText="FullName"
                                            SortExpression="FullName" HeaderText="Indiividual Name" UniqueName="FullName">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="ArcRefreanceKey" FilterControlAltText="ArcRefreanceKey" SortExpression="ArcRefreanceKey"
                                            HeaderText="Arc Reference Number" UniqueName="ArcRefreanceKey">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="AddedOn" FilterControlAltText="AddedOn" SortExpression="AddedOn"
                                            HeaderText="Create Date" UniqueName="AddedOn">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Status" FilterControlAltText="Status" SortExpression="Status"
                                            HeaderText="Status" UniqueName="Status">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Notes" FilterControlAltText="Notes" SortExpression="Notes"
                                            HeaderText="Notes" UniqueName="Notes">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>                                        
                                    </Columns>
                                    <EditFormSettings>
                                        <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                        </EditColumn>
                                    </EditFormSettings>
                                </MasterTableView>
                                <HeaderStyle CssClass="gridHeader" />
                                <FilterMenu EnableImageSprites="False">
                                </FilterMenu>
                            </telerik:RadGrid>
                        </div>