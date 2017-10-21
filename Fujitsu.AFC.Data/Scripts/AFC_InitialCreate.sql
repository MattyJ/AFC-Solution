DECLARE @CurrentMigration [nvarchar](max)

IF object_id('[dbo].[__MigrationHistory]') IS NOT NULL
    SELECT @CurrentMigration =
        (SELECT TOP (1)
        [Project1].[MigrationId] AS [MigrationId]
        FROM ( SELECT
        [Extent1].[MigrationId] AS [MigrationId]
        FROM [dbo].[__MigrationHistory] AS [Extent1]
        WHERE [Extent1].[ContextKey] = N'Fujitsu.AFC.Data.Migrations.AFC.Configuration'
        )  AS [Project1]
        ORDER BY [Project1].[MigrationId] DESC)

IF @CurrentMigration IS NULL
    SET @CurrentMigration = '0'

IF @CurrentMigration < '201605061008159_InitialCreate'
BEGIN
    CREATE TABLE [dbo].[Library] (
        [Id] [int] NOT NULL IDENTITY,
        [SiteId] [int] NOT NULL,
        [CaseId] [int] NOT NULL,
        [Title] [nvarchar](max),
        [ProjectId] [int] NOT NULL,
        [IsPrimaryCase] [bit] NOT NULL,
        [ListId] [uniqueidentifier] NOT NULL,
        [IsClosed] [bit] NOT NULL,
        [Url] [nvarchar](max),
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.Library] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_SiteId] ON [dbo].[Library]([SiteId])
    CREATE UNIQUE INDEX [IX_CaseId] ON [dbo].[Library]([CaseId])
    CREATE TABLE [dbo].[Site] (
        [Id] [int] NOT NULL IDENTITY,
        [Url] [nvarchar](max),
        [Title] [nvarchar](max),
        [Pin] [int] NOT NULL,
        [ProvisionedSiteId] [int] NOT NULL,
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.Site] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [IX_Pin] ON [dbo].[Site]([Pin])
    CREATE INDEX [IX_ProvisionedSiteId] ON [dbo].[Site]([ProvisionedSiteId])
    CREATE TABLE [dbo].[ProvisionedSite] (
        [Id] [int] NOT NULL IDENTITY,
        [IsAllocated] [bit] NOT NULL,
        [Name] [nvarchar](36) NOT NULL,
        [Url] [nvarchar](max),
        [ProvisionedSiteCollectionId] [int] NOT NULL,
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.ProvisionedSite] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_ProvisionedSiteCollectionId] ON [dbo].[ProvisionedSite]([ProvisionedSiteCollectionId])
    CREATE TABLE [dbo].[ProvisionedSiteCollection] (
        [Id] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](50),
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.ProvisionedSiteCollection] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [IX_Name] ON [dbo].[ProvisionedSiteCollection]([Name])
    CREATE TABLE [dbo].[Parameter] (
        [Id] [int] NOT NULL IDENTITY,
        [Name] [nvarchar](50) NOT NULL,
        [Value] [nvarchar](max) NOT NULL,
        [Description] [nvarchar](250) NOT NULL,
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.Parameter] PRIMARY KEY ([Id])
    )
    CREATE UNIQUE INDEX [IX_Name] ON [dbo].[Parameter]([Name])
    CREATE TABLE [dbo].[HistoryLog] (
        [Id] [int] NOT NULL IDENTITY,
        [TaskId] [int] NOT NULL,
        [EventType] [nvarchar](max),
        [EventDetail] [nvarchar](max),
        [CompletedDate] [datetime] NOT NULL,
        [SiteId] [int],
        [Escalated] [bit],
        [Handler] [nvarchar](50) NOT NULL,
        [Name] [nvarchar](50) NOT NULL,
        [Frequency] [nvarchar](max) NOT NULL,
        [Pin] [int],
        [ProjectId] [int],
        [ProjectName] [nvarchar](100),
        [CaseId] [int],
        [SiteTitle] [nvarchar](100),
        [CaseTitle] [nvarchar](100),
        [Dictionary] [nvarchar](max),
        [IsPrimary] [bit],
        [CurrentProjectId] [int],
        [NewProjectId] [int],
        [FromPin] [int],
        [ToPin] [int],
        [CurrentCaseId] [int],
        [NewCaseId] [int],
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.HistoryLog] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[Task] (
        [Id] [int] NOT NULL IDENTITY,
        [CompletedDate] [datetime],
        [NextScheduledDate] [datetime],
        [SiteId] [int],
        [Handler] [nvarchar](50) NOT NULL,
        [Name] [nvarchar](50) NOT NULL,
        [Frequency] [nvarchar](max) NOT NULL,
        [Pin] [int],
        [ProjectId] [int],
        [ProjectName] [nvarchar](100),
        [CaseId] [int],
        [SiteTitle] [nvarchar](100),
        [CaseTitle] [nvarchar](100),
        [Dictionary] [nvarchar](max),
        [IsPrimary] [bit],
        [CurrentProjectId] [int],
        [NewProjectId] [int],
        [FromPin] [int],
        [ToPin] [int],
        [CurrentCaseId] [int],
        [NewCaseId] [int],
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.Task] PRIMARY KEY ([Id])
    )
    CREATE TABLE [dbo].[TimerLock] (
        [Id] [int] NOT NULL IDENTITY,
        [LockedInstance] [uniqueidentifier] NOT NULL,
        [LockedPin] [int] NOT NULL,
        [TaskId] [int] NOT NULL,
        [InsertedDate] [datetime] NOT NULL,
        [InsertedBy] [nvarchar](150) NOT NULL,
        [UpdatedDate] [datetime] NOT NULL,
        [UpdatedBy] [nvarchar](150) NOT NULL,
        CONSTRAINT [PK_dbo.TimerLock] PRIMARY KEY ([Id])
    )
    CREATE INDEX [IX_TaskId] ON [dbo].[TimerLock]([TaskId])
    ALTER TABLE [dbo].[Library] ADD CONSTRAINT [FK_dbo.Library_dbo.Site_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [dbo].[Site] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[Site] ADD CONSTRAINT [FK_dbo.Site_dbo.ProvisionedSite_ProvisionedSiteId] FOREIGN KEY ([ProvisionedSiteId]) REFERENCES [dbo].[ProvisionedSite] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[ProvisionedSite] ADD CONSTRAINT [FK_dbo.ProvisionedSite_dbo.ProvisionedSiteCollection_ProvisionedSiteCollectionId] FOREIGN KEY ([ProvisionedSiteCollectionId]) REFERENCES [dbo].[ProvisionedSiteCollection] ([Id]) ON DELETE CASCADE
    ALTER TABLE [dbo].[TimerLock] ADD CONSTRAINT [FK_dbo.TimerLock_dbo.Task_TaskId] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Task] ([Id]) ON DELETE CASCADE
    CREATE TABLE [dbo].[__MigrationHistory] (
        [MigrationId] [nvarchar](150) NOT NULL,
        [ContextKey] [nvarchar](300) NOT NULL,
        [Model] [varbinary](max) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
    )
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'201605061008159_InitialCreate', N'Fujitsu.AFC.Data.Migrations.AFC.Configuration',  0x1F8B0800000000000400ED5D5B6FE4B6157E2FD0FF20E8A92D361EDB8B16AD314EE01DAF13A3F6DAD8B183BE2D68899E65A29126BAB81E04F9657DE84FEA5F28A92B6FA2484AA3B11D21C0C6438A1F0FC9730E0F6FE7FCEF3FFF9D7FF7BC0E9C271827280A4FDDA38343D781A117F9285C9DBA59FAF8CDDFDDEFBEFDE31FE61FFDF5B3F363F5DD7BF21D2E1926A7EED734DD9CCC6689F715AE4172B0465E1C25D1637AE045EB19F0A3D9F1E1E13F6647473388215C8CE538F3CF5998A235CC7FE09F8B28F4E026CD40701DF93048CA749CB3CC519D4F600D930DF0E0A97B91FD84D2243B38BB581C9C8314B8CE598000A663098347D7016118A520C5549EDC277099C651B85A6E700208EEB61B88BF7B0441024BEA4F9ACF751B72784C1A326B0A56505E96A4D1DA10F0E87DD93333BEB855FFBA75CFE1BEFB88FB38DD9256E7FD77EA5EA18718C45BD7E1EB3A590431F98EEDDD7C2C0ECA42EF1C21EB5DCD0F986DC87FEF9C4516A4590C4F4398A531C05FDC660F01F2FE09B777D1CF303C0DB320A069C454E23C260127DDC6D106C6E9F6337C2C29BFF45D67C6969BF105EB625499A2519761FAFED8753EE1CAC143006B16A03A60994631FC1E86300629F46F419AC238241830EF44A176AEAE254A61777D6A8C05487A63DCA134801504667E2CC5AE730D9EAF60B84ABF9EBAF84FD7B940CFD0AF524AD8FB1061A1C785D238EBAC05FFFC097A695F622F93DB18AD316F919657501FA22880203406BB420945D0F719F22DE859045102FDBEA4DCC7C1CE87E0324CF02FE8630D58771DF9FB0E6B55F38697601FB60ABA8FFE7AA845B769676D7C30543B4AAC519AF1093CA155AE3B24AAC0753EC320CF4CBEA24D3135558AF44BF1C1451CAD3F4741A395F3F42FCB288B3DD2139124F30EC42B98B2A4CC678D9A572AFFA25A13CD4F4A4C6A7FDF923E924A47613F658E7F3E21C215D01F62329CF4DB4BD56FDC404B555DAEAD840F1B9527CB17549FF4A35E2A50A0C8441B728527C5A836A5CE8220F248D9BED614F957C1EFEFFFB613A91D41B1730CB58882001BD5F8F7A43B7F27BAB31972A916E5759FA274A35AB50B09FA56BFE4904A986E450F75DCC04C8AB98736D5148049FFBC5CFDA32F8820C6FFC7EC63287855B149D0762F68864CF62308B221D68A86D59EC3C48BD1A650E2AD951FEFA6CD93B67915DAE60784554CBCBD8A5666EAA62937E91BE53611487EEEBB70F8F884EB22003B5FFBE4359DC314A0DDAFB316D17A13C0A1244BBAB9D5D1D8C403817429AC2EF70308FD804CCF23CF227B99BA2E62F84B06434FA5B176347D897B9F76C75F5AA53A3AF7E870080B5C7A94D8CDD75D5BCDC311374E4DE7285F11E647DEBB3E8FABCE320D657C91C5315685762CF509FEDBAE20D92E30E5FABBC8B444D9360B66C40DB328355982ED582FC81224968A990D484A4CD65F0F1BA74BDA9E5372CFCACF023B000B9B68B26D26DB66B26D26DB46A19426DB66B26D5E9D6D839B115F459EA98153159BAC1C555DA487A08FB93705A157B38FD55DCF02AAF7ADA72176DD26D1DEA768AB8EEB8B958AE464BE96D72FC527CDF13B9B239CB173D92607E96749127928A7447E92AEB81FC0B6F763E83BD697058AC1905C4FC04383D512DA604584A93F75FF22F4B34DBD550FB5D74B5F2060293872799577139E43B24C72CEBCE285069E893DE08B2C81C7C06753B0968464C6472058605EC07A1785A9A85251E8A10D086CDBC9016A6A6942705D359F730E3730240AD676CC756852DE651289AD69E27ABDAB93E7334A0AD4C221BD82D8C68FEAFB880DEB99F2B9F20AA3BE28ED9F91550D19816755E363C19E7B634AE609401BD3C8DF0334CC52BFE2D2E743E92B820EBEDE3FD3C9A81E81D964FDAF53ED5E398BB348DA18A1CD3C6958815AC1E8F3578B5943C3E676D24BE33039DD23F0987C1C742AAE561C2370596187E232292E01E39200BC3025CF6E49327C4E25EBDCFB04964BDDA4B4C079B621B04B98324A11C1C4751ACB97D77502EBB11844F664E50BC5D651989B1D6438C2046D06D9D8431AE0B441DB554D75174F0ADBDCEFEB80213CD5DCB39161D1B77734C064108506E82A5CC98514A1514D1C0CC5C0AD63A0BA3E4D9537BE7BCDCB619FD555DD1D32A61424BECF7AAABD26865779B5CB76B4C620C85F0289FDDD6DAEEB1BEC54DBBABB4E69A21B8C8745DFB00F42C53E69B716BBED458A724ABF2AFA416A2276F5A445A3F9AD13B1D92A5346C798A188A6D589A2ED2DE60B0D542835EDD6579B38F5E459E7CD67854B8B32613E6BF17D31BF069B0D0A57942F8C32C559168E3016DF2CCD7D44AC0B8C9997485C45D4D4D63561A50F5690CB25C7CD3EBC407192124BE0217FBDBFF0D7C2679CA9D0A2FAABCA786B401CAF6A3EA84A90BFCB431FCE29486D3888265559F802376C4D0CB27C4F5A10174949877823010188253BE0586966EBB0DD346C2F5DAD1A6884B695443B4A75A244A35469FA28E5C9250D5226E963508776340E95AC8FC53988603A99CDD2C7ACFC44D060559A096595AB0896A82A551F297F694883E409069430A7070C354C8E3922D97B97E17D10CC7165FBE84301A69D7486311E4F1C952C62CD679CD00B6B2741B708CB4D565969A9B262CAECA5C66453BF860E9317DB8D02EBCBC083281D728EC7A81B242C9E94E5C5FD414E79756D1F2A7A7712D0972AA0824DDF4B563B562F1A62DB89B01B09661EEBB3F31995A18F575C03A3818A94F1748AF2384A21D9EA732B450F4E32FE4A649CDEDD1852DADB3784CCE55E85B51B0DD05F62270978B912D06C12F7E3F87A3FD982C3DBCBBE548E2EDF52D31065923E06F3309A46623226297B0352C69FA1F41235EAB8C55CD6548577236CD59924B3866B39A76C47A15E1FD34054B22156F5BE5840AB320CF6D7D8F735CC361B9B35F6CE1FF5B898696793AC8F553FCAA191EAC431952FF5628686A19247DD1C186C47F3967E28234133EDA761F67DA9F7313C331A6FC5502F6078A28CB1E8372ECCEC49A55BEC26B7EC241BB55278DAC23456C835901DE6DD0B23434C8E892C950F6258492A130DB47C24A09449C6FD26655B36CBA8C7647854F26460BD1103ABA759253B4FD630A8E4C576634AEDC2D090BCEF650545C81EDB88990C8FC9F0980C0FAD564E86C7647818214E86079F6F6A783497C8FA591FF5F5550B13A4BDEC6EEC10FEA92D735D86CB334515A4994A1E7BD36992BB3DCB9D705392FFA4AEBD4CA97FD73725CB5B8ADDA1C3846B8BC527AE931FC0F9E4CAE2729BA4705D5EBEF925580408B7B7F9E01A84E8112669F1CEDE3D3E3C3AE6E28FBD9C5860B324F103C92D4FCA5D81F4AEE318BE0210E9D44E6F00BD5C33E655088F532E431F3E9FBABFE6454E9CCB7F7D294ABD736E623CBE27CEA1F35BCFC85D1D155F26F721C28B8913E72ECEA0456D8C7F99F009C4DE5710FF690D9EFF4C4359C5F1CA491F228AD7033207622378657927A19C171E115906F68CE66543121506A25747CB5C3C10859B0EE3E2A1A24DF48ED0DF838335998207074B2ACD025CBD0D4D3614DF0DA82A1A4F29BBD66FB7E26D4B338D2E00F452EE93EC8E20BBB7AA9B8FAF568C2551906C66A1626F4E31102406D29E344CEBD5B5DE624B434D02FCCA04B8D5F7D06B1565B90C9ACC829C97D1C9807C793C2CBFA438F1ACC8B3E6D1685A6719DB0833526E38B6207392AD1164ABED52E2AB152ED6F3A3CDC689106DA597212689A8D20B4FEA51DC9ADF240B39C3E6F1315438435A0783F3392E65790BB9EC34CF2D30058FE1FDD427BF7A1F769BD000A3B3AF72E7D8869C2AEEBE9A0108EEC187A46C27C0A2F3EF7EDB93BC836F0BE16A73F66D311E32D7DF16309C23700B04C62DB84579A99370BB0EE98D31593923583912DF7BAFD5BED19EFFF518B825C2882D606F7B62B205265BA08DB1265B60B205265B60B205860A05F2360C0279F08DDED73484401C365B2992CD188D03A7A2D474B6F4C22469844017B4EFD7A20D7B8C2CD14A65BF681B4304AC507A35D1D15A4E8FC814BF87C0133D467C473C2890311EB78D1549E2ED858CD01AB41D314CEDA29DAA5E3B4485753092110341BC9DE00FB9C3656A98761E9441E262D936D8845D7C07F983DFC13945F5B447AC6CEF211C44F7CDFCB0C922346C55F1198A9714D8AA7C88F00817369F49F086D6D80D32609B100C3A411D6475091F99554BDB905D04D0DF6A90621327421D26425AA97614093A4684500B9D29ABC630C0446B7C0919B659E40975E00929FEEB8E4B21B2688753425171290C5A857B5361FE7829D1257A34426296F17E99FB377BA0C0115AE4C96D3DAA509B96DF6F68084A47343E3C544D13E49F7B77ABDF3C83D80FE2CB453CD76721D996297E9DC304AD1A8839C60C0B296C40AB6F2EC3C7A83236388AAA4FB89D9C6B98021F9B0067718A1E814736EC3D9824799CD5FCF2DEA9FB71FD4036FD6EB27493A5B8C970FD1030634D8C1655FD79800B96E6F94D7E912F19A209984C44B6A36EC20F190AFC9AEE0BC97E530B04B186CA7D52329629D92F5D6D6BA44F58D4F580CAEEAB8DB83BB8DE903B4BC94DB8044FD086B6FB045EC115F0B6B7E503D47690EE8160BB7D7E8EC00ACFED4989D194C73F310FFBEBE76FFF0FD9145EEC21A80000 , N'6.1.3-40302')
END

IF @CurrentMigration < '201605061010222_AddeAspireStoredProcedures'
BEGIN
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateAvailableSites                                                     */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Retrieves the number of sites which have been provisioned but not yet allocated   */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: None                                                               */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @AvailableSiteCount as an OUT parameter                                               */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 22/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 19/04/2016     0.2      Matt Jordan       Fixed bug the code was returning the number          */
        /*                                           of allocated sites and not unallocated sites.        */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateAvailableSites]
        	@AvailableSiteCount AS INTEGER OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	SET @AvailableSiteCount = (SELECT COUNT(IsAllocated)
        		FROM dbo.ProvisionedSite
        		WHERE IsAllocated = 0)
        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateCaseIdRequested                                                        */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 21/04/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the selected CaseId has already been requested, via a previous          */
        /*              AllocateCaseId  request but has not yet been processed.                           */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @CaseId                                                            */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @CaseIdRequested as an OUT parameter                                                  */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 21/04/2016     0.0      Ray Banister	     First implementation                                 */
        /* 02/08/2016     0.1      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateCaseIdRequested]
        	@CaseId INT,
        	@CaseIdRequested BIT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	IF EXISTS(SELECT  1 FROM dbo.Task
        		WHERE (Handler = ''OperationsHandler'' AND Name = ''AllocateCase'' AND CaseId = @CaseId AND InsertedDate <= GETDATE()))
        		SET @CaseIdRequested = ''True''
        	ELSE SET @CaseIdRequested = ''False''
        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateCaseIdInUse                                                            */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the CaseId has already been used.                                       */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @CaseId                                                            */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @CaseIdInUse as an OUT parameter                                                      */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 21/04/2016     0.2      Ray Banister      Correct functionality now added                      */
        /* 06/05/2016     0.3      Matt Jordan       Removed Explicit USE database                        */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateCaseIdInUse]
        	@CaseId INT,
        	@CaseIdInUse BIT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;


        	IF EXISTS(SELECT  1 FROM dbo.Library WHERE CaseId = @CaseId)
        		SET @CaseIdInUse = ''True''
        	ELSE SET @CaseIdInUse = ''False''

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateDictionary                                                             */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks the Dictionary string for well formed xml.                                 */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @Dictionary                                                        */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @IsValid as an OUT parameter                                                          */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*               NOTE: XML VALIDATION NEEDS IMPROVEMENT                                           */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 11/04/2016	  0.1	   Ray Banister      Dictionary size now NVARCHAR(MAX)                    */
        /* 12/04/2016     0.2      Matt Jordan       Removed Explicit USE database                        */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateDictionary]
        	@Dictionary NVARCHAR(MAX),
        	@IsValid BIT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @XML AS XML

        	-- Check Dictionary XML is valid.
        	-- *********************************************************
        	--  This currently isn''t good enough  !!! But it''s a starter
        	-- *********************************************************
        	BEGIN TRY
        		SELECT @XML = CAST(REPLACE(REPLACE(REPLACE(@Dictionary,CHAR(10) + CHAR(13), ''''),CHAR(10), ''''), CHAR(13), '''') AS XML)
        		SET @IsValid = 1
        	END TRY
        	BEGIN CATCH
        		SET @IsValid = 0
        	END CATCH


        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateIsPIN_MergeFromPIN                                                     */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the selected PIN = FromPin in any earlier ''merge'' task awaiting         */
        /*              processing.                                                                       */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @IsMergeFromPIN as an OUT parameter                                                   */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 02/08/2016     0.2      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateIsPIN_MergeFromPIN]
        	@PIN INT,
        	@IsMergeFromPIN BIT OUT

        AS
        BEGIN
        	-- Check if there is an earlier ''merge'' task with the same PIN
        	IF EXISTS(SELECT  1 FROM dbo.Task
        		WHERE (Handler = ''OperationsHandler'' AND Name = ''MergePin'' AND FromPin = @PIN AND InsertedDate <= GETDATE()))
        		SET @IsMergeFromPIN = ''True''
        	ELSE SET @IsMergeFromPIN = ''False''
        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateIsPIN_MergeToPIN                                                       */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the selected PIN = ToPin in any earlier ''merge'' task awaiting           */
        /*              processing.                                                                       */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @IsMergeToPIN as an OUT parameter                                                     */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 02/08/2016     0.2      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateIsPIN_MergeToPIN]
        	@PIN INT,
        	@IsMergeToPIN BIT OUT

        AS
        BEGIN
        	-- Check if there is an earlier ''merge'' task with the same PIN
        	IF EXISTS(SELECT  1 FROM dbo.Task
        		WHERE (Handler = ''OperationsHandler'' AND Name = ''MergePin'' AND ToPin = @PIN AND InsertedDate <= GETDATE()))
        		SET @IsMergeToPIN = ''True''
        	ELSE SET @IsMergeToPIN = ''False''
        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidatePINExists                                                              */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the selected PIN exists in either the Site Table or ProvisionedSite     */
        /*              Table.                                                                            */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @PINExits as an OUT parameter                                                         */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 22/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidatePINExists]
        	@PIN INT,
        	@PinExists BIT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	IF EXISTS(SELECT  1 FROM dbo.Site WHERE Pin = @PIN)
        		SET @PinExists = ''True''
        	-- May need to add the ProvisionedSite table in here but note name may be a GUID !!
        	ELSE SET @PinExists = ''False''

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidatePINRequested                                                           */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the selected PIN has already been requested, via a previous AllocatePin */
        /*              request but has not yet been processed.                                           */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @PINRequested as an OUT parameter                                                     */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 22/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 30/03/2016     0.1	   Ray Banister      Replaced 0/1 with True/False                         */
        /* 12/04/2016     0.2      Matt Jordan       Removed Explicit USE database                        */
        /* 05/05/2016     0.3      Ray Banister      Method name corrected                                */
        /* 02/08/2016     0.4      Ray Banister      Timestamp comparison now <= (ref: TFS: 32618)        */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidatePINRequested]
        	@PIN INT,
        	@PINRequested BIT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	IF EXISTS(SELECT  1 FROM dbo.Task
        		WHERE (Handler = ''OperationsHandler'' AND Name = ''AllocatePin'' AND Pin = @PIN AND InsertedDate <= GETDATE()))
        		SET @PINRequested = ''True''
        	ELSE SET @PINRequested = ''False''
        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_AllocateCase                                                           */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Adds an AllocateCase task to the list providing the PIN exists, the CaseId is     */
        /*              unique and the Dictionary, if submitted is valid XML.                             */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @ProjectId, @CaseId,@CaseTitle,@IsPrimary,@Dictionary        */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVAL, ERR_CASE_IN_USE,ERR_PIN_BEING_MERGED            */
        /*                ERR_CASEID_ALREADY_REQUESTED,ERR_INVALID_PARAMETERS                             */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 21/04/2016     0.2      Ray Banister      Added check for Pending PIN Allocation and pending   */
        /*                                           Allocate Case with same CaseId                       */
        /* 03/05/2016     0.3      Ray Banister      Dictionary size changed to NVARCHAR(MAX)             */
        /* 05/05/2016     0.4      Ray Banister      Dictionary is now MANDATORY                          */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_AllocateCase]
            -- Add the parameters for the stored procedure here
            @PIN INT = NULL,
            @ProjectId INT = NULL,
            @CaseId INT = NULL,
            @CaseTitle NVARCHAR(100) = NULL,
            @Dictionary NVARCHAR (MAX) = NULL,
            @IsPrimary BIT = NULL
        AS
        BEGIN
            -- SET NOCOUNT ON added to prevent extra result sets from
            -- interfering with SELECT statements.
            SET NOCOUNT ON;
            DECLARE @RESULT NVARCHAR(300) = ''''
            DECLARE @PINExists AS BIT = 0
            DECLARE @IsValid AS BIT = 0
            DECLARE @CaseIdInUse AS BIT = 0
            DECLARE @IsMergeFromPIN AS BIT = 0
            DECLARE @PINRequested AS BIT = 0
            DECLARE @CaseIdRequested AS BIT = 0

            -- Check all Parameters are there
            IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR
                (@ProjectId IS NULL) OR (ISNUMERIC(@ProjectId) = 0) OR
                (@CaseId IS NULL) OR (ISNUMERIC(@CaseId) = 0) OR
                (@IsPrimary IS NULL) OR (NULLIF(@CaseTitle,'''') IS NULL) OR
        		(NULLIF(@Dictionary,'''') IS NULL)
            BEGIN
                RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
                RETURN
            END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

            -- Check if PIN is in use for an earlier ''merge'' as the FromPin - This is an error if it does not exist
            EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPIN OUTPUT
            IF @IsMergeFromPIN = 1
            BEGIN
                RAISERROR(60012,16,0)		-- ERR_PIN_BEING_MERGED
                RETURN
            END

            -- Check if the CaseId has already been used - This is an error if it has
            EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
            IF @CaseIdInUse = 1
            BEGIN
                RAISERROR(60003,16,0)		-- ERR_CASE_IN_USE
                RETURN
            END

            -- Check if someone has already requested a Case with the same CaseId
            EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
            IF @CaseIdRequested = 1
            BEGIN
                RAISERROR(60015,16,0)		-- ERR_CASEID_ALREADY_REQUESTED
                RETURN
            END

            -- Check Dictionary XML is valid.
            EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
            IF @IsValid = 0
            BEGIN
                RAISERROR(60007,16,0)		-- ERR_DICT_INVAL
                RETURN
            END


            -- All preliminary checks are OK so write the Task to the Task Table
            INSERT INTO dbo.Task (PIN,ProjectId,CaseId,CaseTitle,Dictionary,IsPrimary,
                					Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
            VALUES (@PIN,@ProjectId,@CaseId,@CaseTitle,@Dictionary,@IsPrimary,
                		''OperationsHandler'',''AllocateCase'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_AllocatePIN                                                            */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 22/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Allocate a new PIN if it doesn''t already exist, is in use and there are some      */
        /*              Provisioned (unallocated) Sites available.                                        */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @SiteTitle,@Dictionary                                       */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                    Site URL                                                           */
        /* Errors Raised: ERR_NOSITES, ERR_PIN_IN_USE, ERR_DICT_INVAL, ERR_PIN_ALREADY_REQUESTED          */
        /*   		      ERR_INVALID_PARAMETERS                                                          */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 22/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 23/03/2016     0.1      Ray Banister      Task name corrected                                  */
        /* 30/03/2016     0.2      Ray Banister      Replaced ''PININUse with PINExists [Duplicate]        */
        /* 12/04/2016     0.3      Matt Jordan       Removed Explicit USE database                        */
        /* 03/05/2016     0.4      Ray Banister      Dictionary size changed to NVARCHAR(MAX)             */
        /* 05/05/2016     0.5      Ray Banister      Dictionary is now MANDATORY                          */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_AllocatePIN]
        	-- Add the parameters for the stored procedure here
        	@PIN INT =NULL,
        	@SiteTitle NVARCHAR(100) = NULL,
        	@Dictionary NVARCHAR (MAX) = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @XML AS XML
        	DECLARE @COUNT AS INTEGER = 0
        	DECLARE @PINExists AS BIT = 0
        	DECLARE @PINRequested AS BIT = 0
        	DECLARE @IsValid AS BIT = 0

        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR
        		(NULLIF(@SiteTitle,'''') IS NULL ) OR
        		(NULLIF(@Dictionary,'''') IS NULL)
        	BEGIN
        		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
        		RETURN
        	END

        	-- Check some Unallocated Site(s) Exist [ProvisionedSite Table]. Error if none available
        	EXEC dbo.sp_ValidateAvailableSites @COUNT OUTPUT
        	IF @COUNT = 0
        	BEGIN
        		RAISERROR(60001,16,0)
        		RETURN
        	END

        	-- Check if PIN already in use [Site Table] - This is an error
        	EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
        	IF @PINExists = 1
        	BEGIN
        		RAISERROR(60002,16,0)
        		RETURN
        	END

        	-- Check if PIN already requested - This is an error
        	EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
        	IF @PINRequested = 1
        	BEGIN
        		RAISERROR(60005,16,0)
        		RETURN
        	END

        	-- Check Dictionary XML is valid.
        	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
        	IF @IsValid = 0
        	BEGIN
        		RAISERROR(60007,16,0)
        		RETURN
        	END


        	-- All preliminary checks are OK OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,SiteTitle,Dictionary,
        							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@SiteTitle,@Dictionary,
        				''OperationsHandler'',''AllocatePin'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_DeletePIN                                                              */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 30/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Delete the PIN Site                                                               */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED                    */
        /*                                                                                                */
        /* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
        /* and the PIN = (FromPIN OR ToPIN) in that MergePIN request.                                     */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
        /* 16/05/2016     0.2      Ray Banister	     Removed error on "ToPIN" check                       */
        /* 25/05/2016     0.3      Ray Banister      Now check for ''pending'' PIN                          */
        /* 19/08/2016     0.4      Ray Banister	     Restored error on "ToPIN" check                      */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        Create PROCEDURE [dbo].[sp_eAspire_DeletePIN]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = 0
        	DECLARE @IsMergeFromPin AS BIT = ''False''
        	DECLARE @IsMergeToPin AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

        -- Check if there is an earlier ''merge'' task with the same PIN as the FromPIN
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
        	IF @IsMergeFromPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END

        -- Check if there is an earlier ''merge'' task with the same PIN as the ToPIN
        	EXEC dbo.sp_ValidateIsPIN_MergeToPIN @PIN, @IsMergeToPin OUTPUT
        	IF @IsMergeToPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END

        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,''OperationsHandler'',''DeletePin'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_GetDictionaryXml                                                    */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 29/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Retrieves a sample Dictionary XML                                                 */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: NONE                                                               */
        /* Optional Input Parameters:  NONE                                                               */
        /* Returns: @Dictionary                                                                           */
        /* Errors Raised: NONE                                                                            */
        /*                                                                                                */
        /*                       TEST FUNCTION ONLY                                                       */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 29/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 11/04/2016	  0.1	   Ray Banister      Renamed to GetDictionaryXml as per v10 of spec.      */
        /* 11/04/2016	  0.2	   Ray Banister      Dictionary size increased to NVARCHAR(max)           */
        /* 12/04/2016     0.3      Matt Jordan       Removed Explicit USE database                        */
        /* 06/05/2016     0.4      Matt Jordan       Spurious ; removed before GO command                 */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_GetDictionaryXml]
        	@Dictionary NVARCHAR(MAX) OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''

        	BEGIN
        		SET @Dictionary = ''<items><item><key>Service Type</key><value/></item><item><key>Service User Pin</key><value/></item></items>''
        	END
        END

    ')
    EXECUTE('
                /* ============================================================================================== */
                /* Name:        sp_eAspire_MergePIN                                                               */
                /* Author:		Ray Banister                                                                      */
                /* Create date: 11/04/2016                                                                        */
                /*                                                                                                */
                /* Description:	Merge DCF for two Service users                                                   */
                /*                                                                                                */
                /*		                                                                                          */
                /* Mandatory Input Parameters: @ToPIN, @FromPIN                                                   */
                /* Optional Input Parameters:  NONE                                                               */
                /* Returns:                                                                                       */
                /* Errors Raised: ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED, ERR_INVALID_PARAMETERS                    */
                /*                ERR_PIN_PENDING_DELETION                                                        */
                /*                                                                                                */
                /*                                                                                                */
                /* Copyright 2016 Fujitsu Services Ltd                                                            */
                /* ============================================================================================== */
                /*                                        Change Record                                           */
                /* ============================================================================================== */
                /*    Date      Version     Changed By                     Comments                               */
                /* ============================================================================================== */
                /* 11/04/2016     0.0      Ray Banister	     First implementation                                 */
                /* 12/04/2016     0.1      Matt Jordan       Removed Explicit USE database                        */
                /* 06/05/2016     0.2      Matt Jordan       SELECT @RESULT removed                               */
                /* 25/05/2016     0.3      Ray Banister      Now check for ''pending'' PINs                         */
                /* 18/08/2016     0.4      Ray Banister      Added check for PIN pending deletion                 */
                /*                                                                                                */
                /*                                                                                                */
                /* ============================================================================================== */

                CREATE PROCEDURE [dbo].[sp_eAspire_MergePIN]
                	@ToPIN INT,
                	@FromPIN INT

                AS
                BEGIN
                	-- SET NOCOUNT ON added to prevent extra result sets from
                	-- interfering with SELECT statements.
                	SET NOCOUNT ON;
                	DECLARE @RESULT NVARCHAR(300) = ''''
                	DECLARE @PINExists BIT = ''False''
                	DECLARE @IsMergeToPIN BIT = ''False''
                	DECLARE @IsMergeFromPIN BIT = ''False''
                	DECLARE @PINRequested AS BIT = ''False''
                	DECLARE @PINAwaitingDeletion AS BIT = ''False''

                	-- Check all Parameters are there
                	IF (@ToPIN is NULL) OR (ISNUMERIC(@ToPIN) = 0) OR
                		(@FromPIN is NULL) or (ISNUMERIC(@FromPIN) = 0)
                	BEGIN
                		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
                		RETURN
                	END

                	-- Check that ToPIN <> FromPIN as that''s silly
                	IF @ToPIN = @FromPIN
                	BEGIN
                		RAISERROR(60014,16,1)		-- ERR_TOPIN_EQUALS_FROMPIN
                		RETURN
                	END

                	-- Check that both PINS exist

                	-- Check if ToPIN already in use [Site Table] - This is an error if it does not exist
                    -- or if it is not pending allocation
                    EXEC dbo.sp_ValidatePINExists @ToPIN, @PINExists OUTPUT
                    IF @PINExists = ''False''
                    BEGIN
                        EXEC dbo.sp_ValidatePINRequested @ToPIN, @PINRequested OUTPUT
                        IF @PINRequested = ''False''
                        BEGIN
                        	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                        	RETURN
                        END
                    END

                	-- Check if FromPIN already in use [Site Table] - This is an error if it does not exist
                    -- or if it is not pending allocation
                	EXEC dbo.sp_ValidatePINExists @FromPIN, @PINExists OUTPUT
                    IF @PINExists = ''False''
                    BEGIN
                        EXEC dbo.sp_ValidatePINRequested @FromPIN, @PINRequested OUTPUT
                        IF @PINRequested = ''False''
                        BEGIN
                        	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                        	RETURN
                        END
                    END

                	-- Check if the ToPIN is the FromPIN in a pending merge
                	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @ToPIN, @IsMergeFromPIN OUTPUT
                	IF @IsMergeFromPIN = ''True''
                	BEGIN
                		RAISERROR(60012,16,1)		-- ERR_PIN_BEING_MERGED
                		RETURN
                	END

                	-- Check if the FromPIN is the FromPIN in a pending merge
                	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @FromPIN, @IsMergeFromPIN OUTPUT
                	IF @IsMergeFromPIN = ''True''
                	BEGIN
                		RAISERROR(60012,16,1)		-- ERR_PIN_BEING_MERGED
                		RETURN
                	END

                	-- Check if the FromPIN is in a pending DeletePIN Request
                	EXEC dbo.sp_ValidateIsPIN_AwaitingDeletion @FromPIN, @PINAwaitingDeletion OUTPUT
                	IF @PINAwaitingDeletion = ''True''
                	BEGIN
                		RAISERROR(60018,16,1)		-- ERR_PIN_PENDING_DELETION
                		RETURN
                	END

                	-- Check if the ToPIN is in a pending DeletePIN Request
                	EXEC dbo.sp_ValidateIsPIN_AwaitingDeletion @ToPIN, @PINAwaitingDeletion OUTPUT
                	IF @PINAwaitingDeletion = ''True''
                	BEGIN
                		RAISERROR(60018,16,1)		-- ERR_PIN_PENDING_DELETION
                		RETURN
                	END

                	-- All preliminary checks are OK so write the Task to the Task Table
                	INSERT INTO dbo.Task (ToPIN,FromPIN,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
                	VALUES (@ToPIN,@FromPIN,''OperationsHandler'',''MergePin'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)


                END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_PINUrl                                                                 */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 29/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Retrive the URL for a specified PIN                                               */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:  NONE                                                               */
        /* Returns: @Url [OUT], @StatusId [OUT]                                                           */
        /* Errors Raised:  None                                                                           */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 29/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 30/03/2016	  0.1      Ray Banister      Now call sp_ValidatePINRequested                     */
        /* 12/04/2016     0.2      Matt Jordan       Removed Explicit USE database                        */
        /* 16/05/2016     0.3      Ray Banister	     The SP was incorrectly raising ERR_NO_PIN_DCF.       */
        /*                                           The spec does not require this, you just set StausId */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_PINUrl]
        	@PIN INT,
        	@Url NVARCHAR(2000) OUT,
        	@StatusId INT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists BIT = ''False''
        	DECLARE @PINRequested BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
        		RETURN
        	END

        	-- Check if PIN already allocated
        	EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
        	IF @PINExists = ''True''
        	BEGIN
        		-- Return "Initialised" and the Url
        		SET @Url = (SELECT Url FROM dbo.Site WHERE Pin = @PIN)
        		SET @StatusId = 2
        		RETURN
        	END

        	-- Check if PIN awaiting allocation
        	EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
        	IF @PINRequested = ''True''
        	BEGIN
        		-- Return "Initialising" and a NULL Url
        		SET @Url = NULL
        		SET @StatusId = 1
        		RETURN
        	END

        	-- PIN not found and not awaiting allocation. Set "Undeclared"
        	BEGIN
        		SET @Url = NULL
        		SET @StatusId = 0
        		RETURN
        	END


        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_UpdatePINTitle                                                         */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 23/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Update the PIN’s Title with a new value                                           */
        /*              [Also known as Update Service User Title]                                         */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @SiteTitle                                                   */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED                    */
        /*                                                                                                */
        /* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
        /* and the PIN = FromPIN in that MergePIN request.                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 23/03/2016     0.0      Ray Banister	     First implementation                                 */
        /* 23/03/2016	  0.1	   Ray Banister      Merge FromPin check now a validation stored          */
        /*                                           procedure                                            */
        /* 24/03/2016     0.2      Ray Banister      Added check to see if PIN is waiting allocation      */
        /* 30/03/2016     0.3      Ray Banister      Call PINRequested instead of PINwaiting (duplicate)  */
        /* 12/04/2016     0.4      Matt Jordan       Removed Explicit USE database                        */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_UpdatePINTitle]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL,
        	@SiteTitle NVARCHAR(100) = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = ''False''
        	DECLARE @IsMergeFromPin AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR (NULLIF(@SiteTitle,'''')IS NULL )
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

        	-- Check if PIN exists in use [Site Table] - If not, this is an error
        	EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
        	IF @PINExists = ''False''
        	BEGIN
        		EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
        		IF @PINRequested = ''False''
        		BEGIN
        			RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
        			RETURN
        		END
        	END

        -- Check if there is an earlier ''merge'' task with the same PIN
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
        	IF @IsMergeFromPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END

        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,SiteTitle,
        							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@SiteTitle,
        				''OperationsHandler'',''UpdateServiceUserTitle'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_UpdateCaseTitle                                                        */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 22/04/2016                                                                        */
        /*                                                                                                */
        /* Description:	Update the Case Title with a new value                                            */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @CaseTitle , @CaseId                                         */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_NO_CASE_DCF, ERR_PIN_BEING_MERGED   */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 22/04/2016     0.0      Ray Banister	     First implementation                                 */
        /* 11/05/2016	  0.1      Ray Banister      Fixed bug - @IsMergeFromPIN was not set to OUTPUT    */
        /* 25/05/2016     0.2      Ray Banister      Now check for ''pending'' PIN and Case                 */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitle]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL,
        	@CaseId INT = NULL,
        	@CaseTitle NVARCHAR(100) = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = ''False''
        	DECLARE @CaseIdInUse AS BIT = ''False''
        	DECLARE @IsMergeFromPIN AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''
        	DECLARE @CaseIdRequested AS BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR
        	   (@CaseId is null) OR (ISNUMERIC(@CaseId) = 0) OR
        	   (NULLIF(@CaseTitle,'''')IS NULL )
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

            -- Check if Case already in use [Library Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
            IF @CaseIdInUse = ''False''
            BEGIN
        		EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
            	IF @CaseIdRequested = ''False''
        		BEGIN
        			RAISERROR(60006,16,0)	-- Case not found nor awaiting allocation [ERR_NO_CASE_DCF]
                    RETURN
        		END
            END

        	-- Check if the PIN is a "from" PIN in a pending merge
        	exec dbo.sp_ValidateIsPIN_MergeFromPIN @PIN,@IsMergeFromPIN OUTPUT
        	IF @IsMergeFromPIN = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- ERR_PIN_BEING_MERGED
        			RETURN
        		END

        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,CaseId,CaseTitle,
        							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@CaseId,@CaseTitle,
        				''OperationsHandler'',''UpdateCaseTitle'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_UpdateCaseTitleByProject                                               */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 14/03/2016                                                                        */
        /*                                                                                                */
        /* Description:	Replace all the Project Names in the title of cases for the given ProjectId       */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @ProjectName @ProjectId, @Dictionary                               */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_DICT_INVAL                                                                  */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 11/04/2016     0.0      Ray Banister	     First implementation                                 */
        /* 12/04/2016     0.1      Ray Banister	     Removed the "USE" clause.                            */
        /* 26/04/2016     0.2      Ray Banister      Now added the code to insert the task                */
        /* 05/05/2016     0.3      Ray Banister      Dictionary is now MANDATORY                          */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseTitleByProject]
        	@ProjectId INT = NULL,
        	@ProjectName NVARCHAR(100) = NULL,
        	@Dictionary NVARCHAR(MAX) = NULL

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @IsValid AS BIT = 0

        	DECLARE @RESULT NVARCHAR(300) = ''''

        	-- Check all Parameters are there
        	IF (@ProjectId is null) OR
        		(ISNUMERIC(@ProjectId) = 0) OR
        		(NULLIF(@ProjectName,'''')IS NULL ) OR
        		(NULLIF(@Dictionary,'''')IS NULL )
        	BEGIN
        		RAISERROR(60000,16,1)
        		RETURN
        	END

        	-- Check Dictionary XML is valid.
        	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
        	IF @IsValid = 0
        	BEGIN
        		RAISERROR(60007,16,0)
        		RETURN
        	END


        	-- All preliminary checks are OK OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (ProjectId,ProjectName,Dictionary,
        							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@ProjectId,@ProjectName,@Dictionary,
        				''OperationsHandler'',''UpdateCaseTitleByProject'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)


        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_CloseCase                                                              */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 03/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Close the Case                                                                    */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @CaseId, @ProjectId, @IsPrimary, @Dictionary                 */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_NO_CASE_DCF ERR_PIN_BEING_MERGED    */
        /*                ERR_DICT_INVAL                                                                  */
        /* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
        /* and the PIN = FromPIN in that MergePIN request.                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 03/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 05/05/2016     0.1      Matt Jordan       IsPrimary superfluous as per email Andy East         */
        /* 17/05/2016     0.2      Ray Banister      Dictionary was not being checked for NULL            */
        /* 25/05/2016     0.3      Ray Banister      Now check for ''pending'' PIN and Case                 */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_CloseCase]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL,
        	@ProjectId INT = NULL,
        	@CaseId INT = NULL,
        	@Dictionary NVARCHAR(MAX) = NULL

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = ''False''
        	DECLARE @CaseIdInUse AS BIT = ''False''
        	DECLARE @IsMergeFromPin AS BIT = ''False''
        	DECLARE @IsValid AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''
        	DECLARE @CaseIdRequested AS BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is NULL) OR (ISNUMERIC(@PIN) = 0) OR
        	   (@ProjectId is NULL) OR (ISNUMERIC(@ProjectId) = 0) OR
        	   (@CaseId is NULL) OR (ISNUMERIC(@CaseId) = 0) OR
        	   (NULLIF(@Dictionary,'''') IS NULL)

        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

            -- Check if Case already in use [Library Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
            IF @CaseIdInUse = ''False''
            BEGIN
        		EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
            	IF @CaseIdRequested = ''False''
        		BEGIN
        			RAISERROR(60006,16,0)	-- Case not found nor awaiting allocation [ERR_NO_CASE_DCF]
                    RETURN
        		END
            END

        	-- Check Dictionary XML is valid.
        	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
        	IF @IsValid = 0
        	BEGIN
        		RAISERROR(60007,16,0)
        		RETURN
        	END

            -- Check if there is an earlier ''merge'' task with the same PIN as the FromPIN
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
        	IF @IsMergeFromPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END


        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,ProjectId,CaseId,Dictionary,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@Projectid,@CaseId,@Dictionary,''OperationsHandler'',''CloseCase'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_ArchiveCase                                                            */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 03/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Archive the Case                                                                  */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @CaseId                                                      */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_NO_CASE_DCF ERR_PIN_BEING_MERGED    */
        /*                                                                                                */
        /* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
        /* and the PIN = (FromPIN OR ToPIN) in that MergePIN request.                                     */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 03/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 16/05/2016     0.1      Ray Banister	     Removed error on "ToPIN" check                       */
        /* 25/05/2016     0.2      Ray Banister      Now check for ''pending'' PIN and Case                 */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_ArchiveCase]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL,
        	@CaseId INT = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = 0
        	DECLARE @CaseIdInUse AS BIT = ''False''
        	DECLARE @IsMergeFromPin AS BIT = ''False''
        	DECLARE @IsMergeToPin AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''
        	DECLARE @CaseIdRequested AS BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is NULL) OR (ISNUMERIC(@PIN) = 0) OR
        	   (@CaseId is NULL) OR (ISNUMERIC(@CaseId) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

            -- Check if Case already in use [Library Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
            IF @CaseIdInUse = ''False''
            BEGIN
        		EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
            	IF @CaseIdRequested = ''False''
        		BEGIN
        			RAISERROR(60006,16,0)	-- Case not found nor awaiting allocation [ERR_NO_CASE_DCF]
                    RETURN
        		END
            END

        -- Check if there is an earlier ''merge'' task with the same PIN as the FromPIN
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
        	IF @IsMergeFromPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END



        -- Check if there is an earlier ''merge'' task with the same PIN as the ToPIN
        /*
        	EXEC dbo.sp_ValidateIsPIN_MergeToPIN @PIN, @IsMergeToPin OUTPUT
        	IF @IsMergeToPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END
        */
        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,CaseId,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@CaseId,''OperationsHandler'',''ArchiveCase'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_MoveCase                                                               */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Adds a MoveCase task to the list providing the PIN exists, the CaseId is          */
        /*              unique and the Dictionary, if submitted is valid XML.                             */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN,  @CaseId, @CurrentProjectId, @NewProjectId,@IsPrimary,       */
        /*                             @Dictionary                                                        */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVAL, ERR_NO_CASE_DCF,ERR_PIN_BEING_MERGED            */
        /*                ERR_INVALID_PARAMETERS, ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT                  */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister      First Implementation                                 */
        /* 25/05/2016     0.2      Ray Banister      Now check for ''pending'' PIN and Case                 */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_MoveCase]
            @PIN INT = NULL,
            @CaseId INT = NULL,
            @CurrentProjectId INT = NULL,
            @NewProjectId INT = NULL,
            @IsPrimary BIT = NULL,
            @Dictionary NVARCHAR (MAX) = NULL
        AS
        BEGIN
            -- SET NOCOUNT ON added to prevent extra result sets from
            -- interfering with SELECT statements.
            SET NOCOUNT ON;
            DECLARE @RESULT NVARCHAR(300) = ''''
            DECLARE @PINExists AS BIT = ''False''
            DECLARE @IsValid AS BIT = ''False''
            DECLARE @CaseIdInUse AS BIT = ''False''
            DECLARE @IsMergeFromPIN AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''
        	DECLARE @CaseIdRequested AS BIT = ''False''

            -- Check all Parameters are there
            IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR
                (@CaseId IS NULL) OR (ISNUMERIC(@CaseId) = 0) OR
                (@CurrentProjectId IS NULL) OR (ISNUMERIC(@CurrentProjectId) = 0) OR
                (@NewProjectId IS NULL) OR (ISNUMERIC(@NewProjectId) = 0) OR
                (@IsPrimary IS NULL) OR
        		(NULLIF(@Dictionary,'''') IS NULL)
            BEGIN
                RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
                RETURN
            END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

            -- Check if Case already in use [Library Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
            IF @CaseIdInUse = ''False''
            BEGIN
        		EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
            	IF @CaseIdRequested = ''False''
        		BEGIN
        			RAISERROR(60006,16,0)	-- Case not found nor awaiting allocation [ERR_NO_CASE_DCF]
                    RETURN
        		END
            END


            -- Check if PIN is in use for an earlier ''merge'' as the FromPin - This is an error if it does not exist
            EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPIN OUTPUT
            IF @IsMergeFromPIN = 1
            BEGIN
                RAISERROR(60012,16,0)		-- ERR_PIN_BEING_MERGED
                RETURN
            END

            -- Check Dictionary XML is valid.
            EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
            IF @IsValid = 0
            BEGIN
                RAISERROR(60007,16,0)		-- ERR_DICT_INVAL
                RETURN
            END

        	-- Check NewProjectId <> CurrentProjectId as that would be a bit daft
            IF @NewProjectId = @CurrentProjectId
            BEGIN
                RAISERROR(60016,16,0)		-- ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT
                RETURN
            END


            -- All preliminary checks are OK so write the Task to the Task Table
            INSERT INTO dbo.Task (PIN,CaseId,CurrentProjectId,NewProjectId,IsPrimary,Dictionary,
                					Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
            VALUES (@PIN,@CaseId,@CurrentProjectId,@NewProjectId,@IsPrimary,@Dictionary,
                		''OperationsHandler'',''MoveCase'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_ChangePrimaryProject                                                   */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Adds a ChangePrimaryProject task to the list providing the PIN exists,            */
        /*				the PIN is not a FromPIN in a pending merge and the Dictionary is valid XML.      */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @CurrentProjectId, @NewProjectId, @Dictionary                */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_NO_PIN_DCF, ERR_DICT_INVAL, ERR_CASE_IN_USE,ERR_PIN_BEING_MERGED            */
        /*                ERR_INVALID_PARAMETERS                                                          */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 18/05/2016     0.1      Ray Banister	     Check for the two ProjectIds being the same added    */
        /* 25/05/2016     0.2      Ray Banister      Now check for ''pending'' PIN                          */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_ChangePrimaryProject]
            @PIN INT = NULL,
            @CurrentProjectId INT = NULL,
            @NewProjectId INT = NULL,
            @Dictionary NVARCHAR (MAX) = NULL
        AS
        BEGIN
            -- SET NOCOUNT ON added to prevent extra result sets from
            -- interfering with SELECT statements.
            SET NOCOUNT ON;
            DECLARE @RESULT NVARCHAR(300) = ''''
            DECLARE @PINExists AS BIT = 0
            DECLARE @IsValid AS BIT = 0
            DECLARE @IsMergeFromPIN AS BIT = 0
        	DECLARE @PINRequested AS BIT = ''False''

            -- Check all Parameters are there
            IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR
                (@CurrentProjectId IS NULL) OR (ISNUMERIC(@CurrentProjectId) = 0) OR
                (@NewProjectId IS NULL) OR (ISNUMERIC(@NewProjectId) = 0) OR
                (NULLIF(@Dictionary,'''')IS NULL )
            BEGIN
                RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
                RETURN
            END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

        		-- Check NewProjectId <> CurrentProjectId as that would be a bit daft
            IF @NewProjectId = @CurrentProjectId
            BEGIN
                RAISERROR(60016,16,0)		-- ERR_NEW_PROJECT_EQUALS_CURRENT_PROJECT
                RETURN
            END



            -- Check if PIN is in use for an earlier ''merge'' as the FromPin - This is an error if it does not exist
            EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPIN OUTPUT
            IF @IsMergeFromPIN = 1
            BEGIN
                RAISERROR(60012,16,0)		-- ERR_PIN_BEING_MERGED
                RETURN
            END

            -- Check Dictionary XML is valid.
            EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
            IF @IsValid = 0
            BEGIN
                RAISERROR(60007,16,0)		-- ERR_DICT_INVAL
                RETURN
            END


            -- All preliminary checks are OK so write the Task to the Task Table
            INSERT INTO dbo.Task (PIN,Dictionary, CurrentProjectId,NewProjectId,
                					Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
            VALUES (@PIN,@Dictionary,@CurrentProjectId,@NewProjectId,
                		''OperationsHandler'',''ChangePrimaryProject'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_RestrictUser                                                              */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Restrict a User                                                               */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED                    */
        /*                                                                                                */
        /* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
        /* and the PIN = FromPIN in that MergePIN request.                                     */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 25/05/2016     0.1      Ray Banister      Now check for ''pending'' PIN                          */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        Create PROCEDURE [dbo].[sp_eAspire_RestrictUser]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = 0
        	DECLARE @IsMergeFromPin AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''

        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

        -- Check if there is an earlier ''merge'' task with the same PIN as the FromPIN
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
        	IF @IsMergeFromPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END


        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,''OperationsHandler'',''RestrictUser'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_RemoveRestrictedUser                                                   */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Remove Restricted User                                                            */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_PIN_BEING_MERGED                    */
        /*                                                                                                */
        /* ERR_PIN_BEING_MERGED will be raised IF there is an outstanding (i.e. earlier) MergePIN task    */
        /* and the PIN = FromPIN in that MergePIN request.                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 25/05/2016     0.1      Ray Banister      Now check for ''pending'' PIN                          */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        Create PROCEDURE [dbo].[sp_eAspire_RemoveRestrictedUser]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = 0
        	DECLARE @IsMergeFromPin AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''


        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

        -- Check if there is an earlier ''merge'' task with the same PIN as the FromPIN
        	EXEC dbo.sp_ValidateIsPIN_MergeFromPIN @PIN, @IsMergeFromPin OUTPUT
        	IF @IsMergeFromPin = ''True''
        		BEGIN
        			RAISERROR(60012,16,0)	-- PIN being Merged
        			RETURN
        		END


        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,''OperationsHandler'',''RemoveRestrictedUser'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_UpdatePINWithDictionaryValues                                          */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Update the PIN’s Dictionary data with new values                                  */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN, @Dictionary                                                  */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_PIN_DCF, ERR_DICT_INVAL                          */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 16/05/2016     0.1      Ray Banister	     Method name corrected                                */
        /* 25/05/2016     0.2      Ray Banister      Now check for ''pending'' PIN and Case                 */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_UpdatePINWithDictionaryValues]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL,
        	@Dictionary NVARCHAR (MAX) = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @PINExists AS BIT = ''False''
        	DECLARE @IsValid AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''


        	-- Check all Parameters are there
        	IF (@PIN is null) OR (ISNUMERIC(@PIN) = 0) OR
        		(NULLIF(@Dictionary,'''')IS NULL )
        	BEGIN
        		RAISERROR(60000,16,1)		-- Error in Parameters
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

        	-- Check Dictionary XML is valid.
        	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
        	IF @IsValid = 0
        	BEGIN
        		RAISERROR(60007,16,0)	-- ERR_DICT_INVAL
        		RETURN
        	END

        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,Dictionary,
        							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@Dictionary,
        				''OperationsHandler'',''UpdatePinWithDictionaryValues'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_UpdateCaseWithDictionaryValues                                         */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Update the Case’s Dictionary data with new values                                 */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @CaseId, @Dictionary, @PIN                                         */
        /* Optional Input Parameters:                                                                     */
        /* Returns:                                                                                       */
        /* Errors Raised: ERR_INVALID_PARAMETERS, ERR_NO_CASE_DCF, ERR_DICT_INVAL. ERR_NO_PIN_DCF         */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 16/05/2016     0.1      Ray Banister	     Method name corrected                                */
        /* 25/05/2016     0.2      Ray Banister      Now check for ''pending'' PIN and Case                 */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_UpdateCaseWithDictionaryValues]
        	-- Add the parameters for the stored procedure here
        	@PIN INT = NULL,
        	@CaseId INT = NULL,
        	@Dictionary NVARCHAR (MAX) = NULL
        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @CaseIdInUse AS BIT = ''False''
        	DECLARE @PINExists AS BIT = ''False''
        	DECLARE @IsValid AS BIT = ''False''
        	DECLARE @PINRequested AS BIT = ''False''
        	DECLARE @CaseIdRequested AS BIT = ''False''


        	-- Check all Parameters are there
        	IF (@PIN is NULL) OR (ISNUMERIC(@PIN) = 0) OR
        		(@CaseId is NULL) OR (ISNUMERIC(@CaseId) = 0) OR
        		(NULLIF(@Dictionary,'''')IS NULL )
        	BEGIN
        		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
        		RETURN
        	END

            -- Check if PIN already in use [Site Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidatePINExists @PIN, @PINExists OUTPUT
            IF @PINExists = ''False''
            BEGIN
                EXEC dbo.sp_ValidatePINRequested @PIN, @PINRequested OUTPUT
                IF @PINRequested = ''False''
                BEGIN
                	RAISERROR(60004,16,0)	-- PIN not found nor awaiting allocation
                	RETURN
                END
            END

            -- Check if Case already in use [Library Table] - This is an error if it does not exist
            -- or if it is not pending allocation
            EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
            IF @CaseIdInUse = ''False''
            BEGIN
        		EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
            	IF @CaseIdRequested = ''False''
        		BEGIN
        			RAISERROR(60006,16,0)	-- Case not found nor awaiting allocation [ERR_NO_CASE_DCF]
                    RETURN
        		END
            END


        	-- Check Dictionary XML is valid.
        	EXEC dbo.sp_ValidateDictionary @Dictionary, @IsValid OUTPUT
        	IF @IsValid = 0
        	BEGIN
        		RAISERROR(60007,16,0)		-- ERR_DICT_INVAL
        		RETURN
        	END

        	-- All preliminary checks are OK so write the Task to the Task Table
        	INSERT INTO dbo.Task (PIN,CaseId,Dictionary,
        							Handler,Name,Frequency,InsertedDate,InsertedBy,UpdatedDate,UpdatedBy)
        	VALUES (@PIN,@CaseId,@Dictionary,
        				''OperationsHandler'',''UpdateCaseWithDictionaryValues'',''O'',GETDATE(),SYSTEM_USER,GETDATE(),SYSTEM_USER)

        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_eAspire_CaseUrl                                                                */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 05/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Retrive the URL for a specified Case                                              */
        /*                                                                                                */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @CaseId                                                            */
        /* Optional Input Parameters:  NONE                                                               */
        /* Returns: @Url [OUT], @StatusId [OUT]                                                           */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 05/05/2016     0.0      Ray Banister	     First implementation                                 */
        /* 16/05/2016     0.1      Ray Banister	     The SP was incorrectly raising ERR_NO_CASE_DCF.      */
        /*                                           The spec does not require this, you just set StausId */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_eAspire_CaseUrl]
        	@CaseId INT,
        	@Url NVARCHAR(2000) OUT,
        	@StatusId INT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	DECLARE @RESULT NVARCHAR(300) = ''''
        	DECLARE @CaseIdInUse BIT = ''False''
        	DECLARE @CaseIdRequested BIT = ''False''

        	-- Check all Parameters are there
        	IF (@CaseId is null) OR (ISNUMERIC(@CaseId) = 0)
        	BEGIN
        		RAISERROR(60000,16,1)		-- ERR_INVALID_PARAMETERS
        		RETURN
        	END

        	-- Check if Case already allocated
        	EXEC dbo.sp_ValidateCaseIdInUse @CaseId, @CaseIdInUse OUTPUT
        	IF @CaseIdInUse = ''True''
        	BEGIN
        		-- Return "Initialised" and the Url
        		SET @Url = (SELECT Url FROM dbo.Library WHERE CaseId = @CaseId)
        		SET @StatusId = 2
        		RETURN
        	END

        	-- Check if Case awaiting allocation
        	EXEC dbo.sp_ValidateCaseIdRequested @CaseId, @CaseIdRequested OUTPUT
        	IF @CaseIdRequested = ''True''
        	BEGIN
        		-- Return "Initialising" and a NULL Url
        		SET @Url = NULL
        		SET @StatusId = 1
        		RETURN
        	END

        	-- PIN not found and not awaiting allocation. Set "Undeclared"
        	BEGIN
        		SET @Url = NULL
        		SET @StatusId = 0
        		RETURN
        	END


        END

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        sp_ValidateIsPIN_AwaitingDeletion                                                 */
        /* Author:		Ray Banister                                                                      */
        /* Create date: 17/08/2016                                                                        */
        /*                                                                                                */
        /* Description:	Checks if the selected PIN is awaiting a DeletePIN request                        */
        /*              but has not yet been processed.                                                   */
        /*		                                                                                          */
        /* Mandatory Input Parameters: @PIN                                                               */
        /* Optional Input Parameters:  None                                                               */
        /* Returns: @PINAwaitingDeletion as an OUT parameter                                              */
        /* Errors Raised: None                                                                            */
        /*                                                                                                */
        /*                                                                                                */
        /*                                                                                                */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 17/08/2016     0.0      Ray Banister	     First implementation                                 */
        /*                                                                                                */
        /* ============================================================================================== */

        CREATE PROCEDURE [dbo].[sp_ValidateIsPIN_AwaitingDeletion]
        	@PIN INT,
        	@PINAwaitingDeletion BIT OUT

        AS
        BEGIN
        	-- SET NOCOUNT ON added to prevent extra result sets from
        	-- interfering with SELECT statements.
        	SET NOCOUNT ON;
        	IF EXISTS(SELECT  1 FROM dbo.Task
        		WHERE (Handler = ''OperationsHandler'' AND Name = ''DeletePin'' AND Pin = @PIN AND InsertedDate <= GETDATE()))
        		SET @PINAwaitingDeletion = ''True''
        	ELSE SET @PINAwaitingDeletion = ''False''
        END

    ')
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'201605061010222_AddeAspireStoredProcedures', N'Fujitsu.AFC.Data.Migrations.AFC.Configuration',  0x1F8B0800000000000400ED5D5B6FE4B6157E2FD0FF20E8A92D361EDB8B16AD314EE01DAF13A3F6DAD8B183BE2D68899E65A29126BAB81E04F9657DE84FEA5F28A92B6FA2484AA3B11D21C0C6438A1F0FC9730E0F6FE7FCEF3FFF9D7FF7BC0E9C271827280A4FDDA38343D781A117F9285C9DBA59FAF8CDDFDDEFBEFDE31FE61FFDF5B3F363F5DD7BF21D2E1926A7EED734DD9CCC6689F715AE4172B0465E1C25D1637AE045EB19F0A3D9F1E1E13F6647473388215C8CE538F3CF5998A235CC7FE09F8B28F4E026CD40701DF93048CA749CB3CC519D4F600D930DF0E0A97B91FD84D2243B38BB581C9C8314B8CE598000A663098347D7016118A520C5549EDC277099C651B85A6E700208EEB61B88BF7B0441024BEA4F9ACF751B72784C1A326B0A56505E96A4D1DA10F0E87DD93333BEB855FFBA75CFE1BEFB88FB38DD9256E7FD77EA5EA18718C45BD7E1EB3A590431F98EEDDD7C2C0ECA42EF1C21EB5DCD0F986DC87FEF9C4516A4590C4F4398A531C05FDC660F01F2FE09B777D1CF303C0DB320A069C454E23C260127DDC6D106C6E9F6337C2C29BFF45D67C6969BF105EB625499A2519761FAFED8753EE1CAC143006B16A03A60994631FC1E86300629F46F419AC238241830EF44A176AEAE254A61777D6A8C05487A63DCA134801504667E2CC5AE730D9EAF60B84ABF9EBAF84FD7B940CFD0AF524AD8FB1061A1C785D238EBAC05FFFC097A695F622F93DB18AD316F919657501FA22880203406BB420945D0F719F22DE859045102FDBEA4DCC7C1CE87E0324CF02FE8630D58771DF9FB0E6B55F38697601FB60ABA8FFE7AA845B769676D7C30543B4AAC519AF1093CA155AE3B24AAC0753EC320CF4CBEA24D3135558AF44BF1C1451CAD3F4741A395F3F42FCB288B3DD2139124F30EC42B98B2A4CC678D9A572AFFA25A13CD4F4A4C6A7FDF923E924A47613F658E7F3E21C215D01F62329CF4DB4BD56FDC404B555DAEAD840F1B9527CB17549FF4A35E2A50A0C8441B728527C5A836A5CE8220F248D9BED614F957C1EFEFFFB613A91D41B1730CB58882001BD5F8F7A43B7F27BAB31972A916E5759FA274A35AB50B09FA56BFE4904A986E450F75DCC04C8AB98736D5148049FFBC5CFDA32F8820C6FFC7EC63287855B149D0762F68864CF62308B221D68A86D59EC3C48BD1A650E2AD951FEFA6CD93B67915DAE60784554CBCBD8A5666EAA62937E91BE53611487EEEBB70F8F884EB22003B5FFBE4359DC314A0DDAFB316D17A13C0A1244BBAB9D5D1D8C403817429AC2EF70308FD804CCF23CF227B99BA2E62F84B06434FA5B176347D897B9F76C75F5AA53A3AF7E870080B5C7A94D8CDD75D5BCDC311374E4DE7285F11E647DEBB3E8FABCE320D657C91C5315685762CF509FEDBAE20D92E30E5FABBC8B444D9360B66C40DB328355982ED582FC81224968A990D484A4CD65F0F1BA74BDA9E5372CFCACF023B000B9B68B26D26DB66B26D26DB46A19426DB66B26D5E9D6D839B115F459EA98153159BAC1C555DA487A08FB93705A157B38FD55DCF02AAF7ADA72176DD26D1DEA768AB8EEB8B958AE464BE96D72FC527CDF13B9B239CB173D92607E96749127928A7447E92AEB81FC0B6F763E83BD697058AC1905C4FC04383D512DA604584A93F75FF22F4B34DBD550FB5D74B5F2060293872799577139E43B24C72CEBCE285069E893DE08B2C81C7C06753B0968464C6472058605EC07A1785A9A85251E8A10D086CDBC9016A6A6942705D359F730E3730240AD676CC756852DE651289AD69E27ABDAB93E7334A0AD4C221BD82D8C68FEAFB880DEB99F2B9F20AA3BE28ED9F91550D19816755E363C19E7B634AE609401BD3C8DF0334CC52BFE2D2E743E92B820EBEDE3FD3C9A81E81D964FDAF53ED5E398BB348DA18A1CD3C6958815AC1E8F3578B5943C3E676D24BE33039DD23F0987C1C742AAE561C2370596187E232292E01E39200BC3025CF6E49327C4E25EBDCFB04964BDDA4B4C079B621B04B98324A11C1C4751ACB97D77502EBB11844F664E50BC5D651989B1D6438C2046D06D9D8431AE0B441DB554D75174F0ADBDCEFEB80213CD5DCB39161D1B77734C064108506E82A5CC98514A1514D1C0CC5C0AD63A0BA3E4D9537BE7BCDCB619FD555DD1D32A61424BECF7AAABD26865779B5CB76B4C620C85F0289FDDD6DAEEB1BEC54DBBABB4E69A21B8C8745DFB00F42C53E69B716BBED458A724ABF2AFA416A2276F5A445A3F9AD13B1D92A5346C798A188A6D589A2ED2DE60B0D542835EDD6579B38F5E459E7CD67854B8B32613E6BF17D31BF069B0D0A57942F8C32C559168E3016DF2CCD7D44AC0B8C9997485C45D4D4D63561A50F5690CB25C7CD3EBC407192124BE0217FBDBFF0D7C2679CA9D0A2FAABCA786B401CAF6A3EA84A90BFCB431FCE29486D3888265559F802376C4D0CB27C4F5A10174949877823010188253BE0586966EBB0DD346C2F5DAD1A6884B695443B4A75A244A35469FA28E5C9250D5226E963508776340E95AC8FC53988603A99CDD2C7ACFC44D060559A096595AB0896A82A551F297F694883E409069430A7070C354C8E3922D97B97E17D10CC7165FBE84301A69D7486311E4F1C952C62CD679CD00B6B2741B708CB4D565969A9B262CAECA5C66453BF860E9317DB8D02EBCBC083281D728EC7A81B242C9E94E5C5FD414E79756D1F2A7A7712D0972AA0824DDF4B563B562F1A62DB89B01B09661EEBB3F31995A18F575C03A3818A94F1748AF2384A21D9EA732B450F4E32FE4A649CDEDD1852DADB3784CCE55E85B51B0DD05F62270978B912D06C12F7E3F87A3FD982C3DBCBBE548E2EDF52D31065923E06F3309A46623226297B0352C69FA1F41235EAB8C55CD6548577236CD59924B3866B39A76C47A15E1FD34054B22156F5BE5840AB320CF6D7D8F735CC361B9B35F6CE1FF5B898696793AC8F553FCAA191EAC431952FF5628686A19247DD1C186C47F3967E28234133EDA761F67DA9F7313C331A6FC5502F6078A28CB1E8372ECCEC49A55BEC26B7EC241BB55278DAC23456C835901DE6DD0B23434C8E892C950F6258492A130DB47C24A09449C6FD26655B36CBA8C7647854F26460BD1103ABA759253B4FD630A8E4C576634AEDC2D090BCEF650545C81EDB88990C8FC9F0980C0FAD564E86C7647818214E86079F6F6A783497C8FA591FF5F5550B13A4BDEC6EEC10FEA92D735D86CB334515A4994A1E7BD36992BB3DCB9D705392FFA4AEBD4CA97FD73725CB5B8ADDA1C3846B8BC527AE931FC0F9E4CAE2729BA4705D5EBEF925580408B7B7F9E01A84E8112669F1CEDE3D3E3C3AE6E28FBD9C5860B324F103C92D4FCA5D81F4AEE318BE0210E9D44E6F00BD5C33E655088F532E431F3E9FBABFE6454E9CCB7F7D294ABD736E623CBE27CEA1F35BCFC85D1D155F26F721C28B8913E72ECEA0456D8C7F99F009C4DE5710FF690D9EFF4C4359C5F1CA491F228AD7033207622378657927A19C171E115906F68CE66543121506A25747CB5C3C10859B0EE3E2A1A24DF48ED0DF838335998207074B2ACD025CBD0D4D3614DF0DA82A1A4F29BBD66FB7E26D4B338D2E00F452EE93EC8E20BBB7AA9B8FAF568C2551906C66A1626F4E31102406D29E344CEBD5B5DE624B434D02FCCA04B8D5F7D06B1565B90C9ACC829C97D1C9807C793C2CBFA438F1ACC8B3E6D1685A6719DB0833526E38B6207392AD1164ABED52E2AB152ED6F3A3CDC689106DA597212689A8D20B4FEA51DC9ADF240B39C3E6F1315438435A0783F3392E65790BB9EC34CF2D30058FE1FDD427BF7A1F769BD000A3B3AF72E7D8869C2AEEBE9A0108EEC187A46C27C0A2F3EF7EDB93BC836F0BE16A73F66D311E32D7DF16309C23700B04C62DB84579A99370BB0EE98D31593923583912DF7BAFD5BED19EFFF518B825C2882D606F7B62B205265BA08DB1265B60B205265B60B205860A05F2360C0279F08DDED73484401C365B2992CD188D03A7A2D474B6F4C22469844017B4EFD7A20D7B8C2CD14A65BF681B4304AC507A35D1D15A4E8FC814BF87C0133D467C473C2890311EB78D1549E2ED858CD01AB41D314CEDA29DAA5E3B4485753092110341BC9DE00FB9C3656A98761E9441E262D936D8845D7C07F983DFC13945F5B447AC6CEF211C44F7CDFCB0C922346C55F1198A9714D8AA7C88F00817369F49F086D6D80D32609B100C3A411D6475091F99554BDB905D04D0DF6A90621327421D26425AA97614093A4684500B9D29ABC630C0446B7C0919B659E40975E00929FEEB8E4B21B2688753425171290C5A857B5361FE7829D1257A34426296F17E99FB377BA0C0115AE4C96D3DAA509B96DF6F68084A47343E3C544D13E49F7B77ABDF3C83D80FE2CB453CD76721D996297E9DC304AD1A8839C60C0B296C40AB6F2EC3C7A83236388AAA4FB89D9C6B98021F9B0067718A1E814736EC3D9824799CD5FCF2DEA9FB71FD4036FD6EB27493A5B8C970FD1030634D8C1655FD79800B96E6F94D7E912F19A209984C44B6A36EC20F190AFC9AEE0BC97E530B04B186CA7D52329629D92F5D6D6BA44F58D4F580CAEEAB8DB83BB8DE903B4BC94DB8044FD086B6FB045EC115F0B6B7E503D47690EE8160BB7D7E8EC00ACFED4989D194C73F310FFBEBE76FFF0FD9145EEC21A80000 , N'6.1.3-40302')
END

IF @CurrentMigration < '201605061011127_AddeAspireViews'
BEGIN
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        vw_eAspireCase                                                                    */
        /* Author:		Jonathan King                                                                     */
        /* Create date: 03/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Retrieve the details about created and pending PIN creations    				  */
        /*                                                                                                */
        /*		                                                                                          */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 03/05/2016     0.0      Jonathan King         First implementation                             */
        /*                                                                                                */
        /* ============================================================================================== */


        CREATE VIEW [dbo].[vw_eAspireCase]
        AS
        	SELECT PIN as [PIN], CaseId as [CaseID], 2 as [Status]
        	FROM dbo.Site WITH (NOLOCK) INNER JOIN
        		dbo.Library WITH (NOLOCK) ON dbo.Site.Id = dbo.Library.SiteId
        		UNION
        			-- Combine Results with Pending ''AllocateCase'' requests
        			SELECT PIN as [PIN], CaseId as [CaseID], 1 as [Status]
        			FROM dbo.Task WITH (NOLOCK)
        			WHERE (Handler = ''OperationsHandler'' AND Name = ''AllocateCase'')

    ')
    EXECUTE('
        /* ============================================================================================== */
        /* Name:        vw_eAspireCase                                                                    */
        /* Author:		Jonathan King                                                                     */
        /* Create date: 03/05/2016                                                                        */
        /*                                                                                                */
        /* Description:	Retrieve the details about created and pending PIN creations    				  */
        /*                                                                                                */
        /*		                                                                                          */
        /* Copyright 2016 Fujitsu Services Ltd                                                            */
        /* ============================================================================================== */
        /*                                        Change Record                                           */
        /* ============================================================================================== */
        /*    Date      Version     Changed By                     Comments                               */
        /* ============================================================================================== */
        /* 03/05/2016     0.0      Jonathan King         First implementation                             */
        /* 05/05/2016	  0.1	   Ray Banister          Method name corrected                            */
        /* ============================================================================================== */


        CREATE VIEW [dbo].[vw_eAspirePIN]
        AS
        	SELECT PIN AS [PIN], 2 AS [Status]
        	FROM dbo.Site  WITH (NOLOCK)
        		UNION
        		-- Combine Results with Pending ''AllocatePin'' requests
        		SELECT PIN AS [PIN], 1 AS [Status]
        		FROM dbo.Task  WITH (NOLOCK)
        		WHERE (Handler = ''OperationsHandler'' AND Name = ''AllocatePin'')

    ')
    INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
    VALUES (N'201605061011127_AddeAspireViews', N'Fujitsu.AFC.Data.Migrations.AFC.Configuration',  0x1F8B0800000000000400ED5D5B6FE4B6157E2FD0FF20E8A92D361EDB8B16AD314EE01DAF13A3F6DAD8B183BE2D68899E65A29126BAB81E04F9657DE84FEA5F28A92B6FA2484AA3B11D21C0C6438A1F0FC9730E0F6FE7FCEF3FFF9D7FF7BC0E9C271827280A4FDDA38343D781A117F9285C9DBA59FAF8CDDFDDEFBEFDE31FE61FFDF5B3F363F5DD7BF21D2E1926A7EED734DD9CCC6689F715AE4172B0465E1C25D1637AE045EB19F0A3D9F1E1E13F6647473388215C8CE538F3CF5998A235CC7FE09F8B28F4E026CD40701DF93048CA749CB3CC519D4F600D930DF0E0A97B91FD84D2243B38BB581C9C8314B8CE598000A663098347D7016118A520C5549EDC277099C651B85A6E700208EEB61B88BF7B0441024BEA4F9ACF751B72784C1A326B0A56505E96A4D1DA10F0E87DD93333BEB855FFBA75CFE1BEFB88FB38DD9256E7FD77EA5EA18718C45BD7E1EB3A590431F98EEDDD7C2C0ECA42EF1C21EB5DCD0F986DC87FEF9C4516A4590C4F4398A531C05FDC660F01F2FE09B777D1CF303C0DB320A069C454E23C260127DDC6D106C6E9F6337C2C29BFF45D67C6969BF105EB625499A2519761FAFED8753EE1CAC143006B16A03A60994631FC1E86300629F46F419AC238241830EF44A176AEAE254A61777D6A8C05487A63DCA134801504667E2CC5AE730D9EAF60B84ABF9EBAF84FD7B940CFD0AF524AD8FB1061A1C785D238EBAC05FFFC097A695F622F93DB18AD316F919657501FA22880203406BB420945D0F719F22DE859045102FDBEA4DCC7C1CE87E0324CF02FE8630D58771DF9FB0E6B55F38697601FB60ABA8FFE7AA845B769676D7C30543B4AAC519AF1093CA155AE3B24AAC0753EC320CF4CBEA24D3135558AF44BF1C1451CAD3F4741A395F3F42FCB288B3DD2139124F30EC42B98B2A4CC678D9A572AFFA25A13CD4F4A4C6A7FDF923E924A47613F658E7F3E21C215D01F62329CF4DB4BD56FDC404B555DAEAD840F1B9527CB17549FF4A35E2A50A0C8441B728527C5A836A5CE8220F248D9BED614F957C1EFEFFFB613A91D41B1730CB58882001BD5F8F7A43B7F27BAB31972A916E5759FA274A35AB50B09FA56BFE4904A986E450F75DCC04C8AB98736D5148049FFBC5CFDA32F8820C6FFC7EC63287855B149D0762F68864CF62308B221D68A86D59EC3C48BD1A650E2AD951FEFA6CD93B67915DAE60784554CBCBD8A5666EAA62937E91BE53611487EEEBB70F8F884EB22003B5FFBE4359DC314A0DDAFB316D17A13C0A1244BBAB9D5D1D8C403817429AC2EF70308FD804CCF23CF227B99BA2E62F84B06434FA5B176347D897B9F76C75F5AA53A3AF7E870080B5C7A94D8CDD75D5BCDC311374E4DE7285F11E647DEBB3E8FABCE320D657C91C5315685762CF509FEDBAE20D92E30E5FABBC8B444D9360B66C40DB328355982ED582FC81224968A990D484A4CD65F0F1BA74BDA9E5372CFCACF023B000B9B68B26D26DB66B26D26DB46A19426DB66B26D5E9D6D839B115F459EA98153159BAC1C555DA487A08FB93705A157B38FD55DCF02AAF7ADA72176DD26D1DEA768AB8EEB8B958AE464BE96D72FC527CDF13B9B239CB173D92607E96749127928A7447E92AEB81FC0B6F763E83BD697058AC1905C4FC04383D512DA604584A93F75FF22F4B34DBD550FB5D74B5F2060293872799577139E43B24C72CEBCE285069E893DE08B2C81C7C06753B0968464C6472058605EC07A1785A9A85251E8A10D086CDBC9016A6A6942705D359F730E3730240AD676CC756852DE651289AD69E27ABDAB93E7334A0AD4C221BD82D8C68FEAFB880DEB99F2B9F20AA3BE28ED9F91550D19816755E363C19E7B634AE609401BD3C8DF0334CC52BFE2D2E743E92B820EBEDE3FD3C9A81E81D964FDAF53ED5E398BB348DA18A1CD3C6958815AC1E8F3578B5943C3E676D24BE33039DD23F0987C1C742AAE561C2370596187E232292E01E39200BC3025CF6E49327C4E25EBDCFB04964BDDA4B4C079B621B04B98324A11C1C4751ACB97D77502EBB11844F664E50BC5D651989B1D6438C2046D06D9D8431AE0B441DB554D75174F0ADBDCEFEB80213CD5DCB39161D1B77734C064108506E82A5CC98514A1514D1C0CC5C0AD63A0BA3E4D9537BE7BCDCB619FD555DD1D32A61424BECF7AAABD26865779B5CB76B4C620C85F0289FDDD6DAEEB1BEC54DBBABB4E69A21B8C8745DFB00F42C53E69B716BBED458A724ABF2AFA416A2276F5A445A3F9AD13B1D92A5346C798A188A6D589A2ED2DE60B0D542835EDD6579B38F5E459E7CD67854B8B32613E6BF17D31BF069B0D0A57942F8C32C559168E3016DF2CCD7D44AC0B8C9997485C45D4D4D63561A50F5690CB25C7CD3EBC407192124BE0217FBDBFF0D7C2679CA9D0A2FAABCA786B401CAF6A3EA84A90BFCB431FCE29486D3888265559F802376C4D0CB27C4F5A10174949877823010188253BE0586966EBB0DD346C2F5DAD1A6884B695443B4A75A244A35469FA28E5C9250D5226E963508776340E95AC8FC53988603A99CDD2C7ACFC44D060559A096595AB0896A82A551F297F694883E409069430A7070C354C8E3922D97B97E17D10CC7165FBE84301A69D7486311E4F1C952C62CD679CD00B6B2741B708CB4D565969A9B262CAECA5C66453BF860E9317DB8D02EBCBC083281D728EC7A81B242C9E94E5C5FD414E79756D1F2A7A7712D0972AA0824DDF4B563B562F1A62DB89B01B09661EEBB3F31995A18F575C03A3818A94F1748AF2384A21D9EA732B450F4E32FE4A649CDEDD1852DADB3784CCE55E85B51B0DD05F62270978B912D06C12F7E3F87A3FD982C3DBCBBE548E2EDF52D31065923E06F3309A46623226297B0352C69FA1F41235EAB8C55CD6548577236CD59924B3866B39A76C47A15E1FD34054B22156F5BE5840AB320CF6D7D8F735CC361B9B35F6CE1FF5B898696793AC8F553FCAA191EAC431952FF5628686A19247DD1C186C47F3967E28234133EDA761F67DA9F7313C331A6FC5502F6078A28CB1E8372ECCEC49A55BEC26B7EC241BB55278DAC23456C835901DE6DD0B23434C8E892C950F6258492A130DB47C24A09449C6FD26655B36CBA8C7647854F26460BD1103ABA759253B4FD630A8E4C576634AEDC2D090BCEF650545C81EDB88990C8FC9F0980C0FAD564E86C7647818214E86079F6F6A783497C8FA591FF5F5550B13A4BDEC6EEC10FEA92D735D86CB334515A4994A1E7BD36992BB3DCB9D705392FFA4AEBD4CA97FD73725CB5B8ADDA1C3846B8BC527AE931FC0F9E4CAE2729BA4705D5EBEF925580408B7B7F9E01A84E8112669F1CEDE3D3E3C3AE6E28FBD9C5860B324F103C92D4FCA5D81F4AEE318BE0210E9D44E6F00BD5C33E655088F532E431F3E9FBABFE6454E9CCB7F7D294ABD736E623CBE27CEA1F35BCFC85D1D155F26F721C28B8913E72ECEA0456D8C7F99F009C4DE5710FF690D9EFF4C4359C5F1CA491F228AD7033207622378657927A19C171E115906F68CE66543121506A25747CB5C3C10859B0EE3E2A1A24DF48ED0DF838335998207074B2ACD025CBD0D4D3614DF0DA82A1A4F29BBD66FB7E26D4B338D2E00F452EE93EC8E20BBB7AA9B8FAF568C2551906C66A1626F4E31102406D29E344CEBD5B5DE624B434D02FCCA04B8D5F7D06B1565B90C9ACC829C97D1C9807C793C2CBFA438F1ACC8B3E6D1685A6719DB0833526E38B6207392AD1164ABED52E2AB152ED6F3A3CDC689106DA597212689A8D20B4FEA51DC9ADF240B39C3E6F1315438435A0783F3392E65790BB9EC34CF2D30058FE1FDD427BF7A1F769BD000A3B3AF72E7D8869C2AEEBE9A0108EEC187A46C27C0A2F3EF7EDB93BC836F0BE16A73F66D311E32D7DF16309C23700B04C62DB84579A99370BB0EE98D31593923583912DF7BAFD5BED19EFFF518B825C2882D606F7B62B205265BA08DB1265B60B205265B60B205860A05F2360C0279F08DDED73484401C365B2992CD188D03A7A2D474B6F4C22469844017B4EFD7A20D7B8C2CD14A65BF681B4304AC507A35D1D15A4E8FC814BF87C0133D467C473C2890311EB78D1549E2ED858CD01AB41D314CEDA29DAA5E3B4485753092110341BC9DE00FB9C3656A98761E9441E262D936D8845D7C07F983DFC13945F5B447AC6CEF211C44F7CDFCB0C922346C55F1198A9714D8AA7C88F00817369F49F086D6D80D32609B100C3A411D6475091F99554BDB905D04D0DF6A90621327421D26425AA97614093A4684500B9D29ABC630C0446B7C0919B659E40975E00929FEEB8E4B21B2688753425171290C5A857B5361FE7829D1257A34426296F17E99FB377BA0C0115AE4C96D3DAA509B96DF6F68084A47343E3C544D13E49F7B77ABDF3C83D80FE2CB453CD76721D996297E9DC304AD1A8839C60C0B296C40AB6F2EC3C7A83236388AAA4FB89D9C6B98021F9B0067718A1E814736EC3D9824799CD5FCF2DEA9FB71FD4036FD6EB27493A5B8C970FD1030634D8C1655FD79800B96E6F94D7E912F19A209984C44B6A36EC20F190AFC9AEE0BC97E530B04B186CA7D52329629D92F5D6D6BA44F58D4F580CAEEAB8DB83BB8DE903B4BC94DB8044FD086B6FB045EC115F0B6B7E503D47690EE8160BB7D7E8EC00ACFED4989D194C73F310FFBEBE76FFF0FD9145EEC21A80000 , N'6.1.3-40302')
END