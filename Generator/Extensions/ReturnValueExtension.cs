﻿using Repository.Model;
using Type = Repository.Model.Type;

namespace Generator
{
    internal static class ReturnValueExtension
    {
        public static bool IsVoid(this ReturnValue returnValue)
            => returnValue.SymbolReference.GetSymbol().NativeName == "void";

        public static string WriteNative(this ReturnValue returnValue, Namespace currentNamespace)
        {
            return returnValue.WriteNativeType(currentNamespace);
        }

        public static string WriteManaged(this ReturnValue returnValue, Namespace currentNamespace)
        {
            return returnValue.WriteManagedType(currentNamespace);
        }
    }
}
