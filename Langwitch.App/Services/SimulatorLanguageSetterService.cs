using System;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using WindowsInput;

namespace Langwitch
{
    public class SimulatorLanguageSetterService : ILanguageSetterService
    {
        private const int InterruptionDelay = 10;

        private void SendCtrlShift(int amount = 1)
        {
            InputSimulator.SimulateKeyDown(VirtualKeyCode.LCONTROL);
            for (int i = 0; i < amount; i++) 
            {
                InputSimulator.SimulateKeyPress(VirtualKeyCode.LSHIFT);
            }
            InputSimulator.SimulateKeyUp(VirtualKeyCode.LCONTROL);
        }

        private void SendAltShift(int amount = 1)
        {
            InputSimulator.SimulateKeyDown(VirtualKeyCode.LMENU);
            for (int i = 0; i < amount; i++)
            {
                InputSimulator.SimulateKeyPress(VirtualKeyCode.LSHIFT);
            }
            InputSimulator.SimulateKeyUp(VirtualKeyCode.LMENU);
        }

        private void SendGraveAccent(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                InputSimulator.SimulateKeyPress(VirtualKeyCode.OEM_3);
            }
        }

        private int? CurrentLanguageSwitch { get; set; }
        private int? CurrentLayoutSwitch { get; set; }

        private void SendSequence(int sequenceCode, int amount)
        {
            switch (sequenceCode)
            {
                case WindowsSequenceCode.CtrlShift:
                    SendCtrlShift(amount);
                    break;
                case WindowsSequenceCode.AltShift:
                    SendAltShift(amount);
                    break;
                case WindowsSequenceCode.GraveAccent:
                    SendGraveAccent(amount);
                    break;
            }
        }

        private void SwitchLayout()
        {
        }

        public bool SetCurrentLayout(IntPtr targetHandle)
        {
            var result = false;
            try
            {
                if (CurrentLanguageSwitch == null && CurrentLayoutSwitch == null)
                {
                    // If those values are not set, we suppose we need to read this cache
                    // for the first time (guessing it's pointless to use this switch mode
                    // if no standard hotkeys set at all).
                    CurrentLanguageSwitch = Utils.ParseInt(Registry.GetValue("HKEY_CURRENT_USER\\Keyboard Layout\\Toggle", "Language Hotkey", null));
                    // Fallback to perhaps "old"-Windows-version key for the language sequence
                    if (CurrentLanguageSwitch == null)
                        CurrentLanguageSwitch = Utils.ParseInt(Registry.GetValue("HKEY_CURRENT_USER\\Keyboard Layout\\Toggle", "Hotkey", null));
                    CurrentLayoutSwitch = Utils.ParseInt(Registry.GetValue("HKEY_CURRENT_USER\\Keyboard Layout\\Toggle", "Layout Hotkey", null));
                }

                var inputLayouts = InputLayoutHelper.InputLayouts;
                var currentLayout = InputLayoutHelper.GetCurrentLayout();
                var targetLayout = inputLayouts.FirstOrDefault(x => x.Handle == targetHandle);

                var inputLanguageNames = inputLayouts
                    .Select(x => x.LanguageName)
                    .Distinct().ToList();
                var indexOfCurrentLanguage = inputLanguageNames.IndexOf(currentLayout.LanguageName);
                var indexOfTargetLanguage = inputLanguageNames.IndexOf(targetLayout.LanguageName);
                var amountOfLanguageSwitches = indexOfTargetLanguage - indexOfCurrentLanguage;
                if (amountOfLanguageSwitches < 0)
                    amountOfLanguageSwitches += inputLanguageNames.Count;

                if (amountOfLanguageSwitches > 0)
                {
                    if (CurrentLanguageSwitch == null)
                        throw new Exception(
                            "cannot enumerate languages 'cause the system key sequence was not set");

                    SendSequence(CurrentLanguageSwitch.Value, amountOfLanguageSwitches);
                }

                // Simulate "interruption" so that the system can process the key sequence.
                Thread.Sleep(InterruptionDelay);

                // Re-read the current layout, to know the layout the default switcher selected
                // for the language.
                currentLayout = InputLayoutHelper.GetCurrentLayout();

                var inputLayoutNamesWithinLanguage = inputLayouts
                    .Where(x => x.LanguageName == targetLayout.LanguageName)
                    .Select(x => x.Name)
                    .ToList();
                var indexOfCurrentLayout = inputLayoutNamesWithinLanguage.IndexOf(currentLayout.Name);
                var indexOfTargetLayout = inputLayoutNamesWithinLanguage.IndexOf(targetLayout.Name);
                var amountOfLayoutSwitches = indexOfTargetLayout - indexOfCurrentLayout;
                if (amountOfLayoutSwitches < 0)
                    amountOfLayoutSwitches += inputLayoutNamesWithinLanguage.Count;

                if (amountOfLayoutSwitches > 0)
                {
                    if (CurrentLayoutSwitch == null)
                        throw new Exception(
                            "cannot enumerate layouts, because the system key sequence was not set");
                    SendSequence(CurrentLayoutSwitch.Value, amountOfLayoutSwitches);
                }

                // Simulating "interruption" once again, so that synchronous code can read
                // the current (new) layout after this method finishes.
                Thread.Sleep(InterruptionDelay);
                result = true;
            }
            catch { /* ummm, logging must be here */ }
            return result;
        }
    }
}
