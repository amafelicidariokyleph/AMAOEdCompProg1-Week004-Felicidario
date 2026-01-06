using System;
using System.Runtime.InteropServices;  // For P/Invoke (cursor position)
using System.Threading.Tasks;
using Avalonia;  // For Avalonia.Point
using SharpHook;  // For EventSimulator
using Autodraw.Core.Interfaces;

namespace Autodraw.Core.Services
{
    public class InputService : IInputService
    {
        private readonly EventSimulator _simulator;

        // P/Invoke for getting cursor position (Windows-specific)
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        public InputService()
        {
            _simulator = new EventSimulator();
        }

        // Use SharpHook for cross-platform simulation
        public void MoveTo(int x, int y)
        {
            _simulator.SimulateMouseMovement((short)x, (short)y);
        }

        public void ClickDown()
        {
            _simulator.SimulateMousePress(SharpHook.Native.MouseButton.Button1);
        }

        public void ClickUp()
        {
            _simulator.SimulateMouseRelease(SharpHook.Native.MouseButton.Button1);
        }

        public async Task Click(int delayMs = 50)
        {
            ClickDown();
            await Task.Delay(delayMs);
            ClickUp();
        }

        // Use P/Invoke to get cursor position (SharpHook doesn't provide this)
        public Point GetCursorPosition()
        {
            POINT p = new POINT();
            if (GetCursorPos(out p))
            {
                return new Point(p.X, p.Y);  // Convert to Avalonia.Point
            }
            else
            {
                throw new InvalidOperationException("Failed to get cursor position.");
            }
        }
    }
}