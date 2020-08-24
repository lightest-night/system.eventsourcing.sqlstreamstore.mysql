using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints
{
    public class MySqlCheckpointManager
    {
        private readonly Func<MySqlConnection> _createConnection;
        private readonly Scripts.Scripts _scripts;
        private readonly ILogger<MySqlCheckpointManager> _logger;

        public MySqlCheckpointManager(MySqlConnectionBuilder connectionBuilder, ILogger<MySqlCheckpointManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scripts = new Scripts.Scripts();
            
            _createConnection = () => new MySqlConnection(connectionBuilder.BuildConnectionString());
        }
        
        public async Task<long?> GetGlobalCheckpoint(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace(new EventId(3, "Get Checkpoint"),
                $"Getting Checkpoint with Id '{Constants.GlobalCheckpointId}'");

            await using var connection = _createConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var command = new MySqlCommand(_scripts.GetCheckpoint, connection);
            command.Parameters.Add(new MySqlParameter("@CheckpointId", MySqlDbType.String, 500)
            {
                Value = Constants.GlobalCheckpointId
            });

            return await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) as long?;
        }
        
        public async Task SetGlobalCheckpoint(long? checkpoint, CancellationToken cancellationToken = default)
        {
            _logger.LogTrace(new EventId(2, "Set Checkpoint"), $"Setting Global Checkpoint: '{checkpoint}'");
            await using var connection = _createConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var command = new MySqlCommand(_scripts.SetCheckpoint, connection);
            command.Parameters.Add(new MySqlParameter("@CheckpointId", MySqlDbType.String, 500)
            {
                Value = Constants.GlobalCheckpointId
            });
            command.Parameters.AddWithValue("@Checkpoint", checkpoint);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Creates a schema that will hold checkpoints, if the schema does not exist.
        /// Calls to this should be part of an application's deployment/upgrade process and
        /// not every time your application boots up.
        /// </summary>
        /// <param name="cancellationToken">Any <see cref="CancellationToken" /> used to marshall the operation</param>
        /// <returns></returns>
        public async Task CreateSchemaIfNotExists(CancellationToken cancellationToken = default)
        {
            _logger.LogTrace(new EventId(1, "Create Schema"), "Creating Checkpoint Schema");
            await using var connection = _createConnection();
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            await using var transaction =
                await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            await using (var command = new MySqlCommand(_scripts.CreateSchema, transaction.Connection, transaction))
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}