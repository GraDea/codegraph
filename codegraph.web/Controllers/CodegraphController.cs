using codegraph.common;
using Microsoft.AspNetCore.Mvc;

namespace codegraph.web.Controllers;

[ApiController]
[Route("[controller]")]
public class CodegraphController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<CodegraphController> _logger;
    public static List<QueueModel>? Items; 

    public CodegraphController(ILogger<CodegraphController> logger)
    {
        Items = DbWorker.Load(); 

        _logger = logger;
    }

    [HttpGet(Name = "items")]
    public IEnumerable<QueueModel> Get([FromQuery]string name)
    {
        return Items.Where(lst => lst.ExchangeName == name || lst.RoutingKey == name);
    }
}
