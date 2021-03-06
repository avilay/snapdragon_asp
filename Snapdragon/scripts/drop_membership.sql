USE [Snapdragon]
GO

-- drop procedures
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_AnyDataInTables')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_AnyDataInTables
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Applications_CreateApplication')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Applications_CreateApplication
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_CheckSchemaVersion')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_CheckSchemaVersion
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_ChangePasswordQuestionAndAnswer')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_ChangePasswordQuestionAndAnswer
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_CreateUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_CreateUser
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_FindUsersByEmail')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_FindUsersByEmail
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_FindUsersByName')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_FindUsersByName
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetAllUsers')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetAllUsers
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetNumberOfUsersOnline')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetNumberOfUsersOnline
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetPassword')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetPassword
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetPasswordWithFormat')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetPasswordWithFormat
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetUserByEmail')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetUserByEMail
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetUserByName')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetUserByName
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_GetUserByUserId')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_GetUserByUserId
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_ResetPassword')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_ResetPassword
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_SetPassword')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_SetPassword
GO


IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_UnlockUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_UnlockUser
GO


IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_UpdateUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_UpdateUser
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership_UpdateUserInfo')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Membership_UpdateUserInfo
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_RegisterSchemaVersion')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_RegisterSchemaVersion
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Setup_RemoveAllRoleMembers')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Setup_RemoveAllRoleMembers
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Setup_RestorePermissions')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Setup_RestorePermissions
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_UnRegisterSchemaVersion')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_UnRegisterSchemaVersion
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users_CreateUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Users_CreateUser
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users_DeleteUser')
               AND (type = 'P')))
DROP PROCEDURE [dbo].aspnet_Users_DeleteUser
GO

-- drop all views
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'vw_aspnet_Applications')
               AND (type = 'V')))
DROP VIEW [dbo].vw_aspnet_Applications
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'vw_aspnet_MembershipUsers')
               AND (type = 'V')))
DROP VIEW [dbo].vw_aspnet_MembershipUsers
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'vw_aspnet_Users')
               AND (type = 'V')))
DROP VIEW [dbo].vw_aspnet_Users
GO

-- drop all tables
IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_SchemaVersions')
               AND (type = 'U')))
DROP TABLE [dbo].aspnet_SchemaVersions
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Membership')
               AND (type = 'U')))
DROP TABLE [dbo].aspnet_Membership
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Users')
               AND (type = 'U')))
DROP TABLE [dbo].aspnet_Users
GO

IF (EXISTS (SELECT name
              FROM sysobjects
             WHERE (name = N'aspnet_Applications')
               AND (type = 'U')))
DROP TABLE [dbo].aspnet_Applications
GO


