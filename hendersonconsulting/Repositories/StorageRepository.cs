using HendersonConsulting.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HendersonConsulting.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<IStorageRepository> _logger;

        public StorageRepository(ILogger<IStorageRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _appSettings = new AppSettings
            {
                StorageAccountKey = configuration.GetValue<string>("storageAccountKey"),
                StorageAccountName = configuration.GetValue<string>("storageAccountName"),
                BlogPostContainer = configuration.GetValue<string>("blogPostContainer"),
                ImagesContainer = configuration.GetValue<string>("imagesContainer"),
                StaticContainer = configuration.GetValue<string>("staticContainer")
            };
        }

        public async Task<BlogPostContent> GetBlogPostItemAsync(string year, string month, string day, string name)
        {
            var blobName = $"{ year }/{ month }/{ day }/{ name }.md";
            var client = await GetCloudBlobClientAsync();

            var container = client.GetContainerReference(_appSettings.BlogPostContainer);
            var blogPostItem = container.GetBlockBlobReference(blobName);

            var stream = await blogPostItem.OpenReadAsync(null, null, new OperationContext());
            var datePosted = await FormatDatePostedString(blogPostItem.Parent.Prefix);

            using (var reader = new StreamReader(stream))
            {
                var contentBuilder = new StringBuilder();
                var blobContent = await reader.ReadToEndAsync();

                var blogPostContent = new BlogPostContent
                {
                    DatePosted = datePosted,
                    PageContent = blobContent
                };

                return blogPostContent;
            }
        }

        public async Task<List<BlogPostYear>> GetBlogPostListAsync()
        {
            var blogPostContainer = _appSettings.BlogPostContainer;
            var blobList = await GetBlobList(blogPostContainer);

            if (blobList.Count == 0)
            {
                return null;
            }

            var list = await GetBlogYears(blobList);

            return list;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var client = await GetCloudBlobClientAsync();
            var container = client.GetContainerReference(_appSettings.StaticContainer);

            var categoriesFile = container.GetBlockBlobReference("categories.json");
            var operatopnContext = new OperationContext();

            var stream = await categoriesFile.OpenReadAsync(null, null, operatopnContext);

            using (var reader = new StreamReader(stream))
            {
                var list = JsonConvert.DeserializeObject<List<Category>>(await reader.ReadToEndAsync());
                list.ForEach(f => f.BlogPostItems.ForEach(fx => fx.Name = FormatName(fx.Name)));

                return list;
            }
        }

        public async Task<CloudBlobClient> GetCloudBlobClientAsync()
        {
            var cloudStorageAccount = GetCloudStorageAccount(_appSettings.StorageAccountName, _appSettings.StorageAccountKey);
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            return await Task.Run(function: () => cloudBlobClient);
        }

        public async Task<BlogPostContent> GetDefaultPostItemAsync()
        {
            var blobList = await GetBlobList(_appSettings.BlogPostContainer);
            var baseDate = DateTime.Now.AddYears(-2);

            var blob = blobList
                        .Where(x => Convert.ToInt32(x.Parent.Prefix.Substring(0, 4)) >= baseDate.Year)
                        .OrderByDescending(x => Convert.ToInt32(x.Parent.Prefix.Substring(0, 4)))
                        .ThenByDescending(x => Convert.ToInt32(x.Parent.Prefix.Substring(5, 2)))
                        .ThenByDescending(x => Convert.ToInt32(x.Parent.Prefix.Substring(8, 2)))
                        .ThenByDescending(x => x.Properties.LastModified)
                        .FirstOrDefault();

            var operationContext = new OperationContext();
            var stream = await blob.OpenReadAsync(null, null, operationContext);

            var datePosted = await FormatDatePostedString(blob.Parent.Prefix);

            using var reader = new StreamReader(stream);
            var contentBuilder = new StringBuilder();

            var blobContent = await reader.ReadToEndAsync();

            var blogPostContent = new BlogPostContent
            {
                DatePosted = datePosted,
                PageContent = blobContent
            };

            return blogPostContent;
        }

        public async Task<CloudBlockBlob> GetImageBlobAsych(string itemPath)
        {
            var blobList = await GetBlobList(_appSettings.ImagesContainer);

            var imageBlob = blobList
                .Where(x => x.GetType() == typeof(CloudBlockBlob))
                .Select(x => (CloudBlockBlob)x)
                .FirstOrDefault(x => x.Name == itemPath);

            return await Task.Run(() => imageBlob);
        }

        public async Task<string> GetStaticContentBaseUrlAsync()
        {

            var cloudStorageAccount = GetCloudStorageAccount(_appSettings.StorageAccountName, _appSettings.StorageAccountKey);
            var staticContentUrl = await Task.Run(function: () => string.Format("{0}/{1}", cloudStorageAccount.BlobEndpoint.ToString().TrimEnd('/'), _appSettings.BlogPostContainer.TrimStart('/')));

            return staticContentUrl;
        }

        private static int ConvertStringToInt(string input)
        {
            var result = int.TryParse(input, out int output);
            var converted = result ? output : 0;

            return converted;
        }

        private static async Task<string> FormatDatePostedString(string prefix)
        {
            var year = prefix.Substring(0, 4);
            var month = GetMonthLong(prefix.Substring(5, 2));

            var day = prefix.Substring(8, 2);
            var datePostedString = await Task.Run(function: () => $"Posted { day } { month } { year }");

            return datePostedString;
        }

        private static string FormatName(string input)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(input);
        }

        private static string FormatName(string input, string prefix)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            var output = input
                .Replace(prefix, string.Empty)
                .Replace(".md", string.Empty);

            var formattedName = textInfo.ToTitleCase(output);

            return formattedName;
        }

        private static CloudStorageAccount GetCloudStorageAccount(string storageAccountName, string storageAccountKey)
        {
            var cloudStorageAccount = new CloudStorageAccount(new StorageCredentials(storageAccountName, storageAccountKey), true);
            return cloudStorageAccount;
        }

        private static string GetMonthLong(string month)
        {
            var monthLong = month switch
            {
                "01" => "January",
                "02" => "February",
                "03" => "March",
                "04" => "April",
                "05" => "May",
                "06" => "June",
                "07" => "July",
                "08" => "August",
                "09" => "September",
                "10" => "October",
                "11" => "November",
                "12" => "December",
                _ => month,
            };
            return monthLong;
        }

        private async Task<List<CloudBlockBlob>> GetBlobList(string container)
        {
            var blobClient = await GetCloudBlobClientAsync();
            var containerReference = blobClient.GetContainerReference(container);

            var results = new List<IListBlobItem>();
            BlobContinuationToken continuationToken = null;

            do
            {
                var response = await containerReference.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.All, 10, continuationToken, null, null);
                continuationToken = response.ContinuationToken;

                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            var blobList = results
                .Where(x => x.GetType() == typeof(CloudBlockBlob))
                .Select(x => (CloudBlockBlob)x)
                .ToList();

            return blobList;
        }

        private async Task<List<BlogPostDay>> GetBlogDays(List<CloudBlockBlob> blobList)
        {
            var posts = await GetBlogPosts(blobList);

            var days = blobList
                .GroupBy(x => x.Parent.Prefix)
                .Select(grp => grp.First())
                .ToList()
                .Select(selector: x => new BlogPostDay
                {
                    Prefix = x.Parent.Prefix,
                    Day = ConvertStringToInt(x.Name.Substring(8, 2)),
                    DayName = x.Name.Substring(8, 2),
                    BlogPostList = posts.Where(y => y.Prefix == x.Parent.Prefix).ToList()
                })
                .ToList();

            return await Task.Run(function: () => days);
        }

        private async Task<List<BlogPostMonth>> GetBlogMonths(List<CloudBlockBlob> blobList)
        {
            var items = await GetBlogPosts(blobList);

            var months = blobList
                .GroupBy(x => x.Parent.Parent.Prefix)
                .Select(grp => grp.First())
                .ToList()
                .Select(x => new BlogPostMonth
                {
                    Prefix = x.Parent.Parent.Prefix,
                    Month = ConvertStringToInt(x.Name.Substring(5, 2)),
                    MonthName = GetMonthLong(x.Name.Substring(5, 2)),
                    BlogPostItems = items.Where(y => y.Prefix.Substring(0, 8) == x.Parent.Parent.Prefix).ToList()
                })
                .ToList();

            return await Task.Run(function: () => months);
        }

        private async Task<List<BlogPostItem>> GetBlogPosts(List<CloudBlockBlob> blobList)
        {
            var posts = blobList
                .Where(x => x.GetType() == typeof(CloudBlockBlob))
                .Select(x => (CloudBlockBlob)x)
                .ToList()
                .Select(selector: x => new BlogPostItem
                {
                    Prefix = x.Parent.Prefix,
                    Name = FormatName(x.Name, x.Parent.Prefix)
                })
                .ToList();

            return await Task.Run(function: () => posts);
        }

        private async Task<List<BlogPostYear>> GetBlogYears(List<CloudBlockBlob> blobList)
        {
            var months = await GetBlogMonths(blobList);

            var years = blobList
                .GroupBy(x => x.Parent.Parent.Parent.Prefix)
                .Select(grp => grp.First())
                .ToList()
                .Select(x => new BlogPostYear
                {
                    Prefix = x.Parent.Parent.Parent.Prefix,
                    Year = ConvertStringToInt(x.Name.Substring(0, 4)),
                    Months = months.Where(y => y.Prefix.Substring(0, 5) == x.Parent.Parent.Parent.Prefix).ToList()
                })
                .OrderByDescending(x => x.Year)
                .ToList();

            return await Task.Run(function: () => years);
        }
    }
}