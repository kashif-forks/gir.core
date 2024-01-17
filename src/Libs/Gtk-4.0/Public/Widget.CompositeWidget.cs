using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GLib;
using GObject;
using BindingFlags = System.Reflection.BindingFlags;
using Type = GObject.Type;
using TypeDictionary = GObject.Internal.TypeDictionary;
using ObjectWrapper = GObject.Internal.ObjectWrapper;
using NonNullableUtf8StringUnownedHandle = GLib.Internal.NonNullableUtf8StringUnownedHandle;
using Gtk.Internal;

namespace Gtk;

public partial class Widget
{
    protected static void SetTemplate(Type gType, Bytes template)
    {
        var widgetClassHandle = new WidgetClassUnownedHandle(TypeHelper.GetClassPointer(gType));
        var bytesHandle = template.Handle;

        Internal.WidgetClass.SetTemplate(widgetClassHandle, bytesHandle);
    }

    protected void ConnectTemplateChildToField<T>(string name, ref T field) where T : GObject.Object
    {
        var systemType = GetType();
        var gtype = TypeDictionary.GetGType(systemType);
        var handle = Internal.Widget.GetTemplateChild(Handle, gtype.Value, NonNullableUtf8StringUnownedHandle.Create(name));
        field = ObjectWrapper.WrapHandle<T>(handle, false);
    }

    protected static void BindTemplateChild(Type gType, string name)
    {
        // var widgetClassHandle = (WidgetClassHandle) TypeHelper.GetClassHandle(gType);
        var widgetClassHandle = new WidgetClassUnownedHandle(TypeHelper.GetClassPointer(gType));
        Internal.WidgetClass.BindTemplateChildFull(widgetClassHandle, NonNullableUtf8StringUnownedHandle.Create(name), false, 0);
    }

    protected static void BindTemplateSignals(Type gType, System.Type t)
    {
        // var widgetClassHandle = (WidgetClassHandle) TypeHelper.GetClassHandle(gType);
        var widgetClassHandle = new WidgetClassUnownedHandle(TypeHelper.GetClassPointer(gType));

        MarshalHelper.ToPtrAndFree(gType, (ptr) =>
        {
            //TODO Verify if OnConnectEvent and DestroyConnectData get garbage collected
            // WidgetClass.Native.set_connect_func(classPtr, OnConnectEvent, ptr, DestroyConnectData);
        });
    }

    private static void OnConnectEvent(IntPtr builder, IntPtr @object, string signal_name, string handler_name,
        IntPtr connect_object, ConnectFlags flags, IntPtr user_data)
    {
        if (!ObjectWrapper.TryWrapHandle<Widget>(@object, false, out var eventSender))
            return;

        if (!TryGetEvent(eventSender.GetType(), signal_name, out EventInfo? @event))
            return;

        if (@event.EventHandlerType is null)
            return;

        if (!ObjectWrapper.TryWrapHandle<Widget>(connect_object, false, out var compositeWidget))
            return;

        if (!TryGetMethod(compositeWidget.GetType(), handler_name, out MethodInfo? compositeWidgetEventHandler))
            return;

        var eventHandlerDelegate = Delegate.CreateDelegate(@event.EventHandlerType, compositeWidget, compositeWidgetEventHandler);

        ConnectEventWithDelegate(@event, eventSender, eventHandlerDelegate);
    }

    private static bool TryGetEvent(System.Type type, string eventName, [NotNullWhen(true)] out EventInfo? eventInfo)
    {
        eventInfo = type.GetEvent(
            name: "On" + eventName,
            bindingAttr: BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public
        );

        return eventInfo is not null;
    }

    private static bool TryGetMethod(System.Type compositeWidgetType, string methodName, [NotNullWhen(true)] out MethodInfo? methodInfo)
    {
        methodInfo = compositeWidgetType.GetMethod(
            name: methodName,
            bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );

        return methodInfo is not null;
    }

    private static void ConnectEventWithDelegate(EventInfo eventInfo, object eventObject, Delegate del)
    {
        eventInfo.AddMethod?.Invoke(eventObject, new object[] { del });
    }

    private static void DestroyConnectData(IntPtr data)
    {
    }
}