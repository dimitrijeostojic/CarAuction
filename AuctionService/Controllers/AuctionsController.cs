using AuctionService.Entities.Domain;
using AuctionService.Entities.DTOs;
using AuctionService.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {

        private readonly IAuctionsService auctionsService;

        public AuctionsController(IAuctionsService auctionsService)
        {
            this.auctionsService = auctionsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuctions([FromQuery] DateTime? date)
        {
            var auctionsDto = await auctionsService.GetAllAuctionAsync(date);
            return Ok(auctionsDto);
        }


        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAuctionById([FromRoute] Guid id)
        {
            var auctionDto = await auctionsService.GetAuctionByIdAsync(id);
            if (auctionDto == null)
            {
                return NotFound();
            }
            return Ok(auctionDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto createAuctionDto)
        {
            var auctionDto = await auctionsService.CreateAuctionAsync(createAuctionDto,User);
            if (auctionDto == null)
            {
                return BadRequest("Auction creation failed");
            }
            return CreatedAtAction(nameof(GetAuctionById), new { id = auctionDto.Id }, auctionDto);
        }

        [Authorize]
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto updateAuctionDto)
        {
            try
            {
                var auctionDto = await auctionsService.UpdateAuctionAsync(id, updateAuctionDto, User);
                if (auctionDto == null)
                {
                    return NotFound();
                }
                return Ok(auctionDto);
            }
            catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid id)
        {
            try
            {
                var auctionDto = await auctionsService.DeleteAuctionAsync(id, User);
                if (auctionDto == null)
                {
                    return NotFound();
                }
                return Ok(auctionDto);
            }
            catch (Exception ex)
            {
                return Forbid(ex.Message);
            }
        }

    }
}
