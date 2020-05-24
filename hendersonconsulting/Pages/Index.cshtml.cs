using HendersonConsulting.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HendersonConsulting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorageRepository _storageRepository;

        public IndexModel(ILogger<IndexModel> logger, IStorageRepository storageRepository)
        {
            _logger = logger;
            _storageRepository = storageRepository;
        }

        public string DatePosted { get; set; }

        public string PageContent { get; set; }

        public async Task OnGetAsync()
        {
            var blogPostContent = await _storageRepository.GetDefaultPostItemAsync();
            DatePosted = blogPostContent.DatePosted;
            PageContent = blogPostContent.PageContent;
        }
    }
}