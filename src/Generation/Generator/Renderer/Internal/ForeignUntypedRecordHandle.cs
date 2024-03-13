﻿using Generator.Model;

namespace Generator.Renderer.Internal;

internal static class ForeignUntypedRecordHandle
{
    public static string Render(GirModel.Record record)
    {
        var typeName = Model.ForeignUntypedRecord.GetInternalHandle(record);
        var unownedHandleTypeName = Model.ForeignUntypedRecord.GetInternalUnownedHandle(record);
        var ownedHandleTypeName = Model.ForeignUntypedRecord.GetInternalOwnedHandle(record);

        return $@"using System;
using GObject;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#nullable enable

namespace {Namespace.GetInternalName(record.Namespace)};

// AUTOGENERATED FILE - DO NOT MODIFY

{PlatformSupportAttribute.Render(record as GirModel.PlatformDependent)}
public abstract class {typeName} : SafeHandle
{{    
    public sealed override bool IsInvalid => handle == IntPtr.Zero;

    protected {typeName}(bool ownsHandle) : base(IntPtr.Zero, ownsHandle) {{ }}
}}

public class {unownedHandleTypeName} : {typeName}
{{
    private static {unownedHandleTypeName}? nullHandle;
    public static {unownedHandleTypeName} NullHandle => nullHandle ??= new {unownedHandleTypeName}();

    /// <summary>
    /// Creates a new instance of {unownedHandleTypeName}. Used automatically by PInvoke.
    /// </summary>
    internal {unownedHandleTypeName}() : base(false) {{ }}

    /// <summary>
    /// Creates a new instance of {unownedHandleTypeName}. Assumes that the given pointer is unowned by the runtime.
    /// </summary>
    public {unownedHandleTypeName}(IntPtr ptr) : base(false)
    {{
        SetHandle(ptr);
    }}

    protected override bool ReleaseHandle()
    {{
        throw new System.Exception(""UnownedHandle must not be freed"");
    }}
}}

public partial class {ownedHandleTypeName} : {typeName}
{{
    /// <summary>
    /// Creates a new instance of {ownedHandleTypeName}. Used automatically by PInvoke.
    /// </summary>
    internal {ownedHandleTypeName}() : base(true) {{ }}

    /// <summary>
    /// Creates a new instance of {ownedHandleTypeName}. Assumes that the given pointer is owned by the runtime.
    /// </summary>
    public {ownedHandleTypeName}(IntPtr ptr) : base(true)
    {{
        SetHandle(ptr);
    }}

    protected override partial bool ReleaseHandle();
}}";
    }
}
