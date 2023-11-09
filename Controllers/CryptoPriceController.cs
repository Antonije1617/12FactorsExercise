using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class CryptoController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CryptoController> _logger;
    private readonly string _coinGeckoBaseUrl;

    public CryptoController(IHttpClientFactory httpClientFactory,
                            ILogger<CryptoController> logger,
                            IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
        _coinGeckoBaseUrl = configuration.GetValue<string>("CoinGeckoApiBaseUrl");
    }

    [HttpGet("/getCoinPrice/")]
    public async Task<IActionResult> GetPrice(string coinId, string currency)
    {

        if (string.IsNullOrWhiteSpace(coinId) || string.IsNullOrWhiteSpace(currency))
        {
            _logger.LogError("CoinId or Currency was not provided.");
            return BadRequest("You have to provide CoinId and Currency.");
        }

        string url = $"{_coinGeckoBaseUrl}/simple/price?ids={coinId}&vs_currencies={currency}";

        try
        {
            var response = await _httpClient.GetStringAsync(url, HttpContext.RequestAborted);
            _logger.LogInformation($"Retrieved price for {coinId} in {currency}: {response}");
            return Ok(response);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError($"Error fetching price for {coinId} in {currency}: {e.Message}");
            return StatusCode(503, e.Message);
        }
    }
}
