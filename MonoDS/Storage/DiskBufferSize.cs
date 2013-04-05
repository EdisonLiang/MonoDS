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

namespace MonoDS.Storage
{
	public enum DiskBufferSize
	{
		/// <summary>
		/// The default 8 Bytes.
		/// </summary>
		Default = 8,
		/// <summary>
		/// Small 8 KB (8192 Bytes).
		/// </summary>
		Small = 8192,
		/// <summary>
		/// Medium 16 KB (16384 Bytes).
		/// </summary>
		Medium = 16384,
		/// <summary>
		/// Large 32 KB (32768 Bytes).
		/// </summary>
		Large = 32768,
		/// <summary>
		/// Larger 64 KB (65536 Bytes).
		/// </summary>
		Larger = 65536,
		/// <summary>
		/// Largest 128 KB (131072 Bytes).
		/// </summary>
		Maximum = 131072,
	}
}