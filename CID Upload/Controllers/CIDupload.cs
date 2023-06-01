using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CID_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CIDupload : ControllerBase
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;

        public CIDupload(IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageFromUrl([FromForm] string imageUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var fileName = Path.GetFileName(imageUrl);

                        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=supportcentralsa;AccountKey=df1j1BL8ENXILlreJYCHOyGGtgi3+Z42uuTHPc3LeYnCw1Us3HTlDsXx4qPUknORo8CSEDgXLWdt+AStU8H7Dw==;EndpointSuffix=core.windows.net";
                        BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

                        BlobClient blobClient = containerClient.GetBlobClient(fileName);
                        await blobClient.UploadAsync(stream, true);

                        string uploadedImageUrl = blobClient.Uri.ToString();

                        return Ok(new { imageUrl = uploadedImageUrl });
                    }

                    return BadRequest("Unable to download the image from the specified URL.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
