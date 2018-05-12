using OA.Core;
using System;

namespace OA.Tes.FilePacks
{
    public class STRVField : Field
    {
        public string Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
        }
    }

    // variable size
    public class INTVField : Field
    {
        public long Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
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

    public class INTVTwoI32Field : Field
    {
        public int Value0, Value1;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            //Debug.Assert(Header.DataSize == 8);
            Value0 = r.ReadLEInt32();
            Value1 = r.ReadLEInt32();
        }
    }

    public class DataField : Field
    {
        public bool ValueB;
        public int ValueI;
        public float ValueF;
        public string ValueS;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            ValueB = r.ReadLEInt32() != 0;
            ValueI = r.ReadLEInt32();
            ValueF = r.ReadLESingle();
            ValueS = r.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
        }
    }

    public class FLTVField : Field
    {
        public float Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLESingle();
        }
    }

    public class ByteField : Field
    {
        public byte Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadByte();
        }
    }
    public class Int32Field : Field
    {
        public int Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt32();
        }
    }
    public class UInt32Field : Field
    {
        public uint Value;

        public override void Read(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEUInt32();
        }
    }

    public class INDXBNAMCNAMGroup
    {
        public INTVField INDX;
        public STRVField BNAM;
        public STRVField CNAM;
    }
}