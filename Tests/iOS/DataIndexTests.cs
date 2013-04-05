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
using NUnit.Framework;
using MonoDS.Storage;

namespace MonoDS.Tests
{
	[TestFixture]
	public class DataIndexTests
	{
		[Test]
		public void CreateDataIndexGetBytesLengthOf32 ()
		{
			var dataIndex = new DataIndex(1, 1, 1, 50);
			var bytes = dataIndex.GetBytes();

			int count = bytes.Length;
			Assert.AreEqual(32, count);
		}

		[Test]
		public void CreateDataIndexGetBytesAndConvertBackToDataIndex()
		{
			// create the initial index and convert to bytes.
			var dataIndex = new DataIndex(1, 1, 1, 50);
			var bytes = dataIndex.GetBytes();

			// convert the bytes back into a data index structure
			var newDataIndex = DataIndex.Parse(bytes);

			Assert.AreEqual(dataIndex.DocumentKey, newDataIndex.DocumentKey);
			Assert.AreEqual(dataIndex.Pointer, newDataIndex.Pointer);
			Assert.AreEqual(dataIndex.RecordLength, newDataIndex.RecordLength);
		}

		[Test]
		public void UpdateRecordLengthCheckRequiresRelocation()
		{
			// create the initial index and convert to bytes.
			var dataIndex = new DataIndex(1, 1, 100, 100);
			Assert.IsFalse(dataIndex.RequiresRelocation);

			dataIndex.UpdateRecordLength(301);
			Assert.IsTrue(dataIndex.RequiresRelocation);
		}

		[Test]
		public void UpdateRecordLengthCheckDoesNotRequireRelocation()
		{
			// create the initial index and convert to bytes.
			var dataIndex = new DataIndex(1, 1, 100, 100);
			Assert.IsFalse(dataIndex.RequiresRelocation);
			
			dataIndex.UpdateRecordLength(200);
			Assert.IsFalse(dataIndex.RequiresRelocation);
		}

		[Test]
		public void UpdateRecordLengthNoPaddingFactorCheckDoesRequireRelocation()
		{
			// create the initial index and convert to bytes.
			var dataIndex = new DataIndex(1, 1, 100, 0);
			Assert.IsFalse(dataIndex.RequiresRelocation);
			
			dataIndex.UpdateRecordLength(101);
			Assert.IsTrue(dataIndex.RequiresRelocation);
		}

		[Test]
		public void CreateDataIndexWith50PercentPaddingFactorResultIn50BytesPaddingLength()
		{
			var dataIndex = new DataIndex(1, 1, 100, 50);
			Assert.AreEqual(50, dataIndex.PaddingLength);
		}

	}
}