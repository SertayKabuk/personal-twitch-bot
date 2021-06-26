using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DoberDogBot.Application.Extensions
{
    public static class DotEnvExtension
    {
        public static void LoadEnvVariables(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddEnvironmentVariables();

            var root = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(root, ".env");

            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (line.Length > 0)
                {
                    var delimeterIndex = line.IndexOf("=");

                    if (delimeterIndex > 0)
                    {
                        var key = line[..delimeterIndex];

                        var value = line[(delimeterIndex + 1)..];

                        if (key.StartsWith("#"))
                            continue;

                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
            }
        }
    }
}
