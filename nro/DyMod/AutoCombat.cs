using System;

namespace DyMod;

public static class AutoCombat
{
	private static long _lastAttackTime;

	private static int _nguHanhSonPatrolIndex;

	private static long _lastPatrolTime;

	private static int _currentAttackingMobId = -1;

	private static long _attackMobStartTime = 0L;

	private static int _unreachableMobId = -1;

	private static long _unreachableUntilTime = 0L;

	public static void EnsureDefaultSkill(Char myChar)
	{
		if (myChar == null)
		{
			return;
		}
		try
		{
			if (myChar.myskill == null)
			{
				if (myChar.vSkillFight != null && myChar.vSkillFight.size() > 0)
				{
					myChar.myskill = (Skill)myChar.vSkillFight.elementAt(0);
				}
				else if (myChar.vSkill != null && myChar.vSkill.size() > 0)
				{
					myChar.myskill = (Skill)myChar.vSkill.elementAt(0);
				}
				if (myChar.myskill != null && myChar.myskill.template != null)
				{
					Service.gI().selectSkill(myChar.myskill.template.id);
				}
			}
		}
		catch
		{
		}
	}

	public static void AttackMob(Char myChar, Mob mob, long now)
	{
		if (myChar == null || mob == null)
		{
			return;
		}
		myChar.mobFocus = mob;
		int mapW = (TileMap.pxw > 0) ? TileMap.pxw : 1200;

		// 1. Kiểm tra mob có bị kẹt / không thể với tới quá lâu không
		if (mob.mobId != _currentAttackingMobId)
		{
			_currentAttackingMobId = mob.mobId;
			_attackMobStartTime = now;
		}
		else if (now - _attackMobStartTime > 3200)
		{
			// Đã cố tiếp cận con quái này hơn 3.2s mà không đánh được (kẹt rìa map / vách cao)
			// Tạm bỏ qua con này 7s để đánh quái khác, tránh nhảy giật tại chỗ
			_unreachableMobId = mob.mobId;
			_unreachableUntilTime = now + 7000;
			_currentAttackingMobId = -1;
			myChar.mobFocus = null;

			// Lùi sâu vào trong map để thoát góc kẹt
			int escapeX = (myChar.cx < mapW / 2) ? (myChar.cx + 90) : (myChar.cx - 90);
			escapeX = System.Math.Max(100, System.Math.Min(mapW - 100, escapeX));
			AutoMove.MoveTowards(escapeX, myChar.cy);
			return;
		}

		// 2. Chọn vị trí đứng an toàn, tuyệt đối không đứng sát mép bản đồ
		int targetX;
		if (mob.x < 100)
		{
			// Quái ở sát mép trái -> bắt buộc đứng bên phải quái (hướng vào trong map)
			targetX = mob.x + 35;
		}
		else if (mob.x > mapW - 100)
		{
			// Quái ở sát mép phải -> bắt buộc đứng bên trái quái (hướng vào trong map)
			targetX = mob.x - 35;
		}
		else
		{
			// Bình thường: đứng phía gần nhân vật hơn
			targetX = mob.x + ((myChar.cx < mob.x) ? -32 : 32);
		}
		targetX = System.Math.Max(70, System.Math.Min(mapW - 70, targetX));

		// 3. Kiểm tra cự ly đánh
		int distMobX = System.Math.Abs(myChar.cx - mob.x);
		int diffY = System.Math.Abs(myChar.cy - mob.y);

		// Nếu đã trong tầm đánh (ngang <= 48, dọc <= 55) -> dừng di chuyển và tấn công
		if (distMobX <= 48 && diffY <= 55)
		{
			myChar.currentMovePoint = null;
			myChar.cdir = ((myChar.cx < mob.x) ? 1 : (-1));
			if (now - _lastAttackTime > 260)
			{
				_lastAttackTime = now;
				_attackMobStartTime = now; // Reset bộ đếm kẹt vì đang đánh trúng quái
				GameScr.gI().doFire(isFireByShortCut: false, skipWaypoint: true);
			}
			return;
		}

		// 4. Nếu nhân vật đang ở dưới thấp cạnh rìa map mà quái ở trên cao
		// Không được nhảy thẳng tại rìa map (vì sẽ đụng biên và rơi xuống) -> phải bước lùi vào trong để lấy đà
		if (myChar.cy > mob.y + 25)
		{
			if (myChar.cx < 80)
			{
				AutoMove.MoveTowards(95, myChar.cy);
				return;
			}
			if (myChar.cx > mapW - 80)
			{
				AutoMove.MoveTowards(mapW - 95, myChar.cy);
				return;
			}
		}

		// Di chuyển tới vị trí an toàn cạnh quái
		AutoMove.MoveTowards(targetX, mob.y);
	}

	public static void PatrolAndAttackNguHanhSon(Char myChar, long now)
	{
		Mob mob = FindAliveMob();
		if (mob != null && System.Math.Abs(myChar.cx - mob.x) < 280)
		{
			AttackMob(myChar, mob, now);
			return;
		}
		if (now - _lastPatrolTime > 2500)
		{
			_lastPatrolTime = now;
			_nguHanhSonPatrolIndex = (_nguHanhSonPatrolIndex + 1) % 4;
		}
		int targetX = AutoMove.NguHanhSonZones[_nguHanhSonPatrolIndex, 0];
		int targetY = AutoMove.NguHanhSonZones[_nguHanhSonPatrolIndex, 1];
		AutoMove.MoveTowards(targetX, targetY);
	}

	public static Mob FindMocNhan()
	{
		if (GameScr.vMob == null)
		{
			return null;
		}
		Char obj = Char.myCharz();
		if (obj == null) return null;

		Mob result = null;
		int num = int.MaxValue;
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			if (mob != null && mob.hp > 0 && mob.status != 0 && mob.status != 1 && (mob.templateId == 0 || mob.x >= 650))
			{
				int num2 = System.Math.Abs(obj.cx - mob.x);
				if (num2 < num)
				{
					num = num2;
					result = mob;
				}
			}
		}
		return result;
	}

	public static Mob FindKhungLong()
	{
		if (GameScr.vMob == null)
		{
			return null;
		}
		Char obj = Char.myCharz();
		if (obj == null) return null;

		Mob result = null;
		int bestScore = int.MaxValue;
		long now = mSystem.currentTimeMillis();
		int mapW = (TileMap.pxw > 0) ? TileMap.pxw : 1200;

		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			if (mob == null || mob.hp <= 0 || mob.status == 0 || mob.status == 1 || mob.isMobMe)
			{
				continue;
			}

			if (mob.mobId == _unreachableMobId && now < _unreachableUntilTime)
			{
				continue;
			}

			int dx = System.Math.Abs(obj.cx - mob.x);
			int dy = System.Math.Abs(obj.cy - mob.y);
			int score = dx + dy * 2;

			if ((mob.x < 85 || mob.x > mapW - 85) && dy > 30)
			{
				score += 600;
			}

			if (score < bestScore)
			{
				bestScore = score;
				result = mob;
			}
		}
		return result;
	}

	public static Mob FindAliveMob()
	{
		if (GameScr.vMob == null)
		{
			return null;
		}
		Char obj = Char.myCharz();
		if (obj == null) return null;

		Mob result = null;
		int bestScore = int.MaxValue;
		long now = mSystem.currentTimeMillis();
		int mapW = (TileMap.pxw > 0) ? TileMap.pxw : 1200;

		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			if (mob == null || mob.hp <= 0 || mob.status == 0 || mob.status == 1 || mob.isMobMe)
			{
				continue;
			}

			// Bỏ qua quái đang bị đánh dấu là không thể với tới
			if (mob.mobId == _unreachableMobId && now < _unreachableUntilTime)
			{
				continue;
			}

			int dx = System.Math.Abs(obj.cx - mob.x);
			int dy = System.Math.Abs(obj.cy - mob.y);
			int score = dx + dy * 2;

			// Nếu quái ở trên cao mà lại sát mép bản đồ (< 85px hoặc > mapW - 85px), cộng điểm phạt lớn
			if ((mob.x < 85 || mob.x > mapW - 85) && dy > 30)
			{
				score += 600;
			}

			if (score < bestScore)
			{
				bestScore = score;
				result = mob;
			}
		}
		return result;
	}
}
