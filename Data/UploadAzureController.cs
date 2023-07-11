using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace GettingStartedFileUpload.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadAzureController : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task Upload(IList<IFormFile> UploadFiles)
        {
            string containerAzure = "container";
            string subcontainerAzure = "subcontainer";

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=artvault;AccountKey=MffFLMml6QW4zHHxHL+GGsvrGspy9hA8e85yhBRzATfzvUwFEieKjAcG1b65G/hdeVpy/8wG23jP+ASt7Qw9fg==;EndpointSuffix=core.windows.net";

            try
            {

                Console.Write(" a ");
                // Azure connection string and container name passed as an argument to get the Blob reference of the container.
                var container = new BlobContainerClient(connectionString, containerAzure);

                // Method to create our container if it doesn’t exist.
                var createResponse = await container.CreateIfNotExistsAsync();

                // If container successfully created, then set public access type to Blob.
                if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                {
                    await container.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                }
                    // Method to create a new Blob client.
                    var blob = container.GetBlobClient(subcontainerAzure + "/" + UploadFiles[0].FileName);

                    // If a blob with the same name exists, then we delete the Blob and its snapshots.
                    await blob.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);

                    // Create a file stream and use the UploadSync method to upload the Blob.
                    using (var fileStream = UploadFiles[0].OpenReadStream())
                    {
                        await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = UploadFiles[0].ContentType });
                    }

                if (UploadFiles[0].ContentType == "application/pdf" || UploadFiles[0].ContentType == "application/postscript")
                {
                    Console.Write("a");
                }
                
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "gh,m.jmb";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }
    }
}