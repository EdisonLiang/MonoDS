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
using NUnit.Framework;
using MonoDS;
using MiniNoSql.Tests.TestObjects;
using System.Linq;

namespace MiniNoSql.Tests
{
	[TestFixture]
	public class DocumentStoreTests
	{
		private string _dataDirectory;

		public DocumentStoreTests ()
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var libraryPath = Path.Combine (documentsPath, "..", "Library");
			_dataDirectory = libraryPath;
		}

		[Test]
		public void StoreMultipleDocumentTypes()
		{
			using (var ds = new DocumentStore(_dataDirectory))
			{
				ds.DestroyAllData();

				var person = new PersonEntity() { Name = "MonoDS", Email = "test" };
				var car = new CarEntity() { Name = "MonoDS", Make = "MonoDS", Model = "MonoDS GTI" };

				ds.Store<PersonEntity>(person);
				ds.Store<CarEntity>(car);

				Assert.AreEqual(1, ds.Count<PersonEntity>());
				Assert.AreEqual(1, ds.Count<CarEntity>());
			}
		}

		[Test]
		public void DeleteMultipleDocumentTypes()
		{
			using (var ds = new DocumentStore(_dataDirectory))
			{
				ds.DestroyAllData();

				var person = new PersonEntity() { Name = "MonoDS", Email = "test" };
				var car = new CarEntity() { Name = "MonoDS", Make = "MonoDS", Model = "MonoDS GTI" };
				
				ds.Store<PersonEntity>(person);
				ds.Store<CarEntity>(car);

				ds.Delete<PersonEntity>(person.Id);
				ds.Delete<CarEntity>(car.Id);
				
				Assert.AreEqual(0, ds.Count<PersonEntity>());
				Assert.AreEqual(0, ds.Count<CarEntity>());
			}
		}

		[Test]
		public void FindMultipleDocumentTypes()
		{
			var person = new PersonEntity() { Name = "MonoDS", Email = "test" };
			var car = new CarEntity() { Name = "MonoDS", Make = "MonoDS", Model = "MonoDS GTI" };

			using (var ds = new DocumentStore(_dataDirectory))
			{
				ds.DestroyAllData();
				ds.Store<PersonEntity>(person);
				ds.Store<CarEntity>(car);
			}

			using (var ds = new DocumentStore(_dataDirectory))
			{
				var loadedPerson = ds.Find<PersonEntity>(person.Id);
				var loadedCar = ds.Find<CarEntity>(car.Id);

				Assert.AreEqual(person.Id, loadedPerson.Id);
				Assert.AreEqual(person.Name, loadedPerson.Name);
				Assert.AreEqual(person.Email, loadedPerson.Email);
				Assert.AreEqual(car.Id, loadedCar.Id);
				Assert.AreEqual(car.Name, loadedCar.Name);
				Assert.AreEqual(car.Make, loadedCar.Make);
				Assert.AreEqual(car.Model, loadedCar.Model);
			}
		}

		[Test]
		public void AllMultipleDocumentTypes()
		{
			// STORE
			using (var ds = new DocumentStore(_dataDirectory))
			{
				ds.DestroyAllData();

				for (int i = 0; i < 100; i++) {
					var person = new PersonEntity() { Name = "MonoDS", Email = "test" };
					var car = new CarEntity() { Name = "MonoDS", Make = "MonoDS", Model = "MonoDS GTI" };

					ds.Store<PersonEntity>(person);
					ds.Store<CarEntity>(car);
				}
			}

			// LOAD ALL
			using (var ds = new DocumentStore(_dataDirectory))
			{
				var loadedPersons = ds.All<PersonEntity>();
				var loadedCars = ds.All<CarEntity>();

				Assert.AreEqual(100, loadedPersons.Count());
				foreach(var person in loadedPersons)
				{
					Assert.AreEqual("MonoDS", person.Name);
					Assert.AreEqual("test", person.Email);
				}

				Assert.AreEqual(100, loadedCars.Count());
				foreach(var car in loadedCars)
				{
					Assert.AreEqual("MonoDS", car.Name);
					Assert.AreEqual("MonoDS", car.Make);
					Assert.AreEqual("MonoDS GTI", car.Model);
				}
			}
		}



	}
}