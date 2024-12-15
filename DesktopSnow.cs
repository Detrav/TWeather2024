using Godot;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TWeather2024;
using static Godot.OpenXRInterface;

public partial class DesktopSnow : Node2D
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);


    private const int GwlExStyle = -20;
    private const int GWL_STYLE = -16;
    private const uint WsExLayered = 0x00080000;
    private const uint WsExTransparent = 0x00000020;
    private const uint WS_EX_APPWINDOW = 0x00040000;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;
    private const uint WS_VISIBLE = 0x10000000;
    private MultiMeshInstance2D meshinstance;
    private CustomParticlesInstance cpi;
    private bool updateWindows = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        GetTree().Root.Borderless = true;
        GetTree().Root.Transparent = true;
        GetTree().Root.TransparentBg = true;
        GetWindow().Transparent = true;
        GetWindow().TransparentBg = true;
        GetWindow().AlwaysOnTop = true;
        GetWindow().Position = Vector2I.Zero;
        GetWindow().Size = DisplayServer.ScreenGetSize(GetWindow().CurrentScreen);
        GetWindow().MousePassthrough = true;
        GetWindow().MousePassthroughPolygon = new Vector2[] { };



        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Transparent, true);
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.NoFocus, true);
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.MousePassthrough, true);

        var _hWnd = GetActiveWindow();
        var flags = GetWindowLong(_hWnd, GwlExStyle);
        flags |= WsExTransparent | WsExLayered;
        flags = flags & ~WS_EX_APPWINDOW & ~WS_EX_TOOLWINDOW;
        SetWindowLong(_hWnd, GwlExStyle, flags);

        var size = GetWindow().Size;

        var part = GetNode<CpuParticles2D>("CPUParticles2D");
        part.EmissionRectExtents = new Vector2(size.X / 2 + 100, 0);
        part.Position = new Vector2(size.X / 2, 0);

        meshinstance = GetNode<MultiMeshInstance2D>("MultiMeshInstance2D");

        cpi = new CustomParticlesInstance(5000);
        cpi.MinX = -10;
        cpi.MaxX = size.X + 10;
        cpi.MinY = -10;
        cpi.MaxY = size.Y + 10;
        meshinstance.Multimesh.InstanceCount = cpi.Count;
        // have a bug with godot 4 not working
        meshinstance.Multimesh.VisibleInstanceCount = -1;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // do not update window by layers, dont want
        //if (updateWindows)
        //{
        //    //Stopwatch sw = Stopwatch.StartNew();
        //    var hwnds = new ArrayList();
        //    EnumWindows(EnumWindowsCallback, hwnds);
        //    //updateWindows = false;

        //    //GD.Print(hwnds.Count);

        //    foreach (IntPtr hwnd in hwnds)
        //    {
        //        Rect rect = new Rect();
        //        if (GetWindowRect(hwnd, ref rect))
        //        {
        //            if (rect.Top > 10 && (rect.Bottom - rect.Top) > 10 && (rect.Right - rect.Left) > 10)
        //            {
        //                GD.Print(rect);
        //                //cpi.SetWindowPosition(hwnd, rect);
        //            }
        //            //
        //        }
        //    }
        //    //GD.Print(sw.Elapsed);
        //}

        cpi.ProcessCurrent(delta);
        meshinstance = GetNode<MultiMeshInstance2D>("MultiMeshInstance2D");
        meshinstance.Multimesh.Buffer = cpi.GetBuffer();
        // have a bug with godot 4 not working
        //meshinstance.Multimesh.VisibleInstanceCount = cpi.VisibleCount;

    }


    static bool EnumWindowsCallback(IntPtr hWnd, ArrayList lParam)
    {
        uint exStyle = GetWindowLong(hWnd, GwlExStyle);
        uint style = GetWindowLong(hWnd, GWL_STYLE);
        if ((style & WS_VISIBLE) == WS_VISIBLE)
        {
            lParam.Add(hWnd);
        }
        return true;
    }



    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ArrayList lParam);

    public delegate bool EnumWindowsProc(IntPtr hwnd, ArrayList lParam);

    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", Left, Top, Right, Bottom);
        }
    }
}
