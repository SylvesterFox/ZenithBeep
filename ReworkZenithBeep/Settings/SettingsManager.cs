
using DotNetEnv;

namespace ReworkZenithBeep.Settings
{
    public static class SettingsManager
    {
        public static BotConfig LoadedConfig = new BotConfig();

        public static T LoadFromEnv<T>() where T : new()
        {
            DotNetEnv.Env.Load();
            var config = new T();
            var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var prop in props)
            {
                string? envValue = Environment.GetEnvironmentVariable(prop.Name);

                if (string.IsNullOrEmpty(envValue)) continue;

                try
                {
                    object value = prop.PropertyType switch
                    {
                        Type t when t == typeof(int) => int.Parse(envValue),
                        Type t when t == typeof(bool) => bool.Parse(envValue),
                        Type t when t == typeof(double) => double.Parse(envValue),
                        Type t when t == typeof(float) => float.Parse(envValue),
                        Type t when t == typeof(long) => long.Parse(envValue),
                        Type t when t == typeof(string) => envValue,
                        _ => Convert.ChangeType(envValue, prop.PropertyType)
                    };
                    prop.SetValue(config, value);
                }
                catch
                {
                    Console.WriteLine($"Не удалось конвертировать переменную {prop.Name}");
                }
            }
            return config;
        }



        // public const string BOT_NAME = "ZenithBeep";
        // public const string ENV_FILE = ".env";

        // public readonly string BotDataDirectory;

        // private SettingsManager()
        // {

        //     BotDataDirectory = Directory.GetCurrentDirectory();
        // }


        // public bool LoadConfiguration()
        // {
        //     // string configFilePath = Path.Combine(BotDataDirectory, ENV_FILE);
        //     // try
        //     // {
        //     //     Directory.CreateDirectory(BotDataDirectory);
        //     //     if (!File.Exists(configFilePath)) return false;
        //     // }
        //     // catch (Exception ex)
        //     // {
        //     //     return false;
        //     // }

        //     LoadedConfig = EnvSerializer.Deserialize(configFilePath);
        //     // return LoadedConfig != null;
        // }

        // private static SettingsManager? _PrivateInstance;
        // public static SettingsManager Instance
        // {
        //     get
        //     {
        //         return _PrivateInstance ??= new SettingsManager();
        //     }
        // }

    }
}

