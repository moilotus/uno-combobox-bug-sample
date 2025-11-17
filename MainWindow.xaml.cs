using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;

namespace UnoComboBoxBugSample;

public sealed partial class MainWindow : Window
{
    private int _pointerPressedCount = 0;
    private int _dropDownOpenedCount = 0;
    private int _dropDownClosedCount = 0;
    private SecondaryWindow? _secondaryWindow;

    public MainWindow()
    {
        this.InitializeComponent();
        LogMessage("MainWindow initialized");

        // Subscribe to window activation events
        this.Activated += MainWindow_Activated;
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        LogMessage($"MainWindow activated - State: {args.WindowActivationState}");
    }

    private void TestComboBox_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _pointerPressedCount++;
        PointerPressedCountText.Text = $"PointerPressed events: {_pointerPressedCount}";

        var source = e.OriginalSource?.GetType().Name ?? "unknown";
        LogMessage($"[PointerPressed #{_pointerPressedCount}] Source: {source}");

        // Check if dropdown is currently open
        if (TestComboBox.IsDropDownOpen)
        {
            LogMessage("  → ComboBox dropdown is OPEN");
        }
        else
        {
            LogMessage("  → ComboBox dropdown is CLOSED (expecting it to open...)");
        }
    }

    private void TestComboBox_DropDownOpened(object? sender, object e)
    {
        _dropDownOpenedCount++;
        DropDownOpenedCountText.Text = $"DropDownOpened events: {_dropDownOpenedCount}";
        LastEventText.Text = $"Last event: DropDownOpened at {DateTime.Now:HH:mm:ss.fff}";

        LogMessage($"[DropDownOpened #{_dropDownOpenedCount}] ✅ Dropdown successfully opened");
    }

    private void TestComboBox_DropDownClosed(object? sender, object e)
    {
        _dropDownClosedCount++;
        DropDownClosedCountText.Text = $"DropDownClosed events: {_dropDownClosedCount}";
        LastEventText.Text = $"Last event: DropDownClosed at {DateTime.Now:HH:mm:ss.fff}";

        LogMessage($"[DropDownClosed #{_dropDownClosedCount}] Dropdown closed");
    }

    private void OpenSecondaryButton_Click(object sender, RoutedEventArgs e)
    {
        LogMessage("Opening secondary window...");

        _secondaryWindow = new SecondaryWindow();
        _secondaryWindow.ItemDoubleClicked += SecondaryWindow_ItemDoubleClicked;
        _secondaryWindow.Closed += SecondaryWindow_Closed;
        _secondaryWindow.Activate();

        LogMessage("Secondary window opened");
    }

    private void SecondaryWindow_ItemDoubleClicked(object? sender, string itemName)
    {
        LogMessage($"[MESSAGE FROM SECONDARY] Item double-clicked: '{itemName}'");
        LogMessage("  → Simulating app-specific message handling");

        // This simulates what happens in real-world apps:
        // A view model method sends messages to navigate/update the main window
        LogMessage("  → Activating main window...");
        this.Activate();

        LogMessage("  → Closing secondary window...");
        _secondaryWindow?.Close();
    }

    private void SecondaryWindow_Closed(object sender, WindowEventArgs args)
    {
        LogMessage("Secondary window closed");
        LogMessage("==================================================");
        LogMessage("NOW TRY CLICKING THE COMBOBOX - IT SHOULD FAIL ❌");
        LogMessage("==================================================");

        if (_secondaryWindow != null)
        {
            _secondaryWindow.ItemDoubleClicked -= SecondaryWindow_ItemDoubleClicked;
            _secondaryWindow.Closed -= SecondaryWindow_Closed;
            _secondaryWindow = null;
        }
    }

    private void LogMessage(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] {message}\n";

        LogOutput.Text += logEntry;
        Debug.WriteLine(logEntry.TrimEnd());

        // Auto-scroll to bottom would require more complex logic with ScrollViewer
    }
}
