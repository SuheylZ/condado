using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using SalesTool.DataAccess;
using System.Web.Security;

public partial class LoginPage : System.Web.UI.Page
{
    private GlobalAppSettings settings;
    public LoginPage()
    {
        settings = System.Web.HttpContext.Current. Application.ApplicationSettings();
    }
    public override void Dispose()
    {
        base.Dispose();
        settings = null;
    }
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (User.Identity.IsAuthenticated)
        {
            Response.Redirect(Konstants.K_HOMEPAGE, true);
        }
        //if (ApplicationSettings.IsSSOMode)
        if (settings.IsSSOMode)
        {

            if (Request.QueryString["NetworkUser"] != null)
            {
                DBEngine _E = null;
                _E = new DBEngine();
                _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
                //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
                _E.Init(ApplicationSettings.ADOConnectionString);
                var U = _E.UserActions.GetUserByNetworkName(Request.QueryString["NetworkUser"]);
                if (U != null)
                {
                    ValidateNetworkUser(U);
                }
            }
            //Response.Redirect(ApplicationSettings.DefaultSSOLoginPage);
            Response.Redirect(settings.DefaultSSOLoginPage);
        }

        if (!IsPostBack)
            Session[Konstants.K_FIRST_TIME] = true.ToString();

        if (Request.QueryString["action"] != null)
        {
            Response.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Request.QueryString["action"].Remove(0);
        }


        //AspnetSecurity sec = new AspnetSecurity();
        //sec.Login(Context, "rgrant@selectquotesenior.com", "National$0", Konstants.K_VIEW_LEADS_PAGE);

    }

    private void ValidateNetworkUser(SalesTool.DataAccess.Models.User U)
    {
        try
        {
            //if (Membership.ValidateUser(U.Email, ApplicationSettings.DefaultSSOPassword))
            if (Membership.ValidateUser(U.Email, settings.DefaultSSOPassword))
            {
                FormsAuthentication.SetAuthCookie(U.Email, false);
                Response.Redirect(Konstants.K_HOMEPAGE, true);
            }
            else
            {

                AspnetSecurity security = new AspnetSecurity();
                //security.ResetPassword(U.Key, ApplicationSettings.DefaultSSOPassword);
                //if (Membership.ValidateUser(U.Email, ApplicationSettings.DefaultSSOPassword))
                security.ResetPassword(U.Key, settings.DefaultSSOPassword);
                if (Membership.ValidateUser(U.Email, settings.DefaultSSOPassword))
                {
                    FormsAuthentication.SetAuthCookie(U.Email, false);
                    Response.Redirect(Konstants.K_HOMEPAGE, true);
                }
            }
            UpdateEventCalendarIsOpenedFlag(U.Email);
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }

    }

    protected void OnLoggedIn(object sender, EventArgs e)
    {
        TextBox tb = (TextBox)LoginUser.FindControl("UserName");
        UpdateEventCalendarIsOpenedFlag(tb.Text);
    }

    /// <summary>
    /// this function resets IsOpened flag to false at 
    /// loggedIn time. So that pending events can be opened again.
    /// </summary>
    /// <param name="userName"></param>
    private void UpdateEventCalendarIsOpenedFlag(String userName)
    {
        SqlConnection sqlCon = new SqlConnection(ApplicationSettings.ADOConnectionString);


        try
        {
            using (SqlCommand command = new SqlCommand("proj_UpdateEventCalendarIsOpenedFlag", sqlCon))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;

                sqlCon.Open();
                command.ExecuteNonQuery();
            }
        }
        catch
        {
            sqlCon.Close();
        }
        finally { sqlCon.Close(); }

    }

    //string WindowsUser(string sDomain)
    //{
    //    //System.Web.HttpContext.Current.Request.LogonUserIdentity.Impersonate();
    //   string x = Context.Request.LogonUserIdentity.Name;



    //    //PrincipalContext pc = null;
    //    //string userName = User.Identity.Name;
    //    //using (pc = new PrincipalContext(ContextType.Domain, sDomain))
    //    //{

    //    //    using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
    //    //    {
    //    //        UserPrincipal up = UserPrincipal.FindByIdentity(ctx, userName);
    //    //        UserPrincipal.FindByIdentity(pc, userName);
    //    //    }

    //    //    //pc.ValidateCredentials("suheyl.zafar", "@Password1");
    //    //    //principal = UserPrincipal.FindByIdentity(pc, userName);

    //    //}

    //    ////string firstName = principal.GivenName ?? string.Empty;
    //    ////string lastName = principal.Surname ?? string.Empty;

    //   string user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
    //   System.Security.Principal.WindowsImpersonationContext xt = System.Web.HttpContext.Current.Request.LogonUserIdentity.Impersonate();
    //   //UserPrincipal principal = UserPrincipal.FindByIdentity(xt, "");

    //    var xuser = User.Identity.Name;
    //    return System.Environment.UserName;
    //}
}
