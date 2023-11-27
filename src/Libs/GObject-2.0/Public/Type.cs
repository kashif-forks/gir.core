﻿namespace GObject;

public partial struct Type
{
    public nuint Value => _value;

    #region Statics

    public static readonly Type Invalid = new Type((nuint) Internal.BasicType.Invalid);
    public static readonly Type None = new Type((nuint) Internal.BasicType.None);
    public static readonly Type Interface = new Type((nuint) Internal.BasicType.Interface);
    public static readonly Type Char = new Type((nuint) Internal.BasicType.Char);
    public static readonly Type UChar = new Type((nuint) Internal.BasicType.UChar);
    public static readonly Type Boolean = new Type((nuint) Internal.BasicType.Boolean);
    public static readonly Type Int = new Type((nuint) Internal.BasicType.Int);
    public static readonly Type UInt = new Type((nuint) Internal.BasicType.UInt);
    public static readonly Type Long = new Type((nuint) Internal.BasicType.Long);
    public static readonly Type ULong = new Type((nuint) Internal.BasicType.ULong);
    public static readonly Type Int64 = new Type((nuint) Internal.BasicType.Int64);
    public static readonly Type UInt64 = new Type((nuint) Internal.BasicType.UInt64);
    public static readonly Type Enum = new Type((nuint) Internal.BasicType.Enum);
    public static readonly Type Flags = new Type((nuint) Internal.BasicType.Flags);
    public static readonly Type Float = new Type((nuint) Internal.BasicType.Float);
    public static readonly Type Double = new Type((nuint) Internal.BasicType.Double);
    public static readonly Type String = new Type((nuint) Internal.BasicType.String);
    public static readonly Type Pointer = new Type((nuint) Internal.BasicType.Pointer);
    public static readonly Type Boxed = new Type((nuint) Internal.BasicType.Boxed);
    public static readonly Type Param = new Type((nuint) Internal.BasicType.Param);
    public static readonly Type Object = new Type((nuint) Internal.BasicType.Object);
    public static readonly Type Variant = new Type((nuint) Internal.BasicType.Variant);
    public static readonly Type StringArray = new Type((nuint) Internal.Functions.StrvGetType());

    #endregion Statics

    public override string ToString()
    {
        return Internal.Functions.TypeName(_value).ConvertToString();
    }

    //Offsets see: https://gitlab.gnome.org/GNOME/glib/blob/master/gobject/gtype.h
}
