using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public class Controller : IMessageHandler
{
	protected static Controller me;

	protected static Controller me2;

	public Message messWait;

	public static bool isLoadingData = false;

	public static bool isConnectOK;

	public static bool isConnectionFail;

	public static bool isDisconnected;

	public static bool isMain;

	private float demCount;

	private int move;

	private int total;

	public static bool isStopReadMessage;

	public static bool isGet_CLIENT_INFO = false;

	public static MyHashTable frameHT_NEWBOSS = new MyHashTable();

	public const sbyte PHUBAN_TYPE_CHIENTRUONGNAMEK = 0;

	public const sbyte PHUBAN_START = 0;

	public const sbyte PHUBAN_UPDATE_POINT = 1;

	public const sbyte PHUBAN_END = 2;

	public const sbyte PHUBAN_LIFE = 4;

	public const sbyte PHUBAN_INFO = 5;

	public static bool isEXTRA_LINK = false;

	public static Controller gI()
	{
		if (me == null)
		{
			me = new Controller();
		}
		return me;
	}

	public static Controller gI2()
	{
		if (me2 == null)
		{
			me2 = new Controller();
		}
		return me2;
	}

	public void onConnectOK(bool isMain1)
	{
		isMain = isMain1;
		mSystem.onConnectOK();
	}

	public void onConnectionFail(bool isMain1)
	{
		isMain = isMain1;
		mSystem.onConnectionFail();
	}

	public void onDisconnected(bool isMain1)
	{
		isMain = isMain1;
		mSystem.onDisconnected();
	}

	public void requestItemPlayer(Message msg)
	{
		try
		{
			int num = msg.reader().readUnsignedByte();
			Item item = GameScr.currentCharViewInfo.arrItemBody[num];
			item.saleCoinLock = msg.reader().readInt();
			item.sys = msg.reader().readByte();
			item.options = new MyVector();
			try
			{
				while (true)
				{
					ItemOption itemOption = readItemOption(msg);
					if (itemOption != null)
					{
						item.options.addElement(itemOption);
					}
				}
			}
			catch (Exception ex)
			{
				Cout.println("Loi tairequestItemPlayer 1" + ex.ToString());
			}
		}
		catch (Exception ex2)
		{
			Cout.println("Loi tairequestItemPlayer 2" + ex2.ToString());
		}
	}

	public void onMessage(Message msg)
	{
		GameCanvas.debugSession.removeAllElements();
		GameCanvas.debug("SA1", 2);
		try
		{
			if (msg.command != -74)
			{
				Res.outz("=========> [READ] cmd= " + msg.command);
			}
			Char obj = null;
			Mob mob = null;
			MyVector myVector = new MyVector();
			int num = 0;
			GameCanvas.timeLoading = 15;
			Controller2.readMessage(msg);
			switch (msg.command)
			{
			case 12:
				read_cmdExtraBig(msg);
				LoginScr.isUpdateItem = false;
				GameScr.gI().readOk();
				GameCanvas.endDlg();
				break;
			case 0:
				readLogin(msg);
				break;
			case 24:
				read_cmdExtra(msg);
				break;
			case 20:
				phuban_Info(msg);
				break;
			case 66:
				readGetImgByName(msg);
				break;
			case 65:
			{
				sbyte id2 = msg.reader().readSByte();
				string text3 = msg.reader().readUTF();
				short num28 = msg.reader().readShort();
				if (ItemTime.isExistMessage(id2))
				{
					if (num28 != 0)
					{
						ItemTime.getMessageById(id2).initTimeText(id2, text3, num28);
					}
					else
					{
						GameScr.textTime.removeElement(ItemTime.getMessageById(id2));
					}
				}
				else
				{
					ItemTime itemTime = new ItemTime();
					itemTime.initTimeText(id2, text3, num28);
					GameScr.textTime.addElement(itemTime);
				}
				break;
			}
			case 112:
			{
				sbyte b32 = msg.reader().readByte();
				Res.outz("spec type= " + b32);
				switch (b32)
				{
				case 0:
					Panel.spearcialImage = msg.reader().readShort();
					Panel.specialInfo = msg.reader().readUTF();
					break;
				case 1:
				{
					sbyte b33 = msg.reader().readByte();
					Char.myCharz().infoSpeacialSkill = new string[b33][];
					Char.myCharz().imgSpeacialSkill = new short[b33][];
					GameCanvas.panel.speacialTabName = new string[b33][];
					for (int num87 = 0; num87 < b33; num87++)
					{
						GameCanvas.panel.speacialTabName[num87] = new string[2];
						string[] array9 = Res.split(msg.reader().readUTF(), "\n", 0);
						if (array9.Length == 2)
						{
							GameCanvas.panel.speacialTabName[num87] = array9;
						}
						if (array9.Length == 1)
						{
							GameCanvas.panel.speacialTabName[num87][0] = array9[0];
							GameCanvas.panel.speacialTabName[num87][1] = string.Empty;
						}
						int num88 = msg.reader().readByte();
						Char.myCharz().infoSpeacialSkill[num87] = new string[num88];
						Char.myCharz().imgSpeacialSkill[num87] = new short[num88];
						for (int num89 = 0; num89 < num88; num89++)
						{
							Char.myCharz().imgSpeacialSkill[num87][num89] = msg.reader().readShort();
							Char.myCharz().infoSpeacialSkill[num87][num89] = msg.reader().readUTF();
						}
					}
					GameCanvas.panel.tabName[25] = GameCanvas.panel.speacialTabName;
					GameCanvas.panel.setTypeSpeacialSkill();
					GameCanvas.panel.show();
					break;
				}
				}
				break;
			}
			case -98:
			{
				sbyte b43 = msg.reader().readByte();
				GameCanvas.menu.showMenu = false;
				if (b43 == 0)
				{
					GameCanvas.startYesNoDlg(msg.reader().readUTF(), new Command(mResources.YES, GameCanvas.instance, 888397, msg.reader().readUTF()), new Command(mResources.NO, GameCanvas.instance, 888396, null));
				}
				break;
			}
			case -97:
				Char.myCharz().cNangdong = msg.reader().readInt();
				break;
			case -96:
			{
				sbyte typeTop = msg.reader().readByte();
				GameCanvas.panel.vTop.removeAllElements();
				string topName = msg.reader().readUTF();
				sbyte b34 = msg.reader().readByte();
				for (int num90 = 0; num90 < b34; num90++)
				{
					int rank = msg.reader().readInt();
					int pId = msg.reader().readInt();
					short headID = msg.reader().readShort();
					short headICON = msg.reader().readShort();
					short body = msg.reader().readShort();
					short leg = msg.reader().readShort();
					string name = msg.reader().readUTF();
					string info3 = msg.reader().readUTF();
					TopInfo topInfo = new TopInfo();
					topInfo.rank = rank;
					topInfo.headID = headID;
					topInfo.headICON = headICON;
					topInfo.body = body;
					topInfo.leg = leg;
					topInfo.name = name;
					topInfo.info = info3;
					topInfo.info2 = msg.reader().readUTF();
					topInfo.pId = pId;
					GameCanvas.panel.vTop.addElement(topInfo);
				}
				GameCanvas.panel.topName = topName;
				GameCanvas.panel.setTypeTop(typeTop);
				GameCanvas.panel.show();
				break;
			}
			case -94:
				while (msg.reader().available() > 0)
				{
					short num19 = msg.reader().readShort();
					int num20 = msg.reader().readInt();
					for (int m = 0; m < Char.myCharz().vSkill.size(); m++)
					{
						Skill skill = (Skill)Char.myCharz().vSkill.elementAt(m);
						if (skill != null && skill.skillId == num19)
						{
							if (num20 < skill.coolDown)
							{
								skill.lastTimeUseThisSkill = mSystem.currentTimeMillis() - (skill.coolDown - num20);
							}
							Res.outz("1 chieu id= " + skill.template.id + " cooldown= " + num20 + "curr cool down= " + skill.coolDown);
						}
					}
				}
				break;
			case -95:
			{
				sbyte b49 = msg.reader().readByte();
				Res.outz("MOB_ME_UPDATE type= " + b49);
				if (b49 == 0)
				{
					int num113 = msg.reader().readInt();
					short templateId = msg.reader().readShort();
					long num114 = msg.reader().readLong();
					SoundMn.gI().explode_1();
					if (num113 == Char.myCharz().charID)
					{
						Char.myCharz().mobMe = new Mob(num113, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num114, 0, num114, (short)(Char.myCharz().cx + ((Char.myCharz().cdir != 1) ? (-40) : 40)), (short)Char.myCharz().cy, 4, 0);
						Char.myCharz().mobMe.isMobMe = true;
						EffecMn.addEff(new Effect(18, Char.myCharz().mobMe.x, Char.myCharz().mobMe.y, 2, 10, -1));
						Char.myCharz().tMobMeBorn = 30;
						GameScr.vMob.addElement(Char.myCharz().mobMe);
					}
					else
					{
						obj = GameScr.findCharInMap(num113);
						if (obj != null)
						{
							Mob mob10 = new Mob(num113, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num114, 0, num114, (short)obj.cx, (short)obj.cy, 4, 0);
							mob10.isMobMe = true;
							obj.mobMe = mob10;
							GameScr.vMob.addElement(obj.mobMe);
						}
						else
						{
							Mob mob11 = GameScr.findMobInMap(num113);
							if (mob11 == null)
							{
								mob11 = new Mob(num113, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num114, 0, num114, -100, -100, 4, 0);
								mob11.isMobMe = true;
								GameScr.vMob.addElement(mob11);
							}
						}
					}
				}
				if (b49 == 1)
				{
					int num115 = msg.reader().readInt();
					int mobId = msg.reader().readByte();
					Res.outz("mod attack id= " + num115);
					if (num115 == Char.myCharz().charID)
					{
						if (GameScr.findMobInMap(mobId) != null)
						{
							Char.myCharz().mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
						}
					}
					else
					{
						obj = GameScr.findCharInMap(num115);
						if (obj != null && GameScr.findMobInMap(mobId) != null)
						{
							obj.mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
						}
					}
				}
				if (b49 == 2)
				{
					int num116 = msg.reader().readInt();
					int num117 = msg.reader().readInt();
					long num118 = msg.reader().readLong();
					long cHPNew = msg.reader().readLong();
					if (num116 == Char.myCharz().charID)
					{
						Res.outz("mob dame= " + num118);
						obj = GameScr.findCharInMap(num117);
						if (obj != null)
						{
							obj.cHPNew = cHPNew;
							if (Char.myCharz().mobMe.isBusyAttackSomeOne)
							{
								obj.doInjure(num118, 0L, isCrit: false, isMob: true);
							}
							else
							{
								Char.myCharz().mobMe.dame = num118;
								Char.myCharz().mobMe.setAttack(obj);
							}
						}
					}
					else
					{
						mob = GameScr.findMobInMap(num116);
						if (mob != null)
						{
							if (num117 == Char.myCharz().charID)
							{
								Char.myCharz().cHPNew = cHPNew;
								if (mob.isBusyAttackSomeOne)
								{
									Char.myCharz().doInjure(num118, 0L, isCrit: false, isMob: true);
								}
								else
								{
									mob.dame = num118;
									mob.setAttack(Char.myCharz());
								}
							}
							else
							{
								obj = GameScr.findCharInMap(num117);
								if (obj != null)
								{
									obj.cHPNew = cHPNew;
									if (mob.isBusyAttackSomeOne)
									{
										obj.doInjure(num118, 0L, isCrit: false, isMob: true);
									}
									else
									{
										mob.dame = num118;
										mob.setAttack(obj);
									}
								}
							}
						}
					}
				}
				if (b49 == 3)
				{
					int num119 = msg.reader().readInt();
					int mobId2 = msg.reader().readInt();
					long hp = msg.reader().readLong();
					long num120 = msg.reader().readLong();
					obj = null;
					obj = ((Char.myCharz().charID != num119) ? GameScr.findCharInMap(num119) : Char.myCharz());
					if (obj != null)
					{
						mob = GameScr.findMobInMap(mobId2);
						if (obj.mobMe != null)
						{
							obj.mobMe.attackOtherMob(mob);
						}
						if (mob != null)
						{
							mob.hp = hp;
							mob.updateHp_bar();
							if (num120 == 0)
							{
								mob.x = mob.xFirst;
								mob.y = mob.yFirst;
								GameScr.startFlyText(mResources.miss, mob.x, mob.y - mob.h, 0, -2, mFont.MISS);
							}
							else
							{
								GameScr.startFlyText("-" + num120, mob.x, mob.y - mob.h, 0, -2, mFont.ORANGE);
							}
						}
					}
				}
				if (b49 == 4)
				{
				}
				if (b49 == 5)
				{
					int num121 = msg.reader().readInt();
					sbyte b50 = msg.reader().readByte();
					int mobId3 = msg.reader().readInt();
					long num122 = msg.reader().readLong();
					long hp2 = msg.reader().readLong();
					Res.outz("MOB_ME_UPDATE type= 5   playerAttack:" + num121 + "  skillID:" + b50 + "  mobAttacked:" + mobId3);
					obj = null;
					obj = ((num121 != Char.myCharz().charID) ? GameScr.findCharInMap(num121) : Char.myCharz());
					if (obj == null)
					{
						Res.outz("MOB_ME_UPDATE char = null == null");
						return;
					}
					Res.outz(obj.cName + "   MOB_ME_UPDATE Attack Mob With Skill ID===" + b50);
					if ((TileMap.tileTypeAtPixel(obj.cx, obj.cy) & 2) == 2)
					{
						obj.setSkillPaint(GameScr.sks[b50], 0);
					}
					else
					{
						obj.setSkillPaint(GameScr.sks[b50], 1);
					}
					Mob mob12 = GameScr.findMobInMap(mobId3);
					if (mob12 == null)
					{
						Res.err(obj.cName + "   MOB_ME_UPDATE mob  nullllllllll");
					}
					if (obj.cx <= mob12.x)
					{
						obj.cdir = 1;
					}
					else
					{
						obj.cdir = -1;
					}
					obj.mobFocus = mob12;
					mob12.hp = hp2;
					mob12.updateHp_bar();
					GameCanvas.debug("SA83v2", 2);
					if (num122 == 0)
					{
						mob12.x = mob12.xFirst;
						mob12.y = mob12.yFirst;
						GameScr.startFlyText(mResources.miss, mob12.x, mob12.y - mob12.h, 0, -2, mFont.MISS);
					}
					else
					{
						GameScr.startFlyText("-" + num122, mob12.x, mob12.y - mob12.h, 0, -2, mFont.ORANGE);
					}
				}
				if (b49 == 6)
				{
					int num123 = msg.reader().readInt();
					if (num123 == Char.myCharz().charID)
					{
						Char.myCharz().mobMe.startDie();
					}
					else
					{
						GameScr.findCharInMap(num123)?.mobMe.startDie();
					}
				}
				if (b49 != 7)
				{
					break;
				}
				int num124 = msg.reader().readInt();
				if (num124 == Char.myCharz().charID)
				{
					Char.myCharz().mobMe = null;
					for (int num125 = 0; num125 < GameScr.vMob.size(); num125++)
					{
						if (((Mob)GameScr.vMob.elementAt(num125)).mobId == num124)
						{
							GameScr.vMob.removeElementAt(num125);
						}
					}
					break;
				}
				obj = GameScr.findCharInMap(num124);
				for (int num126 = 0; num126 < GameScr.vMob.size(); num126++)
				{
					if (((Mob)GameScr.vMob.elementAt(num126)).mobId == num124)
					{
						GameScr.vMob.removeElementAt(num126);
					}
				}
				if (obj != null)
				{
					obj.mobMe = null;
				}
				break;
			}
			case -92:
				mSystem.clientType = msg.reader().readByte();
				if (Rms.loadRMSString(Rms.RMS_ResVersion) != null)
				{
					Rms.clearAll();
				}
				Rms.saveRMSInt(Rms.RMS_clienttype, mSystem.clientType);
				Rms.saveRMSInt(Rms.RMS_lastZoomlevel, mGraphics.zoomLevel);
				if (Rms.loadRMSString(Rms.RMS_ResVersion) == null)
				{
					GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
				}
				break;
			case -91:
			{
				sbyte b28 = msg.reader().readByte();
				GameCanvas.panel.mapNames = new string[b28];
				GameCanvas.panel.planetNames = new string[b28];
				for (int num70 = 0; num70 < b28; num70++)
				{
					GameCanvas.panel.mapNames[num70] = msg.reader().readUTF();
					GameCanvas.panel.planetNames[num70] = msg.reader().readUTF();
				}
				GameCanvas.panel.setTypeMapTrans();
				GameCanvas.panel.show();
				break;
			}
			case -90:
			{
				sbyte b58 = msg.reader().readByte();
				int num146 = msg.reader().readInt();
				Res.outz("===> UPDATE_BODY:    type = " + b58);
				obj = ((Char.myCharz().charID != num146) ? GameScr.findCharInMap(num146) : Char.myCharz());
				if (b58 != -1)
				{
					short num147 = msg.reader().readShort();
					short num148 = msg.reader().readShort();
					short num149 = msg.reader().readShort();
					sbyte isMonkey = msg.reader().readByte();
					if (obj != null)
					{
						if (obj.charID == num146)
						{
							obj.isMask = true;
							obj.isMonkey = isMonkey;
							if (obj.isMonkey != 0)
							{
								obj.isWaitMonkey = false;
								obj.isLockMove = false;
							}
						}
						else if (obj != null)
						{
							obj.isMask = true;
							obj.isMonkey = isMonkey;
						}
						if (num147 != -1)
						{
							obj.head = num147;
						}
						if (num148 != -1)
						{
							obj.body = num148;
						}
						if (num149 != -1)
						{
							obj.leg = num149;
						}
					}
				}
				if (b58 == -1 && obj != null)
				{
					obj.isMask = false;
					obj.isMonkey = 0;
				}
				if (obj == null)
				{
					break;
				}
				Effect.GetCharEff(obj);
				if (obj.bag == 30 && obj.me)
				{
					GameScr.isPickNgocRong = true;
				}
				if (!obj.me)
				{
					break;
				}
				GameScr.isudungCapsun4 = false;
				GameScr.isudungCapsun3 = false;
				for (int num150 = 0; num150 < Char.myCharz().arrItemBag.Length; num150++)
				{
					Item item4 = Char.myCharz().arrItemBag[num150];
					if (item4 == null)
					{
						continue;
					}
					if (item4.template.id == 194)
					{
						GameScr.isudungCapsun4 = item4.quantity > 0;
						if (GameScr.isudungCapsun4)
						{
							break;
						}
					}
					else if (item4.template.id == 193)
					{
						GameScr.isudungCapsun3 = item4.quantity > 0;
					}
				}
				break;
			}
			case -88:
				GameCanvas.endDlg();
				GameCanvas.serverScreen.switchToMe();
				break;
			case -87:
			{
				Res.outz("GET UPDATE_DATA " + msg.reader().available() + " bytes");
				msg.reader().mark(500000);
				createData(msg.reader(), isSaveRMS: true);
				msg.reader().reset();
				sbyte[] data4 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data4);
				sbyte[] data5 = new sbyte[1] { GameScr.vcData };
				Rms.saveRMS("NRdataVersion", data5);
				LoginScr.isUpdateData = false;
				GameScr.gI().readOk();
				break;
			}
			case -86:
			{
				sbyte b35 = msg.reader().readByte();
				Res.outz("server gui ve giao dich action = " + b35);
				if (b35 == 0)
				{
					int playerID = msg.reader().readInt();
					GameScr.gI().giaodich(playerID);
				}
				if (b35 == 1)
				{
					int num91 = msg.reader().readInt();
					Char obj8 = GameScr.findCharInMap(num91);
					if (obj8 == null)
					{
						return;
					}
					GameCanvas.panel.setTypeGiaoDich(obj8);
					GameCanvas.panel.show();
					Service.gI().getPlayerMenu(num91);
				}
				if (b35 == 2)
				{
					sbyte b36 = msg.reader().readByte();
					for (int num92 = 0; num92 < GameCanvas.panel.vMyGD.size(); num92++)
					{
						Item item = (Item)GameCanvas.panel.vMyGD.elementAt(num92);
						if (item.indexUI == b36)
						{
							GameCanvas.panel.vMyGD.removeElement(item);
							break;
						}
					}
				}
				if (b35 == 5)
				{
				}
				if (b35 == 6)
				{
					GameCanvas.panel.isFriendLock = true;
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.panel2.isFriendLock = true;
					}
					GameCanvas.panel.vFriendGD.removeAllElements();
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.panel2.vFriendGD.removeAllElements();
					}
					int friendMoneyGD = msg.reader().readInt();
					sbyte b37 = msg.reader().readByte();
					Res.outz("item size = " + b37);
					for (int num93 = 0; num93 < b37; num93++)
					{
						Item item2 = new Item();
						item2.template = ItemTemplates.get(msg.reader().readShort());
						item2.quantity = msg.reader().readInt();
						int num94 = msg.reader().readUnsignedByte();
						if (num94 != 0)
						{
							item2.itemOption = new ItemOption[num94];
							for (int num95 = 0; num95 < item2.itemOption.Length; num95++)
							{
								ItemOption itemOption5 = readItemOption(msg);
								if (itemOption5 != null)
								{
									item2.itemOption[num95] = itemOption5;
									item2.compare = GameCanvas.panel.getCompare(item2);
								}
							}
						}
						if (GameCanvas.panel2 != null)
						{
							GameCanvas.panel2.vFriendGD.addElement(item2);
						}
						else
						{
							GameCanvas.panel.vFriendGD.addElement(item2);
						}
					}
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.panel2.setTabGiaoDich(isMe: false);
						GameCanvas.panel2.friendMoneyGD = friendMoneyGD;
					}
					else
					{
						GameCanvas.panel.friendMoneyGD = friendMoneyGD;
						if (GameCanvas.panel.currentTabIndex == 2)
						{
							GameCanvas.panel.setTabGiaoDich(isMe: false);
						}
					}
				}
				if (b35 == 7)
				{
					InfoDlg.hide();
					if (GameCanvas.panel.isShow)
					{
						GameCanvas.panel.hide();
					}
				}
				break;
			}
			case -85:
			{
				Res.outz("CAP CHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
				sbyte b18 = msg.reader().readByte();
				if (b18 == 0)
				{
					int num44 = msg.reader().readUnsignedShort();
					Res.outz("lent =" + num44);
					sbyte[] data2 = new sbyte[num44];
					msg.reader().read(ref data2, 0, num44);
					GameScr.imgCapcha = Image.createImage(data2, 0, num44);
					GameScr.gI().keyInput = "-----";
					GameScr.gI().strCapcha = msg.reader().readUTF();
					GameScr.gI().keyCapcha = new int[GameScr.gI().strCapcha.Length];
					GameScr.gI().mobCapcha = new Mob();
					GameScr.gI().right = null;
				}
				if (b18 == 1)
				{
					MobCapcha.isAttack = true;
				}
				if (b18 == 2)
				{
					MobCapcha.explode = true;
					GameScr.gI().right = GameScr.gI().cmdFocus;
				}
				break;
			}
			case -112:
			{
				sbyte b62 = msg.reader().readByte();
				if (b62 == 0)
				{
					sbyte mobIndex = msg.reader().readByte();
					GameScr.findMobInMap(mobIndex).clearBody();
				}
				if (b62 == 1)
				{
					sbyte mobIndex2 = msg.reader().readByte();
					GameScr.findMobInMap(mobIndex2).setBody(msg.reader().readShort());
				}
				break;
			}
			case -84:
			{
				int index2 = msg.reader().readUnsignedByte();
				Mob mob9 = null;
				try
				{
					mob9 = (Mob)GameScr.vMob.elementAt(index2);
				}
				catch (Exception)
				{
				}
				if (mob9 != null)
				{
					mob9.maxHp = msg.reader().readLong();
				}
				break;
			}
			case -83:
			{
				sbyte b25 = msg.reader().readByte();
				if (b25 == 0)
				{
					int num61 = msg.reader().readShort();
					int bgRID = msg.reader().readShort();
					int num62 = msg.reader().readUnsignedByte();
					int num63 = msg.reader().readInt();
					string text5 = msg.reader().readUTF();
					int xR = msg.reader().readShort();
					int yR = msg.reader().readShort();
					sbyte b26 = msg.reader().readByte();
					if (b26 == 1)
					{
						GameScr.gI().isRongNamek = true;
					}
					else
					{
						GameScr.gI().isRongNamek = false;
					}
					GameScr.gI().xR = xR;
					GameScr.gI().yR = yR;
					Res.outz("xR= " + xR + " yR= " + yR + " +++++++++++++++++++++++++++++++++++++++");
					if (Char.myCharz().charID == num63)
					{
						GameCanvas.panel.hideNow();
						GameScr.gI().activeRongThanEff(isMe: true);
					}
					else if (TileMap.mapID == num61 && TileMap.zoneID == num62)
					{
						GameScr.gI().activeRongThanEff(isMe: false);
					}
					else if (mGraphics.zoomLevel > 1)
					{
						GameScr.gI().doiMauTroi();
					}
					GameScr.gI().mapRID = num61;
					GameScr.gI().bgRID = bgRID;
					GameScr.gI().zoneRID = num62;
				}
				if (b25 == 1)
				{
					Res.outz("map RID = " + GameScr.gI().mapRID + " zone RID= " + GameScr.gI().zoneRID);
					Res.outz("map ID = " + TileMap.mapID + " zone ID= " + TileMap.zoneID);
					if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
					{
						GameScr.gI().hideRongThanEff();
					}
					else
					{
						GameScr.gI().isRongThanXuatHien = false;
						if (GameScr.gI().isRongNamek)
						{
							GameScr.gI().isRongNamek = false;
						}
					}
				}
				if (b25 == 2)
				{
				}
				break;
			}
			case -82:
			{
				sbyte b54 = msg.reader().readByte();
				TileMap.tileIndex = new int[b54][][];
				TileMap.tileType = new int[b54][];
				Res.outz(">>>>>>Cmd.TILE_SET:nTile: " + b54);
				for (int num142 = 0; num142 < b54; num142++)
				{
					Res.outz(num142 + ">>>>>>Cmd.TILE_SET: forr");
					sbyte b55 = msg.reader().readByte();
					Res.outz(num142 + ">>>>>>Cmd.TILE_SET:nTypeSize: " + b55);
					TileMap.tileType[num142] = new int[b55];
					TileMap.tileIndex[num142] = new int[b55][];
					for (int num143 = 0; num143 < b55; num143++)
					{
						TileMap.tileType[num142][num143] = msg.reader().readInt();
						sbyte b56 = msg.reader().readByte();
						TileMap.tileIndex[num142][num143] = new int[b56];
						for (int num144 = 0; num144 < b56; num144++)
						{
							TileMap.tileIndex[num142][num143][num144] = msg.reader().readByte();
						}
					}
				}
				break;
			}
			case -81:
			{
				sbyte b39 = msg.reader().readByte();
				if (b39 == 0)
				{
					string src = msg.reader().readUTF();
					string src2 = msg.reader().readUTF();
					GameCanvas.panel.setTypeCombine();
					GameCanvas.panel.combineInfo = mFont.tahoma_7b_blue.splitFontArray(src, Panel.WIDTH_PANEL);
					GameCanvas.panel.combineTopInfo = mFont.tahoma_7.splitFontArray(src2, Panel.WIDTH_PANEL);
					GameCanvas.panel.show();
				}
				if (b39 == 1)
				{
					GameCanvas.panel.vItemCombine.removeAllElements();
					sbyte b40 = msg.reader().readByte();
					for (int num99 = 0; num99 < b40; num99++)
					{
						sbyte b41 = msg.reader().readByte();
						for (int num100 = 0; num100 < Char.myCharz().arrItemBag.Length; num100++)
						{
							Item item3 = Char.myCharz().arrItemBag[num100];
							if (item3 != null && item3.indexUI == b41)
							{
								item3.isSelect = true;
								GameCanvas.panel.vItemCombine.addElement(item3);
							}
						}
					}
					if (GameCanvas.panel.isShow)
					{
						GameCanvas.panel.setTabCombine();
					}
				}
				if (b39 == 2)
				{
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(0);
				}
				if (b39 == 3)
				{
					GameCanvas.panel.combineSuccess = 1;
					GameCanvas.panel.setCombineEff(0);
				}
				if (b39 == 4)
				{
					short iconID = msg.reader().readShort();
					GameCanvas.panel.iconID3 = iconID;
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(1);
				}
				if (b39 == 5)
				{
					short iconID2 = msg.reader().readShort();
					GameCanvas.panel.iconID3 = iconID2;
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(2);
				}
				if (b39 == 6)
				{
					short iconID3 = msg.reader().readShort();
					short iconID4 = msg.reader().readShort();
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(3);
					GameCanvas.panel.iconID1 = iconID3;
					GameCanvas.panel.iconID3 = iconID4;
				}
				if (b39 == 7)
				{
					short iconID5 = msg.reader().readShort();
					GameCanvas.panel.iconID3 = iconID5;
					GameCanvas.panel.combineSuccess = 0;
					GameCanvas.panel.setCombineEff(4);
				}
				if (b39 == 8)
				{
					GameCanvas.panel.iconID3 = -1;
					GameCanvas.panel.combineSuccess = 1;
					GameCanvas.panel.setCombineEff(4);
				}
				short num101 = 21;
				int num102 = 0;
				int num103 = 0;
				try
				{
					num101 = msg.reader().readShort();
					num102 = msg.reader().readShort();
					num103 = msg.reader().readShort();
					GameCanvas.panel.xS = num102 - GameScr.cmx;
					GameCanvas.panel.yS = num103 - GameScr.cmy;
				}
				catch (Exception)
				{
				}
				for (int num104 = 0; num104 < GameScr.vNpc.size(); num104++)
				{
					Npc npc6 = (Npc)GameScr.vNpc.elementAt(num104);
					if (npc6.template.npcTemplateId == num101)
					{
						GameCanvas.panel.xS = npc6.cx - GameScr.cmx;
						GameCanvas.panel.yS = npc6.cy - GameScr.cmy;
						GameCanvas.panel.idNPC = num101;
						break;
					}
				}
				break;
			}
			case -80:
			{
				sbyte b30 = msg.reader().readByte();
				InfoDlg.hide();
				if (b30 == 0)
				{
					GameCanvas.panel.vFriend.removeAllElements();
					int num80 = msg.reader().readUnsignedByte();
					for (int num81 = 0; num81 < num80; num81++)
					{
						Char obj6 = new Char();
						obj6.charID = msg.reader().readInt();
						obj6.head = msg.reader().readShort();
						obj6.headICON = msg.reader().readShort();
						obj6.body = msg.reader().readShort();
						obj6.leg = msg.reader().readShort();
						obj6.bag = msg.reader().readShort();
						obj6.cName = msg.reader().readUTF();
						bool isOnline2 = msg.reader().readBoolean();
						InfoItem infoItem2 = new InfoItem(mResources.power + ": " + msg.reader().readUTF());
						infoItem2.charInfo = obj6;
						infoItem2.isOnline = isOnline2;
						GameCanvas.panel.vFriend.addElement(infoItem2);
					}
					GameCanvas.panel.setTypeFriend();
					GameCanvas.panel.show();
				}
				if (b30 == 3)
				{
					MyVector vFriend = GameCanvas.panel.vFriend;
					int num82 = msg.reader().readInt();
					Res.outz("online offline id=" + num82);
					for (int num83 = 0; num83 < vFriend.size(); num83++)
					{
						InfoItem infoItem3 = (InfoItem)vFriend.elementAt(num83);
						if (infoItem3.charInfo != null && infoItem3.charInfo.charID == num82)
						{
							Res.outz("online= " + infoItem3.isOnline);
							infoItem3.isOnline = msg.reader().readBoolean();
							break;
						}
					}
				}
				if (b30 != 2)
				{
					break;
				}
				MyVector vFriend2 = GameCanvas.panel.vFriend;
				int num84 = msg.reader().readInt();
				for (int num85 = 0; num85 < vFriend2.size(); num85++)
				{
					InfoItem infoItem4 = (InfoItem)vFriend2.elementAt(num85);
					if (infoItem4.charInfo != null && infoItem4.charInfo.charID == num84)
					{
						vFriend2.removeElement(infoItem4);
						break;
					}
				}
				if (GameCanvas.panel.isShow)
				{
					GameCanvas.panel.setTabFriend();
				}
				break;
			}
			case -99:
				InfoDlg.hide();
				if (msg.reader().readByte() == 0)
				{
					GameCanvas.panel.vEnemy.removeAllElements();
					int num67 = msg.reader().readUnsignedByte();
					for (int num68 = 0; num68 < num67; num68++)
					{
						Char obj5 = new Char();
						obj5.charID = msg.reader().readInt();
						obj5.head = msg.reader().readShort();
						obj5.headICON = msg.reader().readShort();
						obj5.body = msg.reader().readShort();
						obj5.leg = msg.reader().readShort();
						obj5.bag = msg.reader().readShort();
						obj5.cName = msg.reader().readUTF();
						InfoItem infoItem = new InfoItem(msg.reader().readUTF());
						bool isOnline = msg.reader().readBoolean();
						infoItem.charInfo = obj5;
						infoItem.isOnline = isOnline;
						Res.outz("isonline = " + isOnline);
						GameCanvas.panel.vEnemy.addElement(infoItem);
					}
					GameCanvas.panel.setTypeEnemy();
					GameCanvas.panel.show();
				}
				break;
			case -79:
			{
				InfoDlg.hide();
				int num69 = msg.reader().readInt();
				Char charMenu = GameCanvas.panel.charMenu;
				if (charMenu == null)
				{
					return;
				}
				charMenu.cPower = msg.reader().readLong();
				charMenu.currStrLevel = msg.reader().readUTF();
				break;
			}
			case -93:
			{
				short num32 = msg.reader().readShort();
				BgItem.newSmallVersion = new sbyte[num32];
				for (int num33 = 0; num33 < num32; num33++)
				{
					BgItem.newSmallVersion[num33] = msg.reader().readByte();
				}
				break;
			}
			case -77:
			{
				short num26 = msg.reader().readShort();
				SmallImage.newSmallVersion = new sbyte[num26];
				SmallImage.maxSmall = num26;
				SmallImage.imgNew = new Small[num26];
				for (int num27 = 0; num27 < num26; num27++)
				{
					SmallImage.newSmallVersion[num27] = msg.reader().readByte();
				}
				break;
			}
			case -76:
				switch (msg.reader().readByte())
				{
				case 0:
				{
					sbyte b11 = msg.reader().readByte();
					if (b11 <= 0)
					{
						return;
					}
					Char.myCharz().arrArchive = new Archivement[b11];
					for (int n = 0; n < b11; n++)
					{
						Char.myCharz().arrArchive[n] = new Archivement();
						Char.myCharz().arrArchive[n].info1 = n + 1 + ". " + msg.reader().readUTF();
						Char.myCharz().arrArchive[n].info2 = msg.reader().readUTF();
						Char.myCharz().arrArchive[n].money = msg.reader().readShort();
						Char.myCharz().arrArchive[n].isFinish = msg.reader().readBoolean();
						Char.myCharz().arrArchive[n].isRecieve = msg.reader().readBoolean();
					}
					GameCanvas.panel.setTypeArchivement();
					GameCanvas.panel.show();
					break;
				}
				case 1:
				{
					int num22 = msg.reader().readUnsignedByte();
					if (Char.myCharz().arrArchive[num22] != null)
					{
						Char.myCharz().arrArchive[num22].isRecieve = true;
					}
					break;
				}
				}
				break;
			case -74:
			{
				if (ServerListScreen.stopDownload)
				{
					return;
				}
				if (!GameCanvas.isGetResourceFromServer())
				{
					Service.gI().getResource(3, null);
					SmallImage.loadBigRMS();
					SplashScr.imgLogo = null;
					if (Rms.loadRMSString(Rms.RMS_acc) != null || Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect) != null)
					{
						LoginScr.isContinueToLogin = true;
					}
					GameCanvas.loginScr = new LoginScr();
					GameCanvas.loginScr.switchToMe();
					return;
				}
				bool flag3 = true;
				Res.outz("1>>GET_IMAGE_SOURCE = " + msg.reader().available());
				sbyte b10 = msg.reader().readByte();
				Res.outz("2>GET_IMAGE_SOURCE = " + b10);
				if (b10 == 0)
				{
					int num13 = msg.reader().readInt();
					Res.outz("3>GET_IMAGE_SOURCE serverVersion = " + num13);
					string text = Rms.loadRMSString(Rms.RMS_ResVersion);
					int num14 = ((text == null || !(text != string.Empty)) ? (-1) : int.Parse(text));
					Res.outz("4>>>GET_IMAGE_SOURCE: version>> " + text + " <> " + num14 + "!=" + num13);
					if (num14 == -1 || num14 != num13)
					{
						GameCanvas.serverScreen.show2();
					}
					else
					{
						SmallImage.loadBigRMS();
						SplashScr.imgLogo = null;
						ServerListScreen.loadScreen = true;
						Res.outz(">>>vo ne: " + GameCanvas.currentScreen);
						if (GameCanvas.currentScreen != GameCanvas.loginScr)
						{
							if (GameCanvas.serverScreen == null)
							{
								GameCanvas.serverScreen = new ServerListScreen();
							}
							GameCanvas.serverScreen.switchToMe();
						}
						else
						{
							if (GameCanvas.loginScr == null)
							{
								GameCanvas.loginScr = new LoginScr();
							}
							GameCanvas.loginScr.doLogin();
						}
					}
				}
				if (b10 == 1)
				{
					ServerListScreen.strWait = mResources.downloading_data;
					short nBig = msg.reader().readShort();
					ServerListScreen.nBig = nBig;
					Service.gI().getResource(2, null);
				}
				if (b10 == 2)
				{
					try
					{
						isLoadingData = true;
						GameCanvas.endDlg();
						ServerListScreen.demPercent++;
						ServerListScreen.percent = ServerListScreen.demPercent * 100 / ServerListScreen.nBig;
						string text2 = msg.reader().readUTF();
						Res.outz(">>>vo serverPath: " + text2);
						string[] array2 = Res.split(text2, "/", 0);
						string filename = "x" + mGraphics.zoomLevel + array2[array2.Length - 1];
						int num15 = msg.reader().readInt();
						sbyte[] data = new sbyte[num15];
						msg.reader().read(ref data, 0, num15);
						Rms.saveRMS(filename, data);
					}
					catch (Exception)
					{
						GameCanvas.startOK(mResources.pls_restart_game_error, 8885, null);
					}
				}
				if (b10 == 3 && flag3)
				{
					isLoadingData = false;
					int num16 = msg.reader().readInt();
					Res.outz(">>>GET_IMAGE_SOURCE: lastVersion>> " + num16);
					Rms.saveRMSString(Rms.RMS_ResVersion, num16 + string.Empty);
					Service.gI().getResource(3, null);
					GameCanvas.endDlg();
					SplashScr.imgLogo = null;
					SmallImage.loadBigRMS();
					mSystem.gcc();
					ServerListScreen.bigOk = true;
					ServerListScreen.loadScreen = true;
					GameScr.gI().loadGameScr();
					GameScr.isLoadAllData = false;
					Service.gI().updateData();
					if (GameCanvas.currentScreen != GameCanvas.loginScr)
					{
						GameCanvas.serverScreen.switchToMe();
					}
				}
				break;
			}
			case -43:
			{
				sbyte itemAction = msg.reader().readByte();
				sbyte b24 = msg.reader().readByte();
				sbyte index = msg.reader().readByte();
				string info = msg.reader().readUTF();
				GameCanvas.panel.itemRequest(itemAction, info, b24, index);
				break;
			}
			case -59:
			{
				sbyte typePK = msg.reader().readByte();
				GameScr.gI().player_vs_player(msg.reader().readInt(), msg.reader().readInt(), msg.reader().readUTF(), typePK);
				break;
			}
			case -62:
			{
				int num153 = msg.reader().readUnsignedByte();
				sbyte b60 = msg.reader().readByte();
				if (b60 <= 0)
				{
					break;
				}
				ClanImage clanImage2 = ClanImage.getClanImage((short)num153);
				if (clanImage2 == null)
				{
					break;
				}
				clanImage2.idImage = new short[b60];
				for (int num154 = 0; num154 < b60; num154++)
				{
					clanImage2.idImage[num154] = msg.reader().readShort();
					if (clanImage2.idImage[num154] > 0)
					{
						SmallImage.vKeys.addElement(clanImage2.idImage[num154] + string.Empty);
					}
				}
				break;
			}
			case -65:
			{
				InfoDlg.hide();
				int num86 = msg.reader().readInt();
				sbyte b31 = msg.reader().readByte();
				if (b31 == 0)
				{
					break;
				}
				if (Char.myCharz().charID == num86)
				{
					isStopReadMessage = true;
					GameScr.lockTick = 500;
					GameScr.gI().center = null;
					if (b31 == 0 || b31 == 1 || b31 == 3)
					{
						Teleport p = new Teleport(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 0, isMe: true, (b31 != 1) ? b31 : Char.myCharz().cgender);
						Teleport.addTeleport(p);
					}
					if (b31 == 2)
					{
						GameScr.lockTick = 50;
						Char.myCharz().hide();
					}
				}
				else
				{
					Char obj7 = GameScr.findCharInMap(num86);
					if ((b31 == 0 || b31 == 1 || b31 == 3) && obj7 != null)
					{
						obj7.isUsePlane = true;
						Teleport teleport = new Teleport(obj7.cx, obj7.cy, obj7.head, obj7.cdir, 0, isMe: false, (b31 != 1) ? b31 : obj7.cgender);
						teleport.id = num86;
						Teleport.addTeleport(teleport);
					}
					if (b31 == 2)
					{
						obj7.hide();
					}
				}
				break;
			}
			case -64:
			{
				int num17 = msg.reader().readInt();
				int num18 = msg.reader().readShort();
				obj = null;
				obj = ((num17 != Char.myCharz().charID) ? GameScr.findCharInMap(num17) : Char.myCharz());
				if (obj == null)
				{
					return;
				}
				obj.bag = num18;
				Effect.GetCharEff(obj);
				Res.outz("cmd:-64 UPDATE BAG PLAER = " + ((obj != null) ? obj.cName : string.Empty) + num17 + " BAG ID= " + num18);
				if (num18 == 30 && obj.me)
				{
					GameScr.isPickNgocRong = true;
				}
				break;
			}
			case -63:
			{
				Res.outz("GET BAG");
				int iD = msg.reader().readShort();
				sbyte b7 = msg.reader().readByte();
				ClanImage clanImage = new ClanImage();
				clanImage.ID = iD;
				if (b7 > 0)
				{
					clanImage.idImage = new short[b7];
					for (int j = 0; j < b7; j++)
					{
						clanImage.idImage[j] = msg.reader().readShort();
						Res.outz("ID=  " + iD + " frame= " + clanImage.idImage[j]);
					}
					ClanImage.idImages.put(iD + string.Empty, clanImage);
				}
				break;
			}
			case -57:
			{
				string strInvite = msg.reader().readUTF();
				int clanID = msg.reader().readInt();
				int code = msg.reader().readInt();
				GameScr.gI().clanInvite(strInvite, clanID, code);
				break;
			}
			case -51:
				InfoDlg.hide();
				readClanMsg(msg, 0);
				if (GameCanvas.panel.isMessage && GameCanvas.panel.type == 5)
				{
					GameCanvas.panel.initTabClans();
				}
				break;
			case -53:
			{
				InfoDlg.hide();
				bool flag7 = false;
				int num76 = msg.reader().readInt();
				Res.outz("clanId= " + num76);
				if (num76 == -1)
				{
					flag7 = true;
					Char.myCharz().clan = null;
					ClanMessage.vMessage.removeAllElements();
					if (GameCanvas.panel.member != null)
					{
						GameCanvas.panel.member.removeAllElements();
					}
					if (GameCanvas.panel.myMember != null)
					{
						GameCanvas.panel.myMember.removeAllElements();
					}
					if (GameCanvas.currentScreen == GameScr.gI())
					{
						GameCanvas.panel.setTabClans();
					}
					return;
				}
				GameCanvas.panel.tabIcon = null;
				if (Char.myCharz().clan == null)
				{
					Char.myCharz().clan = new Clan();
				}
				Char.myCharz().clan.ID = num76;
				Char.myCharz().clan.name = msg.reader().readUTF();
				Char.myCharz().clan.slogan = msg.reader().readUTF();
				Char.myCharz().clan.imgID = msg.reader().readShort();
				Char.myCharz().clan.powerPoint = msg.reader().readUTF();
				Char.myCharz().clan.leaderName = msg.reader().readUTF();
				Char.myCharz().clan.currMember = msg.reader().readUnsignedByte();
				Char.myCharz().clan.maxMember = msg.reader().readUnsignedByte();
				Char.myCharz().role = msg.reader().readByte();
				Char.myCharz().clan.clanPoint = msg.reader().readInt();
				Char.myCharz().clan.level = msg.reader().readByte();
				GameCanvas.panel.myMember = new MyVector();
				for (int num77 = 0; num77 < Char.myCharz().clan.currMember; num77++)
				{
					Member member5 = new Member();
					member5.ID = msg.reader().readInt();
					member5.head = msg.reader().readShort();
					member5.headICON = msg.reader().readShort();
					member5.leg = msg.reader().readShort();
					member5.body = msg.reader().readShort();
					member5.name = msg.reader().readUTF();
					member5.role = msg.reader().readByte();
					member5.powerPoint = msg.reader().readUTF();
					member5.donate = msg.reader().readInt();
					member5.receive_donate = msg.reader().readInt();
					member5.clanPoint = msg.reader().readInt();
					member5.curClanPoint = msg.reader().readInt();
					member5.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					GameCanvas.panel.myMember.addElement(member5);
				}
				int num78 = msg.reader().readUnsignedByte();
				for (int num79 = 0; num79 < num78; num79++)
				{
					readClanMsg(msg, -1);
				}
				if (GameCanvas.panel.isSearchClan || GameCanvas.panel.isViewMember || GameCanvas.panel.isMessage)
				{
					GameCanvas.panel.setTabClans();
				}
				if (flag7)
				{
					GameCanvas.panel.setTabClans();
				}
				Res.outz("=>>>>>>>>>>>>>>>>>>>>>> -537 MY CLAN INFO");
				break;
			}
			case -52:
			{
				sbyte b29 = msg.reader().readByte();
				if (b29 == 0)
				{
					Member member2 = new Member();
					member2.ID = msg.reader().readInt();
					member2.head = msg.reader().readShort();
					member2.headICON = msg.reader().readShort();
					member2.leg = msg.reader().readShort();
					member2.body = msg.reader().readShort();
					member2.name = msg.reader().readUTF();
					member2.role = msg.reader().readByte();
					member2.powerPoint = msg.reader().readUTF();
					member2.donate = msg.reader().readInt();
					member2.receive_donate = msg.reader().readInt();
					member2.clanPoint = msg.reader().readInt();
					member2.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					if (GameCanvas.panel.myMember == null)
					{
						GameCanvas.panel.myMember = new MyVector();
					}
					GameCanvas.panel.myMember.addElement(member2);
					GameCanvas.panel.initTabClans();
				}
				if (b29 == 1)
				{
					GameCanvas.panel.myMember.removeElementAt(msg.reader().readByte());
					Panel panel = GameCanvas.panel;
					panel.currentListLength--;
					GameCanvas.panel.initTabClans();
				}
				if (b29 == 2)
				{
					Member member3 = new Member();
					member3.ID = msg.reader().readInt();
					member3.head = msg.reader().readShort();
					member3.headICON = msg.reader().readShort();
					member3.leg = msg.reader().readShort();
					member3.body = msg.reader().readShort();
					member3.name = msg.reader().readUTF();
					member3.role = msg.reader().readByte();
					member3.powerPoint = msg.reader().readUTF();
					member3.donate = msg.reader().readInt();
					member3.receive_donate = msg.reader().readInt();
					member3.clanPoint = msg.reader().readInt();
					member3.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					for (int num75 = 0; num75 < GameCanvas.panel.myMember.size(); num75++)
					{
						Member member4 = (Member)GameCanvas.panel.myMember.elementAt(num75);
						if (member4.ID == member3.ID)
						{
							if (Char.myCharz().charID == member3.ID)
							{
								Char.myCharz().role = member3.role;
							}
							Member o = member3;
							GameCanvas.panel.myMember.removeElement(member4);
							GameCanvas.panel.myMember.insertElementAt(o, num75);
							return;
						}
					}
				}
				Res.outz("=>>>>>>>>>>>>>>>>>>>>>> -52  MY CLAN UPDSTE");
				break;
			}
			case -50:
			{
				InfoDlg.hide();
				GameCanvas.panel.member = new MyVector();
				sbyte b14 = msg.reader().readByte();
				for (int num31 = 0; num31 < b14; num31++)
				{
					Member member = new Member();
					member.ID = msg.reader().readInt();
					member.head = msg.reader().readShort();
					member.headICON = msg.reader().readShort();
					member.leg = msg.reader().readShort();
					member.body = msg.reader().readShort();
					member.name = msg.reader().readUTF();
					member.role = msg.reader().readByte();
					member.powerPoint = msg.reader().readUTF();
					member.donate = msg.reader().readInt();
					member.receive_donate = msg.reader().readInt();
					member.clanPoint = msg.reader().readInt();
					member.joinTime = NinjaUtil.getDate(msg.reader().readInt());
					GameCanvas.panel.member.addElement(member);
				}
				GameCanvas.panel.isViewMember = true;
				GameCanvas.panel.isSearchClan = false;
				GameCanvas.panel.isMessage = false;
				GameCanvas.panel.currentListLength = GameCanvas.panel.member.size() + 2;
				GameCanvas.panel.initTabClans();
				break;
			}
			case -47:
			{
				InfoDlg.hide();
				sbyte b6 = msg.reader().readByte();
				Res.outz("clan = " + b6);
				if (b6 == 0)
				{
					GameCanvas.panel.clanReport = mResources.cannot_find_clan;
					GameCanvas.panel.clans = null;
				}
				else
				{
					GameCanvas.panel.clans = new Clan[b6];
					Res.outz("clan search lent= " + GameCanvas.panel.clans.Length);
					for (int i = 0; i < GameCanvas.panel.clans.Length; i++)
					{
						GameCanvas.panel.clans[i] = new Clan();
						GameCanvas.panel.clans[i].ID = msg.reader().readInt();
						GameCanvas.panel.clans[i].name = msg.reader().readUTF();
						GameCanvas.panel.clans[i].slogan = msg.reader().readUTF();
						GameCanvas.panel.clans[i].imgID = msg.reader().readShort();
						GameCanvas.panel.clans[i].powerPoint = msg.reader().readUTF();
						GameCanvas.panel.clans[i].leaderName = msg.reader().readUTF();
						GameCanvas.panel.clans[i].currMember = msg.reader().readUnsignedByte();
						GameCanvas.panel.clans[i].maxMember = msg.reader().readUnsignedByte();
						GameCanvas.panel.clans[i].date = msg.reader().readInt();
					}
				}
				GameCanvas.panel.isSearchClan = true;
				GameCanvas.panel.isViewMember = false;
				GameCanvas.panel.isMessage = false;
				if (GameCanvas.panel.isSearchClan)
				{
					GameCanvas.panel.initTabClans();
				}
				break;
			}
			case -46:
			{
				InfoDlg.hide();
				sbyte b61 = msg.reader().readByte();
				if (b61 == 1 || b61 == 3)
				{
					GameCanvas.endDlg();
					ClanImage.vClanImage.removeAllElements();
					int num156 = msg.reader().readShort();
					for (int num157 = 0; num157 < num156; num157++)
					{
						ClanImage clanImage3 = new ClanImage();
						clanImage3.ID = msg.reader().readShort();
						clanImage3.name = msg.reader().readUTF();
						clanImage3.xu = msg.reader().readInt();
						clanImage3.luong = msg.reader().readInt();
						if (!ClanImage.isExistClanImage(clanImage3.ID))
						{
							ClanImage.addClanImage(clanImage3);
							continue;
						}
						ClanImage.getClanImage((short)clanImage3.ID).name = clanImage3.name;
						ClanImage.getClanImage((short)clanImage3.ID).xu = clanImage3.xu;
						ClanImage.getClanImage((short)clanImage3.ID).luong = clanImage3.luong;
					}
					if (Char.myCharz().clan != null)
					{
						GameCanvas.panel.changeIcon();
					}
				}
				if (b61 == 4)
				{
					Char.myCharz().clan.imgID = msg.reader().readShort();
					Char.myCharz().clan.slogan = msg.reader().readUTF();
				}
				break;
			}
			case -61:
			{
				int num130 = msg.reader().readInt();
				if (num130 != Char.myCharz().charID)
				{
					if (GameScr.findCharInMap(num130) != null)
					{
						GameScr.findCharInMap(num130).clanID = msg.reader().readInt();
						if (GameScr.findCharInMap(num130).clanID == -2)
						{
							GameScr.findCharInMap(num130).isCopy = true;
						}
					}
				}
				else if (Char.myCharz().clan != null)
				{
					Char.myCharz().clan.ID = msg.reader().readInt();
				}
				break;
			}
			case -42:
				Char.myCharz().cHPGoc = msg.readInt3Byte();
				Char.myCharz().cMPGoc = msg.readInt3Byte();
				Char.myCharz().cDamGoc = msg.reader().readInt();
				Char.myCharz().cHPFull = msg.reader().readLong();
				Char.myCharz().cMPFull = msg.reader().readLong();
				Char.myCharz().cHP = msg.reader().readLong();
				Char.myCharz().cMP = msg.reader().readLong();
				Char.myCharz().cspeed = msg.reader().readByte();
				Char.myCharz().hpFrom1000TiemNang = msg.reader().readByte();
				Char.myCharz().mpFrom1000TiemNang = msg.reader().readByte();
				Char.myCharz().damFrom1000TiemNang = msg.reader().readByte();
				Char.myCharz().cDamFull = msg.reader().readLong();
				Char.myCharz().cDefull = msg.reader().readLong();
				Char.myCharz().cCriticalFull = msg.reader().readByte();
				Char.myCharz().cTiemNang = msg.reader().readLong();
				Char.myCharz().expForOneAdd = msg.reader().readShort();
				Char.myCharz().cDefGoc = msg.reader().readInt();
				Char.myCharz().cCriticalGoc = msg.reader().readByte();
				Char.myCharz().cGiamST = msg.reader().readByte();
				Char.myCharz().cCritDameFull = msg.reader().readShort();
				InfoDlg.hide();
				break;
			case 1:
			{
				bool flag8 = msg.reader().readBool();
				Res.outz("isRes= " + flag8);
				if (!flag8)
				{
					GameCanvas.startOKDlg(msg.reader().readUTF());
					break;
				}
				GameCanvas.loginScr.isLogin2 = false;
				Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, string.Empty);
				GameCanvas.endDlg();
				GameCanvas.loginScr.doLogin();
				break;
			}
			case 2:
				Char.isLoadingMap = false;
				LoginScr.isLoggingIn = false;
				if (!GameScr.isLoadAllData)
				{
					GameScr.gI().initSelectChar();
				}
				BgItem.clearHashTable();
				GameCanvas.endDlg();
				CreateCharScr.isCreateChar = true;
				CreateCharScr.gI().switchToMe();
				break;
			case -107:
			{
				sbyte b19 = msg.reader().readByte();
				if (b19 == 0)
				{
					Char.myCharz().havePet = false;
				}
				if (b19 == 1)
				{
					Char.myCharz().havePet = true;
				}
				if (b19 != 2)
				{
					break;
				}
				InfoDlg.hide();
				Char.myPetz().head = msg.reader().readShort();
				Debug.LogWarning(">>>cmd head:" + Char.myPetz().avatarz());
				Res.outz("tra ve head= " + Char.myCharz().head);
				Char.myPetz().setDefaultPart();
				int num45 = msg.reader().readUnsignedByte();
				Res.outz("num body = " + num45);
				Char.myPetz().arrItemBody = new Item[num45];
				for (int num46 = 0; num46 < num45; num46++)
				{
					short num47 = msg.reader().readShort();
					Res.outz("template id= " + num47);
					if (num47 == -1)
					{
						continue;
					}
					Res.outz("1");
					Char.myPetz().arrItemBody[num46] = new Item();
					Char.myPetz().arrItemBody[num46].template = ItemTemplates.get(num47);
					int type2 = Char.myPetz().arrItemBody[num46].template.type;
					Char.myPetz().arrItemBody[num46].quantity = msg.reader().readInt();
					Res.outz("3");
					Char.myPetz().arrItemBody[num46].info = msg.reader().readUTF();
					Char.myPetz().arrItemBody[num46].content = msg.reader().readUTF();
					int num48 = msg.reader().readUnsignedByte();
					Res.outz("option size= " + num48);
					if (num48 != 0)
					{
						Char.myPetz().arrItemBody[num46].itemOption = new ItemOption[num48];
						for (int num49 = 0; num49 < Char.myPetz().arrItemBody[num46].itemOption.Length; num49++)
						{
							ItemOption itemOption3 = readItemOption(msg);
							if (itemOption3 != null)
							{
								Char.myPetz().arrItemBody[num46].itemOption[num49] = itemOption3;
							}
						}
					}
					switch (type2)
					{
					case 0:
						Char.myPetz().body = Char.myPetz().arrItemBody[num46].template.part;
						break;
					case 1:
						Char.myPetz().leg = Char.myPetz().arrItemBody[num46].template.part;
						break;
					}
				}
				Char.myPetz().cHP = msg.reader().readLong();
				Char.myPetz().cHPFull = msg.reader().readLong();
				Char.myPetz().cMP = msg.reader().readLong();
				Char.myPetz().cMPFull = msg.reader().readLong();
				Char.myPetz().cDamFull = msg.reader().readLong();
				Char.myPetz().cName = msg.reader().readUTF();
				Char.myPetz().currStrLevel = msg.reader().readUTF();
				Char.myPetz().cPower = msg.reader().readLong();
				Char.myPetz().cTiemNang = msg.reader().readLong();
				Char.myPetz().petStatus = msg.reader().readByte();
				Char.myPetz().cStamina = msg.reader().readShort();
				Char.myPetz().cMaxStamina = msg.reader().readShort();
				Char.myPetz().cCriticalFull = msg.reader().readByte();
				Char.myPetz().cDefull = msg.reader().readLong();
				Char.myPetz().arrPetSkill = new Skill[msg.reader().readByte()];
				Res.outz("SKILLENT = " + Char.myPetz().arrPetSkill);
				for (int num50 = 0; num50 < Char.myPetz().arrPetSkill.Length; num50++)
				{
					short num51 = msg.reader().readShort();
					if (num51 != -1)
					{
						Char.myPetz().arrPetSkill[num50] = Skills.get(num51);
						continue;
					}
					Char.myPetz().arrPetSkill[num50] = new Skill();
					Char.myPetz().arrPetSkill[num50].template = null;
					Char.myPetz().arrPetSkill[num50].moreInfo = msg.reader().readUTF();
				}
				Char.myPetz().cGiamST = msg.reader().readByte();
				Char.myPetz().cCritDameFull = msg.reader().readShort();
				if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
				{
					GameCanvas.panel2 = new Panel();
					GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
					GameCanvas.panel2.setTypeBodyOnly();
					GameCanvas.panel2.show();
					GameCanvas.panel.setTypePetMain();
					GameCanvas.panel.show();
				}
				else
				{
					GameCanvas.panel.tabName[21] = mResources.petMainTab;
					GameCanvas.panel.setTypePetMain();
					GameCanvas.panel.show();
				}
				break;
			}
			case -37:
			{
				sbyte b17 = msg.reader().readByte();
				Res.outz("cAction= " + b17);
				if (b17 != 0)
				{
					break;
				}
				Char.myCharz().head = msg.reader().readShort();
				Char.myCharz().setDefaultPart();
				int num39 = msg.reader().readUnsignedByte();
				Res.outz("num body = " + num39);
				Char.myCharz().arrItemBody = new Item[num39];
				for (int num40 = 0; num40 < num39; num40++)
				{
					short num41 = msg.reader().readShort();
					if (num41 == -1)
					{
						continue;
					}
					Char.myCharz().arrItemBody[num40] = new Item();
					Char.myCharz().arrItemBody[num40].template = ItemTemplates.get(num41);
					int type = Char.myCharz().arrItemBody[num40].template.type;
					Char.myCharz().arrItemBody[num40].quantity = msg.reader().readInt();
					Char.myCharz().arrItemBody[num40].info = msg.reader().readUTF();
					Char.myCharz().arrItemBody[num40].content = msg.reader().readUTF();
					int num42 = msg.reader().readUnsignedByte();
					if (num42 != 0)
					{
						Char.myCharz().arrItemBody[num40].itemOption = new ItemOption[num42];
						for (int num43 = 0; num43 < Char.myCharz().arrItemBody[num40].itemOption.Length; num43++)
						{
							ItemOption itemOption2 = readItemOption(msg);
							if (itemOption2 != null)
							{
								Char.myCharz().arrItemBody[num40].itemOption[num43] = itemOption2;
							}
						}
					}
					switch (type)
					{
					case 0:
						Char.myCharz().body = Char.myCharz().arrItemBody[num40].template.part;
						break;
					case 1:
						Char.myCharz().leg = Char.myCharz().arrItemBody[num40].template.part;
						break;
					}
				}
				break;
			}
			case -36:
			{
				sbyte b8 = msg.reader().readByte();
				Res.outz("cAction= " + b8);
				GameScr.isudungCapsun4 = false;
				GameScr.isudungCapsun3 = false;
				if (b8 == 0)
				{
					int num9 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemBag = new Item[num9];
					GameScr.hpPotion = 0;
					Res.outz("numC=" + num9);
					for (int k = 0; k < num9; k++)
					{
						short num10 = msg.reader().readShort();
						if (num10 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBag[k] = new Item();
						Char.myCharz().arrItemBag[k].template = ItemTemplates.get(num10);
						Char.myCharz().arrItemBag[k].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBag[k].info = msg.reader().readUTF();
						Char.myCharz().arrItemBag[k].content = msg.reader().readUTF();
						Char.myCharz().arrItemBag[k].indexUI = k;
						int num11 = msg.reader().readUnsignedByte();
						if (num11 != 0)
						{
							Char.myCharz().arrItemBag[k].itemOption = new ItemOption[num11];
							for (int l = 0; l < Char.myCharz().arrItemBag[k].itemOption.Length; l++)
							{
								ItemOption itemOption = readItemOption(msg);
								if (itemOption != null)
								{
									Char.myCharz().arrItemBag[k].itemOption[l] = itemOption;
								}
							}
							Char.myCharz().arrItemBag[k].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemBag[k]);
						}
						if (Char.myCharz().arrItemBag[k].template.type == 11)
						{
						}
						if (Char.myCharz().arrItemBag[k].template.type == 6)
						{
							GameScr.hpPotion += Char.myCharz().arrItemBag[k].quantity;
						}
						if (Char.myCharz().arrItemBag[k].template.id == 194)
						{
							GameScr.isudungCapsun4 = Char.myCharz().arrItemBag[k].quantity > 0;
						}
						else if (Char.myCharz().arrItemBag[k].template.id == 193 && !GameScr.isudungCapsun4)
						{
							GameScr.isudungCapsun3 = Char.myCharz().arrItemBag[k].quantity > 0;
						}
					}
				}
				if (b8 == 2)
				{
					sbyte b9 = msg.reader().readByte();
					int num12 = msg.reader().readInt();
					int quantity = Char.myCharz().arrItemBag[b9].quantity;
					int id = Char.myCharz().arrItemBag[b9].template.id;
					Char.myCharz().arrItemBag[b9].quantity = num12;
					if (Char.myCharz().arrItemBag[b9].quantity < quantity && Char.myCharz().arrItemBag[b9].template.type == 6)
					{
						GameScr.hpPotion -= quantity - Char.myCharz().arrItemBag[b9].quantity;
					}
					if (Char.myCharz().arrItemBag[b9].quantity == 0)
					{
						Char.myCharz().arrItemBag[b9] = null;
					}
					switch (id)
					{
					case 194:
						GameScr.isudungCapsun4 = num12 > 0;
						break;
					case 193:
						GameScr.isudungCapsun3 = num12 > 0;
						break;
					}
				}
				break;
			}
			case -35:
			{
				sbyte b51 = msg.reader().readByte();
				Res.outz("cAction= " + b51);
				if (b51 == 0)
				{
					int num137 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemBox = new Item[num137];
					GameCanvas.panel.hasUse = 0;
					for (int num138 = 0; num138 < num137; num138++)
					{
						short num139 = msg.reader().readShort();
						if (num139 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBox[num138] = new Item();
						Char.myCharz().arrItemBox[num138].template = ItemTemplates.get(num139);
						Char.myCharz().arrItemBox[num138].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBox[num138].info = msg.reader().readUTF();
						Char.myCharz().arrItemBox[num138].content = msg.reader().readUTF();
						int num140 = msg.reader().readUnsignedByte();
						if (num140 != 0)
						{
							Char.myCharz().arrItemBox[num138].itemOption = new ItemOption[num140];
							for (int num141 = 0; num141 < Char.myCharz().arrItemBox[num138].itemOption.Length; num141++)
							{
								ItemOption itemOption6 = readItemOption(msg);
								if (itemOption6 != null)
								{
									Char.myCharz().arrItemBox[num138].itemOption[num141] = itemOption6;
								}
							}
						}
						Panel panel = GameCanvas.panel;
						panel.hasUse++;
					}
				}
				if (b51 == 1)
				{
					bool isBoxClan = false;
					try
					{
						sbyte b52 = msg.reader().readByte();
						if (b52 == 1)
						{
							isBoxClan = true;
						}
					}
					catch (Exception)
					{
					}
					GameCanvas.panel.setTypeBox();
					GameCanvas.panel.isBoxClan = isBoxClan;
					GameCanvas.panel.show();
				}
				if (b51 == 2)
				{
					sbyte b53 = msg.reader().readByte();
					int quantity2 = msg.reader().readInt();
					Char.myCharz().arrItemBox[b53].quantity = quantity2;
					if (Char.myCharz().arrItemBox[b53].quantity == 0)
					{
						Char.myCharz().arrItemBox[b53] = null;
					}
				}
				break;
			}
			case -45:
			{
				sbyte b44 = msg.reader().readByte();
				int num107 = msg.reader().readInt();
				short num108 = msg.reader().readShort();
				Res.outz(">.SKILL_NOT_FOCUS      skillNotFocusID: " + num108 + " skill type= " + b44 + "   player use= " + num107);
				if (b44 == 20)
				{
					sbyte typeFrame = msg.reader().readByte();
					sbyte dir = msg.reader().readByte();
					short timeGong = msg.reader().readShort();
					bool isFly = ((msg.reader().readByte() != 0) ? true : false);
					sbyte typePaint = msg.reader().readByte();
					sbyte typeItem = -1;
					try
					{
						typeItem = msg.reader().readByte();
					}
					catch (Exception)
					{
					}
					Res.outz(">.SKILL_NOT_FOCUS  skill typeFrame= " + typeFrame);
					obj = ((Char.myCharz().charID != num107) ? GameScr.findCharInMap(num107) : Char.myCharz());
					obj.SetSkillPaint_NEW(num108, isFly, typeFrame, typePaint, dir, timeGong, typeItem);
				}
				if (b44 == 21)
				{
					Point point = new Point();
					point.x = msg.reader().readShort();
					point.y = msg.reader().readShort();
					short timeDame = msg.reader().readShort();
					short rangeDame = msg.reader().readShort();
					sbyte typePaint2 = 0;
					sbyte typeItem2 = -1;
					Point[] array10 = null;
					obj = ((Char.myCharz().charID != num107) ? GameScr.findCharInMap(num107) : Char.myCharz());
					try
					{
						typePaint2 = msg.reader().readByte();
						sbyte b45 = msg.reader().readByte();
						if (b45 > 0)
						{
							array10 = new Point[b45];
							for (int num109 = 0; num109 < array10.Length; num109++)
							{
								array10[num109] = new Point();
								array10[num109].type = msg.reader().readByte();
								if (array10[num109].type == 0)
								{
									array10[num109].id = msg.reader().readByte();
								}
								else
								{
									array10[num109].id = msg.reader().readInt();
								}
							}
						}
					}
					catch (Exception)
					{
					}
					try
					{
						typeItem2 = msg.reader().readByte();
					}
					catch (Exception)
					{
					}
					Res.outz(">.SKILL_NOT_FOCUS  skill targetDame= " + point.x + ":" + point.y + "    c:" + obj.cx + ":" + obj.cy + "   cdir:" + obj.cdir);
					obj.SetSkillPaint_STT(1, num108, point, timeDame, rangeDame, typePaint2, array10, typeItem2);
				}
				if (b44 == 0)
				{
					Res.outz("id use= " + num107);
					if (Char.myCharz().charID != num107)
					{
						obj = GameScr.findCharInMap(num107);
						if ((TileMap.tileTypeAtPixel(obj.cx, obj.cy) & 2) == 2)
						{
							obj.setSkillPaint(GameScr.sks[num108], 0);
						}
						else
						{
							obj.setSkillPaint(GameScr.sks[num108], 1);
							obj.delayFall = 20;
						}
					}
					else
					{
						Char.myCharz().saveLoadPreviousSkill();
						Res.outz("LOAD LAST SKILL");
					}
					sbyte b46 = msg.reader().readByte();
					Res.outz("npc size= " + b46);
					for (int num110 = 0; num110 < b46; num110++)
					{
						sbyte index3 = msg.reader().readByte();
						sbyte seconds = msg.reader().readByte();
						Res.outz("index= " + index3);
						if (num108 >= 42 && num108 <= 48)
						{
							((Mob)GameScr.vMob.elementAt(index3)).isFreez = true;
							((Mob)GameScr.vMob.elementAt(index3)).seconds = seconds;
							((Mob)GameScr.vMob.elementAt(index3)).last = (((Mob)GameScr.vMob.elementAt(index3)).cur = mSystem.currentTimeMillis());
						}
					}
					sbyte b47 = msg.reader().readByte();
					for (int num111 = 0; num111 < b47; num111++)
					{
						int num112 = msg.reader().readInt();
						sbyte b48 = msg.reader().readByte();
						Res.outz("player ID= " + num112 + " my ID= " + Char.myCharz().charID);
						if (num108 < 42 || num108 > 48)
						{
							continue;
						}
						if (num112 == Char.myCharz().charID)
						{
							if (!Char.myCharz().isFlyAndCharge && !Char.myCharz().isStandAndCharge)
							{
								GameScr.gI().isFreez = true;
								Char.myCharz().isFreez = true;
								Char.myCharz().freezSeconds = b48;
								Char.myCharz().lastFreez = (Char.myCharz().currFreez = mSystem.currentTimeMillis());
								Char.myCharz().isLockMove = true;
							}
						}
						else
						{
							obj = GameScr.findCharInMap(num112);
							if (obj != null && !obj.isFlyAndCharge && !obj.isStandAndCharge)
							{
								obj.isFreez = true;
								obj.seconds = b48;
								obj.freezSeconds = b48;
								obj.lastFreez = (GameScr.findCharInMap(num112).currFreez = mSystem.currentTimeMillis());
							}
						}
					}
				}
				if (b44 == 1 && num107 != Char.myCharz().charID)
				{
					try
					{
						GameScr.findCharInMap(num107).isCharge = true;
					}
					catch (Exception)
					{
					}
				}
				if (b44 == 3)
				{
					if (num107 == Char.myCharz().charID)
					{
						Char.myCharz().isCharge = false;
						SoundMn.gI().taitaoPause();
						Char.myCharz().saveLoadPreviousSkill();
					}
					else
					{
						GameScr.findCharInMap(num107).isCharge = false;
					}
				}
				if (b44 == 4)
				{
					if (num107 == Char.myCharz().charID)
					{
						Char.myCharz().seconds = msg.reader().readShort() - 1000;
						Char.myCharz().last = mSystem.currentTimeMillis();
						Res.outz("second= " + Char.myCharz().seconds + " last= " + Char.myCharz().last);
					}
					else if (GameScr.findCharInMap(num107) != null)
					{
						Char obj10 = GameScr.findCharInMap(num107);
						switch (obj10.cgender)
						{
						case 0:
							if (TileMap.mapID != 170)
							{
								obj.useChargeSkill(isGround: false);
								break;
							}
							if (num108 >= 77 && num108 <= 83)
							{
								obj.useChargeSkill(isGround: true);
							}
							if (num108 >= 70 && num108 <= 76)
							{
								obj.useChargeSkill(isGround: false);
							}
							break;
						case 1:
						{
							if (TileMap.mapID != 170)
							{
								obj.useChargeSkill(isGround: true);
								break;
							}
							bool isGround2 = true;
							if (num108 >= 70 && num108 <= 76)
							{
								isGround2 = false;
							}
							if (num108 >= 77 && num108 <= 83)
							{
								isGround2 = true;
							}
							obj.useChargeSkill(isGround2);
							break;
						}
						default:
							if (TileMap.mapID == 170)
							{
								bool isGround = true;
								if (num108 >= 70 && num108 <= 76)
								{
									isGround = false;
								}
								if (num108 >= 77 && num108 <= 83)
								{
									isGround = true;
								}
								obj.useChargeSkill(isGround);
							}
							break;
						}
						obj.skillTemplateId = num108;
						if (num108 >= 70 && num108 <= 76)
						{
							obj.isUseSkillAfterCharge = true;
						}
						obj.seconds = msg.reader().readShort();
						obj.last = mSystem.currentTimeMillis();
					}
				}
				if (b44 == 5)
				{
					if (num107 == Char.myCharz().charID)
					{
						Char.myCharz().stopUseChargeSkill();
					}
					else if (GameScr.findCharInMap(num107) != null)
					{
						GameScr.findCharInMap(num107).stopUseChargeSkill();
					}
				}
				if (b44 == 6)
				{
					if (num107 == Char.myCharz().charID)
					{
						Char.myCharz().setAutoSkillPaint(GameScr.sks[num108], 0);
					}
					else if (GameScr.findCharInMap(num107) != null)
					{
						GameScr.findCharInMap(num107).setAutoSkillPaint(GameScr.sks[num108], 0);
						SoundMn.gI().gong();
					}
				}
				if (b44 == 7)
				{
					if (num107 == Char.myCharz().charID)
					{
						Char.myCharz().seconds = msg.reader().readShort();
						Res.outz("second = " + Char.myCharz().seconds);
						Char.myCharz().last = mSystem.currentTimeMillis();
					}
					else if (GameScr.findCharInMap(num107) != null)
					{
						GameScr.findCharInMap(num107).useChargeSkill(isGround: true);
						GameScr.findCharInMap(num107).seconds = msg.reader().readShort();
						GameScr.findCharInMap(num107).last = mSystem.currentTimeMillis();
						SoundMn.gI().gong();
					}
				}
				if (b44 == 8 && num107 != Char.myCharz().charID && GameScr.findCharInMap(num107) != null)
				{
					GameScr.findCharInMap(num107).setAutoSkillPaint(GameScr.sks[num108], 0);
				}
				break;
			}
			case -44:
			{
				bool flag6 = false;
				if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
				{
					flag6 = true;
				}
				sbyte b21 = msg.reader().readByte();
				int num53 = msg.reader().readUnsignedByte();
				Char.myCharz().arrItemShop = new Item[num53][];
				GameCanvas.panel.shopTabName = new string[num53 + ((!flag6) ? 1 : 0)][];
				for (int num54 = 0; num54 < GameCanvas.panel.shopTabName.Length; num54++)
				{
					GameCanvas.panel.shopTabName[num54] = new string[2];
				}
				if (b21 == 2)
				{
					GameCanvas.panel.maxPageShop = new int[num53];
					GameCanvas.panel.currPageShop = new int[num53];
				}
				if (!flag6)
				{
					GameCanvas.panel.shopTabName[num53] = mResources.inventory;
				}
				for (int num55 = 0; num55 < num53; num55++)
				{
					string[] array5 = Res.split(msg.reader().readUTF(), "\n", 0);
					if (b21 == 2)
					{
						GameCanvas.panel.maxPageShop[num55] = msg.reader().readUnsignedByte();
					}
					if (array5.Length == 2)
					{
						GameCanvas.panel.shopTabName[num55] = array5;
					}
					if (array5.Length == 1)
					{
						GameCanvas.panel.shopTabName[num55][0] = array5[0];
						GameCanvas.panel.shopTabName[num55][1] = string.Empty;
					}
					int num56 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemShop[num55] = new Item[num56];
					Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy;
					if (b21 == 1)
					{
						Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy2;
					}
					for (int num57 = 0; num57 < num56; num57++)
					{
						short num58 = msg.reader().readShort();
						if (num58 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemShop[num55][num57] = new Item();
						Char.myCharz().arrItemShop[num55][num57].template = ItemTemplates.get(num58);
						switch (b21)
						{
						case 8:
							Char.myCharz().arrItemShop[num55][num57].buyCoin = msg.reader().readInt();
							Char.myCharz().arrItemShop[num55][num57].buyGold = msg.reader().readInt();
							Char.myCharz().arrItemShop[num55][num57].quantity = msg.reader().readInt();
							break;
						case 4:
							Char.myCharz().arrItemShop[num55][num57].reason = msg.reader().readUTF();
							break;
						case 0:
							Char.myCharz().arrItemShop[num55][num57].buyCoin = msg.reader().readInt();
							Char.myCharz().arrItemShop[num55][num57].buyGold = msg.reader().readInt();
							break;
						case 1:
							Char.myCharz().arrItemShop[num55][num57].powerRequire = msg.reader().readLong();
							break;
						case 2:
							Char.myCharz().arrItemShop[num55][num57].itemId = msg.reader().readShort();
							Char.myCharz().arrItemShop[num55][num57].buyCoin = msg.reader().readInt();
							Char.myCharz().arrItemShop[num55][num57].buyGold = msg.reader().readInt();
							Char.myCharz().arrItemShop[num55][num57].buyType = msg.reader().readByte();
							Char.myCharz().arrItemShop[num55][num57].quantity = msg.reader().readInt();
							Char.myCharz().arrItemShop[num55][num57].isMe = msg.reader().readByte();
							break;
						case 3:
							Char.myCharz().arrItemShop[num55][num57].isBuySpec = true;
							Char.myCharz().arrItemShop[num55][num57].iconSpec = msg.reader().readShort();
							Char.myCharz().arrItemShop[num55][num57].buySpec = msg.reader().readInt();
							break;
						}
						int num59 = msg.reader().readUnsignedByte();
						if (num59 != 0)
						{
							Char.myCharz().arrItemShop[num55][num57].itemOption = new ItemOption[num59];
							for (int num60 = 0; num60 < Char.myCharz().arrItemShop[num55][num57].itemOption.Length; num60++)
							{
								ItemOption itemOption4 = readItemOption(msg);
								if (itemOption4 != null)
								{
									Char.myCharz().arrItemShop[num55][num57].itemOption[num60] = itemOption4;
									Char.myCharz().arrItemShop[num55][num57].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemShop[num55][num57]);
								}
							}
						}
						sbyte b22 = msg.reader().readByte();
						Char.myCharz().arrItemShop[num55][num57].newItem = ((b22 != 0) ? true : false);
						sbyte b23 = msg.reader().readByte();
						if (b23 == 1)
						{
							int headTemp = msg.reader().readShort();
							int bodyTemp = msg.reader().readShort();
							int legTemp = msg.reader().readShort();
							int bagTemp = msg.reader().readShort();
							Char.myCharz().arrItemShop[num55][num57].setPartTemp(headTemp, bodyTemp, legTemp, bagTemp);
						}
						if (b21 == 2 && GameMidlet.intVERSION >= 237)
						{
							Char.myCharz().arrItemShop[num55][num57].nameNguoiKyGui = msg.reader().readUTF();
							Res.err("nguoi ki gui  " + Char.myCharz().arrItemShop[num55][num57].nameNguoiKyGui);
						}
					}
				}
				if (flag6)
				{
					if (b21 != 2)
					{
						GameCanvas.panel2 = new Panel();
						GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
						GameCanvas.panel2.setTypeBodyOnly();
						GameCanvas.panel2.show();
					}
					else
					{
						GameCanvas.panel2 = new Panel();
						GameCanvas.panel2.setTypeKiGuiOnly();
						GameCanvas.panel2.show();
					}
				}
				GameCanvas.panel.tabName[1] = GameCanvas.panel.shopTabName;
				if (b21 == 2)
				{
					string[][] array6 = GameCanvas.panel.tabName[1];
					if (flag6)
					{
						GameCanvas.panel.tabName[1] = new string[4][]
						{
							array6[0],
							array6[1],
							array6[2],
							array6[3]
						};
					}
					else
					{
						GameCanvas.panel.tabName[1] = new string[5][]
						{
							array6[0],
							array6[1],
							array6[2],
							array6[3],
							array6[4]
						};
					}
				}
				GameCanvas.panel.setTypeShop(b21);
				GameCanvas.panel.show();
				break;
			}
			case -41:
			{
				sbyte b20 = msg.reader().readByte();
				Char.myCharz().strLevel = new string[b20];
				for (int num52 = 0; num52 < b20; num52++)
				{
					string text4 = msg.reader().readUTF();
					Char.myCharz().strLevel[num52] = text4;
				}
				Res.outz("---   xong  level caption cmd : " + msg.command);
				break;
			}
			case -34:
			{
				sbyte b12 = msg.reader().readByte();
				Res.outz("act= " + b12);
				if (b12 == 0 && GameScr.gI().magicTree != null)
				{
					Res.outz("toi duoc day");
					MagicTree magicTree = GameScr.gI().magicTree;
					magicTree.id = msg.reader().readShort();
					magicTree.name = msg.reader().readUTF();
					magicTree.name = Res.changeString(magicTree.name);
					magicTree.x = msg.reader().readShort();
					magicTree.y = msg.reader().readShort();
					magicTree.level = msg.reader().readByte();
					magicTree.currPeas = msg.reader().readShort();
					magicTree.maxPeas = msg.reader().readShort();
					Res.outz("curr Peas= " + magicTree.currPeas);
					magicTree.strInfo = msg.reader().readUTF();
					magicTree.seconds = msg.reader().readInt();
					magicTree.timeToRecieve = magicTree.seconds;
					sbyte b13 = msg.reader().readByte();
					magicTree.peaPostionX = new int[b13];
					magicTree.peaPostionY = new int[b13];
					for (int num24 = 0; num24 < b13; num24++)
					{
						magicTree.peaPostionX[num24] = msg.reader().readByte();
						magicTree.peaPostionY[num24] = msg.reader().readByte();
					}
					magicTree.isUpdate = msg.reader().readBool();
					magicTree.last = (magicTree.cur = mSystem.currentTimeMillis());
					GameScr.gI().magicTree.isUpdateTree = true;
				}
				if (b12 == 1)
				{
					myVector = new MyVector();
					try
					{
						while (msg.reader().available() > 0)
						{
							string caption = msg.reader().readUTF();
							myVector.addElement(new Command(caption, GameCanvas.instance, 888392, null));
						}
					}
					catch (Exception ex5)
					{
						Cout.println("Loi MAGIC_TREE " + ex5.ToString());
					}
					GameCanvas.menu.startAt(myVector, 3);
				}
				if (b12 == 2)
				{
					GameScr.gI().magicTree.remainPeas = msg.reader().readShort();
					GameScr.gI().magicTree.seconds = msg.reader().readInt();
					GameScr.gI().magicTree.last = (GameScr.gI().magicTree.cur = mSystem.currentTimeMillis());
					GameScr.gI().magicTree.isUpdateTree = true;
					GameScr.gI().magicTree.isPeasEffect = true;
				}
				break;
			}
			case 11:
			{
				GameCanvas.debug("SA9", 2);
				int num162 = msg.reader().readShort();
				sbyte b63 = msg.reader().readByte();
				if (b63 != 0)
				{
					Mob.arrMobTemplate[num162].data.readDataNewBoss(NinjaUtil.readByteArray(msg), b63);
				}
				else
				{
					Mob.arrMobTemplate[num162].data.readData(NinjaUtil.readByteArray(msg));
				}
				for (int num163 = 0; num163 < GameScr.vMob.size(); num163++)
				{
					mob = (Mob)GameScr.vMob.elementAt(num163);
					if (mob.templateId == num162)
					{
						mob.w = Mob.arrMobTemplate[num162].data.width;
						mob.h = Mob.arrMobTemplate[num162].data.height;
					}
				}
				sbyte[] array18 = NinjaUtil.readByteArray(msg);
				Image img = Image.createImage(array18, 0, array18.Length);
				Mob.arrMobTemplate[num162].data.img = img;
				int num164 = msg.reader().readByte();
				Mob.arrMobTemplate[num162].data.typeData = num164;
				if (num164 == 1 || num164 == 2)
				{
					readFrameBoss(msg, num162);
				}
				break;
			}
			case -69:
				Char.myCharz().cMaxStamina = msg.reader().readShort();
				break;
			case -68:
				Char.myCharz().cStamina = msg.reader().readShort();
				break;
			case -67:
			{
				demCount += 1f;
				int num155 = msg.reader().readInt();
				Res.outz("RECIEVE  hinh small: " + num155);
				sbyte[] array16 = null;
				try
				{
					array16 = NinjaUtil.readByteArray(msg);
					Res.outz(">SIZE CHECK= " + array16.Length);
					if (num155 == 3896)
					{
					}
					SmallImage.imgNew[num155].img = createImage(array16);
				}
				catch (Exception)
				{
					array16 = null;
					SmallImage.imgNew[num155].img = Image.createRGBImage(new int[1], 1, 1, bl: true);
				}
				if (array16 != null && mGraphics.zoomLevel > 1)
				{
					Rms.saveRMS(mGraphics.zoomLevel + "Small" + num155, array16);
				}
				break;
			}
			case -66:
			{
				short id5 = msg.reader().readShort();
				sbyte[] data3 = NinjaUtil.readByteArray(msg);
				EffectData effDataById = Effect.getEffDataById(id5);
				sbyte b59 = msg.reader().readSByte();
				if (b59 == 0)
				{
					effDataById.readData(data3);
				}
				else
				{
					effDataById.readDataNewBoss(data3, b59);
				}
				sbyte[] array15 = NinjaUtil.readByteArray(msg);
				effDataById.img = Image.createImage(array15, 0, array15.Length);
				break;
			}
			case -32:
			{
				short id4 = msg.reader().readShort();
				int num133 = msg.reader().readInt();
				sbyte[] array11 = null;
				Image image = null;
				try
				{
					array11 = new sbyte[num133];
					for (int num134 = 0; num134 < num133; num134++)
					{
						array11[num134] = msg.reader().readByte();
					}
					image = Image.createImage(array11, 0, num133);
					BgItem.imgNew.put(id4 + string.Empty, image);
				}
				catch (Exception)
				{
					array11 = null;
					BgItem.imgNew.put(id4 + string.Empty, Image.createRGBImage(new int[1], 1, 1, bl: true));
				}
				if (array11 != null)
				{
					if (mGraphics.zoomLevel > 1)
					{
						Rms.saveRMS(mGraphics.zoomLevel + "bgItem" + id4, array11);
					}
					BgItemMn.blendcurrBg(id4, image);
				}
				break;
			}
			case 92:
			{
				if (GameCanvas.currentScreen == GameScr.instance)
				{
					GameCanvas.endDlg();
				}
				string text6 = msg.reader().readUTF();
				string str2 = msg.reader().readUTF();
				str2 = Res.changeString(str2);
				string empty = string.Empty;
				Char obj9 = null;
				sbyte b42 = 0;
				if (!text6.Equals(string.Empty))
				{
					obj9 = new Char();
					obj9.charID = msg.reader().readInt();
					obj9.head = msg.reader().readShort();
					obj9.headICON = msg.reader().readShort();
					obj9.body = msg.reader().readShort();
					obj9.bag = msg.reader().readShort();
					obj9.leg = msg.reader().readShort();
					b42 = msg.reader().readByte();
					obj9.cName = text6;
				}
				empty += str2;
				InfoDlg.hide();
				if (text6.Equals(string.Empty))
				{
					GameScr.info1.addInfo(empty, 0);
					break;
				}
				GameScr.info2.addInfoWithChar(empty, obj9, b42 == 0);
				if (GameCanvas.panel.isShow && GameCanvas.panel.type == 8)
				{
					GameCanvas.panel.initLogMessage();
				}
				break;
			}
			case -26:
				ServerListScreen.testConnect = 2;
				GameCanvas.debug("SA2", 2);
				GameCanvas.startOKDlg(msg.reader().readUTF());
				InfoDlg.hide();
				LoginScr.isContinueToLogin = false;
				Char.isLoadingMap = false;
				if (GameCanvas.currentScreen == GameCanvas.loginScr)
				{
					GameCanvas.serverScreen.switchToMe();
				}
				break;
			case -25:
				GameCanvas.debug("SA3", 2);
				GameScr.info1.addInfo(msg.reader().readUTF(), 0);
				break;
			case 94:
				GameCanvas.debug("SA3", 2);
				GameScr.info1.addInfo(msg.reader().readUTF(), 0);
				break;
			case 47:
				GameCanvas.debug("SA4", 2);
				GameScr.gI().resetButton();
				break;
			case 81:
			{
				GameCanvas.debug("SXX4", 2);
				Mob mob8 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob8.isDisable = msg.reader().readBool();
				break;
			}
			case 82:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob7.isDontMove = msg.reader().readBool();
				break;
			}
			case 85:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob6 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob6.isFire = msg.reader().readBool();
				break;
			}
			case 86:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob5 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob5.isIce = msg.reader().readBool();
				if (!mob5.isIce)
				{
					ServerEffect.addServerEffect(77, mob5.x, mob5.y - 9, 1);
				}
				break;
			}
			case 87:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob4 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				mob4.isWind = msg.reader().readBool();
				break;
			}
			case 56:
			{
				GameCanvas.debug("SXX6", 2);
				obj = null;
				int num34 = msg.reader().readInt();
				if (num34 == Char.myCharz().charID)
				{
					bool flag4 = false;
					obj = Char.myCharz();
					obj.cHP = msg.reader().readLong();
					long num35 = msg.reader().readLong();
					Res.outz("dame hit = " + num35);
					if (num35 != 0)
					{
						obj.doInjure();
					}
					int num36 = 0;
					try
					{
						flag4 = msg.reader().readBoolean();
						sbyte b15 = msg.reader().readByte();
						if (b15 != -1)
						{
							Res.outz("hit eff= " + b15);
							EffecMn.addEff(new Effect(b15, obj.cx, obj.cy, 3, 1, -1));
						}
					}
					catch (Exception)
					{
					}
					num35 += num36;
					if (Char.myCharz().cTypePk != 4)
					{
						if (num35 == 0)
						{
							GameScr.startFlyText(mResources.miss, obj.cx, obj.cy - obj.ch, 0, -3, mFont.MISS_ME);
						}
						else
						{
							GameScr.startFlyText("-" + num35, obj.cx, obj.cy - obj.ch, 0, -3, flag4 ? mFont.FATAL : mFont.RED);
						}
					}
					break;
				}
				obj = GameScr.findCharInMap(num34);
				if (obj == null)
				{
					return;
				}
				obj.cHP = msg.reader().readLong();
				bool flag5 = false;
				long num37 = msg.reader().readLong();
				if (num37 != 0)
				{
					obj.doInjure();
				}
				int num38 = 0;
				try
				{
					flag5 = msg.reader().readBoolean();
					sbyte b16 = msg.reader().readByte();
					if (b16 != -1)
					{
						Res.outz("hit eff= " + b16);
						EffecMn.addEff(new Effect(b16, obj.cx, obj.cy, 3, 1, -1));
					}
				}
				catch (Exception)
				{
				}
				num37 += num38;
				if (obj.cTypePk != 4)
				{
					if (num37 == 0)
					{
						GameScr.startFlyText(mResources.miss, obj.cx, obj.cy - obj.ch, 0, -3, mFont.MISS);
					}
					else
					{
						GameScr.startFlyText("-" + num37, obj.cx, obj.cy - obj.ch, 0, -3, flag5 ? mFont.FATAL : mFont.ORANGE);
					}
				}
				break;
			}
			case 83:
			{
				GameCanvas.debug("SXX8", 2);
				int num25 = msg.reader().readInt();
				obj = ((num25 != Char.myCharz().charID) ? GameScr.findCharInMap(num25) : Char.myCharz());
				if (obj == null)
				{
					return;
				}
				Mob mobToAttack = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				if (obj.mobMe != null)
				{
					obj.mobMe.attackOtherMob(mobToAttack);
				}
				break;
			}
			case 84:
			{
				int num23 = msg.reader().readInt();
				if (num23 == Char.myCharz().charID)
				{
					obj = Char.myCharz();
				}
				else
				{
					obj = GameScr.findCharInMap(num23);
					if (obj == null)
					{
						return;
					}
				}
				obj.cHP = obj.cHPFull;
				obj.cMP = obj.cMPFull;
				obj.cx = msg.reader().readShort();
				obj.cy = msg.reader().readShort();
				obj.liveFromDead();
				break;
			}
			case 46:
				GameCanvas.debug("SA5", 2);
				Cout.LogWarning("Controler RESET_POINT  " + Char.ischangingMap);
				Char.isLockKey = false;
				Char.myCharz().setResetPoint(msg.reader().readShort(), msg.reader().readShort());
				break;
			case -29:
				messageNotLogin(msg);
				break;
			case -28:
				messageNotMap(msg);
				break;
			case -30:
				messageSubCommand(msg);
				break;
			case 62:
				GameCanvas.debug("SZ3", 2);
				obj = GameScr.findCharInMap(msg.reader().readInt());
				if (obj != null)
				{
					obj.killCharId = Char.myCharz().charID;
					Char.myCharz().npcFocus = null;
					Char.myCharz().mobFocus = null;
					Char.myCharz().itemFocus = null;
					Char.myCharz().charFocus = obj;
					Char.isManualFocus = true;
					GameScr.info1.addInfo(obj.cName + mResources.CUU_SAT, 0);
				}
				break;
			case 63:
				GameCanvas.debug("SZ4", 2);
				Char.myCharz().killCharId = msg.reader().readInt();
				Char.myCharz().npcFocus = null;
				Char.myCharz().mobFocus = null;
				Char.myCharz().itemFocus = null;
				Char.myCharz().charFocus = GameScr.findCharInMap(Char.myCharz().killCharId);
				Char.isManualFocus = true;
				break;
			case 64:
				GameCanvas.debug("SZ5", 2);
				obj = Char.myCharz();
				try
				{
					obj = GameScr.findCharInMap(msg.reader().readInt());
				}
				catch (Exception ex3)
				{
					Cout.println("Loi CLEAR_CUU_SAT " + ex3.ToString());
				}
				obj.killCharId = -9999;
				break;
			case 39:
				GameCanvas.debug("SA49", 2);
				GameScr.gI().typeTradeOrder = 2;
				if (GameScr.gI().typeTrade >= 2 && GameScr.gI().typeTradeOrder >= 2)
				{
					InfoDlg.showWait();
				}
				break;
			case 57:
			{
				GameCanvas.debug("SZ6", 2);
				MyVector myVector2 = new MyVector();
				myVector2.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88817, null));
				GameCanvas.menu.startAt(myVector2, 3);
				break;
			}
			case 58:
			{
				GameCanvas.debug("SZ7", 2);
				int num166 = msg.reader().readInt();
				Char obj12 = ((num166 != Char.myCharz().charID) ? GameScr.findCharInMap(num166) : Char.myCharz());
				obj12.moveFast = new short[3];
				obj12.moveFast[0] = 0;
				short num167 = msg.reader().readShort();
				short num168 = msg.reader().readShort();
				obj12.moveFast[1] = num167;
				obj12.moveFast[2] = num168;
				try
				{
					num166 = msg.reader().readInt();
					Char obj13 = ((num166 != Char.myCharz().charID) ? GameScr.findCharInMap(num166) : Char.myCharz());
					obj13.cx = num167;
					obj13.cy = num168;
				}
				catch (Exception ex26)
				{
					Cout.println("Loi MOVE_FAST " + ex26.ToString());
				}
				break;
			}
			case 88:
			{
				string info4 = msg.reader().readUTF();
				short num165 = msg.reader().readShort();
				GameCanvas.inputDlg.show(info4, new Command(mResources.ACCEPT, GameCanvas.instance, 88818, num165), TField.INPUT_TYPE_ANY);
				break;
			}
			case 27:
			{
				myVector = new MyVector();
				string text8 = msg.reader().readUTF();
				int num159 = msg.reader().readByte();
				for (int num160 = 0; num160 < num159; num160++)
				{
					string caption4 = msg.reader().readUTF();
					short num161 = msg.reader().readShort();
					myVector.addElement(new Command(caption4, GameCanvas.instance, 88819, num161));
				}
				GameCanvas.menu.startWithoutCloseButton(myVector, 3);
				break;
			}
			case 33:
			{
				GameCanvas.debug("SA51", 2);
				InfoDlg.hide();
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				myVector = new MyVector();
				try
				{
					while (true)
					{
						string caption3 = msg.reader().readUTF();
						myVector.addElement(new Command(caption3, GameCanvas.instance, 88822, null));
					}
				}
				catch (Exception ex25)
				{
					Cout.println("Loi OPEN_UI_MENU " + ex25.ToString());
				}
				if (Char.myCharz().npcFocus == null)
				{
					return;
				}
				for (int num158 = 0; num158 < Char.myCharz().npcFocus.template.menu.Length; num158++)
				{
					string[] array17 = Char.myCharz().npcFocus.template.menu[num158];
					myVector.addElement(new Command(array17[0], GameCanvas.instance, 88820, array17));
				}
				GameCanvas.menu.startAt(myVector, 3);
				break;
			}
			case 40:
			{
				GameCanvas.debug("SA52", 2);
				GameCanvas.taskTick = 150;
				short taskId = msg.reader().readShort();
				sbyte index4 = msg.reader().readByte();
				string str3 = msg.reader().readUTF();
				str3 = Res.changeString(str3);
				string str4 = msg.reader().readUTF();
				str4 = Res.changeString(str4);
				string[] array12 = new string[msg.reader().readByte()];
				string[] array13 = new string[array12.Length];
				GameScr.tasks = new int[array12.Length];
				GameScr.mapTasks = new int[array12.Length];
				short[] array14 = new short[array12.Length];
				short count = -1;
				for (int num151 = 0; num151 < array12.Length; num151++)
				{
					string str5 = msg.reader().readUTF();
					str5 = Res.changeString(str5);
					GameScr.tasks[num151] = msg.reader().readByte();
					GameScr.mapTasks[num151] = msg.reader().readShort();
					string str6 = msg.reader().readUTF();
					str6 = Res.changeString(str6);
					array14[num151] = -1;
					array12[num151] = str5;
					if (!str6.Equals(string.Empty))
					{
						array13[num151] = str6;
					}
				}
				try
				{
					count = msg.reader().readShort();
					Cout.println(" TASK_GET count:" + count);
					for (int num152 = 0; num152 < array12.Length; num152++)
					{
						array14[num152] = msg.reader().readShort();
						Cout.println(num152 + " i TASK_GET   counts[i]:" + array14[num152]);
					}
				}
				catch (Exception ex23)
				{
					Cout.println("Loi TASK_GET " + ex23.ToString());
				}
				Char.myCharz().taskMaint = new Task(taskId, index4, str3, str4, array12, array14, count, array13);
				if (Char.myCharz().npcFocus != null)
				{
					Npc.clearEffTask();
				}
				Char.taskAction(isNextStep: true);
				break;
			}
			case 41:
			{
				GameCanvas.debug("SA53", 2);
				GameCanvas.taskTick = 100;
				Res.outz("TASK NEXT");
				Task taskMaint = Char.myCharz().taskMaint;
				taskMaint.index++;
				Char.myCharz().taskMaint.count = 0;
				Npc.clearEffTask();
				Char.taskAction(isNextStep: true);
				break;
			}
			case 50:
			{
				sbyte b57 = msg.reader().readByte();
				Panel.vGameInfo.removeAllElements();
				for (int num145 = 0; num145 < b57; num145++)
				{
					GameInfo gameInfo = new GameInfo();
					gameInfo.id = msg.reader().readShort();
					gameInfo.main = msg.reader().readUTF();
					gameInfo.content = msg.reader().readUTF();
					Panel.vGameInfo.addElement(gameInfo);
					bool hasRead = Rms.loadRMSInt(gameInfo.id + string.Empty) != -1;
					gameInfo.hasRead = hasRead;
				}
				break;
			}
			case 43:
				GameCanvas.taskTick = 50;
				GameCanvas.debug("SA55", 2);
				Char.myCharz().taskMaint.count = msg.reader().readShort();
				if (Char.myCharz().npcFocus != null)
				{
					Npc.clearEffTask();
				}
				try
				{
					short x_hint = msg.reader().readShort();
					short y_hint = msg.reader().readShort();
					Char.myCharz().x_hint = x_hint;
					Char.myCharz().y_hint = y_hint;
				}
				catch (Exception)
				{
				}
				break;
			case 90:
				GameCanvas.debug("SA577", 2);
				requestItemPlayer(msg);
				break;
			case 29:
				GameCanvas.debug("SA58", 2);
				GameScr.gI().openUIZone(msg);
				break;
			case -21:
			{
				GameCanvas.debug("SA60", 2);
				short num135 = msg.reader().readShort();
				for (int num136 = 0; num136 < GameScr.vItemMap.size(); num136++)
				{
					if (((ItemMap)GameScr.vItemMap.elementAt(num136)).itemMapID == num135)
					{
						GameScr.vItemMap.removeElementAt(num136);
						break;
					}
				}
				break;
			}
			case -20:
			{
				GameCanvas.debug("SA61", 2);
				Char.myCharz().itemFocus = null;
				short num131 = msg.reader().readShort();
				for (int num132 = 0; num132 < GameScr.vItemMap.size(); num132++)
				{
					ItemMap itemMap4 = (ItemMap)GameScr.vItemMap.elementAt(num132);
					if (itemMap4.itemMapID != num131)
					{
						continue;
					}
					itemMap4.setPoint(Char.myCharz().cx, Char.myCharz().cy - 10);
					string text7 = msg.reader().readUTF();
					num = 0;
					try
					{
						num = msg.reader().readShort();
						if (itemMap4.template.type == 9)
						{
							num = msg.reader().readShort();
							Char obj11 = Char.myCharz();
							obj11.xu += num;
							Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
						}
						else if (itemMap4.template.type == 10)
						{
							num = msg.reader().readShort();
							Char obj11 = Char.myCharz();
							obj11.luong += num;
							Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
						}
						else if (itemMap4.template.type == 34)
						{
							num = msg.reader().readShort();
							Char obj11 = Char.myCharz();
							obj11.luongKhoa += num;
							Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
						}
					}
					catch (Exception)
					{
					}
					if (text7.Equals(string.Empty))
					{
						if (itemMap4.template.type == 9)
						{
							GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.YELLOW);
							SoundMn.gI().getItem();
						}
						else if (itemMap4.template.type == 10)
						{
							GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.GREEN);
							SoundMn.gI().getItem();
						}
						else if (itemMap4.template.type == 34)
						{
							GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.RED);
							SoundMn.gI().getItem();
						}
						else
						{
							GameScr.info1.addInfo(mResources.you_receive + " " + ((num <= 0) ? string.Empty : (num + " ")) + itemMap4.template.name, 0);
							SoundMn.gI().getItem();
						}
						if (num > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 4683)
						{
							ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
							ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
						}
					}
					else if (text7.Length == 1)
					{
						Cout.LogError3("strInf.Length =1:  " + text7);
					}
					else
					{
						GameScr.info1.addInfo(text7, 0);
					}
					break;
				}
				break;
			}
			case -19:
			{
				GameCanvas.debug("SA62", 2);
				short num128 = msg.reader().readShort();
				obj = GameScr.findCharInMap(msg.reader().readInt());
				for (int num129 = 0; num129 < GameScr.vItemMap.size(); num129++)
				{
					ItemMap itemMap3 = (ItemMap)GameScr.vItemMap.elementAt(num129);
					if (itemMap3.itemMapID != num128)
					{
						continue;
					}
					if (obj == null)
					{
						return;
					}
					itemMap3.setPoint(obj.cx, obj.cy - 10);
					if (itemMap3.x < obj.cx)
					{
						obj.cdir = -1;
					}
					else if (itemMap3.x > obj.cx)
					{
						obj.cdir = 1;
					}
					break;
				}
				break;
			}
			case -18:
			{
				GameCanvas.debug("SA63", 2);
				int num127 = msg.reader().readByte();
				GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), Char.myCharz().arrItemBag[num127].template.id, Char.myCharz().cx, Char.myCharz().cy, msg.reader().readShort(), msg.reader().readShort()));
				Char.myCharz().arrItemBag[num127] = null;
				break;
			}
			case 68:
			{
				Res.outz("ADD ITEM TO MAP --------------------------------------");
				GameCanvas.debug("SA6333", 2);
				short itemMapID = msg.reader().readShort();
				short itemTemplateID = msg.reader().readShort();
				int x = msg.reader().readShort();
				int y = msg.reader().readShort();
				int num105 = msg.reader().readInt();
				short r = 0;
				if (num105 == -2)
				{
					r = msg.reader().readShort();
				}
				ItemMap itemMap = new ItemMap(num105, itemMapID, itemTemplateID, x, y, r);
				bool flag9 = false;
				for (int num106 = 0; num106 < GameScr.vItemMap.size(); num106++)
				{
					ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(num106);
					if (itemMap2.itemMapID == itemMap.itemMapID)
					{
						flag9 = true;
						break;
					}
				}
				if (!flag9)
				{
					GameScr.vItemMap.addElement(itemMap);
				}
				break;
			}
			case 69:
				SoundMn.IsDelAcc = ((msg.reader().readByte() != 0) ? true : false);
				break;
			case -14:
				GameCanvas.debug("SA64", 2);
				obj = GameScr.findCharInMap(msg.reader().readInt());
				if (obj == null)
				{
					return;
				}
				GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), msg.reader().readShort(), obj.cx, obj.cy, msg.reader().readShort(), msg.reader().readShort()));
				break;
			case -22:
				GameCanvas.debug("SA65", 2);
				Char.isLockKey = true;
				Char.ischangingMap = true;
				GameScr.gI().timeStartMap = 0;
				GameScr.gI().timeLengthMap = 0;
				Char.myCharz().mobFocus = null;
				Char.myCharz().npcFocus = null;
				Char.myCharz().charFocus = null;
				Char.myCharz().itemFocus = null;
				Char.myCharz().focus.removeAllElements();
				Char.myCharz().testCharId = -9999;
				Char.myCharz().killCharId = -9999;
				GameCanvas.resetBg();
				GameScr.gI().resetButton();
				GameScr.gI().center = null;
				if (Effect.vEffData.size() > 15)
				{
					for (int num98 = 0; num98 < 5; num98++)
					{
						Effect.vEffData.removeElementAt(0);
					}
				}
				break;
			case -70:
			{
				Res.outz("BIG MESSAGE .......................................");
				GameCanvas.endDlg();
				int avatar2 = msg.reader().readShort();
				string chat3 = msg.reader().readUTF();
				Npc npc5 = new Npc(-1, 0, 0, 0, 0, 0);
				npc5.avatar = avatar2;
				ChatPopup.addBigMessage(chat3, 100000, npc5);
				sbyte b38 = msg.reader().readByte();
				if (b38 == 0)
				{
					ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
					ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 35;
					ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
				}
				if (b38 == 1)
				{
					string p2 = msg.reader().readUTF();
					string caption2 = msg.reader().readUTF();
					ChatPopup.serverChatPopUp.cmdMsg1 = new Command(caption2, ChatPopup.serverChatPopUp, 1000, p2);
					ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 75;
					ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
					ChatPopup.serverChatPopUp.cmdMsg2 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
					ChatPopup.serverChatPopUp.cmdMsg2.x = GameCanvas.w / 2 + 11;
					ChatPopup.serverChatPopUp.cmdMsg2.y = GameCanvas.h - 35;
				}
				break;
			}
			case 38:
			{
				GameCanvas.debug("SA67", 2);
				InfoDlg.hide();
				int num96 = msg.reader().readShort();
				Res.outz("OPEN_UI_SAY ID= " + num96);
				string str = msg.reader().readUTF();
				str = Res.changeString(str);
				for (int num97 = 0; num97 < GameScr.vNpc.size(); num97++)
				{
					Npc npc3 = (Npc)GameScr.vNpc.elementAt(num97);
					Res.outz("npc id= " + npc3.template.npcTemplateId);
					if (npc3.template.npcTemplateId == num96)
					{
						ChatPopup.addChatPopupMultiLine(str, 100000, npc3);
						GameCanvas.panel.hideNow();
						return;
					}
				}
				Npc npc4 = new Npc(num96, 0, 0, 0, num96, GameScr.info1.charId[Char.myCharz().cgender][2]);
				if (npc4.template.npcTemplateId == 5)
				{
					npc4.charID = 5;
				}
				try
				{
					npc4.avatar = msg.reader().readShort();
				}
				catch (Exception)
				{
				}
				ChatPopup.addChatPopupMultiLine(str, 100000, npc4);
				GameCanvas.panel.hideNow();
				break;
			}
			case 32:
			{
				GameCanvas.debug("SA68", 2);
				int num71 = msg.reader().readShort();
				for (int num72 = 0; num72 < GameScr.vNpc.size(); num72++)
				{
					Npc npc = (Npc)GameScr.vNpc.elementAt(num72);
					if (npc.template.npcTemplateId == num71 && npc.Equals(Char.myCharz().npcFocus))
					{
						string chat = msg.reader().readUTF();
						string[] array7 = new string[msg.reader().readByte()];
						for (int num73 = 0; num73 < array7.Length; num73++)
						{
							array7[num73] = msg.reader().readUTF();
						}
						GameScr.gI().createMenu(array7, npc);
						ChatPopup.addChatPopup(chat, 100000, npc);
						return;
					}
				}
				Npc npc2 = new Npc(num71, 0, -100, 100, num71, GameScr.info1.charId[Char.myCharz().cgender][2]);
				Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
				string chat2 = msg.reader().readUTF();
				string[] array8 = new string[msg.reader().readByte()];
				for (int num74 = 0; num74 < array8.Length; num74++)
				{
					array8[num74] = msg.reader().readUTF();
				}
				try
				{
					short avatar = msg.reader().readShort();
					npc2.avatar = avatar;
				}
				catch (Exception)
				{
				}
				Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
				GameScr.gI().createMenu(array8, npc2);
				ChatPopup.addChatPopup(chat2, 100000, npc2);
				break;
			}
			case 7:
			{
				sbyte type3 = msg.reader().readByte();
				short id3 = msg.reader().readShort();
				string info2 = msg.reader().readUTF();
				GameCanvas.panel.saleRequest(type3, info2, id3);
				break;
			}
			case 6:
				GameCanvas.debug("SA70", 2);
				Char.myCharz().xu = msg.reader().readLong();
				Char.myCharz().luong = msg.reader().readInt();
				Char.myCharz().luongKhoa = msg.reader().readInt();
				Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
				Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
				Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
				GameCanvas.endDlg();
				break;
			case -24:
				Res.outz("***************MAP_INFO**************");
				GameScr.isPickNgocRong = false;
				Char.isLoadingMap = true;
				Cout.println("GET MAP INFO");
				GameScr.gI().magicTree = null;
				GameCanvas.isLoading = true;
				GameCanvas.debug("SA75", 2);
				GameScr.resetAllvector();
				GameCanvas.endDlg();
				TileMap.vGo.removeAllElements();
				PopUp.vPopups.removeAllElements();
				mSystem.gcc();
				TileMap.mapID = msg.reader().readUnsignedByte();
				TileMap.planetID = msg.reader().readByte();
				TileMap.tileID = msg.reader().readByte();
				TileMap.bgID = msg.reader().readByte();
				GameScr.isPaint_CT = TileMap.mapID != 170;
				Cout.println("load planet from server: " + TileMap.planetID + "bgType= " + TileMap.bgType + ".............................");
				TileMap.typeMap = msg.reader().readByte();
				TileMap.mapName = msg.reader().readUTF();
				TileMap.zoneID = msg.reader().readByte();
				GameCanvas.debug("SA75x1", 2);
				try
				{
					TileMap.loadMapFromResource(TileMap.mapID);
				}
				catch (Exception)
				{
					Service.gI().requestMaptemplate(TileMap.mapID);
					messWait = msg;
					break;
				}
				loadInfoMap(msg);
				try
				{
					TileMap.isMapDouble = ((msg.reader().readByte() != 0) ? true : false);
				}
				catch (Exception)
				{
				}
				GameScr.cmx = GameScr.cmtoX;
				GameScr.cmy = GameScr.cmtoY;
				GameCanvas.isRequestMapID = 2;
				GameCanvas.waitingTimeChangeMap = mSystem.currentTimeMillis() + 1000;
				break;
			case -31:
			{
				TileMap.vItemBg.removeAllElements();
				short num64 = msg.reader().readShort();
				Res.err("[ITEM_BACKGROUND] nItem= " + num64);
				for (int num65 = 0; num65 < num64; num65++)
				{
					BgItem bgItem = new BgItem();
					bgItem.id = num65;
					bgItem.idImage = msg.reader().readShort();
					bgItem.layer = msg.reader().readByte();
					bgItem.dx = msg.reader().readShort();
					bgItem.dy = msg.reader().readShort();
					sbyte b27 = msg.reader().readByte();
					bgItem.tileX = new int[b27];
					bgItem.tileY = new int[b27];
					for (int num66 = 0; num66 < b27; num66++)
					{
						bgItem.tileX[num65] = msg.reader().readByte();
						bgItem.tileY[num65] = msg.reader().readByte();
					}
					TileMap.vItemBg.addElement(bgItem);
				}
				break;
			}
			case -4:
			{
				GameCanvas.debug("SA76", 2);
				obj = GameScr.findCharInMap(msg.reader().readInt());
				if (obj == null)
				{
					return;
				}
				GameCanvas.debug("SA76v1", 2);
				if ((TileMap.tileTypeAtPixel(obj.cx, obj.cy) & 2) == 2)
				{
					obj.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 0);
				}
				else
				{
					obj.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 1);
				}
				GameCanvas.debug("SA76v2", 2);
				obj.attMobs = new Mob[msg.reader().readByte()];
				for (int num29 = 0; num29 < obj.attMobs.Length; num29++)
				{
					Mob mob3 = (Mob)GameScr.vMob.elementAt(msg.reader().readByte());
					obj.attMobs[num29] = mob3;
					if (num29 == 0)
					{
						if (obj.cx <= mob3.x)
						{
							obj.cdir = 1;
						}
						else
						{
							obj.cdir = -1;
						}
					}
				}
				GameCanvas.debug("SA76v3", 2);
				obj.charFocus = null;
				obj.mobFocus = obj.attMobs[0];
				Char[] array4 = new Char[10];
				num = 0;
				try
				{
					for (num = 0; num < array4.Length; num++)
					{
						int num30 = msg.reader().readInt();
						Char obj4 = (array4[num] = ((num30 != Char.myCharz().charID) ? GameScr.findCharInMap(num30) : Char.myCharz()));
						if (num == 0)
						{
							if (obj.cx <= obj4.cx)
							{
								obj.cdir = 1;
							}
							else
							{
								obj.cdir = -1;
							}
						}
					}
				}
				catch (Exception ex6)
				{
					Cout.println("Loi PLAYER_ATTACK_N_P " + ex6.ToString());
				}
				GameCanvas.debug("SA76v4", 2);
				if (num > 0)
				{
					obj.attChars = new Char[num];
					for (num = 0; num < obj.attChars.Length; num++)
					{
						obj.attChars[num] = array4[num];
					}
					obj.charFocus = obj.attChars[0];
					obj.mobFocus = null;
				}
				GameCanvas.debug("SA76v5", 2);
				break;
			}
			case 54:
			{
				obj = GameScr.findCharInMap(msg.reader().readInt());
				if (obj == null)
				{
					return;
				}
				int num21 = msg.reader().readUnsignedByte();
				if ((TileMap.tileTypeAtPixel(obj.cx, obj.cy) & 2) == 2)
				{
					obj.setSkillPaint(GameScr.sks[num21], 0);
				}
				else
				{
					obj.setSkillPaint(GameScr.sks[num21], 1);
				}
				Mob[] array3 = new Mob[10];
				num = 0;
				try
				{
					for (num = 0; num < array3.Length; num++)
					{
						Mob mob2 = (array3[num] = (Mob)GameScr.vMob.elementAt(msg.reader().readByte()));
						if (num == 0)
						{
							if (obj.cx <= mob2.x)
							{
								obj.cdir = 1;
							}
							else
							{
								obj.cdir = -1;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				if (num > 0)
				{
					obj.attMobs = new Mob[num];
					for (num = 0; num < obj.attMobs.Length; num++)
					{
						obj.attMobs[num] = array3[num];
					}
					obj.charFocus = null;
					obj.mobFocus = obj.attMobs[0];
				}
				break;
			}
			case -60:
			{
				GameCanvas.debug("SA7666", 2);
				int num2 = msg.reader().readInt();
				int num3 = -1;
				if (num2 != Char.myCharz().charID)
				{
					Char obj2 = GameScr.findCharInMap(num2);
					if (obj2 == null)
					{
						return;
					}
					if (obj2.currentMovePoint != null)
					{
						obj2.createShadow(obj2.cx, obj2.cy, 10);
						obj2.cx = obj2.currentMovePoint.xEnd;
						obj2.cy = obj2.currentMovePoint.yEnd;
					}
					int num4 = msg.reader().readUnsignedByte();
					if ((TileMap.tileTypeAtPixel(obj2.cx, obj2.cy) & 2) == 2)
					{
						obj2.setSkillPaint(GameScr.sks[num4], 0);
					}
					else
					{
						obj2.setSkillPaint(GameScr.sks[num4], 1);
					}
					sbyte b = msg.reader().readByte();
					Char[] array = new Char[b];
					for (num = 0; num < array.Length; num++)
					{
						num3 = msg.reader().readInt();
						Char obj3;
						if (num3 == Char.myCharz().charID)
						{
							obj3 = Char.myCharz();
							if (!GameScr.isChangeZone && GameScr.isAutoPlay && GameScr.canAutoPlay)
							{
								Service.gI().requestChangeZone(-1, -1);
								GameScr.isChangeZone = true;
							}
						}
						else
						{
							obj3 = GameScr.findCharInMap(num3);
						}
						array[num] = obj3;
						if (num == 0)
						{
							if (obj2.cx <= obj3.cx)
							{
								obj2.cdir = 1;
							}
							else
							{
								obj2.cdir = -1;
							}
						}
					}
					if (num > 0)
					{
						obj2.attChars = new Char[num];
						for (num = 0; num < obj2.attChars.Length; num++)
						{
							obj2.attChars[num] = array[num];
						}
						obj2.mobFocus = null;
						obj2.charFocus = obj2.attChars[0];
					}
				}
				else
				{
					sbyte b2 = msg.reader().readByte();
					sbyte b3 = msg.reader().readByte();
					num3 = msg.reader().readInt();
				}
				try
				{
					sbyte b4 = msg.reader().readByte();
					Res.outz("isRead continue = " + b4);
					if (b4 != 1)
					{
						break;
					}
					sbyte b5 = msg.reader().readByte();
					Res.outz("type skill = " + b5);
					if (num3 == Char.myCharz().charID)
					{
						bool flag = false;
						obj = Char.myCharz();
						long num5 = msg.reader().readLong();
						Res.outz("dame hit = " + num5);
						obj.isDie = msg.reader().readBoolean();
						if (obj.isDie)
						{
							Char.isLockKey = true;
						}
						Res.outz("isDie=" + obj.isDie + "---------------------------------------");
						int num6 = 0;
						flag = (obj.isCrit = msg.reader().readBoolean());
						obj.isMob = false;
						num5 = (obj.damHP = num5 + num6);
						if (b5 == 0)
						{
							obj.doInjure(num5, 0L, flag, isMob: false);
						}
					}
					else
					{
						obj = GameScr.findCharInMap(num3);
						if (obj == null)
						{
							return;
						}
						bool flag2 = false;
						long num7 = msg.reader().readLong();
						Res.outz("dame hit= " + num7);
						obj.isDie = msg.reader().readBoolean();
						Res.outz("isDie=" + obj.isDie + "---------------------------------------");
						int num8 = 0;
						flag2 = (obj.isCrit = msg.reader().readBoolean());
						obj.isMob = false;
						num7 = (obj.damHP = num7 + num8);
						if (b5 == 0)
						{
							obj.doInjure(num7, 0L, flag2, isMob: false);
						}
					}
				}
				catch (Exception)
				{
				}
				break;
			}
			}
			switch (msg.command)
			{
			case -2:
			{
				GameCanvas.debug("SA77", 22);
				int num175 = msg.reader().readInt();
				Char obj11 = Char.myCharz();
				obj11.yen += num175;
				GameScr.startFlyText((num175 <= 0) ? (string.Empty + num175) : ("+" + num175), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case 95:
			{
				GameCanvas.debug("SA77", 22);
				int num191 = msg.reader().readInt();
				Char obj11 = Char.myCharz();
				obj11.xu += num191;
				Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
				GameScr.startFlyText((num191 <= 0) ? (string.Empty + num191) : ("+" + num191), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case 96:
				GameCanvas.debug("SA77a", 22);
				Char.myCharz().taskOrders.addElement(new TaskOrder(msg.reader().readByte(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readByte(), msg.reader().readByte()));
				break;
			case 97:
			{
				sbyte b65 = msg.reader().readByte();
				for (int num176 = 0; num176 < Char.myCharz().taskOrders.size(); num176++)
				{
					TaskOrder taskOrder = (TaskOrder)Char.myCharz().taskOrders.elementAt(num176);
					if (taskOrder.taskId == b65)
					{
						taskOrder.count = msg.reader().readShort();
						break;
					}
				}
				break;
			}
			case -1:
			{
				GameCanvas.debug("SA77", 222);
				int num180 = msg.reader().readInt();
				Char obj11 = Char.myCharz();
				obj11.xu += num180;
				Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
				obj11 = Char.myCharz();
				obj11.yen -= num180;
				GameScr.startFlyText("+" + num180, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case -3:
			{
				GameCanvas.debug("SA78", 2);
				sbyte b70 = msg.reader().readByte();
				int num190 = msg.reader().readInt();
				if (b70 == 0)
				{
					Char obj11 = Char.myCharz();
					obj11.cPower += num190;
				}
				if (b70 == 1)
				{
					Char obj11 = Char.myCharz();
					obj11.cTiemNang += num190;
				}
				if (b70 == 2)
				{
					Char obj11 = Char.myCharz();
					obj11.cPower += num190;
					obj11 = Char.myCharz();
					obj11.cTiemNang += num190;
				}
				Char.myCharz().applyCharLevelPercent();
				if (Char.myCharz().cTypePk != 3)
				{
					GameScr.startFlyText(((num190 <= 0) ? string.Empty : "+") + num190, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -4, mFont.GREEN);
					if (num190 > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5002)
					{
						ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
						ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
					}
				}
				break;
			}
			case -73:
			{
				sbyte b68 = msg.reader().readByte();
				for (int num185 = 0; num185 < GameScr.vNpc.size(); num185++)
				{
					Npc npc7 = (Npc)GameScr.vNpc.elementAt(num185);
					if (npc7.template.npcTemplateId == b68)
					{
						if (msg.reader().readByte() == 0)
						{
							npc7.isHide = true;
						}
						else
						{
							npc7.isHide = false;
						}
						break;
					}
				}
				break;
			}
			case -5:
			{
				GameCanvas.debug("SA79", 2);
				int charID = msg.reader().readInt();
				int num182 = msg.reader().readInt();
				Char obj17;
				if (num182 != -100)
				{
					obj17 = new Char();
					obj17.charID = charID;
					obj17.clanID = num182;
				}
				else
				{
					obj17 = new Mabu();
					obj17.charID = charID;
					obj17.clanID = num182;
				}
				if (obj17.clanID == -2)
				{
					obj17.isCopy = true;
				}
				if (readCharInfo(obj17, msg))
				{
					sbyte b67 = msg.reader().readByte();
					if (obj17.cy <= 10 && b67 != 0 && b67 != 2)
					{
						Res.outz("nhân vật bay trên trời xuống x= " + obj17.cx + " y= " + obj17.cy);
						Teleport teleport2 = new Teleport(obj17.cx, obj17.cy, obj17.head, obj17.cdir, 1, isMe: false, (b67 != 1) ? b67 : obj17.cgender);
						teleport2.id = obj17.charID;
						obj17.isTeleport = true;
						Teleport.addTeleport(teleport2);
					}
					if (b67 == 2)
					{
						obj17.show();
					}
					for (int num183 = 0; num183 < GameScr.vMob.size(); num183++)
					{
						Mob mob19 = (Mob)GameScr.vMob.elementAt(num183);
						if (mob19 != null && mob19.isMobMe && mob19.mobId == obj17.charID)
						{
							Res.outz("co 1 con quai");
							obj17.mobMe = mob19;
							obj17.mobMe.x = obj17.cx;
							obj17.mobMe.y = obj17.cy - 40;
							break;
						}
					}
					if (GameScr.findCharInMap(obj17.charID) == null)
					{
						GameScr.vCharInMap.addElement(obj17);
					}
					obj17.isMonkey = msg.reader().readByte();
					short num184 = msg.reader().readShort();
					Res.outz("mount id= " + num184 + "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
					if (num184 != -1)
					{
						obj17.isHaveMount = true;
						switch (num184)
						{
						case 346:
						case 347:
						case 348:
							obj17.isMountVip = false;
							break;
						case 349:
						case 350:
						case 351:
							obj17.isMountVip = true;
							break;
						case 396:
							obj17.isEventMount = true;
							break;
						case 532:
							obj17.isSpeacialMount = true;
							break;
						default:
							if (num184 >= Char.ID_NEW_MOUNT)
							{
								obj17.idMount = num184;
							}
							break;
						}
					}
					else
					{
						obj17.isHaveMount = false;
					}
				}
				sbyte cFlag = msg.reader().readByte();
				Res.outz("addplayer:   " + cFlag);
				obj17.cFlag = cFlag;
				obj17.isNhapThe = msg.reader().readByte() == 1;
				try
				{
					obj17.idAuraEff = msg.reader().readShort();
					obj17.idEff_Set_Item = msg.reader().readSByte();
					obj17.idHat = msg.reader().readShort();
					Effect.GetCharEff(obj17);
				}
				catch (Exception ex38)
				{
					Res.outz("cmd: -5 err: " + ex38.StackTrace);
				}
				GameScr.gI().getFlagImage(obj17.charID, obj17.cFlag);
				break;
			}
			case -7:
			{
				GameCanvas.debug("SA80", 2);
				int num177 = msg.reader().readInt();
				for (int num178 = 0; num178 < GameScr.vCharInMap.size(); num178++)
				{
					Char obj16 = null;
					try
					{
						obj16 = (Char)GameScr.vCharInMap.elementAt(num178);
					}
					catch (Exception)
					{
						continue;
					}
					if (obj16 == null || obj16.charID != num177)
					{
						continue;
					}
					GameCanvas.debug("SA8x2y" + num178, 2);
					obj16.moveTo(msg.reader().readShort(), msg.reader().readShort(), 0);
					obj16.lastUpdateTime = mSystem.currentTimeMillis();
					break;
				}
				GameCanvas.debug("SA80x3", 2);
				break;
			}
			case -6:
			{
				GameCanvas.debug("SA81", 2);
				int num173 = msg.reader().readInt();
				for (int num174 = 0; num174 < GameScr.vCharInMap.size(); num174++)
				{
					Char obj15 = (Char)GameScr.vCharInMap.elementAt(num174);
					if (obj15 != null && obj15.charID == num173)
					{
						if (!obj15.isInvisiblez && !obj15.isUsePlane)
						{
							ServerEffect.addServerEffect(60, obj15.cx, obj15.cy, 1);
						}
						if (!obj15.isUsePlane)
						{
							GameScr.vCharInMap.removeElementAt(num174);
						}
						return;
					}
				}
				break;
			}
			case -13:
			{
				GameCanvas.debug("SA82", 2);
				int num186 = msg.reader().readUnsignedByte();
				if (num186 > GameScr.vMob.size() - 1 || num186 < 0)
				{
					return;
				}
				Mob mob20 = (Mob)GameScr.vMob.elementAt(num186);
				mob20.sys = msg.reader().readByte();
				mob20.levelBoss = msg.reader().readByte();
				if (mob20.levelBoss != 0)
				{
					mob20.typeSuperEff = Res.random(0, 3);
				}
				mob20.x = mob20.xFirst;
				mob20.y = mob20.yFirst;
				mob20.status = 5;
				mob20.injureThenDie = false;
				mob20.hp = msg.reader().readLong();
				mob20.maxHp = mob20.hp;
				mob20.updateHp_bar();
				ServerEffect.addServerEffect(60, mob20.x, mob20.y, 1);
				break;
			}
			case -75:
			{
				Mob mob17 = null;
				try
				{
					mob17 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				if (mob17 != null)
				{
					mob17.levelBoss = msg.reader().readByte();
					if (mob17.levelBoss > 0)
					{
						mob17.typeSuperEff = Res.random(0, 3);
					}
				}
				break;
			}
			case -9:
			{
				GameCanvas.debug("SA83", 2);
				Mob mob16 = null;
				try
				{
					mob16 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA83v1", 2);
				if (mob16 != null)
				{
					mob16.hp = msg.reader().readLong();
					mob16.updateHp_bar();
					long num179 = msg.reader().readLong();
					if (num179 == 1)
					{
						return;
					}
					if (num179 > 1)
					{
						mob16.setInjure();
					}
					bool flag10 = false;
					try
					{
						flag10 = msg.reader().readBoolean();
					}
					catch (Exception)
					{
					}
					sbyte b66 = msg.reader().readByte();
					if (b66 != -1)
					{
						EffecMn.addEff(new Effect(b66, mob16.x, mob16.getY(), 3, 1, -1));
					}
					GameCanvas.debug("SA83v2", 2);
					if (flag10)
					{
						GameScr.startFlyText("-" + num179, mob16.x, mob16.getY() - mob16.getH(), 0, -2, mFont.FATAL);
					}
					else if (num179 == 0)
					{
						mob16.x = mob16.xFirst;
						mob16.y = mob16.yFirst;
						GameScr.startFlyText(mResources.miss, mob16.x, mob16.getY() - mob16.getH(), 0, -2, mFont.MISS);
					}
					else if (num179 > 1)
					{
						GameScr.startFlyText("-" + num179, mob16.x, mob16.getY() - mob16.getH(), 0, -2, mFont.ORANGE);
					}
				}
				GameCanvas.debug("SA83v3", 2);
				break;
			}
			case 45:
			{
				GameCanvas.debug("SA84", 2);
				Mob mob14 = null;
				try
				{
					mob14 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception ex29)
				{
					Cout.println("Loi tai NPC_MISS  " + ex29.ToString());
				}
				if (mob14 != null)
				{
					mob14.hp = msg.reader().readLong();
					mob14.updateHp_bar();
					GameScr.startFlyText(mResources.miss, mob14.x, mob14.y - mob14.h, 0, -2, mFont.MISS);
				}
				break;
			}
			case -12:
			{
				Res.outz("SERVER SEND MOB DIE");
				GameCanvas.debug("SA85", 2);
				Mob mob21 = null;
				try
				{
					mob21 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
					Cout.println("LOi tai NPC_DIE cmd " + msg.command);
				}
				if (mob21 == null || mob21.status == 0 || mob21.status == 0)
				{
					break;
				}
				mob21.startDie();
				try
				{
					long num187 = msg.reader().readLong();
					if (msg.reader().readBool())
					{
						GameScr.startFlyText("-" + num187, mob21.x, mob21.y - mob21.h, 0, -2, mFont.FATAL);
					}
					else
					{
						GameScr.startFlyText("-" + num187, mob21.x, mob21.y - mob21.h, 0, -2, mFont.ORANGE);
					}
					sbyte b69 = msg.reader().readByte();
					for (int num188 = 0; num188 < b69; num188++)
					{
						ItemMap itemMap6 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob21.x, mob21.y, msg.reader().readShort(), msg.reader().readShort());
						int num189 = (itemMap6.playerId = msg.reader().readInt());
						Res.outz("playerid= " + num189 + " my id= " + Char.myCharz().charID);
						GameScr.vItemMap.addElement(itemMap6);
						if (Res.abs(itemMap6.y - Char.myCharz().cy) < 24 && Res.abs(itemMap6.x - Char.myCharz().cx) < 24)
						{
							Char.myCharz().charFocus = null;
						}
					}
				}
				catch (Exception)
				{
				}
				break;
			}
			case 74:
			{
				GameCanvas.debug("SA85", 2);
				Mob mob15 = null;
				try
				{
					mob15 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
					Cout.println("Loi tai NPC CHANGE " + msg.command);
				}
				if (mob15 != null && mob15.status != 0 && mob15.status != 0)
				{
					mob15.status = 0;
					ServerEffect.addServerEffect(60, mob15.x, mob15.y, 1);
					ItemMap itemMap5 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob15.x, mob15.y, msg.reader().readShort(), msg.reader().readShort());
					GameScr.vItemMap.addElement(itemMap5);
					if (Res.abs(itemMap5.y - Char.myCharz().cy) < 24 && Res.abs(itemMap5.x - Char.myCharz().cx) < 24)
					{
						Char.myCharz().charFocus = null;
					}
				}
				break;
			}
			case -11:
			{
				GameCanvas.debug("SA86", 2);
				Mob mob13 = null;
				try
				{
					int index5 = msg.reader().readUnsignedByte();
					mob13 = (Mob)GameScr.vMob.elementAt(index5);
				}
				catch (Exception ex27)
				{
					Res.outz("Loi tai NPC_ATTACK_ME " + msg.command + " err= " + ex27.StackTrace);
				}
				if (mob13 != null)
				{
					Char.myCharz().isDie = false;
					Char.isLockKey = false;
					long num170 = msg.reader().readLong();
					long num171;
					try
					{
						num171 = msg.reader().readLong();
					}
					catch (Exception)
					{
						num171 = 0L;
					}
					if (mob13.isBusyAttackSomeOne)
					{
						Char.myCharz().doInjure(num170, num171, isCrit: false, isMob: true);
						break;
					}
					mob13.dame = num170;
					mob13.dameMp = num171;
					mob13.setAttack(Char.myCharz());
				}
				break;
			}
			case -10:
			{
				GameCanvas.debug("SA87", 2);
				Mob mob18 = null;
				try
				{
					mob18 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA87x1", 2);
				if (mob18 != null)
				{
					GameCanvas.debug("SA87x2", 2);
					obj = GameScr.findCharInMap(msg.reader().readInt());
					if (obj == null)
					{
						return;
					}
					GameCanvas.debug("SA87x3", 2);
					long num181 = msg.reader().readLong();
					mob18.dame = obj.cHP - num181;
					obj.cHPNew = num181;
					GameCanvas.debug("SA87x4", 2);
					try
					{
						obj.cMP = msg.reader().readLong();
					}
					catch (Exception)
					{
					}
					GameCanvas.debug("SA87x5", 2);
					if (mob18.isBusyAttackSomeOne)
					{
						obj.doInjure(mob18.dame, 0L, isCrit: false, isMob: true);
					}
					else
					{
						mob18.setAttack(obj);
					}
					GameCanvas.debug("SA87x6", 2);
				}
				break;
			}
			case -17:
				GameCanvas.debug("SA88", 2);
				Char.myCharz().meDead = true;
				Char.myCharz().cPk = msg.reader().readByte();
				Char.myCharz().startDie(msg.reader().readShort(), msg.reader().readShort());
				try
				{
					Char.myCharz().cPower = msg.reader().readLong();
					Char.myCharz().applyCharLevelPercent();
				}
				catch (Exception)
				{
					Cout.println("Loi tai ME_DIE " + msg.command);
				}
				Char.myCharz().countKill = 0;
				break;
			case 66:
				Res.outz("ME DIE XP DOWN NOT IMPLEMENT YET!!!!!!!!!!!!!!!!!!!!!!!!!!");
				break;
			case -8:
				GameCanvas.debug("SA89", 2);
				obj = GameScr.findCharInMap(msg.reader().readInt());
				if (obj == null)
				{
					return;
				}
				obj.cPk = msg.reader().readByte();
				obj.waitToDie(msg.reader().readShort(), msg.reader().readShort());
				break;
			case -16:
				GameCanvas.debug("SA90", 2);
				if (Char.myCharz().wdx != 0 || Char.myCharz().wdy != 0)
				{
					Char.myCharz().cx = Char.myCharz().wdx;
					Char.myCharz().cy = Char.myCharz().wdy;
					Char.myCharz().wdx = (Char.myCharz().wdy = 0);
				}
				Char.myCharz().liveFromDead();
				Char.myCharz().isLockMove = false;
				Char.myCharz().meDead = false;
				break;
			case 44:
			{
				GameCanvas.debug("SA91", 2);
				int num172 = msg.reader().readInt();
				string text9 = msg.reader().readUTF();
				Res.outz("user id= " + num172 + " text= " + text9);
				obj = ((Char.myCharz().charID != num172) ? GameScr.findCharInMap(num172) : Char.myCharz());
				if (obj == null)
				{
					return;
				}
				obj.addInfo(text9);
				break;
			}
			case 18:
			{
				sbyte b64 = msg.reader().readByte();
				for (int num169 = 0; num169 < b64; num169++)
				{
					int charId = msg.reader().readInt();
					int cx = msg.reader().readShort();
					int cy = msg.reader().readShort();
					long cHPShow = msg.reader().readLong();
					Char obj14 = GameScr.findCharInMap(charId);
					if (obj14 != null)
					{
						obj14.cx = cx;
						obj14.cy = cy;
						obj14.cHP = (obj14.cHPShow = cHPShow);
						obj14.lastUpdateTime = mSystem.currentTimeMillis();
					}
				}
				break;
			}
			case 19:
				Char.myCharz().countKill = msg.reader().readUnsignedShort();
				Char.myCharz().countKillMax = msg.reader().readUnsignedShort();
				break;
			}
			GameCanvas.debug("SA92", 2);
		}
		catch (Exception ex41)
		{
			Res.err("[Controller] [error] " + ex41.StackTrace + " msg: " + ex41.Message + " cause " + ex41.Data);
		}
		finally
		{
			msg?.cleanup();
		}
	}

	private void readLogin(Message msg)
	{
		sbyte b = msg.reader().readByte();
		ChooseCharScr.playerData = new PlayerData[b];
		Res.outz("[LEN] sl nguoi choi " + b);
		for (int i = 0; i < b; i++)
		{
			int playerID = msg.reader().readInt();
			string name = msg.reader().readUTF();
			short head = msg.reader().readShort();
			short body = msg.reader().readShort();
			short leg = msg.reader().readShort();
			long ppoint = msg.reader().readLong();
			ChooseCharScr.playerData[i] = new PlayerData(playerID, name, head, body, leg, ppoint);
		}
		GameCanvas.chooseCharScr.switchToMe();
		GameCanvas.chooseCharScr.updateChooseCharacter((byte)b);
	}

	private void createSkill(myReader d)
	{
		GameScr.vcSkill = d.readByte();
		GameScr.gI().sOptionTemplates = new SkillOptionTemplate[d.readByte()];
		for (int i = 0; i < GameScr.gI().sOptionTemplates.Length; i++)
		{
			GameScr.gI().sOptionTemplates[i] = new SkillOptionTemplate();
			GameScr.gI().sOptionTemplates[i].id = i;
			GameScr.gI().sOptionTemplates[i].name = d.readUTF();
		}
		GameScr.nClasss = new NClass[d.readByte()];
		for (int j = 0; j < GameScr.nClasss.Length; j++)
		{
			GameScr.nClasss[j] = new NClass();
			GameScr.nClasss[j].classId = j;
			GameScr.nClasss[j].name = d.readUTF();
			GameScr.nClasss[j].skillTemplates = new SkillTemplate[d.readByte()];
			for (int k = 0; k < GameScr.nClasss[j].skillTemplates.Length; k++)
			{
				GameScr.nClasss[j].skillTemplates[k] = new SkillTemplate();
				GameScr.nClasss[j].skillTemplates[k].id = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].name = d.readUTF();
				GameScr.nClasss[j].skillTemplates[k].maxPoint = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].manaUseType = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].type = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].iconId = d.readShort();
				GameScr.nClasss[j].skillTemplates[k].damInfo = d.readUTF();
				int lineWidth = 130;
				if (GameCanvas.w == 128 || GameCanvas.h <= 208)
				{
					lineWidth = 100;
				}
				GameScr.nClasss[j].skillTemplates[k].description = mFont.tahoma_7_green2.splitFontArray(d.readUTF(), lineWidth);
				GameScr.nClasss[j].skillTemplates[k].skills = new Skill[d.readByte()];
				for (int l = 0; l < GameScr.nClasss[j].skillTemplates[k].skills.Length; l++)
				{
					GameScr.nClasss[j].skillTemplates[k].skills[l] = new Skill();
					GameScr.nClasss[j].skillTemplates[k].skills[l].skillId = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].template = GameScr.nClasss[j].skillTemplates[k];
					GameScr.nClasss[j].skillTemplates[k].skills[l].point = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].powRequire = d.readLong();
					GameScr.nClasss[j].skillTemplates[k].skills[l].manaUse = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].coolDown = d.readInt();
					GameScr.nClasss[j].skillTemplates[k].skills[l].dx = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].dy = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].maxFight = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].damage = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].price = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].moreInfo = d.readUTF();
					Skills.add(GameScr.nClasss[j].skillTemplates[k].skills[l]);
				}
			}
		}
	}

	private void createMap(myReader d)
	{
		GameScr.vcMap = d.readByte();
		TileMap.mapNames = new string[d.readShort()];
		for (int i = 0; i < TileMap.mapNames.Length; i++)
		{
			TileMap.mapNames[i] = d.readUTF();
		}
		Npc.arrNpcTemplate = new NpcTemplate[d.readByte()];
		for (sbyte b = 0; b < Npc.arrNpcTemplate.Length; b++)
		{
			Npc.arrNpcTemplate[b] = new NpcTemplate();
			Npc.arrNpcTemplate[b].npcTemplateId = b;
			Npc.arrNpcTemplate[b].name = d.readUTF();
			Npc.arrNpcTemplate[b].headId = d.readShort();
			Npc.arrNpcTemplate[b].bodyId = d.readShort();
			Npc.arrNpcTemplate[b].legId = d.readShort();
			Npc.arrNpcTemplate[b].menu = new string[d.readByte()][];
			for (int j = 0; j < Npc.arrNpcTemplate[b].menu.Length; j++)
			{
				Npc.arrNpcTemplate[b].menu[j] = new string[d.readByte()];
				for (int k = 0; k < Npc.arrNpcTemplate[b].menu[j].Length; k++)
				{
					Npc.arrNpcTemplate[b].menu[j][k] = d.readUTF();
				}
			}
		}
		Mob.arrMobTemplate = new MobTemplate[d.readShort()];
		for (int l = 0; l < Mob.arrMobTemplate.Length; l++)
		{
			Mob.arrMobTemplate[l] = new MobTemplate();
			Mob.arrMobTemplate[l].mobTemplateId = l;
			Mob.arrMobTemplate[l].type = d.readByte();
			Mob.arrMobTemplate[l].name = d.readUTF();
			Mob.arrMobTemplate[l].hp = d.readLong();
			Mob.arrMobTemplate[l].rangeMove = d.readByte();
			Mob.arrMobTemplate[l].speed = d.readByte();
			Mob.arrMobTemplate[l].dartType = d.readByte();
		}
	}

	private void createData(myReader d, bool isSaveRMS)
	{
		GameScr.vcData = d.readByte();
		if (isSaveRMS)
		{
			Rms.saveRMS("NR_dart", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_arrow", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_effect", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_image", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_part", NinjaUtil.readByteArray(d));
			Rms.saveRMS("NR_skill", NinjaUtil.readByteArray(d));
			Rms.DeleteStorage("NRdata");
		}
	}

	private Image createImage(sbyte[] arr)
	{
		try
		{
			return Image.createImage(arr, 0, arr.Length);
		}
		catch (Exception)
		{
		}
		return null;
	}

	public int[] arrayByte2Int(sbyte[] b)
	{
		int[] array = new int[b.Length];
		for (int i = 0; i < b.Length; i++)
		{
			int num = b[i];
			if (num < 0)
			{
				num += 256;
			}
			array[i] = num;
		}
		return array;
	}

	public void readClanMsg(Message msg, int index)
	{
		try
		{
			ClanMessage clanMessage = new ClanMessage();
			sbyte b = (sbyte)(clanMessage.type = msg.reader().readByte());
			clanMessage.id = msg.reader().readInt();
			clanMessage.playerId = msg.reader().readInt();
			clanMessage.playerName = msg.reader().readUTF();
			clanMessage.role = msg.reader().readByte();
			clanMessage.time = msg.reader().readInt() + 1000000000;
			bool flag = false;
			GameScr.isNewClanMessage = false;
			switch (b)
			{
			case 0:
			{
				string text = msg.reader().readUTF();
				GameScr.isNewClanMessage = true;
				if (mFont.tahoma_7.getWidth(text) > Panel.WIDTH_PANEL - 60)
				{
					clanMessage.chat = mFont.tahoma_7.splitFontArray(text, Panel.WIDTH_PANEL - 10);
				}
				else
				{
					clanMessage.chat = new string[1];
					clanMessage.chat[0] = text;
				}
				clanMessage.color = msg.reader().readByte();
				break;
			}
			case 1:
				clanMessage.recieve = msg.reader().readByte();
				clanMessage.maxCap = msg.reader().readByte();
				flag = msg.reader().readByte() == 1;
				if (flag)
				{
					GameScr.isNewClanMessage = true;
				}
				if (clanMessage.playerId != Char.myCharz().charID)
				{
					if (clanMessage.recieve < clanMessage.maxCap)
					{
						clanMessage.option = new string[1] { mResources.donate };
					}
					else
					{
						clanMessage.option = null;
					}
				}
				if (GameCanvas.panel.cp != null)
				{
					GameCanvas.panel.updateRequest(clanMessage.recieve, clanMessage.maxCap);
				}
				break;
			case 2:
				if (Char.myCharz().role == 0)
				{
					GameScr.isNewClanMessage = true;
					clanMessage.option = new string[2]
					{
						mResources.CANCEL,
						mResources.receive
					};
				}
				break;
			}
			if (GameCanvas.currentScreen != GameScr.instance)
			{
				GameScr.isNewClanMessage = false;
			}
			else if (GameCanvas.panel.isShow && GameCanvas.panel.type == 0 && GameCanvas.panel.currentTabIndex == 3)
			{
				GameScr.isNewClanMessage = false;
			}
			ClanMessage.addMessage(clanMessage, index, flag);
		}
		catch (Exception)
		{
			Cout.println("LOI TAI CMD -= " + msg.command);
		}
	}

	public void loadCurrMap(sbyte teleport3)
	{
		Res.outz("[CONTROLER] start load map " + teleport3);
		GameScr.gI().auto = 0;
		GameScr.isChangeZone = false;
		CreateCharScr.instance = null;
		GameScr.info1.isUpdate = false;
		GameScr.info2.isUpdate = false;
		GameScr.lockTick = 0;
		GameCanvas.panel.isShow = false;
		SoundMn.gI().stopAll();
		if (!GameScr.isLoadAllData && !CreateCharScr.isCreateChar)
		{
			GameScr.gI().initSelectChar();
		}
		GameScr.loadCamera(fullmScreen: false, (teleport3 != 1) ? (-1) : Char.myCharz().cx, (teleport3 == 0) ? (-1) : 0);
		TileMap.loadMainTile();
		TileMap.loadMap(TileMap.tileID);
		Res.outz("LOAD GAMESCR 2");
		Char.myCharz().cvx = 0;
		Char.myCharz().statusMe = 4;
		Char.myCharz().currentMovePoint = null;
		Char.myCharz().mobFocus = null;
		Char.myCharz().charFocus = null;
		Char.myCharz().npcFocus = null;
		Char.myCharz().itemFocus = null;
		Char.myCharz().skillPaint = null;
		Char.myCharz().setMabuHold(m: false);
		Char.myCharz().skillPaintRandomPaint = null;
		GameCanvas.clearAllPointerEvent();
		if (Char.myCharz().cy >= TileMap.pxh - 100)
		{
			Char.myCharz().isFlyUp = true;
			Char.myCharz().cx += Res.abs(Res.random(0, 80));
			Service.gI().charMove();
		}
		GameScr.gI().loadGameScr();
		GameCanvas.loadBG(TileMap.bgID);
		Char.isLockKey = false;
		Res.outz("cy= " + Char.myCharz().cy + "---------------------------------------------");
		for (int i = 0; i < Char.myCharz().vEff.size(); i++)
		{
			EffectChar effectChar = (EffectChar)Char.myCharz().vEff.elementAt(i);
			if (effectChar.template.type == 10)
			{
				Char.isLockKey = true;
				break;
			}
		}
		GameCanvas.clearKeyHold();
		GameCanvas.clearKeyPressed();
		GameScr.gI().dHP = Char.myCharz().cHP;
		GameScr.gI().dMP = Char.myCharz().cMP;
		Char.ischangingMap = false;
		GameScr.gI().switchToMe();
		if (Char.myCharz().cy <= 10 && teleport3 != 0 && teleport3 != 2)
		{
			Teleport p = new Teleport(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 1, isMe: true, (teleport3 != 1) ? teleport3 : Char.myCharz().cgender);
			Teleport.addTeleport(p);
			Char.myCharz().isTeleport = true;
		}
		if (teleport3 == 2)
		{
			Char.myCharz().show();
		}
		if (GameScr.gI().isRongThanXuatHien)
		{
			if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
			{
				GameScr.gI().callRongThan(GameScr.gI().xR, GameScr.gI().yR);
			}
			if (mGraphics.zoomLevel > 1)
			{
				GameScr.gI().doiMauTroi();
			}
		}
		InfoDlg.hide();
		InfoDlg.show(TileMap.mapName, mResources.zone + " " + TileMap.zoneID, 30);
		GameCanvas.endDlg();
		GameCanvas.isLoading = false;
		Hint.clickMob();
		Hint.clickNpc();
		GameCanvas.debug("SA75x9", 2);
		GameCanvas.isRequestMapID = 2;
		GameCanvas.waitingTimeChangeMap = mSystem.currentTimeMillis() + 1000;
		Res.outz("[CONTROLLER] loadMap DONE!!!!!!!!!");
	}

	public void loadInfoMap(Message msg)
	{
		try
		{
			if (mGraphics.zoomLevel == 1)
			{
				SmallImage.clearHastable();
			}
			Char.myCharz().cx = (Char.myCharz().cxSend = (Char.myCharz().cxFocus = msg.reader().readShort()));
			Char.myCharz().cy = (Char.myCharz().cySend = (Char.myCharz().cyFocus = msg.reader().readShort()));
			Char.myCharz().xSd = Char.myCharz().cx;
			Char.myCharz().ySd = Char.myCharz().cy;
			Res.outz("head= " + Char.myCharz().head + " body= " + Char.myCharz().body + " left= " + Char.myCharz().leg + " x= " + Char.myCharz().cx + " y= " + Char.myCharz().cy + " chung toc= " + Char.myCharz().cgender);
			if (Char.myCharz().cx >= 0 && Char.myCharz().cx <= 100)
			{
				Char.myCharz().cdir = 1;
			}
			else if (Char.myCharz().cx >= TileMap.tmw - 100 && Char.myCharz().cx <= TileMap.tmw)
			{
				Char.myCharz().cdir = -1;
			}
			GameCanvas.debug("SA75x4", 2);
			int num = msg.reader().readByte();
			Res.outz("vGo size= " + num);
			if (!GameScr.info1.isDone)
			{
				GameScr.info1.cmx = Char.myCharz().cx - GameScr.cmx;
				GameScr.info1.cmy = Char.myCharz().cy - GameScr.cmy;
			}
			for (int i = 0; i < num; i++)
			{
				Waypoint waypoint = new Waypoint(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readUTF());
				if ((TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23) || waypoint.minX < 0 || waypoint.minX <= 24)
				{
				}
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			GameCanvas.debug("SA75x5", 2);
			num = msg.reader().readByte();
			Mob.newMob.removeAllElements();
			for (sbyte b = 0; b < num; b++)
			{
				Mob mob = new Mob(b, msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readLong(), msg.reader().readByte(), msg.reader().readLong(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readByte());
				mob.xSd = mob.x;
				mob.ySd = mob.y;
				mob.isBoss = msg.reader().readBoolean();
				if (Mob.arrMobTemplate[mob.templateId].type != 0)
				{
					if (b % 3 == 0)
					{
						mob.dir = -1;
					}
					else
					{
						mob.dir = 1;
					}
					mob.x += 10 - b % 20;
				}
				mob.isMobMe = false;
				BigBoss bigBoss = null;
				BachTuoc bachTuoc = null;
				BigBoss2 bigBoss2 = null;
				NewBoss newBoss = null;
				if (mob.templateId == 70)
				{
					bigBoss = new BigBoss(b, (short)mob.x, (short)mob.y, 70, mob.hp, mob.maxHp, mob.sys);
				}
				if (mob.templateId == 71)
				{
					bachTuoc = new BachTuoc(b, (short)mob.x, (short)mob.y, 71, mob.hp, mob.maxHp, mob.sys);
				}
				if (mob.templateId == 72)
				{
					bigBoss2 = new BigBoss2(b, (short)mob.x, (short)mob.y, 72, mob.hp, mob.maxHp, 3);
				}
				if (mob.isBoss)
				{
					newBoss = new NewBoss(b, (short)mob.x, (short)mob.y, mob.templateId, mob.hp, mob.maxHp, mob.sys);
				}
				if (newBoss != null)
				{
					GameScr.vMob.addElement(newBoss);
				}
				else if (bigBoss != null)
				{
					GameScr.vMob.addElement(bigBoss);
				}
				else if (bachTuoc != null)
				{
					GameScr.vMob.addElement(bachTuoc);
				}
				else if (bigBoss2 != null)
				{
					GameScr.vMob.addElement(bigBoss2);
				}
				else
				{
					GameScr.vMob.addElement(mob);
				}
			}
			if (Char.myCharz().mobMe != null && GameScr.findMobInMap(Char.myCharz().mobMe.mobId) == null)
			{
				Char.myCharz().mobMe.getData();
				Char.myCharz().mobMe.x = Char.myCharz().cx;
				Char.myCharz().mobMe.y = Char.myCharz().cy - 40;
				GameScr.vMob.addElement(Char.myCharz().mobMe);
			}
			num = msg.reader().readByte();
			for (byte b2 = 0; b2 < num; b2++)
			{
			}
			GameCanvas.debug("SA75x6", 2);
			num = msg.reader().readByte();
			Res.outz("NPC size= " + num);
			for (int j = 0; j < num; j++)
			{
				sbyte status = msg.reader().readByte();
				short cx = msg.reader().readShort();
				short num2 = msg.reader().readShort();
				sbyte b3 = msg.reader().readByte();
				short num3 = msg.reader().readShort();
				if (b3 != 6 && ((Char.myCharz().taskMaint.taskId >= 7 && (Char.myCharz().taskMaint.taskId != 7 || Char.myCharz().taskMaint.index > 1)) || (b3 != 7 && b3 != 8 && b3 != 9)) && (Char.myCharz().taskMaint.taskId >= 6 || b3 != 16))
				{
					if (b3 == 4)
					{
						GameScr.gI().magicTree = new MagicTree(j, status, cx, num2, b3, num3);
						Service.gI().magicTree(2);
						GameScr.vNpc.addElement(GameScr.gI().magicTree);
					}
					else
					{
						Npc o = new Npc(j, status, cx, num2 + 3, b3, num3);
						GameScr.vNpc.addElement(o);
					}
				}
			}
			GameCanvas.debug("SA75x7", 2);
			num = msg.reader().readByte();
			string empty = string.Empty;
			Res.outz("item size = " + num);
			empty = empty + "item: " + num;
			for (int k = 0; k < num; k++)
			{
				short itemMapID = msg.reader().readShort();
				short itemTemplateID = msg.reader().readShort();
				int x = msg.reader().readShort();
				int y = msg.reader().readShort();
				int num4 = msg.reader().readInt();
				short r = 0;
				if (num4 == -2)
				{
					r = msg.reader().readShort();
				}
				ItemMap itemMap = new ItemMap(num4, itemMapID, itemTemplateID, x, y, r);
				bool flag = false;
				for (int l = 0; l < GameScr.vItemMap.size(); l++)
				{
					ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(l);
					if (itemMap2.itemMapID == itemMap.itemMapID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GameScr.vItemMap.addElement(itemMap);
				}
				empty = empty + itemTemplateID + ",";
			}
			Res.err("sl item on map " + empty + "\n");
			TileMap.vCurrItem.removeAllElements();
			if (mGraphics.zoomLevel == 1)
			{
				BgItem.clearHashTable();
			}
			BgItem.vKeysNew.removeAllElements();
			if (!GameCanvas.lowGraphic || (GameCanvas.lowGraphic && TileMap.isVoDaiMap()) || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48 || TileMap.mapID == 120 || TileMap.mapID == 128 || TileMap.mapID == 170 || TileMap.mapID == 49)
			{
				short num5 = msg.reader().readShort();
				empty = "item high graphic: ";
				for (int m = 0; m < num5; m++)
				{
					short id = msg.reader().readShort();
					short num6 = msg.reader().readShort();
					short num7 = msg.reader().readShort();
					if (TileMap.getBIById(id) != null)
					{
						BgItem bIById = TileMap.getBIById(id);
						BgItem bgItem = new BgItem();
						bgItem.id = id;
						bgItem.idImage = bIById.idImage;
						bgItem.dx = bIById.dx;
						bgItem.dy = bIById.dy;
						bgItem.x = num6 * TileMap.size;
						bgItem.y = num7 * TileMap.size;
						bgItem.layer = bIById.layer;
						if (TileMap.isExistMoreOne(bgItem.id))
						{
							bgItem.trans = ((m % 2 != 0) ? 2 : 0);
							if (TileMap.mapID == 45)
							{
								bgItem.trans = 0;
							}
						}
						Image image = null;
						if (!BgItem.imgNew.containsKey(bgItem.idImage + string.Empty))
						{
							if (mGraphics.zoomLevel == 1)
							{
								image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
								if (image == null)
								{
									image = Image.createRGBImage(new int[1], 1, 1, bl: true);
									Service.gI().getBgTemplate(bgItem.idImage);
								}
								BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
							}
							else
							{
								bool flag2 = false;
								sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "bgItem" + bgItem.idImage);
								if (array != null)
								{
									if (BgItem.newSmallVersion != null)
									{
										Res.outz("Small  last= " + array.Length % 127 + "new Version= " + BgItem.newSmallVersion[bgItem.idImage]);
										if (array.Length % 127 != BgItem.newSmallVersion[bgItem.idImage])
										{
											flag2 = true;
										}
									}
									if (!flag2)
									{
										image = Image.createImage(array, 0, array.Length);
										if (image != null)
										{
											BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
										}
										else
										{
											flag2 = true;
										}
									}
								}
								else
								{
									flag2 = true;
								}
								if (flag2)
								{
									image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
									if (image == null)
									{
										image = Image.createRGBImage(new int[1], 1, 1, bl: true);
										Service.gI().getBgTemplate(bgItem.idImage);
									}
									BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
								}
							}
							BgItem.vKeysLast.addElement(bgItem.idImage + string.Empty);
						}
						if (!BgItem.isExistKeyNews(bgItem.idImage + string.Empty))
						{
							BgItem.vKeysNew.addElement(bgItem.idImage + string.Empty);
						}
						bgItem.changeColor();
						TileMap.vCurrItem.addElement(bgItem);
					}
					empty = empty + id + ",";
				}
				Res.err("item High Graphics: " + empty);
				for (int n = 0; n < BgItem.vKeysLast.size(); n++)
				{
					string text = (string)BgItem.vKeysLast.elementAt(n);
					if (!BgItem.isExistKeyNews(text))
					{
						BgItem.imgNew.remove(text);
						if (BgItem.imgNew.containsKey(text + "blend" + 1))
						{
							BgItem.imgNew.remove(text + "blend" + 1);
						}
						if (BgItem.imgNew.containsKey(text + "blend" + 3))
						{
							BgItem.imgNew.remove(text + "blend" + 3);
						}
						BgItem.vKeysLast.removeElementAt(n);
						n--;
					}
				}
				BackgroudEffect.isFog = false;
				BackgroudEffect.nCloud = 0;
				EffecMn.vEff.removeAllElements();
				BackgroudEffect.vBgEffect.removeAllElements();
				Effect.newEff.removeAllElements();
				short num8 = msg.reader().readShort();
				for (int num9 = 0; num9 < num8; num9++)
				{
					string key = msg.reader().readUTF();
					string value = msg.reader().readUTF();
					keyValueAction(key, value);
				}
			}
			else
			{
				short num10 = msg.reader().readShort();
				for (int num11 = 0; num11 < num10; num11++)
				{
					short num12 = msg.reader().readShort();
					short num13 = msg.reader().readShort();
					short num14 = msg.reader().readShort();
				}
				short num15 = msg.reader().readShort();
				for (int num16 = 0; num16 < num15; num16++)
				{
					string text2 = msg.reader().readUTF();
					string text3 = msg.reader().readUTF();
				}
			}
			TileMap.bgType = msg.reader().readByte();
			sbyte teleport = msg.reader().readByte();
			loadCurrMap(teleport);
			GameCanvas.debug("SA75x8", 2);
		}
		catch (Exception)
		{
			Res.err(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Loadmap khong thanh cong");
			GameCanvas.instance.doResetToLoginScr(GameCanvas.serverScreen);
			ServerListScreen.waitToLogin = true;
			GameCanvas.endDlg();
		}
		GameCanvas.isLoading = false;
		Res.err(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Loadmap thanh cong");
	}

	public void keyValueAction(string key, string value)
	{
		if (key.Equals("eff"))
		{
			if (Panel.graphics > 0)
			{
				return;
			}
			string[] array = Res.split(value, ".", 0);
			int id = int.Parse(array[0]);
			int layer = int.Parse(array[1]);
			int x = int.Parse(array[2]);
			int y = int.Parse(array[3]);
			int loop;
			int loopCount;
			if (array.Length <= 4)
			{
				loop = -1;
				loopCount = 1;
			}
			else
			{
				loop = int.Parse(array[4]);
				loopCount = int.Parse(array[5]);
			}
			Effect effect = new Effect(id, x, y, layer, loop, loopCount);
			if (array.Length > 6)
			{
				effect.typeEff = int.Parse(array[6]);
				if (array.Length > 7)
				{
					effect.indexFrom = int.Parse(array[7]);
					effect.indexTo = int.Parse(array[8]);
				}
			}
			EffecMn.addEff(effect);
		}
		else if (key.Equals("beff") && Panel.graphics <= 1)
		{
			BackgroudEffect.addEffect(int.Parse(value));
		}
	}

	public void messageNotMap(Message msg)
	{
		GameCanvas.debug("SA6", 2);
		try
		{
			sbyte b = msg.reader().readByte();
			Res.outz("---messageNotMap : " + b);
			switch (b)
			{
			case 16:
				MoneyCharge.gI().switchToMe();
				break;
			case 17:
				GameCanvas.debug("SYB123", 2);
				Char.myCharz().clearTask();
				break;
			case 18:
			{
				GameCanvas.isLoading = false;
				GameCanvas.endDlg();
				int num2 = msg.reader().readInt();
				GameCanvas.inputDlg.show(mResources.changeNameChar, new Command(mResources.OK, GameCanvas.instance, 88829, num2), TField.INPUT_TYPE_ANY);
				break;
			}
			case 20:
				Char.myCharz().cPk = msg.reader().readByte();
				GameScr.info1.addInfo(mResources.PK_NOW + " " + Char.myCharz().cPk, 0);
				break;
			case 35:
				GameCanvas.endDlg();
				GameScr.gI().resetButton();
				GameScr.info1.addInfo(msg.reader().readUTF(), 0);
				break;
			case 36:
				GameScr.typeActive = msg.reader().readByte();
				Res.outz("load Me Active: " + GameScr.typeActive);
				break;
			case 4:
			{
				GameCanvas.debug("SA8", 2);
				GameCanvas.loginScr.savePass();
				GameScr.isAutoPlay = false;
				GameScr.canAutoPlay = false;
				LoginScr.isUpdateAll = true;
				LoginScr.isUpdateData = true;
				LoginScr.isUpdateMap = true;
				LoginScr.isUpdateSkill = true;
				LoginScr.isUpdateItem = true;
				GameScr.vsData = msg.reader().readByte();
				GameScr.vsMap = msg.reader().readByte();
				GameScr.vsSkill = msg.reader().readByte();
				GameScr.vsItem = msg.reader().readByte();
				sbyte b2 = msg.reader().readByte();
				if (GameCanvas.loginScr.isLogin2)
				{
					Rms.saveRMSString(Rms.RMS_acc, string.Empty);
					Rms.saveRMSString(Rms.RMS_pass, string.Empty);
				}
				else
				{
					Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, string.Empty);
				}
				if (GameScr.vsData != GameScr.vcData)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateData();
				}
				else
				{
					try
					{
						LoginScr.isUpdateData = false;
					}
					catch (Exception)
					{
						GameScr.vcData = -1;
						Service.gI().updateData();
					}
				}
				if (GameScr.vsMap != GameScr.vcMap)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateMap();
				}
				else
				{
					try
					{
						if (!GameScr.isLoadAllData)
						{
							DataInputStream dataInputStream = new DataInputStream(Rms.loadRMS("NRmap"));
							createMap(dataInputStream.r);
						}
						LoginScr.isUpdateMap = false;
					}
					catch (Exception)
					{
						GameScr.vcMap = -1;
						Service.gI().updateMap();
					}
				}
				if (GameScr.vsSkill != GameScr.vcSkill)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateSkill();
				}
				else
				{
					try
					{
						if (!GameScr.isLoadAllData)
						{
							DataInputStream dataInputStream2 = new DataInputStream(Rms.loadRMS("NRskill"));
							createSkill(dataInputStream2.r);
						}
						LoginScr.isUpdateSkill = false;
					}
					catch (Exception)
					{
						GameScr.vcSkill = -1;
						Service.gI().updateSkill();
					}
				}
				if (GameScr.vsItem != GameScr.vcItem)
				{
					GameScr.isLoadAllData = false;
					Service.gI().updateItem();
				}
				else
				{
					try
					{
						DataInputStream dataInputStream3 = new DataInputStream(Rms.loadRMS("NRitem0"));
						loadItemNew(dataInputStream3.r, 0, isSave: false);
						DataInputStream dataInputStream4 = new DataInputStream(Rms.loadRMS("NRitem1"));
						loadItemNew(dataInputStream4.r, 1, isSave: false);
						DataInputStream dataInputStream5 = new DataInputStream(Rms.loadRMS("NRitem100"));
						loadItemNew(dataInputStream5.r, 100, isSave: false);
						LoginScr.isUpdateItem = false;
					}
					catch (Exception)
					{
						GameScr.vcItem = -1;
						Service.gI().updateItem();
					}
					try
					{
						DataInputStream dataInputStream6 = new DataInputStream(Rms.loadRMS("NRitem101"));
						loadItemNew(dataInputStream6.r, 101, isSave: false);
					}
					catch (Exception)
					{
					}
				}
				if (!GameScr.isLoadAllData)
				{
					GameScr.gI().readOk();
				}
				else
				{
					Service.gI().clientOk();
				}
				sbyte b3 = msg.reader().readByte();
				Res.outz("CAPTION LENT= " + b3);
				GameScr.exps = new long[b3];
				for (int j = 0; j < GameScr.exps.Length; j++)
				{
					GameScr.exps[j] = msg.reader().readLong();
				}
				break;
			}
			case 6:
			{
				Res.outz("GET UPDATE_MAP " + msg.reader().available() + " bytes");
				msg.reader().mark(500000);
				createMap(msg.reader());
				msg.reader().reset();
				sbyte[] data3 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data3);
				Rms.saveRMS("NRmap", data3);
				sbyte[] data4 = new sbyte[1] { GameScr.vcMap };
				Rms.saveRMS("NRmapVersion", data4);
				LoginScr.isUpdateMap = false;
				GameScr.gI().readOk();
				break;
			}
			case 7:
			{
				Res.outz("GET UPDATE_SKILL " + msg.reader().available() + " bytes");
				msg.reader().mark(500000);
				createSkill(msg.reader());
				msg.reader().reset();
				sbyte[] data = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data);
				Rms.saveRMS("NRskill", data);
				sbyte[] data2 = new sbyte[1] { GameScr.vcSkill };
				Rms.saveRMS("NRskillVersion", data2);
				LoginScr.isUpdateSkill = false;
				GameScr.gI().readOk();
				break;
			}
			case 8:
				Res.outz("GET UPDATE_ITEM " + msg.reader().available() + " bytes");
				createItemNew(msg.reader());
				break;
			case 10:
				try
				{
					Char.isLoadingMap = true;
					Res.outz("REQUEST MAP TEMPLATE");
					GameCanvas.isLoading = true;
					TileMap.maps = null;
					TileMap.types = null;
					mSystem.gcc();
					GameCanvas.debug("SA99", 2);
					TileMap.tmw = msg.reader().readByte();
					TileMap.tmh = msg.reader().readByte();
					TileMap.maps = new int[TileMap.tmw * TileMap.tmh];
					Res.err("   M apsize= " + TileMap.tmw * TileMap.tmh);
					for (int i = 0; i < TileMap.maps.Length; i++)
					{
						int num = msg.reader().readByte();
						if (num < 0)
						{
							num += 256;
						}
						TileMap.maps[i] = (ushort)num;
					}
					TileMap.types = new int[TileMap.maps.Length];
					msg = messWait;
					loadInfoMap(msg);
					try
					{
						TileMap.isMapDouble = ((msg.reader().readByte() != 0) ? true : false);
					}
					catch (Exception ex)
					{
						Res.err(" 1 LOI TAI CASE REQUEST_MAPTEMPLATE " + ex.ToString());
					}
				}
				catch (Exception ex2)
				{
					Res.err("2 LOI TAI CASE REQUEST_MAPTEMPLATE " + ex2.ToString());
				}
				msg.cleanup();
				messWait.cleanup();
				msg = (messWait = null);
				GameScr.gI().switchToMe();
				break;
			case 9:
				GameCanvas.debug("SA11", 2);
				break;
			}
		}
		catch (Exception ex8)
		{
			Cout.LogError("LOI TAI messageNotMap=== " + msg.command + "  >>" + ex8.StackTrace);
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public void messageNotLogin(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			Res.outz("---messageNotLogin : " + b);
			if (b == 2)
			{
				string linkDefault = msg.reader().readUTF();
				Res.outz(">>Get CLIENT_INFO");
				ServerListScreen.linkDefault = linkDefault;
				mSystem.AddIpTest();
				ServerListScreen.getServerList(ServerListScreen.linkDefault);
				try
				{
					sbyte b2 = msg.reader().readByte();
					Panel.CanNapTien = b2 == 1;
				}
				catch (Exception)
				{
				}
				isGet_CLIENT_INFO = true;
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public void messageSubCommand(Message msg)
	{
		try
		{
			GameCanvas.debug("SA12", 2);
			sbyte b = msg.reader().readByte();
			Res.outz("---messageSubCommand : " + b);
			switch (b)
			{
			case 63:
			{
				sbyte b4 = msg.reader().readByte();
				if (b4 > 0)
				{
					GameCanvas.panel.vPlayerMenu_id.removeAllElements();
					InfoDlg.showWait();
					MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
					for (int i = 0; i < b4; i++)
					{
						string caption = msg.reader().readUTF();
						string caption2 = msg.reader().readUTF();
						short menuSelect = msg.reader().readShort();
						GameCanvas.panel.vPlayerMenu_id.addElement(menuSelect + string.Empty);
						Char.myCharz().charFocus.menuSelect = menuSelect;
						Command command = new Command(caption, 11115, Char.myCharz().charFocus);
						command.caption2 = caption2;
						vPlayerMenu.addElement(command);
					}
					InfoDlg.hide();
					GameCanvas.panel.setTabPlayerMenu();
				}
				break;
			}
			case 1:
				GameCanvas.debug("SA13", 2);
				Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.myCharz().cTiemNang = msg.reader().readLong();
				Char.myCharz().vSkill.removeAllElements();
				Char.myCharz().vSkillFight.removeAllElements();
				Char.myCharz().myskill = null;
				break;
			case 2:
			{
				GameCanvas.debug("SA14", 2);
				if (Char.myCharz().statusMe != 14 && Char.myCharz().statusMe != 5)
				{
					Char.myCharz().cHP = Char.myCharz().cHPFull;
					Char.myCharz().cMP = Char.myCharz().cMPFull;
					Cout.LogError2(" ME_LOAD_SKILL");
				}
				Char.myCharz().vSkill.removeAllElements();
				Char.myCharz().vSkillFight.removeAllElements();
				sbyte b2 = msg.reader().readByte();
				for (sbyte b3 = 0; b3 < b2; b3++)
				{
					short skillId = msg.reader().readShort();
					Skill skill2 = Skills.get(skillId);
					useSkill(skill2);
				}
				GameScr.gI().sortSkill();
				if (GameScr.isPaintInfoMe)
				{
					GameScr.indexRow = -1;
					GameScr.gI().left = (GameScr.gI().center = null);
				}
				break;
			}
			case 19:
				GameCanvas.debug("SA17", 2);
				Char.myCharz().boxSort();
				break;
			case 21:
			{
				GameCanvas.debug("SA19", 2);
				int num4 = msg.reader().readInt();
				Char.myCharz().xuInBox -= num4;
				Char.myCharz().xu += num4;
				Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
				break;
			}
			case 0:
			{
				GameCanvas.debug("SA21", 2);
				RadarScr.list = new MyVector();
				Teleport.vTeleport.removeAllElements();
				GameScr.vCharInMap.removeAllElements();
				GameScr.vItemMap.removeAllElements();
				Char.vItemTime.removeAllElements();
				GameScr.loadImg();
				GameScr.currentCharViewInfo = Char.myCharz();
				Char.myCharz().charID = msg.reader().readInt();
				Char.myCharz().ctaskId = msg.reader().readByte();
				Char.myCharz().cgender = msg.reader().readByte();
				Char.myCharz().head = msg.reader().readShort();
				Char.myCharz().cName = msg.reader().readUTF();
				Char.myCharz().cPk = msg.reader().readByte();
				Char.myCharz().cTypePk = msg.reader().readByte();
				Char.myCharz().cPower = msg.reader().readLong();
				Char.myCharz().applyCharLevelPercent();
				Char.myCharz().eff5BuffHp = msg.reader().readShort();
				Char.myCharz().eff5BuffMp = msg.reader().readShort();
				Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.myCharz().vSkill.removeAllElements();
				Char.myCharz().vSkillFight.removeAllElements();
				GameScr.gI().dHP = Char.myCharz().cHP;
				GameScr.gI().dMP = Char.myCharz().cMP;
				sbyte b6 = msg.reader().readByte();
				for (sbyte b7 = 0; b7 < b6; b7++)
				{
					Skill skill3 = Skills.get(msg.reader().readShort());
					useSkill(skill3);
				}
				GameScr.gI().sortSkill();
				GameScr.gI().loadSkillShortcut();
				Char.myCharz().xu = msg.reader().readLong();
				Char.myCharz().luongKhoa = msg.reader().readInt();
				Char.myCharz().luong = msg.reader().readInt();
				Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
				Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
				Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
				Char.myCharz().arrItemBody = new Item[msg.reader().readByte()];
				try
				{
					Char.myCharz().setDefaultPart();
					for (int k = 0; k < Char.myCharz().arrItemBody.Length; k++)
					{
						short num5 = msg.reader().readShort();
						if (num5 == -1)
						{
							continue;
						}
						ItemTemplate itemTemplate = ItemTemplates.get(num5);
						int type = itemTemplate.type;
						Char.myCharz().arrItemBody[k] = new Item();
						Char.myCharz().arrItemBody[k].template = itemTemplate;
						Char.myCharz().arrItemBody[k].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBody[k].info = msg.reader().readUTF();
						Char.myCharz().arrItemBody[k].content = msg.reader().readUTF();
						int num6 = msg.reader().readUnsignedByte();
						if (num6 != 0)
						{
							Char.myCharz().arrItemBody[k].itemOption = new ItemOption[num6];
							for (int l = 0; l < Char.myCharz().arrItemBody[k].itemOption.Length; l++)
							{
								ItemOption itemOption = readItemOption(msg);
								if (itemOption != null)
								{
									Char.myCharz().arrItemBody[k].itemOption[l] = itemOption;
								}
							}
						}
						switch (type)
						{
						case 0:
							Res.outz("toi day =======================================" + Char.myCharz().body);
							Char.myCharz().body = Char.myCharz().arrItemBody[k].template.part;
							break;
						case 1:
							Char.myCharz().leg = Char.myCharz().arrItemBody[k].template.part;
							Res.outz("toi day =======================================" + Char.myCharz().leg);
							break;
						}
					}
				}
				catch (Exception)
				{
				}
				Char.myCharz().arrItemBag = new Item[msg.reader().readByte()];
				GameScr.hpPotion = 0;
				GameScr.isudungCapsun4 = false;
				GameScr.isudungCapsun3 = false;
				for (int m = 0; m < Char.myCharz().arrItemBag.Length; m++)
				{
					short num7 = msg.reader().readShort();
					if (num7 == -1)
					{
						continue;
					}
					Char.myCharz().arrItemBag[m] = new Item();
					Char.myCharz().arrItemBag[m].template = ItemTemplates.get(num7);
					Char.myCharz().arrItemBag[m].quantity = msg.reader().readInt();
					Char.myCharz().arrItemBag[m].info = msg.reader().readUTF();
					Char.myCharz().arrItemBag[m].content = msg.reader().readUTF();
					Char.myCharz().arrItemBag[m].indexUI = m;
					sbyte b8 = msg.reader().readByte();
					if (b8 != 0)
					{
						Char.myCharz().arrItemBag[m].itemOption = new ItemOption[b8];
						for (int n = 0; n < Char.myCharz().arrItemBag[m].itemOption.Length; n++)
						{
							ItemOption itemOption2 = readItemOption(msg);
							if (itemOption2 != null)
							{
								Char.myCharz().arrItemBag[m].itemOption[n] = itemOption2;
								Char.myCharz().arrItemBag[m].getCompare();
							}
						}
					}
					if (Char.myCharz().arrItemBag[m].template.type == 6)
					{
						GameScr.hpPotion += Char.myCharz().arrItemBag[m].quantity;
					}
					switch (num7)
					{
					case 194:
						GameScr.isudungCapsun4 = Char.myCharz().arrItemBag[m].quantity > 0;
						break;
					case 193:
						if (!GameScr.isudungCapsun4)
						{
							GameScr.isudungCapsun3 = Char.myCharz().arrItemBag[m].quantity > 0;
						}
						break;
					}
				}
				Char.myCharz().arrItemBox = new Item[msg.reader().readByte()];
				GameCanvas.panel.hasUse = 0;
				for (int num8 = 0; num8 < Char.myCharz().arrItemBox.Length; num8++)
				{
					short num9 = msg.reader().readShort();
					if (num9 == -1)
					{
						continue;
					}
					Char.myCharz().arrItemBox[num8] = new Item();
					Char.myCharz().arrItemBox[num8].template = ItemTemplates.get(num9);
					Char.myCharz().arrItemBox[num8].quantity = msg.reader().readInt();
					Char.myCharz().arrItemBox[num8].info = msg.reader().readUTF();
					Char.myCharz().arrItemBox[num8].content = msg.reader().readUTF();
					Char.myCharz().arrItemBox[num8].itemOption = new ItemOption[msg.reader().readByte()];
					for (int num10 = 0; num10 < Char.myCharz().arrItemBox[num8].itemOption.Length; num10++)
					{
						ItemOption itemOption3 = readItemOption(msg);
						if (itemOption3 != null)
						{
							Char.myCharz().arrItemBox[num8].itemOption[num10] = itemOption3;
							Char.myCharz().arrItemBox[num8].getCompare();
						}
					}
					GameCanvas.panel.hasUse++;
				}
				Char.myCharz().statusMe = 4;
				int num11 = Rms.loadRMSInt(Char.myCharz().cName + "vci");
				if (num11 < 1)
				{
					GameScr.isViewClanInvite = false;
				}
				else
				{
					GameScr.isViewClanInvite = true;
				}
				short num12 = msg.reader().readShort();
				Char.idHead = new short[num12];
				Char.idAvatar = new short[num12];
				for (int num13 = 0; num13 < num12; num13++)
				{
					Char.idHead[num13] = msg.reader().readShort();
					Char.idAvatar[num13] = msg.reader().readShort();
				}
				for (int num14 = 0; num14 < GameScr.info1.charId.Length; num14++)
				{
					GameScr.info1.charId[num14] = new int[3];
				}
				GameScr.info1.charId[Char.myCharz().cgender][0] = msg.reader().readShort();
				GameScr.info1.charId[Char.myCharz().cgender][1] = msg.reader().readShort();
				GameScr.info1.charId[Char.myCharz().cgender][2] = msg.reader().readShort();
				Char.myCharz().isNhapThe = msg.reader().readByte() == 1;
				Res.outz("NHAP THE= " + Char.myCharz().isNhapThe);
				GameScr.deltaTime = mSystem.currentTimeMillis() - (long)msg.reader().readInt() * 1000L;
				GameScr.isNewMember = msg.reader().readByte();
				Service.gI().updateCaption((sbyte)Char.myCharz().cgender);
				Service.gI().androidPack();
				try
				{
					Char.myCharz().idAuraEff = msg.reader().readShort();
					Char.myCharz().idEff_Set_Item = msg.reader().readSByte();
					Char.myCharz().idHat = msg.reader().readShort();
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
			case 4:
				GameCanvas.debug("SA23", 2);
				Char.myCharz().xu = msg.reader().readLong();
				Char.myCharz().luong = msg.reader().readInt();
				Char.myCharz().cHP = msg.reader().readLong();
				Char.myCharz().cMP = msg.reader().readLong();
				Char.myCharz().luongKhoa = msg.reader().readInt();
				Char.myCharz().xuStr = Res.formatNumber2(Char.myCharz().xu);
				Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
				Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
				break;
			case 5:
			{
				GameCanvas.debug("SA24", 2);
				long cHP = Char.myCharz().cHP;
				Char.myCharz().cHP = msg.reader().readLong();
				if (Char.myCharz().cHP > cHP && Char.myCharz().cTypePk != 4)
				{
					GameScr.startFlyText("+" + (Char.myCharz().cHP - cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
					SoundMn.gI().HP_MPup();
					if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5003)
					{
						MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, isBoss: true, -1L, -1L, Char.myCharz(), 29);
					}
				}
				if (Char.myCharz().cHP < cHP)
				{
					GameScr.startFlyText("-" + (cHP - Char.myCharz().cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
				}
				GameScr.gI().dHP = Char.myCharz().cHP;
				if (!GameScr.isPaintInfoMe)
				{
				}
				break;
			}
			case 6:
			{
				GameCanvas.debug("SA25", 2);
				if (Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5)
				{
					break;
				}
				long cMP = Char.myCharz().cMP;
				Char.myCharz().cMP = msg.reader().readLong();
				if (Char.myCharz().cMP > cMP)
				{
					GameScr.startFlyText("+" + (Char.myCharz().cMP - cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
					SoundMn.gI().HP_MPup();
					if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5001)
					{
						MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, isBoss: true, -1L, -1L, Char.myCharz(), 29);
					}
				}
				if (Char.myCharz().cMP < cMP)
				{
					GameScr.startFlyText("-" + (cMP - Char.myCharz().cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
				}
				Res.outz("curr MP= " + Char.myCharz().cMP);
				GameScr.gI().dMP = Char.myCharz().cMP;
				if (!GameScr.isPaintInfoMe)
				{
				}
				break;
			}
			case 7:
			{
				Char obj9 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj9 != null)
				{
					obj9.clanID = msg.reader().readInt();
					if (obj9.clanID == -2)
					{
						obj9.isCopy = true;
					}
					readCharInfo(obj9, msg);
					try
					{
						obj9.idAuraEff = msg.reader().readShort();
						obj9.idEff_Set_Item = msg.reader().readSByte();
						obj9.idHat = msg.reader().readShort();
						Effect.GetCharEff(obj9);
						break;
					}
					catch (Exception)
					{
						break;
					}
				}
				break;
			}
			case 8:
			{
				GameCanvas.debug("SA26", 2);
				Char obj10 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj10 != null)
				{
					obj10.cspeed = msg.reader().readByte();
				}
				break;
			}
			case 9:
			{
				GameCanvas.debug("SA27", 2);
				Char obj8 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj8 != null)
				{
					obj8.cHP = msg.reader().readLong();
					obj8.cHPFull = msg.reader().readLong();
				}
				break;
			}
			case 10:
			{
				GameCanvas.debug("SA28", 2);
				Char obj5 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj5 != null)
				{
					obj5.cHP = msg.reader().readLong();
					obj5.cHPFull = msg.reader().readLong();
					obj5.eff5BuffHp = msg.reader().readShort();
					obj5.eff5BuffMp = msg.reader().readShort();
					obj5.wp = msg.reader().readShort();
					if (obj5.wp == -1)
					{
						obj5.setDefaultWeapon();
					}
				}
				break;
			}
			case 11:
			{
				GameCanvas.debug("SA29", 2);
				Char obj2 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj2 != null)
				{
					obj2.cHP = msg.reader().readLong();
					obj2.cHPFull = msg.reader().readLong();
					obj2.eff5BuffHp = msg.reader().readShort();
					obj2.eff5BuffMp = msg.reader().readShort();
					obj2.body = msg.reader().readShort();
					if (obj2.body == -1)
					{
						obj2.setDefaultBody();
					}
				}
				break;
			}
			case 12:
			{
				GameCanvas.debug("SA30", 2);
				Char obj11 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj11 != null)
				{
					obj11.cHP = msg.reader().readLong();
					obj11.cHPFull = msg.reader().readLong();
					obj11.eff5BuffHp = msg.reader().readShort();
					obj11.eff5BuffMp = msg.reader().readShort();
					obj11.leg = msg.reader().readShort();
					if (obj11.leg == -1)
					{
						obj11.setDefaultLeg();
					}
				}
				break;
			}
			case 13:
			{
				GameCanvas.debug("SA31", 2);
				int num2 = msg.reader().readInt();
				Char obj = ((num2 != Char.myCharz().charID) ? GameScr.findCharInMap(num2) : Char.myCharz());
				if (obj != null)
				{
					obj.cHP = msg.reader().readLong();
					obj.cHPFull = msg.reader().readLong();
					obj.eff5BuffHp = msg.reader().readShort();
					obj.eff5BuffMp = msg.reader().readShort();
				}
				break;
			}
			case 14:
			{
				GameCanvas.debug("SA32", 2);
				Char obj4 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj4 == null)
				{
					break;
				}
				obj4.cHP = msg.reader().readLong();
				sbyte b5 = msg.reader().readByte();
				Res.outz("player load hp type= " + b5);
				if (b5 == 1)
				{
					ServerEffect.addServerEffect(11, obj4, 5);
					ServerEffect.addServerEffect(104, obj4, 4);
				}
				if (b5 == 2)
				{
					obj4.doInjure();
				}
				try
				{
					obj4.cHPFull = msg.reader().readLong();
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
			case 15:
			{
				GameCanvas.debug("SA33", 2);
				Char obj3 = GameScr.findCharInMap(msg.reader().readInt());
				if (obj3 != null)
				{
					obj3.cHP = msg.reader().readLong();
					obj3.cHPFull = msg.reader().readLong();
					obj3.cx = msg.reader().readShort();
					obj3.cy = msg.reader().readShort();
					obj3.statusMe = 1;
					obj3.cp3 = 3;
					ServerEffect.addServerEffect(109, obj3, 2);
				}
				break;
			}
			case 35:
			{
				GameCanvas.debug("SY3", 2);
				int num3 = msg.reader().readInt();
				Res.outz("CID = " + num3);
				if (TileMap.mapID == 130)
				{
					GameScr.gI().starVS();
				}
				if (num3 == Char.myCharz().charID)
				{
					Char.myCharz().cTypePk = msg.reader().readByte();
					if (GameScr.gI().isVS() && Char.myCharz().cTypePk != 0)
					{
						GameScr.gI().starVS();
					}
					Res.outz("type pk= " + Char.myCharz().cTypePk);
					Char.myCharz().npcFocus = null;
					if (!GameScr.gI().isMeCanAttackMob(Char.myCharz().mobFocus))
					{
						Char.myCharz().mobFocus = null;
					}
					Char.myCharz().itemFocus = null;
				}
				else
				{
					Char obj6 = GameScr.findCharInMap(num3);
					if (obj6 != null)
					{
						Res.outz("type pk= " + obj6.cTypePk);
						obj6.cTypePk = msg.reader().readByte();
						if (obj6.isAttacPlayerStatus())
						{
							Char.myCharz().charFocus = obj6;
						}
					}
				}
				for (int j = 0; j < GameScr.vCharInMap.size(); j++)
				{
					Char obj7 = GameScr.findCharInMap(j);
					if (obj7 != null && obj7.cTypePk != 0 && obj7.cTypePk == Char.myCharz().cTypePk)
					{
						if (!Char.myCharz().mobFocus.isMobMe)
						{
							Char.myCharz().mobFocus = null;
						}
						Char.myCharz().npcFocus = null;
						Char.myCharz().itemFocus = null;
						break;
					}
				}
				Res.outz("update type pk= ");
				break;
			}
			case 61:
			{
				string text = msg.reader().readUTF();
				sbyte[] data = new sbyte[msg.reader().readInt()];
				msg.reader().read(ref data);
				if (data.Length == 0)
				{
					data = null;
				}
				if (text.Equals("KSkill"))
				{
					GameScr.gI().onKSkill(data);
				}
				else if (text.Equals("OSkill"))
				{
					GameScr.gI().onOSkill(data);
				}
				else if (text.Equals("CSkill"))
				{
					GameScr.gI().onCSkill(data);
				}
				break;
			}
			case 23:
			{
				short num = msg.reader().readShort();
				Skill skill = Skills.get(num);
				useSkill(skill);
				if (num != 0 && num != 14 && num != 28)
				{
					GameScr.info1.addInfo(mResources.LEARN_SKILL + " " + skill.template.name, 0);
				}
				break;
			}
			case 62:
				Res.outz("ME UPDATE SKILL");
				read_UpdateSkill(msg);
				break;
			}
		}
		catch (Exception ex5)
		{
			Cout.println("Loi tai Sub : " + ex5.ToString());
		}
		finally
		{
			msg?.cleanup();
		}
	}

	private void useSkill(Skill skill)
	{
		if (Char.myCharz().myskill == null)
		{
			Char.myCharz().myskill = skill;
		}
		else if (skill.template.Equals(Char.myCharz().myskill.template))
		{
			Char.myCharz().myskill = skill;
		}
		Char.myCharz().vSkill.addElement(skill);
		if ((skill.template.type == 1 || skill.template.type == 4 || skill.template.type == 2 || skill.template.type == 3) && (skill.template.maxPoint == 0 || (skill.template.maxPoint > 0 && skill.point > 0)))
		{
			if (skill.template.id == Char.myCharz().skillTemplateId)
			{
				Service.gI().selectSkill(Char.myCharz().skillTemplateId);
			}
			Char.myCharz().vSkillFight.addElement(skill);
		}
	}

	public bool readCharInfo(Char c, Message msg)
	{
		try
		{
			c.clevel = msg.reader().readByte();
			c.isInvisiblez = msg.reader().readBoolean();
			c.cTypePk = msg.reader().readByte();
			Res.outz("ADD TYPE PK= " + c.cTypePk + " to player " + c.charID + " @@ " + c.cName);
			c.nClass = GameScr.nClasss[msg.reader().readByte()];
			c.cgender = msg.reader().readByte();
			c.head = msg.reader().readShort();
			c.cName = msg.reader().readUTF();
			c.cHP = msg.reader().readLong();
			c.dHP = c.cHP;
			if (c.cHP == 0)
			{
				c.statusMe = 14;
			}
			c.cHPFull = msg.reader().readLong();
			if (c.cy >= TileMap.pxh - 100)
			{
				c.isFlyUp = true;
			}
			c.body = msg.reader().readShort();
			c.leg = msg.reader().readShort();
			c.bag = msg.reader().readShort();
			Res.outz(" body= " + c.body + " leg= " + c.leg + " bag=" + c.bag + "BAG ==" + c.bag + "*********************************");
			c.isShadown = true;
			sbyte b = msg.reader().readByte();
			if (c.wp == -1)
			{
				c.setDefaultWeapon();
			}
			if (c.body == -1)
			{
				c.setDefaultBody();
			}
			if (c.leg == -1)
			{
				c.setDefaultLeg();
			}
			c.cx = msg.reader().readShort();
			c.cy = msg.reader().readShort();
			c.xSd = c.cx;
			c.ySd = c.cy;
			c.eff5BuffHp = msg.reader().readShort();
			c.eff5BuffMp = msg.reader().readShort();
			int num = msg.reader().readByte();
			for (int i = 0; i < num; i++)
			{
				EffectChar effectChar = new EffectChar(msg.reader().readByte(), msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
				c.vEff.addElement(effectChar);
				if (effectChar.template.type == 12 || effectChar.template.type == 11)
				{
					c.isInvisiblez = true;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			ex.StackTrace.ToString();
		}
		return false;
	}

	private void readGetImgByName(Message msg)
	{
		try
		{
			string name = msg.reader().readUTF();
			sbyte nFrame = msg.reader().readByte();
			sbyte[] array = null;
			array = NinjaUtil.readByteArray(msg);
			Image img = createImage(array);
			ImgByName.SetImage(name, img, nFrame);
			if (array != null)
			{
			}
		}
		catch (Exception)
		{
		}
	}

	private void createItemNew(myReader d)
	{
		try
		{
			loadItemNew(d, -1, isSave: true);
		}
		catch (Exception)
		{
		}
	}

	private void loadItemNew(myReader d, sbyte type, bool isSave)
	{
		try
		{
			d.mark(1000000);
			GameScr.vcItem = d.readByte();
			type = d.readByte();
			Res.err(GameScr.vcItem + ":<<GameScr.vcItem >>>>>>loadItemNew: " + type + "  isSave:" + isSave);
			switch (type)
			{
			case 0:
			{
				GameScr.gI().iOptionTemplates = new ItemOptionTemplate[d.readShort()];
				for (int j = 0; j < GameScr.gI().iOptionTemplates.Length; j++)
				{
					GameScr.gI().iOptionTemplates[j] = new ItemOptionTemplate();
					GameScr.gI().iOptionTemplates[j].id = j;
					GameScr.gI().iOptionTemplates[j].name = d.readUTF();
					GameScr.gI().iOptionTemplates[j].type = d.readByte();
				}
				try
				{
					short num3 = d.readShort();
					for (int k = 0; k < num3; k++)
					{
						short num4 = d.readShort();
						GameScr.gI().iOptionTemplates[num4].color = d.readUnsignedByte();
					}
				}
				catch (Exception)
				{
				}
				if (isSave)
				{
					d.reset();
					sbyte[] data3 = new sbyte[d.available()];
					d.readFully(ref data3);
					Rms.saveRMS("NRitem0", data3);
				}
				break;
			}
			case 1:
			{
				ItemTemplates.itemTemplates.clear();
				int num5 = d.readShort();
				for (int l = 0; l < num5; l++)
				{
					ItemTemplate it = new ItemTemplate((short)l, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBoolean());
					ItemTemplates.add(it);
				}
				if (isSave)
				{
					d.reset();
					sbyte[] data4 = new sbyte[d.available()];
					d.readFully(ref data4);
					Rms.saveRMS("NRitem1", data4);
					sbyte[] data5 = new sbyte[1] { GameScr.vcItem };
					Rms.saveRMS("NRitemVersion", data5);
				}
				LoginScr.isUpdateItem = false;
				GameScr.gI().readOk();
				break;
			}
			case 100:
				Char.Arr_Head_2Fr = readArrHead(d);
				if (isSave)
				{
					d.reset();
					sbyte[] data2 = new sbyte[d.available()];
					d.readFully(ref data2);
					Rms.saveRMS("NRitem100", data2);
				}
				break;
			case 101:
				try
				{
					int num = d.readShort();
					Char.Arr_Head_FlyMove = new short[num];
					for (int i = 0; i < num; i++)
					{
						short num2 = d.readShort();
						Char.Arr_Head_FlyMove[i] = num2;
					}
					if (isSave)
					{
						d.reset();
						sbyte[] data = new sbyte[d.available()];
						d.readFully(ref data);
						Rms.saveRMS("NRitem101", data);
					}
					break;
				}
				catch (Exception)
				{
					Char.Arr_Head_FlyMove = new short[0];
					break;
				}
			}
		}
		catch (Exception ex3)
		{
			ex3.ToString();
		}
	}

	private void readFrameBoss(Message msg, int mobTemplateId)
	{
		try
		{
			int num = msg.reader().readByte();
			int[][] array = new int[num][];
			for (int i = 0; i < num; i++)
			{
				int num2 = msg.reader().readByte();
				array[i] = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					array[i][j] = msg.reader().readByte();
				}
			}
			frameHT_NEWBOSS.put(mobTemplateId + string.Empty, array);
		}
		catch (Exception)
		{
		}
	}

	private int[][] readArrHead(myReader d)
	{
		int[][] array = new int[1][] { new int[2] { 542, 543 } };
		try
		{
			int num = d.readShort();
			array = new int[num][];
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = d.readByte();
				array[i] = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					array[i][j] = d.readShort();
				}
			}
		}
		catch (Exception)
		{
		}
		return array;
	}

	public void phuban_Info(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			if (b == 0)
			{
				readPhuBan_CHIENTRUONGNAMEK(msg, b);
			}
		}
		catch (Exception)
		{
		}
	}

	private void readPhuBan_CHIENTRUONGNAMEK(Message msg, int type_PB)
	{
		try
		{
			switch (msg.reader().readByte())
			{
			case 0:
			{
				short idmapPaint = msg.reader().readShort();
				string nameTeam = msg.reader().readUTF();
				string nameTeam2 = msg.reader().readUTF();
				int maxPoint = msg.reader().readInt();
				short timeSecond = msg.reader().readShort();
				int maxLife = msg.reader().readByte();
				GameScr.phuban_Info = new InfoPhuBan(type_PB, idmapPaint, nameTeam, nameTeam2, maxPoint, timeSecond);
				GameScr.phuban_Info.maxLife = maxLife;
				GameScr.phuban_Info.updateLife(type_PB, 0, 0);
				break;
			}
			case 1:
			{
				int pointTeam = msg.reader().readInt();
				int pointTeam2 = msg.reader().readInt();
				if (GameScr.phuban_Info != null)
				{
					GameScr.phuban_Info.updatePoint(type_PB, pointTeam, pointTeam2);
				}
				break;
			}
			case 2:
			{
				sbyte b = msg.reader().readByte();
				short type = 0;
				short num = -1;
				switch (b)
				{
				case 1:
					type = 1;
					num = 3;
					break;
				case 2:
					type = 2;
					break;
				}
				num = -1;
				GameScr.phuban_Info = null;
				GameScr.addEffectEnd(type, num, 0, GameCanvas.hw, GameCanvas.hh, 0, 0, -1, null);
				break;
			}
			case 5:
			{
				short timeSecond2 = msg.reader().readShort();
				if (GameScr.phuban_Info != null)
				{
					GameScr.phuban_Info.updateTime(type_PB, timeSecond2);
				}
				break;
			}
			case 4:
			{
				int lifeTeam = msg.reader().readByte();
				int lifeTeam2 = msg.reader().readByte();
				if (GameScr.phuban_Info != null)
				{
					GameScr.phuban_Info.updateLife(type_PB, lifeTeam, lifeTeam2);
				}
				break;
			}
			}
		}
		catch (Exception)
		{
		}
	}

	public void read_cmdExtra(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			mSystem.println(">>---read_cmdExtra-sub:" + b);
			switch (b)
			{
			case 0:
			{
				short idHat = msg.reader().readShort();
				Char.myCharz().idHat = idHat;
				SoundMn.gI().getStrOption();
				break;
			}
			case 2:
			{
				int num3 = msg.reader().readInt();
				sbyte b5 = msg.reader().readByte();
				short num4 = msg.reader().readShort();
				string v = num4 + "," + b5;
				MainImage imagePath = ImgByName.getImagePath("banner_" + num4, ImgByName.hashImagePath);
				GameCanvas.danhHieu.put(num3 + string.Empty, v);
				break;
			}
			case 3:
			{
				short num2 = msg.reader().readShort();
				SmallImage.createImage(num2);
				BackgroudEffect.id_water1 = num2;
				break;
			}
			case 4:
			{
				string o = msg.reader().readUTF();
				GameCanvas.messageServer.addElement(o);
				break;
			}
			case 5:
			{
				string text = "------------------|ChienTruong|Log: ";
				text = "\n|ChienTruong|Log: ";
				sbyte b2 = msg.reader().readByte();
				switch (b2)
				{
				case 0:
				{
					GameScr.nCT_team = msg.reader().readUTF();
					GameScr.nCT_TeamA = (GameScr.nCT_TeamB = msg.reader().readByte());
					GameScr.nCT_nBoyBaller = GameScr.nCT_TeamA * 2;
					GameScr.isPaint_CT = false;
					string text4 = text;
					text = text4 + "\tsub    0|  nCT_team= " + GameScr.nCT_team + "|nCT_TeamA =" + GameScr.nCT_TeamA + "  isPaint_CT=false \n";
					break;
				}
				case 1:
				{
					int num = msg.reader().readInt();
					sbyte b4 = (GameScr.nCT_floor = msg.reader().readByte());
					GameScr.nCT_timeBallte = num * 1000 + mSystem.currentTimeMillis();
					GameScr.isPaint_CT = true;
					string text3 = text;
					text = text3 + "\tsub    1 floor= " + b4 + "|timeBallte= " + num + "isPaint_CT=true \n";
					break;
				}
				case 2:
				{
					GameScr.nCT_TeamA = msg.reader().readByte();
					GameScr.nCT_TeamB = msg.reader().readByte();
					GameScr.res_CT.removeAllElements();
					sbyte b3 = msg.reader().readByte();
					for (int i = 0; i < b3; i++)
					{
						string empty = string.Empty;
						empty = empty + msg.reader().readByte() + "|";
						empty = empty + msg.reader().readUTF() + "|";
						empty = empty + msg.reader().readShort() + "|";
						empty += msg.reader().readInt();
						GameScr.res_CT.addElement(empty);
					}
					string text2 = text;
					text = text2 + "\tsub   2|  A= " + GameScr.nCT_TeamA + "|B =" + GameScr.nCT_TeamB + "  isPaint_CT=true \n";
					break;
				}
				case 3:
					Service.gI().sendCT_ready(b, b2);
					GameScr.nCT_floor = 0;
					GameScr.nCT_timeBallte = 0L;
					GameScr.isPaint_CT = false;
					text += "\tsub    3|  isPaint_CT=false \n";
					break;
				case 4:
					GameScr.nUSER_CT = msg.reader().readByte();
					GameScr.nUSER_MAX_CT = msg.reader().readByte();
					break;
				}
				text += "END LOG CT.";
				Res.err(text);
				break;
			}
			default:
				readExtra(b, msg);
				break;
			}
		}
		catch (Exception)
		{
		}
	}

	public void read_UpdateSkill(Message msg)
	{
		try
		{
			short num = msg.reader().readShort();
			sbyte b = -1;
			try
			{
				b = msg.reader().readSByte();
			}
			catch (Exception)
			{
			}
			switch (b)
			{
			case 0:
			{
				short curExp = msg.reader().readShort();
				for (int m = 0; m < Char.myCharz().vSkill.size(); m++)
				{
					Skill skill4 = (Skill)Char.myCharz().vSkill.elementAt(m);
					if (skill4.skillId == num)
					{
						skill4.curExp = curExp;
						break;
					}
				}
				break;
			}
			case 1:
			{
				sbyte b2 = msg.reader().readByte();
				for (int n = 0; n < Char.myCharz().vSkill.size(); n++)
				{
					Skill skill5 = (Skill)Char.myCharz().vSkill.elementAt(n);
					if (skill5.skillId == num)
					{
						for (int num2 = 0; num2 < 20; num2++)
						{
							string nameImg = "Skills_" + skill5.template.id + "_" + b2 + "_" + num2;
							MainImage imagePath = ImgByName.getImagePath(nameImg, ImgByName.hashImagePath);
						}
						break;
					}
				}
				break;
			}
			case -1:
			{
				Skill skill = Skills.get(num);
				for (int i = 0; i < Char.myCharz().vSkill.size(); i++)
				{
					Skill skill2 = (Skill)Char.myCharz().vSkill.elementAt(i);
					if (skill2.template.id == skill.template.id)
					{
						Char.myCharz().vSkill.setElementAt(skill, i);
						break;
					}
				}
				for (int j = 0; j < Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill3 = (Skill)Char.myCharz().vSkillFight.elementAt(j);
					if (skill3.template.id == skill.template.id)
					{
						Char.myCharz().vSkillFight.setElementAt(skill, j);
						break;
					}
				}
				for (int k = 0; k < GameScr.onScreenSkill.Length; k++)
				{
					if (GameScr.onScreenSkill[k] != null && GameScr.onScreenSkill[k].template.id == skill.template.id)
					{
						GameScr.onScreenSkill[k] = skill;
						break;
					}
				}
				for (int l = 0; l < GameScr.keySkill.Length; l++)
				{
					if (GameScr.keySkill[l] != null && GameScr.keySkill[l].template.id == skill.template.id)
					{
						GameScr.keySkill[l] = skill;
						break;
					}
				}
				if (Char.myCharz().myskill.template.id == skill.template.id)
				{
					Char.myCharz().myskill = skill;
				}
				GameScr.info1.addInfo(mResources.hasJustUpgrade1 + skill.template.name + mResources.hasJustUpgrade2 + skill.point, 0);
				break;
			}
			}
		}
		catch (Exception)
		{
		}
	}

	public void readExtra(sbyte sub, Message msg)
	{
		try
		{
			if (sub != sbyte.MaxValue)
			{
				return;
			}
			GameCanvas.endDlg();
			try
			{
				string text = (ServerListScreen.linkDefault = msg.reader().readUTF());
				mSystem.AddIpTest();
				ServerListScreen.getServerList(ServerListScreen.linkDefault);
				Res.outz(">>>>read.isEXTRA_LINK " + text);
				sbyte b = msg.reader().readByte();
				if (b > 0)
				{
					ServerListScreen.typeClass = new sbyte[b];
					ServerListScreen.listChar = new Char[b];
					for (int i = 0; i < b; i++)
					{
						ServerListScreen.typeClass[i] = msg.reader().readByte();
						Res.outz(ServerListScreen.nameServer[i] + ">>>>read.isEXTRA_LINK  typeClass: " + ServerListScreen.typeClass[i]);
						if (ServerListScreen.typeClass[i] > -1)
						{
							ServerListScreen.isHaveChar = true;
							ServerListScreen.listChar[i] = new Char();
							ServerListScreen.listChar[i].cgender = ServerListScreen.typeClass[i];
							ServerListScreen.listChar[i].head = msg.reader().readShort();
							ServerListScreen.listChar[i].body = msg.reader().readShort();
							ServerListScreen.listChar[i].leg = msg.reader().readShort();
							ServerListScreen.listChar[i].bag = msg.reader().readShort();
							ServerListScreen.listChar[i].cName = msg.reader().readUTF();
						}
					}
				}
			}
			catch (Exception)
			{
			}
			isEXTRA_LINK = true;
			ServerListScreen.saveRMS_ExtraLink();
			ServerListScreen.isWait = false;
			Char.isLoadingMap = false;
			LoginScr.isContinueToLogin = false;
			ServerListScreen.waitToLogin = false;
			bool flag = false;
			bool flag2 = false;
			try
			{
				if (!Rms.loadRMSString(Rms.RMS_acc).Equals(string.Empty))
				{
					flag = true;
				}
				if (!Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect).Equals(string.Empty))
				{
					flag2 = true;
				}
			}
			catch (Exception)
			{
			}
			if (!ServerListScreen.isHaveChar && !flag && !flag2)
			{
				GameCanvas.serverScreen.Login_New();
				return;
			}
			if (Rms.loadRMSInt(ServerListScreen.RMS_svselect) == -1)
			{
				ServerScr.isShowSv_HaveChar = false;
				GameCanvas.serverScr.switchToMe();
				return;
			}
			ServerListScreen.SetIpSelect(Rms.loadRMSInt(ServerListScreen.RMS_svselect), issave: false);
			if (ServerListScreen.listChar != null && ServerListScreen.listChar[ServerListScreen.ipSelect] != null)
			{
				GameCanvas._SelectCharScr.SetInfoChar(ServerListScreen.listChar[ServerListScreen.ipSelect]);
			}
			else
			{
				GameCanvas.serverScreen.Login_New();
			}
		}
		catch (Exception)
		{
			Res.outz(">>>>read.isEXTRA_LINK  errr:");
			GameCanvas.serverScr.switchToMe();
		}
	}

	public ItemOption readItemOption(Message msg)
	{
		ItemOption result = null;
		try
		{
			int num = msg.reader().readShort();
			int param = msg.reader().readInt();
			if (num != -1)
			{
				result = new ItemOption(num, param);
			}
		}
		catch (Exception)
		{
			Res.err(">>>>read.ItemOption  errr:");
		}
		return result;
	}

	public void read_cmdExtraBig(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			mSystem.println(">>---read_cmdExtraBig-sub:" + b);
			if (b == 0)
			{
				loadItemNew(msg.reader(), 1, isSave: true);
			}
		}
		catch (Exception)
		{
		}
	}
}
