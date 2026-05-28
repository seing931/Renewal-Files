GO
SET IDENTITY_INSERT [dbo].[SITRenMenuInfo] ON 
GO
INSERT [dbo].[SITRenMenuInfo] ([MenuId], [ParentMenuId], [PageName], [MenuName], [IconName]) VALUES (1, 0, N'', N'Motor', N'oi oi-menu')
GO
INSERT [dbo].[SITRenMenuInfo] ([MenuId], [ParentMenuId], [PageName], [MenuName], [IconName]) VALUES (2, 1, N'motorexport', N'Export Cover Note', N'oi oi-document')
GO
SET IDENTITY_INSERT [dbo].[SITRenMenuInfo] OFF
GO
