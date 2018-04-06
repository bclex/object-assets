using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;
using OA.Ultima.Resources;
using System;
using System.Collections.Generic;

namespace OA.Ultima.Network.Client
{
    public class AsciiSpeechPacket : SendPacket
    {
        public AsciiSpeechPacket(MessageTypes type, int font, int hue, string lang, string text)
            : base(0xAD, "Ascii Speech")
        {
            // get triggers
            int triggerCount; int[] triggers;
            SpeechData.GetSpeechTriggers(text, lang, out triggerCount, out triggers);
            if (triggerCount > 0)
                type = type | MessageTypes.EncodedTriggers;
            Stream.Write((byte)type);
            Stream.Write((short)hue);
            Stream.Write((short)font);
            Stream.WriteAsciiNull(lang);
            if (triggerCount > 0)
            {
                var t = new byte[(int)Math.Ceiling((triggerCount + 1) * 1.5f)];
                // write 12 bits at a time. first write count: byte then half byte.
                t[0] = (byte)((triggerCount & 0x0FF0) >> 4);
                t[1] = (byte)((triggerCount & 0x000F) << 4);
                for (var i = 0; i < triggerCount; i++)
                {
                    var index = (int)((i + 1) * 1.5f);
                    if (i % 2 == 0) // write half byte and then byte
                    {
                        t[index + 0] |= (byte)((triggers[i] & 0x0F00) >> 8);
                        t[index + 1] = (byte)(triggers[i] & 0x00FF);
                    }
                    else // write byte and then half byte
                    {
                        t[index] = (byte)((triggers[i] & 0x0FF0) >> 4);
                        t[index + 1] = (byte)((triggers[i] & 0x000F) << 4);
                    }
                }
                Stream.BaseStream.Write(t, 0, t.Length);
                Stream.WriteAsciiNull(text);
            }
            else Stream.WriteBigUniNull(text);
        }

        List<int> getSpeechTriggers(string text)
        {
            var triggers = new List<int>();
            return triggers;
        }
    }
}
