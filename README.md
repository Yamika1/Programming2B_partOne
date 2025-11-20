The app includes a friendly home screen with a side menu for simplicity. The app enables users to rate and review builders, access monthly claims, and check payments. Under the Payment section, 
users can verify transactions to check that all is fair and square. The Registration section enables users to add new profiles and access their information. New claims may be submitted and tracked 
how they are moving along in the Submit Claims section. The system aims to be easy to use and straightforward helping users handle contracts, claims, and payments more . 

Sql Server management script:

USE MASTER;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MonthlyContractSystemDB')
BEGIN

ALTER DATABASE MonthlyContractSystemDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE 
DROP DATABASE MonthlyContractSystemDB

END
GO

CREATE DATABASE MonthlyContractSystemDB
GO 

USE MonthlyContractSystemDB;
GO

DROP TABLE IF EXISTS Roles ;
CREATE TABLE Roles (
RoleId INT PRIMARY KEY IDENTITY(1,1),
RoleName VARCHAR(50) NOT NULL UNIQUE,
Description VARCHAR(255) NULL
);

CREATE TABLE Users (
UserId INT PRIMARY KEY IDENTITY(1,1),
UserName VARCHAR(100) NOT NULL UNIQUE,
PasswordHash VARCHAR(255) NOT NULL,
Email VARCHAR(255) NOT NULL UNIQUE,
FullName VARCHAR(255) NOT NULL,
IsActive BIT DEFAULT 1,
CreatedDate DATETIME DEFAULT GETDATE()
)

CREATE TABLE UserRoles(
UserRoleId INT PRIMARY KEY IDENTITY(1,1),
UserId INT NOT NULL,
RoleId INT NOT NULL,
FOREIGN KEY (UserId) REFERENCES Users (UserId) ON DELETE CASCADE,
FOREIGN KEY (RoleId) REFERENCES Roles (RoleId) ON DELETE CASCADE,
);

CREATE TABLE Claim(
Id INT PRIMARY KEY IDENTITY(1,1),
ClaimName VARCHAR(255) NOT NULL,
ClaimType VARCHAR(255) NOT NULL,
ClaimMonth VARCHAR(255) NULL,
HoursWorked INT NOT NULL,
SubmittedDate DATETIME DEFAULT GETDATE(),
Status VARCHAR(50) DEFAULT 'Pending',
ReviewedBy VARCHAR(255) NULL,
ReviewedDate DATETIME NULL


);

CREATE TABLE ClaimReviews (
Id INT PRIMARY KEY IDENTITY(1,1),
ClaimId INT NOT NULL,
ReviewerId INT NOT NULL,
ReviewedDate DATETIME NOT NULL,
Decision VARCHAR(50) NOT NULL,
ReviewerName VARCHAR(50) NOT NULL,
ReviewerRole VARCHAR(50) NOT NULL,
Comments VARCHAR(MAX),
FOREIGN KEY (Id) REFERENCES Claim(Id),
FOREIGN KEY (ReviewerId) REFERENCES Users(UserId)
);
CREATE TABLE UploadedDocument (
Id INT PRIMARY KEY IDENTITY (1,1),
BookId INT NOT NULL,
FileName VARCHAR(255) NOT NULL,
FilePath VARCHAR(255) NOT NULL,
FileSize BIGINT DEFAULT 1,
UploadedDate DATETIME DEFAULT GETDATE(),
FOREIGN KEY (Id) REFERENCES Claim(Id)
);


CREATE TABLE Sessions (
SessionId VARCHAR(255) PRIMARY KEY,
UserId INT NOT NULL,
CreatedDate DATETIME DEFAULT GETDATE(),
ExpiryDate DATETIME NOT NULL,
IsActive BIT DEFAULT 1,
IPAddress VARCHAR(50) NULL,
UserAgent VARCHAR(500) NULL,
FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE HR(
Id INT PRIMARY KEY IDENTITY(1,1),
UserName VARCHAR(255) NOT NULL,
FullName VARCHAR(255) NOT NULL,
Email VARCHAR(255) NULL,
ContactNumber INT  NULL,
hourlyRate VARCHAR(255) NULL,

);

-- Insert Roles
INSERT INTO Roles(RoleName, Description) VALUES
('Super User', 'Can create users and approve and reject users'),
('Admin', 'Can approve/reject claims'),
('Lecturer' , 'Can submit claims'),
('Manager', 'can view verified claims');

-- password for admin : Admin@123
-- password for super user: User@123
-- password for lecturer : Lecturer@123
-- password for manager : Manager@123

INSERT INTO Users (UserName, PasswordHash, Email, FullName) VALUES
('super user', 'b6607408aa7bdf331ba73953ea3d9747b5012ea9ea1be041fe7bb4ff8451867e', 'superAdmin@monthlyContractsystem.com', 'Super User'),
('admin', 'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7', 'admin@monthlyContractsystem.com', 'Admin'),
('lecturer', '88d11e52e9ce89aa80722a18a2a9238ff998ec7159945f3219bfc07e2c6e59f8', 'lecturer@monthlyContractsystem.com', ' Lecturer '),
('manager' ,'$2y$12$da7WHELeE1BdvnM/Irh0suSJcJ62ZUAB1b10u9NblZqc5XHbUOsei' , 'manager@monthlyContractsystem.com', ' Manager ')
INSERT INTO UserRoles(UserId, RoleId) VALUES
(1,1),
(2,2),
(3,3),
(4,4);  

PRINT 'Database setup completed';
PRINT 'Default Logins: super user  (Password: User@123)';
PRINT 'Default Logins: admin  (Password: Admin@123)';
PRINT 'Default Logins: lecturer  (Password: Lecturer@123)';
PRINT 'Default Logins: manager  (Password: Manager@123)';
GO
