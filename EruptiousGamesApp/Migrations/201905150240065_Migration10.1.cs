namespace EruptiousGamesApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration101 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Notes", "Title", c => c.String());
            AlterColumn("dbo.Notes", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Notes", "Comment", c => c.String(nullable: false));
            AlterColumn("dbo.Notes", "Title", c => c.String(nullable: false));
        }
    }
}
