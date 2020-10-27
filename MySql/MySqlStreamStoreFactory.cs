using System;
using System.Threading;
using System.Threading.Tasks;
using LightestNight.System.Data.MySql;
using Microsoft.Extensions.Logging;
using SqlStreamStore;
using MySqlConnection = MySqlConnector.MySqlConnection;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public class MySqlStreamStoreFactory : IStreamStoreFactory
    {
        private static MySqlConnection? _mySqlConnection;
        
        private readonly IMySqlConnection _connection;
        private readonly ILogger _logger;

        public MySqlStreamStoreFactory(IMySqlConnection connection, ILogger<MySqlStreamStoreFactory> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IStreamStore> GetStreamStore(int retries = 3, CancellationToken cancellationToken = default)
        {
            if (_mySqlConnection == null || !_connection.ValidateConnection(_mySqlConnection, out _))
            {
                _logger.LogInformation("Getting a new Connection");
                _mySqlConnection = _connection.GetConnection(retries);
            }

            _logger.LogDebug("Connection Details: {@connection}", _mySqlConnection);
            var streamStore = new MySqlStreamStore(new MySqlStreamStoreSettings(_mySqlConnection.ConnectionString));
            return Task.FromResult((IStreamStore) streamStore);
        }
    }
}