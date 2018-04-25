using OA.Ultima.IO;
using System.IO;

namespace OA.Ultima.Resources
{
    /// <summary>
    /// This file contains information about animated effects.
    /// </summary>
    public class EffectDataResource : IResource<EffectData>
    {
        const int Count = 0x0800;
        readonly EffectData[][] _animData;

        public EffectDataResource()
        {
            // From http://wpdev.sourceforge.net/docs/guide/node167.html:
            // There are 2048 blocks, 8 entries per block, 68 bytes per entry.
            // Thanks to Krrios for figuring out the blocksizes.
            // Each block has an 4 byte header which is currently unknown. The
            // entries correspond with the Static ID. You can lookup an entry
            // for a given static with this formula:
            // Offset = (id>>3)*548+(id&15)*68+4;
            // Here is the record format for each entry:
            // byte[64] Frames
            // byte     Unknown
            // byte     Number of Frames Used
            // byte     Frame Interval
            // byte     Start Interval
            _animData = new EffectData[Count][];
            using (var s = FileManager.GetFile("animdata.mul"))
            {
                var r = new BinaryReader(s);
                for (var i = 0; i < Count; i++)
                {
                    var data = new EffectData[8];
                    var header = r.ReadInt32(); // unknown value.
                    for (var j = 0; j < 8; j++)
                        data[j] = new EffectData(r);
                    _animData[i] = data;
                }
            }
        }

        public EffectData GetResource(int itemID)
        {
            itemID &= FileManager.ItemIDMask;
            return _animData[(itemID >> 3)][itemID & 0x07];
        }
    }
}
