using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace DyMod;

public static class ActionLogger
{
	private static readonly Queue<string> _logQueue = new Queue<string>();

	private static readonly object _queueLock = new object();

	private static Thread _workerThread;

	private static volatile bool _isRunning = false;

	private static bool _initialized = false;

	private static string _primaryLogPath = "g:\\My Drive\\dycopy\\game_action_log.txt";

	private static string _localLogPath = string.Empty;

	private static long _lastMoveLogTime;

	private static int _lastLoggedX = -999;

	private static int _lastLoggedY = -999;

	public static bool IsRecordingEnabled = false;

	public static void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			try
			{
				_localLogPath = Path.Combine(Application.dataPath, "../game_action_log.txt");
			}
			catch
			{
				_localLogPath = string.Empty;
			}
			_isRunning = true;
			_workerThread = new Thread(WorkerLoop)
			{
				IsBackground = true,
				Priority = System.Threading.ThreadPriority.BelowNormal
			};
			_workerThread.Start();
			Log("SYSTEM", "=== BẮT ĐẦU PHIÊN GHI LOG HÀNH VI (ACTION RECORDER) ===");
		}
	}

	private static void WorkerLoop()
	{
		List<string> list = new List<string>(128);
		while (_isRunning)
		{
			list.Clear();
			lock (_queueLock)
			{
				while (_logQueue.Count > 0 && list.Count < 200)
				{
					list.Add(_logQueue.Dequeue());
				}
			}
			if (list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(list.Count * 128);
				for (int i = 0; i < list.Count; i++)
				{
					stringBuilder.Append(list[i]);
				}
				string content = stringBuilder.ToString();
				WriteBatchToFile(_primaryLogPath, content);
				string idClientSocket = AutoLogin.IdClientSocket;
				if (!string.IsNullOrEmpty(idClientSocket))
				{
					string path = $"g:\\My Drive\\dycopy\\game_action_log_{idClientSocket}.txt";
					WriteBatchToFile(path, content);
				}
				if (!string.IsNullOrEmpty(_localLogPath))
				{
					WriteBatchToFile(_localLogPath, content);
				}
			}
			try
			{
				Thread.Sleep(200);
			}
			catch
			{
				if (!_isRunning)
				{
					break;
				}
			}
		}
	}

	private static void WriteBatchToFile(string path, string content)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			using FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
			streamWriter.Write(content);
			streamWriter.Flush();
		}
		catch
		{
		}
	}

	public static void Log(string category, string message)
	{
		if (!IsRecordingEnabled)
		{
			return;
		}
		try
		{
			string text = DateTime.Now.ToString("HH:mm:ss.fff");
			string text2 = ((!string.IsNullOrEmpty(AutoLogin.IdClientSocket)) ? AutoLogin.IdClientSocket : "tab1");
			string contextInfo = GetContextInfo();
			string item = $"[{text}] [{text2}] [{category}] {message}{contextInfo}\r\n";
			lock (_queueLock)
			{
				if (_logQueue.Count < 5000)
				{
					_logQueue.Enqueue(item);
				}
			}
		}
		catch
		{
		}
	}

	private static string GetContextInfo()
	{
		try
		{
			Char obj = Char.myCharz();
			if (obj != null && !string.IsNullOrEmpty(obj.cName) && TileMap.mapID >= 0)
			{
				string text = ((!string.IsNullOrEmpty(TileMap.mapName)) ? TileMap.mapName : "Unknown");
				return $" | [Map={TileMap.mapID} ({text}), Pos=({obj.cx},{obj.cy}), Power={obj.cPower}, Xu={obj.xu}, HP={obj.cHP}/{obj.cHPFull}]";
			}
		}
		catch
		{
		}
		return string.Empty;
	}

	public static void LogScreen(string screenName)
	{
		Log("SCREEN", "Chuyển màn hình: " + screenName);
	}

	public static void LogUserAction(string actionName, string detail = "")
	{
		string text = actionName;
		if (!string.IsNullOrEmpty(detail))
		{
			text = text + " -> " + detail;
		}
		Log("USER_ACTION", text);
	}

	private static bool ShouldFilterPacket(sbyte cmd)
	{
		switch (cmd)
		{
		case -123:
		case -122:
		case -121:
		case -113:
		case -93:
		case -74:
		case -38:
		case -32:
		case 12:
		case 51:
		case 52:
			return true;
		default:
			return false;
		}
	}

	public static void LogPacketSend(sbyte cmd, string detail = "")
	{
		if (ServerListScreen.bigOk && !ServerListScreen.isGetData && SplashScr.nData == -1 && !ShouldFilterPacket(cmd))
		{
			string commandName = GetCommandName(cmd, isSend: true);
			string text = $"Gửi gói tin cmd={cmd} ({commandName})";
			if (!string.IsNullOrEmpty(detail))
			{
				text = text + " | Chi tiết: " + detail;
			}
			Log("PACKET_SEND", text);
		}
	}

	public static void LogPacketRecv(sbyte cmd, string detail = "")
	{
		if (ServerListScreen.bigOk && !ServerListScreen.isGetData && SplashScr.nData == -1 && !ShouldFilterPacket(cmd))
		{
			string commandName = GetCommandName(cmd, isSend: false);
			string text = $"Nhận gói tin cmd={cmd} ({commandName})";
			if (!string.IsNullOrEmpty(detail))
			{
				text = text + " | Chi tiết: " + detail;
			}
			Log("PACKET_RECV", text);
		}
	}

	public static void LogMove(int cx, int cy)
	{
		long num = mSystem.currentTimeMillis();
		if (num - _lastMoveLogTime > 1000 || System.Math.Abs(cx - _lastLoggedX) > 100 || System.Math.Abs(cy - _lastLoggedY) > 100)
		{
			_lastMoveLogTime = num;
			_lastLoggedX = cx;
			_lastLoggedY = cy;
			string text = ((!string.IsNullOrEmpty(TileMap.mapName)) ? TileMap.mapName : "Unknown");
			Log("MOVE", $"Di chuyển tới: x={cx}, y={cy} trong Map {TileMap.mapID} ({text})");
		}
	}

	public static void LogMapChange(int fromMap, int toMap, string mapName)
	{
		string arg = ((!string.IsNullOrEmpty(mapName)) ? mapName : "Unknown");
		Log("MAP_CHANGE", $"Chuyển Map từ ID={fromMap} sang ID={toMap} ({arg})");
	}

	public static void LogAttack(int skillId, string skillName, int mobId, string mobName, int mobHp)
	{
		Log("ATTACK", $"Tấn công quái: ID={mobId} ({mobName}), HP={mobHp} bằng Skill ID={skillId} ({skillName})");
	}

	public static void LogNpcDialog(int npcId, string npcName, string[] menuOptions)
	{
		string arg = ((menuOptions != null) ? string.Join(", ", menuOptions) : "none");
		Log("NPC_DIALOG", $"Tương tác NPC ID={npcId} ({npcName}), Menu: [{arg}]");
	}

	public static void LogQuest(int taskId, int index, string taskName, string taskDesc)
	{
		Log("QUEST", $"Nhiệm vụ: TaskID={taskId}, Bước={index}, Tên='{taskName}', Mô tả='{taskDesc}'");
	}

	private static string GetCommandName(sbyte cmd, bool isSend)
	{
		return cmd switch
		{
			-101 => "login2 (Tạo user ảo / Login phụ)", 
			-102 => "checkVersion / login verify", 
			0 => "login (Đăng nhập)", 
			-27 => "getKey (Handshake mã hóa)", 
			-7 => "charMove (Di chuyển tọa độ)", 
			-23 => "requestChangeMap (Đổi map qua Waypoint)", 
			54 => "sendPlayerAttack (Đánh quái)", 
			-60 => "sendPlayerAttack (Đánh người)", 
			-4 => "sendPlayerAttack (Đánh mục tiêu)", 
			67 => "sendPlayerAttack (Skill đặc biệt)", 
			-43 => "pickItem (Nhặt vật phẩm/vàng)", 
			-30 => "dialogAction / menu (Chọn menu NPC)", 
			-34 => "magicTree (Thu đậu / tương tác đậu)", 
			-28 => "charAction (Tạo nhân vật / thao tác NV)", 
			-29 => "authAction (Xác thực / đăng ký)", 
			1 => "loadChar (Tải dữ liệu nhân vật)", 
			2 => "openCreateChar (Mở màn hình tạo nhân vật)", 
			-107 => "petInfo (Thông tin đệ tử)", 
			-100 => "charData (Dữ liệu trang bị / thuộc tính)", 
			-20 => "itemMap (Vật phẩm rơi trên đất)", 
			-22 => "chat (Tin nhắn chat)", 
			40 => "quest / task (Cập nhật nhiệm vụ)", 
			_ => isSend ? "Packet Gửi" : "Packet Nhận", 
		};
	}
}
