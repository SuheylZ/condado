<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AccountAttachments.ascx.cs"
    Inherits="UserControls_AccountAttachments" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>



<div runat="server" id="divAttachment" class="condado">
    <ul>
        <li>
            <table>
                <tr style="vertical-align: top">
                    <td>

                        <telerik:RadAsyncUpload ID="fUploadAsync" runat="server" UploadedFilesRendering="BelowFileInput" MaxFileSize="0"
                            MaxFileInputsCount="1"  Height="27px" Width="320px">
                        </telerik:RadAsyncUpload>
                        <br/>
                        &nbsp;&nbsp;  <b>Note: File size maximum limit is 100 MB </b>
                        <asp:HiddenField runat="server" ID="hdnFieldAccountId" Value="0" />
                        <asp:HiddenField ID="hdSortColumn" runat="server" />
                        <asp:HiddenField ID="hdSortDirection" runat="server" />                        
                    </td>
                    <td>
                        <div id="ErrorHolder">
                            <uc1:StatusLabel ID="lblMessageForm" runat="server" />
                        </div>


                    </td>
                    <td>
                        <asp:Label ID="lblDescription" runat="server">Description</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server" MaxLength="200" ValidationGroup="AttachmentUpload"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqFldVldDescription" runat="server" ControlToValidate="txtDescription"
                            Display="None" ErrorMessage="Enter attachment description." ValidationGroup="AttachmentUpload"></asp:RequiredFieldValidator>
                    </td>
                    <td>
                        <asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Upload"
                            ValidationGroup="AttachmentUpload" CausesValidation="true" OnClientClick="validateGroup('AttachmentUpload');" />
                    </td>
                </tr>
            </table>
        </li>
        <li>
            <asp:UpdatePanel ID="updLead" runat="server" EnableViewState="true">
                <ContentTemplate>
                    <asp:GridView ID="grdTemplateAttachments" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        CssClass="mGrid" DataKeyNames="TemplateAttachmentKey" GridLines="None" OnRowCommand="grdTemplateAttachments_RowCommand"
                        Width="100%" OnSorting="grdTemplateAttachments_Sorting">
                        <AlternatingRowStyle CssClass="alt" />
                        <Columns>
                            <asp:BoundField DataField="TemplateAttachmentKey" HeaderText="Key" Visible="False" />
                            <asp:TemplateField HeaderText="File">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDownloadAttachment" runat="server" CausesValidation="False"
                                        CommandName="Download" Text="Download"></asp:LinkButton>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="10%" />
                            </asp:TemplateField>
                            <%--<asp:BoundField DataField="TemplateAttachmentFileName" HeaderText="File Name" SortExpression="TemplateAttachmentFileName">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                    </asp:BoundField>--%>
                            <asp:BoundField DataField="TemplateAttachmentDescription" HeaderText="Description"
                                SortExpression="TemplateAttachmentDescription">
                                <HeaderStyle HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" Width="70%" />
                            </asp:BoundField>
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDeleteAttachment" runat="server" CausesValidation="False"
                                        CommandName="DeleteAttachment" OnClientClick="if (!confirmDelete()) return false;"
                                        Text="Delete"></asp:LinkButton>
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
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="grdTemplateAttachments" />
                </Triggers>
            </asp:UpdatePanel>
        </li>
    </ul>
</div>

