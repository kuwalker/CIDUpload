using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;

namespace CID_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CIDupload : ControllerBase
    {        [HttpPost]
        public IActionResult Post([FromBody] string imageUrl)
        {
            using (var client = new WebClient())
            {
                // Download the image from the provided URL
                byte[] imageBytes = client.DownloadData(imageUrl);

                // Create the images folder if it doesn't exist
                string imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                // Generate a unique filename for the image
                string imageFileName = Path.GetFileNameWithoutExtension(imageUrl) + "_" + Path.GetRandomFileName() + Path.GetExtension(imageUrl);
                string imagePath = Path.Combine(imagesFolderPath, imageFileName);

                // Save the image to the images folder
                System.IO.File.WriteAllBytes(imagePath, imageBytes);

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
