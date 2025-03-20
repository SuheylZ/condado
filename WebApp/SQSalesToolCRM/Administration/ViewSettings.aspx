<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/Site.master" CodeFile="ViewSettings.aspx.cs" Inherits="ViewSettings" EnableEventValidation="true"%>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <div>
       
            <telerik:RadGrid runat="server" ID="radGrid" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt" GridLines="None"
                CssClass="mGrid" Skin="" AllowFilteringByColumn="True" AllowMultiRowSelection="True" EnableTheming="False" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt">
                 <ClientSettings EnableRowHoverStyle="true">
                                <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                            </ClientSettings>
                 <MasterTableView>
                    <Columns>
                        <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="1" />
                        <telerik:GridBoundColumn AllowFiltering="True" AllowSorting="True" DataField="Name" HeaderText="Property" UniqueName="Name"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="True" AllowSorting="True" DataField="Key" HeaderText="Database Key" UniqueName="Key"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Value" HeaderText="Value" UniqueName="Value"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="AppliedAt" HeaderText="AppliedAt" UniqueName="AppliedAt"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <%-- <table>

                <asp:Repeater runat="server" ID="Rptsettings">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%#Eval("Name") %>
                            </td>
                            <td>
                                <%#Eval("Key") %>
                            </td>
                            <td>
                                <%#Convert.ToString(Eval("Value")) %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>--%>
        
 </div>
</asp:Content>