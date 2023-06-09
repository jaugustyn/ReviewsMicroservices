using System.Text.Json.Serialization;

namespace AuthAPI.Dto.Auth;

public class MessageResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; } = null;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; } = null;

    public MessageResponse(string? message = null, string? errorMessage = null)
    {
        Message = message;
        ErrorMessage = errorMessage;
    }
}