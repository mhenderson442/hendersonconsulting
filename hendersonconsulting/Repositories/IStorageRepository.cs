using HendersonConsulting.Models;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HendersonConsulting.Repositories
{

    public interface IStorageRepository
    {
        Task<List<BlogPostYear>> GetBlogPostYearsAsync();

        Task<CloudBlobClient> GetCloudBlobClientAsync();

        Task<BlogPostContent> GetDefaultPostItemAsync();

        Task<string> GetStaticContentBaseUrlAsync();

        Task<BlogPostContent> GetBlogPostContentAsync(string year, string month, string day, string name);

        Task<CloudBlockBlob> GetImageBlobAsych(string itemPath);

        Task<List<Category>> GetCategoriesAsync();
    }
}
