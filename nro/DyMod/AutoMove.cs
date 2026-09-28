using System;

namespace DyMod;

public static class AutoMove
{
	public static readonly int[,] NguHanhSonZones = new int[4, 2]
	{
		{ 400, 384 },
		{ 550, 408 },
		{ 750, 408 },
		{ 930, 360 }
	};

	private static int _lastCheckX = -999;

	private static long _lastStuckCheckTime = 0L;

	private static int _stuckCount = 0;

	private static long _lastChangeMapTime = 0L;

	public static void MoveTowards(int targetX, int targetY)
	{
		Char obj = Char.myCharz();
		if (obj == null)
		{
			return;
		}
		int terrainY = GetTerrainY(TileMap.mapID, obj.cx, targetX, targetY);
		int num = System.Math.Abs(obj.cx - targetX);
		int num2 = System.Math.Abs(obj.cy - terrainY);
		if (num <= 16 && num2 <= 20)
		{
			obj.currentMovePoint = null;
			return;
		}
		obj.isLockMove = false;
		Char.isLockKey = false;
		if (obj.statusMe == 3 || obj.statusMe == 4)
		{
			obj.statusMe = 1;
		}
		if (obj.currentMovePoint == null || System.Math.Abs(obj.currentMovePoint.xEnd - targetX) > 16 || System.Math.Abs(obj.currentMovePoint.yEnd - terrainY) > 20)
		{
			obj.currentMovePoint = new MovePoint(targetX, terrainY);
			obj.endMovePointCommand = null;
		}
		obj.cdir = ((obj.cx < targetX) ? 1 : (-1));
		long num3 = mSystem.currentTimeMillis();
		if (num3 - _lastStuckCheckTime <= 400)
		{
			return;
		}
		_lastStuckCheckTime = num3;
		if (System.Math.Abs(obj.cx - _lastCheckX) < 4)
		{
			_stuckCount++;
			if (_stuckCount >= 3)
			{
				int mapW = (TileMap.pxw > 0) ? TileMap.pxw : 1200;
				int jumpDir = obj.cdir;
				// Nếu ở sát rìa trái, bắt buộc nhảy sang phải (hướng vào trong map)
				if (obj.cx < 70)
				{
					jumpDir = 1;
				}
				// Nếu ở sát rìa phải, bắt buộc nhảy sang trái (hướng vào trong map)
				else if (obj.cx > mapW - 70)
				{
					jumpDir = -1;
				}

				obj.statusMe = 10;
				obj.cvy = -8;
				obj.cvx = (obj.cspeed + 2) * jumpDir;
				obj.cdir = jumpDir;
				_stuckCount = 0;
			}
		}
		else
		{
			_stuckCount = 0;
			_lastCheckX = obj.cx;
		}
	}

	public static bool TryClickWaypointPopup(Waypoint targetWp = null)
	{
		try
		{
			if (targetWp != null && targetWp.popup != null && targetWp.popup.command != null)
			{
				targetWp.popup.command.performAction();
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	public static void ChangeMapThroughWaypoint(bool toRight, int fallbackX, int fallbackY, long now)
	{
		Char obj = Char.myCharz();
		if (obj == null)
		{
			return;
		}
		Waypoint waypoint = null;
		if (TileMap.vGo != null && TileMap.vGo.size() > 0)
		{
			int num = (toRight ? (-1) : 999999);
			for (int i = 0; i < TileMap.vGo.size(); i++)
			{
				Waypoint waypoint2 = (Waypoint)TileMap.vGo.elementAt(i);
				if (waypoint2 != null)
				{
					int num2 = (waypoint2.minX + waypoint2.maxX) / 2;
					if (toRight && num2 > num)
					{
						num = num2;
						waypoint = waypoint2;
					}
					else if (!toRight && num2 < num)
					{
						num = num2;
						waypoint = waypoint2;
					}
				}
			}
		}
		int targetX = ((waypoint != null) ? ((waypoint.minX + waypoint.maxX) / 2) : fallbackX);
		int targetY = ((int?)waypoint?.maxY) ?? fallbackY;
		bool flag = false;
		if (!((waypoint == null) ? (toRight ? (obj.cx >= fallbackX - 10) : (obj.cx <= fallbackX + 10)) : (obj.cx >= waypoint.minX && obj.cx <= waypoint.maxX)))
		{
			MoveTowards(targetX, targetY);
			return;
		}
		obj.currentMovePoint = null;
		if (!Char.ischangingMap && !Char.isLoadingMap && !InfoDlg.isShow && !GameCanvas.isLoading && now - _lastChangeMapTime > 2500)
		{
			_lastChangeMapTime = now;
			AutoTrain.NotifyMapChangeRequested(now);
			Service.gI().charMove();
			InfoDlg.showWait();
			if ((waypoint != null && waypoint.isOffline) || TileMap.isTrainingMap() || TileMap.isOfflineMap())
			{
				Service.gI().getMapOffline();
			}
			else
			{
				Service.gI().requestChangeMap();
			}
			Char.ischangingMap = true;
			Char.isLockKey = true;
		}
	}

	public static int GetTerrainY(int mapId, int currentX, int targetX, int defaultY)
	{
		if (defaultY > 0)
		{
			return defaultY;
		}
		switch (mapId)
		{
		case 39:
			return 384;
		case 21:
			return 336;
		case 0:
			return 432;
		case 1:
			return (targetX < 450) ? 384 : 408;
		case 42:
			if (targetX <= 700)
			{
				return 408;
			}
			if (targetX <= 880)
			{
				return 432;
			}
			if (targetX <= 1120)
			{
				return 408;
			}
			return 432;
		case 123:
			if (targetX <= 460)
			{
				return 384;
			}
			if (targetX <= 880)
			{
				return 408;
			}
			return 360;
		default:
			return (defaultY > 0) ? defaultY : 384;
		}
	}
}
