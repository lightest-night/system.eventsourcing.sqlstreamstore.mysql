﻿using System;
using LightestNight.System.Data.MySql;
using LightestNight.System.EventSourcing.Checkpoints;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
                    var checkpointManager = sp.GetRequiredService<MySqlCheckpointManager>();
                    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger<MySqlStreamStoreFactory>();

                    return new MySqlStreamStoreFactory(connection, options, checkpointManager, logger);
                });

            //var serviceProvider = services.BuildServiceProvider();
            // if (!(serviceProvider.GetService<IStreamStore>() is MySqlStreamStore))
            // {
                // services.AddSingleton<IStreamStore>(sp =>
                // {
                //     // var connectionString = sp.GetRequiredService<IMySqlConnection>().Build().ConnectionString;
                //     // var streamStore = new MySqlStreamStore(new MySqlStreamStoreSettings(connectionString));
                //     //
                //     // if (!options.CreateSchemaIfNotExists)
                //     //     return streamStore;
                //     //
                //     // streamStore.CreateSchemaIfNotExists().Wait();
                //     // sp.GetRequiredService<MySqlCheckpointManager>().CreateSchemaIfNotExists().Wait();
                //     //
                //     // return streamStore;
                //     var connection = sp.GetRequiredService<IMySqlConnection>();
                //     var streamStoreSettings = new MySqlStreamStoreSettings(connection.GetConnection().ConnectionString)
                //     {
                //         ConnectionFactory = 
                //     }
                // });
            // }

            services.TryAddSingleton<GetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().GetGlobalCheckpoint);

            services.TryAddSingleton<SetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().SetGlobalCheckpoint);

            return services;
        }
    }
}