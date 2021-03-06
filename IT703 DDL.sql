﻿/*
Created: 3/10/2016
Modified: 17/10/2016
Model: Microsoft SQL Server 2014
Database: MS SQL Server 2014
*/


-- Create tables section -------------------------------------------------
use Hotel
select * from INFORMATION_SCHEMA.TABLES
-- Table RoomTypeID

CREATE TABLE [RoomType]
(
 [RoomTypeID] Int IDENTITY(1,1) NOT NULL,
 [type] Varchar(50) NOT NULL,
 [occupants] Int NOT NULL,
 [price] Numeric(6,2) NOT NULL
)
go

-- Add keys for table RoomTypeID

ALTER TABLE [RoomType] ADD CONSTRAINT [RoomType_PK] PRIMARY KEY ([RoomTypeID])
go

-- Insert room type
Insert into [RoomType]([type],occupants,price)
values('Single Room',2,150),('Double Room',4,250),('Superior Room',2,400);
Select * from RoomType

-- Table Room
Drop table Room
CREATE TABLE [Room]
(
 [RoomID] Int IDENTITY(1,1) NOT NULL,
 [roomNumber] Varchar(50) NOT NULL,
 [status] Int NOT NULL,
 [RoomTypeID] Int NOT NULL
)
go

-- Add keys for table Room

ALTER TABLE [Room] ADD CONSTRAINT [Room_PK] PRIMARY KEY ([RoomID])
go
--Insert Room
--1:vacant clean, 2:vacant dirty, 3:occupied clean, 4:occupied service, 5:maintenance
delete from Room
Insert into Room(roomNumber,[status],RoomTypeID)
values('1.01',1,1),('1.02',2,1),
	('2.01',1,2),('2.02',2,2),
	('3.01',1,3),('3.02',2,3)
Select r.RoomID,r.roomNumber,r.status,t.type from Room r, RoomType t where r.RoomTypeID=t.RoomTypeID

update Room set status=1 where status =3
Select * from Room
Select COUNT(*) from Room Where status in (1,2)
Group By status

Select * from Room r, RoomType t where r.RoomTypeID=t.RoomTypeID

-- Table Customer
Drop table Customer
CREATE TABLE [Customer]
(
 [CustomerID] Int IDENTITY(1,1) NOT NULL,
 [firstName] Varchar(50) NOT NULL,
 [lastName] Varchar(50) NOT NULL,
 [personIdentity] Varchar(50) NOT NULL Unique,
 [contactNumber] Varchar(20) NULL,
 [email] Varchar(50) NULL,
 [address] Varchar(50) NULL,
 [city] Varchar(50) NULL,
 [country] Varchar(50) NULL,
 [OrganisationID] Int NULL
)
go

Delete from Customer
Select * from Customer

-- Add keys for table Customer

ALTER TABLE [Customer] ADD CONSTRAINT [Customer_PK] PRIMARY KEY ([CustomerID])
go

-- Table BookingDetail
Drop Table BookingDetail
CREATE TABLE [BookingDetail]
(
 [BookingDetailID] Int IDENTITY(1,1) NOT NULL,
 [bookingReference] Varchar(50) NULL,
 [bookingMethod] Varchar(20) NOT NULL,
 [CardDetailID] Int,
 [date] Date NOT NULL,
 [deposit] Numeric(6,2) NULL,
 [checkin] Date NOT NULL,
 [checkout] Date NOT NULL,
 [CustomerID] Int NOT NULL,
 [RoomID] Int NOT NULL,
 [CarParkID] Int NULL,
 [EmployeeID] Int NULL
)
go
-- Add keys for table BookingDetail

ALTER TABLE [BookingDetail] ADD CONSTRAINT [BookingDetail_PK] PRIMARY KEY ([BookingDetailID])
go

Delete from BookingDetail
Select b.*,c.*,r.*,t.type,t.price,t.occupants from BookingDetail b, Customer c, Room r, RoomType t where b.CustomerID=c.CustomerID and b.RoomID=r.RoomID and r.RoomTypeID=t.RoomTypeID


-- Table CarPark

CREATE TABLE [CarPark]
(
 [CarParkID] Int IDENTITY(1,1) NOT NULL,
 [parkNumber] Varchar(10) NOT NULL
)
go
-- Add keys for table CarPark

ALTER TABLE [CarPark] ADD CONSTRAINT [CarPark_PK] PRIMARY KEY ([CarParkID])
go

-- Table CardDetail
Drop table CardDetail
CREATE TABLE [CardDetail]
(
 [CardDetailID] Int IDENTITY(1,1) NOT NULL,
 [cardType] Varchar(10) NOT NULL,
 [cardNumber] Varchar(20) NOT NULL,
 [nameHolder] Varchar(50) NOT NULL,
 [PaymentID] Int NULL
)
go
-- Add keys for table CardDetail
ALTER TABLE [CardDetail] ADD CONSTRAINT [CardDetail_PK] PRIMARY KEY ([CardDetailID])
go

Delete From CardDetail
Select * from CardDetail

-- Table Note

CREATE TABLE [Note]
(
 [NoteID] Int IDENTITY(1,1) NOT NULL,
 [Description] Text NULL,
 [Date] Date NOT NULL,
 [CustomerID] Int NOT NULL
)
go
-- Add keys for table Note

ALTER TABLE [Note] ADD CONSTRAINT [Note_PK] PRIMARY KEY ([NoteID])
go

-- Table Organisation
Drop Table Organisation
CREATE TABLE [Organisation]
(
 [OrganisationID] Int IDENTITY(1,1) NOT NULL,
 [name] Varchar(50) NOT NULL,
 [contactNumber] Varchar(20) NULL,
 [email] Varchar(1) NULL,
 [address] Varchar(50) NULL,
 [city] Varchar(50) NULL,
 [country] Char(50) NULL,
 [type] Char(10) NOT NULL
)
go

-- Add keys for table Organisation

ALTER TABLE [Organisation] ADD CONSTRAINT [Organisation_PK] PRIMARY KEY ([OrganisationID])
go

-- Table Employee
drop table Employee
CREATE TABLE [Employee]
(
 [EmployeeID] Int IDENTITY(1,1) NOT NULL,
 [firstName] Varchar(50) NOT NULL,
 [lastName] Varchar(50) NOT NULL,
 [personIdentity] varchar(20) Not Null,
 [contact] varchar(20) Not NULL,
 [address] varchar(50) Not Null,
 [AccountID] Int NULL
)
go
-- Add keys for table Employee

ALTER TABLE [Employee] ADD CONSTRAINT [Employee_PK] PRIMARY KEY ([EmployeeID])
go

Insert Into Employee(firstName,lastName, personIdentity, contact,address,AccountID)
Values('Viet','Nguyen', 'AT456789', '02102990073','253 Spey Street, Invercargill',1)

select * from Employee

-- Table Account
Drop table Account
CREATE TABLE [Account]
(
 [AccountID] Int IDENTITY(1,1) NOT NULL,
 [roleName] Varchar(50) NOT NULL,
 [account] Varchar(50) NOT NULL,
 [pass] Varchar(8) NOT NULL
)
go

-- Add keys for table Role
ALTER TABLE [Account] ADD CONSTRAINT [Account_PK] PRIMARY KEY ([AccountID])
go

Delete from Account
Insert Into Account(account,roleName,pass)
Values('hviet','Manager','123456')

Select * from Account

Select e.EmployeeID, e.firstName, e.lastName, a.roleName from Account a, Employee e Where a.account='NNguyen9688' and a.pass='N2016' and a.AccountID=e.AccountID

-- Table Invoice
Drop table Invoice
CREATE TABLE [Invoice]
(
 [InvoiceID] Int IDENTITY(1,1) NOT NULL,
 [date] Date NOT NULL,
 [total] Numeric(6,2) NOT NULL,
 [BookingReference] varchar(50) NOT NULL
)
go
-- Add keys for table Invoice

ALTER TABLE [Invoice] ADD CONSTRAINT [Invoice_PK] PRIMARY KEY ([InvoiceID])
go

Select * From Invoice
-- Table RoomService

CREATE TABLE [RoomService]
(
 [RoomServiceID] Int IDENTITY(1,1) NOT NULL,
 [date] Date NOT NULL,
 [RoomID] Int NOT NULL,
 [EmployeeID] Int NOT NULL
)
go
-- Add keys for table RoomService

ALTER TABLE [RoomService] ADD CONSTRAINT [RoomService_PK] PRIMARY KEY ([RoomServiceID])
go

-- Table Payment

CREATE TABLE [Payment]
(
 [PaymentID] Int IDENTITY(1,1) NOT NULL,
 [date] Date NOT NULL,
 [paymentMethod] Varchar(50) NOT NULL,
 [BookingDetailID] Int NOT NULL
)
go
-- Add keys for table Payment

ALTER TABLE [Payment] ADD CONSTRAINT [Payment_PK] PRIMARY KEY ([PaymentID])
go



