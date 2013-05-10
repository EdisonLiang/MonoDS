MonoDS - A .NET Document Storage Database for Mobile
==

MonoDS is a local NoSQL Document Storage Database for use with Xamarin iOS and Android.
With MonoDS you can store and retrieve domain objects without creating database tables and mapping code.

Why Use MonoDS?
--
1. Fast performance.
2. No unmanaged dependancies (Completely written in C# .NET).
3. No need to create database tables or mapping code.
4. Data files are kept small by allowing room for growth in the data file. By default all documents are padded with 50% extra space so that updates to a document do not require a new record at the end of the data file. Deleted documents are overwritten by new documents in the data file.

Getting Started
--

1. Download the MonoDS.dll and Newtonsoft.dll from the Build folder.
2. Add the two DLL files as references to your project.
3. The current API is very small. See the code sample's below for available methods:

```csharp
using System;
using System.IO;
using MonoDS;

public class Person
{
  public int Id {get; set;} // Required
  public string Name {get; set;}
  public string Email {get; set;}
}

// get the path to the library folder in iOS
var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
var libraryPath = Path.Combine (documentsPath, "..", "Library");

// store
using (var docStore = new DocumentStore(libraryPath))
{
  var person = new Person() { Name = "MonoDS", Email = "hello@monods.net" };
  docStore.Store<Person>(person);
  Console.WriteLine(person.Id);
}

// load
using (var docStore = new DocumentStore(libraryPath))
{
  docStore.Find<Person>(1);
  Console.WriteLine(person.Name);
}

// update
using (var docStore = new DocumentStore(libraryPath))
{
  var person = docStore.Find<Person>(1);
  person.Name = "MonoDS Changed";
  docStore.Update<Person>(person);
}

// delete
using (var docStore = new DocumentStore(libraryPath))
{
  docStore.Delete<Person>(1);
}

// get all
using (var docStore = new DocumentStore(libraryPath))
{
  docStore.Store<Person>(new Person() { Name = "MonoDS", Email = "hello@monods.net" });
  docStore.Store<Person>(new Person() { Name = "MonoDS2", Email = "hello@monods.net" });
  var persons = docStore.All<Person>(1);
  foreach(var person in persons){
    Console.WriteLine(person.Name);
  }
}

// count
using (var docStore = new DocumentStore(libraryPath))
{
  var count = docStore.Count<Person>();
  Console.WriteLine(count);
}
```
Notes
--
1. Domain objects must have an Id property and it must be a short, int or long.
2. Built and tested on iOS using Xamarin Studio.

Performance
--
1. Unit tests show around 5x faster insert performance than Sqlite for 10,000 documents.
2. Autoincrement is faster than specifying a key value because lookups are faster with no duplicate checks. 

Roadmap
--
1. Support Querys as currently all documents of a certain type need loaded and sorted using LINQ.
2. Change tracking and Unit of Work.
3. Handle power failures and crashes using transaction logging.

Bugs
--
For bugs and feature requests please open a [new issue](https://github.com/toneuk/MonoDS/issues).

Applications using MonoDS
--
- MonoDS is used in the free weight lifting app [MyStrength](https://itunes.apple.com/us/app/mystrength/id634177889?mt=8).


License
--
* Freely distributable under the terms of the [MIT License](http://www.opensource.org/licenses/MIT)
* MIT License allows the library to be used in Apps published to the App store's. 




