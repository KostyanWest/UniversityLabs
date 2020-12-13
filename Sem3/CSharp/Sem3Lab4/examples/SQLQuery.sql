/* Создание хранимых процедур для получения таблиц */

USE [AdventureWorksLT2019]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspGetProducts]
    @count int
AS
    SELECT TOP (@count) [ProductID]
        ,[Name]
        ,[ProductNumber]
        ,[Color]
        ,[StandardCost]
        ,[ListPrice]
        ,[Size]
        ,[Weight]
        ,[ProductCategoryID]
        ,[ProductModelID]
        ,[SellStartDate]
        ,[SellEndDate]
        ,[DiscontinuedDate]
        ,[ThumbNailPhoto]
        ,[ThumbnailPhotoFileName]
        ,[rowguid]
        ,[ModifiedDate]
      FROM [AdventureWorksLT2019].[SalesLT].[Product] WITH (NOLOCK)
GO

CREATE PROCEDURE [dbo].[uspGetProductCategories]
    @count int
AS
	SELECT TOP (@count) [ProductCategoryID]
          ,[ParentProductCategoryID]
          ,[Name]
          ,[rowguid]
          ,[ModifiedDate]
      FROM [AdventureWorksLT2019].[SalesLT].[ProductCategory] WITH (NOLOCK)
GO

CREATE PROCEDURE [dbo].[uspGetProductModels]
    @count int
AS
	SELECT TOP (@count) [ProductModelID]
          ,[Name]
          ,[CatalogDescription]
          ,[rowguid]
          ,[ModifiedDate]
      FROM [AdventureWorksLT2019].[SalesLT].[ProductModel] WITH (NOLOCK)
GO

CREATE PROCEDURE [dbo].[uspGetProductModelProductDescriptions]
    @count int
AS
	SELECT TOP (@count) [ProductModelID]
          ,[ProductDescriptionID]
          ,[Culture]
          ,[rowguid]
          ,[ModifiedDate]
      FROM [AdventureWorksLT2019].[SalesLT].[ProductModelProductDescription] WITH (NOLOCK)
GO

CREATE PROCEDURE [dbo].[uspGetProductDescriptions]
    @count int
AS
	SELECT TOP (@count) [ProductDescriptionID]
          ,[Description]
          ,[rowguid]
          ,[ModifiedDate]
      FROM [AdventureWorksLT2019].[SalesLT].[ProductDescription] WITH (NOLOCK)
GO



/* Создание таблицы для хранения логов ошибок */

USE [sem3lab4db]
GO

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
CREATE TABLE [dbo].[ReportLog]
	(
	ExceptionID int NOT NULL IDENTITY (1, 1),
        ExceptionType nchar(50) NULL,
	Message nchar(100) NULL,
	TargetSite nchar(100) NULL,
	StackTrace nchar(100) NULL,
	InnerException int NULL,
	Date date NULL
	)  ON [PRIMARY]
GO
ALTER TABLE [dbo].[ReportLog] ADD CONSTRAINT
	PK_ReportLog PRIMARY KEY CLUSTERED 
	(
	ExceptionID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ReportLog] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
GO

/* Создание хранимой процедуры для вставки репорта */

CREATE PROCEDURE [dbo].[uspInsertReport]
    @ExceptionType nchar(50) NULL,
    @Message nchar(100) NULL,
    @TargetSite nchar(100) NULL,
    @StackTrace nchar(100) NULL,
    @InnerException int NULL,
    @Date date NULL
AS
	INSERT INTO [dbo].[ReportLog]
           ([ExceptionType]
           ,[Message]
           ,[TargetSite]
           ,[StackTrace]
           ,[InnerException]
           ,[Date])
      VALUES
           (@ExceptionType
           ,@Message
           ,@TargetSite
           ,@StackTrace
           ,@InnerException
           ,@Date)

	SELECT SCOPE_IDENTITY()
GO
