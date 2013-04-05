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
using System.Collections.Generic;

using MonoDS.Storage;
using MonoDS.Serialization;
using MonoDS.Utilities;
using System.IO;

namespace MonoDS
{
	public class DocumentStore : IDisposable
	{
		private readonly string _dataDirectory;
		private readonly ISerializer _serializer;
		private readonly Dictionary<string, StorageProcessor> _storageProcessors;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoDS.DocumentStore"/> class.
		/// </summary>
		/// <param name="dataDirectory">The full path to the Data directory.</param>
		public DocumentStore (string dataDirectory)
		{
			if (String.IsNullOrEmpty(dataDirectory))
				throw new ArgumentNullException("The dataDirectory argument is required.");

			if (!Directory.Exists(dataDirectory))
				throw new ArgumentException("The specified directory does not exist.");

			 _dataDirectory = Path.Combine (dataDirectory, "MonoDS");
			_storageProcessors = new Dictionary<string, StorageProcessor>();
			_serializer = new Serializer();
		}

		/// <summary>
		/// Store the specified document.
		/// </summary>
		/// <param name="document">Document.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Store<T> (T document)
		{
			var sp = GetStorageProcessor<T>(document);
			sp.Store<T>(document);
		}

		/// <summary>
		/// Delete the document with the specified document id.
		/// </summary>
		/// <param name="documentId">Document identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Delete<T> (long documentId)
		{
			var entityName = typeof(T).Name;
			var sp = GetStorageProcessor(entityName);
			sp.Delete(documentId);
		}

		/// <summary>
		/// Find the document with the specified document id.
		/// </summary>
		/// <param name="documentId">Document identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T Find<T> (long documentId)
		{
			var entityName = typeof(T).Name;
			var sp = GetStorageProcessor(entityName);
			return sp.Load<T>(documentId);
		}

		/// <summary>
		/// Load all documents of the specified type.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public IEnumerable<T> All<T> ()
		{
			var entityName = typeof(T).Name;
			var sp = GetStorageProcessor(entityName);
			return sp.Load<T>();
		}

		/// <summary>
		/// Total document count for the specified type.
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public long Count<T> ()
		{
			var entityName = typeof(T).Name;
			var sp = GetStorageProcessor(entityName);
			return sp.Count();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="MonoDS.DocumentStore"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="MonoDS.DocumentStore"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MonoDS.DocumentStore"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="MonoDS.DocumentStore"/> so the garbage
		/// collector can reclaim the memory that the <see cref="MonoDS.DocumentStore"/> was occupying.</remarks>
		public void Dispose ()
		{
			foreach(var sp in _storageProcessors)
				sp.Value.Dispose();

			_storageProcessors.Clear();
		}

		public void DestroyAllData()
		{
			EmptyDirectory(_dataDirectory);
		}

		public void EmptyDirectory(string directoryName)
		{
			if (!Directory.Exists(directoryName))
				return;

			var dir = new DirectoryInfo(_dataDirectory);
			foreach(FileInfo fi in dir.GetFiles()){
				fi.Delete();
			}
			foreach (var di in dir.GetDirectories()){
				EmptyDirectory(di.FullName);
				di.Delete();
			}
			foreach(var file in Directory.EnumerateFiles(_dataDirectory, "*.*")){
				File.Delete(file);
			}
		}

		private StorageProcessor GetStorageProcessor(string entityName)
		{
			// search collection for an instantiated storage processor.
			// if its not in the list instantiate it, add then return.
			if (!_storageProcessors.ContainsKey(entityName)){
				var sp = new StorageProcessor(_dataDirectory, entityName, _serializer);
				_storageProcessors.Add(entityName, sp);
				return sp;
			}
			return _storageProcessors[entityName];
		}

		private StorageProcessor GetStorageProcessor<T>(T entity)
		{
			string entityName = entity.GetType().Name;
			return GetStorageProcessor(entityName);
		}

	}
}