using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuctionService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Make = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mileage = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservePrice = table.Column<int>(type: "int", nullable: false),
                    MyProperty = table.Column<int>(type: "int", nullable: false),
                    Seller = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Winner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldAmount = table.Column<int>(type: "int", nullable: true),
                    CurrentHighBid = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuctionEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnqueueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Color", "ImageUrl", "Make", "Mileage", "Model", "Year" },
                values: new object[,]
                {
                    { new Guid("12d87902-4afc-415d-bc14-c44724f16954"), "White", "https://cdn.pixabay.com/photo/2017/08/31/05/47/bmw-2699538_960_720.jpg", "BMW", 90000, "X1", 2017 },
                    { new Guid("1e64b61f-56a2-41c4-84b2-7db83be6eb5a"), "Black", "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg", "Bugatti", 15035, "Veyron", 2018 },
                    { new Guid("247f3fd9-d34d-4f72-8f2c-c38a4be7e3a4"), "White", "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg", "Ford", 50000, "GT", 2020 },
                    { new Guid("9be53e24-81e5-4e38-ab15-c14b2920c8ea"), "Rust", "https://cdn.pixabay.com/photo/2017/08/02/19/47/vintage-2573090_960_720.jpg", "Ford", 150150, "Model T", 1938 },
                    { new Guid("a2510c14-f119-4717-b4d5-c3eefe4b5782"), "Red", "https://cdn.pixabay.com/photo/2017/11/09/01/49/ferrari-458-spider-2932191_960_720.jpg", "Ferrari", 50000, "Spider", 2015 },
                    { new Guid("a899e12c-3ddd-4cab-ae66-d6031a4b626f"), "White", "https://cdn.pixabay.com/photo/2019/12/26/20/50/audi-r8-4721217_960_720.jpg", "Audi", 10050, "R8", 2021 },
                    { new Guid("d166d7c7-ec02-4b1d-9f0a-0841c03c2374"), "Black", "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg", "Ford", 65125, "Mustang", 2023 },
                    { new Guid("d3f9cf03-8a07-493d-9945-2cba4e3e1360"), "Red", "https://cdn.pixabay.com/photo/2017/11/08/14/39/ferrari-f430-2930661_960_720.jpg", "Ferrari", 5000, "F-430", 2022 },
                    { new Guid("d90c328e-a26c-492d-b7c5-3ec037d1919d"), "Black", "https://cdn.pixabay.com/photo/2016/09/01/15/06/audi-1636320_960_720.jpg", "Audi", 25400, "TT", 2020 },
                    { new Guid("eab59875-7c85-457d-b5b2-ad6e5024b0d7"), "Silver", "https://cdn.pixabay.com/photo/2016/04/17/22/10/mercedes-benz-1335674_960_720.png", "Mercedes", 15001, "SLK", 2020 }
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "AuctionEnd", "CreatedAt", "CurrentHighBid", "ItemId", "MyProperty", "ReservePrice", "Seller", "SoldAmount", "Status", "UpdatedAt", "Winner" },
                values: new object[,]
                {
                    { new Guid("155225c1-4448-4066-9886-6786536e05ea"), new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("eab59875-7c85-457d-b5b2-ad6e5024b0d7"), 0, 50000, "tom", null, 2, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("3659ac24-29dd-407a-81f5-ecfe6f924b9b"), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("9be53e24-81e5-4e38-ab15-c14b2920c8ea"), 0, 20000, "bob", null, 1, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("40490065-dac7-46b6-acc4-df507e0d6570"), new DateTime(2025, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("d90c328e-a26c-492d-b7c5-3ec037d1919d"), 0, 20000, "tom", null, 1, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("466e4744-4dc5-4987-aae0-b621acfc5e39"), new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("12d87902-4afc-415d-bc14-c44724f16954"), 0, 20000, "alice", null, 1, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("47111973-d176-4feb-848d-0ea22641c31a"), new DateTime(2025, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("d3f9cf03-8a07-493d-9945-2cba4e3e1360"), 0, 150000, "alice", null, 1, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("6a5011a1-fe1f-47df-9a32-b5346b289391"), new DateTime(2025, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("a899e12c-3ddd-4cab-ae66-d6031a4b626f"), 0, 0, "bob", null, 0, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("afbee524-5972-4075-8800-7d1f9d7b0a0c"), new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("247f3fd9-d34d-4f72-8f2c-c38a4be7e3a4"), 0, 20000, "bob", null, 0, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"), new DateTime(2025, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("d166d7c7-ec02-4b1d-9f0a-0841c03c2374"), 0, 0, "bob", null, 0, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"), new DateTime(2025, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("1e64b61f-56a2-41c4-84b2-7db83be6eb5a"), 0, 90000, "alice", null, 0, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("dc1e4071-d19d-459b-b848-b5c3cd3d151f"), new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("a2510c14-f119-4717-b4d5-c3eefe4b5782"), 0, 20000, "bob", null, 0, new DateTime(2025, 4, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ItemId",
                table: "Auctions",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxState");
        }
    }
}
