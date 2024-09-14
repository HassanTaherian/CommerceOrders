using Contracts.UI.Recommendation;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Asa02_SalesOrdersModule.Controllers
{
    [ApiController, Route("/api/[controller]")]
    public class RecommendController : Controller
    {
        private readonly IRecommendService _recommendService;

        public RecommendController(IRecommendService recommendService)
        {
            _recommendService = recommendService;
        }

        // POST: RecommendController/UI
        [HttpPost]
        public async Task<IActionResult> RecommendUi(RecommendationRequestDto recommendationRequestDto)
        {
            var relatedItems = await _recommendService.Recommended(recommendationRequestDto);
            return Ok(relatedItems);
        }
    }
}
