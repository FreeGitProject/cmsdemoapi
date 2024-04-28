using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Bson;
namespace DemoAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/CMS")]
    [ApiController]
    public class CMSController : ControllerBase
    {
        private readonly IMongoCollection<CMSItem> _cmsItems;

        public CMSController(IMongoClient client)
        {
            var database = client.GetDatabase("CMSDemo");
            _cmsItems = database.GetCollection<CMSItem>("Collection");
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCMSItem([FromBody] CMSItem cmsItem)
        {
            //var newItem = new CMSItem
            //{
            //    Id = ObjectId.GenerateNewId(),
            //    Title = "Example Title",
            //    Description = "Example Description",
            //    CreatedAt = DateTime.UtcNow,
            //    UpdatedAt = DateTime.UtcNow,
            //    Image = "example.jpg",
            //    Link = "https://example.com"
            //};
            try
            {
                if (cmsItem == null)
                {
                    return BadRequest("Invalid CMS item data");
                }

                // Validate the model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Set CreatedAt and UpdatedAt timestamps
                cmsItem.Id = ObjectId.GenerateNewId();
                cmsItem.CreatedAt = DateTime.UtcNow;
                cmsItem.UpdatedAt = DateTime.UtcNow;

                // Insert the CMSItem into MongoDB
                await _cmsItems.InsertOneAsync(cmsItem);

                return Ok(cmsItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
