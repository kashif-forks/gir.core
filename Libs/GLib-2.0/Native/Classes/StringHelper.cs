﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GLib.Native
{
    public static class StringHelper
    {
        /// <summary>
        /// Interprets the given ptr as a non nullable string.
        /// </summary>
        /// <returns>a managed version of the string.</returns>
        /// <remarks>Use this method if the ptr should not be freed by the marshaller.</remarks>
        public static string ToStringUTF8(IntPtr ptr)
            => Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
        
        /// <summary>
        /// Interprets the given ptr as a nullable string.
        /// </summary>
        /// <returns>a managed version of the string.</returns>
        /// <remarks>Use this method if the ptr should not be freed by the marshaller.</remarks>
        public static string? ToNullableStringUTF8(IntPtr ptr)
            => (ptr != IntPtr.Zero) ? Marshal.PtrToStringUTF8(ptr) : null;

        /// <summary>
        /// Interprets the given ptr as a null terminated string array.
        /// </summary>
        /// <returns>A managed version of the string array.</returns>
        /// <remarks>Use this method if the ptr should not be freed by the marshaller.</remarks>
        public static string[] ToStringUTF8Array(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return System.Array.Empty<string>();

            var data = new List<string>();
            var offset = 0;
            while (true)
            {
                var currentPointer = Marshal.ReadIntPtr(ptr, offset * IntPtr.Size);

                if (currentPointer == IntPtr.Zero)
                    break;
                
                var str = ToStringUTF8(currentPointer);
                data.Add(str);
                
                offset++;
            }
            
            return data.ToArray();
        }
    }
    
    public class StringArrayNullTerminatedSafeHandle : SafeHandle
    {
        private GCHandle _gcHandle;
        private readonly IntPtr[] _data;
        public StringArrayNullTerminatedSafeHandle(string[] array) : base(IntPtr.Zero, true)
        {
            var numStrings = array.Length;
            _data = new IntPtr[numStrings + 1];
            
            // UTF-8 Encoding Information
            Encoding encoding = System.Text.Encoding.UTF8;

            // Populate with UTF-8 encoded bytes
            for (var i = 0; i < numStrings; i++)
            {
                // Get UTF-8 byte array
                var bytes = encoding.GetBytes(array[i]);
                
                // Marshal as pointer
                IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);

                // Save in data array
                _data[i] = ptr;
            }

            // Null terminate the array
            _data[numStrings] = IntPtr.Zero;

            // Keep in memory until done
            _gcHandle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            SetHandle(_gcHandle.AddrOfPinnedObject());
        }

        protected override bool ReleaseHandle()
        {
            // Free string memory
            foreach (IntPtr ptr in _data)
                Marshal.FreeHGlobal(ptr);
            
            // Allow GC to free memory
            _gcHandle.Free();

            return true;
        }

        public override bool IsInvalid => !_gcHandle.IsAllocated;
    }
}
