using System;

namespace Product
{
    public class EventService : IEventService
    {
        public event Action MouseInput;
        public event Action KeyboardInput;

        public void RaiseMouseInput()
        {
            MouseInput?.Invoke();
        }

        public void RaiseKeyboardInput()
        {
            KeyboardInput?.Invoke();
        }
    }
}