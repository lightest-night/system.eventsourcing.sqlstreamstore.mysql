using System;
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

                    var checkpointManager = sp.GetRequiredService<MySqlCheckpointManager>();
                    logger.LogInformation("Creating Checkpoint Schema");
                    checkpointManager.CreateSchemaIfNotExists().Wait();

                    var streamStore = factory.GetStreamStore().Result;
                    if (streamStore == null)
                        throw new ArgumentNullException(nameof(streamStore));

                    logger.LogInformation("Creating Stream Store Schema");
                    ((MySqlStreamStore) streamStore).CreateSchemaIfNotExists().Wait();

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