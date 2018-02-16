USE [neww]
GO

/****** Object:  Table [dbo].[UsersDB]    Script Date: 2/16/2018 5:40:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UsersDB](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[MiddleName] [varchar](50) NULL,
	[LastName] [varchar](50) NOT NULL,
	[EmailID] [varchar](254) NOT NULL,
	[DateofBirth] [date] NULL,
	[Password] [nvarchar](max) NOT NULL,
	[isEmailVerified] [bit] NOT NULL,
	[ActivationCode] [uniqueidentifier] NOT NULL,
	[ResetPasswordCode] [nvarchar](100) NULL,
 CONSTRAINT [PK_UsersDB] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

