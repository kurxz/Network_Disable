using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Collections;

namespace Network_Disable
{
	class Program
	{

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[
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

		static void Main(string[] args)
		{
			Console.Title = "Network Disable";

			Console.WriteLine("Este aplicativo irá desativar/reativar todos os seus dispositivos de rede");
			Console.WriteLine("Pressione a tecla F3 para Desativar. E F4 para Reativar");
			Console.WriteLine("Caso deseje encerrar o app pressione ESC");
			Console.WriteLine("Download original em https://github.com/kurxz/Network_Disable \n");

			_hookID = SetHook(_proc);
			Application.Run();
			UnhookWindowsHookEx(_hookID);

		}

		private static IntPtr SetHook(LowLevelKeyboardProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
			}
		}

		private delegate IntPtr LowLevelKeyboardProc(
		int nCode, IntPtr wParam, IntPtr lParam);

		private static IntPtr HookCallback(
		int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
			{
				int vkCode = Marshal.ReadInt32(lParam);

				//A linha abaixo Imprime o código da tecla no console
				//Console.WriteLine(vkCode);

				processarInfo(vkCode);
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		static void processarInfo(int info)
		{

			switch (info)
			{

				case 27:
					//ESC
					Application.Exit();

					break;

				case 114:
					//F3

					Desabilitar();

					break;

				case 115:
					//F4

					Habilitar();

					break;

				default:

					break;
			}

		}

		static void Desabilitar()
		{
			Console.Write("Desativando, aguarde...\n");
			try
			{
				PowerShell.Create()
				.AddCommand("Disable-NetAdapter")
				.AddParameter("Name", "*")
				.AddParameter("Confirm", new SwitchParameter(false))
				.Invoke();

				Console.Write("Dispositivos desativados\n");

			}
			catch (Exception e)
			{
				Console.Write("Ocorreu um erro ao desativar os dispositivos\n");
			}

		}

		static void Habilitar()
		{
			Console.Write("Ativando, aguarde...\n");

			try
			{
				PowerShell.Create()
				.AddCommand("Enable-NetAdapter")
				.AddParameter("Name", "*")
				.Invoke();

				Console.Write("Dispositivos ativados\nSua internet voltará em breve");
				
			}
			catch (Exception e)
			{
				Console.Write("Ocorreu um erro ao ativar os dispositivos\n");
			}
		}
	}
}
