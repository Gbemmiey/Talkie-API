using System.Text.Json.Serialization;

namespace Talkie.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeliveryStatus
    {
        Sent = 1,
        Delivered = 2,
        Read = 3
    }
}