namespace TreeView;

[GObject.Subclass<GObject.Object>]
public partial class TreeViewItem
{
    public string Label { get; set; } 
    public Gio.ListStore? Children { get; set; }

    public TreeViewItem(string label, Gio.ListStore? children = null)
        : this()
    {
        Label = label;
        Children = children;
    }
}

// public sealed class TreeItemWidget : Gtk.Box
// {
//     private readonly Gtk.Label label;
//
//     public TreeItemWidget()
//     {
//         Spacing = 6;
//         MarginTop = MarginBottom = MarginStart = MarginEnd = 2;
//         SetOrientation(Gtk.Orientation.Horizontal);
//         
//         label = Gtk.Label.New("");
//         label.Halign = Gtk.Align.Start;
//     }
//
//     public void Update(TreeViewItem item)
//     {
//         if (item.Children is not null)
//         {
//             var expander = Gtk.TreeExpander.New();
//             expander.Child = label;
//             Append(expander);
//         }
//         else
//         {
//             Append(label);
//         }
//         
//         label.SetText(item.Label);
//     }
// }

[GObject.Subclass<Gtk.Window>]
public partial class TreeViewWindow
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
        SetDefaultSize(300, 300);
        
        var model = CreateModel();
        
        var signalFactory = Gtk.SignalListItemFactory.New();
        signalFactory.OnSetup += (factory, args) => {
            var label = Gtk.Label.New(null);
            label.Halign = Gtk.Align.Start;
            label.CanTarget = false;
            
            var expander = Gtk.TreeExpander.New();
            expander.Child = label;
            
            var listItem = args.Object as Gtk.ListItem;
            listItem!.Child = expander;
        };
        signalFactory.OnBind += (factory, args) => {
            var listItem = (Gtk.ListItem) args.Object;
            var treeListRow = (Gtk.TreeListRow) listItem.GetItem()!;
            
            var expander = listItem.Child as Gtk.TreeExpander;
            
            var label = expander!.Child as Gtk.Label;
            var treeViewItem = treeListRow.Item as TreeViewItem;
            label!.SetMarkup(treeViewItem!.Label);
            
            expander.ListRow = treeListRow;
        };

        var selectionModel = Gtk.SingleSelection.New(model);
        selectionModel.SetSelected(0);
        selectionModel.Autoselect = false;
        selectionModel.CanUnselect = true;
        selectionModel.OnSelectionChanged += HandleSelectionChanged;
        
        var listView = Gtk.ListView.New(selectionModel, signalFactory);

        var scrolledWindow = Gtk.ScrolledWindow.New();
        scrolledWindow.Child = listView;
        Child = scrolledWindow;
    }
    
    private Gio.ListModel CreateModel()
    {
        var rootModel = Gio.ListStore.New(TreeViewItem.GetGType());
        foreach (var key in _capitals.Keys)
        {
            var children = Gio.ListStore.New(TreeViewItem.GetGType());
            children.Append(new TreeViewItem(key + "_dummy"));
            
            rootModel.Append(new TreeViewItem(key, children));
        }
        
        var treeModel = Gtk.TreeListModel.New(rootModel, false, false, CreateChildModel);
        return treeModel;
    }

    private Gio.ListModel? CreateChildModel(GObject.Object? item)
    {
        var treeViewItem = item as TreeViewItem;
        if (treeViewItem!.Children is not null)
        {
            var children = Gio.ListStore.New(TreeViewItem.GetGType());
            foreach (var capital in _capitals[treeViewItem!.Label])
            {
                children.Append(new TreeViewItem(capital));
            }
            
            return children;
        }
        
        return null;
    }
    
    private static void HandleSelectionChanged (Gtk.SelectionModel sender, EventArgs e)
    {
        if (sender is Gtk.SingleSelection singleSelection)
        {
            try
            {
                var treeListRow = singleSelection.SelectedItem as Gtk.TreeListRow;
                var treeViewItem = treeListRow!.Item as TreeViewItem;
                if (treeViewItem!.Children is not null)
                {
                    treeListRow.Expanded = !treeListRow.Expanded;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
