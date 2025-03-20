// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      C:\Projects\Live\SQ Sales Tool\SQSalesToolCRM\
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Security = System.Web.Security;

/// <summary>
/// Summary description for MembershipSecurity
/// </summary>
public class AspnetSecurity
{
    public Guid Create(string login, string userName, string email, bool isActive)
    {
        string password = Security.Membership.GeneratePassword(6, 0);
        Security.MembershipUser aspUser = Security.Membership.CreateUser(login, password, email);
        aspUser.IsApproved = true;
        Security.Membership.UpdateUser(aspUser);
        Guid key = Guid.Parse(aspUser.ProviderUserKey.ToString());
        //SendPasswordEmail(email, userName, login, password);
        return key;
    }
    public void Change(Guid key, string userName, string email, bool isActive)
    {
        Security.MembershipUser aspUser = Security.Membership.GetUser(key);
        // does not work. asp membership does not allow changing user name
        // aspUser.UserName = userName;    
        if (aspUser == null)
            throw new Exception(ErrorMessages.NOSecurityAccount);

        aspUser.Email = email;
        aspUser.IsApproved = isActive;
        Security.Membership.UpdateUser(aspUser);
    }

    public void Active(Guid key, bool isActive)
    {
        Security.MembershipUser aspUser = Security.Membership.GetUser(key);
        if (aspUser == null)
            throw new Exception(ErrorMessages.NOSecurityAccount);

        aspUser.IsApproved = isActive;
        Security.Membership.UpdateUser(aspUser);
    }

    public void ResetPassword(Guid key, string newPassword)
    {

        newPassword = newPassword.Trim();
        if (newPassword == string.Empty)
            throw new Exception(ErrorMessages.PasswordBlank);
        Security.MembershipUser aspUser = Security.Membership.GetUser(key);
        if (aspUser == null)
            throw new UnauthorizedAccessException(ErrorMessages.NOSecurityAccount);
        if (aspUser.IsLockedOut)
            aspUser.UnlockUser();
        string tmpPassword = aspUser.ResetPassword();
        bool bChanged = aspUser.ChangePassword(tmpPassword, newPassword);
        if (!bChanged)
            throw new Exception(ErrorMessages.PasswordNotchanged);
    }

    public void Delete(Guid key)
    {
        Security.MembershipUser aspUser = Security.Membership.GetUser(key);
        if(aspUser!=null)
            Security.Membership.DeleteUser(aspUser.UserName, true);
    }

    //SZ [Aug 15, 2013] this logs in the user programatically 
    public void Login(HttpContext ctx, string user, string password, string homepage)
    {
        if (Security.Membership.ValidateUser(user, password))
        {
            ctx.Response.Cookies.Remove(Security.FormsAuthentication.FormsCookieName);
            Security.FormsAuthentication.SetAuthCookie(user, true);
            ctx.Response.Redirect(homepage);
        }
    }

    public Guid Login(string user, string password)
    {
        Guid gAns = Guid.Empty;
        bool bRet = Security.Membership.ValidateUser(user, password);
        if (bRet)
        {
            Security.MembershipUser U = Security.Membership.GetUser(user);
            gAns = new Guid(U.ProviderUserKey.ToString());
        }
        return gAns;
    }

    public Security.MembershipUser GetUserDetails(string login)
    {
        return Security.Membership.GetUser(login);
    }
}