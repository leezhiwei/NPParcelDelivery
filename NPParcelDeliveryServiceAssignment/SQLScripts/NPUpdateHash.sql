-- Team A, Update Database
-- Update database, alter structure and add a new col, OriginalPassword for reference.
-- Use MSSQL, Hashing function to hash salt, add input + salt and hash, then base64 it.
USE NPCS;
DROP FUNCTION IF EXISTS dbo.SHA512Hash;
GO
CREATE FUNCTION dbo.SHA512Hash(@Input VARCHAR(MAX))
RETURNS VARBINARY(64)
AS
BEGIN
    DECLARE @Hash VARBINARY(64);
    DECLARE @Salt VARCHAR(MAX);

    SET @Salt = HASHBYTES('SHA2_512','ThisIsASaltToMakePassHashesNotSoEasyToCrack');
    SET @Hash = HASHBYTES('SHA2_512', @Input + @Salt);

    RETURN @Hash;
END;
GO
DROP FUNCTION IF EXISTS dbo.uFnStringToBase64;  
GO
CREATE FUNCTION [dbo].[uFnStringToBase64]
(
    @InputString VARCHAR(MAX)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    RETURN (
		SELECT
			CAST(N'' AS XML).value(
                  'xs:base64Binary(xs:hexBinary(sql:column("bin")))'
                , 'VARCHAR(MAX)'
            )
        FROM (
            SELECT CAST(@InputString AS VARBINARY(MAX)) AS bin
        ) AS RetVal
    )
END;
GO

BEGIN TRANSACTION
ALTER TABLE Member ADD OriginalPassword VARCHAR(25);
GO
UPDATE Member SET OriginalPassword = Password;
GO
ALTER TABLE Member ALTER COLUMN Password VARCHAR(500) NOT NULL;
UPDATE Member SET Password = dbo.uFnStringToBase64(CAST(dbo.SHA512Hash(Password) AS VARCHAR(MAX)));
SELECT * FROM Member;
COMMIT;

BEGIN TRANSACTION
ALTER TABLE Staff ADD OriginalPassword VARCHAR(25);
GO
UPDATE Staff SET OriginalPassword = Password;
GO
ALTER TABLE Staff ALTER COLUMN Password VARCHAR(500) NOT NULL;
UPDATE Staff SET Password = dbo.uFnStringToBase64(CAST(dbo.SHA512Hash(Password) AS VARCHAR(MAX)));
SELECT * FROM Staff;
COMMIT;