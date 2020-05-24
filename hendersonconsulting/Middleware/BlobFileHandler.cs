using HendersonConsulting.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HendersonConsulting.Middleware
{
    public class BlobFileHandler
    {
        public BlobFileHandler(RequestDelegate next)
        {
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string itemPath = context.Request.PathBase + context.Request.Path;
            var storageRepository = (IStorageRepository)context.RequestServices.GetService(typeof(IStorageRepository));

            var imageBlob = await storageRepository.GetImageBlobAsych(itemPath.Substring(1));
            await imageBlob.DownloadToStreamAsync(context.Response.Body);
        }
    }
}
