USE [store_app]
GO

-- Delete all records from tbl_customer
DELETE FROM tbl_customer;
GO

-- Reset identity seed to 0
DBCC CHECKIDENT ('tbl_customer', RESEED, 0);
GO

-- Verification query
SELECT COUNT(*) AS CustomerCount FROM tbl_customer;
GO
