using Microsoft.AspNetCore.Builder;

namespace HendersonConsulting.Middleware
{
    public static class BlobFileHandlerExtensions
    {
        public static IApplicationBuilder UseBlobFileHandler(this IApplicationBuilder builder)
        {
            var applicaitonBuilder = builder.UseMiddleware<BlobFileHandler>();

            return applicaitonBuilder;
        }
    }
}