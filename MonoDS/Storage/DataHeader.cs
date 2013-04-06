//
// Copyright (c) 2013 Tony Mackay <toneuk@viewmodel.net>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;

namespace MonoDS.Storage
{
	public class DataHeader
	{
		// total header length is 64 Bytes 
		private int _fieldLength = 64;

		public Int64 RecordCount {get; private set;}     // 8 Bytes
		public Int64 LastRecordId {get; private set;}    // 8 Bytes
		public Int64 LargestRecordId {get; private set;} // 8 Bytes                             		   
									             		 // 40 Bytes Reserved
		public DataHeader()
		{
			this.RecordCount = 0;
			this.LastRecordId = 0;
			this.LargestRecordId = 0;
		}

		public DataHeader (Int64 recordCount, Int64 nextRecordId, Int64 largestRecordId)
		{
			this.RecordCount = recordCount;
			this.LastRecordId = nextRecordId;
			this.LargestRecordId = largestRecordId;
		}

		public Int64 GenerateNextRecord()
		{
			this.RecordCount++;
			this.LastRecordId = this.LargestRecordId + 1;
			this.LargestRecordId = this.LastRecordId;
			return this.LastRecordId;
		}

		public Int64 GenerateNextRecord(Int64 documentKey)
		{
			this.RecordCount++;

			// if document key is 0 then auto generate
			if (documentKey == 0){

				// increment the highest record been stored yet and use that.
				this.LargestRecordId++;
				this.LastRecordId = this.LargestRecordId;
				return this.LastRecordId;
			
			}else{

				// check if higher than the last largest record
				if (documentKey > this.LargestRecordId)
					this.LastRecordId = documentKey;

				return this.LastRecordId;
			}
		}

		public void RemoveRecord()
		{
			this.RecordCount--;
		}

		public byte[] GetBytes()
		{
			byte[] recordCount = BitConverter.GetBytes(this.RecordCount);
			byte[] nextRecordId = BitConverter.GetBytes(this.LastRecordId);
			byte[] largestRecordId = BitConverter.GetBytes(this.LargestRecordId);

			// combine the arrays into 20 long
			byte[] bytes = new byte[_fieldLength];
			Buffer.BlockCopy(recordCount, 0, bytes, 0, 8);
			Buffer.BlockCopy(nextRecordId, 0, bytes, 8, 8);
			Buffer.BlockCopy(largestRecordId, 0, bytes, 16, 8);
			
			// If the system architecture is little-endian (that is, little end first), 
			// reverse the byte array. 
			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
			
			return bytes;
		}

		/// <summary>
		/// Parse the 64 Byte array into Data Header object.
		/// </summary>
		/// <param name="bytes">The 64 Byte array.</param>
		public static DataHeader Parse(byte[] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException("byte array is required.");
			if (bytes.Length != 64)
				throw new ArgumentException("byte array of length 64 is required.");
			
			// If the system architecture is little-endian (that is, little end first), 
			// reverse the byte array. 
			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
			
			var dataHeader = new DataHeader();
			dataHeader.RecordCount = BitConverter.ToInt64(bytes, 0);
			dataHeader.LastRecordId = BitConverter.ToInt64(bytes, 8);
			dataHeader.LargestRecordId = BitConverter.ToInt64(bytes, 16);
			return dataHeader;
		}
	}
}