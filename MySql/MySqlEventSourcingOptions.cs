using MySql.Data.MySqlClient;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public class MySqlEventSourcingOptions : EventSourcingOptions
    {
        /// <summary>
        /// Whether to create the database schema if it doesn't already exist
        /// </summary>
        public bool CreateSchemaIfNotExists { get; set; } = true;

        public string Server { get; set; } = string.Empty;

		public uint Port { get; set; }

		public string UserId { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;

		public string Database { get; set; } = string.Empty;

		public MySqlLoadBalance LoadBalance { get; set; }

		public MySqlConnectionProtocol ConnectionProtocol { get; set; }

		public string? PipeName { get; set; }

		// SSL/TLS Options
		public MySqlSslMode SslMode { get; set; }

		public string? CertificateFile { get; set; }

		public string? CertificatePassword { get; set; }

		public string? SslCert { get; set; }

		public string? SslKey { get; set; }

		public string? SslCa { get; set; }

		public MySqlCertificateStoreLocation CertificateStoreLocation { get; set; }

		public string? CertificateThumbprint { get; set; }

		public bool Pooling { get; set; }

		public uint ConnectionLifeTime { get; set; }

		public bool ConnectionReset { get; set; }

		public uint ConnectionIdlePingTime { get; set; }

		public uint ConnectionIdleTimeout { get; set; }

		public uint MinimumPoolSize { get; set; }

		public uint MaximumPoolSize { get; set; }
		
		public bool AllowLoadLocalInfile { get; set; }

		public bool AllowPublicKeyRetrieval { get; set; }

		public bool AllowUserVariables { get; set; }

		public bool AllowZeroDateTime { get; set; }

		public string? ApplicationName { get; set; }

		public bool AutoEnlist { get; set; }

		public string? CharacterSet { get; set; }

		public uint ConnectionTimeout { get; set; }

		public bool ConvertZeroDateTime { get; set; }

		public MySqlDateTimeKind DateTimeKind { get; set; }

		public uint DefaultCommandTimeout { get; set; }

		public bool ForceSynchronous { get; set; }

		public MySqlGuidFormat GuidFormat { get; set; }

		public bool IgnoreCommandTransaction { get; set; }

		public bool IgnorePrepare { get; set; }

		public bool InteractiveSession { get; set; }

		public uint Keepalive { get; set; }

		public bool NoBackslashEscapes { get; set; }

		public bool OldGuids { get; set; }

		public bool PersistSecurityInfo { get; set; }

		public string? ServerRsaPublicKeyFile { get; set; }

		public string? ServerSpn { get; set; }

		public bool TreatTinyAsBoolean { get; set; }

		public bool UseAffectedRows { get; set; }

		public bool UseCompression { get; set; }

		public bool UseXaTransactions { get; set; }
    }
}