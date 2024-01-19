﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace GObject.Internal;

/// <summary>
/// Thrown when type registration with GType fails
/// </summary>
internal class TypeRegistrationException : Exception
{
    public TypeRegistrationException(string message) : base(message) { }
}

/// <summary>
/// A set of utility functions to register new types with the
/// GType dynamic type system.
/// </summary>
public static class TypeRegistrar
{
    private static TypeQueryData GetTypeMetrics(Type parentType)
    {
        TypeQueryHandle handle = TypeQueryManagedHandle.Create();
        Functions.TypeQuery(parentType.Value, handle);

        return Marshal.PtrToStructure<TypeQueryData>(handle.DangerousGetHandle());
    }

    /// <summary>
    /// Registers with GType a new child class of 'parentType'.
    /// </summary>
    /// <param name="qualifiedName">The name of the class</param>
    /// <param name="parentType">The parent class to derive from</param>
    /// <returns>The newly registered type</returns>
    /// <exception cref="TypeRegistrationException">The type could not be registered</exception>
    internal static Type RegisterGType(string qualifiedName, Type parentType, IReflect type)
    {
        // Get metrics about parent type
        TypeQueryData query = GetTypeMetrics(parentType);

        if (query.Type == 0)
            throw new TypeRegistrationException("Could not query parent type");

        // Create TypeInfo
        //TODO: Callbacks for "ClassInit" and "InstanceInit" are disabled because if multiple instances
        //of the same type are created, the typeInfo object can get garbagec collected in the mean time
        //and with it the instances of "DoClassInit" and "DoInstanceInit". If the callback occurs the
        //runtime can't do the call anymore and crashes with:
        //A callback was made on a garbage collected delegate of type 'GObject-2.0!GObject.Internal.InstanceInitFunc::Invoke'
        //Fix this by caching the garbage collected instances somehow
        var typeInfo = new TypeInfoData()
        {
            ClassSize = (ushort) query.ClassSize,
            InstanceSize = (ushort) query.InstanceSize,
            ClassInit = GetClassInitFunc(type),
            //InstanceInit = DoInstanceInit
        };

        // Perform Registration
        Debug.WriteLine($"Registering new type {qualifiedName} with parent {parentType.ToString()}");

        TypeInfoHandle handle = TypeInfoManagedHandle.Create(typeInfo);
        var typeid = Functions.TypeRegisterStatic(parentType.Value, GLib.Internal.NonNullableUtf8StringOwnedHandle.Create(qualifiedName), handle, 0);

        if (typeid == 0)
            throw new TypeRegistrationException("Type Registration Failed!");

        return new Type(typeid);
    }

    private static ClassInitFunc GetClassInitFunc(IReflect type) => (gClass, classData) =>
    {
        Type gtype = GetGTypeFromTypeClass(gClass);

        InvokeStaticMethod(
            type: type,
            name: "ClassInit",
            parameters: new object[] { gtype, type, classData }
        );
        Console.WriteLine("Subclass type class initialised!");
    };

    private static Type GetGTypeFromTypeClass(IntPtr classDataPtr)
    {
        try
        {
            TypeClassData typeClassData = Marshal.PtrToStructure<TypeClassData>(classDataPtr);
            return new Type(typeClassData.GType);
        }
        catch (Exception ex)
        {
            // TODO: Check if pointer is actually a GObject?
            throw new Exception("Could not resolve type from pointer" + ex.Message);
        }
    }

    private static void InvokeStaticMethod(IReflect type, string name, params object[] parameters)
    {
        MethodInfo? method = type.GetMethod(
            name: name,
            bindingAttr:
            System.Reflection.BindingFlags.Static
            | System.Reflection.BindingFlags.DeclaredOnly
            | System.Reflection.BindingFlags.NonPublic
        );

        method?.Invoke(null, parameters);
    }

    /* TODO: Enable if init functions are supported again
    // Default Handler for instance initialisation.
    private static void DoInstanceInit(IntPtr gClass, IntPtr classData)
    {
        Console.WriteLine("Subclass instance initialised!");
    }
    */
}
