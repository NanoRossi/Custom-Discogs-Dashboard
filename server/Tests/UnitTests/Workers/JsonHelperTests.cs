using System.Text.Json;
using System.Text.Json.Nodes;
using DiscogsProxy.Workers;
using Shouldly;

namespace UnitTests.Workers;

public class JsonHelperTests
{
    [Fact]
    public void GetProperyValue_GetsStringProperty()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = "Hello"
        };

        // Act
        var result = input.GetPropertyValue<string>("MyProp");

        // Assert
        result.ShouldBe("Hello");
    }

    [Fact]
    public void GetProperyValue_GetsIntProperty()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = 123
        };

        // Act
        var result = input.GetPropertyValue<int>("MyProp");

        // Assert
        result.ShouldBe(123);
    }

    [Fact]
    public void GetProperyValue_GetsBoolProperty()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = true
        };

        // Act
        var result = input.GetPropertyValue<bool>("MyProp");

        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void GetProperyValue_GetsUriProperty()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = "https://example.com"
        };

        // Act
        var result = input.GetPropertyValue<Uri>("MyProp");

        // Assert
        result!.ToString().ShouldBe("https://example.com/");
    }

    [Fact]
    public void GetPropertyValue_GetsPropertyMultipleLevelsDeep()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = new JsonObject
            {
                ["MyOtherProp"] = new JsonObject
                {
                    ["MyFinalProp"] = "Hi"
                }
            }
        };

        // Act
        var result = input.GetPropertyValue<string>("MyProp", "MyOtherProp", "MyFinalProp");

        // Assert
        result.ShouldBe("Hi");
    }

    [Fact]
    public void GetPropertyValue_GetsArray()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = new JsonObject
            {
                ["MyArray"] = new JsonArray
                {
                    new JsonObject { ["MyFinalProp"] = "Not Me!"},
                    new JsonObject { ["MyFinalProp"] = "Me!"}
                }
            }
        };

        // Act
        var result = input.GetPropertyValue<JsonArray>("MyProp", "MyArray");

        // Assert
        result.ShouldBeOfType(typeof(JsonArray));
        result.Count.ShouldBe(2);
        result.First()!.AsObject()["MyFinalProp"]!.ToString().ShouldBe("Not Me!");
        result.Last()!.AsObject()["MyFinalProp"]!.ToString().ShouldBe("Me!");
    }

    [Fact]
    public void GetPropertyValue_GetsFromArray()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = new JsonObject
            {
                ["MyArray"] = new JsonArray
                {
                    new JsonObject { ["MyFinalProp"] = "Not Me!"},
                    new JsonObject { ["MyFinalProp"] = "Me!"}
                }
            }
        };

        // Act
        // Passing in 1 as a param, so we will get index 1 from the array
        var result = input.GetPropertyValue<string>("MyProp", "MyArray", "1", "MyFinalProp");

        // Assert
        result.ShouldBe("Me!");
    }

    [Fact]
    public void GetPropertyValue_CanGetListFromJsonArray()
    {
        // Arrange
        var input = new JsonObject
        {
            ["MyProp"] = new JsonObject
            {
                ["MyArray"] = new JsonArray
                {
                    "1", "2", "3"
                }
            }
        };

        // Act
        var result = input.GetPropertyValue<List<string>, string>("MyProp", "MyArray");

        // Assert
        result.ShouldBeOfType(typeof(List<string>));
        result.Count.ShouldBe(3);
        result[0].ShouldBe("1");
        result[1].ShouldBe("2");
        result[2].ShouldBe("3");
    }
}