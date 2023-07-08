using System.Text.Json.Serialization;

namespace Talkie.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageType
    {
        Text = 1,
        Transaction = 2,
        File = 3
    }
}