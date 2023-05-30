using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CID_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CIDupload : ControllerBase
    {
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

                string uniqueFileName =  DateTime.Now.ToShortDateString()+ file.FileName ; 
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
