using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace DyMod;

public class SocketClient
{
	private static TcpClient _client;

	private static NetworkStream _stream;

	private static Thread _listenThread;

	private static volatile bool _isRunning;

	private static long _lastConnectAttempt;

	private static long _lastSyncTime;

	private static int _cachedPid;

	private static readonly Queue<string> _messageQueue = new Queue<string>();

	private static readonly object _queueLock = new object();

	public static bool IsRunning => _isRunning;

	public static int ProcessId
	{
		get
		{
			if (_cachedPid == 0)
			{
				try
				{
					_cachedPid = Process.GetCurrentProcess().Id;
				}
				catch
				{
					_cachedPid = 1;
				}
			}
			return _cachedPid;
		}
	}

	public static bool Connect(string host = "127.0.0.1", int port = 9999)
	{
		if (_isRunning)
		{
			return true;
		}
		try
		{
			_client = new TcpClient();
			_client.ReceiveBufferSize = 4096;
			_client.SendBufferSize = 4096;
			_client.Connect(host, port);
			_stream = _client.GetStream();
			_isRunning = true;
			_listenThread = new Thread(ListenFromServer)
			{
				IsBackground = true
			};
			_listenThread.Start();
			if (!string.IsNullOrEmpty(AutoLogin.IdClientSocket))
			{
				Send("REQ_CONFIG|" + AutoLogin.IdClientSocket);
			}
			return true;
		}
		catch (Exception)
		{
			Disconnect();
			return false;
		}
	}

	private static void ListenFromServer()
	{
		byte[] array = new byte[4096];
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			while (_isRunning && _stream != null)
			{
				int num = _stream.Read(array, 0, array.Length);
				if (num <= 0)
				{
					Disconnect();
					break;
				}
				stringBuilder.Append(Encoding.UTF8.GetString(array, 0, num));
				string text = stringBuilder.ToString();
				int num2;
				while ((num2 = text.IndexOf('\n')) != -1)
				{
					string text2 = text.Substring(0, num2).Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						lock (_queueLock)
						{
							_messageQueue.Enqueue(text2);
						}
					}
					text = text.Substring(num2 + 1);
				}
				stringBuilder.Length = 0;
				stringBuilder.Append(text);
			}
		}
		catch
		{
			Disconnect();
		}
	}

	public static void Send(string message)
	{
		try
		{
			if (_isRunning && _stream != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(message + "\n");
				_stream.Write(bytes, 0, bytes.Length);
				_stream.Flush();
			}
		}
		catch
		{
			Disconnect();
		}
	}

	public static void Update()
	{
		if (!_isRunning)
		{
			long num = mSystem.currentTimeMillis();
			if (num - _lastConnectAttempt > 3000)
			{
				_lastConnectAttempt = num;
				Connect();
			}
			return;
		}
		lock (_queueLock)
		{
			while (_messageQueue.Count > 0)
			{
				string message = _messageQueue.Dequeue();
				HandleIncomingMessage(message);
			}
		}
		long num2 = mSystem.currentTimeMillis();
		if (num2 - _lastSyncTime >= 5000)
		{
			_lastSyncTime = num2;
			SendClientData();
		}
	}

	public static void ForceSync()
	{
		_lastSyncTime = mSystem.currentTimeMillis();
		SendClientData();
	}

	private static void SendClientData()
	{
		if (string.IsNullOrEmpty(AutoLogin.IdClientSocket))
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string textTiemNang = string.Empty;
		try
		{
			Char obj = Char.myCharz();
			if (obj != null && !string.IsNullOrEmpty(obj.cName))
			{
				text = obj.cName;
				text2 = obj.cgender.ToString();
				text3 = obj.cPower.ToString();
				text4 = obj.xu.ToString();
				textTiemNang = obj.cTiemNang.ToString();
			}
		}
		catch
		{
		}
		int num = ServerListScreen.ipSelect + 1;
		if (num <= 0)
		{
			num = AutoLogin.Server;
		}
		string currentStatus = GetCurrentStatus();
		string text5 = ProcessId.ToString();
		string text6 = string.Empty;
		if (AutoLogin.IsRealAccount)
		{
			text6 = AutoLogin.CustomUser;
		}
		else
		{
			try
			{
				text6 = Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect);
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text6))
			{
				text6 = AutoLogin.CustomUserAo;
			}
		}
		string message = $"DATA|{AutoLogin.IdClientSocket}|{text}|{num}|{text2}|{text3}|{text4}|{currentStatus}|{text5}|{text6}|{textTiemNang}";
		Send(message);
	}

	private static string GetCurrentStatus()
	{
		if (AutoLogin.IsDyTest)
		{
			return "dytest: đang ghi log thao tác";
		}
		if (!ServerListScreen.bigOk)
		{
			if (ServerListScreen.isGetData)
			{
				return "đang tải dữ liệu " + ServerListScreen.percent + "%";
			}
			return "đang tải dữ liệu...";
		}
		if (GameCanvas.currentScreen is CreateCharScr || CreateCharScr.isCreateChar)
		{
			return "đang tạo nhân vật";
		}
		Char obj = Char.myCharz();
		if (obj == null || string.IsNullOrEmpty(obj.cName))
		{
			return "đang mở tab";
		}
		if (!string.IsNullOrEmpty(AutoTrain.CurrentStatus))
		{
			return AutoTrain.CurrentStatus;
		}
		return "đang làm nhiệm vụ 1";
	}

	private static void HandleIncomingMessage(string message)
	{
		try
		{
			string[] array = message.Split('|');
			if (array.Length < 2)
			{
				return;
			}
			string text = array[0];
			if (text.Equals("CMD", StringComparison.OrdinalIgnoreCase) && array.Length >= 2)
			{
				string subCmd = array[1];
				if (subCmd.Equals("CLEAN_RAM", StringComparison.OrdinalIgnoreCase))
				{
					MemoryOptimizer.CleanMemory();
				}
				else if (subCmd.Equals("SET_BLACKSCREEN", StringComparison.OrdinalIgnoreCase) && array.Length >= 3)
				{
					GameScr.isBlackScreen = array[2] == "1";
				}
				else if (subCmd.Equals("SET_FPS", StringComparison.OrdinalIgnoreCase) && array.Length >= 3 && int.TryParse(array[2], out var f) && f > 0)
				{
					AutoLogin.TargetFps = f;
					Application.targetFrameRate = f;
				}
				return;
			}
			if (!text.Equals("CONFIG", StringComparison.OrdinalIgnoreCase) || array.Length < 5)
			{
				return;
			}
			if (int.TryParse(array[2], out var result))
			{
				AutoLogin.Server = result;
			}
			AutoLogin.IsRandomName = array[3] == "1";
			AutoLogin.IsDyTest = array[4] == "1";
			ActionLogger.IsRecordingEnabled = AutoLogin.IsDyTest;
			if (array.Length >= 6 && !string.IsNullOrEmpty(array[5]))
			{
				AutoLogin.ParseScreenSize(array[5]);
				if (AutoLogin.CustomWidth > 0 && AutoLogin.CustomHeight > 0)
				{
					Screen.SetResolution(AutoLogin.CustomWidth, AutoLogin.CustomHeight, fullscreen: false);
				}
			}
			if (array.Length < 7 || string.IsNullOrEmpty(array[6]))
			{
				return;
			}
			if (string.IsNullOrEmpty(AutoLogin.CustomUserAo))
			{
				AutoLogin.CustomUserAo = array[6];
			}
			try
			{
				string value = Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect);
				if (string.IsNullOrEmpty(value))
				{
					Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, array[6]);
				}
			}
			catch
			{
			}
			if (array.Length >= 8 && int.TryParse(array[7], out var fpsVal) && fpsVal > 0)
			{
				AutoLogin.TargetFps = fpsVal;
				Application.targetFrameRate = fpsVal;
			}
			if (array.Length >= 9)
			{
				AutoLogin.IsOptimizeRam = array[8] == "1";
				MemoryOptimizer.IsEnabled = AutoLogin.IsOptimizeRam;
			}
			if (array.Length >= 10)
			{
				AutoLogin.IsBlackScreen = array[9] == "1";
				GameScr.isBlackScreen = AutoLogin.IsBlackScreen;
			}
		}
		catch
		{
		}
	}

	public static void Disconnect()
	{
		if (!_isRunning)
		{
			return;
		}
		_isRunning = false;
		try
		{
			if (_stream != null)
			{
				_stream.Close();
			}
			if (_client != null)
			{
				_client.Close();
			}
		}
		catch
		{
		}
		_listenThread = null;
		_stream = null;
		_client = null;
	}
}
