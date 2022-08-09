/*
   Monday, July 18, 20227:45:50 PM
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
CREATE TABLE dbo.Approvals
	(
	ApprovalId int NOT NULL IDENTITY (1, 1),
	UserId int NOT NULL,
	ChangeRequestId int NOT NULL,
	ApprovalStatus int NOT NULL,
	EmailAddress varchar(100) NOT NULL,
	Created datetime NOT NULL,
	ApprovedDate datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Approvals SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
