using System;
using Microsoft.Win32;
using WindowsInput;

namespace Langwitch
{
    public class SimulatorLanguageSetterService : ILanguageSetterService
    {
        private void SendCtrlShift()
        {
            InputSimulator.SimulateKeyDown(VirtualKeyCode.LCONTROL);
            InputSimulator.SimulateKeyPress(VirtualKeyCode.LSHIFT);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.LCONTROL);
        }

        private void SendAltShift()
        {
            InputSimulator.SimulateKeyDown(VirtualKeyCode.LMENU);
            InputSimulator.SimulateKeyPress(VirtualKeyCode.LSHIFT);
            InputSimulator.SimulateKeyUp(VirtualKeyCode.LMENU);
        }

        private void SendGraveAccent()
        {
            InputSimulator.SimulateKeyPress(VirtualKeyCode.OEM_3);
        }

        private int? CurrentLanguageSwitch { get; set; }
        private int? CurrentLayoutSwitch { get; set; }

        public bool SetCurrentLayout(IntPtr handle)
        {
            if (CurrentLanguageSwitch == null && CurrentLayoutSwitch == null)
            {
                // If those values are not set, we suppose we need to read this cache
                // for the first time (guessing it's pointless to use this switch mode
                // if no standard hotkeys set at all).
                CurrentLanguageSwitch = Utils.ParseInt(Registry.GetValue("HKEY_CURRENT_USER\\Keyboard Layout\\Toggle", "Language Hotkey", null));
                CurrentLayoutSwitch = Utils.ParseInt(Registry.GetValue("HKEY_CURRENT_USER\\Keyboard Layout\\Toggle", "Layout Hotkey", null));
            }
            // TODO: We must calculate the amount of simulated key-presses 
            // in order to get to the layout we need to turn on.

            if (CurrentLanguageSwitch == 2)
                SendCtrlShift();
            else if (CurrentLanguageSwitch == 1)
                SendAltShift();
            else if (CurrentLanguageSwitch == 4)
                SendGraveAccent();

            return false;
        }
    }
}
