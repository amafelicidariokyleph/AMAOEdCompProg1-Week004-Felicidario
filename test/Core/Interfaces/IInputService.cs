using System.Threading.Tasks;
using Avalonia;

namespace Autodraw.Core.Interfaces
{
    public interface IInputService
    {
        void MoveTo(int x, int y);
        void ClickDown();
        void ClickUp();
        Task Click(int delayMs = 50);
        Point GetCursorPosition();
    }
}