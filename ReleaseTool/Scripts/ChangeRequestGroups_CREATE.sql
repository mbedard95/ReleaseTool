/*
   Monday, July 18, 20227:52:08 PM
   User: 
   Server: MSI
   Database: Capstone
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
--DROP TABLE dbo.ChangeRequestGroups
CREATE TABLE dbo.ChangeRequestGroups
	(
	ChangeRequestGroupId UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	ChangeRequestId UNIQUEIDENTIFIER NOT NULL,
	GroupId UNIQUEIDENTIFIER NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ChangeRequestGroups SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
