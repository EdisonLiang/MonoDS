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
using MonoDS.Serialization;
using MiniNoSql.Tests.TestObjects;

namespace MonoDS.Tests
{
	[TestFixture]
	public class StorageProcessorTests
	{
		private string _dataDirectory;
		private string _entity;
		private ISerializer _serializer;
		public StorageProcessorTests()
		{
			// create path and filename to the database file.
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			_dataDirectory = libraryPath;

			_entity = "PersonEntity";
			_serializer = new Serializer();
		}

		[Test]
		public void StoreThreeDocumentsLoadFirstDocument ()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
		
				var person = new PersonEntity() { Name = "Test 2", Email = "Test 2" };
				sp.Store<PersonEntity>(person);
			 	sp.Store<PersonEntity>(new PersonEntity() { Name = "Test 1", Email = "Test 1" });

				var loadPerson =  sp.Load<PersonEntity>(person.Id);
			
			Assert.AreEqual (person.Id, loadPerson.Id);
			}
		}

		[Test]
		public void StoreThreeDocumentsDeleteOneDocumentLoadDeletedDocumentReturnsNull()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
			
			 var person = new PersonEntity() { Name = "Test 2", Email = "Test 2" };
			 sp.Store<PersonEntity>(person);
			 sp.Store<PersonEntity>(new PersonEntity() { Name = "Test 1", Email = "Test 1" });
			 sp.Delete(person.Id);

			var loadPerson =  sp.Load<PersonEntity>(person.Id);
			Assert.IsNull(loadPerson);
			}
		}

		[Test]
		public void Insert10000Documents()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
			for (int i = 1; i < 10001; i++) {
				 sp.Store<PersonEntity>(new PersonEntity() { Name = "Person " + i, Email = "Email " + i });
				#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
				#endif
			}
			}
			Assert.True (true);
		}

		[Test]
		public void CheckDocumentCountIsZero()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
			long count =  sp.Count();
			Assert.AreEqual (0, count);
			}
		}

		[Test]
		public void Insert100DocumentsCheckDocumentCountIs100()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
			for (int i = 1; i < 101; i++) {
				 sp.Store<PersonEntity>(new PersonEntity() { Name = "Person " + i, Email = "Email " + i });
				#if DEBUG
				Console.WriteLine("INSERT: {0}", i);
				#endif
			}
			long count =  sp.Count();
			Assert.AreEqual (100, count);

			}
		}

		[Test]
		public void Insert100RecordsLoad100CheckAllMatchInserted()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
				for (int i = 1; i < 101; i++) {
					sp.Store<PersonEntity>(new PersonEntity() { Name = "Person " + i, Email = "Email " + i });
					#if DEBUG
					Console.WriteLine("INSERT: {0}", i);
					#endif
				}
				long count =  sp.Count();
				Assert.AreEqual (100, count);

				// load
				var records = sp.Load<PersonEntity>();
				int countCheck = 1;
				foreach(var r in records){
					Assert.AreEqual(r.Name, "Person " + countCheck);
					Assert.AreEqual(r.Email, "Email " + countCheck);
					countCheck++;
				}
			}
		}

		[Test]
		public void Insert1RecordsCheckCount1RemoveCheckDocumentCountIs0()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
			var person = new PersonEntity() { Name = "Person " + 1, Email = "Email " + 1 };
			 sp.Store<PersonEntity>(person);
			Assert.AreEqual(1,  sp.Count());
			 sp.Delete(person.Id);
			Assert.AreEqual (0,  sp.Count());
			}
		}

		[Test]
		public void Insert1RecordChangeDataAndUpdateCheckUpdated()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
			var person = new PersonEntity() { Name = "Person", Email = "Email" };
			 sp.Store<PersonEntity>(person);
			

				Assert.AreEqual(1,  sp.Count());

			var existingPerson =  sp.Load<PersonEntity>(person.Id);
			existingPerson.Name = "Person With Longer Name";
			 sp.Update<PersonEntity>(existingPerson);
			Assert.AreEqual(1,  sp.Count());

			// check updated
			var updatedPerson =  sp.Load<PersonEntity>(person.Id);
			Assert.AreEqual ("Person With Longer Name", updatedPerson.Name);
			}
		}

		[Test]
		public void InitialiseStorageProcessor100Times()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
				for (int i = 0; i < 100; i++) {
					sp.Initialise();
				}
			}
			Assert.IsTrue(true);
		}

		[Test]
		public void InsertOneDocumentDeleteDocumentInsertNewDocumentWithSameSizeCheckFileSizeIsSame()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();

				// insert one record and get file size
				var entity = new PersonEntity() { Name = "Test", Email = "Test" };
				var entity2 = new PersonEntity() { Name = "Test", Email = "Test" };
				sp.Store<PersonEntity>(entity);

				// store file size for the assert
				long fileSize = sp.FileSize;

				sp.Delete(entity.Id);
				Assert.AreEqual(fileSize, sp.FileSize);

				// store another documet file size should be the same.
				sp.Store<PersonEntity>(entity2);
				Assert.AreEqual(fileSize, sp.FileSize);
			}
		}

		[Test]
		public void Inser100DocumentsDelete100DocumentsInsert100CheckFileSizeIsSameAsAfterDelete()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
				for (int i = 1; i < 101; i++) {
					sp.Store<PersonEntity>(new PersonEntity() { Id = i, Name = "Test", Email = "Test" });
					#if DEBUG
					Console.WriteLine("File Size: " + sp.FileSize);
					#endif
				}
		
				// store file size for the assert
				long fileSize = sp.FileSize;
				for (int i = 1; i < 101; i++) {
					sp.Delete(i);
					#if DEBUG
					Console.WriteLine("File Size: " + sp.FileSize);
					#endif
				}
				Assert.AreEqual(fileSize, sp.FileSize);
				
				// store another documet file size should be the same.
				for (int i = 1; i < 101; i++) {
					sp.Store<PersonEntity>(new PersonEntity() { Id = i, Name = "Test", Email = "Test" });
					#if DEBUG
					Console.WriteLine("File Size: " + sp.FileSize);
					#endif
				}
				Assert.AreEqual(fileSize, sp.FileSize);
			}
		}

		[Test]
		public void InsertOneDocumentUpdateDocumentSoRelocatedInsertNewDocumentWithSameSizeCheckFileSizeIsSame()
		{
			using (var sp = new StorageProcessor(_dataDirectory, _entity, _serializer))
			{
				sp.DestroyExistingData();
				
				// insert one record and get file size
				var entity = new PersonEntity() { Name = "Test", Email = "Test" };
				var entity2 = new PersonEntity() { Name = "Test", Email = "Test" };
				sp.Store<PersonEntity>(entity);
				
				// store file size for the assert
				entity.Name = "1000000hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh";
				sp.Update<PersonEntity>(entity);

				var fileSize = sp.FileSize;
				Assert.AreEqual(fileSize, sp.FileSize);
				
				// store another documet file size should be the same.
				sp.Store<PersonEntity>(entity2);
				Assert.AreEqual(fileSize, sp.FileSize);
			}
		}

	}
}
