namespace EruptiousGamesApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration41 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        CamID = c.Int(nullable: false, identity: true),
                        CamName = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Inventory = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CamID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustID = c.Int(nullable: false, identity: true),
                        CamID = c.Int(nullable: false),
                        EmpID = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        CustName = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        City = c.String(),
                        Age = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        PTCheck = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustID)
                .ForeignKey("dbo.Campaigns", t => t.CamID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmpID, cascadeDelete: true)
                .Index(t => t.CamID)
                .Index(t => t.EmpID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmpID = c.Int(nullable: false, identity: true),
                        EmpName = c.String(),
                        Role = c.Int(nullable: false),
                        EmpStatus = c.Int(nullable: false),
                        DecksOnHand = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EmpID);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        NoteID = c.Int(nullable: false, identity: true),
                        EmpID = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        Title = c.String(),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.NoteID)
                .ForeignKey("dbo.Employees", t => t.EmpID, cascadeDelete: true)
                .Index(t => t.EmpID);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        RequestID = c.Int(nullable: false, identity: true),
                        CamID = c.Int(nullable: false),
                        EmpID = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        Amount = c.Int(nullable: false),
                        Action = c.Int(nullable: false),
                        RequestStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RequestID)
                .ForeignKey("dbo.Campaigns", t => t.CamID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmpID, cascadeDelete: true)
                .Index(t => t.CamID)
                .Index(t => t.EmpID);
            
            CreateTable(
                "dbo.Works",
                c => new
                    {
                        WorkID = c.Int(nullable: false, identity: true),
                        CamID = c.Int(nullable: false),
                        EmpID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Sold = c.Int(nullable: false),
                        CustomerPlayWith = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WorkID)
                .ForeignKey("dbo.Campaigns", t => t.CamID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmpID, cascadeDelete: true)
                .Index(t => t.CamID)
                .Index(t => t.EmpID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Employee_EmpID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.Employee_EmpID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Employee_EmpID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmployeeCampaigns",
                c => new
                    {
                        Employee_EmpID = c.Int(nullable: false),
                        Campaign_CamID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Employee_EmpID, t.Campaign_CamID })
                .ForeignKey("dbo.Employees", t => t.Employee_EmpID, cascadeDelete: true)
                .ForeignKey("dbo.Campaigns", t => t.Campaign_CamID, cascadeDelete: true)
                .Index(t => t.Employee_EmpID)
                .Index(t => t.Campaign_CamID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Employee_EmpID", "dbo.Employees");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Works", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Works", "CamID", "dbo.Campaigns");
            DropForeignKey("dbo.Requests", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Requests", "CamID", "dbo.Campaigns");
            DropForeignKey("dbo.Notes", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Customers", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.EmployeeCampaigns", "Campaign_CamID", "dbo.Campaigns");
            DropForeignKey("dbo.EmployeeCampaigns", "Employee_EmpID", "dbo.Employees");
            DropForeignKey("dbo.Customers", "CamID", "dbo.Campaigns");
            DropIndex("dbo.EmployeeCampaigns", new[] { "Campaign_CamID" });
            DropIndex("dbo.EmployeeCampaigns", new[] { "Employee_EmpID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Employee_EmpID" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Works", new[] { "EmpID" });
            DropIndex("dbo.Works", new[] { "CamID" });
            DropIndex("dbo.Requests", new[] { "EmpID" });
            DropIndex("dbo.Requests", new[] { "CamID" });
            DropIndex("dbo.Notes", new[] { "EmpID" });
            DropIndex("dbo.Customers", new[] { "EmpID" });
            DropIndex("dbo.Customers", new[] { "CamID" });
            DropTable("dbo.EmployeeCampaigns");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Works");
            DropTable("dbo.Requests");
            DropTable("dbo.Notes");
            DropTable("dbo.Employees");
            DropTable("dbo.Customers");
            DropTable("dbo.Campaigns");
        }
    }
}
