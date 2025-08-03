using DiscogsProxy.DTO;
using DiscogsProxy.Services;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;
using Shouldly;

namespace UnitTests.Services;

public class ImportServiceTests
{
    private readonly Mock<DiscogsContext> _mockDiscogsContext;
    private readonly Mock<IDiscogsApiHelper> _mockApiHelper;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IWantListService> _mockWantlistService;
    private readonly Mock<ICollectionService> _mockCollectionService;
    private readonly ImportService _importService;

    public ImportServiceTests()
    {
        _mockDiscogsContext = new Mock<DiscogsContext>();
        _mockApiHelper = new Mock<IDiscogsApiHelper>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockWantlistService = new Mock<IWantListService>();
        _mockCollectionService = new Mock<ICollectionService>();

        _importService = new ImportService(_mockDiscogsContext.Object,
         _mockApiHelper.Object,
         _mockConfiguration.Object,
          _mockCollectionService.Object,
           _mockWantlistService.Object);
    }

    #region ImportCollection
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ImportCollection_InvalidUsername(string input)
    {
        // Arrange
        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns(input);

        // Act
        var result = await _importService.ImportCollection();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Invalid Username");

        _mockConfiguration.Verify(x => x["DiscogsUsername"], Times.Once);
        _mockCollectionService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ImportCollection_ErrorFromService()
    {
        // Arrange
        var returnErr = new ResultObject<List<CollectionItem>>() { Error = new Exception("Boom") };

        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns("Blah");
        _mockCollectionService.Setup(x => x.GetCollection("Blah", 1, 200)).Returns(Task.FromResult(returnErr));

        // Act
        var result = await _importService.ImportCollection();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Boom");

        _mockConfiguration.Verify(x => x["DiscogsUsername"], Times.Once);
        _mockCollectionService.Verify(x => x.GetCollection("Blah", 1, 200), Times.Once);
    }

    [Fact]
    public async Task ImportCollection_HappyPath()
    {
        // Arrange
        var collectionDbSetMock = new Mock<DbSet<CollectionItem>>();
        _mockDiscogsContext.Setup(x => x.Collection).Returns(collectionDbSetMock.Object);

        var item = new List<CollectionItem>() { new() { Id = 123 } };
        var returnSuccess = new ResultObject<List<CollectionItem>>() { Result = item };

        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns("Blah");
        _mockCollectionService.Setup(x => x.GetCollection("Blah", 1, 200)).Returns(Task.FromResult(returnSuccess));

        // Act
        var result = await _importService.ImportCollection();

        // Assert
        result.HasError.ShouldBeFalse();

        _mockConfiguration.Verify(x => x["DiscogsUsername"], Times.Once);
        _mockCollectionService.Verify(x => x.GetCollection("Blah", 1, 200), Times.Once);
        collectionDbSetMock.Verify(x => x.AddRange(item), Times.Once);
        _mockDiscogsContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region ImportWantlist
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ImportWantlist_InvalidUsername(string input)
    {
        // Arrange
        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns(input);

        // Act
        var result = await _importService.ImportWantlist();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Invalid Username");

        _mockConfiguration.Verify(x => x["DiscogsUsername"], Times.Once);
        _mockWantlistService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ImportWantlist_ErrorFromService()
    {
        // Arrange
        var returnErr = new ResultObject<List<WantlistItem>>() { Error = new Exception("Boom") };

        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns("Blah");
        _mockWantlistService.Setup(x => x.GetWantList("Blah", 1, 200)).Returns(Task.FromResult(returnErr));

        // Act
        var result = await _importService.ImportWantlist();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Boom");

        _mockConfiguration.Verify(x => x["DiscogsUsername"], Times.Once);
        _mockWantlistService.Verify(x => x.GetWantList("Blah", 1, 200), Times.Once);
    }

    [Fact]
    public async Task ImportWantlist_HappyPath()
    {
        // Arrange
        var wantlistdbSetMock = new Mock<DbSet<WantlistItem>>();
        _mockDiscogsContext.Setup(x => x.Wantlist).Returns(wantlistdbSetMock.Object);

        var item = new List<WantlistItem>() { new() { Id = 123 } };
        var returnSuccess = new ResultObject<List<WantlistItem>>() { Result = item };

        _mockConfiguration.SetupGet(x => x["DiscogsUsername"]).Returns("Blah");
        _mockWantlistService.Setup(x => x.GetWantList("Blah", 1, 200)).Returns(Task.FromResult(returnSuccess));

        // Act
        var result = await _importService.ImportWantlist();

        // Assert
        result.HasError.ShouldBeFalse();

        _mockConfiguration.Verify(x => x["DiscogsUsername"], Times.Once);
        _mockWantlistService.Verify(x => x.GetWantList("Blah", 1, 200), Times.Once);
        wantlistdbSetMock.Verify(x => x.AddRange(item), Times.Once);
        _mockDiscogsContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion
}