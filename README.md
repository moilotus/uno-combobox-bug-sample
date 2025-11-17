# Uno Platform ComboBox Multi-Window Bug Sample

This is a minimal reproduction sample for a bug in Uno Platform 6.4.26 where ComboBox controls become unresponsive after specific multi-window interactions on macOS Skia.

## What This Demonstrates

This sample reproduces a bug where:
1. Opening a secondary window
2. Double-clicking an item in that window (which sends a message to the main window)
3. Returning to the main window

...causes ALL ComboBox controls to stop opening when clicked.

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- macOS (where the bug reproduces)
- Uno Platform workload installed

### Build and Run

```bash
# From this directory
dotnet build -f net9.0-desktop
dotnet run -f net9.0-desktop
```

## Reproduction Steps

1. **Launch the app** - you'll see the main window with instructions
2. **Click the ComboBox** - it should open normally ✅
3. **Click "Open Secondary Window"**
4. **Double-click any item** in the TreeView of the secondary window
5. **Return to main window** - it automatically activates
6. **Click the ComboBox again** - ❌ **BUG: It won't open!**
7. **Alt-Tab away and back** - ComboBox works again ✅

## What to Observe

The sample includes diagnostic counters and a live event log showing:
- **PointerPressed events** - These WILL fire when clicking the broken ComboBox
- **DropDownOpened events** - These will NOT fire (the bug)
- **DropDownClosed events** - For tracking dropdown state

When the bug is active, you'll see PointerPressed events firing but no corresponding DropDownOpened events.

## Project Structure

```
UnoComboBoxBugSample/
├── UnoComboBoxBugSample.csproj  # Project file (Uno.Sdk 6.4.26)
├── Program.cs                    # Entry point
├── App.xaml / App.xaml.cs        # Application entry point
├── MainWindow.xaml               # Main window with ComboBox + diagnostics
├── MainWindow.xaml.cs            # Event handling and logging
├── SecondaryWindow.xaml          # Secondary window with TreeView
├── SecondaryWindow.xaml.cs       # Double-click handler
└── README.md                     # This file
```

## Real-World Context

This bug was discovered in a production mapping application where:
- The main window has a map view with a ComboBox for selecting map types
- A secondary window shows a TreeView of routes/waypoints
- Double-clicking a route sends messages to navigate the map
- After this interaction, the map picker ComboBox stops working

The pattern of "secondary window → user interaction → message to main window → window close" triggers the bug consistently.

## Platform Specificity

This bug appears to be **macOS Skia-specific**. Expected behavior on other platforms:
- ✅ Windows (WinUI3) - ComboBox should work
- ✅ iOS - ComboBox should work
- ✅ Android - ComboBox should work
- ? WebAssembly - Untested

## Technical Details

**Suspected Issue:** Uno Platform's multi-window focus/pointer handling on macOS Skia has a bug where:
- Secondary window activation + message passing + window closure
- Leaves the main window in a corrupted pointer/focus state
- ComboBox controls receive PointerPressed but the dropdown mechanism is blocked
- Alt-tabbing resets the state

**Uno SDK Version:** 6.4.26
**Target Framework:** net9.0-desktop
**.NET Version:** 9.0

## Upstream Issue

- **Status:** To be filed with Uno Platform
- **Repository:** https://github.com/unoplatform/uno
- **Issue Number:** *[To be added after filing]*

---

*This minimal sample was created to help the Uno Platform team identify and fix the underlying multi-window focus bug.*
