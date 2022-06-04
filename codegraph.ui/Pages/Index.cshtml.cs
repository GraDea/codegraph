using codegraph.common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace codegraph.ui.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    public IEnumerable<QueueModel> Publishers { get; set; }  
    public IEnumerable<QueueModel> Consumers { get; set; }  
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string name)
    {
        var items =  DbWorker.Load();
        if(string.IsNullOrWhiteSpace(name) ||items==null) return;
        Consumers = items.Where(lst => (lst.ExchangeName == name || lst.RoutingKey == name) && lst.MemberType ==  QueueMemberType.Consumer);
        Publishers = items.Where(lst => (lst.ExchangeName == name || lst.RoutingKey == name) && lst.MemberType ==  QueueMemberType.Publisher);
    }
}