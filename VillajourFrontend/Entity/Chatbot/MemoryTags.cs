﻿namespace Villajour.Domain.Entities.Chatbot;

/// <summary>
/// Tag names for kernel memory.
/// </summary>
internal static class MemoryTags
{
    /// <summary>
    /// Associates memory with a specific chat
    /// </summary>
    public const string TagChatId = "chatid";

    /// <summary>
    /// Associates memory with specific type.
    /// </summary>
    public const string TagMemory = "memory";
}
