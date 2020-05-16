using System;
using System.Reflection;
using System.Threading.Tasks;
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
            
            services.AddEventStore(eventSourcingOptions => eventSourcingOptions = mysqlOptions, eventAssemblies);

            services.Configure(optionsAccessor);
            services.TryAddSingleton<MySqlConnection>();

            var serviceProvider = services.BuildServiceProvider();
            if (!(serviceProvider.GetService<IStreamStore>() is MySqlStreamStore))
            {
                services.AddSingleton<IStreamStore>(sp =>
                {
                    var connectionString = sp.GetRequiredService<MySqlConnection>().BuildConnectionString();
                    var streamStore = new MySqlStreamStore(new MySqlStreamStoreSettings(connectionString));

                    if (mysqlOptions.CreateSchemaIfNotExists)
                        Task.Run(async () => await streamStore.CreateSchemaIfNotExists().ConfigureAwait(false));

                    return streamStore;
                });
            }

            return services;
        }
    }
}