using System;
using LightestNight.System.Data.MySql;
using LightestNight.System.EventSourcing.Checkpoints;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                .AddSingleton<IStreamStoreFactory, MySqlStreamStoreFactory>();

            services.TryAddSingleton<GetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().GetGlobalCheckpoint);

            services.TryAddSingleton<SetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().SetGlobalCheckpoint);

            if (!options.CreateSchemaIfNotExists) 
                return services;
            
            var serviceProvider = services.BuildServiceProvider();
            var streamStoreFactory = serviceProvider.GetRequiredService<IStreamStoreFactory>();
            var checkpointManager = serviceProvider.GetRequiredService<MySqlCheckpointManager>();

            ((MySqlStreamStore) streamStoreFactory.GetStreamStore().Result).CreateSchemaIfNotExists().Wait();
            checkpointManager.CreateSchemaIfNotExists().Wait();

            return services;
        }
    }
}