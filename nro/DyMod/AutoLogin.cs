using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Assets.src.g;
using UnityEngine;

namespace DyMod;

public class AutoLogin
{
	private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	public static string IdClientSocket = "tab_1";

	public static int STT = 1;

	public static int Server = 1;

	public static bool IsDyTest = false;

	public static bool IsRandomName = true;

	public static bool IsEnabled = true;

	public static int TargetFps = 20;

	public static bool IsOptimizeRam = true;

	public static bool IsBlackScreen = false;

	public static int CustomHeight = 0;

	public static int CustomWidth = 0;

	public static string CustomUserAo = string.Empty;

	private static long _lastActionTime;

	private static long _lastLoginAttempt;

	private static long _lastCreateCharAttempt;

	private static IntPtr _cachedWindowHandle = IntPtr.Zero;

	private static string _lastAppliedTitle = string.Empty;

	private static long _lastTitleUpdateCheck = 0L;

	public static void Init()
	{
		try
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs == null || commandLineArgs.Length < 2)
			{
				return;
			}
			for (int i = 1; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				int result4;
				if (text.Equals("-id", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					IdClientSocket = commandLineArgs[++i];
					IsEnabled = true;
					if (IdClientSocket.StartsWith("tab_"))
					{
						int.TryParse(IdClientSocket.Substring(4), out STT);
					}
				}
				else if (text.Equals("-stt", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					if (int.TryParse(commandLineArgs[++i], out var result))
					{
						STT = result;
					}
				}
				else if (text.Equals("-server", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					if (int.TryParse(commandLineArgs[++i], out var result2))
					{
						Server = result2;
					}
				}
				else if (text.Equals("-dytest", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					IsDyTest = commandLineArgs[++i] == "1";
				}
				else if (text.Equals("-randomname", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					IsRandomName = commandLineArgs[++i] == "1";
				}
				else if (text.Equals("-userao", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					CustomUserAo = commandLineArgs[++i];
				}
				else if (text.Equals("-size", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					ParseScreenSize(commandLineArgs[++i]);
				}
				else if (text.Equals("-screen-height", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					if (int.TryParse(commandLineArgs[++i], out var result3))
					{
						CustomHeight = result3;
					}
				}
				else if (text.Equals("-screen-width", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length && int.TryParse(commandLineArgs[++i], out result4))
				{
					CustomWidth = result4;
				}
				else if (text.Equals("-fps", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					if (int.TryParse(commandLineArgs[++i], out var resultFps) && resultFps > 0)
					{
						TargetFps = resultFps;
					}
				}
				else if (text.Equals("-lowram", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					IsOptimizeRam = commandLineArgs[++i] == "1";
				}
				else if (text.Equals("-blackscreen", StringComparison.OrdinalIgnoreCase) && i + 1 < commandLineArgs.Length)
				{
					IsBlackScreen = commandLineArgs[++i] == "1";
				}
			}
			ActionLogger.IsRecordingEnabled = IsDyTest;
			if (TargetFps > 0)
			{
				Application.targetFrameRate = TargetFps;
			}
			MemoryOptimizer.IsEnabled = IsOptimizeRam;
			GameScr.isBlackScreen = IsBlackScreen;
		}
		catch
		{
		}
	}

	public static void ParseScreenSize(string sizeStr)
	{
		try
		{
			if (!string.IsNullOrEmpty(sizeStr))
			{
				string[] array = sizeStr.Split(new char[6] { 'x', 'X', '*', ',', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2 && int.TryParse(array[0].Trim(), out var result) && int.TryParse(array[1].Trim(), out var result2) && result > 0 && result2 > 0)
				{
					CustomHeight = result;
					CustomWidth = result2;
				}
			}
		}
		catch
		{
		}
	}

	public static void Update()
	{
		if (!IsEnabled || IsDyTest)
		{
			return;
		}
		long num = mSystem.currentTimeMillis();
		if (!ServerListScreen.bigOk)
		{
			if ((GameCanvas.currentScreen is ServerListScreen || GameCanvas.serverScreen == GameCanvas.currentScreen) && !ServerListScreen.isGetData)
			{
				if (ServerListScreen.cmdDownload != null)
				{
					ServerListScreen.cmdDownload.performAction();
				}
				else if (GameCanvas.serverScreen != null)
				{
					GameCanvas.serverScreen.perform(2, null);
				}
			}
		}
		else
		{
			if (num - _lastActionTime < 500)
			{
				return;
			}
			_lastActionTime = num;
			if (IsLoginSuccess())
			{
				_lastLoginAttempt = 0L;
			}
			else if (GameCanvas.currentScreen is RegisterScreen || (GameCanvas.registerScr != null && GameCanvas.currentScreen == GameCanvas.registerScr))
			{
				RegisterScreen registerScreen = (GameCanvas.currentScreen as RegisterScreen) ?? GameCanvas.registerScr;
				if (registerScreen != null)
				{
					if (registerScreen.tfUser != null)
					{
						registerScreen.tfUser.setText("a hp hp");
					}
					if (registerScreen.tfSodt != null)
					{
						registerScreen.tfSodt.setText("0981634582");
					}
					if (registerScreen.tfNgay != null)
					{
						registerScreen.tfNgay.setText("2");
					}
					if (registerScreen.tfThang != null)
					{
						registerScreen.tfThang.setText("2");
					}
					if (registerScreen.tfNam != null)
					{
						registerScreen.tfNam.setText("2000");
					}
					Service.gI().charInfo("2", "2", "2000", string.Empty, string.Empty, string.Empty, string.Empty, "0981634582", "a hp hp");
					DismissDialogs();
				}
			}
			else if (GameCanvas.currentScreen is CreateCharScr || CreateCharScr.isCreateChar)
			{
				DismissDialogs();
				if (num - _lastCreateCharAttempt > 3000)
				{
					_lastCreateCharAttempt = num;
					DismissDialogs();
					string text = GetNextSequentialCharName();
					int num2 = 0;
					int hair = CreateCharScr.hairID[0][UnityEngine.Random.Range(0, 3)];
					if (CreateCharScr.tAddName != null)
					{
						CreateCharScr.tAddName.setText(text);
						CreateCharScr.indexGender = num2;
					}
					Service.gI().createChar(text, num2, hair);
				}
			}
			else if (GameCanvas.currentScreen is SelectCharScr || (GameCanvas._SelectCharScr != null && GameCanvas.currentScreen == GameCanvas._SelectCharScr))
			{
				DismissDialogs();
				if (Session_ME.gI().isConnected())
				{
					if (SelectCharScr.gI() != null)
					{
						SelectCharScr.gI().perform(100, null);
					}
					else if (GameCanvas.serverScreen != null)
					{
						GameCanvas.serverScreen.Login_New();
					}
				}
				else
				{
					ServerListScreen.ConnectIP();
				}
			}
			else if (GameCanvas.currentScreen is ServerScr)
			{
				if (GameCanvas.serverScreen != null)
				{
					GameCanvas.serverScreen.switchToMe();
				}
			}
			else if (GameCanvas.currentScreen is LoginScr)
			{
				if (!(GameCanvas.currentDialog is MsgDlg { isWait: not false }) && num - _lastLoginAttempt > 8000)
				{
					_lastLoginAttempt = num;
					string value = Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect);
					if (string.IsNullOrEmpty(value))
					{
						value = CustomUserAo;
					}
					if (!string.IsNullOrEmpty(value))
					{
						GameCanvas.loginScr.doLogin();
					}
					else if (GameCanvas.serverScreen != null)
					{
						GameCanvas.serverScreen.switchToMe();
					}
				}
			}
			else
			{
				if (!(GameCanvas.currentScreen is ServerListScreen) && GameCanvas.serverScreen != GameCanvas.currentScreen)
				{
					return;
				}
				int num3 = Server - 1;
				if (IsValidServerIndex(num3) && ServerListScreen.ipSelect != num3)
				{
					SwitchServer(num3);
				}
				else
				{
					if (GameCanvas.currentDialog is MsgDlg { isWait: not false })
					{
						return;
					}
					DismissDialogs();
					if (num - _lastLoginAttempt < 3000)
					{
						return;
					}
					_lastLoginAttempt = num;
					string value2 = Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect);
					if (string.IsNullOrEmpty(value2) && !string.IsNullOrEmpty(CustomUserAo))
					{
						try
						{
							Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, CustomUserAo);
						}
						catch
						{
						}
					}
					if (GameCanvas.serverScreen != null)
					{
						GameCanvas.serverScreen.Login_New();
					}
				}
			}
		}
	}

	public static void DismissDialogs()
	{
		try
		{
			if (GameCanvas.currentDialog != null)
			{
				if (GameCanvas.currentDialog is MsgDlg { isWait: not false })
				{
					return;
				}
				if (GameCanvas.currentDialog.center != null && GameCanvas.currentDialog.center.idAction == 8884)
				{
					GameCanvas.endDlg();
					return;
				}
				if (GameCanvas.currentDialog.center != null)
				{
					GameCanvas.currentDialog.center.performAction();
				}
				else if (GameCanvas.currentDialog.left != null)
				{
					GameCanvas.currentDialog.left.performAction();
				}
				else
				{
					GameCanvas.endDlg();
				}
			}
			if (InfoDlg.isShow)
			{
				InfoDlg.hide();
			}
			if (ChatPopup.currChatPopup != null)
			{
				ChatPopup.currChatPopup.perform(8000, null);
			}
			if (ChatPopup.serverChatPopUp != null)
			{
				ChatPopup.serverChatPopUp = null;
				Char.chatPopup = null;
				Char.isLockKey = false;
			}
		}
		catch
		{
		}
	}

	private static readonly object _counterLock = new object();
	private const string COUNTER_MUTEX_NAME = "Global\\DragonBoy_CharNameCounter_Mutex";
	private const string COUNTER_FILE_NAME = "char_name_counter.txt";

	private static string[] GetCounterFilePaths()
	{
		List<string> list = new List<string>();
		try
		{
			string iphoneDocumentsPath = Rms.GetiPhoneDocumentsPath();
			if (!string.IsNullOrEmpty(iphoneDocumentsPath))
			{
				list.Add(Path.Combine(iphoneDocumentsPath, COUNTER_FILE_NAME));
			}
		}
		catch
		{
		}
		try
		{
			if (!string.IsNullOrEmpty(Application.dataPath))
			{
				list.Add(Path.Combine(Application.dataPath, "../" + COUNTER_FILE_NAME));
			}
		}
		catch
		{
		}
		try
		{
			list.Add("g:\\My Drive\\upnhs\\" + COUNTER_FILE_NAME);
		}
		catch
		{
		}
		return list.ToArray();
	}

	public static string GetNextSequentialCharName()
	{
		lock (_counterLock)
		{
			Mutex mutex = null;
			bool hasHandle = false;
			try
			{
				try
				{
					mutex = new Mutex(false, COUNTER_MUTEX_NAME);
					hasHandle = mutex.WaitOne(3000, false);
				}
				catch (AbandonedMutexException)
				{
					hasHandle = true;
				}
				catch
				{
				}

				long currentCounter = 1L;
				string[] counterFilePaths = GetCounterFilePaths();
				for (int i = 0; i < counterFilePaths.Length; i++)
				{
					try
					{
						string path = counterFilePaths[i];
						if (File.Exists(path))
						{
							string text = File.ReadAllText(path).Trim();
							if (long.TryParse(text, out var val) && val > currentCounter)
							{
								currentCounter = val;
							}
						}
					}
					catch
					{
					}
				}

				try
				{
					string[] possibleAccFiles = new string[]
					{
						Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accounts.json"),
						Path.Combine(Application.dataPath, "../../qltk/bin/Debug/net8.0-windows/accounts.json"),
						"g:\\My Drive\\upnhs\\qltk\\bin\\Debug\\net8.0-windows\\accounts.json"
					};
					for (int j = 0; j < possibleAccFiles.Length; j++)
					{
						string accPath = possibleAccFiles[j];
						if (File.Exists(accPath))
						{
							string content = File.ReadAllText(accPath);
							int idx = 0;
							while ((idx = content.IndexOf("\"dn", idx, StringComparison.OrdinalIgnoreCase)) != -1)
							{
								int start = idx + 3;
								int len = 0;
								while (start + len < content.Length && char.IsDigit(content[start + len]))
								{
									len++;
								}
								if (len > 0)
								{
									string sub = content.Substring(start, len);
									if (long.TryParse(sub, out var existingNum) && existingNum >= currentCounter)
									{
										currentCounter = existingNum + 1;
									}
								}
								idx += 3;
							}
						}
					}
				}
				catch
				{
				}

				string name = string.Format("dn{0:D8}", currentCounter);
				long nextCounter = currentCounter + 1;

				for (int k = 0; k < counterFilePaths.Length; k++)
				{
					try
					{
						string path2 = counterFilePaths[k];
						string dir = Path.GetDirectoryName(path2);
						if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
						{
							Directory.CreateDirectory(dir);
						}
						File.WriteAllText(path2, nextCounter.ToString());
					}
					catch
					{
					}
				}

				return name;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("Error in GetNextSequentialCharName: " + ex);
				return "dn00000001";
			}
			finally
			{
				if (hasHandle && mutex != null)
				{
					try
					{
						mutex.ReleaseMutex();
					}
					catch
					{
					}
					try
					{
						mutex.Close();
					}
					catch
					{
					}
				}
			}
		}
	}

	public static string GenerateRandomName(int minLength = 5, int maxLength = 8)
	{
		return GetNextSequentialCharName();
	}

	public static bool IsLoginSuccess()
	{
		try
		{
			Char obj = Char.myCharz();
			return !(GameCanvas.currentScreen is ServerListScreen) && !(GameCanvas.currentScreen is LoginScr) && !(GameCanvas.currentScreen is CreateCharScr) && !CreateCharScr.isCreateChar && !(GameCanvas.currentScreen is RegisterScreen) && (GameCanvas.registerScr == null || GameCanvas.currentScreen != GameCanvas.registerScr) && !(GameCanvas.currentScreen is SelectCharScr) && !(GameCanvas.currentScreen is ServerScr) && obj != null && !string.IsNullOrEmpty(obj.cName);
		}
		catch
		{
			return false;
		}
	}

	private static bool IsValidServerIndex(int index)
	{
		return ServerListScreen.nameServer != null && index >= 0 && index < ServerListScreen.nameServer.Length;
	}

	private static void SwitchServer(int index)
	{
		try
		{
			Rms.saveRMSInt(ServerListScreen.RMS_svselect, index);
			ServerListScreen.ipSelect = index;
			if (GameCanvas.serverScreen != null)
			{
				GameCanvas.serverScreen.selectServer();
			}
		}
		catch
		{
		}
	}

	[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowTextW")]
	private static extern bool SetWindowText(IntPtr hWnd, string lpString);

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	public static IntPtr GetGameWindowHandle()
	{
		if (_cachedWindowHandle != IntPtr.Zero)
		{
			return _cachedWindowHandle;
		}
		try
		{
			uint currentPid = (uint)Process.GetCurrentProcess().Id;
			IntPtr foundHwnd = IntPtr.Zero;
			EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
			{
				GetWindowThreadProcessId(hWnd, out var lpdwProcessId);
				if (lpdwProcessId == currentPid && IsWindowVisible(hWnd))
				{
					foundHwnd = hWnd;
					return false;
				}
				return true;
			}, IntPtr.Zero);
			if (foundHwnd != IntPtr.Zero)
			{
				_cachedWindowHandle = foundHwnd;
				return foundHwnd;
			}
		}
		catch
		{
		}
		try
		{
			IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
			if (mainWindowHandle != IntPtr.Zero)
			{
				_cachedWindowHandle = mainWindowHandle;
				return mainWindowHandle;
			}
		}
		catch
		{
		}
		return IntPtr.Zero;
	}

	public static void UpdateWindowTitle()
	{
		long num = mSystem.currentTimeMillis();
		if (num - _lastTitleUpdateCheck < 1000)
		{
			return;
		}
		_lastTitleUpdateCheck = num;
		try
		{
			if (STT <= 0)
			{
				if (!string.IsNullOrEmpty(IdClientSocket) && IdClientSocket.StartsWith("tab_"))
				{
					int.TryParse(IdClientSocket.Substring(4), out STT);
				}
				if (STT <= 0)
				{
					STT = 1;
				}
			}
			Char obj = Char.myCharz();
			string text = ((obj == null || string.IsNullOrEmpty(obj.cName)) ? $"STT {STT} - SV {Server}" : $"STT {STT} - {obj.cName}");
			if (!(text == _lastAppliedTitle))
			{
				IntPtr gameWindowHandle = GetGameWindowHandle();
				if (gameWindowHandle != IntPtr.Zero && SetWindowText(gameWindowHandle, text))
				{
					_lastAppliedTitle = text;
				}
			}
		}
		catch
		{
		}
	}
}
