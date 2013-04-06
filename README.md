MonoDS
==

MonoDS is a local NoSQL Document Storage Database for use with Xamarin iOS and Android.
With MonoDS you can store and retrieve domain objects without creating database tables and mapping code.

Getting Started
--

1. Download the MonoDS.dll and Newtonsoft.dll from the Build folder.
2. Add the two DLL files as references to your project.

```csharp
using System;
using System.IO;
using MonoDS;

public class Person
{
  public long Id {get; set;} // Required (Don't set for autoincrement)
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
```
Notes
--
1. MonoDS is still being developed and is not ready for production use. 
2. Currently domain objects must have an Id property and it must be a short, int or long.


Performance
--
1. Unit tests show around 5x faster insert performance than Sqlite for 10,000 documents.
2. Autoincrement is faster than specifying a key value because lookups are faster with no duplicate checks. 

Roadmap
--
1. Handle power failures and crashes using transaction logging.
2. Support Querys as currently all documents of a certain type need loaded and sorted using LINQ.
2. Change tracking and Unit of Work.

Bugs
--
For bugs and feature requests please open a [new issue](https://github.com/toneuk/MonoDS/issues).


License
--
* Freely distributable under the terms of the [MIT License](http://www.opensource.org/licenses/MIT)
* MIT License allows the library to be used in Apps published to the App store's. 




