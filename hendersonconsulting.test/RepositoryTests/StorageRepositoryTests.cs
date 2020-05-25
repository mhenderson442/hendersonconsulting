using HendersonConsulting.Models;
using HendersonConsulting.Repositories;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HendersonConsulting.Test.RepositoryTests
{
    public class StorageRepositoryTests
    {
        private readonly IStorageRepository _storageRepository;

        public StorageRepositoryTests()
        {
            var logger = new Mock<ILogger<StorageRepository>>();

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.Development.json", false);

            var configuration = configurationBuilder.Build();
            _storageRepository = new StorageRepository(logger.Object, configuration);
        }

        [Fact(DisplayName = "GetBlogPostItemAsync should return a CloudBlobClient")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetBlogAsyncPostItemReturnsCloudBlobClient()
        {
            // Arrange
            var year = "2018";
            var month = "04";
            var day = "07";
            var name = "reboot";

            // Act
            var sut = await _storageRepository.GetBlogPostContentAsync(year, month, day, name);

            // Assert
            Assert.IsType<BlogPostContent>(sut);
        }

        [Fact(DisplayName = "GetPostItemsAsync should return a list of type PostYears")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetBlogPostsAsyncReturnsList()
        {
            // Arrange
            // Act
            var sut = await _storageRepository.GetBlogPostYearsAsync();

            // Assert
            Assert.IsType<List<BlogPostYear>>(sut);
        }

        [Fact(DisplayName = "GetCategoriesAsync should return a list of type Category")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetCategoriesAsyncReturnsList()
        {
            // Arrange
            // Act
            var sut = await _storageRepository.GetCategoriesAsync();

            // Assert
            Assert.IsType<List<Category>>(sut);
        }
        [Fact(DisplayName = "GetStaticPageContent should return a string")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetStaticPageContentReturnsString()
        {
            // Arrange
            var fileName = "sql-style-guide.md"; 

            // Act
            var sut = await _storageRepository.GetStaticPageContentAsync(fileName);

            // Assert
            Assert.IsType<string>(sut);
        }

        [Fact(DisplayName = "GetCloudBlobClient should return a an instance of a CloudBlobClient")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetCloudBlobClientAsyncReturnsCloudBlobClient()
        {
            // Arrange
            // Act
            var sut = await _storageRepository.GetCloudBlobClientAsync();

            // Assert
            Assert.IsType<CloudBlobClient>(sut);
        }

        [Fact(DisplayName = "GetDefaultPostItemAsync should return a CloudBlobClient")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetDefaultPostItemAsyncReturnsCloudBlobClient()
        {
            // Arrange
            // Act
            var sut = await _storageRepository.GetDefaultPostItemAsync();

            // Assert
            Assert.IsType<BlogPostContent>(sut);
        }

        [Fact(DisplayName = "GetImageBlobAsych should return a CloudBlockBlob")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetImageBlobAsychReturnsCloudBlockBlob()
        {
            // Arrange
            var itemPath = "images/test.png";

            // Act
            var sut = await _storageRepository.GetImageBlobAsych(itemPath);

            // Assert
            Assert.IsType<CloudBlockBlob>(sut);
        }

        [Fact(DisplayName = "GetStaticContentBaseUrl should return a string")]
        [Trait("Category", "StorageRepositoryTests")]
        public async Task GetStaticContentBaseUrlAsyncReturnsString()
        {
            // Arrange
            // Act
            var sut = await _storageRepository.GetStaticContentBaseUrlAsync();

            // Assert
            Assert.IsType<string>(sut);
        }
    }
}