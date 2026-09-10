CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Agents" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Agents" PRIMARY KEY AUTOINCREMENT,
    "DisplayName" TEXT NOT NULL,
    "Email" TEXT NULL
);

CREATE TABLE "Categories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);

CREATE TABLE "Customers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Customers" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Email" TEXT NULL
);

CREATE TABLE "Tickets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tickets" PRIMARY KEY AUTOINCREMENT,
    "Number" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Status" TEXT NOT NULL,
    "Priority" TEXT NOT NULL,
    "IsUrgent" INTEGER NOT NULL,
    "DueDate" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "CustomerId" INTEGER NOT NULL,
    "AgentId" INTEGER NULL,
    "CategoryId" INTEGER NOT NULL,
    "RowVersion" BLOB NOT NULL,
    CONSTRAINT "CK_Tickets_Title_Length" CHECK (length("Title") <= 180),
    CONSTRAINT "FK_Tickets_Agents_AgentId" FOREIGN KEY ("AgentId") REFERENCES "Agents" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Tickets_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Tickets_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "TicketComments" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_TicketComments" PRIMARY KEY AUTOINCREMENT,
    "TicketId" INTEGER NOT NULL,
    "Body" TEXT NOT NULL,
    "Author" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_TicketComments_Tickets_TicketId" FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Customers_Name" ON "Customers" ("Name");

CREATE INDEX "IX_TicketComments_TicketId" ON "TicketComments" ("TicketId");

CREATE INDEX "IX_Tickets_AgentId" ON "Tickets" ("AgentId");

CREATE INDEX "IX_Tickets_CategoryId" ON "Tickets" ("CategoryId");

CREATE INDEX "IX_Tickets_CustomerId" ON "Tickets" ("CustomerId");

CREATE UNIQUE INDEX "IX_Tickets_Number" ON "Tickets" ("Number");

CREATE INDEX "IX_Tickets_Status_DueDate" ON "Tickets" ("Status", "DueDate");

CREATE INDEX "IX_Tickets_UpdatedAt" ON "Tickets" ("UpdatedAt");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260910150534_InitialCreate', '10.0.12');

COMMIT;

