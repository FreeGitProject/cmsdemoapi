using DemoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Bson;
using DemoAPI.Entities;
using System.Net;
using DemoAPI.Models.Collections;
using Newtonsoft.Json;


namespace DemoAPI.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/CMS")]
    [ApiController]
    public class CMSController : ControllerBase
    {
        private readonly IMongoCollection<Collection> _collection;
        private readonly ILogger<CMSController> _logger;

        public CMSController(IMongoClient client, ILogger<CMSController> logger)
        {
            var database = client.GetDatabase("CMSDemo");
            _collection = database.GetCollection<Collection>("Collection");
            _logger = logger;
        }

        /// <summary>
        /// Create a new collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost("collection")]
        public async Task<IActionResult> Create(CollectionModel collection)
        {
            _logger.LogInformation("Create collection : {@collection}", JsonConvert.SerializeObject(collection));
            try
            {
                if (collection == null)
                {
                    return BadRequest("Invalid CMS item data");
                }

                var newCollection = new Collection
                {
                    Title = collection.Title,
                    Description = collection.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Image = collection.Image,
                    Link = collection.Link
                };

                // Insert the new Collection into MongoDB
                await _collection.InsertOneAsync(newCollection);

                // Prepare the response model
                var response = new ResponseModel<Collection>
                {
                    StatusCode = HttpStatusCode.OK,
                    Status = "Success",
                    Message = "Collection created successfully",
                    Result = newCollection
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                var errorResponse = new ResponseModel<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Status = "Error",
                    Error = "Internal server error",
                    Message = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("collections")]
        public async Task<IActionResult> GetAllCollections(string searchTitle = "", int currentPage = 1, int pageSize = 10)
        {
            try
            {
                // Filter definition for title-based search (case-insensitive)
                FilterDefinition<Collection> filter = Builders<Collection>.Filter.Empty;
                if (!string.IsNullOrWhiteSpace(searchTitle))
                {
                    filter = Builders<Collection>.Filter.Regex("Title", new MongoDB.Bson.BsonRegularExpression(searchTitle, "i"));
                }

                // Calculate the number of records to skip based on pagination parameters
                int skip = (currentPage - 1) * pageSize;

                // Retrieve total count of filtered documents in the collection
                long totalRecords = await _collection.CountDocumentsAsync(filter);

                // Retrieve paginated collections from MongoDB with search and pagination
                List<Collection> collections = await _collection
                    .Find(filter)
                    .SortByDescending(c => c.CreatedAt) // Example sorting by createdAt, adjust as needed
                    .Skip(skip)
                    .Limit(pageSize)
                    .ToListAsync();

                // Create a paginated response model
                var response = new ResponseModelPaginated<List<Collection>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Status = "Success",
                    Message = "Collections retrieved successfully",
                    Result = collections,
                    TotalRecords = (int)totalRecords,
                    CurrentPage = currentPage,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Status = "Error",
                    Error = "Internal server error",
                    Message = ex.Message
                });
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

                // Find the existing document in MongoDB using the specified ID
                var collectionItem = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

                if (collectionItem == null)
                {
                    return NotFound(new ResponseModel<object>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Status = "Error",
                        Error = "Not Found",
                        Message = "Collection with specified ID not found"
                    });
                }

                // Create an updated Collection object based on the incoming model
                var updatedItem = new Collection
                {
                    Id = id,
                    Title = updatedCollection.Title,
                    Description = updatedCollection.Description,
                    CreatedAt = collectionItem.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    Image = updatedCollection.Image,
                    Link = updatedCollection.Link
                };

                // Find and replace the existing document in MongoDB
                var result = await _collection.ReplaceOneAsync(c => c.Id == id, updatedItem);

                if (result.ModifiedCount > 0)
                {
                    // Prepare the response
                    var response = new ResponseModel<Collection>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Status = "Success",
                        Message = "Collection updated successfully",
                        Result = updatedItem
                    };

                    return Ok(response);
                }
                else
                {
                    return NotFound(new ResponseModel<object>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Status = "Error",
                        Error = "Not Found",
                        Message = "Collection with specified ID not found"
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                var errorResponse = new ResponseModel<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Status = "Error",
                    Error = "Internal server error",
                    Message = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
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
                    // Document successfully deleted
                    var response = new ResponseModel<object>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Status = "Success",
                        Message = "Collection deleted successfully"
                    };

                    return Ok(response);
                }
                else
                {
                    // Document with specified ID not found
                    return NotFound(new ResponseModel<object>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Status = "Error",
                        Error = "Not Found",
                        Message = "Collection with specified ID not found"
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                var errorResponse = new ResponseModel<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Status = "Error",
                    Error = "Internal server error",
                    Message = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
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
                    // Document found, prepare the response
                    var response = new ResponseModel<Collection>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Status = "Success",
                        Message = "Collection retrieved successfully",
                        Result = collectionItem
                    };

                    return Ok(response);
                }
                else
                {
                    // Document with specified ID not found
                    return NotFound(new ResponseModel<object>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Status = "Error",
                        Error = "Not Found",
                        Message = "Collection with specified ID not found"
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                var errorResponse = new ResponseModel<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Status = "Error",
                    Error = "Internal server error",
                    Message = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
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

                // Prepare the response
                var response = new ResponseModel<long>
                {
                    StatusCode = HttpStatusCode.OK,
                    Status = "Success",
                    Message = $"Deleted {result.DeletedCount} collections",
                    Result = result.DeletedCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                var errorResponse = new ResponseModel<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Status = "Error",
                    Error = "Internal server error",
                    Message = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}