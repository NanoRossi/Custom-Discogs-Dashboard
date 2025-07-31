
using System.Net;
using System.Threading.Tasks;
using DiscogsProxy.Workers;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Shouldly;

namespace UnitTests.Workers;

public class DiscogsApiHelperTests
{
    private Mock<IHttpClientFactory> _mockClientFactory;
    private Mock<IConfiguration> _mockConfiguration;
    private DiscogsApiHelper _apiHelper;

    public DiscogsApiHelperTests()
    {
        _mockClientFactory = new Mock<IHttpClientFactory>();
        _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.SetupGet(x => x["UserAgent"]).Returns("MyAgent");
        _mockConfiguration.SetupGet(x => x["DiscogsToken"]).Returns("MyToken");
        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns("MyUsername");

        _apiHelper = new DiscogsApiHelper(_mockClientFactory.Object, _mockConfiguration.Object);
    }

    #region CreateClient
    [Fact]
    public void CreateClient()
    {
        // Act
        var result = _apiHelper.CreateClient();

        // Assert
        result.ShouldNotBeNull();
        result.BaseAddress!.ToString().ShouldBe("https://api.discogs.com/");
        result.DefaultRequestHeaders.Authorization!.ToString().ShouldBe("Discogs token=MyToken");
        result.DefaultRequestHeaders.UserAgent!.ToString().ShouldBe("DiscogsApiApp/1.0 (MyAgent)");
    }
    #endregion

    #region GetJsonObjectFromResponseAsync
    [Fact]
    public async void GetJsonObjectFromResponseAsync()
    {
        // Arrange
        var input = new HttpResponseMessage()
        {
            Content = new StringContent("{ \"Property\" : \"Value\" }")
        };

        // Act
        var result = await _apiHelper.GetJsonObjectFromResponseAsync(input);

        // Assert
        result["Property"]!.ToString().ShouldBe("Value");
    }
    #endregion

    #region ProfileIsValid
    [Fact]
    public async Task ProfileIsValid_StatusUnsuccessful()
    {
        // Arrange
        _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
        .Returns(GetInteceptedClient(HttpStatusCode.BadRequest, "{ \"blah\" : \"blah2\"}", out var dummy));

        // Act
        var result = await GetService().ProfileIsValid();

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ProfileIsValid_TokenNotValid()
    {
        // Arrange
        _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
        .Returns(GetInteceptedClient(HttpStatusCode.OK, "{ \"blah\" : \"blah2\"}", out var dummy));

        // Act
        var result = await GetService().ProfileIsValid();

        // Assert
        // Email wasn't in the faked response
        // This is included in the discogs response if the username/token combo is valid
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ProfileIsValid_TokenValid()
    {
        // Arrange
        _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
        .Returns(GetInteceptedClient(HttpStatusCode.OK, "{ \"email\" : \"hello\"}", out var dummy));

        // Act
        var result = await GetService().ProfileIsValid();

        // Assert
        // Email in the faked response
        // So we're all good
        result.ShouldBeTrue();
    }
    #endregion

    #region Private Helpers
    private DiscogsApiHelper GetService()
    {
        return new DiscogsApiHelper(_mockClientFactory.Object, _mockConfiguration.Object);
    }

    /// <summary>
    /// Build a HttpClient with a mocked handler
    /// That will intercept the call, and send back our own message
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    private static HttpClient GetInteceptedClient(HttpStatusCode statusCode, string content, out HttpResponseMessage responseMessage)
    {
        var mockHandler = new Mock<HttpMessageHandler>();

        responseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        };

        mockHandler
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri("https://api.discogs.com/users/MyUsername")),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(responseMessage);

        return new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.discogs.com/")
        };
    }
    #endregion
}