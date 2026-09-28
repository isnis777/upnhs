namespace DyMod;

public static class DyManager
{
	private static bool _initialized;

	public static void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			AutoLogin.Init();
			ActionLogger.Init();
			SocketClient.Connect();
		}
	}

	public static void Update()
	{
		if (!_initialized)
		{
			Init();
		}
		long num = mSystem.currentTimeMillis();
		MemoryOptimizer.Update(num);
		if (!UnityEngine.Application.isFocused)
		{
			int unfocusedFps = System.Math.Min(15, AutoLogin.TargetFps);
			if (UnityEngine.Application.targetFrameRate != unfocusedFps)
			{
				UnityEngine.Application.targetFrameRate = unfocusedFps;
			}
		}
		else
		{
			if (UnityEngine.Application.targetFrameRate != AutoLogin.TargetFps)
			{
				UnityEngine.Application.targetFrameRate = AutoLogin.TargetFps;
			}
		}
		if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.B))
		{
			GameScr.isBlackScreen = !GameScr.isBlackScreen;
			if (GameScr.info1 != null)
			{
				GameScr.info1.addInfo(GameScr.isBlackScreen ? "Bật màn hình đen (tiết kiệm CPU/RAM)" : "Tắt màn hình đen", 0);
			}
		}
		SocketClient.Update();
		AutoLogin.UpdateWindowTitle();
		if (!AutoLogin.IsDyTest)
		{
			AutoLogin.Update();
			AutoTrain.Update();
		}
	}
}
