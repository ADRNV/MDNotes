using System.Text.Json.Serialization;

namespace MdNotesServer.Infrastructure.Entities
{
    public class JwtAuthResult
    {
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}
