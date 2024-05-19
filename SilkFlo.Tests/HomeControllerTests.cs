using Xunit;
using SilkFlo.Controllers;
using SilkFlo.Models;
using Microsoft.AspNetCore.Mvc;
using Assert = Xunit.Assert;

public class HomeControllerTests
{
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _controller = new HomeController();
    }

    [Fact]
    public void Calculate_ReturnsViewResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var model = new CalculationViewModel();
        _controller.ModelState.AddModelError("Error", "Invalid model state");

        // Act
        var result = _controller.Calculate(model);

        // Assert
        Xunit.Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Calculate_ReturnsViewResult_WhenExpressionIsInvalid()
    {
        // Arrange
        var model = new CalculationViewModel { Expression = "invalid expression" };

        // Act
        var result = _controller.Calculate(model);

        // Assert
        Xunit.Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Calculate_ReturnsViewResult_WhenExpressionIsValid()
    {
        // Arrange
        var model = new CalculationViewModel { Expression = "2 + 2" };

        // Act
        var result = _controller.Calculate(model);

        // Assert
        Xunit.Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void EvaluateExpression_ReturnsCorrectResult()
    {
        // Arrange
        var expression = "2 + 2";

        // Act
        var result = _controller.EvaluateExpression(expression);

        // Assert
        Xunit.Assert.Equal(4, result);
    }

    [Fact]
    public void EvaluateExpression_ThrowsDivideByZeroException_WhenExpressionIsDivisionByZero()
    {
        // Arrange
        var expression = "2 / 0";

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => _controller.EvaluateExpression(expression));
    }

    [Fact]
    public void EvaluateExpression_ThrowsException_WhenExpressionIsInvalid()
    {
        // Arrange
        var expression = "invalid expression";

        // Act & Assert
        Assert.Throws<Exception>(() => _controller.EvaluateExpression(expression));
    }
}