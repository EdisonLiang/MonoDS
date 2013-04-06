MonoDS
==

MonoDS is a local NoSQL Document Storage Database for use with Xamarin iOS and Android.
With MonoDS you can store and retrieve domain objects without creating database tables and mapping code.

Getting Started
--

1. Download the MonoDS.dll and Newtonsoft.dll from the Build folder.
2. Add the two DLL files as references to your project.

```csharp
public class Person
{
  public long Id {get; set;} // Required
  public string Name {get; set;}
  public string Email {get; set;}
}

string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);
string libraryPath = System.IO.Path.Combine (documentsPath, "..", "Library");
      
using (var documentStore = new MonoDS.DocumentStore(libraryPath))
{
  var person = new Person() { Name = "MonoDS", Email = "hello@monods.net" };
  documentStore.Store<Person>(person);
  Console.WriteLine(person.Id);
}
```


Note: MonoDS is still being developed and is not stable. 

Roadmap
--
1. Query support (Currently loads all documents via documentStore.All()).
2. Change tracking and Unit of Work.

Bugs
--
For bugs and feature requests open a [new issue](https://github.com/toneuk/MonoDS/issues).


License
--
* Freely distributable under the terms of the [MIT License](http://www.opensource.org/licenses/MIT)




