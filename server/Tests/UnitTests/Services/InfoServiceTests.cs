using DiscogsProxy.DTO;
using DiscogsProxy.Services;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;

namespace UnitTests.Services;

public class InfoServiceTests
{
    private readonly Mock<DiscogsContext> _mockContext;
    private readonly Mock<IDatabaseChecker> _mockDbChecker;
    private readonly Mock<IFactGenerator> _mockFactGenerator;
    private InfoService _infoService;

    public InfoServiceTests()
    {
        _mockContext = new Mock<DiscogsContext>();
        _mockDbChecker = new Mock<IDatabaseChecker>();
        _mockFactGenerator = new Mock<IFactGenerator>();

        _infoService = new(_mockContext.Object, _mockDbChecker.Object, _mockFactGenerator.Object);
    }

    #region GetArtists
    [Fact]
    public void GetArtists_ReturnsError_WhenDbIsDisconnected()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(false);

        // Act
        var result = _infoService.GetArtists();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Cannot connect to Database");
    }

    [Fact]
    public void GetArtists_NoArtists()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Collection).ReturnsDbSet([]);

        // Act
        var result = _infoService.GetArtists();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldBeEmpty();
    }

    [Fact]
    public void GetArtists_ReturnsArtists()
    {
        // Arrange
        var data = new List<CollectionItem>
        {
            new() { ArtistName = ["Radiohead", "Muse"] },
            new() { ArtistName = ["Muse", "Oasis"] },
        };

        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Collection).ReturnsDbSet(data);

        // Act
        var result = _infoService.GetArtists();

        // Assert
        result.HasError.ShouldBeFalse();

        // Muse duplicate is removed
        // And they're ordered alphabetically
        result.Result.ShouldBe(["Muse", "Oasis", "Radiohead"], ignoreOrder: false);
    }
    #endregion

    #region GetGenres
    [Fact]
    public void GetGenres_ReturnsError_WhenDbIsDisconnected()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(false);

        // Act
        var result = _infoService.GetGenres();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Cannot connect to Database");
    }

    [Fact]
    public void GetGenres_NoGenres()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Genres).ReturnsDbSet([]);

        // Act
        var result = _infoService.GetGenres();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldBeEmpty();
    }

    [Fact]
    public void GetGenres_ReturnsGenres()
    {
        // Arrange
        var data = new List<MusicGenre>
        {
            new() { Text = "Pop" },
            new() { Text = "Jazz" },
            new() { Text = "Classical"}
        };

        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Genres).ReturnsDbSet(data);

        // Act
        var result = _infoService.GetGenres();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldBe(["Classical", "Jazz", "Pop"], ignoreOrder: false);
    }
    #endregion

    #region GetStyles
    [Fact]
    public void GetStyles_ReturnsError_WhenDbIsDisconnected()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(false);

        // Act
        var result = _infoService.GetStyles();

        // Assert
        result.HasError.ShouldBeTrue();
        result.Error!.Message.ShouldBe("Cannot connect to Database");
    }

    [Fact]
    public void GetStyles_NoStyles()
    {
        // Arrange
        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Styles).ReturnsDbSet([]);

        // Act
        var result = _infoService.GetStyles();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldBeEmpty();
    }

    [Fact]
    public void GetStyles_ReturnsStyles()
    {
        // Arrange
        var data = new List<MusicStyle>
        {
            new() { Text = "C" },
            new() { Text = "B" },
            new() { Text = "A"}
        };

        _mockDbChecker.Setup(x => x.CanConnect()).Returns(true);
        _mockContext.Setup(x => x.Styles).ReturnsDbSet(data);

        // Act
        var result = _infoService.GetStyles();

        // Assert
        result.HasError.ShouldBeFalse();
        result.Result.ShouldBe(["A", "B", "C"], ignoreOrder: false);
    }
    #endregion
}