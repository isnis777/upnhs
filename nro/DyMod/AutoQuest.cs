using System;

namespace DyMod;

public static class AutoQuest
{
	public static bool HasVisitedShop = true;

	public static bool NeedRestockAfterDeath = false;

	public static int RestockStep = 0;

	public static bool HasUpgradedMp = false;

	private static long _lastNpcInteractTime = 0L;

	private static long _lastPeaHarvestTime = 0L;

	private static int _targetMocNhanX = 680;

	private static long _lastEnterHouseTime = 0L;

	public static bool IsReturningHomeFromDeath = false;

	private static bool _hasHarvestedThisVisit = false;

	private static long _lastPeaUseTime = 0L;

	public static void ResetHomeHarvestFlag()
	{
		_hasHarvestedThisVisit = false;
	}

	public static bool IsMpUpgraded(Char myChar)
	{
		if (HasUpgradedMp)
		{
			return true;
		}
		if (myChar == null)
		{
			return false;
		}
		int baseMp = ((myChar.cgender == 1) ? 200 : 100);
		if (myChar.cMPGoc > baseMp)
		{
			HasUpgradedMp = true;
			return true;
		}
		return false;
	}

	public static void EnsureMpUpgradedOnce(Char myChar)
	{
		if (!IsMpUpgraded(myChar))
		{
			Service.gI().upPotential(1, 1);
			HasUpgradedMp = true;
			AutoTrain.CurrentStatus = "cộng 1 điểm tiềm năng vào MP gốc";
		}
	}

	public static void HandleVachNuiAru(Char myChar, long now)
	{
		AutoTrain.CurrentStatus = "đang vào Nhà Gôhan";
		AutoMove.ChangeMapThroughWaypoint(toRight: true, 755, 384, now);
	}

	private static bool HandleActiveMenuIfAny()
	{
		try
		{
			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				if (GameCanvas.menu.menuItems != null && GameCanvas.menu.menuItems.size() > 0)
				{
					GameCanvas.menu.menuSelectedItem = 0;
					((Command)GameCanvas.menu.menuItems.elementAt(0))?.performAction();
				}
				GameCanvas.menu.doCloseMenu();
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	public static void HandleNhaGohan(Char myChar, int taskId, int taskIndex, long now)
	{
		AutoTrain.DismissGiftDialogIfAny();

		// Task 0: Lấy rada trong rương, thu hoạch đậu, nói chuyện Ông Gôhan nhận NV1
		if (taskId == 0)
		{
			AutoTrain.CurrentStatus = "đang làm nhiệm vụ 1";
			if (ChatPopup.currChatPopup != null)
			{
				if (ChatPopup.currChatPopup.cmdNextLine != null)
				{
					ChatPopup.currChatPopup.cmdNextLine.performAction();
				}
				ChatPopup.currChatPopup.perform(8000, null);
				ChatPopup.currChatPopup = null;
			}
			if (ChatPopup.serverChatPopUp != null)
			{
				ChatPopup.serverChatPopUp.perform(1001, null);
				ChatPopup.serverChatPopUp = null;
			}
			Char.chatPopup = null;
			Char.isLockKey = false;

			if (taskIndex <= 2)
			{
				if (GameCanvas.panel != null && GameCanvas.panel.isShow)
				{
					GameCanvas.panel.isShow = false;
				}
				if (HandleActiveMenuIfAny())
				{
					return;
				}
				if (GameCanvas.currentDialog != null)
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
				if (System.Math.Abs(myChar.cx - 235) > 30)
				{
					AutoMove.MoveTowards(235, 336);
				}
				else if (now - _lastNpcInteractTime > 1200)
				{
					_lastNpcInteractTime = now;
					Npc npc = FindNpcNear(235);
					int npcId = npc?.template.npcTemplateId ?? 0;
					myChar.npcFocus = npc;
					Service.gI().openMenu(npcId);
					GameCanvas.endDlg();
				}
				return;
			}
			if (taskIndex == 3)
			{
				if (GameCanvas.currentDialog != null)
				{
					try
					{
						if (GameCanvas.currentDialog.center != null)
						{
							GameCanvas.currentDialog.center.performAction();
						}
					}
					catch
					{
					}
					GameCanvas.endDlg();
				}
				if (System.Math.Abs(myChar.cx - 90) > 30)
				{
					AutoMove.MoveTowards(90, 336);
				}
				else
				{
					if (now - _lastNpcInteractTime <= 1000)
					{
						return;
					}
					_lastNpcInteractTime = now;
					Npc npc2 = FindNpcNear(90);
					int npcId2 = npc2?.template.npcTemplateId ?? 3;
					myChar.npcFocus = npc2;
					Service.gI().openMenu(npcId2);
					if (myChar.arrItemBox != null)
					{
						for (int i = 0; i < myChar.arrItemBox.Length; i++)
						{
							if (myChar.arrItemBox[i] != null)
							{
								Service.gI().getItem(0, (sbyte)i);
							}
						}
					}
					Service.gI().getItem(0, 0);
					Service.gI().getItem(0, 1);
					if (GameCanvas.panel != null && GameCanvas.panel.isShow)
					{
						GameCanvas.panel.isShow = false;
					}
					GameCanvas.endDlg();
				}
				return;
			}
			if (taskIndex == 4)
			{
				if (GameCanvas.panel != null && GameCanvas.panel.isShow)
				{
					GameCanvas.panel.isShow = false;
				}
				if (GameCanvas.currentDialog != null)
				{
					GameCanvas.endDlg();
				}
				if (HandleActiveMenuIfAny())
				{
					return;
				}
				if (System.Math.Abs(myChar.cx - 325) > 30)
				{
					AutoMove.MoveTowards(325, 336);
				}
				else if (now - _lastNpcInteractTime > 1200)
				{
					_lastNpcInteractTime = now;
					Npc npc3 = FindNpcNear(325);
					int npcId3 = npc3?.template.npcTemplateId ?? 4;
					myChar.npcFocus = npc3;
					Service.gI().openMenu(npcId3);
					Service.gI().menu(npcId3, 0, 0);
					if (GameScr.gI().magicTree != null && GameScr.gI().magicTree.currPeas > 0)
					{
						Service.gI().magicTree(1);
					}
					GameCanvas.endDlg();
				}
				return;
			}
			if (taskIndex >= 5)
			{
				if (GameCanvas.panel != null && GameCanvas.panel.isShow)
				{
					GameCanvas.panel.isShow = false;
				}
				if (GameCanvas.currentDialog != null)
				{
					GameCanvas.endDlg();
				}
				if (!HandleActiveMenuIfAny())
				{
					if (System.Math.Abs(myChar.cx - 235) > 30)
					{
						AutoMove.MoveTowards(235, 336);
					}
					else if (now - _lastNpcInteractTime > 1200)
					{
						_lastNpcInteractTime = now;
						Npc npc4 = FindNpcNear(235);
						int npcId4 = npc4?.template.npcTemplateId ?? 0;
						myChar.npcFocus = npc4;
						Service.gI().openMenu(npcId4);
						Service.gI().selectSkill(0);
						GameCanvas.endDlg();
					}
				}
				return;
			}
		}

		// Task 1: Báo cáo trả nhiệm vụ 1 (Đánh 5 Mộc nhân) cho Ông Gôhan
		if (taskId == 1)
		{
			if (IsTaskReadyToReport(myChar))
			{
				AutoTrain.CurrentStatus = "về gặp Ông Gôhan trả nhiệm vụ 1";
				if (HandleActiveMenuIfAny())
				{
					return;
				}
				if (GameCanvas.currentDialog != null)
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
				if (ChatPopup.currChatPopup != null)
				{
					if (ChatPopup.currChatPopup.cmdNextLine != null)
					{
						ChatPopup.currChatPopup.cmdNextLine.performAction();
					}
					ChatPopup.currChatPopup.perform(8000, null);
					ChatPopup.currChatPopup = null;
				}
				if (System.Math.Abs(myChar.cx - 235) > 30)
				{
					AutoMove.MoveTowards(235, 336);
				}
				else if (now - _lastNpcInteractTime > 1200)
				{
					_lastNpcInteractTime = now;
					Npc npc5 = FindNpcNear(235);
					int num = npc5?.template.npcTemplateId ?? 0;
					myChar.npcFocus = npc5;
					Service.gI().openMenu(num);
					Service.gI().confirmMenu((short)num, 0);
				}
				return;
			}
			else
			{
				// Chưa xong 5 Mộc nhân -> ra Làng Aru đánh tiếp
				AutoTrain.CurrentStatus = "đang ra Làng Aru đánh Mộc Nhân";
				AutoMove.ChangeMapThroughWaypoint(toRight: true, 484, 336, now);
				return;
			}
		}

		// Task 2: Báo cáo trả nhiệm vụ 2 (10 đùi gà) cho Ông Gôhan
		if (taskId == 2)
		{
			if (IsTaskReadyToReport(myChar))
			{
				AutoTrain.CurrentStatus = "về gặp Ông Gôhan trả nhiệm vụ 2";
				if (HandleActiveMenuIfAny())
				{
					return;
				}
				if (GameCanvas.currentDialog != null)
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
				if (ChatPopup.currChatPopup != null)
				{
					if (ChatPopup.currChatPopup.cmdNextLine != null)
					{
						ChatPopup.currChatPopup.cmdNextLine.performAction();
					}
					ChatPopup.currChatPopup.perform(8000, null);
					ChatPopup.currChatPopup = null;
				}
				if (System.Math.Abs(myChar.cx - 235) > 30)
				{
					AutoMove.MoveTowards(235, 336);
				}
				else if (now - _lastNpcInteractTime > 1200)
				{
					_lastNpcInteractTime = now;
					Npc npc7 = FindNpcNear(235);
					int num2 = npc7?.template.npcTemplateId ?? 0;
					myChar.npcFocus = npc7;
					Service.gI().openMenu(num2);
					Service.gI().confirmMenu((short)num2, 0);
				}
				return;
			}
			else
			{
				// Chưa đủ 10 đùi gà -> ra Làng Aru để sang Đồi hoa cúc
				AutoTrain.CurrentStatus = "đang ra Đồi hoa cúc nhặt đùi gà";
				AutoMove.ChangeMapThroughWaypoint(toRight: true, 484, 336, now);
				return;
			}
		}

		// Task >= 3: Đã hoàn thành Task 1 và Task 2!
		// Theo đúng video và yêu cầu: khi vào Nhà Gôhan (kể cả sau khi chết hồi sinh về nhà):
		// Thu hoạch đậu thần, hồi phục đầy máu bằng ĐẬU THẦN (tuyệt đối KHÔNG ăn đùi gà), rồi ra Làng Aru gặp Đường Tăng vào Ngũ Hành Sơn
		if (taskId >= 3)
		{
			EnsureMpUpgradedOnce(myChar);

			bool needHarvest = !_hasHarvestedThisVisit || (GameScr.gI().magicTree != null && GameScr.gI().magicTree.currPeas > 0);
			if (IsReturningHomeFromDeath || myChar.cHP < myChar.cHPFull || needHarvest)
			{
				AutoTrain.CurrentStatus = "về nhà thu hoạch đậu thần";

				// 1. Nếu chưa tới gần cây đậu thần (x ~ 325) -> di chuyển lại gần
				if (System.Math.Abs(myChar.cx - 325) > 30 && !_hasHarvestedThisVisit)
				{
					AutoMove.MoveTowards(325, 336);
					return;
				}

				// 2. Thu hoạch đậu thần
				if (!_hasHarvestedThisVisit && now - _lastPeaHarvestTime > 1500)
				{
					_lastPeaHarvestTime = now;
					_hasHarvestedThisVisit = true;
					Npc npcTree = FindNpcNear(325);
					int npcTreeId = npcTree?.template.npcTemplateId ?? 4;
					myChar.npcFocus = npcTree;
					Service.gI().openMenu(npcTreeId);
					Service.gI().magicTree(1);
					Service.gI().menu(npcTreeId, 0, 0);
					GameCanvas.endDlg();
					return;
				}

				// 3. Nếu máu chưa đầy, dùng ĐẬU THẦN (tuyệt đối không ăn đùi gà)
				if (myChar.cHP < myChar.cHPFull)
				{
					if (GetMagicPeaCount(myChar) > 0 && now - _lastPeaUseTime > 1000)
					{
						_lastPeaUseTime = now;
						UseMagicPeaOnly(myChar);
						return;
					}
					// Chờ nhặt đậu nếu vừa thu hoạch xong trong vòng 2s
					if (now - _lastPeaHarvestTime < 2000)
					{
						return;
					}
				}
			}

			IsReturningHomeFromDeath = false;
			AutoTrain.CurrentStatus = "đang ra Làng Aru tìm Đường Tăng vào Ngũ Hành Sơn";
			AutoMove.ChangeMapThroughWaypoint(toRight: true, 484, 336, now);
			return;
		}

		AutoTrain.CurrentStatus = "đang trên đường ra map đánh quái";
		AutoMove.ChangeMapThroughWaypoint(toRight: true, 484, 336, now);
	}

	public static void HandleLangAru(Char myChar, int taskId, int taskIndex, long now)
	{
		if (taskId == 0)
		{
			EnterNhaGohanFromLangAru(myChar, now);
			return;
		}

		if (taskId == 1)
		{
			if (!IsTaskReadyToReport(myChar))
			{
				int num = ((myChar.taskMaint != null) ? myChar.taskMaint.count : 0);
				AutoTrain.CurrentStatus = $"đang đánh Mộc Nhân ({num}/5)";
				Mob mob = AutoCombat.FindMocNhan();
				if (mob != null)
				{
					_targetMocNhanX = mob.x;
					AutoCombat.AttackMob(myChar, mob, now);
				}
				else
				{
					_targetMocNhanX = ((_targetMocNhanX == 680) ? 760 : 680);
					AutoMove.MoveTowards(_targetMocNhanX, 432);
				}
			}
			else
			{
				AutoTrain.CurrentStatus = "đã xong 5 Mộc Nhân -> về Nhà Gôhan trả NV";
				EnterNhaGohanFromLangAru(myChar, now);
			}
			return;
		}

		if (taskId == 2)
		{
			if (!IsTaskReadyToReport(myChar))
			{
				int duiGaCount = GetDuiGaCount(myChar);
				int num2 = ((myChar.taskMaint != null) ? myChar.taskMaint.count : duiGaCount);
				AutoTrain.CurrentStatus = $"đang sang Đồi hoa cúc ({num2}/10 đùi gà)";
				AutoMove.ChangeMapThroughWaypoint(toRight: true, 1228, 432, now);
			}
			else
			{
				AutoTrain.CurrentStatus = "đã đủ 10 đùi gà -> về Nhà Gôhan trả NV";
				EnterNhaGohanFromLangAru(myChar, now);
			}
			return;
		}

		// taskId >= 3: Đã làm xong NV 1 và NV 2!
		// Đi thẳng tới Đường Tăng tại tọa độ x=825, y=432 để vào Ngũ Hành Sơn (Map 123)
		EnsureMpUpgradedOnce(myChar);
		AutoTrain.CurrentStatus = "đang gặp Đường Tăng vào Ngũ Hành Sơn";
		if (System.Math.Abs(myChar.cx - 825) > 30)
		{
			AutoMove.MoveTowards(825, 432);
			return;
		}
		Npc npc = FindNpcNear(825);
		if (npc == null)
		{
			return;
		}
		myChar.npcFocus = npc;
		if (GameCanvas.menu.showMenu)
		{
			if (GameCanvas.menu.menuItems != null && GameCanvas.menu.menuItems.size() > 0)
			{
				((Command)GameCanvas.menu.menuItems.elementAt(0))?.performAction();
			}
			Service.gI().confirmMenu((short)npc.template.npcTemplateId, 0);
			GameCanvas.menu.doCloseMenu();
		}
		else if (now - _lastNpcInteractTime > 1200)
		{
			_lastNpcInteractTime = now;
			Service.gI().openMenu(npc.template.npcTemplateId);
			Service.gI().confirmMenu((short)npc.template.npcTemplateId, 0);
		}
	}

	public static void HandleDoiHoaCuc(Char myChar, int taskId, int taskIndex, long now)
	{
		if (taskId == 2 && !IsTaskReadyToReport(myChar))
		{
			int duiGaCount = GetDuiGaCount(myChar);
			int num = ((myChar.taskMaint != null) ? myChar.taskMaint.count : duiGaCount);
			AutoTrain.CurrentStatus = $"đang săn quái nhặt đùi gà ({num}/10)";
			Mob mob = AutoCombat.FindKhungLong();
			if (mob != null)
			{
				AutoCombat.AttackMob(myChar, mob, now);
			}
			else if (myChar.cx < 440)
			{
				AutoMove.MoveTowards(545, 408);
			}
			else
			{
				AutoMove.MoveTowards(340, 384);
			}
		}
		else
		{
			AutoTrain.CurrentStatus = "đã đủ 10 đùi gà -> về Làng Aru trả NV";
			AutoMove.ChangeMapThroughWaypoint(toRight: false, 20, 384, now);
		}
	}

	public static void HandleVachNuiAruNgoai(Char myChar, long now)
	{
		AutoTrain.CurrentStatus = "đang về Làng Aru";
		AutoMove.ChangeMapThroughWaypoint(toRight: true, 1425, 432, now);
	}

	public static bool HasRadar(Char myChar)
	{
		if (myChar == null)
		{
			return false;
		}
		if (myChar.arrItemBag != null)
		{
			for (int i = 0; i < myChar.arrItemBag.Length; i++)
			{
				Item item = myChar.arrItemBag[i];
				if (item != null && item.template != null)
				{
					string text = ((item.template.name != null) ? item.template.name.ToLower() : "");
					if (text.Contains("ra-đa") || text.Contains("rada") || text.Contains("ra đa") || item.template.id == 12 || item.template.type == 4)
					{
						return true;
					}
				}
			}
		}
		if (myChar.arrItemBody != null)
		{
			for (int j = 0; j < myChar.arrItemBody.Length; j++)
			{
				Item item2 = myChar.arrItemBody[j];
				if (item2 != null && item2.template != null)
				{
					string text2 = ((item2.template.name != null) ? item2.template.name.ToLower() : "");
					if (text2.Contains("ra-đa") || text2.Contains("rada") || text2.Contains("ra đa") || item2.template.id == 12 || item2.template.type == 4)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool HasRadarInBag(Char myChar)
	{
		return HasRadar(myChar);
	}

	public static int GetMagicPeaCount(Char myChar)
	{
		if (myChar == null || myChar.arrItemBag == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < myChar.arrItemBag.Length; i++)
		{
			Item item = myChar.arrItemBag[i];
			if (item != null && item.template != null && item.template.type == 6)
			{
				string text = ((item.template.name != null) ? item.template.name.ToLower() : "");
				// Tuyệt đối không tính đùi gà
				if (item.template.id == 73 || text.Contains("đùi gà") || text.Contains("dui ga") || text.Contains("gà"))
				{
					continue;
				}
				num += ((item.quantity <= 0) ? 1 : item.quantity);
			}
		}
		return num;
	}

	public static bool UseMagicPeaOnly(Char myChar)
	{
		if (myChar == null || myChar.arrItemBag == null)
		{
			return false;
		}
		for (int i = 0; i < myChar.arrItemBag.Length; i++)
		{
			Item item = myChar.arrItemBag[i];
			if (item != null && item.template != null && item.template.type == 6)
			{
				string text = ((item.template.name != null) ? item.template.name.ToLower() : "");
				// Tuyệt đối không ăn đùi gà
				if (item.template.id == 73 || text.Contains("đùi gà") || text.Contains("dui ga") || text.Contains("gà"))
				{
					continue;
				}
				Service.gI().useItem(0, 1, -1, item.template.id);
				return true;
			}
		}
		return false;
	}

	public static int GetDuiGaCount(Char myChar)
	{
		if (myChar == null || myChar.arrItemBag == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < myChar.arrItemBag.Length; i++)
		{
			Item item = myChar.arrItemBag[i];
			if (item != null && item.template != null)
			{
				string text = ((item.template.name != null) ? item.template.name.ToLower() : "");
				if (text.Contains("đùi gà") || text.Contains("dui ga") || text.Contains("gà nướng") || item.template.id == 73)
				{
					num += ((item.quantity <= 0) ? 1 : item.quantity);
				}
			}
		}
		return num;
	}

	public static void EnterNhaGohanFromLangAru(Char myChar, long now)
	{
		Waypoint waypoint = null;
		if (TileMap.vGo != null)
		{
			for (int i = 0; i < TileMap.vGo.size(); i++)
			{
				Waypoint waypoint2 = (Waypoint)TileMap.vGo.elementAt(i);
				if (waypoint2 != null)
				{
					int midX = (waypoint2.minX + waypoint2.maxX) / 2;
					// Cửa Nhà Gôhan trong Làng Aru nằm ở x ~ 330-380, isOffline = true
					if (waypoint2.isOffline || (midX >= 280 && midX <= 420))
					{
						waypoint = waypoint2;
						break;
					}
				}
			}
		}
		int targetX = ((waypoint != null) ? ((waypoint.minX + waypoint.maxX) / 2) : 355);
		int targetY = waypoint?.maxY ?? 432;
		if (System.Math.Abs(myChar.cx - targetX) > 20)
		{
			AutoMove.MoveTowards(targetX, targetY);
			return;
		}
		myChar.currentMovePoint = null;
		if (now - _lastEnterHouseTime > 2000)
		{
			_lastEnterHouseTime = now;
			AutoTrain.NotifyMapChangeRequested(now);
			Service.gI().charMove();
			if (waypoint != null && waypoint.popup != null && waypoint.popup.command != null)
			{
				waypoint.popup.command.performAction();
			}
			else if (waypoint != null)
			{
				waypoint.perform(2, null);
			}
			else
			{
				InfoDlg.showWait();
				Service.gI().getMapOffline();
				Char.ischangingMap = true;
			}
		}
	}

	public static bool IsTaskReadyToReport(Char myChar)
	{
		if (myChar == null || myChar.taskMaint == null)
		{
			return false;
		}
		Task taskMaint = myChar.taskMaint;

		// Task 1: Đánh 5 Mộc Nhân
		if (taskMaint.taskId == 1)
		{
			if (taskMaint.count >= 5)
			{
				return true;
			}
			if (taskMaint.counts != null && taskMaint.counts.Length > 0 && taskMaint.counts[0] > 0 && taskMaint.count >= taskMaint.counts[0])
			{
				return true;
			}
			if (taskMaint.index >= 1)
			{
				return true;
			}
			return false;
		}

		// Task 2: Săn 10 Đùi gà
		if (taskMaint.taskId == 2)
		{
			if (taskMaint.count >= 10 || GetDuiGaCount(myChar) >= 10)
			{
				return true;
			}
			if (taskMaint.counts != null && taskMaint.counts.Length > 0 && taskMaint.counts[0] > 0 && taskMaint.count >= taskMaint.counts[0])
			{
				return true;
			}
			if (taskMaint.index >= 1)
			{
				return true;
			}
			return false;
		}

		// Task >= 3: Đã xong các nhiệm vụ trước khi vào Ngũ Hành Sơn
		if (taskMaint.taskId >= 3)
		{
			return true;
		}

		if (taskMaint.counts != null && taskMaint.index < taskMaint.counts.Length && taskMaint.counts[taskMaint.index] > 0 && taskMaint.count >= taskMaint.counts[taskMaint.index])
		{
			return true;
		}
		if (taskMaint.index >= 1)
		{
			return true;
		}
		return false;
	}

	public static Npc FindNpcNear(int targetX)
	{
		if (GameScr.vNpc == null)
		{
			return null;
		}
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc != null && System.Math.Abs(npc.cx - targetX) < 90)
			{
				return npc;
			}
		}
		return null;
	}
}
