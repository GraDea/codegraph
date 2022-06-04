using System.Collections.Generic;
using System.Linq;
using CodeGraph.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeGraph.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CodeGraphController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<CodeGraphController> _logger;
    public static List<QueueModel>? Items; 

    public CodeGraphController(ILogger<CodeGraphController> logger)
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
