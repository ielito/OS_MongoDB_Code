using Newtonsoft.Json;

namespace MongoDB_Code.Services
{
    public static class Extensions
    {
        public static T DeepCopy<T>(this T obj)
        {
            var serialized = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
