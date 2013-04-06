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
using System.Collections.Generic;
using MonoDS.Utilities;
using MonoDS.Exceptions;
using MonoDS.Serialization;

namespace MonoDS.Storage
{
	public class StorageProcessor : IDisposable
	{
		// paths to the data path and indexes.
		private string _dataFilePath;
		private string _dataIndexFilePath;
		private string _deletedDataIndexFilePath;

		// the document store entity type for checking document is valid.
		private string _entityName;

		// the data index processor is used for saving/loading documents in the datastore.
		private DataIndexProcessor _dataIndexProcessor;
		
		// the data index processor is used for saving/loading pointers to deleted documents in the data file.
		private DataIndexProcessor _deletedDataIndexProcessor;

		// the filestream and binary reader for querying documents from the data file.
		private FileStream _fileStreamReader;
		private BinaryReader _binaryReader;

		// the filestream and binary writer for saving documents to the data file.
		private FileStream _fileStreamWriter;
		private BinaryWriter _binaryWriter;

		// the serilizer for converting to and from BSON/Document
		private readonly ISerializer _serializer;

		/// <summary>
		/// Gets the size of the document data file in Bytes.
		/// </summary>
		/// <value>The size of the file in Bytes.</value>
		public Int64 FileSize {get; private set;}

		/// <summary>
		/// Gets or sets the padding factor for each document stored.
		/// This is how much extra space is assigned to each document for allowing the document to expand.
		/// </summary>
		/// <value>The padding factor percentage.</value>
		public int PaddingFactor {get; set;}

		// return the data header from the start of the data file/
		private DataHeader GetDataHeader()
		{
			// if the data file is empty create a new header
			if (this.FileSize == 0){
				var header = new DataHeader();
				UpdateDataHeader(header);
				this.FileSize = 64;
				return header;
			}

			_binaryReader.BaseStream.Position = 0;
			var headerBytes = _binaryReader.ReadBytes(64);
			var dataHeader = DataHeader.Parse(headerBytes);
			return dataHeader;
		}

		/// <summary>
		/// Initializes a new instance of the StorageProcessor class.
		/// Stores document 
		/// </summary>
		/// <param name="dataDirectory">Data directory.</param>
		/// <param name="entityName">Entity name.</param>
		public StorageProcessor (string dataDirectory, string entityName, ISerializer serializer)
		{
			// create directory
			if (!Directory.Exists(dataDirectory)){
				Directory.CreateDirectory(dataDirectory);
			}

			// assign file paths
			_dataFilePath = Path.Combine(dataDirectory, String.Format(@"{0}.mds", entityName));
			_dataIndexFilePath = Path.Combine(dataDirectory, String.Format(@"{0}-DataIndex.idx", entityName));
			_deletedDataIndexFilePath = Path.Combine(dataDirectory, String.Format(@"{0}-DeletedDataIndex.idx", entityName));

			// assign the type of entities that are to be stores in this document store;
			_entityName = entityName;

			// assign serializer
			_serializer = serializer;

			// call function to initialise document store
			Initialise();
		}

		/// <summary>
		/// Initialise this instance. 
		/// </summary>
		public void Initialise()
		{	
			Dispose();

			// load a file stream for reading
			_fileStreamReader = new FileStream(_dataFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Write, 
			                                   (int)DiskBufferSize.Default, FileOptions.SequentialScan);
			
			// load a file stream for writing
			_fileStreamWriter = new FileStream(_dataFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read,
			                                   (int)DiskBufferSize.Default, FileOptions.SequentialScan);
			
			// load the binary reader
			_binaryReader = new BinaryReader(_fileStreamReader);
			
			// load the binary writer
			_binaryWriter = new BinaryWriter(_fileStreamWriter);

			// create the data index processor for storing document key indexes that reference to document records.
			_dataIndexProcessor = new DataIndexProcessor(_dataIndexFilePath, DiskBufferSize.Larger, DiskBufferSize.Default);

			// create the deleted data index processor for storing data indexes to deleted or moved document records.
			_deletedDataIndexProcessor = new DataIndexProcessor(_deletedDataIndexFilePath, DiskBufferSize.Larger, DiskBufferSize.Default);

			// set if the datastore is empty or not.
			if (!File.Exists (_dataFilePath)) {
				DestroyExistingData ();
			}

			// assign file size
			this.FileSize = _binaryReader.BaseStream.Length;
		}

		/// <summary>
		/// Destroys all document data and indexes for the entity belonging to the current instance.
		/// </summary>
		public void DestroyExistingData()
		{
			// remove document store and indexes.
			if (File.Exists (_dataFilePath))
				File.Delete (_dataFilePath);
			
			if (File.Exists (_dataIndexFilePath))
				File.Delete (_dataIndexFilePath);

			if (File.Exists (_deletedDataIndexFilePath))
				File.Delete (_deletedDataIndexFilePath);

			// load the total size of the index file
			this.FileSize = _binaryWriter.BaseStream.Length;

			_dataIndexProcessor.DestroyExistingData();
		}

		/// <summary>
		/// Store the specified entity into the document store.
		/// </summary>
		/// <param name="entity">The new Entity (Document to store).</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <exception cref="ConcurrencyException">When an entity with the same Id is found.</exception>
		public void Store<T>(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException("Entity argument can't be null");

			// make sure the entity name matches the document store type.
			string requiredEntityName = entity.GetType().Name;
			if (_entityName != requiredEntityName)
				throw new ArgumentException("Entity type is not valid for this data store.");

			// make sure entity has key field
			if (!Reflection.PropertyExists (entity, "Id"))
				throw new InvalidEntityException ("Entity must have an Id property and be of type short, integer or long." +
					"This is used as the primary key for the entity being stored.");

			// load the document key from the entity as its needed for adding to the index.
			var documentKey = Reflection.GetPropertyValueInt64(entity, "Id");

			// get the data store header so we can generate keys and store record counts.
			var header = GetDataHeader();

			// boolean so we know to check for duplicate or not on insert.
			// duplicates only need checked when the user has specified the document key.
			bool checkForDuplicate = true;
			if (documentKey == 0)
				checkForDuplicate = false;

			// get the next document key from the data file header record.
			documentKey = header.GenerateNextRecord(documentKey);

			// update the entity value so that the callers entity gets the saved document key.
			Reflection.SetPropertyValue(entity, "Id", documentKey);

			// parse the document into a binary json document for storing in the data file.
			byte[] binaryJson = _serializer.Serialize<T>(entity);

			// create the data index with the data pointer at the end of the document.
			// check to see if there is a deleted slot that can be used to store the data.
			var dataIndex = _deletedDataIndexProcessor.GetDataIndexWithEnoughSpace(binaryJson.Length);
			if (dataIndex != null){
				// assign this document key to the deleted index.
				dataIndex.ChangeDocumentKey(documentKey);
				dataIndex.UpdateRecordLength(binaryJson.Length);
			}
			else{
				// create a new data index.
				dataIndex = new DataIndex(documentKey, this.FileSize, binaryJson.Length, this.PaddingFactor);

				// update the size of the datafile
				this.FileSize = this.FileSize + dataIndex.RecordLength + dataIndex.PaddingLength;
			}

			// create the data index (AddIndex throws ConcurrencyException so no data will save)
			if (checkForDuplicate)
				_dataIndexProcessor.AddIndexCheckForDuplicate (dataIndex);
			else
				_dataIndexProcessor.AddIndex (dataIndex);

			// remove the index from the deleted index file if it exists
			_deletedDataIndexProcessor.RemoveIndex(dataIndex.DocumentKey);

			// add the data record to the data file
			_binaryWriter.BaseStream.Position = dataIndex.Pointer;

			// write the record
			_binaryWriter.Write(binaryJson);

			// write the padding.
			if (dataIndex.PaddingLength > 0){
				_binaryWriter.Write(new Byte[dataIndex.PaddingLength]);
			}

			// save the data
			_binaryWriter.Flush();

			// update the header record
			UpdateDataHeader(header);
		}

		public T Load<T>(long searchKey)
		{
			var dataIndex = _dataIndexProcessor.FindIndex(searchKey);
			if (dataIndex == null)
				return default(T);

			// locate the data in the data store file.
			_binaryReader.BaseStream.Position = dataIndex.Pointer;
			var dataBytes = _binaryReader.ReadBytes(dataIndex.RecordLength);

			// parse bytes into an entity and then return
			var entity = _serializer.Deserialize<T>(dataBytes);
			return entity;
		}


		public IEnumerable<T> Load<T>()
		{
			// create a list to hold the documents.
			var documentList = new List<T>();

			long count = 1;
			var dataIndex = _dataIndexProcessor.GetDataIndex(count);
			while (dataIndex != null){  

				// load the data from the pointer specified in the data index.
				// locate the data in the data store file.
				_binaryReader.BaseStream.Position = dataIndex.Pointer;
				var dataBytes = _binaryReader.ReadBytes(dataIndex.RecordLength);
				
				// parse bytes into an entity and then return
				var entity = _serializer.Deserialize<T>(dataBytes);
				documentList.Add(entity);

				count++;
				dataIndex = _dataIndexProcessor.GetDataIndex(count);
			}
			return documentList;
		}
		
		public void Delete(long searchKey)
		{
			var dataIndex = _dataIndexProcessor.FindIndex(searchKey);
			if (dataIndex == null)
				return;

			// create a new deleted data index in deleted file pointing to deleted data.
			_deletedDataIndexProcessor.AddIndexOverwriteDeleted(dataIndex);

			// no need to delete the data just mark the index entry as deleted
			_dataIndexProcessor.RemoveIndex(searchKey);

			// update the record count.
			var header = GetDataHeader();
			header.RemoveRecord();
			UpdateDataHeader(header);
		}

		public void Update<T>(T entity)
		{
			if (entity == null)
				throw new ArgumentNullException("Entity argument can't be null");
			
			// make sure the entity name matches the document store type.
			string requiredEntityName = entity.GetType().Name;
			if (_entityName != requiredEntityName)
				throw new ArgumentException("Entity type is not valid for this data store.");
			
			// make sure entity has key field
			if (!Reflection.PropertyExists (entity, "Id"))
				throw new InvalidEntityException ("Entity must have an Id property and be of type short, integer or long." +
					"This is used as the primary key for the entity being stored.");
			
			// load the document key from the entity as its needed for adding to the index.
			var documentKey = Reflection.GetPropertyValueInt64(entity, "Id");

			// search the data store for the document.
			var dataIndex = _dataIndexProcessor.FindIndex(documentKey);
			if (dataIndex == null)
				throw new InvalidEntityException("Could not find an existing entity to update.");

			// either overwrite entity in existing data slot or append to the the end of the file.
			// parse the document into a binary json document
			byte[] binaryJson = _serializer.Serialize<T>(entity);
			
			// write the data record to the data file
			// update the record index to the documents data size.
			dataIndex.UpdateRecordLength(binaryJson.Length);

			// check to see if the record needs relocated.
			// if it does then set the position to the end of the file.
			if (dataIndex.RequiresRelocation == true){

				// create a deleted record pointer to the data file.
				_deletedDataIndexProcessor.AddIndexOverwriteDeleted(dataIndex);

				// set position of the document to the end of the file
				_binaryWriter.BaseStream.Position = this.FileSize;

				// update the record pointer in the data index
				dataIndex.UpdateRecordPointer(_binaryWriter.BaseStream.Position, this.PaddingFactor);

				// update the file size.
				this.FileSize = this.FileSize + dataIndex.RecordLength + dataIndex.PaddingLength;
			}
			else{	
				// set the position of where the document is in the datafile.
				_binaryWriter.BaseStream.Position = dataIndex.Pointer;
			}
		
			// make changes to the data
			_binaryWriter.Write(binaryJson);

			// write the padding bytes
			if (dataIndex.PaddingLength > 0){
				_binaryWriter.Write(new Byte[dataIndex.PaddingLength]);
			}

			// update data index pointer to point to new location.
			_dataIndexProcessor.UpdateIndex(dataIndex);
			
			// save the data
			_binaryWriter.Flush();
		}

		/// <summary>
		/// Return the Total Record Count for this Document Collection.
		/// </summary>
		public long Count()
		{
			var header = GetDataHeader();
			return header.RecordCount;
		}

		/// <summary>
		/// Releases all resource used by the StorageProcessor object.
		/// Closed all pointers to data files and emptys read cache.
		/// </summary>
		public void Dispose ()
		{
			// close the index processor
			if (_dataIndexProcessor != null)
				_dataIndexProcessor.Dispose();

			// close the deleted index processor
			if (_deletedDataIndexProcessor != null)
				_deletedDataIndexProcessor.Dispose();
			
			// dispose of the reader and writer
			if (_binaryReader != null)
				_binaryReader.Dispose();
			
			// close writer
			if(_binaryWriter != null)
				_binaryWriter.Dispose();
		}

		// update the data header located at the start of the data file.
		private void UpdateDataHeader(DataHeader dataHeader)
		{
			var headerBytes = dataHeader.GetBytes();
			_binaryWriter.BaseStream.Position = 0;
			_binaryWriter.Write(headerBytes);
			_binaryWriter.Flush();
		}
	}
}