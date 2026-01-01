using System.Runtime.InteropServices;
using System.Text;
using BlazeEngine;

namespace BlazeEnvironment;

public unsafe class ImGui
{
    [DllImport("ImGui.dll", EntryPoint = "ImGui_Init")]
    public static extern void Init();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_PreDraw")]
    public static extern void PreDraw();
    [DllImport("ImGui.dll", EntryPoint = "ImGui_PostDraw")]
    public static extern void PostDraw();

    [DllImport("ImGui.dll", EntryPoint = "ImGui_IsRunning")]
    public static extern bool IsRunning();

    [DllImport("ImGui.dll", EntryPoint = "ImGui_BeginWithCloseButton")]
    public static extern bool Begin(string name, ref bool p_open, int flags = 0);
    [DllImport("ImGui.dll", EntryPoint = "ImGui_Begin")]
    public static extern bool Begin(string name, int flags = 0);

    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_End")]
    public static extern void End();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_Text")]
    public static extern void Text(string text);

    [DllImport("ImGui.dll", EntryPoint = "ImGui_Button")]
    public static extern bool Button(string text);
    [DllImport("ImGui.dll", EntryPoint = "ImGui_ButtonWithSize")]
    public static extern bool Button(string text, float width, float height);
    public static bool Button(in string text, in Vec2 size) => Button(text,  size.X, size.Y);

    [DllImport("ImGui.dll", CharSet = CharSet.Ansi, EntryPoint = "ImGui_InputText")]
    public static extern bool InputText(string label, StringBuilder buffer, int bufferSize);
    public static bool InputText(string label, StringBuilder buffer)
    {
        return InputText(label, buffer, buffer.Capacity);
    }
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_PushStringId")]
    public static extern bool PushId(string id);
    [DllImport("ImGui.dll", EntryPoint = "ImGui_PushIntId")]
    public static extern int PushId(int id);
    [DllImport("ImGui.dll", EntryPoint = "ImGui_PopId")]
    public static extern int PopId();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_BeginTable")]
    public static extern bool BeginTable(string name, int columns, ImGuiTableFlags flags = 0);
    [DllImport("ImGui.dll", EntryPoint = "ImGui_EndTable")]
    public static extern bool EndTable();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_TableNextRow")]
    public static extern bool TableNextRow();
    [DllImport("ImGui.dll", EntryPoint = "ImGui_TableNextColumn")]
    public static extern bool TableNextColumn();
    [DllImport("ImGui.dll", EntryPoint = "ImGui_TableSetColumnIndex")]
    public static extern bool TableSetColumnIndex(int index);
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_ShowDemoWindow")]
    public static extern void ShowDemoWindow();

    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_GetDroppedFile")]
    public static extern int GetDroppedFile(StringBuilder buffer, int bufferSize);
    public static bool GetDroppedFile(out string output)
    {
        StringBuilder buffer = new(256, 256);
        int result = GetDroppedFile(buffer, buffer.Capacity);
        output = string.Empty;
        if (result != 0)
        {
            output = buffer.ToString();
        }
        return result != 0;
    }
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_Separator")]
    public static extern void Separator();
    [DllImport("ImGui.dll", EntryPoint = "ImGui_SeparatorText")]
    public static extern void SeparatorText(string text);

    [DllImport("ImGui.dll", EntryPoint = "ImGui_SameLine")]
    public static extern void SameLine();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_BeginMainMenuBar")]
    public static extern bool BeginMainMenuBar();
    [DllImport("ImGui.dll", EntryPoint = "ImGui_EndMainMenuBar")]
    public static extern void EndMainMenuBar();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_BeginMenu")]
    public static extern bool BeginMenu(string label, bool enabled = true);
    [DllImport("ImGui.dll", EntryPoint = "ImGui_EndMenu")]
    public static extern void EndMenu();
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_MenuItem")]
    public static extern bool MenuItem(string label, string shortcut = "", bool p_selected = false, bool enabled = true);
    
    [DllImport("ImGui.dll", EntryPoint = "ImGui_BeginCustomMainDockspace")]
    public static extern bool BeginCustomMainDockspace();

}

public enum ImGuiTableFlags : int
{
    // Features
    ImGuiTableFlags_None                       = 0,
    ImGuiTableFlags_Resizable                  = 1 << 0,   // Enable resizing columns.
    ImGuiTableFlags_Reorderable                = 1 << 1,   // Enable reordering columns in header row (need calling TableSetupColumn() + TableHeadersRow() to display headers)
    ImGuiTableFlags_Hideable                   = 1 << 2,   // Enable hiding/disabling columns in context menu.
    ImGuiTableFlags_Sortable                   = 1 << 3,   // Enable sorting. Call TableGetSortSpecs() to obtain sort specs. Also see ImGuiTableFlags_SortMulti and ImGuiTableFlags_SortTristate.
    ImGuiTableFlags_NoSavedSettings            = 1 << 4,   // Disable persisting columns order, width, visibility and sort settings in the .ini file.
    ImGuiTableFlags_ContextMenuInBody          = 1 << 5,   // Right-click on columns body/contents will display table context menu. By default it is available in TableHeadersRow().
    // Decorations
    ImGuiTableFlags_RowBg                      = 1 << 6,   // Set each RowBg color with ImGuiCol_TableRowBg or ImGuiCol_TableRowBgAlt (equivalent of calling TableSetBgColor with ImGuiTableBgFlags_RowBg0 on each row manually)
    ImGuiTableFlags_BordersInnerH              = 1 << 7,   // Draw horizontal borders between rows.
    ImGuiTableFlags_BordersOuterH              = 1 << 8,   // Draw horizontal borders at the top and bottom.
    ImGuiTableFlags_BordersInnerV              = 1 << 9,   // Draw vertical borders between columns.
    ImGuiTableFlags_BordersOuterV              = 1 << 10,  // Draw vertical borders on the left and right sides.
    ImGuiTableFlags_BordersH                   = ImGuiTableFlags_BordersInnerH | ImGuiTableFlags_BordersOuterH, // Draw horizontal borders.
    ImGuiTableFlags_BordersV                   = ImGuiTableFlags_BordersInnerV | ImGuiTableFlags_BordersOuterV, // Draw vertical borders.
    ImGuiTableFlags_BordersInner               = ImGuiTableFlags_BordersInnerV | ImGuiTableFlags_BordersInnerH, // Draw inner borders.
    ImGuiTableFlags_BordersOuter               = ImGuiTableFlags_BordersOuterV | ImGuiTableFlags_BordersOuterH, // Draw outer borders.
    ImGuiTableFlags_Borders                    = ImGuiTableFlags_BordersInner | ImGuiTableFlags_BordersOuter,   // Draw all borders.
    ImGuiTableFlags_NoBordersInBody            = 1 << 11,  // [ALPHA] Disable vertical borders in columns Body (borders will always appear in Headers). -> May move to style
    ImGuiTableFlags_NoBordersInBodyUntilResize = 1 << 12,  // [ALPHA] Disable vertical borders in columns Body until hovered for resize (borders will always appear in Headers). -> May move to style
    // Sizing Policy (read above for defaults)
    ImGuiTableFlags_SizingFixedFit             = 1 << 13,  // Columns default to _WidthFixed or _WidthAuto (if resizable or not resizable), matching contents width.
    ImGuiTableFlags_SizingFixedSame            = 2 << 13,  // Columns default to _WidthFixed or _WidthAuto (if resizable or not resizable), matching the maximum contents width of all columns. Implicitly enable ImGuiTableFlags_NoKeepColumnsVisible.
    ImGuiTableFlags_SizingStretchProp          = 3 << 13,  // Columns default to _WidthStretch with default weights proportional to each columns contents widths.
    ImGuiTableFlags_SizingStretchSame          = 4 << 13,  // Columns default to _WidthStretch with default weights all equal, unless overridden by TableSetupColumn().
    // Sizing Extra Options
    ImGuiTableFlags_NoHostExtendX              = 1 << 16,  // Make outer width auto-fit to columns, overriding outer_size.x value. Only available when ScrollX/ScrollY are disabled and Stretch columns are not used.
    ImGuiTableFlags_NoHostExtendY              = 1 << 17,  // Make outer height stop exactly at outer_size.y (prevent auto-extending table past the limit). Only available when ScrollX/ScrollY are disabled. Data below the limit will be clipped and not visible.
    ImGuiTableFlags_NoKeepColumnsVisible       = 1 << 18,  // Disable keeping column always minimally visible when ScrollX is off and table gets too small. Not recommended if columns are resizable.
    ImGuiTableFlags_PreciseWidths              = 1 << 19,  // Disable distributing remainder width to stretched columns (width allocation on a 100-wide table with 3 columns: Without this flag: 33,33,34. With this flag: 33,33,33). With larger number of columns, resizing will appear to be less smooth.
    // Clipping
    ImGuiTableFlags_NoClip                     = 1 << 20,  // Disable clipping rectangle for every individual columns (reduce draw command count, items will be able to overflow into other columns). Generally incompatible with TableSetupScrollFreeze().
    // Padding
    ImGuiTableFlags_PadOuterX                  = 1 << 21,  // Default if BordersOuterV is on. Enable outermost padding. Generally desirable if you have headers.
    ImGuiTableFlags_NoPadOuterX                = 1 << 22,  // Default if BordersOuterV is off. Disable outermost padding.
    ImGuiTableFlags_NoPadInnerX                = 1 << 23,  // Disable inner padding between columns (double inner padding if BordersOuterV is on, single inner padding if BordersOuterV is off).
    // Scrolling
    ImGuiTableFlags_ScrollX                    = 1 << 24,  // Enable horizontal scrolling. Require 'outer_size' parameter of BeginTable() to specify the container size. Changes default sizing policy. Because this creates a child window, ScrollY is currently generally recommended when using ScrollX.
    ImGuiTableFlags_ScrollY                    = 1 << 25,  // Enable vertical scrolling. Require 'outer_size' parameter of BeginTable() to specify the container size.
    // Sorting
    ImGuiTableFlags_SortMulti                  = 1 << 26,  // Hold shift when clicking headers to sort on multiple column. TableGetSortSpecs() may return specs where (SpecsCount > 1).
    ImGuiTableFlags_SortTristate               = 1 << 27,  // Allow no sorting, disable default sorting. TableGetSortSpecs() may return specs where (SpecsCount == 0).
    // Miscellaneous
    ImGuiTableFlags_HighlightHoveredColumn     = 1 << 28,  // Highlight column headers when hovered (may evolve into a fuller highlight)

    // [Internal] Combinations and masks
    ImGuiTableFlags_SizingMask_                = ImGuiTableFlags_SizingFixedFit | ImGuiTableFlags_SizingFixedSame | ImGuiTableFlags_SizingStretchProp | ImGuiTableFlags_SizingStretchSame,
};

enum ImGuiWindowFlags : int
{
    ImGuiWindowFlags_None                   = 0,
    ImGuiWindowFlags_NoTitleBar             = 1 << 0,   // Disable title-bar
    ImGuiWindowFlags_NoResize               = 1 << 1,   // Disable user resizing with the lower-right grip
    ImGuiWindowFlags_NoMove                 = 1 << 2,   // Disable user moving the window
    ImGuiWindowFlags_NoScrollbar            = 1 << 3,   // Disable scrollbars (window can still scroll with mouse or programmatically)
    ImGuiWindowFlags_NoScrollWithMouse      = 1 << 4,   // Disable user vertically scrolling with mouse wheel. On child window, mouse wheel will be forwarded to the parent unless NoScrollbar is also set.
    ImGuiWindowFlags_NoCollapse             = 1 << 5,   // Disable user collapsing window by double-clicking on it. Also referred to as Window Menu Button (e.g. within a docking node).
    ImGuiWindowFlags_AlwaysAutoResize       = 1 << 6,   // Resize every window to its content every frame
    ImGuiWindowFlags_NoBackground           = 1 << 7,   // Disable drawing background color (WindowBg, etc.) and outside border. Similar as using SetNextWindowBgAlpha(0.0f).
    ImGuiWindowFlags_NoSavedSettings        = 1 << 8,   // Never load/save settings in .ini file
    ImGuiWindowFlags_NoMouseInputs          = 1 << 9,   // Disable catching mouse, hovering test with pass through.
    ImGuiWindowFlags_MenuBar                = 1 << 10,  // Has a menu-bar
    ImGuiWindowFlags_HorizontalScrollbar    = 1 << 11,  // Allow horizontal scrollbar to appear (off by default). You may use SetNextWindowContentSize(ImVec2(width,0.0f)); prior to calling Begin() to specify width. Read code in imgui_demo in the "Horizontal Scrolling" section.
    ImGuiWindowFlags_NoFocusOnAppearing     = 1 << 12,  // Disable taking focus when transitioning from hidden to visible state
    ImGuiWindowFlags_NoBringToFrontOnFocus  = 1 << 13,  // Disable bringing window to front when taking focus (e.g. clicking on it or programmatically giving it focus)
    ImGuiWindowFlags_AlwaysVerticalScrollbar= 1 << 14,  // Always show vertical scrollbar (even if ContentSize.y < Size.y)
    ImGuiWindowFlags_AlwaysHorizontalScrollbar=1<< 15,  // Always show horizontal scrollbar (even if ContentSize.x < Size.x)
    ImGuiWindowFlags_NoNavInputs            = 1 << 16,  // No keyboard/gamepad navigation within the window
    ImGuiWindowFlags_NoNavFocus             = 1 << 17,  // No focusing toward this window with keyboard/gamepad navigation (e.g. skipped by Ctrl+Tab)
    ImGuiWindowFlags_UnsavedDocument        = 1 << 18,  // Display a dot next to the title. When used in a tab/docking context, tab is selected when clicking the X + closure is not assumed (will wait for user to stop submitting the tab). Otherwise closure is assumed when pressing the X, so if you keep submitting the tab may reappear at end of tab bar.
    ImGuiWindowFlags_NoDocking              = 1 << 19,  // Disable docking of this window
    ImGuiWindowFlags_NoNav                  = ImGuiWindowFlags_NoNavInputs | ImGuiWindowFlags_NoNavFocus,
    ImGuiWindowFlags_NoDecoration           = ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags_NoCollapse,
    ImGuiWindowFlags_NoInputs               = ImGuiWindowFlags_NoMouseInputs | ImGuiWindowFlags_NoNavInputs | ImGuiWindowFlags_NoNavFocus,

    // [Internal]
    ImGuiWindowFlags_DockNodeHost           = 1 << 23,  // Don't use! For internal use by Begin()/NewFrame()
    ImGuiWindowFlags_ChildWindow            = 1 << 24,  // Don't use! For internal use by BeginChild()
    ImGuiWindowFlags_Tooltip                = 1 << 25,  // Don't use! For internal use by BeginTooltip()
    ImGuiWindowFlags_Popup                  = 1 << 26,  // Don't use! For internal use by BeginPopup()
    ImGuiWindowFlags_Modal                  = 1 << 27,  // Don't use! For internal use by BeginPopupModal()
    ImGuiWindowFlags_ChildMenu              = 1 << 28,  // Don't use! For internal use by BeginMenu()
};