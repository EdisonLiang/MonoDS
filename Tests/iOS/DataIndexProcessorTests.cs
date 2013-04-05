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
using System.IO;
using MonoDS.Storage;
using MonoDS.Exceptions;

namespace MonoDS.Tests
{
	[TestFixture]
	public class DataIndexProcessorTests
	{
		private string _databasePath;
		private string _dataIndexPath;
		private string _deletedDataIndexPath;
		public DataIndexProcessorTests()
		{
			// create path and filename to the database file.
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			_databasePath = Path.Combine (libraryPath, "MonoDS");

			_dataIndexPath = String.Format(@"{0}\{1}-DataIndexDebug.idx", _databasePath, "PersonEntity");
			_deletedDataIndexPath = String.Format(@"{0}\{1}-DeletedDataIndexDebug.idx", _databasePath, "PersonEntity");
		}
		
		private void Delete()
		{
			if (File.Exists (_dataIndexPath))
				File.Delete (_dataIndexPath);
			if (File.Exists (_deletedDataIndexPath))
				File.Delete (_deletedDataIndexPath);	
		}

		[Test]
		public void DeleteDataIndexSuccess ()
		{
			Delete();
			//var deletedDataIndexProcessor = new DeletedDataIndexProcessor(_deletedDataIndexPath);
			var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath);
			var dataIndex = new DataIndex(1, 1, 100, 50);
			dataIndexProcessor.AddIndex(dataIndex);
			dataIndexProcessor.RemoveIndex(1);

			bool doesExist = dataIndexProcessor.DoesIndexExist(1);
			Assert.False (doesExist);
		}



		[Test]
		public void AddDataIndexSuccess ()
		{
			Delete();
			//var deletedDataIndexProcessor = new DeletedDataIndexProcessor(_deletedDataIndexPath);
			var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath);
			var dataIndex = new DataIndex(1, 1, 100, 50);
			dataIndexProcessor.AddIndex(dataIndex);

			Assert.True (true);
		}

		[Test]
		public void AddDataIndexFailsDuplicate ()
		{
			Delete();
			//var deletedDataIndexProcessor = new DeletedDataIndexProcessor(_deletedDataIndexPath);
			var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath);
			bool duplicateFound = false;
			try {
				var dataIndex = new DataIndex(1, 1, 100, 50);
				dataIndexProcessor.AddIndexCheckForDuplicate(dataIndex);
				dataIndexProcessor.AddIndexCheckForDuplicate(dataIndex);
			} catch (ConcurrencyException ex) {
				if (!String.IsNullOrEmpty (ex.Message))
					duplicateFound = true;	
				#if DEBUG
				Console.WriteLine(ex.Message);
				#endif
			}
			Assert.True(duplicateFound);
		}


		[Test]
		public void GetDataIndexWithEnoughSpaceReturnsNull()
		{
			Delete();
			//var deletedDataIndexProcessor = new DeletedDataIndexProcessor(_deletedDataIndexPath);
			var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath);

			// insert an index with a total space of 200 bytes (100 * 0.2)
			var dataIndex = new DataIndex(1, 1, 100, 50);
			dataIndexProcessor.AddIndexCheckForDuplicate(dataIndex);

			var loaded = dataIndexProcessor.GetDataIndexWithEnoughSpace(500);
			Assert.IsNull(loaded);
		}

		[Test]
		public void GetDataIndexWithEnoughSpaceReturnsDataIndex()
		{
			Delete();
			//var deletedDataIndexProcessor = new DeletedDataIndexProcessor(_deletedDataIndexPath);
			var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath);
			
			// insert an index with a total space of 200 bytes (100 * 0.2)
			var dataIndex = new DataIndex(1, 1, 100, 50);
			dataIndexProcessor.AddIndexCheckForDuplicate(dataIndex);
			
			var loaded = dataIndexProcessor.GetDataIndexWithEnoughSpace(150);
			Assert.IsNotNull(loaded);
		}

		[Test]
		public void DeleteIndexAddIndexCheckOverwrite()
		{
			Delete();
			var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath);
			
			// insert an index with a total space of 200 bytes (100 * 0.2)
			var dataIndex = new DataIndex(1, 1, 100, 0);
			var dataIndex2 = new DataIndex(2, 1, 100, 0);
			dataIndexProcessor.AddIndexOverwriteDeleted(dataIndex);
			dataIndexProcessor.RemoveIndex(1);

			var fileSizeBeforeAdd = dataIndexProcessor.FileSize;
			dataIndexProcessor.AddIndexOverwriteDeleted(dataIndex2);

			Assert.AreEqual(fileSizeBeforeAdd, dataIndexProcessor.FileSize);
		}


	}
}
