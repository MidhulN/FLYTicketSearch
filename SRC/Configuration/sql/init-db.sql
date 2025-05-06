IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'flights')
BEGIN
    CREATE DATABASE [flights];
END
