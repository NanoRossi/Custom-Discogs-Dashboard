using DiscogsProxy.DTO;
using DiscogsProxy.Services;
using DiscogsProxy.Constants;
using Shouldly;
using DiscogsProxy.Workers;
using Moq;
using Moq.EntityFrameworkCore;

namespace UnitTests.Services;

public class StatusServiceTests
{
    private readonly Mock<IDatabaseChecker> _mockDbChecker;
    private readonly Mock<DiscogsContext> _mockContext;

    public StatusServiceTests()
    {
        _mockDbChecker = new Mock<IDatabaseChecker>();
        _mockContext = new Mock<DiscogsContext>();
    }

    [Fact]
    public void GetStatus_HandlesDBDisconnect()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(false);

        // Act
        var result = GetStatusService().GetStatus();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldNotBeNull();
        result.Result.CollectionCount.ShouldBeNull();
        result.Result.WantlistCount.ShouldBeNull();
        result.Result.DatabaseStatus.ShouldBe(DbStatus.Disconnected);

        _mockDbChecker.Verify(x => x.CanConnect(), Times.Once);
        _mockDbChecker.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetStatus_NoData()
    {
        // Arrange
        // Will get 0 collection count for both tables
        // Will cause Empty db status
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Collection).ReturnsDbSet([]);
        _mockContext.Setup(x => x.Wantlist).ReturnsDbSet([]);

        // Act
        var result = GetStatusService().GetStatus();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldNotBeNull();
        result.Result.CollectionCount.ShouldBe(0);
        result.Result.WantlistCount.ShouldBe(0);
        result.Result.DatabaseStatus.ShouldBe(DbStatus.Empty);

        _mockDbChecker.Verify(x => x.CanConnect(), Times.Once);
        _mockDbChecker.VerifyNoOtherCalls();

        _mockContext.Verify(x => x.Collection, Times.Once);
        _mockContext.Verify(x => x.Wantlist, Times.Once);
    }

    [Fact]
    public void GetStatus_GetsCollectionInfo()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Collection).ReturnsDbSet([new(), new(), new()]);
        _mockContext.Setup(x => x.Wantlist).ReturnsDbSet([new(), new(), new(), new()]);

        // Act
        var result = GetStatusService().GetStatus();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldNotBeNull();
        result.Result.CollectionCount.ShouldBe(3);
        result.Result.WantlistCount.ShouldBe(4);
        result.Result.DatabaseStatus.ShouldBe(DbStatus.Active);

        _mockDbChecker.Verify(x => x.CanConnect(), Times.Once);
        _mockDbChecker.VerifyNoOtherCalls();

        _mockContext.Verify(x => x.Collection, Times.Once);
        _mockContext.Verify(x => x.Wantlist, Times.Once);
    }

    /// <summary>
    /// We need to mock the context per test
    /// Which means we will need a specific mocked service each time
    /// </summary>
    /// <returns></returns>
    private StatusService GetStatusService()
    {
        return new StatusService(_mockDbChecker.Object, _mockContext.Object);
    }
}