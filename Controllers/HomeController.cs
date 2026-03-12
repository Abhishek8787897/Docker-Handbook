using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HandbookChatbot.Models;
using HandbookChatbot.Services;
using System.Threading.Tasks;

namespace HandbookChatbot.Controllers;

public class HomeController : Controller
{
    private readonly HandbookService _handbookService;

    public HomeController(HandbookService handbookService)
    {
        _handbookService = handbookService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        string response = await _handbookService.AskHandbookAsync(request.Message);
        
        return Ok(new { answer = response });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

