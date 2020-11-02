using System;
using System.Threading.Tasks;
using AutoFixture;
using LightestNight.System.Data.MySql;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints;
using Microsoft.Extensions.Logging.Abstractions;
using SqlStreamStore;
using Xunit;

namespace LightestNight.EventSourcing.SqlStreamStore.MySql.Tests
{
    public class MySqlStreamStoreFactoryTests
    {
        private readonly MySqlStreamStoreFactory _sut;
        private readonly MySqlCheckpointManager _checkpointManager;
        
        public MySqlStreamStoreFactoryTests()
        {
            var fixture = new Fixture();
            var options = fixture.Build<MySqlOptions>()
                .Without(o => o.Server)
                .Without(o => o.Port)
                .Without(o => o.UserId)
                .Without(o => o.Password)
                .Without(o => o.Database)
                .Without(o => o.Pooling)
                .Without(o => o.MinimumPoolSize)
                .Without(o => o.MaximumPoolSize)
                .Without(o => o.AllowUserVariables)
                .Do(o =>
                {
                    o.Server = Environment.GetEnvironmentVariable("MYSQL_SERVER") ?? "localhost";
                    o.Port = Convert.ToUInt32(Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306");
                    o.UserId = Environment.GetEnvironmentVariable("MYSQL_USERID") ?? "mysql";
                    o.Password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "mysql";
                    o.Database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "mysql";
                    o.Pooling = false;
                    o.MinimumPoolSize = 1;
                    o.MaximumPoolSize = 1;
                    o.AllowUserVariables = true;
                })
                .Create();

            _sut = new MySqlStreamStoreFactory(new MySqlConnection(() => options, NullLogger<MySqlConnection>.Instance),
                new NullLogger<MySqlStreamStoreFactory>());
            _checkpointManager = new MySqlCheckpointManager(new MySqlConnection(() => options, NullLogger<MySqlConnection>.Instance),
                new NullLogger<MySqlCheckpointManager>());
        }

        [Fact]
        public void ShouldCreateStreamStoreThatCanThenBeUsed()
        {
           // Act
           var result = _sut.GetStreamStore().Result;

           _checkpointManager.CreateSchemaIfNotExists().Wait();
           ((MySqlStreamStore) result).CreateSchemaIfNotExists().Wait();
        }
    }
}