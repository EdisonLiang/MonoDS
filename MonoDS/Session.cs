////
//// Copyright (c) 2013 Tony Mackay <toneuk@viewmodel.net>
////
//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:
////
//// The above copyright notice and this permission notice shall be included in
//// all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//// THE SOFTWARE.
////
//using System;
//using System.Collections.Generic;
//using MonoDS.Storage;
//
//namespace MonoDS
//{
//	public class Session : ISession
//	{
//		// the path to where data is stored
//		private string _dataPath;
//
//		private Dictionary<string, StorageProcessor> _storageProcessors;
//
//		/// <summary>
//		/// Initializes a new instance of the <see cref="MonoDS.Session"/> class.
//		/// </summary>
//		/// <param name="dataPath">Specify the full path of where the document data files are to be stored. eg. C:\MonoDS</param>
//		public Session (string dataPath)
//		{
//			// validate the data path specified
//			if (String.IsNullOrEmpty(dataPath))
//				throw new ArgumentNullException("The data path is required.");
//		
//			// instantiate the dicttionary of processors.
//			_storageProcessors = new Dictionary<string, StorageProcessor>();
//		}
//
//		public void SaveChanges ()
//		{
//			throw new NotImplementedException ();
//		}
//
//		public void Dispose ()
//		{
//			throw new NotImplementedException ();
//		}
//
//		public void Store<T> (T document)
//		{
//			throw new NotImplementedException ();
//		}
//		public void Delete<T> (long documentId)
//		{
//			throw new NotImplementedException ();
//		}
//		public T Find<T> (long documentId)
//		{
//			throw new NotImplementedException ();
//		}
//		public IEnumerable<T> Query<T> ()
//		{
//			throw new NotImplementedException ();
//		}
//
//		private void CheckDocumentTypeHasBeenStoredBefore()
//		{
//
//		}
//	}
//}