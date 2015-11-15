using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WoW.Fishing
{
    /// <summary>
    /// Note: Most of this was pulled from another source and
    /// used as an example for multiple projects. I believe it
    /// was originally in C++, but have long since forgotten
    /// who wrote it and what it looked like when I found it.
    /// </summary>
    class Devices
    {
        private const int MaxPressDuration = 125;

        #region Windows APIs

        /// <summary>
        /// The SendInput function synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        /// <param name="numberOfInputs">Number of structures in the Inputs array.</param>
        /// <param name="inputs">Pointer to an array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
        /// <param name="sizeOfInputStructure">Specifies the size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
        /// <returns>The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread. To get extended error information, call GetLastError.Microsoft Windows Vista. This function fails when it is blocked by User Interface Privilege Isolation (UIPI). Note that neither GetLastError nor the return value will indicate the failure was caused by UIPI blocking.</returns>
        /// <remarks>
        /// Microsoft Windows Vista. This function is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal or lesser integrity level.
        /// The SendInput function inserts the events in the INPUT structures serially into the keyboard or mouse input stream. These events are not interspersed with other keyboard or mouse input events inserted either by the user (with the keyboard or mouse) or by calls to keybd_event, mouse_event, or other calls to SendInput.
        /// This function does not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with the events that this function generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState function and correct as necessary.
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 SendInput(UInt32 numberOfInputs, Input[] inputs, Int32 sizeOfInputStructure);

        #endregion

        #region Structures

        /// <summary>
        /// The structure used to send data to the Windows API
        /// </summary>
        private struct Input
        {
            /// <summary>
            /// Type of input: Mouse = 0, Keyboard = 1
            /// </summary>
            public UInt32 Type;

            /// <summary>
            /// Set only the Mouse or Keyboard values since
            /// they overwrite each other in memory and are
            /// sent to the same Windows API method which
            /// then decides how and what was meant to happen.
            /// </summary>
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        /// <summary>
        /// The combined/overlayed structure that includes Mouse and Keyboard Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public Mouse.MOUSEINPUT Mouse;

            [FieldOffset(0)]
            public Keyboard.KEYBDINPUT Keyboard;
        }

        #endregion

        private static Random _randomizer = new Random(DateTime.Now.Millisecond);

        public static class Mouse
        {
            #region Properties

            public static Point Location
            {
                get
                {
                    return System.Windows.Forms.Cursor.Position;
                }
            }

            private static Bitmap _Image = null;
            public static Bitmap Image
            {
                get
                {
                    if (_Image != null)
                        _Image.Dispose();

                    CURSORINFO pci;
                    pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                    if (GetCursorInfo(out pci))
                        _Image = new Bitmap(35, 35);

                    using (Graphics g = Graphics.FromImage(_Image))
                        DrawIcon(g.GetHdc(), 0, 0, pci.hCursor);

                    return _Image;
                }
            }

            #endregion

            #region Windows APIs

            [DllImport("user32.dll")]
            static extern bool GetCursorInfo(out CURSORINFO pci);

            [DllImport("user32.dll")]
            static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

            #endregion

            #region Structures

            [StructLayout(LayoutKind.Sequential)]
            struct CURSORINFO
            {
                public Int32 cbSize;
                public Int32 flags;
                public IntPtr hCursor;
                public POINTAPI ptScreenPos;
            }

            [StructLayout(LayoutKind.Sequential)]
            struct POINTAPI
            {
                public int x;
                public int y;
            }

            /// <summary>
            /// The MOUSEINPUT structure contains information about a simulated mouse event. (see: http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
            /// Declared in Winuser.h, include Windows.h
            /// </summary>
            /// <remarks>
            /// If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dxand dy specify information about that movement. The information is specified as absolute or relative integer values. 
            /// If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain normalized absolute coordinates between 0 and 65,535. The event procedure maps these coordinates onto the display surface. Coordinate (0,0) maps onto the upper-left corner of the display surface; coordinate (65535,65535) maps onto the lower-right corner. In a multimonitor system, the coordinates map to the primary monitor. 
            /// Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK is specified, the coordinates map to the entire virtual desktop.
            /// If the MOUSEEVENTF_ABSOLUTE value is not specified, dxand dy specify movement relative to the previous mouse event (the last reported position). Positive values mean the mouse moved right (or down); negative values mean the mouse moved left (or up). 
            /// Relative mouse motion is subject to the effects of the mouse speed and the two-mouse threshold values. A user sets these three values with the Pointer Speed slider of the Control Panel's Mouse Properties sheet. You can obtain and set these values using the SystemParametersInfo function. 
            /// The system applies two tests to the specified relative mouse movement. If the specified distance along either the x or y axis is greater than the first mouse threshold value, and the mouse speed is not zero, the system doubles the distance. If the specified distance along either the x or y axis is greater than the second mouse threshold value, and the mouse speed is equal to two, the system doubles the distance that resulted from applying the first threshold test. It is thus possible for the system to multiply specified relative mouse movement along the x or y axis by up to four times.
            /// </remarks>
            public struct MOUSEINPUT
            {
                /// <summary>
                /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
                /// </summary>
                public Int32 X;

                /// <summary>
                /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
                /// </summary>
                public Int32 Y;

                /// <summary>
                /// If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
                /// Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.
                /// Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
                /// If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
                /// </summary>
                public UInt32 MouseData;

                /// <summary>
                /// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
                /// The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
                /// You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
                /// </summary>
                public UInt32 Flags;

                /// <summary>
                /// Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
                /// </summary>
                public UInt32 Time;

                /// <summary>
                /// Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
                /// </summary>
                public IntPtr ExtraInfo;
            }

            /// <summary>
            /// The mouse button to issue the click event to
            /// </summary>
            public enum MouseButton
            {
                /// <summary>
                /// left mouse button
                /// </summary>
                Left,

                /// <summary>
                /// right mouse button
                /// </summary>
                Right,

                /// <summary>
                /// middle mouse button
                /// </summary>
                Middle,

                /// <summary>
                /// X button, requires the xButton integer to be supplied when simulating click
                /// </summary>
                X
            }

            /// <summary>
            /// The set of MouseFlags for use in the Flags property of the <see cref="MOUSEINPUT"/> structure. (See: http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
            /// </summary>
            public enum MouseFlag : uint
            {
                /// <summary>
                /// Specifies that movement occurred.
                /// </summary>
                MOVE = 0x0001,

                /// <summary>
                /// Specifies that the left button was pressed.
                /// </summary>
                LEFTDOWN = 0x0002,

                /// <summary>
                /// Specifies that the left button was released.
                /// </summary>
                LEFTUP = 0x0004,

                /// <summary>
                /// Specifies that the right button was pressed.
                /// </summary>
                RIGHTDOWN = 0x0008,

                /// <summary>
                /// Specifies that the right button was released.
                /// </summary>
                RIGHTUP = 0x0010,

                /// <summary>
                /// Specifies that the middle button was pressed.
                /// </summary>
                MIDDLEDOWN = 0x0020,

                /// <summary>
                /// Specifies that the middle button was released.
                /// </summary>
                MIDDLEUP = 0x0040,

                /// <summary>
                /// Windows 2000/XP: Specifies that an X button was pressed.
                /// </summary>
                XDOWN = 0x0080,

                /// <summary>
                /// Windows 2000/XP: Specifies that an X button was released.
                /// </summary>
                XUP = 0x0100,

                /// <summary>
                /// Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData. 
                /// </summary>
                WHEEL = 0x0800,

                /// <summary>
                /// Windows 2000/XP: Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
                /// </summary>
                VIRTUALDESK = 0x4000,

                /// <summary>
                /// Specifies that the dx and dy members contain normalized absolute coordinates. If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system. For further information about relative mouse motion, see the following Remarks section.
                /// </summary>
                ABSOLUTE = 0x8000,
            }

            #endregion

            #region Methods

            public static void Move(Point Location)
            {
                System.Windows.Forms.Cursor.Position = Location;

                // 65,000 dots in each direction

                //var inputEvent = new Input();
                //inputEvent.Type = 0;
                //inputEvent.Data.Mouse = new MOUSEINPUT();
                //inputEvent.Data.Mouse.X = Location.X;
                //inputEvent.Data.Mouse.Y = Location.Y;
                //inputEvent.Data.Mouse.Time = 0;
                //inputEvent.Data.Mouse.Flags = ((uint)MouseFlag.ABSOLUTE | (uint)MouseFlag.MOVE);  // (uint)flag;
                //inputEvent.Data.Mouse.MouseData = 0;
                //inputEvent.Data.Mouse.ExtraInfo = IntPtr.Zero;

                //Input[] inputList = new Input[1];
                //inputList[0] = inputEvent;

                //var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof(Input)));
                //if (numberOfSuccessfulSimulatedInputs == 0) throw new Exception("The click simulation was not successful.");
            }

            /// <summary>
            /// simulate the mousedown + mouseup events for the specified mouse key.
            /// </summary>
            /// <param name="button"></param>
            /// <param name="xButton"></param>
            /// <param name="clickDuration"></param>
            public static void Click(MouseButton button, int xButton = 0, int clickDuration = 0)
            {
                MouseFlag downFlag;
                MouseFlag upFlag;

                switch (button)
                {
                    case MouseButton.Left:
                        downFlag = MouseFlag.LEFTDOWN;
                        upFlag = MouseFlag.LEFTUP;
                        break;
                    case MouseButton.Right:
                        downFlag = MouseFlag.RIGHTDOWN;
                        upFlag = MouseFlag.RIGHTUP;
                        break;
                    case MouseButton.Middle:
                        downFlag = MouseFlag.MIDDLEDOWN;
                        upFlag = MouseFlag.MIDDLEUP;
                        break;
                    case MouseButton.X:
                        downFlag = MouseFlag.XDOWN;
                        upFlag = MouseFlag.XUP;
                        break;
                    default:
                        return;
                }

                FireEvent(downFlag, xButton);
                System.Threading.Thread.Sleep(clickDuration != 0 ? clickDuration : _randomizer.Next(MaxPressDuration));
                FireEvent(upFlag, xButton);
            }



            private static void FireEvent(MouseFlag flag, int data)
            {
                var inputEvent = new Input();
                inputEvent.Type = 0;
                inputEvent.Data.Mouse = new MOUSEINPUT();
                inputEvent.Data.Mouse.Flags = (uint)flag;
                //inputEvent.Data.Mouse.MouseData = (uint)data;
                inputEvent.Data.Mouse.ExtraInfo = IntPtr.Zero;

                Input[] inputList = new Input[1];
                inputList[0] = inputEvent;

                var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof(Input)));
                if (numberOfSuccessfulSimulatedInputs == 0) throw new Exception("The click simulation was not successful.");
            }

            #endregion

        }

        public static class Keyboard
        {
            #region Structures

            /// <summary>
            /// The KEYBDINPUT structure contains information about a simulated keyboard event.  (see: http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx)
            /// Declared in Winuser.h, include Windows.h
            /// </summary>
            /// <remarks>
            /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input methodssuch as handwriting recognition or voice recognitionas if it were text input by using the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or WM_KEYUP message to the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
            /// Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag to define keyboard input in terms of the scan code. This is useful to simulate a physical keystroke regardless of which keyboard is currently being used. The virtual key value of a key may alter depending on the current keyboard layout or what other keys were pressed, but the scan code will always be the same.
            /// </remarks>
            public struct KEYBDINPUT
            {
                /// <summary>
                /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
                /// </summary>
                public UInt16 Vk;

                /// <summary>
                /// Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application. 
                /// </summary>
                public UInt16 Scan;

                /// <summary>
                /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
                /// KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
                /// KEYEVENTF_KEYUP - If specified, the key is being released. If not specified, the key is being pressed.
                /// KEYEVENTF_SCANCODE - If specified, wScan identifies the key and wVk is ignored. 
                /// KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
                /// </summary>
                public UInt32 Flags;

                /// <summary>
                /// Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
                /// </summary>
                public UInt32 Time;

                /// <summary>
                /// Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
                /// </summary>
                public IntPtr ExtraInfo;
            }

            /// <summary>
            /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
            /// </summary>
            public enum KeyboardFlag : uint
            {
                /// <summary>
                /// KEYEVENTF_EXTENDEDKEY = 0x0001 (If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).)
                /// </summary>
                EXTENDEDKEY = 0x0001,

                /// <summary>
                /// KEYEVENTF_KEYUP = 0x0002 (If specified, the key is being released. If not specified, the key is being pressed.)
                /// </summary>
                KEYUP = 0x0002,

                /// <summary>
                /// KEYEVENTF_UNICODE = 0x0004 (If specified, wScan identifies the key and wVk is ignored.)
                /// </summary>
                UNICODE = 0x0004,

                /// <summary>
                /// KEYEVENTF_SCANCODE = 0x0008 (Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section.)
                /// </summary>
                SCANCODE = 0x0008,
            }

            #endregion

            #region Methods

            /// <summary>
            /// simulate the keydown + keyup events using the UTF8Encoding.ASCII.GetBytes(key) value.
            /// </summary>
            public static void Click(UInt16 keyCode, int clickDuration = 0)
            {
                KeyDown(keyCode);
                System.Threading.Thread.Sleep(clickDuration != 0 ? clickDuration : _randomizer.Next(MaxPressDuration));
                KeyUp(keyCode);
            }



            /// <summary>
            /// simulate the keydown event using the UTF8Encoding.ASCII.GetBytes(key) value
            /// </summary>
            private static void KeyDown(UInt16 keyCode)
            {
                var down = new Input();
                down.Type = 1;
                down.Data.Keyboard = new KEYBDINPUT();
                down.Data.Keyboard.Vk = keyCode;
                down.Data.Keyboard.Scan = 0;
                down.Data.Keyboard.Flags = 0;
                down.Data.Keyboard.Time = 0;
                down.Data.Keyboard.ExtraInfo = IntPtr.Zero;

                Input[] inputList = new Input[1];
                inputList[0] = down;

                var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof(Input)));
                if (numberOfSuccessfulSimulatedInputs == 0) throw new Exception(string.Format("The key down simulation for {0} was not successful.", keyCode));
            }

            /// <summary>
            /// simulate the keyup event using the UTF8Encoding.ASCII.GetBytes(key) value
            /// </summary>
            private static void KeyUp(UInt16 keyCode)
            {
                var up = new Input();
                up.Type = 1;
                up.Data.Keyboard = new KEYBDINPUT();
                up.Data.Keyboard.Vk = keyCode;
                up.Data.Keyboard.Scan = 0;
                up.Data.Keyboard.Flags = (UInt32)KeyboardFlag.KEYUP;
                up.Data.Keyboard.Time = 0;
                up.Data.Keyboard.ExtraInfo = IntPtr.Zero;

                Input[] inputList = new Input[1];
                inputList[0] = up;

                var numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(typeof(Input)));
                if (numberOfSuccessfulSimulatedInputs == 0) throw new Exception(string.Format("The key up simulation for {0} was not successful.", keyCode));
            }

            #endregion

        }
    }
}
