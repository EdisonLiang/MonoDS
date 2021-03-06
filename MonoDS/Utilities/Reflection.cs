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

namespace MonoDS.Utilities
{
	public class Reflection
	{
		public static bool PropertyExists<T>(T entity, string propertyName)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
					return true;
			}
			return false;
		}

		public static long GetPropertyValueInt16<T>(T entity, string propertyName)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
				{
					return Convert.ToInt16(p.GetValue(entity, null));
				}
			}
			return 0;
		}

		public static long GetPropertyValueInt64<T>(T entity, string propertyName)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
				{
					return Convert.ToInt64(p.GetValue(entity, null));
				}
			}
			return 0;
		}

		public static int GetPropertyValueInt32<T>(T entity, string propertyName)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase))
				{
					return Convert.ToInt32(p.GetValue(entity, null));
				}
			}
			return 0;
		}

		public static void SetPropertyValue<T>(T entity, string propertyName, long propertyValue)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase)){
					string propertyTypeName = p.PropertyType.Name;
					if (propertyTypeName == "Int64"){
						p.SetValue(entity, propertyValue, null);
					} else if(propertyTypeName == "Int32"){
						p.SetValue (entity, Convert.ToInt32(propertyValue), null);
					} else if(propertyTypeName == "Int16"){
						p.SetValue (entity, Convert.ToInt16(propertyValue), null);
					}
					return;
				}
			}
		}

		public static void SetPropertyValue<T>(T entity, string propertyName, int propertyValue)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase)){
					string propertyTypeName = p.PropertyType.Name;
					if (propertyTypeName == "Int64"){
						p.SetValue(entity, Convert.ToInt64(propertyValue), null);
					} else if(propertyTypeName == "Int32"){
						p.SetValue (entity, propertyValue, null);
					} else if(propertyTypeName == "Int16"){
						p.SetValue (entity, Convert.ToInt16(propertyValue), null);
					}
					return;
				}
			}
		}

		public static void SetPropertyValue<T>(T entity, string propertyName, short propertyValue)
		{
			var propertyInfo = entity.GetType().GetProperties();
			foreach(var p in propertyInfo)
			{
				if (p.Name.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase)){
					string propertyTypeName = p.PropertyType.Name;
					if (propertyTypeName == "Int64"){
						p.SetValue(entity, Convert.ToInt64(propertyValue), null);
					} else if(propertyTypeName == "Int32"){
						p.SetValue (entity, Convert.ToInt32(propertyValue), null);
					} else if(propertyTypeName == "Int16"){
						p.SetValue (entity, propertyValue, null);
					}
					return;
				}
			}
		}

	}
}