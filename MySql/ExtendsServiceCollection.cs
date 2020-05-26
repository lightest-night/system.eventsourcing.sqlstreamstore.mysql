using System;
using System.Reflection;
using System.Threading.Tasks;
using LightestNight.System.EventSourcing.Checkpoints;
using LightestNight.System.EventSourcing.SqlStreamStore.MySql.Checkpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SqlStreamStore;

namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddMySqlEventStore(this IServiceCollection services,
            Action<MySqlEventSourcingOptions>? optionsAccessor = null, params Assembly[] eventAssemblies)
        {
            var mysqlOptions = new MySqlEventSourcingOptions();
            optionsAccessor?.Invoke(mysqlOptions);
            // ReSharper disable once RedundantAssignment
            services.AddEventStore(eventSourcingOptions => eventSourcingOptions = mysqlOptions, eventAssemblies)
                .Configure(optionsAccessor)
                .AddSingleton<MySqlCheckpointManager>()
                .TryAddSingleton<MySqlConnectionBuilder>();

            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is MySqlStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connectionString = sp.GetRequiredService<MySqlConnectionBuilder>().BuildConnectionString();
                    var streamStore = new MySqlStreamStore(new MySqlStreamStoreSettings(connectionString));

                    if (!mysqlOptions.CreateSchemaIfNotExists)
                        return streamStore;

                    streamStore.CreateSchemaIfNotExists().Wait();
                    sp.GetRequiredService<MySqlCheckpointManager>().CreateSchemaIfNotExists().Wait();

                    return streamStore;
                });
            }

            services.TryAddSingleton<GetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().GetGlobalCheckpoint);
            
            services.TryAddSingleton<SetGlobalCheckpoint>(sp =>
                sp.GetRequiredService<MySqlCheckpointManager>().SetGlobalCheckpoint);

            return services;
        }
    }
}