using Newtonsoft.Json;
using Framework.Core;

namespace Framework.Cache
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Validate the Key entry
        /// </summary>
        /// <param name="key">key</param>
        internal static void Validate(this string key)
        {
            key.ThrowIfNull("The key is null or empty");
        }

        /// <summary>
        /// Serialize an object to JSON format
        /// </summary>
        /// <param name="input">object</param>
        /// <returns></returns>
        internal static string ToJSON(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Deserialize an object from JSON 
        /// </summary>
        /// <param name="input">JSON content</param>
        /// <returns>param type</returns>
        internal static T FromJSON<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
