﻿namespace Seedwork.Engine
{
    public interface IInputHandler
    {
        void ProcessEvents();
        void Reset();
        void SetControls(IInputHandler inputHandler);
        bool GetControlState(int control);
    }
}
