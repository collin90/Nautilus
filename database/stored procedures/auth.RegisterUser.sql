CREATE OR ALTER PROCEDURE auth.RegisterUser
    @UserName NVARCHAR(255),
    @Email NVARCHAR(255),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if username already exists
    IF EXISTS (SELECT 1 FROM auth.Users WHERE UserName = @UserName)
    BEGIN
        RAISERROR ('Username already exists.', 16, 1);
        RETURN;
    END

    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM auth.Users WHERE Email = @Email)
    BEGIN
        RAISERROR ('Email already exists.', 16, 1);
        RETURN;
    END

    -- Insert new user
    INSERT INTO auth.Users (
        UserName,
        Email,
        PasswordHash,
        PasswordSalt
    )
    VALUES (
        @UserName,
        @Email,
        @PasswordHash,
        @PasswordSalt
    );

    -- Optional: return the new user’s ID
    SELECT Id
    FROM auth.Users
    WHERE UserName = @UserName;
END
GO
