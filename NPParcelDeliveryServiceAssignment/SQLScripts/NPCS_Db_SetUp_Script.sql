/*======================================================*/
/*  Created in April 2023                               */
/*  WEB 2023 April Semester				                */
/*  Diploma in IT                                       */
/*                                                      */
/*  Database Script for setting up the database         */
/*  required for WEB Assignment.                        */
/*  Updates: 31 May 2023                                */
/*  (Change Status for Parcel 2 and 11 from '1' to '2') */
/*======================================================*/

Create Database NPCS
GO

Use NPCS
GO

/***************************************************************/
/***           Delete tables before creating                 ***/
/***************************************************************/

/* Table: dbo.CashVoucher*/
if exists (select * from sysobjects 
  where id = object_id('dbo.CashVoucher') and sysstat & 0xf = 3)
  drop table dbo.CashVoucher
GO

/* Table: dbo.FeedbackEnquiry */
if exists (select * from sysobjects 
  where id = object_id('dbo.FeedbackEnquiry') and sysstat & 0xf = 3)
  drop table dbo.FeedbackEnquiry
GO

/* Table: dbo.DeliveryFailure */
if exists (select * from sysobjects 
  where id = object_id('dbo.DeliveryFailure') and sysstat & 0xf = 3)
  drop table dbo.DeliveryFailure
GO

/* Table: dbo.DeliveryHistory  */
if exists (select * from sysobjects 
  where id = object_id('dbo.DeliveryHistory') and sysstat & 0xf = 3)
  drop table dbo.DeliveryHistory
GO

/* Table: dbo.PaymentTransaction */
if exists (select * from sysobjects 
  where id = object_id('dbo.PaymentTransaction') and sysstat & 0xf = 3)
  drop table dbo.PaymentTransaction
GO

/* Table: dbo.Parcel */
if exists (select * from sysobjects 
  where id = object_id('dbo.Parcel') and sysstat & 0xf = 3)
  drop table dbo.Parcel
GO

/* Table: dbo.ShippingRate */
if exists (select * from sysobjects 
  where id = object_id('dbo.ShippingRate') and sysstat & 0xf = 3)
  drop table dbo.ShippingRate
GO

/* Table: dbo.Member */
if exists (select * from sysobjects 
  where id = object_id('dbo.Member') and sysstat & 0xf = 3)
  drop table dbo.Member
GO

/* Table: dbo.Staff */
if exists (select * from sysobjects 
  where id = object_id('dbo.Staff') and sysstat & 0xf = 3)
  drop table dbo.Staff
GO

/***************************************************************/
/***                     Creating tables                     ***/
/***************************************************************/

/* Table: dbo.Staff */
CREATE TABLE dbo.Staff
(
  StaffID 			int IDENTITY (1,1),
  StaffName			varchar(50) 	NOT NULL,
  LoginID			varchar(20)  	NOT NULL,
  [Password]		varchar(25)  	NOT NULL,
  Appointment		varchar(50)  	NULL,
  OfficeTelNo		varchar(20)  	NULL,
  Location		    varchar(50)  	NULL,
  CONSTRAINT PK_Staff PRIMARY KEY NONCLUSTERED (StaffID)
)
GO

/* Table: dbo.Member */
CREATE TABLE dbo.Member
(
  MemberID 			int IDENTITY (1,1),
  [Name]			varchar(50) 	NOT NULL,
  Salutation		varchar(5)  	NOT NULL,
  TelNo		        varchar(20)  	NOT NULL,
  EmailAddr		    varchar(50)  	NOT NULL,
  [Password]		varchar(25)  	NOT NULL,
  BirthDate			datetime  		NULL,
  City		    	varchar(50)  	NULL,
  Country			varchar(50)  	NOT NULL,
  CONSTRAINT PK_Member PRIMARY KEY NONCLUSTERED (MemberID)
)
GO

/* Table: dbo.ShippingRate */
CREATE TABLE dbo.ShippingRate
(
  ShippingRateID 	int IDENTITY (1,1),
  FromCity			varchar(50) 	NOT NULL,
  FromCountry		varchar(50)  	NOT NULL,
  ToCity			varchar(50) 	NOT NULL,
  ToCountry			varchar(50)  	NOT NULL,
  ShippingRate		money  			NOT NULL DEFAULT (0),
  Currency			char(3)  		NOT NULL DEFAULT ('SGD'),
  TransitTime		int  			NOT NULL DEFAULT (1),
  LastUpdatedBy		int  			NOT NULL,
  CONSTRAINT PK_ShippingRate PRIMARY KEY NONCLUSTERED (ShippingRateID),
  CONSTRAINT FK_ShippingRate_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) 
  REFERENCES dbo.Staff(StaffID)
)
GO

/* Table: dbo.Parcel */
CREATE TABLE dbo.Parcel
(
  ParcelID 				int IDENTITY (1,1),
  ItemDescription		varchar(255) 	NULL,
  SenderName			varchar(50)	NOT NULL,
  SenderTelNo			varchar(20)	NOT NULL,
  ReceiverName			varchar(50)	NOT NULL,
  ReceiverTelNo			varchar(20)	NOT NULL,
  DeliveryAddress		varchar(255)	NOT NULL,
  FromCity				varchar(50) 	NOT NULL,
  FromCountry			varchar(50)  	NOT NULL,
  ToCity				varchar(50) 	NOT NULL,
  ToCountry				varchar(50)  	NOT NULL,
  ParcelWeight			float			NOT NULL DEFAULT (0.0),
  DeliveryCharge		money  			NOT NULL DEFAULT (0),
  Currency				char(3)  		NOT NULL DEFAULT ('SGD'),
  TargetDeliveryDate	datetime		NULL,
  DeliveryStatus		char(1)  		NOT NULL DEFAULT ('0'),
  DeliveryManID			int  			NULL,
  CONSTRAINT PK_Parcel PRIMARY KEY NONCLUSTERED (ParcelID),
  CONSTRAINT FK_Parcel_DeliveryManID FOREIGN KEY (DeliveryManID) 
  REFERENCES dbo.Staff(StaffID)
)
GO

/* Table: dbo.PaymentTransaction */
CREATE TABLE dbo.PaymentTransaction
(
 TransactionID		int IDENTITY (1,1),
 ParcelID 			int		NOT NULL,
 AmtTran			money  		NOT NULL DEFAULT (0),
 Currency			char(3)  	NOT NULL DEFAULT ('SGD'),
 TranType			char(4)  	NOT NULL DEFAULT ('CASH'),
 TranDate			datetime	NOT NULL DEFAULT (getdate()),
 CONSTRAINT PK_PaymentTransaction PRIMARY KEY NONCLUSTERED (TransactionID),
 CONSTRAINT FK_PaymentTransaction_ParcelID  FOREIGN KEY (ParcelID) 
 REFERENCES dbo.Parcel(ParcelID)
)
GO

/* Table: dbo.DeliveryHistory */
CREATE TABLE dbo.DeliveryHistory
(
  RecordID 			int IDENTITY (1,1),
  ParcelID 			int				NOT NULL,
  [Description]		varchar(255)	NOT NULL,
  CONSTRAINT PK_DeliveryHistory PRIMARY KEY NONCLUSTERED (RecordID),
  CONSTRAINT FK_DeliveryHistory_ParcelID  FOREIGN KEY (ParcelID) 
  REFERENCES dbo.Parcel(ParcelID)
)
GO

/* Table: dbo.DeliveryFailure */
CREATE TABLE dbo.DeliveryFailure
(
  ReportID				int IDENTITY (1,1),
  ParcelID 				int				NOT NULL,
  DeliveryManID			int  			NOT NULL,
  FailureType			char(1)  		NOT NULL,
  [Description]			varchar(255)	NOT NULL,
  StationMgrID			int  			NULL,
  FollowUpAction		varchar(255)	NULL,
  DateCreated			datetime		NOT NULL DEFAULT (getdate()), 
  CONSTRAINT PK_DeliveryFailure PRIMARY KEY NONCLUSTERED (ReportID),
  CONSTRAINT FK_DeliveryFailure_ParcelID  FOREIGN KEY (ParcelID) 
  REFERENCES dbo.Parcel(ParcelID),
  CONSTRAINT FK_DeliveryFailure_DeliveryManID FOREIGN KEY (DeliveryManID) 
  REFERENCES dbo.Staff(StaffID),
  CONSTRAINT FK_DeliveryFailure_StationMgrID FOREIGN KEY (StationMgrID) 
  REFERENCES dbo.Staff(StaffID)
)
GO

/* Table: dbo.FeedbackEnquiry  */
CREATE TABLE dbo.FeedbackEnquiry 
(
  FeedbackEnquiryID		int IDENTITY (1,1),
  MemberID				int				NOT NULL,
  Content				varchar(255)	NOT NULL,
  DateTimePosted		datetime		NOT NULL DEFAULT (getdate()),
  StaffID				int				NULL,
  Response				varchar(255)	NULL,  
  [Status]				char(1)  		NOT NULL DEFAULT ('0'), 
  CONSTRAINT PK_FeedbackEnquiry PRIMARY KEY NONCLUSTERED (FeedbackEnquiryID),
  CONSTRAINT FK_FeedbackEnquiry_MemberID FOREIGN KEY (MemberID) 
  REFERENCES dbo.Member(MemberID),
  CONSTRAINT FK_FeedbackEnquiry_StaffID FOREIGN KEY (StaffID) 
  REFERENCES dbo.Staff(StaffID)
)
GO

/* Table: dbo.CashVoucher */
CREATE TABLE dbo.CashVoucher
(
 CashVoucherID			int IDENTITY (1,1),
 StaffID				int			NOT NULL,
 Amount					money  		NOT NULL DEFAULT (0),
 Currency				char(3)  	NOT NULL DEFAULT ('SGD'),
 IssuingCode			char(1)  	NOT NULL,
 ReceiverName			varchar(50)	NOT NULL,
 ReceiverTelNo			varchar(20)	NOT NULL,
 DateTimeIssued			datetime	NOT NULL DEFAULT (getdate()),
 [Status]				char(1)  	NOT NULL DEFAULT ('0'), 
 CONSTRAINT PK_CashVoucher PRIMARY KEY NONCLUSTERED (CashVoucherID),
 CONSTRAINT FK_CashVoucher_StaffID  FOREIGN KEY (StaffID) 
 REFERENCES dbo.Staff(StaffID)
)
GO

SET IDENTITY_INSERT [dbo].[Staff] ON
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (1, 'Samatha Sun', 'FrontOffSG1', 'passFront', 'Front Office Staff', '+6564561234', 'Singapore')
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (2, 'Benjamin Bean', 'StationMgrSG', 'passStation', 'Station Manager', '+6564561235', 'Singapore')
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (3, 'Pinky Pander', 'AdminMgrSG', 'passAdmin', 'Admin Manager', '+6564561236', 'Singapore')
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (4, 'Edward Lee', 'EWL4', 'p@55EWL', 'Delivery Man', '+6591234567', 'Singapore')
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (5, 'Ali Imran', 'ALI5', 'p@55ALI', 'Delivery Man', '+6591234568', 'Singapore')
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (6, 'K Kannan', 'KAN6', 'p@55KAN', 'Delivery Man', '+6591234569', 'Singapore')
INSERT [dbo].[Staff] ([StaffID], [StaffName], [LoginID], [Password], [Appointment], [OfficeTelNo], [Location]) VALUES (7, 'Chen Yi', 'CY7', 'p@55CY7', 'Delivery Man', '+8698765432101', 'China')
SET IDENTITY_INSERT [dbo].[Staff] OFF

SET IDENTITY_INSERT [dbo].[Member] ON 
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (1, 'Peter Ghim', 'Mr', '+6560000071', 'pg91@hotmail.com', 'pgP@55', '31-Aug-1971', 'Singapore', 'Singapore')
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (2, 'Xu Yazhi', 'Ms', '+6595000002', 'xyz@np.edu.sg', 'xyzP@55', '05-May-1995', 'Singapore', 'Singapore')
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (3, 'Fatimah Bte Ahmad', 'Ms', '+6592000003', 'fa92@yahoo.com', 'faP@55', '21-Jun-1982', 'Singapore', 'Singapore')
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (4, 'Adam Ang', 'Mr', '+6593000004', 'aa93@gmail.com', 'aaP@55', '24-Jul-1993', 'Singapore', 'Singapore')
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (5, 'Ang Boon Cheng', 'Mr', '+6580987654', 'abc@yahoo.com.sg', 'abcP@55', '17-Jun-1973', 'Singapore', 'Singapore')
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (6, 'Yang Yan', 'Ms', '+6590765423', 'yy8@yahoo.com.sg', 'yy8P@55', '09-Aug-1991', 'Singapore', 'Singapore')
INSERT [dbo].[Member] ([MemberID], [Name], [Salutation], [TelNo], [EmailAddr], [Password], [BirthDate], [City], [Country]) VALUES (7, 'S Subramaniam', 'Mr', '+6564532987', 'Subra7@yahoo.com.sg', 'ss7P@55', '18-Jul-1988', 'Singapore', 'Singapore')
SET IDENTITY_INSERT [dbo].[Member] OFF

SET IDENTITY_INSERT [dbo].[ShippingRate] ON 
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (1, 'Singapore', 'Singapore', 'Singapore', 'Singapore', 1.50, 'SGD', 1, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (2, 'Singapore', 'Singapore', 'Kuala Lumpur', 'Malaysia', 2.50, 'SGD', 2, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (3, 'Singapore', 'Singapore', 'Hong Kong', 'China', 8.35, 'SGD', 3, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (4, 'Singapore', 'Singapore', 'Beijing', 'China', 9.25, 'SGD', 3, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (5, 'Singapore', 'Singapore', 'Tokyo', 'Japan', 10.25, 'SGD', 3, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (6, 'Singapore', 'Singapore', 'Sdyney', 'Australia', 12.20, 'SGD', 4, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (7, 'Singapore', 'Singapore', 'New York', 'USA', 20.70, 'SGD', 5, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (8, 'Singapore', 'Singapore', 'San Francisco', 'USA', 18.50, 'SGD', 4, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (9, 'Singapore', 'Singapore', 'Paris', 'France', 17.85, 'SGD', 5, 3)
INSERT [dbo].[ShippingRate] ([ShippingRateID], [FromCity], [FromCountry], [ToCity], [ToCountry], [ShippingRate], [Currency], [TransitTime], [LastUpdatedBy]) VALUES (10, 'Singapore', 'Singapore', 'London', 'UK', 16.10, 'SGD', 5, 3)
SET IDENTITY_INSERT [dbo].[ShippingRate] OFF

SET IDENTITY_INSERT [dbo].[Parcel] ON 
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (1, 'Books', 'James Bond', '+14151234567', 'Peter Ghim', '+6560000071', 'Blk 1, #07-429, Toa Payoh Lorong 6, Singapore 320001', 'San Francisco', 'USA', 'Singapore', 'Singapore', 6.3, 88.00, 'USD', '03-Mar-2023', '3', 4)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (2, 'Porcelain Tiles', 'Tan Tang', '+6598000012', 'Lee Wei', '+6592000012', 'Blk 123, Clementi Ave 3, Singapore 590123', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 18.4, 28.00, 'SGD', '13-Mar-2023', '4', 4)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (3, 'Books', 'Peter Ghim', '+6560000071', 'Xu Yazhi', '+6595000002', 'Ngee Ann Polytechnic, 535 Clementi Road, Singapore 599489', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 6.3, 9.00, 'SGD', '4-Apr-2023', '3', 6)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (4, 'Clothings', 'Wang Fei', '+8612345678901', 'Xu Yazhi', '+6595000002', 'Ngee Ann Polytechnic, 535 Clementi Road, Singapore 599489', 'Beijing', 'China', 'Singapore', 'Singapore', 1.2, 53.00, 'CNY', '31-May-2023', '2', 7)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (5, 'Computer Notbooks', 'Xu Yazhi', '+6595000002', 'Wang Fei', '+8612345678901', '101, Pinggou Hutong, Beijing, China 102501', 'Singapore', 'Singapore', 'Beijing', 'China', 4.3, 40.00, 'SGD', '2-Jun-2023', '2', 5)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (6, 'Handphones and accessories, 4 sets', 'Fatimah Bte Ahmad', '+6592000003', 'Siti Hidayah', '+6031234567', '3, Jalan Bukit Bintang, Kuala Lumpur, Malaysia 74446', 'Singapore', 'Singapore', 'Kuala Lumpur', 'Malaysia', 2.2, 6.00, 'SGD', '4-Jun-2023', '2', 6)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (7, 'Perfumes', 'Adalene Lebeau', '+33109876543', 'Adam Ang', '+6593000004', 'Blk 100, #10-1001, Ang Mo Kio Ave 3, Singapore 560100', 'Paris', 'France', 'Singapore', 'Singapore', 0.85, 10.00, 'EUR', '10-Jun-2023', '1', 5)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (8, 'Clothings', 'Ang Boon Cheng', '+6580987654', 'Deng Eu Fang', '+6591000204', '21, Defu Lane 3, Singapore 550021', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 0.45, 5.00, 'SGD', '5-Jun-2023', '1', 5)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (9, 'Canned Foods', 'Ang Boon Cheng', '+6580987654', 'Goh Hai Ing', '+6583000107', '10, Gombak Drive, Singapore 650010', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 0.45, 5.00, 'SGD', '5-Jun-2023', '1', 5)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (10, 'TV set', 'Jang Jun', '+6581135790', 'Koh Limin', '+6594567812', 'Blk 110A, Pasir Ris Drive 1, Singapore 521101', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 8.67, 13.00, 'SGD', '5-Jun-2023', '1', 5)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (11, 'Books and Stationery', 'Mohammad Irwan', '+6599876512', 'P Muthu', '+6057654321', '13, Jalan Sultan Yusuf, Ipoh, Perak, Malaysia 30987', 'Singapore', 'Singapore', 'Kuala Lumpur', 'Malaysia', 3.54, 9.00, 'SGD', '6-Jun-2023', '2', 4)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (12, 'Notebook Computers, 2 sets', 'S Subramaniam', '+6564532987', 'Ang Boon Cheng', '+6580987654', '15, Amber Road, Singapore 151543', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 3.54, 5.00, 'SGD', '6-Jun-2023', '1', 4)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (13, 'Art Works', 'Lemon Long', '+6594567123', 'Mohammad Irwan', '+6599876512', '12, Jalan Eunos, Singapore 415912', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 6.21, 9.00, 'SGD', '7-Jun-2023', '1', 4)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (14, 'Contract Document', 'Ang Boon Cheng', '+6580987654', 'Li Na', '+8613254769801', '23, Luo Yu Road, Hong Kong, Hubei, China 430023', 'Singapore', 'Singapore', 'Hong Kong', 'China', 0.2, 5.00, 'SGD', '11-Jun-2023', '0', NULL)
INSERT [dbo].[Parcel] ([ParcelID], [ItemDescription], [SenderName], [SenderTelNo], [ReceiverName], [ReceiverTelNo], [DeliveryAddress], [FromCity], [FromCountry], [ToCity], [ToCountry], [ParcelWeight], [DeliveryCharge], [Currency], [TargetDeliveryDate], [DeliveryStatus], [DeliveryManID])
VALUES (15, 'Smart Watch', 'Zheng Zhi', '+6580765432', 'Yang Yan', '+6590765423', 'Blk 453C, #08-1234, Sembawang Way, Singapore 764533', 'Singapore', 'Singapore', 'Singapore', 'Singapore', 0.12, 5.00, 'SGD', '14-Jun-2023', '0', NULL)
SET IDENTITY_INSERT [dbo].[Parcel] OFF

SET IDENTITY_INSERT [dbo].[PaymentTransaction] ON 
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (1, 1, 88.00, 'USD', 'CASH', '28-Feb-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (2, 2, 28.00, 'SGD', 'CASH', '12-Mar-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (3, 3, 9.00, 'SGD', 'CASH', '3-Apr-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (4, 4, 53.00, 'CNY', 'CASH', '28-May-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (5, 5, 20.00, 'SGD', 'CASH', '29-May-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (6, 5, 20.00, 'SGD', 'VOUC', '29-May-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (7, 6, 6.00, 'SGD', 'CASH', '2-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (8, 7, 10.00, 'EUR', 'CASH', '4-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (9, 8, 5.00, 'SGD', 'CASH', '4-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (10, 9, 5.00, 'SGD', 'CASH', '4-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (11, 10, 3.00, 'SGD', 'CASH', '4-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (12, 10, 10.00, 'SGD', 'VOUC', '4-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (13, 11, 9.00, 'SGD', 'CASH', '4-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (14, 12, 5.00, 'SGD', 'CASH', '5-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (15, 13, 9.00, 'SGD', 'CASH', '6-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (16, 14, 5.00, 'SGD', 'CASH', '8-Jun-2023')
INSERT [dbo].[PaymentTransaction] ([TransactionID], [ParcelID], [AmtTran], [Currency], [TranType], [TranDate]) VALUES (17, 15, 5.00, 'SGD', 'CASH', '13-Jun-2023')
SET IDENTITY_INSERT [dbo].[PaymentTransaction] OFF

SET IDENTITY_INSERT [dbo].[DeliveryHistory] ON 
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (1, 1, 'Received parcel by FrontOffSF1 on 28 Feb 2023 10:10am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (2, 1, 'Received parcel by StationMgrUS on 29 Feb 2023 1:25pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (3, 1, 'Parcel delivered to airport by ABM1 on 1 Mar 2023 1:35pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (4, 1, 'Received parcel by StationMgrSG on 2 Mar 2023 2:15pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (5, 1, 'Parcel delivered successfully by EWL4 on 3 Mar 2023 11:30am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (6, 2, 'Received parcel by FrontOffSG1 on 12 Mar 2023 9:20am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (7, 2, 'Received parcel by StationMgrSG on 12 Mar 2023 2:05pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (8, 2, 'Follow up with sender for delivery failure, completed by StationMgrSG on 14 Mar 2023 10:25am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (9, 3, 'Received parcel by FrontOffSG1 on 3 Apr 2023 4:20pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (10, 3, 'Received parcel by StationMgrSG on 3 Apr 2023 7:25pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (11, 3, 'Parcel delivered successfully by KAN6 on 4 Mar 2023 1:45pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (12, 4, 'Received parcel by FrontOffCHN1 on 28 May 2023 8:30am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (13, 4, 'Received parcel by StationMgrCHN on 28 May 2023 7:15pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (14, 4, 'Parcel delivered to airport by CY7 on 29 May 2023 7:55am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (15, 5, 'Received parcel by FrontOffSG1 on 29 May 2023 9:15am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (16, 5, 'Received parcel by StationMgrSG on 29 Jun 2023 3:20pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (17, 6, 'Received parcel by FrontOffSG1 on 2 Jun 2023 8:05am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (18, 6, 'Received parcel by StationMgrSG on 2 Jun 2023 1:10pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (19, 6, 'Parcel delivered to airport by KAN6 on 3 Jun 2023 8:35am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (20, 7, 'Received parcel by FrontOffPAR4 on 4 Jun 2023 10:15am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (21, 8, 'Received parcel by FrontOffSG1 on 4 Jun 2023 11:05am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (22, 9, 'Received parcel by FrontOffSG1 on 4 Jun 2023 11:15am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (23, 8, 'Received parcel by StationMgrSG on 4 Jun 2023 2:15pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (24, 9, 'Received parcel by StationMgrSG on 4 Jun 2023 2:20pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (25, 10, 'Received parcel by FrontOffSG1 on 4 Jun 2023 3:05pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (26, 11, 'Received parcel by FrontOffSG1 on 4 Jun 2023 4:15pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (27, 7, 'Received parcel by StationMgrFRA on 4 Jun 2023 6:25pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (28, 10, 'Received parcel by StationMgrSG on 4 Jun 2023 7:30pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (29, 11, 'Received parcel by StationMgrSG on 4 Jun 2023 7:45pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (30, 12, 'Received parcel by FrontOffSG1 on 5 Jun 2023 11:25am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (31, 12, 'Received parcel by StationMgrSG on 5 Jun 2023 4:45pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (32, 7, 'Parcel delivered to airport by JCF11 on 5 Jun 2023 8:35pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (33, 13, 'Received parcel by FrontOffSG1 on 6 Jun 2023 9:35am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (34, 13, 'Received parcel by StationMgrSG on 6 Jun 2023 3:25pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (35, 7, 'Received parcel by StationMgrSG on 7 Jun 2023 1:10pm.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (36, 14, 'Received parcel by FrontOffSG1 on 8 Jun 2023 9:15am.')
INSERT [dbo].[DeliveryHistory] ([RecordID], [ParcelID], [Description]) VALUES (37, 15, 'Received parcel by FrontOffSG1 on 13 Jun 2023 10:20am.')
SET IDENTITY_INSERT [dbo].[DeliveryHistory] OFF

SET IDENTITY_INSERT [dbo].[DeliveryFailure] ON 
INSERT [dbo].[DeliveryFailure] ([ReportID], [ParcelID], [DeliveryManID], [FailureType], [Description], [StationMgrID], [FollowUpAction], [DateCreated]) 
VALUES (1, 2, 4, '3', '2 out of 10 pieces of porcelain tiles were broken due to vehicle collision in an road accident while driving along PIE.', 
        2, 'Contacted Sender on 14 Mar 2023 to send our apology, issued $20 cash voucher to the sender, who accepted our apology and satisfied with the compensation.', 
        '13-Mar-2023')
SET IDENTITY_INSERT [dbo].[DeliveryFailure] OFF

SET IDENTITY_INSERT [dbo].[FeedbackEnquiry] ON 
INSERT [dbo].[FeedbackEnquiry] ([FeedbackEnquiryID], [MemberID], [Content], [DateTimePosted], [StaffID], [Response], [Status]) 
VALUES (1, 1, 'May I know it will take how many days to send a parcel from Singapore to Shanghai?', '1-Mar-2023', 2, 'It will take 3 days.', '1')
INSERT [dbo].[FeedbackEnquiry] ([FeedbackEnquiryID], [MemberID], [Content], [DateTimePosted], [StaffID], [Response], [Status]) 
VALUES (2, 2, 'Delivery man Mr K Kannan has ben polite and considerate to customers.', '5-Mar-2023', NULL, NULL, '0')
SET IDENTITY_INSERT [dbo].[FeedbackEnquiry] OFF

SET IDENTITY_INSERT [dbo].[CashVoucher] ON 
INSERT [dbo].[CashVoucher] ([CashVoucherID], [StaffID], [Amount], [Currency], [IssuingCode], [ReceiverName], [ReceiverTelNo], [DateTimeIssued], [Status]) 
VALUES (1, 3, 10.00, 'SGD', '1', 'Peter Ghim', '+6560000071', '31-Aug-2022', '1')
INSERT [dbo].[CashVoucher] ([CashVoucherID], [StaffID], [Amount], [Currency], [IssuingCode], [ReceiverName], [ReceiverTelNo], [DateTimeIssued], [Status]) 
VALUES (2, 2, 20.00, 'SGD', '2', 'Tan Tang', '+6598000012', '14-Mar-2023', '1')
INSERT [dbo].[CashVoucher] ([CashVoucherID], [StaffID], [Amount], [Currency], [IssuingCode], [ReceiverName], [ReceiverTelNo], [DateTimeIssued], [Status]) 
VALUES (3, 3, 10.00, 'SGD', '1', 'Ang Boon Cheng', '+6580987654', '17-Jun-2023', '0')
INSERT [dbo].[CashVoucher] ([CashVoucherID], [StaffID], [Amount], [Currency], [IssuingCode], [ReceiverName], [ReceiverTelNo], [DateTimeIssued], [Status]) 
VALUES (4, 3, 10.00, 'SGD', '1', 'S Subramaniam', '+6564532987', '18-Jul-2023', '0')
SET IDENTITY_INSERT [dbo].[CashVoucher] OFF
