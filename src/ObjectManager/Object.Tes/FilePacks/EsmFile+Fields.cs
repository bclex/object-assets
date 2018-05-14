using OA.Core;
using System;

namespace OA.Tes.FilePacks
{
    public interface IHaveEDID
    {
        STRVField EDID { get; }
    }

    public interface IHaveMODL
    {
        FILEField MODL { get; }
    }

    public struct STRVField
    {
        public override string ToString() => $"{Value}";
        public string Value;

        public STRVField(UnityBinaryReader r, uint dataSize, ASCIIFormat format = ASCIIFormat.PossiblyNullTerminated)
        {
            Value = r.ReadASCIIString((int)dataSize, format);
        }
    }

    public struct FILEField
    {
        public override string ToString() => $"{Value}";
        public string Value;

        public FILEField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }
    }

    public struct INTVField
    {
        public override string ToString() => $"{Value}";
        public long Value;

        public INTVField(UnityBinaryReader r, uint dataSize)
        {
            switch (dataSize)
            {
                case 1: Value = r.ReadByte(); break;
                case 2: Value = r.ReadLEInt16(); break;
                case 4: Value = r.ReadLEInt32(); break;
                case 8: Value = r.ReadLEInt64(); break;
                default: throw new NotImplementedException($"Tried to read an INTV subrecord with an unsupported size ({dataSize})");
            }
        }
    }

    //[StructLayout(LayoutKind.Explicit)]
    public struct DATVField
    {
        //[FieldOffset(0)]
        public bool ValueB;
        //[FieldOffset(0)]
        public int ValueI;
        //[FieldOffset(0)]
        public float ValueF;
        //[FieldOffset(0)]
        public string ValueS;

        public static DATVField Create(UnityBinaryReader r, uint dataSize, char type)
        {
            switch (type)
            {
                case 'b': return new DATVField { ValueB = r.ReadLEInt32() != 0 };
                case 'i': return new DATVField { ValueI = r.ReadLEInt32() };
                case 'f': return new DATVField { ValueF = r.ReadLESingle() };
                case 's': return new DATVField { ValueS = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated) };
                default: throw new InvalidOperationException();
            }
        }
    }

    public struct FLTVField
    {
        public override string ToString() => $"{Value}";
        public float Value;

        public FLTVField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLESingle();
        }
    }

    public struct BYTEField
    {
        public override string ToString() => $"{Value}";
        public byte Value;

        public BYTEField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadByte();
        }
    }

    public struct IN16Field
    {
        public override string ToString() => $"{Value}";
        public short Value;

        public IN16Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt16();
        }
    }

    public struct IN32Field
    {
        public override string ToString() => $"{Value}";
        public int Value;

        public IN32Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt32();
        }
    }

    public struct UI32Field
    {
        public override string ToString() => $"{Value}";
        public uint Value;

        public UI32Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEUInt32();
        }
    }

    public struct CREFField // COLORREF
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte NullByte;

        public CREFField(UnityBinaryReader r, uint dataSize)
        {
            Red = r.ReadByte();
            Green = r.ReadByte();
            Blue = r.ReadByte();
            NullByte = r.ReadByte();
        }
    }

    public struct NPCOField
    {
        public override string ToString() => $"{ItemName}";
        public uint ItemCount; // Number of the item
        public string ItemName; // The ID of the item

        public NPCOField(UnityBinaryReader r, uint dataSize)
        {
            ItemCount = r.ReadLEUInt32();
            ItemName = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
        }
    }

    public struct UNKNField
    {
        public override string ToString() => $"UNKN";
        public byte[] Value;

        public UNKNField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadBytes((int)dataSize);
        }
    }
}