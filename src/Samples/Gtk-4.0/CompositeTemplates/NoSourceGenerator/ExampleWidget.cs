using System;
using System.Reflection;
using GObject;
using Type = GObject.Type;
using GLib;

namespace NoSourceGenerator;

public class ExampleWidget : Gtk.Widget
{
    private Gtk.Entry entry;
    private Gtk.Button button;

    private static void ClassInit(Type gClass, System.Type type, IntPtr classData)
    {
        var templateData = Assembly.GetExecutingAssembly().ReadResourceAsByteArray($"{nameof(ExampleWidget)}.ui");
        SetTemplate(
            gType: gClass,
            template: Bytes.New(templateData)
        );
        BindTemplateChild(gClass, nameof(entry));
        BindTemplateChild(gClass, nameof(button));
        BindTemplateSignals(gClass, type);
    }

    protected override void Initialize()
    {
        InitTemplate();
        // ConnectTemplateChildToField(nameof(Button), ref Button);
    }

    // private void OnButtonClicked(Button sender, System.EventArgs args)
    // {
    //     sender.Label = "Clicked!";
    // }
}
