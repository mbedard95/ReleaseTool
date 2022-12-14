/*
   Monday, July 18, 20227:10:56 PM
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
DROP TABLE dbo.Users
CREATE TABLE dbo.Users
	(
	UserId UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
	EmailAddress varchar(128) NOT NULL,
	Password varchar(128) NOT NULL,
	FirstName varchar(50) NOT NULL,
	LastName varchar(100) NOT NULL,
	UserStatus int NOT NULL,
	Created datetime NOT NULL,
	UserProfile int NOT NULL,
	IsActiveUser bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Users SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
