using DiscogsProxy.DTO;
using DiscogsProxy.Services;
using Shouldly;
using DiscogsProxy.Workers;
using Moq;
using Moq.EntityFrameworkCore;
using Moq.Protected;
using System.Net;
using System.Text.Json.Nodes;

namespace UnitTests.Services;

public class WantlistServiceTests
{
    private readonly Mock<DiscogsContext> _mockContext;
    private readonly Mock<IDiscogsApiHelper> _mockApiHelper;

    public WantlistServiceTests()
    {
        _mockContext = new Mock<DiscogsContext>();
        _mockApiHelper = new Mock<IDiscogsApiHelper>();
    }

    #region GetWantlistItems
    [Fact]
    public void GetWantlistItems_NoWantlistAvailable()
    {
        // Arrange
        _mockContext.Setup(x => x.Wantlist).ReturnsDbSet([]);

        // Act
        var result = GetWantlistService().GetWantlistItems();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("No wantlist available");

        _mockContext.Verify(x => x.Wantlist, Times.Once);
    }

    [Fact]
    public void GetWantlistItems_GetsWantlist()
    {
        // Arrange
        _mockContext.Setup(x => x.Wantlist).ReturnsDbSet([new() { Id = 123 }]);

        // Act
        var result = GetWantlistService().GetWantlistItems();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result!.Count.ShouldBe(1);
        result.Result[0].Id.ShouldBe(123);

        _mockContext.Verify(x => x.Wantlist, Times.Exactly(2));
    }
    #endregion

    #region GetWantList
    [Fact]
    public async Task GetWantList_ErrorGettingPage()
    {
        // Arrange
        _mockApiHelper.Setup(x => x.CreateClient()).Returns(GetWantlistInterceptedClient(HttpStatusCode.BadRequest, "{ \"message\": \"uh oh\" }", out _));

        // Act
        var result = await GetWantlistService().GetWantList("myUser", 1, 1);

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Error from Discogs collection API: { \"message\": \"uh oh\" }");

        _mockApiHelper.Verify(x => x.CreateClient(), Times.Once);
        _mockApiHelper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetWantList_ReturnsMappedReleases()
    {
        // Arrange
        var client = GetWantlistInterceptedClient(HttpStatusCode.OK, "{ \"message\": \"All Good\" }", out HttpResponseMessage mockedMessage);
        JsonObject? jsonObj = [];

        _mockApiHelper.Setup(x => x.CreateClient()).Returns(client);
        _mockApiHelper.Setup(x => x.GetJsonObjectFromResponseAsync(mockedMessage)).Returns(Task.FromResult(jsonObj));
        _mockApiHelper.Setup(x => x.MapReleases<WantlistItem>(It.IsAny<JsonArray>())).Returns(new ResultObject<List<WantlistItem>>() { Result = [new WantlistItem() { Id = 123 }] });

        // Act
        var result = await GetWantlistService().GetWantList("myUser", 1, 1);

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result!.Count.ShouldBe(1);
        result.Result[0].Id.ShouldBe(123);

        _mockApiHelper.Verify(x => x.CreateClient(), Times.Once);
        _mockApiHelper.Verify(x => x.GetJsonObjectFromResponseAsync(mockedMessage), Times.Once);
        _mockApiHelper.Verify(x => x.GetEntriesFromPage(It.IsAny<JsonArray>(), jsonObj, "wants"), Times.Once);
        _mockApiHelper.Verify(x => x.CheckAllPages(jsonObj, client, "myUser", 1, 1, It.IsAny<Func<HttpClient, string, int, int, Task<ResultObject<HttpResponseMessage>>>>(), It.IsAny<JsonArray>(), "wants"), Times.Once);
        _mockApiHelper.Verify(x => x.MapReleases<WantlistItem>(It.IsAny<JsonArray>()), Times.Once);
        _mockApiHelper.VerifyNoOtherCalls();
    }

    #endregion

    #region GetWantlistPage
    [Fact]
    public async Task GetWantlistPage_NoWantlistAvailable()
    {
        // Arrange
        _mockContext.Setup(x => x.Wantlist).ReturnsDbSet([]);

        // Act
        var result = await GetWantlistService().GetWantlistPage(GetWantlistInterceptedClient(HttpStatusCode.InternalServerError, "{ \"message\": \"BOOM\" }", out _), "myUser", 1, 1);

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Error from Discogs collection API: { \"message\": \"BOOM\" }");
    }

    [Fact]
    public async Task GetWantlistPage_ReturnPage()
    {
        // Arrange
        _mockContext.Setup(x => x.Wantlist).ReturnsDbSet([]);

        // Act
        var result = await GetWantlistService().GetWantlistPage(GetWantlistInterceptedClient(HttpStatusCode.OK, "{ \"message\": \"All Good!\" }", out _), "myUser", 1, 1);

        // Assert
        result.HasError.ShouldBeFalse();

        var content = await result.Result!.Content.ReadAsStringAsync();
        content.ShouldBe("{ \"message\": \"All Good!\" }");
    }
    #endregion

    /// <summary>
    /// Build a HttpClient with a mocked handler
    /// That will intercept the call, and send back our own message
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    private static HttpClient GetWantlistInterceptedClient(HttpStatusCode statusCode, string content, out HttpResponseMessage responseMessage)
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
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri("https://api.discogs.com/users/myUser/wants?page=1&per_page=1")),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(responseMessage);

        return new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.discogs.com/")
        };
    }

    /// <summary>
    /// We need to mock the context per test
    /// Which means we will need a specific mocked service each time
    /// </summary>
    /// <returns></returns>
    private WantListService GetWantlistService()
    {
        return new WantListService(_mockContext.Object, _mockApiHelper.Object);
    }
}