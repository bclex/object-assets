using OA.Core.Diagnostics;
using OA.Ultima.Data;
using OA.Ultima.IO;
using System;
using System.IO;
using System.Text;

namespace OA.Ultima.Resources
{
    public class TileData
    {
        public static LandData[] LandData = new LandData[0x4000];
        public static ItemData[] ItemData = new ItemData[0x4000];

        public static ItemData ItemData_ByAnimID(int animID)
        {
            for (var i = 0; i < ItemData.Length; i++)
                if (ItemData[i].AnimID == animID)
                    return ItemData[i];
            return new ItemData();
        }

        // Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
        // Stairs IDs, taken from RunUO Data folder (stairs.txt)
        static readonly int[] _stairsID = {
            1006, 1007, 1008, 1009, 1010, 1012, 1014, 1016, 1017,
            1801, 1802, 1803, 1804, 1805, 1807, 1809, 1811, 1812,
            1822, 1823, 1825, 1826, 1827, 1828, 1829, 1831, 1833,
            1835, 1836, 1846, 1847, 1848, 1849, 1850, 1851, 1852,
            1854, 1856, 1861, 1862, 1865, 1867, 1869, 1872, 1873,
            1874, 1875, 1876, 1878, 1880, 1882, 1883, 1900, 1901,
            1902, 1903, 1904, 1906, 1908, 1910, 1911, 1928, 1929,
            1930, 1931, 1932, 1934, 1936, 1938, 1939, 1955, 1956,
            1957, 1958, 1959, 1961, 1963, 1978, 1979, 1980, 1991,
            7600, 7601, 7602, 7603, 7604, 7605, 7606, 7607, 7608,
            7609, 7610, 7611, 7612, 7613, 7614, 7615, 7616, 7617,
            7618, 7619, 7620, 7621, 7622, 7623, 7624, 7625, 7626,
            7627, 7628, 7629, 7630, 7631, 7632, 7633, 7634, 7635,
            7636, 7639
        };
        // Issue 5 - End

        static TileData()
        {
            using (var fileStream = FileManager.GetFile("tiledata.mul"))
            {
                var r = new BinaryReader(fileStream);
                LandData landData;
                ItemData itemData;
                if (fileStream.Length == 3188736) // 7.0.9.0
                {
                    LandData = new LandData[0x4000];
                    for (var i = 0; i < 0x4000; ++i)
                    {
                        landData = new LandData();
                        if (i == 1 || (i > 0 && (i & 0x1F) == 0))
                            r.ReadInt32();
                        var flags = (TileFlag)r.ReadInt64();
                        var textureID = r.ReadInt16();
                        r.BaseStream.Seek(20, SeekOrigin.Current);
                        landData.Flags = flags;
                        landData.TextureID = textureID;
                        LandData[i] = landData;
                    }
                    ItemData = new ItemData[0x10000];
                    for (var i = 0; i < 0x10000; ++i)
                    {
                        itemData = new ItemData();
                        if ((i & 0x1F) == 0)
                            r.ReadInt32();
                        itemData.Flags = (TileFlag)r.ReadInt64();
                        itemData.Weight = r.ReadByte();
                        itemData.Quality = r.ReadByte();
                        itemData.Unknown1 = r.ReadByte();
                        itemData.Unknown2 = r.ReadByte();
                        itemData.Unknown3 = r.ReadByte();
                        itemData.Quantity = r.ReadByte();
                        itemData.AnimID = r.ReadInt16();
                        r.BaseStream.Seek(2, SeekOrigin.Current); // hue?
                        itemData.Unknown4 = r.ReadByte();
                        itemData.Value = r.ReadByte();
                        itemData.Height = r.ReadByte();
                        itemData.Name = Encoding.ASCII.GetString((r.ReadBytes(20)));
                        itemData.Name = itemData.Name.Trim('\0');
                        // binaryReader.BaseStream.Seek(20, SeekOrigin.Current);
                        // Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
                        if (i > 1005 && i < 7640)
                            itemData.IsStairs = !(Array.BinarySearch(_stairsID, i) < 0);
                        // Issue 5 - End
                        ItemData[i] = itemData;
                    }
                }
                else
                {
                    LandData = new LandData[0x4000];
                    for (var i = 0; i < 0x4000; ++i)
                    {
                        landData = new LandData();
                        if ((i & 0x1F) == 0)
                            r.ReadInt32();
                        var flags = (TileFlag)r.ReadInt32();
                        var iTextureID = r.ReadInt16();
                        r.BaseStream.Seek(20, SeekOrigin.Current);
                        landData.Flags = flags;
                        landData.TextureID = iTextureID;
                        LandData[i] = landData;
                    }
                    if (fileStream.Length == 1644544) // 7.0.0.0
                    {
                        ItemData = new ItemData[0x8000];
                        for (var i = 0; i < 0x8000; ++i)
                        {
                            itemData = new ItemData();
                            if ((i & 0x1F) == 0)
                                r.ReadInt32();
                            itemData.Flags = (TileFlag)r.ReadInt32();
                            itemData.Weight = r.ReadByte();
                            itemData.Quality = r.ReadByte();
                            itemData.Unknown1 = r.ReadByte();
                            itemData.Unknown2 = r.ReadByte();
                            itemData.Unknown3 = r.ReadByte();
                            itemData.Quantity = r.ReadByte();
                            itemData.AnimID = r.ReadInt16();
                            r.BaseStream.Seek(2, SeekOrigin.Current); // hue?
                            itemData.Unknown4 = r.ReadByte();
                            itemData.Value = r.ReadByte();
                            itemData.Height = r.ReadByte();
                            itemData.Name = Encoding.ASCII.GetString((r.ReadBytes(20)));
                            itemData.Name = itemData.Name.Trim('\0');
                            // binaryReader.BaseStream.Seek(20, SeekOrigin.Current);
                            // Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
                            if (i > 1005 && i < 7640)
                                itemData.IsStairs = !(Array.BinarySearch(_stairsID, i) < 0);
                            // Issue 5 - End
                            ItemData[i] = itemData;
                        }
                    }
                    else
                    {
                        ItemData = new ItemData[0x4000];
                        for (var i = 0; i < 0x4000; ++i)
                        {
                            itemData = new ItemData();
                            if ((i & 0x1F) == 0)
                                r.ReadInt32();
                            itemData.Flags = (TileFlag)r.ReadInt32();
                            itemData.Weight = r.ReadByte();
                            itemData.Quality = r.ReadByte();
                            itemData.Unknown1 = r.ReadByte();
                            itemData.Unknown2 = r.ReadByte();
                            itemData.Unknown3 = r.ReadByte();
                            itemData.Quantity = r.ReadByte();
                            itemData.AnimID = r.ReadInt16();
                            r.BaseStream.Seek(2, SeekOrigin.Current); // hue?
                            itemData.Unknown4 = r.ReadByte();
                            itemData.Value = r.ReadByte();
                            itemData.Height = r.ReadByte();
                            itemData.Name = Encoding.ASCII.GetString((r.ReadBytes(20)));
                            itemData.Name = itemData.Name.Trim('\0');
                            // binaryReader.BaseStream.Seek(20, SeekOrigin.Current);
                            // Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
                            if (i > 1005 && i < 7640)
                                itemData.IsStairs = !(Array.BinarySearch(_stairsID, i) < 0);
                            // Issue 5 - End
                            ItemData[i] = itemData;
                        }
                    }
                }
                Metrics.ReportDataRead((int)r.BaseStream.Position);
            }
        }
    }

    public struct ItemData
    {
        public int Weight;
        public TileFlag Flags;
        public int Height;
        public int Quality;
        public int Quantity;
        public int AnimID;
        public int Value;
        public string Name;
        public bool IsStairs;

        public byte Unknown1, Unknown2, Unknown3, Unknown4;

        public bool IsBackground
        {
            get { return (Flags & TileFlag.Background) != 0; }
        }

        public bool IsBridge
        {
            get { return (Flags & TileFlag.Bridge) != 0; }
        }

        public int CalcHeight
        {
            get
            {
                if ((Flags & TileFlag.Bridge) != 0) return Height; // / 2;
                else return Height;
            }
        }

        public bool IsAnimation
        {
            get { return (Flags & TileFlag.Animation) != 0; }
        }

        public bool IsContainer
        {
            get { return (Flags & TileFlag.Container) != 0; }
        }

        public bool IsFoliage
        {
            get { return (Flags & TileFlag.Foliage) != 0; }
        }

        public bool IsGeneric
        {
            get { return (Flags & TileFlag.Generic) != 0; }
        }

        public bool IsImpassable
        {
            get { return (Flags & TileFlag.Impassable) != 0; }
        }

        public bool IsLightSource
        {
            get { return (Flags & TileFlag.LightSource) != 0; }
        }

        public bool IsPartialHue
        {
            get { return (Flags & TileFlag.PartialHue) != 0; }
        }

        public bool IsRoof
        {
            get { return (Flags & TileFlag.Roof) != 0; }
        }

        public bool IsDoor
        {
            get { return (Flags & TileFlag.Door) != 0; }
        }

        public bool IsSurface
        {
            get { return (Flags & TileFlag.Surface) != 0; }
        }

        public bool IsWall
        {
            get { return (Flags & TileFlag.Wall) != 0; }
        }

        public bool IsWearable
        {
            get { return (Flags & TileFlag.Wearable) != 0; }
        }

        public bool IsWet
        {
            get { return (Flags & TileFlag.Wet) != 0; }
        }
    }

    public struct LandData
    {
        public TileFlag Flags;
        public int TextureID;

        public bool IsWet
        {
            get { return (Flags & TileFlag.Wet) != 0; }
        }

        public bool IsImpassible
        {
            get { return (Flags & TileFlag.Impassable) != 0; }
        }
    }
}