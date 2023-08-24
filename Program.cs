using System.Data;
using System.Diagnostics;
using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;

const string connectionString =
    "Server=127.0.0.1;Port=5445;Database=npgsql_test;UserId=npgsql_test;Password=npgsql_test;IncludeErrorDetail=true;";

await using var logicalReplicationConnection = new LogicalReplicationConnection(connectionString);
await logicalReplicationConnection.Open();
var slot = await logicalReplicationConnection.CreatePgOutputReplicationSlot(
    "test_slot",
    temporarySlot: true,
    slotSnapshotInitMode: LogicalSlotSnapshotInitMode.Export,
    twoPhase: false
);

var snapshotName = slot.SnapshotName ?? throw new UnreachableException("Expected snapshot name");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
List<int> ids = new();
await using (var dataSource = dataSourceBuilder.Build()) {
    await using var connection = await dataSource.OpenConnectionAsync();
    await using var txn = await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
    await using var setTransactionCommand = new NpgsqlCommand($"SET TRANSACTION SNAPSHOT '{snapshotName}'", connection);
    await setTransactionCommand.ExecuteNonQueryAsync();
    await using var idsQueryCommand = new NpgsqlCommand("SELECT id FROM test_table", connection);
    await using var reader = await idsQueryCommand.ExecuteReaderAsync();
    while (await reader.ReadAsync()) {
        ids.Add(reader.GetInt32(0));
    }
}

var pgOutputOptions = new PgOutputReplicationOptions(
    publicationName: "test_publication",
    protocolVersion: 3,
    binary: false,
    streaming: false,
    messages: true, // allow receiving messages from pg_logical_emit_message
    twoPhase: false
);

Task.Delay(500).ContinueWith(async _ => {
    await using var dataSource = dataSourceBuilder.Build();
    await using var conn = await dataSource.OpenConnectionAsync();
    await using var emitCommand =
        new NpgsqlCommand("SELECT * FROM pg_logical_emit_message(false, 'some_prefix', 'ping')", conn);
    await emitCommand.ExecuteNonQueryAsync();
});

await foreach (var message in logicalReplicationConnection.StartReplication(slot, pgOutputOptions, default)) {
    Console.WriteLine($"Received message of type {message.GetType().Name}");
    logicalReplicationConnection.SetReplicationStatus(message.WalEnd);
}