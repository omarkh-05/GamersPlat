IF OBJECT_ID(N'dbo.CustomerRequests', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.CustomerRequests
	(
		RequestId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_CustomerRequests PRIMARY KEY,
		CenterId int NOT NULL,
		PlayerId int NULL,
		Type nvarchar(100) NOT NULL,
		PlayerName nvarchar(150) NOT NULL,
		Subject nvarchar(200) NOT NULL,
		Message nvarchar(max) NOT NULL,
		Status nvarchar(50) NOT NULL CONSTRAINT DF_CustomerRequests_Status DEFAULT N'Pending',
		CreatedAt datetime2 NOT NULL CONSTRAINT DF_CustomerRequests_CreatedAt DEFAULT SYSUTCDATETIME(),
		CONSTRAINT FK_CustomerRequests_Centers FOREIGN KEY (CenterId) REFERENCES dbo.Centers(CenterId),
		CONSTRAINT FK_CustomerRequests_Users FOREIGN KEY (PlayerId) REFERENCES dbo.Users(UserId)
	);
	CREATE INDEX IX_CustomerRequests_CenterId_Status_CreatedAt ON dbo.CustomerRequests(CenterId, Status, CreatedAt);
END;

IF EXISTS
(
	SELECT 1
	FROM sys.key_constraints
	WHERE parent_object_id = OBJECT_ID(N'dbo.StaffRoles')
		AND name = N'UQ_StaffRoles_Name'
		AND type = N'UQ'
)
BEGIN
	ALTER TABLE dbo.StaffRoles DROP CONSTRAINT UQ_StaffRoles_Name;
END
ELSE IF EXISTS
(
	SELECT 1
	FROM sys.indexes
	WHERE object_id = OBJECT_ID(N'dbo.StaffRoles')
		AND name = N'UQ_StaffRoles_Name'
		AND is_unique_constraint = 0
)
BEGIN
	DROP INDEX UQ_StaffRoles_Name ON dbo.StaffRoles;
END;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.StaffRoles') AND name = N'UQ_StaffRoles_CenterId_Name')
BEGIN
	CREATE UNIQUE INDEX UQ_StaffRoles_CenterId_Name ON dbo.StaffRoles(CenterId, Name);
END;

IF COL_LENGTH(N'dbo.Users', N'Company') IS NULL
BEGIN
	ALTER TABLE dbo.Users ADD Company nvarchar(150) NULL;
END;

IF COL_LENGTH(N'dbo.Centers', N'PhoneNumber') IS NULL
	ALTER TABLE dbo.Centers ADD PhoneNumber nvarchar(30) NULL;

IF COL_LENGTH(N'dbo.Centers', N'Latitude') IS NULL
	ALTER TABLE dbo.Centers ADD Latitude decimal(9, 6) NULL;

IF COL_LENGTH(N'dbo.Centers', N'Longitude') IS NULL
	ALTER TABLE dbo.Centers ADD Longitude decimal(9, 6) NULL;

IF COL_LENGTH(N'dbo.ResourcesTypes', N'Description') IS NULL
	ALTER TABLE dbo.ResourcesTypes ADD Description nvarchar(500) NULL;

IF COL_LENGTH(N'dbo.ResourcesTypes', N'Unit') IS NULL
	ALTER TABLE dbo.ResourcesTypes ADD Unit nvarchar(50) NULL;

IF COL_LENGTH(N'dbo.ResourcesTypes', N'ImageUrl') IS NULL
	ALTER TABLE dbo.ResourcesTypes ADD ImageUrl nvarchar(500) NULL;

IF COL_LENGTH(N'dbo.Services', N'Description') IS NULL
	ALTER TABLE dbo.Services ADD Description nvarchar(500) NULL;

IF COL_LENGTH(N'dbo.Services', N'Icon') IS NULL
	ALTER TABLE dbo.Services ADD Icon nvarchar(100) NULL;

IF COL_LENGTH(N'dbo.Services', N'ImageUrl') IS NULL
	ALTER TABLE dbo.Services ADD ImageUrl nvarchar(500) NULL;

IF OBJECT_ID(N'dbo.ResourceUnits', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.ResourceUnits
	(
		ResourceUnitId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ResourceUnits PRIMARY KEY,
		ResourcesTypeId int NOT NULL,
		Label nvarchar(100) NOT NULL,
		Status nvarchar(20) NOT NULL CONSTRAINT DF_ResourceUnits_Status DEFAULT N'Available',
		CONSTRAINT FK_ResourceUnits_ResourcesTypes FOREIGN KEY (ResourcesTypeId) REFERENCES dbo.ResourcesTypes(ResourcesTypeId),
		CONSTRAINT UQ_ResourceUnits_ResourcesTypeId_Label UNIQUE (ResourcesTypeId, Label),
		CONSTRAINT CK_ResourceUnits_Status CHECK (Status IN (N'Available', N'Reserved', N'Maintenance', N'Disabled'))
	);
END;
