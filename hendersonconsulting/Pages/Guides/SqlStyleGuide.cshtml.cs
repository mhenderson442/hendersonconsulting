using HendersonConsulting.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HendersonConsulting.Pages.Guides
{
    public class SqlStyleGuideModel : PageModel
    {
        private readonly IStorageRepository _storageRepository;

        public SqlStyleGuideModel(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }

        public string PageContent { get; set; }

        public async Task OnGetAsync()
        {
            var fileName = "sql-style-guide.md";
            PageContent = await _storageRepository.GetStaticPageContentAsync(fileName);
        }
    }
}