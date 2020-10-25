using System;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Data.MySql;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints;
using Microsoft.Extensions.Logging;
using SqlStreamStore;
using MySqlConnection = MySqlConnector.MySqlConnection;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public interface IStreamStoreFactory
    {
        Task<IStreamStore> GetStreamStore(int retries = 3, CancellationToken cancellationToken = default);
    }
    
    public class MySqlStreamStoreFactory : IStreamStoreFactory
    {
        private static IStreamStore? _streamStore;
        private static MySqlConnection? _mySqlConnection;
        
        private readonly IMySqlConnection _connection;
        private readonly MySqlEventSourcingOptions _options;
        private readonly MySqlCheckpointManager _checkpointManager;
        private readonly ILogger _logger;
        
        public MySqlStreamStoreFactory(IMySqlConnection connection, MySqlEventSourcingOptions options, MySqlCheckpointManager checkpointManager,
            ILogger<MySqlStreamStoreFactory> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _checkpointManager = checkpointManager ?? throw new ArgumentNullException(nameof(checkpointManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IStreamStore> GetStreamStore(int retries = 3, CancellationToken cancellationToken = default)
        {
            if (_streamStore != null && _mySqlConnection != null &&
                _connection.ValidateConnection(_mySqlConnection, out _))
            {
                _logger.LogInformation("Using existing StreamStore & Connection...");
                return _streamStore;
            }

            _logger.LogInformation("Building new StreamStore & Connection");
            _mySqlConnection = _connection.GetConnection(retries);
            _streamStore = new MySqlStreamStore(new MySqlStreamStoreSettings(_mySqlConnection.ConnectionString));

            if (!_options.CreateSchemaIfNotExists)
                return _streamStore;

            await Task.WhenAll(((MySqlStreamStore) _streamStore).CreateSchemaIfNotExists(cancellationToken),
                _checkpointManager.CreateSchemaIfNotExists(cancellationToken)).ConfigureAwait(false);

            return _streamStore;
        }
    }
}