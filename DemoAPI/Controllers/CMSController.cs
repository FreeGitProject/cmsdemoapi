using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Bson;
using DemoAPI.Entities;

namespace DemoAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/CMS")]
    [ApiController]
    public class CMSController : ControllerBase
    {
        private readonly IMongoCollection<Collection> _collection;

        public CMSController(IMongoClient client)
        {
            var database = client.GetDatabase("CMSDemo");
            _collection = database.GetCollection<Collection>("Collection");
        }

        /// <summary>
        /// Create a new collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost("collection")]
        public async Task<IActionResult> Create(CollectionModel collection)
        {
            try
            {
                if (collection == null)
                {
                    return BadRequest("Invalid CMS item data");
                }
                var newcollection = new Collection
                {
                    Title = collection.Title,
                    Description = collection.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Image = collection.Image,
                    Link = collection.Link
                };

                // Insert the CMSItem into MongoDB
                await _collection.InsertOneAsync(newcollection);

                return Ok(newcollection);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all collections
        /// </summary>
        /// <returns></returns>
        [HttpGet("collections")]
        public async Task<IActionResult> GetAllCollections()
        {
            try
            {
                // Retrieve all collections from MongoDB
                var collections = await _collection.Find(_ => true).ToListAsync();

                return Ok(collections);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a collection
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedCollection"></param>
        /// <returns></returns>
        [HttpPut("collection/{id}")]
        public async Task<IActionResult> Update(string id, CollectionModel updatedCollection)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || updatedCollection == null)
                {
                    return BadRequest("Invalid request");
                }

                // Create an updated Collection object based on the incoming model
                var updatedItem = new Collection
                {
                    Id = id,
                    Title = updatedCollection.Title,
                    Description = updatedCollection.Description,
                    UpdatedAt = DateTime.UtcNow,
                    Image = updatedCollection.Image,
                    Link = updatedCollection.Link
                };

                // Find and replace the existing document in MongoDB
                var result = await _collection.ReplaceOneAsync(c => c.Id == id, updatedItem);

                if (result.ModifiedCount > 0)
                {
                    return Ok(updatedItem);
                }
                else
                {
                    return NotFound(); // Document with specified ID not found
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("collection/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid request");
                }

                // Find and delete the document from MongoDB
                var result = await _collection.DeleteOneAsync(c => c.Id == id);

                if (result.DeletedCount > 0)
                {
                    return Ok(); // Document successfully deleted
                }
                else
                {
                    return NotFound(); // Document with specified ID not found
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a collection by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("collection/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Invalid request");
                }

                // Find the document in MongoDB using the specified ID
                var collectionItem = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

                if (collectionItem != null)
                {
                    return Ok(collectionItem); // Return the document if found
                }
                else
                {
                    return NotFound(); // Document with specified ID not found
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete all collections
        /// </summary>
        /// <returns></returns>
        [HttpDelete("collections")]
        public async Task<IActionResult> DeleteAllCollections()
        {
            try
            {
                // Delete all collections from MongoDB
                var result = await _collection.DeleteManyAsync(Builders<Collection>.Filter.Empty);

                return Ok(result.DeletedCount); // Return the number of deleted documents
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}