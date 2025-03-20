<%@Page Title="" Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/GAL.master" 
    CodeFile="DashboardEditor.aspx.cs" Inherits="SQS_Dialer.DashboardEditor"%>
<%@ MasterType VirtualPath="~/MasterPages/GAL.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


        
<style type="text/css">
  .mGrid tr:hover
{
      color: #fff;
    background: #5D7B9D/*rgb(232,145,69)*/;
}
  /*.mGrid td:nth-child(2):hover 
{
    background: rgb(232,145,69);
}*/
 /* .mGrid table tr td:nth-child(2n+1) 
{
    background: rgb(232,145,69);
}*/
  /*table tr td:nth-child('+ownerIndex+')
 .mGrid th:hover

 {           

     background-color: #8064a2;

     color: #fff;

     border: 1px solid #ccc;

 }

 .mGrid th a

 {

     color: #fff;

     text-decoration: none;

    }*/
  .hover { background-color: #eee; }
    
</style>
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2><asp:Label ID="lblAgentCampaignsTitle0" runat="server" Text="Dashboard Editor: "/></h2>
                <asp:DropDownList ID="DashModule" runat="server" AutoPostBack="true" Width="175px" OnSelectedIndexChanged="DashModule_SelectedIndexChanged">
                    <asp:ListItem Selected="True">Campaign Groups</asp:ListItem>
                    <asp:ListItem>State Groups</asp:ListItem>
                    <asp:ListItem>Age Groups</asp:ListItem>
                </asp:DropDownList>
                &nbsp;

               
    <asp:Button ID="btnDashUpdate" runat="server" Text="Update" OnClick="btnDashUpdate_Click" />
    <br />
   
  
                <asp:SqlDataSource ID="CampaignGroupDash" runat="server" 
                    ConnectionString="<%$ConnectionStrings:ApplicationServices%>" 
                    
                    SelectCommand="spDashboardViewCampaignGroups" 
                    SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="StateDash" runat="server" 
                    ConnectionString="<%$ConnectionStrings:ApplicationServices%>"                     
                    SelectCommand="spDashboardViewStateGroups" 
                    SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
                </asp:SqlDataSource>
                <asp:SqlDataSource ID="AgeDash" runat="server" 
                    ConnectionString="<%$ConnectionStrings:ApplicationServices%>"                       
                    SelectCommand="spDashboardViewAgeGroups" 
                    SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="bGALType" Type="Boolean"  />
                </SelectParameters>
                </asp:SqlDataSource>

      <telerik:RadGrid ID="grdDashEditor" runat="server"  Width="1500px" DataSourceID="CampaignGroupDash" Height="800px"
                                                                 BorderWidth="1px" AutoGenerateColumns="False" CssClass="mGrid" Skin=""  EnableTheming="true"
                                                                 ViewStateMode="Enabled" CellSpacing="0" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt"
                                                                 onfocus="this.blur();" GridLines="None" OnItemDataBound="grdDashEditor_ItemDataBound" ShowHeader="true">
                                                                 <MasterTableView Width="100%" DataKeyNames="agent_group_id" >
                                                                     <NoRecordsTemplate>
                                                                         No record to display.
                                                                     </NoRecordsTemplate>
                                                                     <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                                                     </RowIndicatorColumn>
                                                                     <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                                                     </ExpandCollapseColumn>
                                                                  
                                                                 </MasterTableView>
                                                                 <ClientSettings  EnableRowHoverStyle="true" AllowKeyboardNavigation="true" >
                                                                     <Selecting UseClientSelectColumnOnly="true" AllowRowSelect="true" CellSelectionMode="MultiColumn" />
                                                                     <Scrolling AllowScroll="True" UseStaticHeaders="true"  FrozenColumnsCount="2"></Scrolling>                                                                    
                                                                 </ClientSettings>
                                                                 <HeaderStyle CssClass="gridHeader" />
                                                                
                                                                 <FilterMenu EnableImageSprites="False">
                                                                 </FilterMenu>
          
                                                             </telerik:RadGrid>
      <script language="javascript" type="text/javascript">

          //For selecting the column on row hover
          //$(function () {

          //    $("table[class='mGrid']").delegate('td', 'mouseover mouseleave', function (e) {
          //        if (e.type == 'mouseover') {
          //            alert("asfd");
          //            $(this).parent().addClass("hover");
          //            $("colgroup").eq($(this).index()).addClass("hover");
          //        } else {
          //            $(this).parent().removeClass("hover");
          //            $("colgroup").eq($(this).index()).removeClass("hover");
          //        }
          //    });

          //});

          //$('.mGrid,td').hover(function () {
          //    var t = parseInt($(this).index()) + 1;
          //    $('td:nth-child(' + t + ')').addClass('hover');
          //},
          //  function () {
          //      var t = parseInt($(this).index()) + 1;
          //      $('td:nth-child(' + t + ')').removeClass('hover');
          //  });

    </script>
    </asp:Content>
