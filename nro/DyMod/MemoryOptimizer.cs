using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DyMod;

public static class MemoryOptimizer
{
	[DllImport("psapi.dll")]
	private static extern int EmptyWorkingSet(IntPtr hwProc);

	[DllImport("kernel32.dll")]
	private static extern bool SetProcessWorkingSetSize(IntPtr proc, int min, int max);

	public static bool IsEnabled = true;

	private static long _lastCleanTime = 0L;

	private const long CLEAN_INTERVAL_MS = 30000L; // Clean every 30 seconds

	public static void CleanMemory()
	{
		if (!IsEnabled)
		{
			return;
		}
		try
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
			GC.WaitForPendingFinalizers();

			IntPtr handle = Process.GetCurrentProcess().Handle;
			EmptyWorkingSet(handle);
			SetProcessWorkingSetSize(handle, -1, -1);
		}
		catch
		{
		}
	}

	public static void Update(long now)
	{
		if (!IsEnabled)
		{
			return;
		}
		if (_lastCleanTime == 0L)
		{
			_lastCleanTime = now;
			CleanMemory();
			return;
		}
		if (now - _lastCleanTime >= CLEAN_INTERVAL_MS)
		{
			_lastCleanTime = now;
			CleanMemory();
		}
	}

	public static long GetCurrentProcessMemoryMB()
	{
		try
		{
			return Process.GetCurrentProcess().WorkingSet64 / (1024L * 1024L);
		}
		catch
		{
			return 0L;
		}
	}
}
