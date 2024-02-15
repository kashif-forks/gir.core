﻿using System.Collections.Generic;

namespace Generator.Renderer.Public;

internal static class ReturnTypeRendererCallback
{
    private static readonly List<ReturnType.ReturnTypeConverter> converters = new()
    {
        new ReturnType.Bitfield(),
        new ReturnType.Class(),
        new ReturnType.Enumeration(),
        new ReturnType.ForeignTypedRecord(),
        new ReturnType.Interface(),
        new ReturnType.OpaqueTypedRecord(),
        new ReturnType.OpaqueUntypedRecord(),
        new ReturnType.Pointer(),
        new ReturnType.PointerAlias(),
        new ReturnType.PrimitiveValueType(),
        new ReturnType.PrimitiveValueTypeAlias(),
        new ReturnType.Record(),
        new ReturnType.RecordArray(),
        new ReturnType.StringArray(),
        new ReturnType.StringInCallback(),
        new ReturnType.TypedRecord(),
        new ReturnType.TypedRecordArray(),
        new ReturnType.Void(),
    };

    public static string Render(GirModel.ReturnType returnType)
    {
        foreach (var converter in converters)
            if (converter.Supports(returnType))
                return converter.Create(returnType).NullableTypeName;

        throw new System.NotImplementedException($"Missing converter for public return type {returnType.AnyType}.");
    }
}
