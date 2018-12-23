using System;

namespace Product {
    public interface IEventService {
        event Action MouseInput;
        event Action KeyboardInput;

        void RaiseMouseInput();

        void RaiseKeyboardInput();
    }
}