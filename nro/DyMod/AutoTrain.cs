using System.Diagnostics;
using System.Threading;
using Assets.src.g;
using UnityEngine;

namespace DyMod;

public static class AutoTrain
{
	public static bool IsEnabled = true;

	public static string CurrentStatus = "đang mở tab";

	private static long _lastTickTime = 0L;

	private static long _lastPeaTime = 0L;

	private const int TICK_INTERVAL = 100;

	private const long TARGET_POWER = 1300000L;

	private static bool _hasShownCompleted = false;

	private static long _lastMapChangeRequestTime = 0L;

	private static long _lastPower = 0L;

	private static long _lastTiemNang = 0L;

	private static long _lastProgressTime = 0L;

	private static int _lastProgressMapId = -1;

	private static int _lastProgressTaskId = -1;

	private static int _lastProgressTaskIndex = -1;

	private const long INACTIVE_TIMEOUT_MS = 60000L;

	public static void NotifyProgress()
	{
		_lastProgressTime = mSystem.currentTimeMillis();
	}

	public static void NotifyMapChangeRequested(long time)
	{
		_lastMapChangeRequestTime = time;
		_lastProgressTime = time;
	}

	public static void Update()
	{
		if (!IsEnabled || AutoLogin.IsDyTest)
		{
			return;
		}
		DismissGiftDialogIfAny();
		if (GameCanvas.currentScreen != GameScr.instance || Char.myCharz() == null)
		{
			return;
		}
		long num = mSystem.currentTimeMillis();
		if (num - _lastTickTime < 100)
		{
			return;
		}
		_lastTickTime = num;
		if (Char.ischangingMap || Char.isLoadingMap || InfoDlg.isShow || GameCanvas.isLoading)
		{
			if (_lastMapChangeRequestTime > 0 && num - _lastMapChangeRequestTime > 3000)
			{
				Char.ischangingMap = false;
				Char.isLoadingMap = false;
				Char.isLockKey = false;
				if (InfoDlg.isShow)
				{
					InfoDlg.hide();
				}
				_lastMapChangeRequestTime = 0L;
			}
			else
			{
				return;
			}
		}
		else
		{
			_lastMapChangeRequestTime = 0L;
		}
		Char obj = Char.myCharz();
		DismissInGameDialogs();
		AutoCombat.EnsureDefaultSkill(obj);
		if (obj.cPower >= TARGET_POWER)
		{
			CurrentStatus = "hoàn thành";
			IsEnabled = false;
			if (!_hasShownCompleted)
			{
				_hasShownCompleted = true;
				GameCanvas.startOKDlg("Hoàn thành! Nhân vật đã đạt " + Res.formatNumber(obj.cPower) + " sức mạnh.");
				GameScr.info1.addInfo("Hoàn thành! Đạt 1.300.000 sức mạnh", 0);
			}
			SocketClient.ForceSync();
			return;
		}
		if (obj.cHP <= 0)
		{
			CurrentStatus = "đang hồi sinh về nhà";
			AutoQuest.IsReturningHomeFromDeath = true;
			Service.gI().returnTownFromDead();
			return;
		}
		int mapID = TileMap.mapID;
		int num2 = ((obj.taskMaint != null) ? obj.taskMaint.taskId : 0);
		int taskIndex = ((obj.taskMaint != null) ? obj.taskMaint.index : 0);
		CheckInactiveTimeout(obj, mapID, num2, taskIndex, num);
		if (num2 >= 3 && !AutoQuest.IsMpUpgraded(obj))
		{
			AutoQuest.EnsureMpUpgradedOnce(obj);
		}
		if ((mapID == 1 || mapID == 123 || mapID == 21) && ItemPicker.TryPickItem(obj, num))
		{
			return;
		}
		if (IsCombatMap(mapID))
		{
			bool flag = obj.cHPFull > 0 && obj.cHP * 100 / obj.cHPFull < 15;
			bool flag2 = obj.cMPFull > 0 && obj.cMP * 100 / obj.cMPFull < 15;
			if ((flag || flag2) && AutoQuest.GetMagicPeaCount(obj) > 0 && num - _lastPeaTime > 1500)
			{
				_lastPeaTime = num;
				AutoQuest.UseMagicPeaOnly(obj);
			}
		}
		switch (mapID)
		{
		case 39:
			AutoQuest.HandleVachNuiAru(obj, num);
			break;
		case 21:
			AutoQuest.HandleNhaGohan(obj, num2, taskIndex, num);
			break;
		case 0:
			AutoQuest.ResetHomeHarvestFlag();
			AutoQuest.HandleLangAru(obj, num2, taskIndex, num);
			break;
		case 1:
			AutoQuest.HandleDoiHoaCuc(obj, num2, taskIndex, num);
			break;
		case 123:
			AutoQuest.ResetHomeHarvestFlag();
			CurrentStatus = "đang đánh quái";
			AutoCombat.PatrolAndAttackNguHanhSon(obj, num);
			break;
		default:
			CurrentStatus = "đang ra map đánh quái";
			break;
		}
	}

	private static bool IsCombatMap(int mapId)
	{
		return mapId == 123 || mapId == 1;
	}

	public static void DismissInGameDialogs()
	{
		try
		{
			DismissGiftDialogIfAny();
			if (ChatPopup.currChatPopup != null)
			{
				ChatPopup.currChatPopup.perform(8000, null);
			}
			if (ChatPopup.serverChatPopUp != null)
			{
				ChatPopup.serverChatPopUp = null;
				Char.chatPopup = null;
			}
			if (GameCanvas.currentDialog != null && (GameCanvas.currentDialog != GameCanvas.msgdlg || !GameCanvas.msgdlg.isWait))
			{
				try
				{
					if (GameCanvas.currentDialog.center != null)
					{
						GameCanvas.currentDialog.center.performAction();
					}
					else if (GameCanvas.currentDialog.left != null)
					{
						GameCanvas.currentDialog.left.performAction();
					}
				}
				catch
				{
				}
				GameCanvas.endDlg();
			}
			if (InfoDlg.isShow)
			{
				InfoDlg.hide();
			}
			Char.isLockKey = false;
		}
		catch
		{
		}
	}

	public static bool DismissGiftDialogIfAny()
	{
		bool result = false;
		try
		{
			if (GameCanvas.currentScreen is ClientInput || ClientInput.instance != null)
			{
				ClientInput clientInput = (GameCanvas.currentScreen as ClientInput) ?? ClientInput.instance;
				if (clientInput != null)
				{
					if (clientInput.left != null)
					{
						clientInput.left.performAction();
					}
					else
					{
						clientInput.perform(1, null);
					}
					clientInput.clearScreen();
				}
				if (GameScr.instance != null && GameCanvas.currentScreen != GameScr.instance)
				{
					GameScr.instance.switchToMe();
				}
				GameCanvas.endDlg();
				result = true;
			}
			if (GameCanvas.currentDialog is InputDlg inputDlg)
			{
				if (inputDlg.left != null)
				{
					inputDlg.left.performAction();
				}
				inputDlg.hide();
				GameCanvas.endDlg();
				result = true;
			}
			if (GameCanvas.currentDialog != null)
			{
				try
				{
					if (GameCanvas.currentDialog.left != null)
					{
						GameCanvas.currentDialog.left.performAction();
					}
					else if (GameCanvas.currentDialog.center != null)
					{
						GameCanvas.currentDialog.center.performAction();
					}
				}
				catch
				{
				}
				GameCanvas.endDlg();
				result = true;
			}
			if (InfoDlg.isShow)
			{
				InfoDlg.hide();
				result = true;
			}
			if (GameCanvas.menu != null && GameCanvas.menu.showMenu && GameCanvas.menu.menuItems != null)
			{
				for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
				{
					Command command = (Command)GameCanvas.menu.menuItems.elementAt(i);
					if (command != null && command.caption != null)
					{
						string text = command.caption.ToLower();
						if (text.Contains("từ chối") || text.Contains("tu choi") || text.Contains("đóng") || text.Contains("dong") || text.Contains("bỏ qua") || text.Contains("bo qua"))
						{
							command.performAction();
							GameCanvas.menu.doCloseMenu();
							result = true;
							break;
						}
					}
				}
			}
		}
		catch
		{
		}
		return result;
	}

	private static void CheckInactiveTimeout(Char obj, int mapID, int taskId, int taskIndex, long now)
	{
		if (_lastProgressTime == 0L)
		{
			_lastProgressTime = now;
			_lastPower = obj.cPower;
			_lastTiemNang = obj.cTiemNang;
			_lastProgressMapId = mapID;
			_lastProgressTaskId = taskId;
			_lastProgressTaskIndex = taskIndex;
			return;
		}

		// 1. Tăng sức mạnh hoặc tiềm năng -> Reset watchdog
		if (obj.cPower > _lastPower || obj.cTiemNang > _lastTiemNang)
		{
			_lastPower = obj.cPower;
			_lastTiemNang = obj.cTiemNang;
			_lastProgressTime = now;
		}

		// 2. Chuyển map hoặc bước nhiệm vụ thay đổi -> Reset watchdog
		if (mapID != _lastProgressMapId || taskId != _lastProgressTaskId || taskIndex != _lastProgressTaskIndex)
		{
			_lastProgressMapId = mapID;
			_lastProgressTaskId = taskId;
			_lastProgressTaskIndex = taskIndex;
			_lastProgressTime = now;
		}

		// 3. Quá 60s không tăng sức mạnh hoặc tiềm năng -> Tự động đóng tab
		if (now - _lastProgressTime > INACTIVE_TIMEOUT_MS)
		{
			TriggerInactiveTimeout(obj);
		}
	}

	public static void TriggerInactiveTimeout(Char obj)
	{
		try
		{
			CurrentStatus = "tự tắt do không tăng SM/TN (>60s)";
			SocketClient.Send("STUCK_TIMEOUT|" + AutoLogin.IdClientSocket + "|" + obj.cPower + "|" + obj.cTiemNang);
			Thread.Sleep(200);
		}
		catch
		{
		}
		try
		{
			Process.GetCurrentProcess().Kill();
		}
		catch
		{
			Application.Quit();
		}
	}
}
