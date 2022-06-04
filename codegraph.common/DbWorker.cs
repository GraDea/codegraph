using System.Collections.Generic;
using System.IO;
using CodeGraph.Common;

public static class DbWorker
{
    public static  void Save(List<QueueModel> model)
    {
        if (File.Exists(StaticData.PathToDb))
        {
            File.Delete(StaticData.PathToDb);
        }
        File.WriteAllText(StaticData.PathToDb,Newtonsoft.Json.JsonConvert.SerializeObject(model));
        
    }
    
    public static List<QueueModel>? Load()
    {
        if (!File.Exists(StaticData.PathToDb))
        {
            return new List<QueueModel>()
            {
                new QueueModel(QueueMemberType.Consumer){ExchangeName = "a", RoutingKey = "a1"},
                new QueueModel(QueueMemberType.Consumer){ExchangeName = "a", RoutingKey = "a1"},
                new QueueModel(QueueMemberType.Publisher){ExchangeName = "a", RoutingKey = "a1"}
            };
        }
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<QueueModel>>(File.ReadAllText(StaticData.PathToDb));
    }
}