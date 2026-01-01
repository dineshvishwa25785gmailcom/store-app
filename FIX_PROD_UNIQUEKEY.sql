-- Fix UniqueKeyTracker for PROD prefix
USE [store_app]
GO

-- Check current max PROD key
SELECT MAX(CAST(SUBSTRING(UniqueKeyID, 5, LEN(UniqueKeyID)) AS INT)) AS MaxProdNumber
FROM Tbl_product
WHERE UniqueKeyID LIKE 'PROD%';
GO

-- Update UniqueKeyTracker to match (use LastNumber column)
UPDATE UniqueKeyTracker 
SET LastNumber = (
    SELECT ISNULL(MAX(CAST(SUBSTRING(UniqueKeyID, 5, LEN(UniqueKeyID)) AS INT)), 0) + 1
    FROM Tbl_product
    WHERE UniqueKeyID LIKE 'PROD%'
)
WHERE Prefix = 'PROD';
GO

-- Verify
SELECT * FROM UniqueKeyTracker WHERE Prefix = 'PROD';
GO
