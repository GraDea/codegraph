using System;
using System.Collections.Generic;
using System.Linq;
using CodeGraph.Common;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodeGraph.UI.Pages
{
    public class AllModel : PageModel
    {
        public void OnGet()
        {
            var items =  DbWorker.Load()??new List<QueueModel>();
           
            var consumers = items.Where(lst =>  lst.MemberType ==  QueueMemberType.Consumer).ToArray();
            var publishers = items.Where(lst =>  lst.MemberType ==  QueueMemberType.Publisher).ToArray();

            Edges = new List<Tuple<string, string>>();

            foreach (var publisher in publishers)
            {
                var linkedConsumers = consumers.Where(x => x.ExchangeName == publisher.ExchangeName && x.RoutingKey == publisher.RoutingKey);
                foreach (var consumer in linkedConsumers)
                {
                    edges.Add(new Tuple<string, string>(publisher.Microservice, consumer.Microservice));
                }
            }
            
            
        }

        public List<Tuple<string, string>> Edges { get; set; }
        
    }
}