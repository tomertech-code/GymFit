using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymFit.Infrastructure.Migrations;

public partial class AddGymManagementModules : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[DietPlans]', N'U') IS NULL
BEGIN
    CREATE TABLE [DietPlans] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [MemberId] int NOT NULL,
        [TrainerId] int NULL,
        [Name] nvarchar(150) NOT NULL,
        [Goal] nvarchar(100) NOT NULL,
        [DailyCalories] int NOT NULL,
        [ProteinGrams] int NOT NULL,
        [CarbsGrams] int NOT NULL,
        [FatGrams] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NULL,
        [IsActive] bit NOT NULL CONSTRAINT [DF_DietPlans_IsActive] DEFAULT (1),
        [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_DietPlans_CreatedAt] DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_DietPlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DietPlans_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DietPlans_Trainers_TrainerId] FOREIGN KEY ([TrainerId]) REFERENCES [Trainers]([Id]) ON DELETE SET NULL
    );
    CREATE INDEX [IX_DietPlans_MemberId_IsActive] ON [DietPlans] ([MemberId], [IsActive]);
    CREATE INDEX [IX_DietPlans_TrainerId] ON [DietPlans] ([TrainerId]);
END
IF OBJECT_ID(N'[DietMeals]', N'U') IS NULL
BEGIN
    CREATE TABLE [DietMeals] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [DietPlanId] int NOT NULL,
        [MealOrder] int NOT NULL,
        [MealType] nvarchar(60) NOT NULL,
        [FoodItems] nvarchar(1000) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [Calories] int NULL,
        CONSTRAINT [PK_DietMeals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DietMeals_DietPlans_DietPlanId] FOREIGN KEY ([DietPlanId]) REFERENCES [DietPlans]([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_DietMeals_DietPlanId_MealOrder] ON [DietMeals] ([DietPlanId], [MealOrder]);
END
IF OBJECT_ID(N'[ProgressRecords]', N'U') IS NULL
BEGIN
    CREATE TABLE [ProgressRecords] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [MemberId] int NOT NULL,
        [RecordDate] datetime2 NOT NULL,
        [WeightKg] decimal(6,2) NULL,
        [BodyFatPercentage] decimal(5,2) NULL,
        [MuscleMassKg] decimal(6,2) NULL,
        [StrengthScore] decimal(8,2) NULL,
        [Notes] nvarchar(1000) NULL,
        CONSTRAINT [PK_ProgressRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ProgressRecords_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_ProgressRecords_MemberId_RecordDate] ON [ProgressRecords] ([MemberId], [RecordDate]);
END
IF OBJECT_ID(N'[BodyMeasurements]', N'U') IS NULL
BEGIN
    CREATE TABLE [BodyMeasurements] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [MemberId] int NOT NULL,
        [MeasurementDate] datetime2 NOT NULL,
        [ChestCm] decimal(6,2) NULL,
        [WaistCm] decimal(6,2) NULL,
        [HipsCm] decimal(6,2) NULL,
        [LeftArmCm] decimal(6,2) NULL,
        [RightArmCm] decimal(6,2) NULL,
        [LeftThighCm] decimal(6,2) NULL,
        [RightThighCm] decimal(6,2) NULL,
        [Notes] nvarchar(1000) NULL,
        CONSTRAINT [PK_BodyMeasurements] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BodyMeasurements_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members]([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_BodyMeasurements_MemberId_MeasurementDate] ON [BodyMeasurements] ([MemberId], [MeasurementDate]);
END
IF OBJECT_ID(N'[UserNotifications]', N'U') IS NULL
BEGIN
    CREATE TABLE [UserNotifications] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Message] nvarchar(2000) NOT NULL,
        [Type] nvarchar(40) NOT NULL,
        [Url] nvarchar(500) NULL,
        [IsRead] bit NOT NULL CONSTRAINT [DF_UserNotifications_IsRead] DEFAULT (0),
        [CreatedAt] datetime2 NOT NULL CONSTRAINT [DF_UserNotifications_CreatedAt] DEFAULT (GETUTCDATE()),
        [ReadAt] datetime2 NULL,
        CONSTRAINT [PK_UserNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserNotifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_UserNotifications_UserId_IsRead_CreatedAt] ON [UserNotifications] ([UserId], [IsRead], [CreatedAt]);
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF OBJECT_ID(N'[UserNotifications]', N'U') IS NOT NULL DROP TABLE [UserNotifications];
IF OBJECT_ID(N'[BodyMeasurements]', N'U') IS NOT NULL DROP TABLE [BodyMeasurements];
IF OBJECT_ID(N'[ProgressRecords]', N'U') IS NOT NULL DROP TABLE [ProgressRecords];
IF OBJECT_ID(N'[DietMeals]', N'U') IS NOT NULL DROP TABLE [DietMeals];
IF OBJECT_ID(N'[DietPlans]', N'U') IS NOT NULL DROP TABLE [DietPlans];
");
    }
}
