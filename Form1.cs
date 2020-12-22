using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Network_Disable {
  public partial class Form1: Form {

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)][
    return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    public static String teclaF = "";

    public Form1() {
      InitializeComponent();
      _hookID = SetHook(_proc);

      Application.Run();

      UnhookWindowsHookEx(_hookID);

    }

    private void Form1_Load(object sender, EventArgs e) {

      this.notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
      this.notifyIcon1.ContextMenuStrip.Items.Add("Sair", null, this.MenuSair_Click);
      this.WindowState = FormWindowState.Minimized;

    }

    private void Form1_Resize(object sender, EventArgs e) {
      if (this.WindowState == FormWindowState.Minimized) {
        this.Hide();

      }
    }

    private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
      this.Show();
      this.WindowState = FormWindowState.Normal;

    }
    static void fechar() {
      Application.Exit();

      foreach(var process in Process.GetProcessesByName("Network_Disable")) {
        process.Kill();
      }
    }

    void MenuSair_Click(object sender, EventArgs e) {
      fechar();

    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
      fechar();
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start("https://github.com/kurxz/Network_Disable/");
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc) {
      using(Process curProcess = Process.GetCurrentProcess())
      using(ProcessModule curModule = curProcess.MainModule) {
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
      }
    }

    private delegate IntPtr LowLevelKeyboardProc(
    int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(
    int nCode, IntPtr wParam, IntPtr lParam) {
      if (nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN) {
        int vkCode = Marshal.ReadInt32(lParam);

        Debug.WriteLine(vkCode);
        processarInfo(vkCode);

      }
      return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    static void processarInfo(int tecla) {

      if ((Control.ModifierKeys & Keys.Shift) != 0) {

        switch (tecla) {

        case 35:

          fechar();

          break;

        case 115:

          Desativar();

          break;

        case 116:

          Ativar();

          break;

        default:

          break;

        }

      }

    }

    static void Desativar() {
      try {

        System.Diagnostics.Process.Start("CMD.exe", "/c wmic path win32_networkadapter where NetEnabled=TRUE call disable");
        SystemSounds.Beep.Play();

      } catch(Exception) {

        MessageBox.Show("Não foi possivel desativar o adaptador");
      }
    }

    static void Ativar() {
      try {

        System.Diagnostics.Process.Start("CMD.exe", "/C wmic path win32_networkadapter where NetEnabled=FALSE call enable");
        SystemSounds.Beep.Play();

      } catch(Exception) {

        MessageBox.Show("Não foi possivel ativar o adaptador");
      }
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
      fechar();

    }

  }
}