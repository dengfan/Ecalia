﻿/*!
Copyright 2014 Yaminike

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkMapleLib.Helpers
{
    /// <summary>
    /// Class to handle reading data from an byte array
    /// </summary>
    public class ArrayReader
    {
        /// <summary>
        /// Buffer holding the packet data
        /// </summary>
        private byte[] Buffer { get; set; }

        /// <summary>
        /// Length of the packet
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The position to start reading on
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Bytes left to read
        /// </summary>
        public int Available
        {
            get { return Length - Position; }
        }

        /// <summary>
        /// Creates a new instance of a ArrayReader using <paramref name="data"/>
        /// </summary>
        /// <param name="length">Max length to use</param>
        public ArrayReader(byte[] data, int length = -1)
        {
            Length = length > data.Length ? length : data.Length;
            Buffer = new byte[Length];
            System.Buffer.BlockCopy(data, 0, Buffer, 0, Length);
        }

        /// <summary>
        /// Read function to set the new position 
        /// </summary>
        /// <param name="length">The length to read</param>
        /// <returns>The current position</returns>
        private int StartRead(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length", "Length cannot be zero or negative");

            int sPosition = Position;
            Position += length;
            if (Available < 0)
            {
                Position = sPosition; //restore old
                throw new Exception("Not enough data");
            }

            return sPosition;
        }

        /// <summary>
        /// Reads a bool from the buffer
        /// </summary>
        public bool ReadBool()
        {
            return Buffer[StartRead(1)] > 0;
        }

        /// <summary>
        /// Reads a signed byte from the buffer
        /// </summary>
        public sbyte ReadSByte()
        {
            return (sbyte)Buffer[StartRead(1)];
        }

        /// <summary>
        /// Reads a unsigned byte from the buffer
        /// </summary>
        public byte ReadByte()
        {
            return Buffer[StartRead(1)];
        }

        /// <summary>
        /// Reads an byte array from the buffer
        /// </summary>
        public byte[] ReadBytes(int length)
        {
            byte[] toRead = new byte[length];
            System.Buffer.BlockCopy(Buffer, StartRead(length), toRead, 0, length);
            return toRead;
        }

        /// <summary>
        /// Reads a signed short from the buffer
        /// </summary>
        public short ReadShort()
        {
            return BitConverter.ToInt16(Buffer, StartRead(2));
        }

        /// <summary>
        /// Reads a unsigned short from the buffer
        /// </summary>
        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(Buffer, StartRead(2));
        }

        /// <summary>
        /// Reads a signed int from the buffer
        /// </summary>
        public int ReadInt()
        {
            return BitConverter.ToInt32(Buffer, StartRead(4));
        }

        /// <summary>
        /// Reads a unsigned int from the buffer
        /// </summary>
        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(Buffer, StartRead(4));
        }

        /// <summary>
        /// Reads a signed long from the buffer
        /// </summary>
        public long ReadLong()
        {
            return BitConverter.ToInt64(Buffer, StartRead(8));
        }

        /// <summary>
        /// Reads a unsigned long from the buffer
        /// </summary>
        public ulong ReadULong()
        {
            return BitConverter.ToUInt64(Buffer, StartRead(8));
        }

        /// <summary>
        /// Reads an ASCII string from the stream
        /// </summary>
        /// <param name="length">Length to read</param>
        public string ReadString(int length, char nullchar = '.')
        {
            if (length == 0) return String.Empty;

            byte[] bytes = ReadBytes(length);

            char[] ret = new char[bytes.Length];
            for (int x = 0; x < bytes.Length; x++)
            {
                if (bytes[x] < 32 && bytes[x] >= 0)
                    ret[x] = nullchar;
                else
                {
                    int chr = ((short)bytes[x]) & 0xFF;
                    ret[x] = (char)chr;
                }
            }
            if (nullchar != '.')
            {
                return new String(ret).Replace(nullchar.ToString(), "");
            }
            else
            {
                return new String(ret);
            }
        }

        /// <summary>
        /// Reads a MapleString from the buffer
        /// </summary>
        public string ReadMapleString()
        {
            return ReadString(ReadShort());
        }

        /// <summary>
        /// Skips bytes in the stream
        /// </summary>
        /// <param name="length">Amount of bytes to skip</param>
        public void Skip(int length)
        {
            Position += length;
        }

        /// <summary>
        /// Creates an byte array of the current ArrayReader
        /// </summary>
        /// <param name="direct">If true, returns a direct reference of the buffer</param>
        public byte[] ToArray(bool direct = false)
        {
            if (direct)
                return Buffer;
            else
            {
                byte[] toRet = new byte[Buffer.Length];
                System.Buffer.BlockCopy(Buffer, 0, toRet, 0, Buffer.Length);
                return toRet;
            }
        }
    }
}
