namespace BugTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTicketHistoryProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketHistoryProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TicketHistories", "PropertyId", c => c.Int(nullable: false));
            CreateIndex("dbo.TicketHistories", "PropertyId");
            AddForeignKey("dbo.TicketHistories", "PropertyId", "dbo.TicketHistoryProperties", "Id", cascadeDelete: true);
            DropColumn("dbo.TicketHistories", "Property");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketHistories", "Property", c => c.String());
            DropForeignKey("dbo.TicketHistories", "PropertyId", "dbo.TicketHistoryProperties");
            DropIndex("dbo.TicketHistories", new[] { "PropertyId" });
            DropColumn("dbo.TicketHistories", "PropertyId");
            DropTable("dbo.TicketHistoryProperties");
        }
    }
}
