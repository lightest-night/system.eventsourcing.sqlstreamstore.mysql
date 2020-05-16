using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public class MySqlConnection
    {
        private readonly MySqlEventSourcingOptions _options;

        public MySqlConnection(IOptions<MySqlEventSourcingOptions> options)
        {
            _options = options.ThrowIfNull().Value;
        }

        public string BuildConnectionString()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = _options.Server,
				Port = _options.Port,
				UserID = _options.UserId,
				Password = _options.Password,
				Database = _options.Database,
				LoadBalance = _options.LoadBalance,
				ConnectionProtocol = _options.ConnectionProtocol,
				PipeName = _options.PipeName,
				SslMode = _options.SslMode,
				CertificateFile = _options.CertificateFile,
				CertificatePassword = _options.CertificatePassword,
				SslCert = _options.SslCert,
				SslKey = _options.SslKey,
				SslCa = _options.SslCa,
				CertificateStoreLocation = _options.CertificateStoreLocation,
				CertificateThumbprint = _options.CertificateThumbprint,
				Pooling = _options.Pooling,
				ConnectionLifeTime = _options.ConnectionLifeTime,
				ConnectionReset = _options.ConnectionReset,
				ConnectionIdlePingTime = _options.ConnectionIdlePingTime,
				ConnectionIdleTimeout = _options.ConnectionIdleTimeout,
				MinimumPoolSize = _options.MinimumPoolSize,
				MaximumPoolSize = _options.MaximumPoolSize,
				AllowLoadLocalInfile = _options.AllowLoadLocalInfile,
				AllowPublicKeyRetrieval = _options.AllowPublicKeyRetrieval,
				AllowUserVariables = _options.AllowUserVariables,
				AllowZeroDateTime = _options.AllowZeroDateTime,
				ApplicationName = _options.ApplicationName,
				AutoEnlist = _options.AutoEnlist,
				CharacterSet = _options.CharacterSet,
				ConnectionTimeout = _options.ConnectionTimeout,
				ConvertZeroDateTime = _options.ConvertZeroDateTime,
				DateTimeKind = _options.DateTimeKind,
				DefaultCommandTimeout = _options.DefaultCommandTimeout,
				ForceSynchronous = _options.ForceSynchronous,
				GuidFormat = _options.GuidFormat,
				IgnoreCommandTransaction = _options.IgnoreCommandTransaction,
				IgnorePrepare = _options.IgnorePrepare,
				InteractiveSession = _options.InteractiveSession,
				Keepalive = _options.Keepalive,
				NoBackslashEscapes = _options.NoBackslashEscapes,
				OldGuids = _options.OldGuids,
				PersistSecurityInfo = _options.PersistSecurityInfo,
				ServerRsaPublicKeyFile = _options.ServerRsaPublicKeyFile,
				ServerSPN = _options.ServerSpn,
				TreatTinyAsBoolean = _options.TreatTinyAsBoolean,
				UseAffectedRows = _options.UseAffectedRows,
				UseCompression = _options.UseCompression,
				UseXaTransactions = _options.UseXaTransactions
            };
            
            return builder.ConnectionString;
        }
    }

    public static class ExtendsObject
    {
        public static T ThrowIfNull<T>(this T target, [CallerMemberName] string? memberName = default) where T : class
            => target ?? throw new ArgumentNullException(memberName);
    }
}