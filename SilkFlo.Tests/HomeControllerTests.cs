using Xunit;
using SilkFlo.Controllers;
using SilkFlo.Models;
using Microsoft.AspNetCore.Mvc;
using Assert = Xunit.Assert;
using Microsoft.Extensions.Logging;
using Moq;
using NCalc;
using Microsoft.EntityFrameworkCore;

namespace SilkFlo.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly SilkFloDbContext _context;
        private readonly Mock<ILogger<HomeController>> _loggerMock;

        public HomeControllerTests()
        {
            var options = new DbContextOptionsBuilder<SilkFloDbContext>()
                .UseInMemoryDatabase(databaseName: "SilkFloTestDb")
                .Options;

            _context = new SilkFloDbContext(options);
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object, _context);
        }

        [Fact]
        public void Calculate_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            var model = new CalculationViewModel();
            _controller.ModelState.AddModelError("Error", "Invalid model state");

            var result = _controller.Calculate(model);

            Xunit.Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Calculate_ReturnsViewResult_WhenExpressionIsInvalid()
        {
            var model = new CalculationViewModel { Expression = "invalid expression" };

            var result = _controller.Calculate(model);

            Xunit.Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Calculate_ReturnsViewResult_WhenExpressionIsValid()
        {
            var model = new CalculationViewModel { Expression = "2 + 2" };

            var result = _controller.Calculate(model);

            Xunit.Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EvaluateExpression_ReturnsCorrectResult()
        {
            var expression = "2 + 2";

            var result = _controller.EvaluateExpression(expression);

            Xunit.Assert.Equal(4, result);
        }

        [Fact]
        public void EvaluateExpression_ReturnsInfinity_WhenExpressionIsDivisionByZero()
        {
            var expression = "2 / 0";

            var result = _controller.EvaluateExpression(expression);

            Assert.True(double.IsInfinity(result));
        }

        [Fact]
        public void EvaluateExpression_ThrowsException_WhenExpressionIsInvalid()
        {
            var expression = "invalid expression";

            Assert.Throws<EvaluationException>(() => _controller.EvaluateExpression(expression));
        }
    }
}