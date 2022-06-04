namespace codegraph.common;

public class QueueModel
{
    public string RoutingQueue { get; set; }
    
    public string ExchangeName { get; set; }
    
    public string[] Fields { get; set; }
    
    public string Microservice { get; set; }
    public QueueMemberType MemberType { get; set; }
}

public enum QueueMemberType
{
    Publisher = 1,
    Consumer = 2
}