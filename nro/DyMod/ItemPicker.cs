using System;

namespace DyMod;

public static class ItemPicker
{
	private static int _lastPickedItemId = -1;

	private static int _pickAttempts = 0;

	private static long _itemIgnoreUntil = 0L;

	private static long _lastPickTime = 0L;

	public static bool TryPickItem(Char myChar, long now)
	{
		if (GameScr.vItemMap == null || GameScr.vItemMap.size() == 0)
		{
			return false;
		}
		ItemMap itemMap = null;
		int num = int.MaxValue;
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(i);
			if (itemMap2 != null && (itemMap2.itemMapID != _lastPickedItemId || now >= _itemIgnoreUntil) && (itemMap2.playerId == -1 || itemMap2.playerId == 0 || itemMap2.playerId == myChar.charID))
			{
				int num2 = System.Math.Abs(myChar.cx - itemMap2.x) + System.Math.Abs(myChar.cy - itemMap2.y);
				if (num2 < num)
				{
					num = num2;
					itemMap = itemMap2;
				}
			}
		}
		if (itemMap != null)
		{
			int num3 = System.Math.Abs(myChar.cx - itemMap.x);
			if (num3 > 25)
			{
				AutoMove.MoveTowards(itemMap.x, itemMap.y);
			}
			else if (now - _lastPickTime > 160)
			{
				_lastPickTime = now;
				Service.gI().pickItem(itemMap.itemMapID);
				AutoTrain.NotifyProgress();
				if (_lastPickedItemId == itemMap.itemMapID)
				{
					_pickAttempts++;
					if (_pickAttempts > 8)
					{
						_itemIgnoreUntil = now + 8000;
						_pickAttempts = 0;
					}
				}
				else
				{
					_lastPickedItemId = itemMap.itemMapID;
					_pickAttempts = 1;
				}
			}
			return true;
		}
		return false;
	}
}
