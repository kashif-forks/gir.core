var application = Gtk.Application.New("org.gir.core", Gio.ApplicationFlags.FlagsNone);
application.OnActivate += (sender, args) =>
{
    var buttonShowTreeViewWindow = CreateButton("Show TreeView with ListView");
    buttonShowTreeViewWindow.OnClicked += (_, _) => new TreeView.TreeViewWindow().Show();
    
    var gtkBox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
    gtkBox.Append(buttonShowTreeViewWindow);

    var window = Gtk.ApplicationWindow.New((Gtk.Application) sender);
    window.Title = "TreeView Sample";
    window.SetDefaultSize(300, 300);
    window.Child = gtkBox;
    window.Show();
};
return application.RunWithSynchronizationContext(null);

static Gtk.Button CreateButton(string label)
{
    var button = Gtk.Button.New();
    button.Label = label;
    button.SetMarginTop(12);
    button.SetMarginBottom(12);
    button.SetMarginStart(12);
    button.SetMarginEnd(12);
    return button;
}


// public partial class TreeDataItem : GObject.Object, GObject.GTypeProvider, GObject.InstanceFactory
// {
//     private static readonly GObject.Type GType = GObject.Internal.SubclassRegistrar.Register<TreeDataItem, GObject.Object>();
//     public new static GObject.Type GetGType() => GType;
//     
//     static object GObject.InstanceFactory.Create(IntPtr handle, bool ownsHandle)
//     {
//         return new TreeDataItem(new GObject.Internal.ObjectHandle(handle, ownsHandle));
//     }
//      
//     public TreeDataItem(GObject.Internal.ObjectHandle handle) : base(handle) 
//     {
//         Initialize();
//     }
//      
//     public TreeDataItem(params GObject.ConstructArgument[] constructArguments) : this(GObject.Internal.ObjectHandle.For<TreeDataItem>(constructArguments)) 
//     {
//         Initialize();
//     }
//      
//     /// <summary>
//     /// This method is called by all generated constructors.
//     /// Implement this partial method to initialize all members.
//     /// Decorating this method with "MemberNotNullAttribute" for
//     /// the appropriate members can remove nullable warnings.
//     /// </summary>
//     partial void Initialize();
// }
