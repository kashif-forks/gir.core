namespace TreeView;

[GObject.Subclass<Gtk.Window>]
public partial class TreeViewWithListViewWindow
{
    private readonly Dictionary<string, List<string>> _capitals = new()
    {
        ["A - B"] = ["Albany", "Annapolis", "Atlanta", "Augusta", "Austin", "Baton Rouge", "Bismarck", "Boise", "Boston"],
        ["C - D"] = ["Carson City", "Charleston", "Cheyenne", "Columbia", "Columbus", "Concord", "Denver", "Des Moines", "Dover"],
        ["E - J"] = ["Frankfort", "Harrisburg", "Hartford", "Helena", "Honolulu", "Indianapolis", "Jackson", "Jefferson City", "Juneau"],
        ["K - O"] = ["Lansing", "Lincoln", "Little Rock", "Madison", "Montgomery", "Montpelier", "Nashville", "Oklahoma City", "Olympia"],
        ["P - S"] = ["Phoenix", "Pierre", "Providence", "Raleigh", "Richmond", "Sacramento", "Salem", "Salt Lake City", "Santa Fe", "Springfield", "St. Paul"],
        ["T - Z"] = ["Tallahassee", "Topeka", "Trenton"],
    };
    
    partial void Initialize()
    {
        Title = "TreeView with ListView";
        SetDefaultSize(400, 300);
        
        var model = CreateModel();
        
        var signalFactory = Gtk.SignalListItemFactory.New();
        signalFactory.OnSetup += (_, args) =>
        {
            if (args.Object is not Gtk.ListItem listItem)
            {
                return;
            }

            listItem.SetChild(new ListViewTreeItemWidget());
        };
        signalFactory.OnBind += (_, args) => {
            if (args.Object is not Gtk.ListItem listItem)
            {
                return;
            }
            
            var treeItemWidget = (ListViewTreeItemWidget) listItem.GetChild()!;
            var treeListRow = (Gtk.TreeListRow) listItem.GetItem()!;
            var treeViewItem = (ListViewTreeItem) treeListRow.GetItem()!;
            
            treeItemWidget.Update(treeViewItem, treeListRow);
        };

        var selectionModel = Gtk.SingleSelection.New(model);
        
        var listView = Gtk.ListView.New(selectionModel, signalFactory);

        var scrolledWindow = Gtk.ScrolledWindow.New();
        scrolledWindow.Child = listView;
        Child = scrolledWindow;
    }
    
    private Gio.ListModel CreateModel()
    {
        var rootModel = Gio.ListStore.New(ListViewTreeItem.GetGType());
        foreach (var key in _capitals.Keys)
        {
            var children = Gio.ListStore.New(ListViewTreeItem.GetGType());
            children.Append(new ListViewTreeItem(key + "_dummy"));
            
            rootModel.Append(new ListViewTreeItem(key, children));
        }
        
        var treeModel = Gtk.TreeListModel.New(rootModel, false, false, CreateChildModel);
        return treeModel;
    }

    private Gio.ListModel? CreateChildModel(GObject.Object? item)
    {
        var treeViewItem = item as ListViewTreeItem;
        if (treeViewItem!.Children is null)
        {
            return null;
        }

        var children = Gio.ListStore.New(ListViewTreeItem.GetGType());
        foreach (var capital in _capitals[treeViewItem.Label])
        {
            children.Append(new ListViewTreeItem(capital));
        }
            
        return children;
    }
}

[GObject.Subclass<GObject.Object>]
public partial class ListViewTreeItem
{
    public string Label { get; set; } 
    public Gio.ListStore? Children { get; set; }

    public ListViewTreeItem(string label, Gio.ListStore? children = null)
        : this()
    {
        Label = label;
        Children = children;
    }
}

public sealed class ListViewTreeItemWidget : Gtk.Box
{
    private readonly Gtk.TreeExpander expander;
    private readonly Gtk.Label label;

    public ListViewTreeItemWidget()
    {
        Spacing = 6;
        MarginTop = MarginBottom = MarginStart = MarginEnd = 2;
        SetOrientation(Gtk.Orientation.Horizontal);
        
        expander = new Gtk.TreeExpander();
        
        label = Gtk.Label.New("");
        label.Halign = Gtk.Align.Start;
        
        expander.SetChild(label);
        
        Append(expander);
    }

    public void Update(ListViewTreeItem item, Gtk.TreeListRow? treeListRow)
    {
        label.SetText(item.Label);
        expander.SetListRow(treeListRow);
    }
}
