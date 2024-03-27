using Microsoft.Extensions.Caching.Distributed;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace ApiRegistration.Db
{
    public class Redis
    {
        public readonly IDistributedCache cache;

        public Redis(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public T? GetData<T>(string key)
        {
            string? value = cache.GetString(key);

            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public bool TryGetValue<T>(string key, out T? value)
        {
            T? data = GetData<T>(key);
            if (data is null)
            {
                value = default;
                return false;
            }
            value = data;
            return true;
        }

        public void SetData<T>(string key, T? value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(value, options);
            cache.SetString(key, jsonString);
        }
    }
}
