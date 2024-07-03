using System.Text.Json.Serialization;
using Villajour.Domain.Entities.Chatbot.Interfaces;

namespace Villajour.Domain.Entities.Chatbot;

/// <summary>
/// A chat participant is a user that is part of a chat.
/// A user can be part of multiple chats, thus a user can have multiple chat participants.
/// </summary>
public class ChatParticipant : IStorageEntity
{
    /// <summary>
    /// Participant ID that is persistent and unique.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// User ID that is persistent and unique.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Chat ID that this participant belongs to.
    /// </summary>
    public string ChatId { get; set; }

    /// <summary>
    /// The partition key for the source.
    /// </summary>
    [JsonIgnore]
    public string Partition => UserId;

    public ChatParticipant(string userId, string chatId)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId;
        ChatId = chatId;
    }
}
