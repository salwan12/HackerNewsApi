using Microsoft.VisualStudio.TestTools.UnitTesting;
using HackerNews.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using HackerNews.Application.Contracts.Models;
using Moq;
using Xunit;

namespace HackerNewsApi.Controllers.Tests
{
    [TestClass()]
    public class HackerNewsControllerTests
    {
        [TestMethod()]
        public async Task GetNewestStories_ReturnsOkResultWithData()
        {
            // Arrange
            var hackerNewsServiceMock = new Mock<IHackerNewsService>();
            var controller = new HackerNewsController(hackerNewsServiceMock.Object);

            // Assuming you have a sample list of stories for testing
            var sampleStories = new List<HackerNewsModel>
            {
                    new HackerNewsModel
                    {
                        title = "Introduction to GPT-4",
                        url = "https://example.com/intro-to-gpt4",
                    },
                    new HackerNewsModel
                    {
                        title = "Machine Learning Advancements in 2024",
                        url = "https://example.com/ml-advancements-2024",
                    },
                    new HackerNewsModel
                    {
                       title = "Web Development Trends for the Future",
                        url = "https://example.com/web-dev-trends-future",
                    },
                // Add more sample stories as needed
            };
            hackerNewsServiceMock.Setup(service => service.GetNewestStoriesAsync())
                .ReturnsAsync(sampleStories);

            // Act
            var result = await controller.GetNewestStories();

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var returnedStories = Xunit.Assert.IsType<List<HackerNewsModel>>(okResult.Value);
            Xunit.Assert.Equal(sampleStories.Count, returnedStories.Count);
        }

        [TestMethod()]
        public async Task GetNewestStories_ReturnsNotFoundWhenNoStories()
        {
            // Arrange
            var hackerNewsServiceMock = new Mock<IHackerNewsService>();
            var controller = new HackerNewsController(hackerNewsServiceMock.Object);

            // Set up the service to return an empty list
            hackerNewsServiceMock.Setup(service => service.GetNewestStoriesAsync())
                .ReturnsAsync(new List<HackerNewsModel>());

            // Act
            var result = await controller.GetNewestStories();

            // Assert
            var notFoundResult = Xunit.Assert.IsType<NotFoundObjectResult>(result);
            Xunit.Assert.Equal("No newest stories found.", notFoundResult.Value);
        }
    }
}