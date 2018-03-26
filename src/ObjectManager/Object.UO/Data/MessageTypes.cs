using System;

namespace OA.Ultima.Data
{
    [Flags]
    public enum MessageTypes
    {
        Normal = 0x00,
        System = 0x01,
        Emote = 0x02,
        SpeechUnknown = 0x03,
        Information = 0x04,     // Overhead information messages
        Label = 0x06,
        Focus = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,
        Guild = 0x0D,
        Alliance = 0x0E,
        Command = 0x0F,
        /// <summary>
        /// This is used for display only. This is not in the UO protocol. Do not send msgs of this type to the server.
        /// </summary>
        PartyDisplayOnly = 0x10,
        EncodedTriggers = 0xC0 // 0x40 + 0x80
    }
}
