using AutoFixture;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql.Tests
{
    public class MySqlConnectionTests
    {
        private readonly MySqlEventSourcingOptions _options;
        private readonly MySqlConnection _sut;

        public MySqlConnectionTests()
        {
            var fixture = new Fixture();
            _options = fixture.Build<MySqlEventSourcingOptions>().Create();
            
            _sut = new MySqlConnection(Options.Create(_options));
        }

        [Fact]
        public void ShouldCreateNewMySqlConnectionAndPopulateConnectionString()
        {
            // Act
            var connectionString = _sut.BuildConnectionString();
            
            // Assert
            connectionString.ShouldContain($"Server={_options.Server}");
            connectionString.ShouldContain($"Port={_options.Port}");
            connectionString.ShouldContain($"User Id={_options.UserId}");
            connectionString.ShouldContain($"Password={_options.Password}");
            connectionString.ShouldContain($"Database={_options.Database}");
            connectionString.ShouldContain($"LoadBalance={_options.LoadBalance}");
            connectionString.ShouldContain($"ConnectionProtocol={_options.ConnectionProtocol}");
            connectionString.ShouldContain($"PipeName={_options.PipeName}");
            connectionString.ShouldContain($"SSL Mode={_options.SslMode}");
            connectionString.ShouldContain($"CertificateFile={_options.CertificateFile}");
            connectionString.ShouldContain($"CertificatePassword={_options.CertificatePassword}");
            connectionString.ShouldContain($"SslCert={_options.SslCert}");
            connectionString.ShouldContain($"SslKey={_options.SslKey}");
            connectionString.ShouldContain($"CACertificateFile={_options.SslCa}");
            connectionString.ShouldContain($"CertificateStoreLocation={_options.CertificateStoreLocation}");
            connectionString.ShouldContain($"CertificateThumbprint={_options.CertificateThumbprint}");
            connectionString.ShouldContain($"Pooling={_options.Pooling}");
            connectionString.ShouldContain($"Connection Lifetime={_options.ConnectionLifeTime}");
            connectionString.ShouldContain($"Connection Reset={_options.ConnectionReset}");
            connectionString.ShouldContain($"Connection Idle Ping Time={_options.ConnectionIdlePingTime}");
            connectionString.ShouldContain($"Connection Idle Timeout={_options.ConnectionIdleTimeout}");
            connectionString.ShouldContain($"Minimum Pool Size={_options.MinimumPoolSize}");
            connectionString.ShouldContain($"Maximum Pool Size={_options.MaximumPoolSize}");
            connectionString.ShouldContain($"AllowLoadLocalInfile={_options.AllowLoadLocalInfile}");
            connectionString.ShouldContain($"AllowPublicKeyRetrieval={_options.AllowPublicKeyRetrieval}");
            connectionString.ShouldContain($"AllowUserVariables={_options.AllowUserVariables}");
            connectionString.ShouldContain($"AllowZeroDateTime={_options.AllowZeroDateTime}");
            connectionString.ShouldContain($"ApplicationName={_options.ApplicationName}");
            connectionString.ShouldContain($"AutoEnlist={_options.AutoEnlist}");
            connectionString.ShouldContain($"CharSet={_options.CharacterSet}");
            connectionString.ShouldContain($"Connection Timeout={_options.ConnectionTimeout}");
            connectionString.ShouldContain($"Convert Zero Datetime={_options.ConvertZeroDateTime}");
            connectionString.ShouldContain($"DateTimeKind={_options.DateTimeKind}");
            connectionString.ShouldContain($"Default Command Timeout={_options.DefaultCommandTimeout}");
            connectionString.ShouldContain($"ForceSynchronous={_options.ForceSynchronous}");
            connectionString.ShouldContain($"GuidFormat={_options.GuidFormat}");
            connectionString.ShouldContain($"IgnoreCommandTransaction={_options.IgnoreCommandTransaction}");
            connectionString.ShouldContain($"IgnorePrepare={_options.IgnorePrepare}");
            connectionString.ShouldContain($"InteractiveSession={_options.InteractiveSession}");
            connectionString.ShouldContain($"Keep Alive={_options.Keepalive}");
            connectionString.ShouldContain($"No Backslash Escapes={_options.NoBackslashEscapes}");
            connectionString.ShouldContain($"Old Guids={_options.OldGuids}");
            connectionString.ShouldContain($"Persist Security Info={_options.PersistSecurityInfo}");
            connectionString.ShouldContain($"ServerRSAPublicKeyFile={_options.ServerRsaPublicKeyFile}");
            connectionString.ShouldContain($"Server SPN={_options.ServerSpn}");
            connectionString.ShouldContain($"Treat Tiny As Boolean={_options.TreatTinyAsBoolean}");
            connectionString.ShouldContain($"Use Affected Rows={_options.UseAffectedRows}");
            connectionString.ShouldContain($"Compress={_options.UseCompression}");
            connectionString.ShouldContain($"Use XA Transactions={_options.UseXaTransactions}");
        }

        [Fact]
        public void ShouldBeAValidConnection()
        {
            // Act
            var connectionString = _sut.BuildConnectionString();
            
            // Assert
            Should.NotThrow(() => new global::MySql.Data.MySqlClient.MySqlConnection(connectionString));
        }
    }
}