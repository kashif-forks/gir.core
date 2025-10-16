var application = Gtk.Application.New("org.gir.core", Gio.ApplicationFlags.FlagsNone);
application.OnActivate += (sender, _) =>
{
    var buttonShowTreeViewWithListViewWindow = CreateButton("Show TreeView with ListView");
    buttonShowTreeViewWithListViewWindow.OnClicked += (_, _) => new TreeView.TreeViewWithListViewWindow().Show();
    
    var buttonShowTreeViewWithDropDownWindow = CreateButton("Show TreeView with DropDown");
    buttonShowTreeViewWithDropDownWindow.OnClicked += (_, _) => new TreeView.TreeViewWithDropDownWindow().Show();
    
    var gtkBox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
    gtkBox.Append(buttonShowTreeViewWithListViewWindow);
    gtkBox.Append(buttonShowTreeViewWithDropDownWindow);

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
