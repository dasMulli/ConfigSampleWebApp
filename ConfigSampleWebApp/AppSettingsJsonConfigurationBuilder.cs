using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using ConfigurationBuilder = System.Configuration.ConfigurationBuilder;
using ConfigurationSection = System.Configuration.ConfigurationSection;

namespace ConfigSampleWebApp
{
    public class AppSettingsJsonConfigurationBuilder : ConfigurationBuilder
    {
#if DEBUG
        private const string DefaultEnvironmentName = "Development";
#else
        private const string DefaultEnvironmentName = "Production";
#endif

        private readonly Lazy<IConfigurationRoot> Configuration = new Lazy<IConfigurationRoot>(() =>
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNET_ENVIRONMENT") ??
                                  Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                                  DefaultEnvironmentName;

            return new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();
        });
        
        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            switch (configSection)
            {
                case AppSettingsSection appSettingsSection:
                    return ProcessAppSettingsSection(appSettingsSection);
                case ConnectionStringsSection connectionStringsSection:
                    return ProcessConnectionStringsSection(connectionStringsSection);
                default:
                    return base.ProcessConfigurationSection(configSection);
            }
        }

        private AppSettingsSection ProcessAppSettingsSection(AppSettingsSection section)
        {
            var appSettingsConfigSection = Configuration.Value.GetSection("AppSettings");
            if (appSettingsConfigSection == null)
            {
                return section;
            }

            foreach (var setting in appSettingsConfigSection.GetChildren())
            {
                // keys that contain subsections have a null value
                if (setting.Value == null)
                {
                    continue;
                }

                if (section.Settings[setting.Key] is KeyValueConfigurationElement existingElement)
                {
                    existingElement.Value = setting.Value;
                }
                else
                {
                    section.Settings.Add(setting.Key, setting.Value);
                }
            }

            appSettingsConfigSection.GetReloadToken().RegisterChangeCallback(AppSettingsSectionChanged, appSettingsConfigSection);

            return section;
        }

        private static void AppSettingsSectionChanged(object configSectionObj)
        {
            var configSection = (IConfigurationSection)configSectionObj;
            foreach (var setting in configSection.GetChildren())
            {
                if (setting.Value == null)
                {
                    continue;
                }

                ConfigurationManager.AppSettings.Set(setting.Key, setting.Value);
            }

            configSection.GetReloadToken().RegisterChangeCallback(AppSettingsSectionChanged, configSection);
        }

        private ConnectionStringsSection ProcessConnectionStringsSection(ConnectionStringsSection section)
        {
            var connectionStringsSection = Configuration.Value.GetSection("ConnectionStrings");
            if (connectionStringsSection == null)
            {
                return section;
            }

            var connectionStringSettingsCollection = section.ConnectionStrings;

            foreach (var setting in connectionStringsSection.GetChildren())
            {
                var key = setting.Key;
                string value;
                string providerName = null;
                // keys that contain subsections have a null value
                if (setting.Value == null)
                {
                    value = setting["ConnectionString"];
                    providerName = setting["ProviderName"];
                }
                else
                {
                    value = setting.Value;
                }

                if (value == null)
                {
                    continue;
                }

                if (connectionStringSettingsCollection[key] is ConnectionStringSettings existingConnectionString)
                {
                    existingConnectionString.ConnectionString = value;
                    if (providerName != null)
                    {
                        existingConnectionString.ProviderName = providerName;
                    }
                }
                else
                {
                    connectionStringSettingsCollection.Add(providerName != null
                        ? new ConnectionStringSettings(key, value, providerName)
                        : new ConnectionStringSettings(key, value));
                }
            }

            return section;
        }
    }
}