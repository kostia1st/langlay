using System;
using System.ComponentModel.DataAnnotations;

namespace Product.Common {
    [Flags]
    public enum KeyCode : byte {
        // Summary: No key pressed.
        None = 0,

        [Display(Name = "Backspace")]
        BackSpace = 8,

        Tab = 9,

        LineFeed = 10,

        Clear = 12,

        Enter = 13,
        Pause = 19,

        [Display(Name = "Caps Lock")]
        CapsLock = 20,

        Escape = 27,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,

        [Display(Name = "Arrow Left")]
        ArrowLeft = 37,

        [Display(Name = "Arrow Up")]
        ArrowUp = 38,

        [Display(Name = "Arrow Right")]
        ArrowRight = 39,

        [Display(Name = "Arrow Down")]
        ArrowDown = 40,

        [Display(Name = "Print Screen")]
        PrintScreen = 44,

        Insert = 45,

        Delete = 46,

        [Display(Name = "Digit 0")]
        D0 = 48,

        [Display(Name = "Digit 1")]
        D1 = 49,

        [Display(Name = "Digit 2")]
        D2 = 50,

        [Display(Name = "Digit 3")]
        D3 = 51,

        [Display(Name = "Digit 4")]
        D4 = 52,

        [Display(Name = "Digit 5")]
        D5 = 53,

        [Display(Name = "Digit 6")]
        D6 = 54,

        [Display(Name = "Digit 7")]
        D7 = 55,

        [Display(Name = "Digit 8")]
        D8 = 56,

        [Display(Name = "Digit 9")]
        D9 = 57,

        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,

        Sleep = 95,

        [Display(Name = "Numpad 0")]
        NumPad0 = 96,

        [Display(Name = "Numpad 1")]
        NumPad1 = 97,

        [Display(Name = "Numpad 2")]
        NumPad2 = 98,

        [Display(Name = "Numpad 3")]
        NumPad3 = 99,

        [Display(Name = "Numpad 4")]
        NumPad4 = 100,

        [Display(Name = "Numpad 5")]
        NumPad5 = 101,

        [Display(Name = "Numpad 6")]
        NumPad6 = 102,

        [Display(Name = "Numpad 7")]
        NumPad7 = 103,

        [Display(Name = "Numpad 8")]
        NumPad8 = 104,

        [Display(Name = "Numpad 9")]
        NumPad9 = 105,

        [Display(Name = "Numpad *")]
        Multiply = 106,

        [Display(Name = "Numpad +")]
        Add = 107,

        [Display(Name = "Numpad -")]
        Subtract = 109,

        [Display(Name = "Numpad .")]
        Decimal = 110,

        [Display(Name = "Numpad /")]
        Divide = 111,

        [Display(Name = "|")]
        Separator = 108,

        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,

        [Display(Name = "Num Lock")]
        NumLock = 144,

        [Display(Name = "Scroll Lock")]
        ScrollLock = 145,

        [Display(Name = "Left Windows")]
        LWin = 91,

        [Display(Name = "Right Windows")]
        RWin = 92,

        [Display(Name = "Left Shift")]
        LShiftKey = 160,

        [Display(Name = "Right Shift")]
        RShiftKey = 161,

        [Display(Name = "Left Control")]
        LControlKey = 162,

        [Display(Name = "Right Control")]
        RControlKey = 163,

        [Display(Name = "Left Alt")]
        LAltKey = 164,

        [Display(Name = "Right Alt")]
        RAltKey = 165,

        VolumeMute = 173,

        VolumeDown = 174,

        VolumeUp = 175,

        MediaNextTrack = 176,

        MediaPreviousTrack = 177,

        MediaStop = 178,

        MediaPlayPause = 179,

        LaunchMail = 180,

        SelectMedia = 181,

        LaunchApplication1 = 182,

        LaunchApplication2 = 183,

        // Summary: The OEM Semicolon key (Windows 2000 or later).
        [Display(Name = "Semicolon")]
        OemSemicolon = 186,

        // Summary: The OEM 1 key.
        Oem1 = 186,

        // Summary: The OEM plus key
        Oemplus = 187,

        // Summary: The OEM comma key
        Oemcomma = 188,

        // Summary: The OEM minus key
        OemMinus = 189,

        // Summary: The OEM period key
        OemPeriod = 190,

        // Summary: The OEM question mark key
        OemQuestion = 191,

        // Summary: The OEM tilde key
        Oemtilde = 192,

        // Summary: The OEM open bracket key
        OemOpenBrackets = 219,

        // Summary: The OEM pipe key
        OemPipe = 220,

        // Summary: The OEM close bracket key (Windows 2000 or later).
        OemCloseBrackets = 221,

        // Summary: The OEM singled/double quote key
        OemQuotes = 222,

        // Summary: The OEM 8 key.
        Oem8 = 223,

        // Summary: The OEM angle bracket or backslash key
        OemBackslash = 226,

        // Summary: The PLAY key.
        Play = 250,

        // Summary: The ZOOM key.
        Zoom = 251,
    }
}