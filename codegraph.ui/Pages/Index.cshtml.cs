using codegraph.common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace codegraph.ui.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public IEnumerable<QueueModel> Items { get; set; }  

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string name)
    {
        var items =  DbWorker.Load();
        if(string.IsNullOrWhiteSpace(name) ||items==null) return;
        Items = items.Where(lst => lst.ExchangeName == name || lst.RoutingQueue == name);
    }
}