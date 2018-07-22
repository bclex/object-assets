using OA.Core;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OA.Tes.FilePacks
{
    public interface IHaveEDID
    {
        STRVField EDID { get; }
    }

    public interface IHaveMODL
    {
        MODLGroup MODL { get; }
    }

    public class MODLGroup
    {
        public string Value;
        public float Bound;
        public byte[] Textures; // Texture Files Hashes
        public override string ToString() => $"{Value}";

        public MODLGroup(UnityBinaryReader r, int dataSize)
        {
            Value = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }

        public void MODBField(UnityBinaryReader r, int dataSize)
        {
            Bound = r.ReadLESingle();
        }

        public void MODTField(UnityBinaryReader r, int dataSize)
        {
            Textures = r.ReadBytes(dataSize);
        }

    }

    public struct FormId32<TRecord> where TRecord : Record
    {
        public override string ToString() => $"{Type}:{Id}";
        public readonly uint Id;
        public string Type => typeof(TRecord).Name.Substring(0, 4);
    }

    public struct FormId<TRecord> where TRecord : Record
    {
        public override string ToString() => $"{Type}:{Name}{Id}";
        public readonly uint Id;
        public readonly string Name;
        public string Type => typeof(TRecord).Name.Substring(0, 4);

        public FormId(uint id) { Id = id; Name = null; }
        public FormId(string name) { Id = 0; Name = name; }
        FormId(uint id, string name) { Id = id; Name = name; }
        public FormId<TRecord> AddName(string name) => new FormId<TRecord>(Id, name);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorRef3 { public byte Red; public byte Green; public byte Blue; public override string ToString() => $"{Red}:{Green}:{Blue}"; }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorRef4 { public byte Red; public byte Green; public byte Blue; public byte Null; public override string ToString() => $"{Red}:{Green}:{Blue}"; public Color32 ToColor32() => new Color32(Red, Green, Blue, 255); }

    public static class ReaderExtension
    {
        public static INTVField ReadINTV(this UnityBinaryReader r, int length)
        {
            switch (length)
            {
                case 1: return new INTVField { Value = r.ReadByte() };
                case 2: return new INTVField { Value = r.ReadLEInt16() };
                case 4: return new INTVField { Value = r.ReadLEInt32() };
                case 8: return new INTVField { Value = r.ReadLEInt64() };
                default: throw new NotImplementedException($"Tried to read an INTV subrecord with an unsupported size ({length})");
            }
        }

        public static DATVField ReadDATV(this UnityBinaryReader r, int length, char type)
        {
            switch (type)
            {
                case 'b': return new DATVField { B = r.ReadLEInt32() != 0 };
                case 'i': return new DATVField { I = r.ReadLEInt32() };
                case 'f': return new DATVField { F = r.ReadLESingle() };
                case 's': return new DATVField { S = r.ReadASCIIString(length, ASCIIFormat.PossiblyNullTerminated) };
                default: throw new InvalidOperationException($"{type}");
            }
        }

        public static STRVField ReadSTRV(this UnityBinaryReader r, int length, ASCIIFormat format = ASCIIFormat.PossiblyNullTerminated)
        {
            return new STRVField { Value = r.ReadASCIIString(length, format) };
        }

        public static FILEField ReadFILE(this UnityBinaryReader r, int length, ASCIIFormat format = ASCIIFormat.PossiblyNullTerminated)
        {
            return new FILEField { Value = r.ReadASCIIString(length, format) };
        }

        public static BYTVField ReadBYTV(this UnityBinaryReader r, int length)
        {
            return new BYTVField { Value = r.ReadBytes(length) };
        }

        public static UNKNField ReadUNKN(this UnityBinaryReader r, int length)
        {
            return new UNKNField { Value = r.ReadBytes(length) };
        }
    }

    public struct STRVField { public string Value; public override string ToString() => Value; }
    public struct FILEField { public string Value; public override string ToString() => Value; }
    public struct INTVField { public long Value; public override string ToString() => $"{Value}"; public UI16Field ToUI16Field() => new UI16Field { Value = (ushort)Value }; }
    public struct DATVField { public bool B; public int I; public float F; public string S; public override string ToString() => "DATV"; }
    public struct FLTVField { public float Value; public override string ToString() => $"{Value}"; }
    public struct BYTEField { public byte Value; public override string ToString() => $"{Value}"; }
    public struct IN16Field { public short Value; public override string ToString() => $"{Value}"; }
    public struct UI16Field { public ushort Value; public override string ToString() => $"{Value}"; }
    public struct IN32Field { public int Value; public override string ToString() => $"{Value}"; }
    public struct UI32Field { public uint Value; public override string ToString() => $"{Value}"; }

    public struct FMIDField<TRecord> where TRecord : Record
    {
        public override string ToString() => $"{Value}";
        public FormId<TRecord> Value;

        public FMIDField(UnityBinaryReader r, int dataSize)
        {
            Value = dataSize == 4 ?
                new FormId<TRecord>(r.ReadLEUInt32()) :
                new FormId<TRecord>(r.ReadASCIIString(dataSize, ASCIIFormat.ZeroPadded));
        }

        public void AddName(string name)
        {
            Value = Value.AddName(name);
        }
    }

    public struct FMID2Field<TRecord> where TRecord : Record
    {
        public override string ToString() => $"{Value1}x{Value2}";
        public FormId<TRecord> Value1;
        public FormId<TRecord> Value2;

        public FMID2Field(UnityBinaryReader r, int dataSize)
        {
            Value1 = new FormId<TRecord>(r.ReadLEUInt32());
            Value2 = new FormId<TRecord>(r.ReadLEUInt32());
        }
    }

    public struct CREFField { public ColorRef4 Color; }

    public struct CNTOField
    {
        public override string ToString() => $"{Item}";
        public uint ItemCount; // Number of the item
        public FormId<Record> Item; // The ID of the item

        public CNTOField(UnityBinaryReader r, int dataSize, GameFormatId format)
        {
            if (format == GameFormatId.TES3)
            {
                ItemCount = r.ReadLEUInt32();
                Item = new FormId<Record>(r.ReadASCIIString(32, ASCIIFormat.ZeroPadded));
                return;
            }
            Item = new FormId<Record>(r.ReadLEUInt32());
            ItemCount = r.ReadLEUInt32();
        }
    }

    public struct BYTVField { public byte[] Value; public override string ToString() => $"BYTS"; }
    public struct UNKNField { public byte[] Value; public override string ToString() => $"UNKN"; }
}