<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ArcCases.ascx.cs" Inherits="Leads_UserControls_ArcCases" %>

<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>

<div id="fldSetGrid" class="condado" style="clear: both;">

                            <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                OnIndexChanged="Evt_PageNumberChanged" />
                            <br /> <br />
                            <telerik:RadGrid ID="grdArcCases" runat="server" Skin="" CssClass="mGrid" Width="100%"
                                CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                                AllowSorting="True" AutoGenerateColumns="False" 
                                OnSortCommand="grdArcCases_SortGrid" SelectedItemStyle-CssClass="gridHeader">
                                <AlternatingItemStyle CssClass="alt" />                                
                                <MasterTableView DataKeyNames="">
                                    <NoRecordsTemplate>
                                        No record found.
                                    </NoRecordsTemplate>

<CommandItemSettings ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif" ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2013.2.717.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"></CommandItemSettings>
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
                                        <telerik:GridBoundColumn DataField="AddOn" FilterControlAltText="AddOn" SortExpression="AddOn"
                                            HeaderText="Create Date" UniqueName="AddOn">
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
                                            <ItemStyle HorizontalAlign="Left" Width="50%" />
                                        </telerik:GridBoundColumn>                          
                                        <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" UniqueName="TemplateColumn" ItemStyle-Width="50px">
                                            <ItemTemplate>
                                                <input type="hidden" id="recordAccountId" value='<%#DataBinder.Eval(Container.DataItem,"AccountKey")%>'/>
                                                <button id="btnOpenCase" onclick="openArc('<%#DataBinder.Eval(Container.DataItem, "ArcRefreanceKey")%>','<%#DataBinder.Eval(Container.DataItem,"AccountKey")%>', '<%#DataBinder.Eval(Container.DataItem,"IndividualId") %>');return false;">Open Case</button>
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                    <EditFormSettings>
                                        <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                        </EditColumn>
                                    </EditFormSettings>
                                </MasterTableView>
                                <HeaderStyle CssClass="gridHeader" />
                                <FilterMenu EnableImageSprites="False">
                                </FilterMenu>

<SelectedItemStyle CssClass="gridHeader"></SelectedItemStyle>
                            </telerik:RadGrid>
                        </div>