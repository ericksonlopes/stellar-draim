using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    // === Win32 API ===
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")] private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")] private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")] public static extern bool ReleaseCapture();
    [DllImport("user32.dll")] public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private struct MARGINS { public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight; }
    [DllImport("Dwmapi.dll")] private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // === Constantes ===
    private const int GWL_STYLE   = -16;
    private const int GWL_EXSTYLE = -20;

    private const uint WS_POPUP   = 0x80000000;
    private const uint WS_VISIBLE = 0x10000000;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint LWA_COLORKEY  = 0x00000001;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOMOVE       = 0x0002;
    private const uint SWP_NOSIZE       = 0x0001;
    private const uint SWP_FRAMECHANGED = 0x0020;
    private const uint SWP_SHOWWINDOW   = 0x0040;

    private const uint WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION         = 0x2;
    private const int SW_SHOW           = 5;

    // MAGENTA como cor transparente (mais confiável que preto)
    // COLORREF formato 0x00BBGGRR: Magenta (R=255,G=0,B=255) = 0x00FF00FF
    private const uint TRANSPARENT_COLOR = 0x00FF00FF;

    private IntPtr hWnd;

    void Start()
    {
#if !UNITY_EDITOR
        // Força a cor de fundo da câmera para Magenta (a cor que será transparente)
        Camera.main.backgroundColor = new Color(1f, 0f, 1f, 1f); // Magenta puro
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // Janela do tamanho do monitor inteiro (fundo transparente, então é invisível)
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed);
        Application.runInBackground = true;

        StartCoroutine(ApplyTransparencyDelayed());
#endif
    }

    IEnumerator ApplyTransparencyDelayed()
    {
        // Espera o Unity criar a janela completamente
        yield return new WaitForSeconds(1f);

        hWnd = GetActiveWindow();

        // Aplica 30 frames seguidos para garantir
        for (int i = 0; i < 30; i++)
        {
            ApplyWindowStyle();
            yield return null;
        }
    }

    private void ApplyWindowStyle()
    {
        if (hWnd == IntPtr.Zero)
        {
            hWnd = GetActiveWindow();
            if (hWnd == IntPtr.Zero) return;
        }

        // 1. DWM: Estende o frame para toda a área da janela
        var margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // 2. Torna a janela Layered (necessário para color key)
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);

        // 3. Define MAGENTA como cor transparente (tudo que for magenta some da tela)
        SetLayeredWindowAttributes(hWnd, TRANSPARENT_COLOR, 0, LWA_COLORKEY);

        // 4. Remove borda e barra de título
        SetWindowLong(hWnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);

        // 5. Sempre no topo + refresh
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED | SWP_SHOWWINDOW);

        ShowWindow(hWnd, SW_SHOW);
    }

    /// <summary>Arrasta a janela inteira do Windows ao clicar na nave.</summary>
    public static void DragWindow()
    {
#if !UNITY_EDITOR
        ReleaseCapture();
        SendMessage(GetActiveWindow(), WM_NCLBUTTONDOWN, HTCAPTION, 0);
#endif
    }
}
