﻿using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace OA.Core
{
    /// <summary>
    /// An improved BinaryReader for Unity.
    /// </summary>
    public class UnityBinaryReader : IDisposable
    {
        BinaryReader r;

        // A buffer for read bytes the size of a decimal variable. Created to minimize allocations. 
        byte[] readBuffer = new byte[16];

        public Stream BaseStream
        {
            get { return r.BaseStream; }
        }

        public UnityBinaryReader(Stream input)
        {
            r = new BinaryReader(input, Encoding.UTF8);
        }

        public UnityBinaryReader(Stream input, Encoding encoding)
        {
            r = new BinaryReader(input, encoding);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~UnityBinaryReader()
        {
            Close();
        }

        public void Close()
        {
            if (r != null)
            {
                r.Close();
                r = null;
            }
        }

        public byte ReadByte()
        {
            return r.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return r.ReadSByte();
        }

        public void Read(byte[] buffer, int index, int count)
        {
            r.Read(buffer, index, count);
        }

        public byte[] ReadBytes(int count)
        {
            return r.ReadBytes(count);
        }

        public byte[] ReadRestOfBytes()
        {
            var remainingByteCount = r.BaseStream.Length - r.BaseStream.Position;
            Debug.Assert(remainingByteCount <= int.MaxValue);
            return r.ReadBytes((int)remainingByteCount);
        }

        public void ReadRestOfBytes(byte[] buffer, int startIndex)
        {
            var remainingByteCount = r.BaseStream.Length - r.BaseStream.Position;
            Debug.Assert((startIndex >= 0) && (remainingByteCount <= int.MaxValue) && ((startIndex + remainingByteCount) <= buffer.Length));
            r.Read(buffer, startIndex, (int)remainingByteCount);
        }

        public string ReadASCIIString(int length)
        {
            Debug.Assert(length >= 0);
            return Encoding.ASCII.GetString(r.ReadBytes(length));
        }

        public string ReadPossiblyNullTerminatedASCIIString(int lengthIncludingPossibleNullTerminator)
        {
            Debug.Assert(lengthIncludingPossibleNullTerminator > 0);
            var bytes = r.ReadBytes(lengthIncludingPossibleNullTerminator);
            // Ignore the null terminator.
            var charCount = (ArrayUtils.Last(bytes) != 0) ? bytes.Length : (bytes.Length - 1);
            return Encoding.Default.GetString(bytes, 0, charCount);
        }

        #region Little Endian

        public bool ReadLEBool32()
        {
            return ReadLEUInt32() != 0;
        }

        public ushort ReadLEUInt16()
        {
            r.Read(readBuffer, 0, 2);
            return (ushort)((readBuffer[1] << 8) | readBuffer[0]);
        }
        public uint ReadLEUInt32()
        {
            r.Read(readBuffer, 0, 4);
            return ((uint)readBuffer[3] << 24) | ((uint)readBuffer[2] << 16) | ((uint)readBuffer[1] << 8) | readBuffer[0];
        }
        public ulong ReadLEUInt64()
        {
            r.Read(readBuffer, 0, 8);
            return ((ulong)readBuffer[7] << 56) | ((ulong)readBuffer[6] << 48) | ((ulong)readBuffer[5] << 40) | ((ulong)readBuffer[4] << 32) | ((ulong)readBuffer[3] << 24) | ((ulong)readBuffer[2] << 16) | ((ulong)readBuffer[1] << 8) | readBuffer[0];
        }

        public short ReadLEInt16()
        {
            r.Read(readBuffer, 0, 2);
            return BitConverter.ToInt16(readBuffer, 0);
        }

        public int ReadLEInt32()
        {
            r.Read(readBuffer, 0, 4);
            return BitConverter.ToInt32(readBuffer, 0);
        }

        public long ReadLEInt64()
        {
            r.Read(readBuffer, 0, 8);
            return BitConverter.ToInt32(readBuffer, 0);
        }

        public float ReadLESingle()
        {
            r.Read(readBuffer, 0, 4);
            return BitConverter.ToSingle(readBuffer, 0);
        }

        public double ReadLEDouble()
        {
            r.Read(readBuffer, 0, 8);
            return BitConverter.ToDouble(readBuffer, 0);
        }

        public byte[] ReadLELength32PrefixedBytes()
        {
            var length = ReadLEUInt32();
            return r.ReadBytes((int)length);
        }

        public string ReadLELength32PrefixedASCIIString()
        {
            return Encoding.ASCII.GetString(ReadLELength32PrefixedBytes());
        }

        public Vector2 ReadLEVector2()
        {
            var x = ReadLESingle();
            var y = ReadLESingle();
            return new Vector2(x, y);
        }

        public Vector3 ReadLEVector3()
        {
            var x = ReadLESingle();
            var y = ReadLESingle();
            var z = ReadLESingle();
            return new Vector3(x, y, z);
        }

        public Vector4 ReadLEVector4()
        {
            var x = ReadLESingle();
            var y = ReadLESingle();
            var z = ReadLESingle();
            var w = ReadLESingle();
            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
        /// </summary>
        public Matrix4x4 ReadLEColumnMajorMatrix3x3()
        {
            var matrix = new Matrix4x4();
            for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                {
                    // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                    if (rowIndex <= 2 && columnIndex <= 2) matrix[rowIndex, columnIndex] = ReadLESingle();
                    else matrix[rowIndex, columnIndex] = rowIndex == columnIndex ? 1 : 0;
                }
            return matrix;
        }

        /// <summary>
        /// Reads a row-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
        /// </summary>
        public Matrix4x4 ReadLERowMajorMatrix3x3()
        {
            var matrix = new Matrix4x4();
            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                {
                    // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                    if (rowIndex <= 2 && columnIndex <= 2) matrix[rowIndex, columnIndex] = ReadLESingle();
                    else matrix[rowIndex, columnIndex] = rowIndex == columnIndex ? 1 : 0;
                }
            return matrix;
        }

        public Matrix4x4 ReadLEColumnMajorMatrix4x4()
        {
            var matrix = new Matrix4x4();
            for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                    matrix[rowIndex, columnIndex] = ReadLESingle();
            return matrix;
        }

        public Matrix4x4 ReadLERowMajorMatrix4x4()
        {
            var matrix = new Matrix4x4();
            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                    matrix[rowIndex, columnIndex] = ReadLESingle();
            return matrix;
        }

        public Quaternion ReadLEQuaternionWFirst()
        {
            var w = ReadLESingle();
            var x = ReadLESingle();
            var y = ReadLESingle();
            var z = ReadLESingle();
            return new Quaternion(x, y, z, w);
        }

        public Quaternion ReadLEQuaternionWLast()
        {
            var x = ReadLESingle();
            var y = ReadLESingle();
            var z = ReadLESingle();
            var w = ReadLESingle();
            return new Quaternion(x, y, z, w);
        }

        #endregion

        #region Big Endian

        public bool ReadBEBool32()
        {
            return ReadBEUInt32() != 0;
        }

        public ushort ReadBEUInt16()
        {
            r.Read(readBuffer, 0, 2);
            return (ushort)((readBuffer[0] << 8) | readBuffer[1]);
        }

        public uint ReadBEUInt32()
        {
            r.Read(readBuffer, 0, 4);
            return ((uint)readBuffer[0] << 24) | ((uint)readBuffer[1] << 16) | ((uint)readBuffer[2] << 8) | readBuffer[3];
        }

        public ulong ReadBEUInt64()
        {
            r.Read(readBuffer, 0, 8);
            return ((ulong)readBuffer[0] << 56) | ((ulong)readBuffer[1] << 48) | ((ulong)readBuffer[2] << 40) | ((ulong)readBuffer[3] << 32) | ((ulong)readBuffer[4] << 24) | ((ulong)readBuffer[5] << 16) | ((ulong)readBuffer[6] << 8) | readBuffer[7];
        }

        public short ReadBEInt16()
        {
            var buffer = new byte[2];
            r.Read(buffer, 0, 2);
            Array.Reverse(buffer);
            return BitConverter.ToInt16(readBuffer, 0);
        }

        public int ReadBEInt32()
        {
            var buffer = new byte[4];
            r.Read(buffer, 0, 4);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(readBuffer, 0);
        }

        public long ReadBEInt64()
        {
            var buffer = new byte[8];
            r.Read(buffer, 0, 8);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(readBuffer, 0);
        }

        public float ReadBESingle()
        {
            var buffer = new byte[4];
            r.Read(buffer, 0, 4);
            Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadBEDouble()
        {
            var buffer = new byte[8];
            r.Read(buffer, 0, 8);
            Array.Reverse(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }

        public byte[] ReadBELength32PrefixedBytes()
        {
            var length = ReadBEUInt32();
            return r.ReadBytes((int)length);
        }

        public string ReadBELength32PrefixedASCIIString()
        {
            return Encoding.ASCII.GetString(ReadBELength32PrefixedBytes());
        }

        public Vector2 ReadBEVector2()
        {
            var x = ReadBESingle();
            var y = ReadBESingle();
            return new Vector2(x, y);
        }

        public Vector3 ReadBEVector3()
        {
            var x = ReadBESingle();
            var y = ReadBESingle();
            var z = ReadBESingle();
            return new Vector3(x, y, z);
        }

        public Vector4 ReadBEVector4()
        {
            var x = ReadBESingle();
            var y = ReadBESingle();
            var z = ReadBESingle();
            var w = ReadBESingle();
            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Reads a column-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
        /// </summary>
        public Matrix4x4 ReadBEColumnMajorMatrix3x3()
        {
            var matrix = new Matrix4x4();
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
                for (int rowIndex = 0; rowIndex < 4; rowIndex++)
                {
                    // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                    if (rowIndex <= 2 && columnIndex <= 2) matrix[rowIndex, columnIndex] = ReadBESingle();
                    else matrix[rowIndex, columnIndex] = rowIndex == columnIndex ? 1 : 0;
                }
            return matrix;
        }

        /// <summary>
        /// Reads a row-major 3x3 matrix but returns a functionally equivalent 4x4 matrix.
        /// </summary>
        public Matrix4x4 ReadBERowMajorMatrix3x3()
        {
            var matrix = new Matrix4x4();
            for (int rowIndex = 0; rowIndex < 4; rowIndex++)
                for (int columnIndex = 0; columnIndex < 4; columnIndex++)
                {
                    // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                    if (rowIndex <= 2 && columnIndex <= 2) matrix[rowIndex, columnIndex] = ReadBESingle();
                    else matrix[rowIndex, columnIndex] = rowIndex == columnIndex ? 1 : 0;
                }
            return matrix;
        }

        public Matrix4x4 ReadBEColumnMajorMatrix4x4()
        {
            var matrix = new Matrix4x4();
            for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                    matrix[rowIndex, columnIndex] = ReadBESingle();
            return matrix;
        }

        public Matrix4x4 ReadBERowMajorMatrix4x4()
        {
            var matrix = new Matrix4x4();
            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
                for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                    matrix[rowIndex, columnIndex] = ReadBESingle();
            return matrix;
        }

        public Quaternion ReadBEQuaternionWFirst()
        {
            var w = ReadBESingle();
            var x = ReadBESingle();
            var y = ReadBESingle();
            var z = ReadBESingle();
            return new Quaternion(x, y, z, w);
        }

        public Quaternion ReadBEQuaternionWLast()
        {
            var x = ReadBESingle();
            var y = ReadBESingle();
            var z = ReadBESingle();
            var w = ReadBESingle();
            return new Quaternion(x, y, z, w);
        }
        #endregion
    }
}