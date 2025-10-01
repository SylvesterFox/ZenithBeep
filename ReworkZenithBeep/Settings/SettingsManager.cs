
namespace ReworkZenithBeep.Settings
{
    public static class SettingsManager
    {
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

    }
}

