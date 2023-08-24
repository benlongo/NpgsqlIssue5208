using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;

const string connectionString =
    "Server=127.0.0.1;Port=5445;Database=npgsql_test;UserId=npgsql_test;Password=npgsql_test;IncludeErrorDetail=true;Pooling=false;";

var logicalReplicationConnection = new LogicalReplicationConnection(connectionString + "Application Name=replicator;");
await logicalReplicationConnection.Open();
var slot = await logicalReplicationConnection.CreatePgOutputReplicationSlot(
    "test_slot",
    temporarySlot: true,
    slotSnapshotInitMode: LogicalSlotSnapshotInitMode.Export,
    twoPhase: false
);


var pgOutputOptions = new PgOutputReplicationOptions(
    publicationName: "test_publication",
    protocolVersion: 3,
    binary: false,
    streaming: false,
    messages: true, // allow receiving messages from pg_logical_emit_message
    twoPhase: false
);

var consumptionTask = ConsumeReplicationStream();
await EmitMessage(false, "test_prefix", "ping");
await consumptionTask;

async Task EmitMessage(bool transactional, string prefix, string message) {
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString + "Application Name=emitter;");
    await using var dataSource = dataSourceBuilder.Build();
    await using var conn = await dataSource.OpenConnectionAsync();
    await using var emitCommand =
        new NpgsqlCommand(
            $"SELECT * FROM pg_logical_emit_message({(transactional ? "true" : "false")}, '{prefix}', '{message}')",
            conn);
    await emitCommand.ExecuteNonQueryAsync();
}

async Task ConsumeReplicationStream() {
    await foreach (var message in logicalReplicationConnection.StartReplication(slot, pgOutputOptions, default)) {
        Console.WriteLine($"Received message of type {message.GetType().Name}");
        logicalReplicationConnection.SetReplicationStatus(message.WalEnd);
    }
}