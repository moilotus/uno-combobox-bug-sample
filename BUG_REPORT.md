# Uno Platform ComboBox Bug - Multi-Window Focus Issue on macOS Skia

**Platform:** macOS with Skia Desktop
**Uno Version:** 6.4.26
**Status:** Ready for upstream filing at https://github.com/unoplatform/uno/issues

## Summary

On macOS Skia, ComboBox controls become unresponsive after opening a secondary window, interacting with it, and returning to the main window. The ComboBox receives pointer events but the dropdown won't open. Alt-tabbing away and back temporarily fixes it.

## Reproduction

1. **Run the sample:**
   ```bash
   cd UnoComboBoxBugSample
   dotnet run -f net9.0-desktop
   ```

2. **Trigger the bug:**
   - Click ComboBox → ✅ Works
   - Click "Open Secondary Window"
   - Double-click any TreeView item (secondary window closes)
   - Click ComboBox again → ❌ **Doesn't open**

3. **Temporary fix:**
   - Alt-Tab away and back
   - ComboBox works again ✅

## Technical Details

**Symptoms:**
- PointerPressed events fire normally
- DropDownOpened events never fire
- Setting `IsDropDownOpen = true` programmatically has no effect
- Only affects macOS Skia Desktop (not Windows/iOS/Android)

**Pattern that triggers bug:**
1. Secondary window opens
2. User interaction in secondary window
3. Message/event sent to main window
4. Secondary window closes
5. Focus returns to main → ComboBox broken

**Root cause:** Appears to be corrupted pointer/focus state in Uno's multi-window handling on macOS Skia. Alt-tabbing resets this state.

## Workaround for Production Apps

If you encounter this bug in your app, here's a temporary workaround that detects and recovers from the broken state:

```csharp
// Listen for ComboBox pointer events and force dropdown open if needed
this.AddHandler(PointerPressedEvent, new PointerEventHandler((sender, args) =>
{
    var source = args.OriginalSource?.GetType().Name ?? "null";

    // Detect clicks on ComboBox elements
    if ((source == "ImplicitTextBlock" || source.Contains("ComboBox")) && MyComboBox != null)
    {
        // Re-enable if disabled (sometimes happens in broken state)
        if (!MyComboBox.IsEnabled)
        {
            MyComboBox.IsEnabled = true;
        }

        // If enabled but dropdown not opening, force it open
        if (MyComboBox.IsEnabled && !MyComboBox.IsDropDownOpen)
        {
            args.Handled = true;
            DispatcherQueue?.TryEnqueue(() =>
            {
                MyComboBox.IsDropDownOpen = true;
            });
        }
    }
}), handledEventsToo: true);
```

**Key points:**
- Use `AddHandler` with `handledEventsToo: true` to catch all pointer events
- Check for `ImplicitTextBlock` (the text in the ComboBox) or any ComboBox-related source
- Re-enable the ComboBox if it got disabled
- Force `IsDropDownOpen = true` on the UI thread
- Mark event as handled to prevent further processing

**Important:** This is a workaround only - remove it once Uno Platform fixes the underlying issue.

## Environment

- **macOS:** Sequoia 15.1+ (likely affects earlier versions)
- **Uno SDK:** 6.4.26
- **Target Framework:** net9.0-desktop
- **.NET:** 9.0

## Files in This Sample

- `Program.cs` - Entry point with UnoPlatformHostBuilder
- `MainWindow.xaml.cs` - Diagnostic logging and event tracking
- `SecondaryWindow.xaml.cs` - TreeView with double-click behavior
- `README.md` - Quick start guide

## For Uno Platform Team

**Suggested investigation areas:**
1. Skia macOS pointer capture state after window close
2. Focus management when secondary window closes
3. Event dispatcher state corruption
4. Differences between macOS and Windows/Linux focus handling

**Diagnostic evidence:**
- See event logs in sample app showing PointerPressed without DropDownOpened
- Alt-tabbing (which triggers window deactivate/activate) fixes it
- Only secondary window interaction triggers it (not dialogs or popups)

---

*This sample reproduces a real-world bug affecting production applications on macOS.*
