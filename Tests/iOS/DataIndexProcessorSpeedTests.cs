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

namespace MonoDS.Tests
{
	public class DataIndexProcessorSpeedTests
	{
		private string _databasePath;
		private string _dataIndexPath;

		public DataIndexProcessorSpeedTests ()
		{
			// create path and filename to the database file.
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			_databasePath = Path.Combine (libraryPath, "MonoDS");

			// create directory
			if (!Directory.Exists(_databasePath)){
				Directory.CreateDirectory(_databasePath);
			}

			_dataIndexPath = Path.Combine(_databasePath, String.Format(@"{0}-DataIndexDebug.idx", "PersonEntity"));
		}

		private void Delete ()
		{
			if (File.Exists (_dataIndexPath))
				File.Delete (_dataIndexPath);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingDisabled ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath)) {
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndex (dataIndex);
					#if DEBUG
					Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
	
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingDisabledWith8KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Small, DiskBufferSize.Small)) {
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndex (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingDisabledWith16KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Medium, DiskBufferSize.Medium)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndex (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingDisabledWith32KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Large, DiskBufferSize.Large)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndex (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingDisabledWith64KBReadCache ()
		{
			Delete ();
			//var deletedDataIndexProcessor = new DeletedDataIndexProcessor(_deletedDataIndexPath);
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Larger, DiskBufferSize.Larger)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndex (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}

		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingEnabled ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Default, DiskBufferSize.Default)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}

		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingEnabledWith8KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Small, DiskBufferSize.Small)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingEnabledWith16KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Medium, DiskBufferSize.Medium)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingEnabledWith32KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Large, DiskBufferSize.Large)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}
		
		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingEnabledWith64KBReadCache ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Larger, DiskBufferSize.Larger)) {
			
				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
					#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}

		[Test]
		public void Insert1000DataIndexEntriesWithDuplicateCheckingEnabledRandomRead ()
		{
			Delete ();
			using (var dataIndexProcessor = new DataIndexProcessor(_dataIndexPath, DiskBufferSize.Default, DiskBufferSize.Default)) {
			
				// adding a random index forces the check for duplicates to search the full index.
				var dataIndex2 = new DataIndex (100000000, 100, 100, 50);
				dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex2);

				for (int i = 1; i < 1001; i++) {
					var dataIndex = new DataIndex (i, i, i, 50);
					dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
					#if DEBUG
					Console.WriteLine("INSERT: {0}", i);
					#endif
				}
			}
			Assert.IsTrue (true);
		}

		

	}
}

