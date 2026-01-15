using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeRateUpdater
{
    public static class Program
    {
        private const string DefaultCnbBaseUrl = "https://www.cnb.cz";

        private static IEnumerable<Currency> currencies = new[]
        {
            new Currency("USD"),
            new Currency("EUR"),
            new Currency("CZK"),
            new Currency("JPY"),
            new Currency("KES"),
            new Currency("RUB"),
            new Currency("THB"),
            new Currency("TRY"),
            new Currency("XYZ")
        };

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var cnbBaseUrl = configuration["CnbApi:BaseUrl"] ?? DefaultCnbBaseUrl;

            var services = new ServiceCollection();
            services.AddSingleton<IExchangeRatesSource>(_ => new CnbRatesSource(cnbBaseUrl));
            services.AddTransient<ExchangeRateProvider>();

            using var serviceProvider = services.BuildServiceProvider();

            try
            {
                var provider = serviceProvider.GetRequiredService<ExchangeRateProvider>();
                var rates = provider.GetExchangeRates(currencies);

                Console.WriteLine($"Successfully retrieved {rates.Count()} exchange rates:");
                foreach (var rate in rates)
                {
                    Console.WriteLine(rate.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not retrieve exchange rates: '{e.Message}'.");
            }

            Console.ReadLine();
        }
    }
}
