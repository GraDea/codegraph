namespace CodeGraph.Common;

public class QueueModel
{
    public QueueModel(QueueMemberType memberType)
    {
        MemberType = memberType;
    }
    
    public string RoutingKey { get; set; }
    
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