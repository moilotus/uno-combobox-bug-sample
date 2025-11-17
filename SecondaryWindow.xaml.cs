using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;

namespace UnoComboBoxBugSample;

public sealed partial class SecondaryWindow : Window
{
    public event EventHandler<string>? ItemDoubleClicked;

    public SecondaryWindow()
    {
        this.InitializeComponent();
        PopulateTreeView();
    }

    private void PopulateTreeView()
    {
        // Create a simple tree structure similar to RTW
        var items = new[]
        {
            "Route: Mountain Peak Trail",
            "Route: Coastal Path Walk",
            "Waypoint: Summit Viewpoint",
            "Waypoint: Parking Area",
            "Track: Morning Run",
            "Folder: Hiking Routes",
            "Route: Forest Loop",
            "Waypoint: Campsite",
            "Track: Bike Ride",
            "Waypoint: Historic Marker"
        };

        foreach (var itemName in items)
        {
            var node = new TreeViewNode
            {
                Content = itemName
            };

            // Create a Grid to handle double-tap events
            var itemGrid = new Grid
            {
                Height = 35,
                Padding = new Thickness(8, 0, 0, 0)
            };

            var textBlock = new TextBlock
            {
                Text = itemName,
                VerticalAlignment = VerticalAlignment.Center
            };

            itemGrid.Children.Add(textBlock);

            // Handle double-tap
            itemGrid.DoubleTapped += ItemGrid_DoubleTapped;
            itemGrid.Tag = itemName; // Store the item name

            // Create a TreeViewItem with the Grid as content
            var treeViewItem = new TreeViewItem
            {
                Content = itemGrid
            };

            ItemsTreeView.RootNodes.Add(new TreeViewNode
            {
                Content = treeViewItem
            });
        }
    }

    private void ItemGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is Grid grid && grid.Tag is string itemName)
        {
            Debug.WriteLine($"[SecondaryWindow] Double-clicked: {itemName}");

            // Fire event to main window (simulates ViewAsync -> MoveMap/GotoPoint)
            ItemDoubleClicked?.Invoke(this, itemName);

            e.Handled = true;
        }
    }
}
