using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.ProjectModel;
using UAM.API.Models;

namespace UAM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController(UaVersionsContext context) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllVersions()
        {
            return Ok(await context.Versions.ToListAsync());
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUpdate(string build)
        {
            var ver = await context.Versions.FirstOrDefaultAsync(v => v.Build == build);

            if (ver == null)
                return NotFound();
            
            var path = $"Files/{ver.Path}";

            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            
            return File(bytes, "application/zip");
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUpdateById(Guid id)
        {
            var ver = await context.Versions.FirstOrDefaultAsync(v => v.Id == id);

            if (ver == null)
                return NotFound();
            
            var path = $"Files/{ver.Path}";

            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            
            return File(bytes, "application/zip");
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLastUpdate()
        {
            var version = await context.Versions.OrderByDescending(v => v.Timestamp).FirstOrDefaultAsync();

            return Ok(version);
        }
        
        [HttpPost("[action]")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> AddUpdate(IFormFile uploadedFile)
        {
            if (uploadedFile.ContentType != "application/x-zip-compressed")
                throw new FileFormatException("available only zip");

            var path = $"Files/{uploadedFile.FileName}";

            await using (var streamWriter = new StreamWriter(path))
            {
                await uploadedFile.CopyToAsync(streamWriter.BaseStream);
            }

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateVersion(Models.Version version)
        {
            try
            {
                await context.Versions.AddAsync(version);

                await context.SaveChangesAsync();

                return Ok(version);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProblems()
        {
            return Ok(await context.Problems.Include(p => p.Status).ToListAsync());
        }
    }
}