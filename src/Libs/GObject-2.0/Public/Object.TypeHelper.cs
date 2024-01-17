using System;
using System.Runtime.InteropServices;

namespace GObject
{
    public partial class Object
    {
        protected static class TypeHelper
        {
            public static IntPtr GetClassPointer(Type type)
            {
                var ptr = Internal.TypeClass.Peek(type);
                if (ptr == IntPtr.Zero)
                    ptr = Internal.TypeClass.Ref(type);

                return ptr;
            }
        }
    }
}