/* NOTE - THE ETL Script that imports users uses the Northwind database. These tables are used by the other scripts and the microservice */

/****** Object:  Table [dbo].[NotificationEmail]    Script Date: 10/19/2015 12:41:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[NotificationEmail](
	[UserId] [varchar](100) NOT NULL,
	[Email] [varchar](200) NOT NULL,
 CONSTRAINT [PK_NotificationEmail] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[NotificationSms]    Script Date: 10/19/2015 12:41:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[NotificationSms](
	[UserId] [varchar](100) NOT NULL,
	[PhoneNumber] [varchar](20) NOT NULL,
 CONSTRAINT [PK_NotificationSms] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[Repository]    Script Date: 10/19/2015 12:41:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Repository](
	[RepositoryID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[FullName] [varchar](100) NOT NULL,
	[CloneUrl] [varchar](200) NOT NULL,
 CONSTRAINT [PK_Repository] PRIMARY KEY CLUSTERED 
(
	[RepositoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[WeatherStation]    Script Date: 10/19/2015 12:42:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[WeatherStation](
	[WeatherStationID] [int] IDENTITY(1,1) NOT NULL,
	[StationID] [varchar](100) NOT NULL,
	[State] [char](2) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[XmlFeedUrl] [varchar](200) NOT NULL,
 CONSTRAINT [PK_WeatherStation] PRIMARY KEY CLUSTERED 
(
	[WeatherStationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


