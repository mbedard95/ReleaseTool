/*
   Monday, July 18, 20227:25:21 PM
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
DROP TABLE dbo.ChangeRequests
CREATE TABLE dbo.ChangeRequests
	(
	ChangeRequestId UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	Title varchar(100) NOT NULL,
	Description varchar(MAX) NOT NULL,
	ReleaseSteps varchar(MAX) NOT NULL,
	RollbackProcedure varchar(MAX) NOT NULL,
	Created datetime NOT NULL,
	UserId UNIQUEIDENTIFIER NOT NULL,
	ChangeRequestStatus int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.ChangeRequests SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
