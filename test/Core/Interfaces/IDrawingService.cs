using System.Threading.Tasks;
using Avalonia;
using SkiaSharp;

namespace Autodraw.Core.Interfaces
{
    public interface IDrawingService
    {
        bool IsDrawing { get; }
        Task StartDrawingAsync(SKBitmap bitmap, Point startPosition);
        void Stop();
        void TogglePause();
    }
}
