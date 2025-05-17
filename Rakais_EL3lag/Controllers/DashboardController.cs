using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rakais_EL3lag.Models;
using Rakais_EL3lag.Models.Dto;
using System.Linq;

namespace Rakais_EL3lag.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class DashboardController : ControllerBase
    {
        private readonly RakaizContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long _maxFileSize = 20 * 1024 * 1024; // 5MB

        public DashboardController(RakaizContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

       
        #region Image Endpoints

       

        // GET: api/Dashboard/images/{id}
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        // GET: api/Dashboard/images/by-section/{sectionName}
        [HttpGet("images/by-section/{sectionName}")]
       
        public async Task<IActionResult> GetImagesBySectionName(string sectionName)
        {
            var section = await _context.Sections
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Name == sectionName);

            if (section == null) return NotFound("Section not found");

            var images = section.Images.Select(img => new ImagesDto
            {
                Id = img.Id,
                
                Url = img.ImageUrl,
                IsActive = img.Active
            });

            return Ok(images);
        }


        // POST: api/Dashboard/upload-image
        [HttpPost("upload-image")]
        [RequestSizeLimit(_maxFileSize)]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDto imageDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validate file
            if (imageDto.ImageFile == null || imageDto.ImageFile.Length == 0)
                return BadRequest("No file uploaded");

            var fileExtension = Path.GetExtension(imageDto.ImageFile.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
                return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", _allowedExtensions)}");

            if (imageDto.ImageFile.Length > _maxFileSize)
                return BadRequest($"File size exceeds {_maxFileSize / (1024 * 1024)}MB limit");

            // Handle section
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Name == imageDto.SectionName);
            if (section == null)
            {
                section = new Section { Name = imageDto.SectionName };
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();
            }

            // Save file
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var imagesFolder = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var filePath = Path.Combine(imagesFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageDto.ImageFile.CopyToAsync(stream);
            }

            // Create image record
            var image = new Image
            {
                SectionId = section.Id,
                
                FileName = uniqueFileName,
                Active = imageDto.Active
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return Ok(new { id = image.Id });
        }

        // PUT: api/Dashboard/images/{id}
        [HttpPut("update-image/{id}")]
        [RequestSizeLimit(_maxFileSize)]
        public async Task<IActionResult> UpdateImage(int id, [FromForm] ImageUpdateDto imageDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var image = await _context.Images.Include(i => i.Section).FirstOrDefaultAsync(i => i.Id == id);
            if (image == null) return NotFound();

           
      

            // ✅ Replace Image File if new file uploaded
            if (imageDto.ImageFile != null && imageDto.ImageFile.Length > 0)
            {
                var fileExtension = Path.GetExtension(imageDto.ImageFile.FileName).ToLower();
                if (!_allowedExtensions.Contains(fileExtension))
                    return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", _allowedExtensions)}");

                if (imageDto.ImageFile.Length > _maxFileSize)
                    return BadRequest($"File size exceeds {_maxFileSize / (1024 * 1024)}MB limit");

                var imagesFolder = Path.Combine(_environment.WebRootPath, "images");

                // Delete old file
                var oldFilePath = Path.Combine(imagesFolder, image.FileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save new file
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var newFilePath = Path.Combine(imagesFolder, uniqueFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await imageDto.ImageFile.CopyToAsync(stream);
                }

                // Update database file name
                image.FileName = uniqueFileName;
            }

            await _context.SaveChangesAsync();

            return Ok(new { id = image.Id, message = "Image updated successfully." });
        }


        // PATCH: api/Dashboard/images/{id}/activate
        [HttpPatch("images/{id}/activate")]
        public async Task<IActionResult> ActivateImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();

            image.Active = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Dashboard/images/{id}/deactivate
        [HttpPatch("images/{id}/deactivate")]
        public async Task<IActionResult> DeactivateImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();

            image.Active = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Dashboard/images/{id}
        [HttpDelete("delete-image/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();

            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, "images", image.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        #endregion

        #region Question Endpoints

        [HttpGet("Question/by-section/{sectionName}")]
        public async Task<IActionResult> GetQuestionBySectionName(string sectionName)
        {
            var section = await _context.Sections
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Name == sectionName);

            if (section == null) return NotFound("Section not found");

            var Questions = section.Questions.Select(que => new QuestionDto
            {
                Id = que.Id,
                IsActive = que.Active
            });

            return Ok(Questions);
        }

        [HttpPatch("Question/{id}/activate")]
        public async Task<IActionResult> ActivateQuestion(int id)
        {
            var Ques = await _context.Questions.FindAsync(id);
            if (Ques == null) return NotFound();

            Ques.Active = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Dashboard/images/{id}/deactivate
        [HttpPatch("Question/{id}/deactivate")]
        public async Task<IActionResult> DeactivateQuestion(int id)
        {
            var Ques = await _context.Questions.FindAsync(id);
            if (Ques == null) return NotFound();

            Ques.Active = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Dashboard/images/{id}
        [HttpDelete("delete-Question/{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var Ques = await _context.Questions.FindAsync(id);
            if (Ques == null) return NotFound();

            _context.Questions.Remove(Ques);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("update-Question/{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionDto QuestionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var Ques = await _context.Questions.FindAsync(id);
            if (Ques == null) return NotFound();

            Ques.AnswerText = QuestionDto.AnswerText;
            Ques.QuestionText = QuestionDto.QuestionText;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("upload-question")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDto dto)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Name == dto.SectionName);
            if (section == null)
            {
                section = new Section { Name = dto.SectionName };
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();
            }
            var question = new Question
            {
                QuestionText = dto.QuestionText,
                AnswerText = dto.AnswerText
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(question);

        }

        #endregion

        private bool SectionExists(int id) => _context.Sections.Any(e => e.Id == id);
        private bool ImageExists(int id) => _context.Images.Any(e => e.Id == id);
    }

}
