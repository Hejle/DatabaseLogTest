CREATE PROCEDURE [AddLog]
	@id UNIQUEIDENTIFIER,
	@logtime datetime2(7),
	@level nvarchar(max),
	@source nvarchar(max),
	@message nvarchar(max),
	@exception nvarchar(max)
AS
BEGIN
	INSERT INTO dbo.[LogEntries]
	(
	[Id],
	[DateTime],
	[LogLevel],
	[Source],
	[Message],
	[Exception])
	VALUES
	(@id,
	@logtime,
	@level,
	@source,
	@message,
	@exception)
END