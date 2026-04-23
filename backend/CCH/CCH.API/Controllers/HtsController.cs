using System;
using System.Threading.Tasks;
using CCH.Core.Features.Hts;
using CCH.Core.Features.Hts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

[ApiController]
[Route("api/hts-recommendation")]
public class HtsController : ControllerBase
{
    private readonly IHtsRecommendationService _htsService;

    public HtsController(IHtsRecommendationService htsService)
    {
        _htsService = htsService;
    }

    [HttpGet("{hts_code}")]
    public async Task<ActionResult<HtsRecommendationResponseDto>> GetRecommendation(string hts_code)
    {
        try
        {
            var response = await _htsService.GetRecommendationAsync(hts_code);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            // For external API failures or parsing issues
            return StatusCode(502, new { detail = ex.Message });
        }
    }
}