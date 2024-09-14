using Newtonsoft.Json;

namespace Services.External
{
    public class JsonBridge<TIn,TOut>
    {
        public string Serialize(TIn item)
        {
            return JsonConvert.SerializeObject(item);
        }

        public string SerializeList(List<TIn> items)
        {
            return JsonConvert.SerializeObject(items);
        }

        public TOut? Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<TOut>(json);
        }

        public List<TOut>? DeserializeList(string json)
        {
            return JsonConvert.DeserializeObject<List<TOut>>(json);
        }
    }
}
