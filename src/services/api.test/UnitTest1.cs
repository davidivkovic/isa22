namespace api.test;

[TestClass]
public class ApiTests
{
    private readonly HttpClient _httpClient;

    public ApiTests()
    {
        var webAppFactory = new CustomWebApplicationFactory<Program>();
        _httpClient = webAppFactory.CreateDefaultClient();
    }

    [TestMethod]
    public async Task CheckHealth_ReturnsOK()
    {
        var response = await _httpClient.GetStringAsync("/");
        Assert.AreEqual("API is up and running V4.", response);
    }
}
