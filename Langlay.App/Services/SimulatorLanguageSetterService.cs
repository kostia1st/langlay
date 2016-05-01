using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Win32;
using Product.Common;
using WindowsInput;
using WindowsInput.Native;

namespace Product
{
    public class SimulatorLanguageSetterService : ILanguageSetterService
    {
        private const int InterruptionDelay = 10;

        private WindowsSequenceCode? CurrentLanguageSwitchSequence { get; set; }
        private WindowsSequenceCode? CurrentLayoutSwitchSequence { get; set; }

        public IHotkeyService HotkeyService { get; set; }
        public ISystemSettingService SystemSettingService { get; set; }
        private KeyboardSimulator KeyboardSimulator { get; set; }

        public SimulatorLanguageSetterService(
            IHotkeyService hotkeyService, ISystemSettingService systemSettingService)
        {
            HotkeyService = hotkeyService;
            SystemSettingService = systemSettingService;
            KeyboardSimulator = new KeyboardSimulator(new InputSimulator());
        }

        private void SendCtrlShift(int amount = 1)
        {
            KeyboardSimulator.KeyDown((VirtualKeyCode) KeyCode.LControlKey);
            for (int i = 0; i < amount; i++)
            {
                KeyboardSimulator.KeyPress((VirtualKeyCode) KeyCode.LShiftKey);
            }
            KeyboardSimulator.KeyUp((VirtualKeyCode) KeyCode.LControlKey);
        }

        private void SendAltShift(int amount = 1)
        {
            // It's important to HOLD the Alt key first, not vice versa
            KeyboardSimulator.KeyDown((VirtualKeyCode) KeyCode.LMenu);
            for (int i = 0; i < amount; i++)
            {
                KeyboardSimulator.KeyPress((VirtualKeyCode) KeyCode.LShiftKey);
            }
            KeyboardSimulator.KeyUp((VirtualKeyCode) KeyCode.LMenu);
        }

        private void SendGraveAccent(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                KeyboardSimulator.KeyPress((VirtualKeyCode) KeyCode.Oemtilde);
            }
        }

        private void SendSequence(WindowsSequenceCode sequenceCode, int amount)
        {
            Trace.WriteLine("-- START Simulating switch sequence");
            Trace.Indent();
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
            Trace.Unindent();
            Trace.WriteLine("-- END Simulating switch sequence");
        }

        public bool SetCurrentLayout(IntPtr targetHandle)
        {
            var result = false;
            HotkeyService.SetEnabled(false);
            try
            {
                // If those values are not set, we suppose we need to read this cache
                // for the first time (guessing it's pointless to use this switch mode
                // if no standard hotkeys set at all).
                if (CurrentLanguageSwitchSequence == null && CurrentLayoutSwitchSequence == null)
                {
                    CurrentLanguageSwitchSequence = SystemSettingService.GetLanguageSwitchSequence();
                    CurrentLayoutSwitchSequence = SystemSettingService.GetLayoutSwitchSequence();
                }

                var inputLayouts = InputLayoutHelper.InputLayouts;
                var currentLayout = InputLayoutHelper.GetCurrentLayout();
                var targetLayout = inputLayouts.FirstOrDefault(x => x.Handle == targetHandle);

                var inputLanguageNames = inputLayouts
                    .Select(x => x.LanguageName)
                    .Distinct().ToList();
                var indexOfCurrentLanguage = inputLanguageNames.IndexOf(currentLayout.LanguageName);
                if (indexOfCurrentLanguage >= 0)
                {
                    var indexOfTargetLanguage = inputLanguageNames.IndexOf(targetLayout.LanguageName);
                    var amountOfLanguageSwitches = indexOfTargetLanguage - indexOfCurrentLanguage;
                    if (amountOfLanguageSwitches < 0)
                        amountOfLanguageSwitches += inputLanguageNames.Count;

                    if (amountOfLanguageSwitches > 0)
                    {
                        if (CurrentLanguageSwitchSequence == null)
                            throw new Exception(
                                "cannot enumerate languages 'cause the system key sequence was not set");

                        SendSequence(CurrentLanguageSwitchSequence.Value, amountOfLanguageSwitches);
                        result = true;
                    }

                    if (result)
                    {
                        // Simulate "interruption" so that the system can process the key sequence.
                        Thread.Sleep(InterruptionDelay);
                    }
                }

                // Re-read the current layout, to know the layout the default switcher selected
                // for the language.
                currentLayout = InputLayoutHelper.GetCurrentLayout();

                var inputLayoutNamesWithinLanguage = inputLayouts
                    .Where(x => x.LanguageName == targetLayout.LanguageName)
                    .Select(x => x.Name)
                    .ToList();
                var indexOfCurrentLayout = inputLayoutNamesWithinLanguage.IndexOf(currentLayout.Name);
                if (indexOfCurrentLayout >= 0)
                {
                    var indexOfTargetLayout = inputLayoutNamesWithinLanguage.IndexOf(targetLayout.Name);
                    var amountOfLayoutSwitches = indexOfTargetLayout - indexOfCurrentLayout;
                    if (amountOfLayoutSwitches < 0)
                        amountOfLayoutSwitches += inputLayoutNamesWithinLanguage.Count;

                    if (amountOfLayoutSwitches > 0)
                    {
                        if (CurrentLayoutSwitchSequence == null)
                            throw new Exception(
                                "cannot enumerate layouts, because the system key sequence was not set");
                        SendSequence(CurrentLayoutSwitchSequence.Value, amountOfLayoutSwitches);
                        result = true;
                    }
                }

                if (result)
                {
                    // Simulating "interruption" once again, so that synchronous code can read
                    // the current (new) layout from OS after this method finishes.
                    Thread.Sleep(InterruptionDelay);
                }
            }
            finally
            {
                HotkeyService.SetEnabled(true);
            }
            return result;
        }
    }
}
