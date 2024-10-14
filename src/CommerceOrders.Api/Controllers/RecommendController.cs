using CommerceOrders.Contracts.UI.Recommendation;

namespace CommerceOrders.Api.Controllers;

[ApiController, Route("/api/[controller]")]
public class RecommendController : Controller
{
    private readonly IRecommendService _recommendService;

    public RecommendController(IRecommendService recommendService)
    {
        _recommendService = recommendService;
    }

    [HttpPost]
    public async Task<IActionResult> Recommend(RecommendationRequestDto request) =>
        Ok(await _recommendService.RecommendProducts(request));
}