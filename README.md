# Npgsql Issue 5208 Reproduction

This repository reproduces npgsql issue [#5208](https://github.com/npgsql/npgsql/issues/5208) 

## Instructions

```shell
docker compose up # -d to use the same shell 
dotnet run
```

You should observe the following output:

```
Received message of type LogicalDecodingMessage
Unhandled exception. Npgsql.NpgsqlException (0x80004005): Unknown message code: 112
   at Npgsql.Util.PGUtil.ValidateBackendMessageCode(BackendMessageCode code)
   at Npgsql.Internal.NpgsqlConnector.ReadMessage(Boolean async, DataRowLoadingMode dataRowLoadingMode, Boolean readingNotifications)
   at Npgsql.Internal.NpgsqlConnector.ReadMessage(Boolean async, DataRowLoadingMode dataRowLoadingMode)
   at Npgsql.Replication.ReplicationConnection.StartReplicationInternal(String command, Boolean bypassingStream, CancellationToken cancellationToken)+MoveNext()
   at Npgsql.Replication.ReplicationConnection.StartReplicationInternal(String command, Boolean bypassingStream, CancellationToken cancellationToken)+MoveNext()
   at Npgsql.Replication.ReplicationConnection.StartReplicationInternal(String command, Boolean bypassingStream, CancellationToken cancellationToken)+System.Threading.Tasks.Sources.IValueTaskSource<System.Boolean>.GetResult()
   at Npgsql.Replication.Internal.LogicalReplicationConnectionExtensions.<StartLogicalReplication>g__StartLogicalReplicationInternal|1_0(LogicalReplicationConnection connection, LogicalReplicationSlot slot, CancellationToken cancellationToken, Nullable`1 walLocation, IEnumerable`1 options, Boolean bypassingStream)+MoveNext()
   at Npgsql.Replication.Internal.LogicalReplicationConnectionExtensions.<StartLogicalReplication>g__StartLogicalReplicationInternal|1_0(LogicalReplicationConnection connection, LogicalReplicationSlot slot, CancellationToken cancellationToken, Nullable`1 walLocation, IEnumerable`1 options, Boolean bypassingStream)+System.Threading.Tasks.Sources.IValueTask
Source<System.Boolean>.GetResult()
   at Npgsql.Replication.PgOutput.PgOutputAsyncEnumerable.StartReplicationInternal(CancellationToken cancellationToken)+MoveNext()
   at Npgsql.Replication.PgOutput.PgOutputAsyncEnumerable.StartReplicationInternal(CancellationToken cancellationToken)+MoveNext()
   at Npgsql.Replication.PgOutput.PgOutputAsyncEnumerable.StartReplicationInternal(CancellationToken cancellationToken)+System.Threading.Tasks.Sources.IValueTaskSource<System.Boolean>.GetResult()
   at Program.<Main>$(String[] args) in C:\Users\benlo\source\repos\NpgsqlIssue5208\Program.cs:line 52
   at Program.<Main>$(String[] args) in C:\Users\benlo\source\repos\NpgsqlIssue5208\Program.cs:line 52
   at Program.<Main>$(String[] args) in C:\Users\benlo\source\repos\NpgsqlIssue5208\Program.cs:line 52
   at Program.<Main>(String[] args)
```