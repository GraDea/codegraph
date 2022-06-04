namespace codegraph.common;

public class DbWorker
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
            return default;
        }
        return Newtonsoft.Json.JsonConvert.DeserializeObject<List<QueueModel>>(File.ReadAllText(StaticData.PathToDb));
    }
}