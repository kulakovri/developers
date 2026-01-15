using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeRateUpdater
{
    /// <summary>
    /// Fetches exchange rates from the Czech National Bank daily fixing endpoint.
    /// </summary>
    public class CnbRatesSource : IExchangeRatesSource
    {
        private const string CnbDailyRatesPath = 
            "/en/financial-markets/foreign-exchange-market/central-bank-exchange-rate-fixing/central-bank-exchange-rate-fixing/daily.txt";

        private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

        private readonly string _ratesUrl;

        public CnbRatesSource(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty.", nameof(baseUrl));

            _ratesUrl = baseUrl.TrimEnd('/') + CnbDailyRatesPath;
        }

        public string GetLatestRatesContent()
        {
            try
            {
                return HttpClient.GetStringAsync(_ratesUrl).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to fetch exchange rates from {_ratesUrl}: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new InvalidOperationException(
                    $"Request timed out while fetching exchange rates from {_ratesUrl}", ex);
            }
        }
    }
}


