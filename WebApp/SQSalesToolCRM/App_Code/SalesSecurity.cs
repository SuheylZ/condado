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

/// <summary>
/// Summary description for SalesSecurity
/// </summary>
[Serializable()]
public class SalesSecurity
{
    private uint _administration = 0;

    #region konstants
    private const uint K_USERS = 1;
    private const uint K_ROLES = 2;
    private const uint K_SKILLGROUPS = 4;
    private const uint K_CAMPAIGNS = 8;
    private const uint K_GETALEAD = 16;
    private const uint K_ROUTING = 32;
    private const uint K_EMAILTEMPLATES = 64;
    private const uint K_POSTS = 128;
    private const uint K_PRIORITIZATION = 256;
    private const uint K_QUICKLINKS = 512;
    private const uint K_RETENTION = 1024;
   private const uint K_ALERTS = 2048;
    private const uint K_MANAGE_DUPLICATES = 1 << 12;
    private const uint K_VIEW_DUPLICATES = 1 << 13;
    private const uint K_MANAGE_DASHBOARD = 1 << 14;
    private const uint K_REASSIGNMENT = 1 << 15;
    private const uint K_CANDELETE = 1 << 16;

    #endregion

    #region methods
    public SalesSecurity(SalesTool.DataAccess.Models.UserPermissions perm)
    {
        _administration |= (perm.Permissions.Administration.CanManageUsers) ? K_USERS : 0;
        _administration |= (perm.Permissions.Administration.CanManageRoles) ? K_ROLES : 0;
        _administration |= (perm.Permissions.Administration.CanManageSkillGroups) ? K_SKILLGROUPS : 0;
        _administration |= (perm.Permissions.Administration.CanManageCampaigns) ? K_CAMPAIGNS : 0;
        _administration |= (perm.Permissions.Administration.CanManageGetALead) ? K_GETALEAD : 0;
        _administration |= (perm.Permissions.Administration.CanManageRouting) ? K_ROUTING : 0;
        _administration |= (perm.Permissions.Administration.CanManageEmailTemplates) ? K_EMAILTEMPLATES : 0;
        _administration |= (perm.Permissions.Administration.CanManagePosts) ? K_POSTS : 0;
        _administration |= (perm.Permissions.Administration.CanManagePrioritization) ? K_PRIORITIZATION : 0;
        _administration |= (perm.Permissions.Administration.CanManageReassignment) ? K_REASSIGNMENT : 0;
        _administration |= (perm.Permissions.Administration.CanManageQuickLinks) ? K_QUICKLINKS : 0;
        _administration |= (perm.Permissions.Administration.CanManageRetention) ? K_RETENTION : 0;
        _administration |= (perm.Permissions.Administration.CanManageAlerts) ? K_ALERTS : 0;
        _administration |= perm.Permissions.Administration.CanManageDuplicateRules ? K_MANAGE_DUPLICATES : 0;
        _administration |= perm.Permissions.Administration.CanViewDuplicates ? K_VIEW_DUPLICATES : 0;
        _administration |= perm.Permissions.Administration.CanManageDashboard ? K_MANAGE_DASHBOARD : 0;
        _administration |= perm.Permissions.Administration.CanDelete ? K_CANDELETE : 0;

    }
    private bool IsAllowed(uint iPosition)
    {
        bool bValue = false;
        bValue = Convert.ToBoolean(_administration & iPosition);
        return bValue;
    }
    #endregion

    #region properties
    public bool CanManageAlerts { get { return IsAllowed(K_ALERTS); } }
    public bool CanManageUsers { get { return IsAllowed(K_USERS); } }
    public bool CanManageRoles { get { return IsAllowed(K_ROLES); } }
    public bool CanManageSkillGroups { get { return IsAllowed(K_SKILLGROUPS); } }
    public bool CanManageCampaigns { get { return IsAllowed(K_CAMPAIGNS); } }
    public bool CanManageGetALead { get { return IsAllowed(K_GETALEAD); } }
    public bool CanManageOutboundRouting { get { return IsAllowed(K_ROUTING); } }
    public bool CanManageEmailTemplates { get { return IsAllowed(K_EMAILTEMPLATES); } }
    public bool CanManagePosts { get { return IsAllowed(K_POSTS); } }
    public bool CanManagePrioritizationRules { get { return IsAllowed(K_PRIORITIZATION); } }
    public bool CanManageQuickLinks { get { return IsAllowed(K_QUICKLINKS); } }
    public bool CanManageRetention { get { return IsAllowed(K_RETENTION); } }
    public bool CanManageReassignment { get { return IsAllowed(K_REASSIGNMENT); } }
    public bool CanManageDuplicateRules { get { return IsAllowed(K_MANAGE_DUPLICATES); } }
    public bool CanViewDuplicates { get { return IsAllowed(K_VIEW_DUPLICATES); } }
    public bool CanManageDashboard { get { return IsAllowed(K_MANAGE_DASHBOARD); } }
    public bool IsAdministrationOff
    {
        get
        {
            return _administration == 0;
        }
    }

    public bool CanDelete { get { return IsAllowed(K_CANDELETE); } }

    #endregion
}