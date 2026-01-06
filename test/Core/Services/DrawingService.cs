using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autodraw.Core.Interfaces;
using Autodraw.Core.Models;
using Avalonia;
using SkiaSharp;

namespace Autodraw.Core.Services
{
    public class DrawingService : IDrawingService
    {
        private readonly IInputService _inputService;
        private CancellationTokenSource? _cts;
        private bool _isPaused;

        public int IntervalDelay { get; set; } = 1;
        public double SmoothingTolerance { get; set; } = 0.5; 
        public int BezierResolution { get; set; } = 5; 
        public bool IsPixelPerfect { get; set; } = false;

        public event EventHandler<double>? ProgressChanged;

        public DrawingService(IInputService inputService)
        {
            _inputService = inputService;
        }

        public bool IsDrawing => _cts != null && !_cts.IsCancellationRequested;

        public async Task StartDrawingAsync(SKBitmap bitmap, Point startPosition)
        {
            if (IsDrawing) return;
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try { await ProcessAndDrawAsync(bitmap, startPosition, token); }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Console.WriteLine($"Drawing error: {ex.Message}"); }
            finally { _inputService.ClickUp(); _cts = null; }
        }

        public void Stop() => _cts?.Cancel();
        public void TogglePause() => _isPaused = !_isPaused;

        private async Task ProcessAndDrawAsync(SKBitmap bitmap, Point startPosition, CancellationToken token)
        {
            // 1. Analyze Image on background thread
            var paths = await Task.Run(() => GenerateRawPaths(bitmap), token);

            int totalPaths = paths.Count;
            int currentPath = 0;

            foreach (var path in paths)
            {
                token.ThrowIfCancellationRequested();
                await HandlePauseAsync(token);

                currentPath++;
                if (totalPaths > 0 && currentPath % 5 == 0) 
                    ProgressChanged?.Invoke(this, (double)currentPath / totalPaths);

                // 2. Optimize Path on background thread
                List<Point> finalPath = await Task.Run(() => {
                    if (IsPixelPerfect) return path;
                    var simplified = PathOptimization.SimplifyPath(path, SmoothingTolerance);
                    return simplified.Count >= 3 ? PathOptimization.GenerateBezierPath(simplified, BezierResolution) : simplified;
                }, token);

                if (finalPath.Count == 0) continue;

                // 3. Move and Draw
                _inputService.MoveTo((int)(startPosition.X + finalPath[0].X), (int)(startPosition.Y + finalPath[0].Y));
                await Task.Delay(25, token); 

                _inputService.ClickDown();
                foreach (var point in finalPath)
                {
                    if (_isPaused) await HandlePauseAsync(token);
                    token.ThrowIfCancellationRequested();

                    _inputService.MoveTo((int)(startPosition.X + point.X), (int)(startPosition.Y + point.Y));
                    if (IntervalDelay > 0) await Task.Delay(IntervalDelay, token); 
                }
                _inputService.ClickUp();
                await Task.Delay(10, token); 
            }
        }

        private async Task HandlePauseAsync(CancellationToken token)
        {
            if (!_isPaused) return;
            _inputService.ClickUp();
            while (_isPaused)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(100, token);
            }
        }

        private List<List<Point>> GenerateRawPaths(SKBitmap bitmap)
        {
            var paths = new List<List<Point>>();
            int width = bitmap.Width;
            int height = bitmap.Height;
            bool[,] visited = new bool[width, height];
            int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] dy = { -1, -1, 0, 1, 1, 1, 0, -1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (visited[x, y]) continue;
                    SKColor color = bitmap.GetPixel(x, y);
                    if (color.Alpha > 128 && (color.Red + color.Green + color.Blue) / 3 < 128)
                    {
                        var currentPath = new List<Point>();
                        Stack<Point> stack = new Stack<Point>();
                        stack.Push(new Point(x, y));
                        visited[x, y] = true;

                        while (stack.Count > 0)
                        {
                            Point p = stack.Pop();
                            currentPath.Add(p);
                            for (int i = 0; i < 8; i++)
                            {
                                int nx = (int)p.X + dx[i];
                                int ny = (int)p.Y + dy[i];
                                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny])
                                {
                                    SKColor nc = bitmap.GetPixel(nx, ny);
                                    if (nc.Alpha > 128 && (nc.Red + nc.Green + nc.Blue) / 3 < 128)
                                    {
                                        visited[nx, ny] = true;
                                        stack.Push(new Point(nx, ny));
                                    }
                                }
                            }
                        }
                        if (currentPath.Count > 0) paths.Add(currentPath);
                    }
                }
            }
            return paths;
        }
    }
}