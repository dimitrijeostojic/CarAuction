using AuctionService.Entities.DTOs;
using AuctionService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionsService auctionsService;
    private readonly ILogger<AuctionsController> logger;

    public AuctionsController(IAuctionsService auctionsService, ILogger<AuctionsController> logger)
    {
        this.auctionsService = auctionsService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuctions([FromQuery] DateTime? date)
    {
        try
        {
            logger.LogInformation($"Fetching all auctions with date filter: {date?.ToString() ?? "none"}");
            var auctionsDto = await auctionsService.GetAllAuctionAsync(date);
            logger.LogInformation($"Successfully fetched {auctionsDto?.Count()} auctions");
            return Ok(auctionsDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while fetching all auctions: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetAuctionById(Guid id)
    {
        try
        {
            logger.LogInformation($"Fetching auction with ID: {id}");
            var auctionDto = await auctionsService.GetAuctionByIdAsync(id);

            if (auctionDto == null)
            {
                logger.LogWarning($"Auction with ID {id} not found");
                return NotFound();
            }

            logger.LogInformation($"Auction with ID {id} successfully fetched");
            return Ok(auctionDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while fetching auction with ID {id}: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    
    [HttpPost]
    public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto createAuctionDto)
    {
        try
        {
            logger.LogInformation("Creating a new auction...");
            logger.LogDebug($"CreateAuctionDto: {JsonSerializer.Serialize(createAuctionDto)}");

            var auctionDto = await auctionsService.CreateAuctionAsync(createAuctionDto, User);

            if (auctionDto == null)
            {
                logger.LogWarning("Auction creation failed");
                return BadRequest("Auction creation failed");
            }

            logger.LogInformation($"Auction created with ID: {auctionDto.Id}");
            return CreatedAtAction(nameof(GetAuctionById), new { id = auctionDto.Id }, auctionDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while creating auction: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDto updateAuctionDto)
    {
        try
        {
            logger.LogInformation($"Updating auction with ID: {id}");
            logger.LogDebug($"UpdateAuctionDto: {JsonSerializer.Serialize(updateAuctionDto)}");

            var auctionDto = await auctionsService.UpdateAuctionAsync(id, updateAuctionDto, User);

            if (auctionDto == null)
            {
                logger.LogWarning($"Auction with ID {id} not found for update");
                return NotFound();
            }

            logger.LogInformation($"Auction with ID {id} successfully updated");
            return Ok(auctionDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while updating auction with ID {id}: {ex.Message}");
            return Forbid("Update failed");
        }
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteAuction(Guid id)
    {
        try
        {
            logger.LogInformation($"Deleting auction with ID: {id}");

            var auctionDto = await auctionsService.DeleteAuctionAsync(id, User);

            if (auctionDto == null)
            {
                logger.LogWarning($"Auction with ID {id} not found for deletion");
                return NotFound();
            }

            logger.LogInformation($"Auction with ID {id} successfully deleted");
            return Ok(auctionDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error occurred while deleting auction with ID {id}: {ex.Message}");
            return Forbid("Delete failed");
        }
    }
}
