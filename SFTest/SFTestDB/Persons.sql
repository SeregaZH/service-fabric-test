CREATE TABLE [dbo].[Persons]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [FirstName] NCHAR(50) NOT NULL, 
    [LastName] NCHAR(50) NOT NULL, 
    [FullName] NCHAR(110) NOT NULL, 
    [BirthDate] DATE NOT NULL
)
