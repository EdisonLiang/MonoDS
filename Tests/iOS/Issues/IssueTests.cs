
using System;
using NUnit.Framework;
using System.IO;
using MonoDS.Tests.TestObjects;

namespace MonoDS.Tests
{
	[TestFixture]
	public class IssueTests
	{
		private readonly string _dataDirectory;
		public IssueTests()
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
		    _dataDirectory = Path.Combine (documentsPath, "..", "Library");
		}

		[Test]
		public void StoreUpdateAllIssue1 ()
		{
			// issue is a Json reader error after a store, update all workflow.
			var person = new PersonEntity();
			person.Name = "Test Insert";
			person.Email = "Test Insert";

			using (var docStore = new DocumentStore(_dataDirectory))
			{
				docStore.DestroyAllData();
				docStore.Store<PersonEntity>(person);
			}

			using (var docStore = new DocumentStore(_dataDirectory))
			{
				person.Id = 0;
				docStore.Store<PersonEntity>(person);
			}

			using (var docStore = new DocumentStore(_dataDirectory))
			{
				person.Id = 1;
				docStore.Update<PersonEntity>(person);
				
				person.Name = "dftgyuhijmljmxdojijdioi4jdoij3io4jdjio3jdiojkdrftyguhijedrftgyuhiohuvhouv3huir" +
					"thvuih3tiuvhuit4hvoi3uhvtiou3huivthui3vuth3iu";
				person.Name += person.Name;
				person.Name += person.Name;
				person.Name += person.Name;
				person.Name += person.Name;
				docStore.Update<PersonEntity>(person);
			}

			using (var docStore = new DocumentStore(_dataDirectory))
			{
				docStore.All<PersonEntity>();
			}
		
			Assert.True (true);
		}


	}
}
