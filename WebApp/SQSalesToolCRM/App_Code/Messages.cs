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
using System.Linq;

public class Messages
{
    public const string UserCreated = "Specified user has been created successfully!";
    public const string UserUpdated = "User information has been updated";

    public const string PasswordChanged = "Password has been changed successfully";
    public const string RevertPermissions = "Are you sure you would like to revert to the assigned role's default settings?";

    public const string RoleSaved = "Role has been saved successfully.";
    //YA[May 03, 2013] General messages added to use in all of the grid and forms. 
    public const string RecordSavedSuccess = "Record saved successfully.";
    public const string RecordDeletedSuccess = "Record deleted successfully.";
    public const string RecordUpdatedSuccess = "Record updated successfully.";
    public const string RecordMovedSuccess = "Record moved successfully.";
    public const string RecordMovedUpSuccess = "Record moved up successfully.";
    public const string RecordMovedDownSuccess = "Record moved down successfully.";

    public const string RecordPrimaryIndividualDeleteFail = "Cannot delete primary individual.";
    
    public const string StatusChangeAssigned = "Status change has been assigned!";
    
    //For users multi business
    public const string UserMultiBusinessCreated = "User Multi-Business has been created successfully.";
    public const string UserMultiBusinessUpdated = "User Multi-Business has been updated successfully.";

    public const string MailSubject = "SQ Sales Tool (account created)";
    public const string MailBody = "Greetings {3}!\nYou account has been created successfully. \nlogin name: {0} \nPassword: {1}";

    //wm - 01.05.2013
    public const string ConfirmDelete = "Are you sure to delete the record?";
    //YA[May 28, 2013] 
    public const string NoRecord = "No record found.";

    public const string ArcCallSuccess = "New Arc call placed successfully.";
    public const string NoRecordModified = "No records were affected";
    
}

public class ErrorMessages
{
    public const string NOPermissions = "The user does not have any permissions. The application security module cannot allow user to continue any further.";
    public const string EmailAlreadyExists = "The specified email already exists. Specify some other email address";
    public const string NOSecurityAccount = "User is not associated with a security account. Contact your administrator";
    public const string UserDoesNotExist = "the user does not exist in the seucurity database. Is the database altered?";

    public const string PasswordBlank = "Blank passwords are not allowed";
    public const string PasswordNotchanged = "Due to some internal problem password could not be changed. try again later.";

    public const string CannotRemoveAssignedRole = "Cannot remove role, it is currently assigned to users.";
    public const string LoggedInUserDeleteRoleError= "Cannot delete the role that is currently logged in.";

    public const string ErrorParsing = "Error In parsing the string. ";
    public const string ActionDeleteError = "The action cannot be removed as it is used by lead record.";
    public const string CustomFilterError = "Error: Custom filter criteria not defined correctly.";
    public const string CustomFilterValueError = "Error in custom filter value.";
    public const string SameTitleError = "Same title already exists, please change the title text.";

    //YA[May 18, 2013] For Custom Reports
    public const string DuplicateReportTitle = "Report title already exists.";
    public const string EnterReportTitle = "Please enter report title.";
    public const string SelectReportColumns = "Please select report columns.";
    //YA[May 29, 2013] Select Email Recipients
    public const string SelectEmailRecipients = "Select Email Recipient(s).";
    //For users multi business
    public const string DuplicateMultiUser = "'User Multi-Business' already exist for this company and the user";
    public const string DuplicateMultiUserPerCompany = "Only one 'User Multi-Business' can be added per company for a user";

    public const string RecordDoesNotExist = "Record does not exist or some general error occured.";
    public const string General = "A general error has occured while performing the operation.";
    public const string PrioritizationUserTypeRequired = "Cannot save record: User type required";
}