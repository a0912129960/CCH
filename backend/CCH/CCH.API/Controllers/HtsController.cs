using System;
using System.Threading.Tasks;
using CCH.Core.Features.Hts;
using CCH.Core.Features.Hts.Interfaces;
using CCH.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCH.API.Controllers;

[Authorize]
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
    public async Task<ActionResult<ApiResponse<HtsRecommendationResponseDto>>> GetRecommendation(string hts_code)
    {
        try
        {
            var response = await _htsService.GetRecommendationAsync(hts_code);
            return Ok(ApiResponse<HtsRecommendationResponseDto>.SuccessResponse(response));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(502, ApiResponse<object>.FailureResponse(ex.Message));
        }
    }
}