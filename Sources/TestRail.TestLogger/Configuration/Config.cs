using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace TestRail.TestLogger.Configuration
{
    public class Config
    {
        private static readonly Lazy<Config> TestRailConfig = new Lazy<Config>(() =>
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AssemblyDirectory)
                .AddJsonFile("TestRail.json", optional: true)
                .Build();

            var testSettings = new Config();
            configBuilder.Bind(testSettings);
            return testSettings;
        });

        private Config()
        {

        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static Config GetInstance()
        {
            return TestRailConfig.Value;
        }

        public string Url { get; set; }

        public User User { get; set; }

        public string ProjectName { get; set; }

        public string RunNameTemplate { get; set; }

        public bool Disabled { get; set; }
    }
}