using System;
using System.Reflection;
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
                .AddSingleton<IStreamStoreFactory>(sp =>
                {
                    var streamStoreFactory = new MySqlStreamStoreFactory(sp.GetRequiredService<IMySqlConnection>(),
                        sp.GetRequiredService<ILoggerFactory>().CreateLogger<MySqlStreamStoreFactory>());

                    if (!options.CreateSchemaIfNotExists)
                        return streamStoreFactory;

                    var checkpointManager = sp.GetRequiredService<MySqlCheckpointManager>();
                    var streamStore = (MySqlStreamStore) streamStoreFactory.GetStreamStore().Result;

                    checkpointManager.CreateSchemaIfNotExists().Wait();
                    streamStore.CreateSchemaIfNotExists().Wait();

                    return streamStoreFactory;
                });

            services.TryAddSingleton<GetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().GetGlobalCheckpoint);

            services.TryAddSingleton<SetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().SetGlobalCheckpoint);

            return services;
        }
    }
}