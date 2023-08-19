CREATE TABLE [dbo].[LogEntries] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [DateTime]  DATETIME2 (7)    NOT NULL,
    [LogLevel]  NVARCHAR (MAX)   NOT NULL,
    [Source]    NVARCHAR (MAX)   NULL,
    [Message]   NVARCHAR (MAX)   NULL,
    [Exception] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_LogEntries] PRIMARY KEY CLUSTERED ([Id] ASC)
);

