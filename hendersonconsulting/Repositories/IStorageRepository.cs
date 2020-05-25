using HendersonConsulting.Models;
using Microsoft.Azure.Storage.Blob;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HendersonConsulting.Repositories
{
    public interface IStorageRepository
    {
        Task<BlogPostContent> GetBlogPostContentAsync(string year, string month, string day, string name);

        Task<List<BlogPostYear>> GetBlogPostYearsAsync();

        Task<List<Category>> GetCategoriesAsync();

        Task<CloudBlobClient> GetCloudBlobClientAsync();

        Task<BlogPostContent> GetDefaultPostItemAsync();

        Task<CloudBlockBlob> GetImageBlobAsych(string itemPath);

        Task<string> GetStaticContentBaseUrlAsync();

        Task<string> GetStaticPageContentAsync(string fileName);
    }
}