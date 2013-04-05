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
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace MonoDS.Serialization
{
	public class Serializer : ISerializer
	{
		/// <summary>
		/// Convert the entity (document) to byte array.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public byte[] Serialize<T> (T entity)
		{
			var serializer = new JsonSerializer();
			using (var memoryStream = new MemoryStream())
				using (var writer = new BsonWriter(memoryStream))
			{
				serializer.Serialize(writer, entity);
				return memoryStream.ToArray();
			}
		}
		/// <summary>
		/// Converts the byte array into an entity (document).
		/// </summary>
		/// <param name="bytes">BSON Bytes.</param>
		public T Deserialize<T> (byte[] bytes)
		{
			var serializer = new JsonSerializer();
			using (var memoryStream = new MemoryStream(bytes))
			{
				var reader = new BsonReader(memoryStream);
				var entity = serializer.Deserialize<T>(reader);
				return entity;
			}
		}
	}
}