using Chat.Application.Queries;
using Chat.Application.Validators;
using Moq;

namespace Chat.UnitTests.Validators;

public class GetServersQueryValidatorTests
{
    private readonly GetServersQueryValidator _validator;

    public GetServersQueryValidatorTests()
    {
        _validator = new GetServersQueryValidator();
    }

    [Fact]
    public async Task Page_IsLessThan1_ReturnsAnError()
    {
        // Arrange
        GetServersQuery query = new(0, 10, "", "");

        // Act
        var result = await _validator.ValidateAsync(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Page"));
    }

    [Fact]
    public async Task Size_IsLessThan1_ReturnsAnError()
    {
        // Arrange
        GetServersQuery query = new(1, 0, "", "");

        // Act
        var result = await _validator.ValidateAsync(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Size"));
    }

    [Fact]
    public async Task Size_IsGreaterThan100_ReturnsAnError()
    {
        // Arrange
        GetServersQuery query = new(1, 101, "", "");

        // Act
        var result = await _validator.ValidateAsync(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Size"));
    }
}
