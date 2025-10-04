using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;


namespace MusicProject.Controllers;

public class HomeMusicController : Controller
{
    private readonly ILogger<HomeMusicController> _logger;

    public HomeMusicController(ILogger<HomeMusicController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
}