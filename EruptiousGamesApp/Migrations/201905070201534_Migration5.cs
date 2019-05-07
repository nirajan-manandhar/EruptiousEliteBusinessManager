namespace EruptiousGamesApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration5 : DbMigration
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
                "dbo.CampaignEmployees",
                c => new
                    {
                        Campaign_CamID = c.Int(nullable: false),
                        Employee_EmpID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Campaign_CamID, t.Employee_EmpID })
                .ForeignKey("dbo.Campaigns", t => t.Campaign_CamID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.Employee_EmpID, cascadeDelete: true)
                .Index(t => t.Campaign_CamID)
                .Index(t => t.Employee_EmpID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notes", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Works", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Works", "CamID", "dbo.Campaigns");
            DropForeignKey("dbo.Requests", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Requests", "CamID", "dbo.Campaigns");
            DropForeignKey("dbo.CampaignEmployees", "Employee_EmpID", "dbo.Employees");
            DropForeignKey("dbo.CampaignEmployees", "Campaign_CamID", "dbo.Campaigns");
            DropForeignKey("dbo.Customers", "EmpID", "dbo.Employees");
            DropForeignKey("dbo.Customers", "CamID", "dbo.Campaigns");
            DropIndex("dbo.CampaignEmployees", new[] { "Employee_EmpID" });
            DropIndex("dbo.CampaignEmployees", new[] { "Campaign_CamID" });
            DropIndex("dbo.Notes", new[] { "EmpID" });
            DropIndex("dbo.Works", new[] { "EmpID" });
            DropIndex("dbo.Works", new[] { "CamID" });
            DropIndex("dbo.Requests", new[] { "EmpID" });
            DropIndex("dbo.Requests", new[] { "CamID" });
            DropIndex("dbo.Customers", new[] { "EmpID" });
            DropIndex("dbo.Customers", new[] { "CamID" });
            DropTable("dbo.CampaignEmployees");
            DropTable("dbo.Notes");
            DropTable("dbo.Works");
            DropTable("dbo.Requests");
            DropTable("dbo.Customers");
            DropTable("dbo.Campaigns");
        }
    }
}
