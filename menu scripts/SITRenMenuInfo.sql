SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SITRenMenuInfo](
	[MenuId] [int] IDENTITY(1,1) NOT NULL,
	[ParentMenuId] [int] NOT NULL,
	[PageName] [varchar](50) NOT NULL,
	[MenuName] [varchar](50) NOT NULL,
	[IconName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SITRenMenuInfo] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SITRenMenuInfo] ADD  DEFAULT ('') FOR [ParentMenuId]
GO

ALTER TABLE [dbo].[SITRenMenuInfo] ADD  DEFAULT ('') FOR [PageName]
GO

ALTER TABLE [dbo].[SITRenMenuInfo] ADD  DEFAULT ('') FOR [MenuName]
GO

ALTER TABLE [dbo].[SITRenMenuInfo] ADD  DEFAULT ('') FOR [IconName]
GO


