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
using System.IO;
using Mono.Data.Sqlite;
using NUnit.Framework;
using MonoDS.Storage;
using MonoDS.Serialization;
using MonoDS.Tests.TestObjects;

namespace MonoDS.Tests
{
	[TestFixture]
	public class SqliteVsMonoDSSpeedTests
	{

		private string _db;
		private string _dataDirectory;
		private string _entity;
		private readonly ISerializer _serializer;

		public SqliteVsMonoDSSpeedTests ()
		{
			// create path and filename to the database file.
			var documents = Environment.GetFolderPath (
				Environment.SpecialFolder.Personal);
			_db = Path.Combine (documents, "mydb.db3");

			if (File.Exists (_db))
				File.Delete (_db);

			SqliteConnection.CreateFile (_db);
			var conn = new SqliteConnection("URI=" + _db);
			using (var c = conn.CreateCommand()) {
				c.CommandText = "CREATE TABLE DataIndex (SearchKey INTEGER NOT NULL,Name TEXT NOT NULL,Email Text NOT NULL)";
				conn.Open ();
				c.ExecuteNonQuery ();
				conn.Close();
			}
			conn.Dispose();

			// create path and filename to the database file.
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			_dataDirectory = Path.Combine (libraryPath, "MonoDS");
			_entity = "PersonEntity";
			_serializer = new Serializer();
		}

		[Test]
		public void SqliteInsert10000Entries()
		{
			using (var conn = new SqliteConnection("URI=" + _db)) {
				SqliteCommand c;
				conn.Open ();
				for (int i = 1; i < 10001; i++) {

					c = new SqliteCommand ( String.Format("INSERT INTO DataIndex (SearchKey, Name, Email) VALUES ({0}, '{1}', '{2}')", i, "Name " + i, "Email" + i) , conn );
						c.ExecuteNonQuery ();
						c.Dispose();
					
					#if DEBUG
					Console.WriteLine("INSERT: {0}", i);
					#endif
				}
				conn.Close();
			}
			Assert.IsTrue(true);
		}

		[Test]
		public void MonoDSInsert10000Documents()
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


	}
}