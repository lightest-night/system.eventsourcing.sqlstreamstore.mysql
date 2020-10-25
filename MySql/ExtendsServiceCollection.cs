using System;
using System.Threading.Tasks;
using LightestNight.System.Data.MySql;
using LightestNight.System.EventSourcing.Checkpoints;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SqlStreamStore;

// ReSharper disable RedundantAssignment

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddMySqlEventStore(this IServiceCollection services,
            MySqlOptionsFactory mySqlOptionsFactory, Action<MySqlEventSourcingOptions>? eventSourcingOptions = null)
        {
            var options = new MySqlEventSourcingOptions();
            eventSourcingOptions?.Invoke(options);

            services.AddMySqlData(mySqlOptionsFactory)
                .AddEventStore(eventSourcingOptionsAccessor: o => o = options)
                .AddSingleton<MySqlCheckpointManager>()
                .AddTransient<IStreamStoreFactory>(sp =>
                {
                    var connection = sp.GetRequiredService<IMySqlConnection>();
                    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<MySqlStreamStoreFactory>();

                    var factory = new MySqlStreamStoreFactory(connection, logger);

                    if (!options.CreateSchemaIfNotExists)
                        return factory;

                    Task.Run(async () =>
                    {
                        var checkpointManager = sp.GetRequiredService<MySqlCheckpointManager>();
                        if (!(await factory.GetStreamStore() is MySqlStreamStore streamStore))
                            throw new InvalidOperationException(
                                "Cannot create schema when Stream Store has not been defined.");

                        await Task.WhenAll(streamStore.CreateSchemaIfNotExists(),
                            checkpointManager.CreateSchemaIfNotExists()).ConfigureAwait(false);
                    });

                    return factory;
                });

            services.TryAddSingleton<GetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().GetGlobalCheckpoint);

            services.TryAddSingleton<SetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().SetGlobalCheckpoint);

            return services;
        }
    }
}