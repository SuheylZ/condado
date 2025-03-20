<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RequiredFields.ascx.cs" Inherits="UserControls_RequiredFields" %>
<%@ Register src="PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<br />
<uc1:PagingBar ID="pgPager" runat="server" />
 <telerik:RadGrid ID="rgRequiredFields" runat="server" Width="100%"
         AllowSorting="True" GridLines="None" AlternatingRowStyle-CssClass="alt" onfocus="this.blur();" CssClass="mGrid" 
         Skin="" EnableTheming="False" CellSpacing="0" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt" 
         AutoGenerateColumns="False" >
                <AlternatingItemStyle CssClass="alt" />
                            
                <MasterTableView>
                    <NoRecordsTemplate>
                        There are no records to display at the moment.
                    </NoRecordsTemplate>

                    <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                        <HeaderStyle Width="20px"/>
                    </RowIndicatorColumn>

                    <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                        <HeaderStyle Width="20px"/>
                    </ExpandCollapseColumn>

                    <Columns>
                        <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="colID" Visible="False"/>
                        <telerik:GridBoundColumn DataField="Title" HeaderText="Title" UniqueName="colTitle" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="40%" />
                    </Columns>
                </MasterTableView>

            </telerik:RadGrid>
