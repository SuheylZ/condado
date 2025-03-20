<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IndividualInformation.aspx.cs" Inherits="Leads_IndividualInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">

    function ChcekFaxnoEmpty_DisableCellPhDial() {
            var faxno = document.getElementById('txtFaxNo').value;
            if (faxno.length == 0) {
                //alert(document.getElementById('spanclkDial'));
                document.getElementById('spanclkDial').enabled = false;
            }

        }
       
    function cancel() {
        window.parent.document.getElementById('btnCancel').click();
         }
         </script>
         <link href="../Styles/StyleSheet.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
  <table><tr><td><div style="background-color:Black;background: url(../Images/cross_icon_normal.png);background-repeat: no-repeat;height: 16px;width: 16px;float: right;cursor: pointer;position:relative" onclick="cancel();"></div> </td>
  </tr></table>

                 
                    
                  
    <div>
           <asp:Panel ID="SentToCustomerContentPanel" runat="server" >

           <table>
           <tr>
           <td>
                              <table>
                                                                                           <tr>
                    <td></td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="170px"> 
                        <span style="margin-left:25%">
                            First Name:

                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto"> 
                            <asp:TextBox ID="txtFirstName" runat="server" Width="180px"></asp:TextBox>

                        </span> 

                    </td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="170px" >
                        <span style="margin-left:25%">
                            Last Name:
                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto"> 
                            <asp:TextBox ID="txtLastName" runat="server" Width="180px"></asp:TextBox>

                        </span> 

                    </td>
                    <td></td>
            </tr>
            
                                                                                           <tr>
                        <td></td>
                         <td style="text-align:center" bgcolor="#CCFFFF" width="170px"> 
                            <span style="margin-left:26%">
                                Date of Birth:

                            </span>
                        </td>
                        <td width="125px">
                            <span style="margin-left:auto"> 
                                <asp:TextBox ID="txtDOB" runat="server" Width="180px"></asp:TextBox>

                            </span> 

                        </td>
                        <td style="text-align:center" bgcolor="#CCFFFF" width="170px" >
                            <span style="margin-left:25%">
                                Is Primary:
                            </span>
                        </td>
                        <td width="125px">
                            <span style="margin-left:auto"> 
                                <asp:CheckBox ID="chkIsPrimary" runat="server" />

                            </span> 

                        </td>
                        <td></td>
            </tr>
            
                                                                                           <tr>
                 <td></td>
                 <td style="text-align:center" bgcolor="#CCFFFF" width="170px" colspan="2"> 
                    <span style="margin-left:26%">
                        

                    </span>
                </td>
               
                <td style="text-align:center" bgcolor="#CCFFFF" width="170px" >
                    <span style="margin-left:25%">
                        Relation to Primary:
                    </span>
                </td>
                <td width="125px">
                    <span style="margin-left:auto"> 
                       <asp:DropDownList ID="ddlRelationtoPrimary"  Width="182px" runat="server"></asp:DropDownList>

                    </span> 

                </td>
                <td></td>
            </tr>
                               </table>
           
           </td>
</tr>

<tr><td></td></tr>

<tr>
   <td> 
   <table>
   <tr><td colspan="6"></td></tr>
           <tr>
                    <td></td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="180px"> 
                        <span style="margin-left:25%">
                            Home Phone Number:

                        </span>
                    </td>
                    <td style="width:220px">
                    <table><tr><td>  <span style="margin-left:auto"> 
                            <asp:TextBox ID="txtHomePhno" runat="server" Width="140px"></asp:TextBox>

                        </span> </td><td style="width:100px"><span id="spanclkDial"><asp:LinkButton ID="lnkClktoDial" runat="server" Text="Click To Dial"></asp:LinkButton></span> </td></tr></table>
                      

                    </td>
                    <td style="text-align:center" bgcolor="#CCFFFF"  width="125px" >
                        <span style="margin-left:25%">
                            Last Name:
                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto"> 
                            <asp:TextBox ID="TextBox2" runat="server" Width="180px"></asp:TextBox>

                        </span> 

                    </td>
                    <td></td>
            </tr>

            <tr>
                    <td></td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="180px"> 
                        <span style="margin-left:25%">
                            Cell Phone Number:

                        </span>
                    </td>
                    <td style="width:220px">
                    <table><tr><td>  <span style="margin-left:auto"> 
                            <asp:TextBox ID="txtCellPhNo" runat="server" Width="140px"></asp:TextBox>

                        </span> </td><td style="width:100px"><a href="#" id="hrfClkDial" runat="server" >&nbsp;Click To Dial</a></td></tr></table>
                      

                    </td>
                    <td style="text-align:center"   width="125px" >
                        <span style="margin-left:25%">
                           
                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto"> 
                          

                        </span> 

                    </td>
                    <td></td>
            </tr>

              <tr>
                    <td></td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="180px"> 
                        <span style="margin-left:25%">
                           Fax Number:

                        </span>
                    </td>
                    <td style="width:220px">
                    <table><tr><td>  <span style="margin-left:auto"> 
                            <asp:TextBox ID="txtFaxNo" runat="server" Width="140px"></asp:TextBox>

                        </span> </td><td style="width:100px"></td></tr></table>
                      

                    </td>
                    <td style="text-align:center"   width="125px" >
                        <span style="margin-left:25%">
                           
                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto"> 
                          

                        </span> 

                    </td>
                    <td></td>
            </tr>
            <tr><td colspan="6"></td></tr>
            </table>
  
  </td>
</tr>
          
<tr>
<td>
      <table>
                <tr>
                 <td></td>
                    <td style="text-align:left" bgcolor="#CCFFFF" width="119px" colspan="4"> 
                        <span style="margin-left:10%">
                           Address:

                        </span>
                          <span style="margin-left:9.5%"> 
                            <asp:TextBox ID="txtAddress1" runat="server" Width="330px"></asp:TextBox>

                        </span> 
                    </td>
                 <td></td>
                  
            </tr>
               <tr>
                 <td></td>
                    <td style="text-align:left" bgcolor="#CCFFFF" width="119px" colspan="4"> 
                        <span style="margin-left:15.3%">
                           

                        </span>
                          <span style="margin-left:10.5%"> 
                            <asp:TextBox ID="txtAddress2" runat="server" Width="330px"></asp:TextBox>

                        </span> 
                    </td>
                 <td></td>
                  
            </tr>
                
                  <tr>
                    <td></td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="181px"> 
                        <span style="margin-left:25%">
                            City:

                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:1%"> 
                            <asp:TextBox ID="txtCity" runat="server" Width="180px"></asp:TextBox>

                        </span> 

                    </td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="170px" >
                        <span style="margin-left:25%">
                            State:
                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto"> 
                           <asp:DropDownList ID="ddlState" runat="server" Width="180px"></asp:DropDownList>

                        </span> 

                    </td>
                    <td></td>
            </tr>
             

                  <tr>
                    <td></td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="181px"> 
                        <span style="margin-left:25%">
                            Zip Code:

                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:1%"> 
                            <asp:TextBox ID="txtZipCode" runat="server" Width="180px"></asp:TextBox>

                        </span> 

                    </td>
                    <td style="text-align:center" bgcolor="#CCFFFF" width="170px" >
                        <span style="margin-left:25%;display:none">
                            
                        </span>
                    </td>
                    <td width="125px">
                        <span style="margin-left:auto;display:block"> 
                          

                        </span> 

                    </td>
                    <td></td>
            </tr>


            </table>

</td>
</tr>

<tr></tr>
<tr></tr>
    <tr>
    <td>
                        <table>
                            <tr>
                                          <td colspan ="2" style="padding-top:6px; padding-left:40%">
                                           <asp:Button ID="btnContactSave" runat="server" Text="Save"   />
                                           <span style="padding-left:2%"> <asp:Button ID="btnContactSaveClose" runat="server" Text="Save and Close"   /></span>
                                           <span style="padding-left:2%">  <asp:Button ID="btnContactClose" runat="server" Text="Close"  /></span>
                                          </td>
                            </tr>

                     </table>
    </td>
    </tr>
           </table>
   
    


   

</asp:Panel>

    </div>
    </form>
</body>
</html>
