using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using MimeTypeMap;

namespace CID_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CIDupload : ControllerBase
    {   
             [HttpPost("CID2")]
        public IActionResult Post([FromBody] string imageUrl)
        {
           
           using (var client = new WebClient())
            {
                // Download the image from the provided URL
                byte[] imageBytes = client.DownloadData(imageUrl);

                // Determine the image extension based on the image data
                string imageExtension = MimeTypeMap.GetExtension(imageBytes);

                // If the extension is not found or not a known image format, use a default extension like ".jpg"
                if (string.IsNullOrEmpty(imageExtension) || !imageExtension.StartsWith("."))
                {
                    imageExtension = ".jpg";
                }

                // Create the images folder if it doesn't exist
                string imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                // Generate a unique filename with the original name and extension
                string imageFileName = Path.GetFileName(new Uri(imageUrl).LocalPath) + imageExtension;
                string imagePath = Path.Combine(imagesFolderPath, imageFileName);

                // Save the image to the images folder
                using (var imageFileStream = new FileStream(imagePath, FileMode.Create))
                {
                    imageFileStream.Write(imageBytes, 0, imageBytes.Length);
                }

                return Ok(new { imagePath });
            }
        }
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file selected.");

                string currentDirectory = Directory.GetCurrentDirectory();
                string uploadsFolder = Path.Combine(currentDirectory, "images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName =   file.FileName ; 
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                string imageUrl = Path.Combine(currentDirectory, "images", uniqueFileName);

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error while uploading the file: " + ex.Message);
            }
        }
    }

    }
