using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class PreciseTimer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        public IntPtr hWnd;
        public Int32 Msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public System.Drawing.Point P;
    }

    [System.Security.SuppressUnmanagedCodeSecurity]
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern bool PeekMessage(
      out Message Msg,
      IntPtr hWnd,
      uint messageFilterMin,
      uint messageFilterMax,
      uint flags);

    [System.Security.SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32")]
    private static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);
    [System.Security.SuppressUnmanagedCodeSecurity]
    [DllImport("kernel32")]
    private static extern bool QueryPerformanceCounter(ref long PerformanceCounter);
    long _tickPerSecond = 0;
    long _previousElapsedTime = 0;
    public PreciseTimer()
    {
        QueryPerformanceFrequency(ref _tickPerSecond);
        GetElapsedTime();
    }
    public double GetElapsedTime()
    {
        long Time = 0;
        QueryPerformanceCounter(ref Time);
        double ElapsedTime = (double)(Time - _previousElapsedTime) / (double)_tickPerSecond;
        _previousElapsedTime = Time;
        return ElapsedTime;
    }

    public class TimeController
    {
        PreciseTimer _preciseTimer = new PreciseTimer();
        public delegate void LoopCallBack(double ElapsedTime);
        LoopCallBack _loopCallBack;
        public TimeController(LoopCallBack CallBack)
        {
            _loopCallBack = CallBack;
            Application.Idle += new System.EventHandler(OnApplicationEnterIdle);
        }
        private void OnApplicationEnterIdle(object sender, EventArgs e)
        {
            while (IsAppStillIdle())
                _loopCallBack(_preciseTimer.GetElapsedTime());
        }
        private bool IsAppStillIdle()
        {
            Message Msg;
            return !PeekMessage(out Msg, IntPtr.Zero, 0, 0, 0);
        }
    }
}

