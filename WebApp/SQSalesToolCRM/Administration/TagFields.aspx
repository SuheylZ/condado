<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TagFields.aspx.cs" Inherits="Admin_TagFields" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

    <asp:Label ID="lblMessage" runat="server" Text="" CssClass="LabelMessage"></asp:Label>

    <asp:GridView ID="grdTagFields" runat="server" Width="100%" 
                  AutoGenerateColumns="False"
                DataKeyNames="TagFieldKey" AllowSorting="True" GridLines="None" AlternatingRowStyle-CssClass="alt"
                CssClass="mGrid">
                <AlternatingRowStyle CssClass="alt" />
                <Columns>
                    <asp:BoundField DataField="TagFieldKey" HeaderText="Key"  Visible="false">                        
                        
                    </asp:BoundField>                                        
                     <asp:BoundField DataField="TagFieldName" HeaderText="Name" SortExpression="TagFieldName">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="50%" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TagFieldValue" HeaderText="Value" SortExpression="TagFieldValue">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="50%" />
                    </asp:BoundField>                    
                   
                </Columns>
                <EmptyDataTemplate>
                    No record found.
                </EmptyDataTemplate>
                <HeaderStyle CssClass="gridHeader" />
                <PagerSettings Position="Top" />
                <PagerStyle VerticalAlign="Bottom" />
            </asp:GridView>
    </div>
    </form>
</body>
</html>
