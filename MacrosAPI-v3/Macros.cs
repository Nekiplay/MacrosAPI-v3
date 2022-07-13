using MacrosAPI_v3.Plugins;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using KeyList = System.Collections.Generic.List<MacrosAPI_v3.Key>;

namespace MacrosAPI_v3
{
    public abstract class Macros
    {
        #region Системное

        private MacrosManager _handler;

        private MacrosManager Handler
        {
            get
            {
                if (master != null)
                    return master.Handler;
                if (_handler != null)
                    return _handler;
                throw new InvalidOperationException("Error");
            }
        }

        public void SetHandler(MacrosManager handler)
        {
            _handler = handler;
        }

        private Macros master;

        protected void SetMaster(Macros master)
        {
            this.master = master;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int keys);

        #endregion

        #region Загрузка и выгрузка плагина

        protected void LoadMacros(Macros macros)
        {
            //Handler.UnLoadMacros(macros);
            Handler.LoadMacros(macros);
        }

        protected void UnLoadMacros(Macros macros)
        {
            Handler.UnLoadMacros(macros);

            if (Handler.OnUnloadPlugin != null) Handler.OnUnloadPlugin(macros);
        }

        protected void UnLoadMacros()
        {
            UnLoadMacros(this);
        }

        protected void RunScript(FileInfo filename)
        {
            Handler.LoadMacros(new Script(filename));
        }

        #endregion

        #region Ивенты плагина

        public virtual void Initialize()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void ReceivedObject(object s)
        {
        }

        public virtual bool OnKeyDown(Key key, bool repeat)
        {
            return false;
        }

        public virtual bool OnKeyUp(Key key)
        {
            return false;
        }

        public virtual bool OnMouseDown(MouseKey key)
        {
            return false;
        }

        public virtual bool OnMouseUp(MouseKey key)
        {
            return false;
        }

        public virtual bool OnMouseMove(int x, int y)
        {
            return false;
        }

        public virtual bool OnMouseWheel(int rolling)
        {
            return false;
        }

        #endregion

        #region Методы плагина

        protected void Sleep(int delay)
        {
            Thread.Sleep(delay);
        }

        #region Работа с клавиатурой

        protected bool IsKeyPressed(Keys key)
        {
            return GetAsyncKeyState((int) key) != 0;
        }

        protected bool IsKeyDown(int deviceID, Key key)
        {
            KeyList deviceDownedKeys;
            if (!Handler.downedKeys.TryGetValue(deviceID, out deviceDownedKeys))
                return false;
            return deviceDownedKeys.Contains(key);
        }

        protected bool IsKeyDown(Key key)
        {
            return IsKeyDown(Handler.keyboardDeviceID, key);
        }

        protected bool IsKeyUp(int deviceID, Key key)
        {
            return !IsKeyDown(deviceID, key);
        }

        protected bool IsKeyUp(Key key)
        {
            return IsKeyUp(Handler.keyboardDeviceID, key);
        }


        protected void KeyDown(int deviceID, params Key[] keys)
        {
            foreach (var key in keys)
            {
                var stroke = new Interception.Stroke();
                stroke.Key = Handler.ToKeyStroke(key, true);
                Interception.Send(Handler.keyboard, deviceID, ref stroke, 1);
            }
        }

        protected void KeyDown(params Key[] keys)
        {
            KeyDown(Handler.keyboardDeviceID, keys);
        }

        protected void KeyUp(int deviceID, params Key[] keys)
        {
            foreach (var key in keys)
            {
                var stroke = new Interception.Stroke();
                stroke.Key = Handler.ToKeyStroke(key, false);
                Interception.Send(Handler.keyboard, deviceID, ref stroke, 1);
            }
        }

        protected void KeyUp(params Key[] keys)
        {
            KeyUp(Handler.keyboardDeviceID, keys);
        }

        #endregion

        #region Работа с мышкой

        protected void MouseScroll(int deviceID, short rolling)
        {
            var stroke = new Interception.Stroke();
            stroke.Mouse.State = Interception.MouseState.Wheel;
            stroke.Mouse.Rolling = rolling;
            Interception.Send(Handler.mouse, deviceID, ref stroke, 1);
        }

        protected void MouseScroll(short rolling)
        {
            MouseScroll(Handler.mouseDeviceID, rolling);
        }

        protected void MouseDown(int deviceID, params MouseKey[] keys)
        {
            foreach (var key in keys)
            {
                var stroke = new Interception.Stroke();
                switch (key)
                {
                    case MouseKey.Left:
                        stroke.Mouse.State = Interception.MouseState.LeftButtonDown;
                        break;
                    case MouseKey.Right:
                        stroke.Mouse.State = Interception.MouseState.RightButtonDown;
                        break;
                    case MouseKey.Middle:
                        stroke.Mouse.State = Interception.MouseState.MiddleButtonDown;
                        break;
                    case MouseKey.Button1:
                        stroke.Mouse.State = Interception.MouseState.Button4Down;
                        break;
                    case MouseKey.Button2:
                        stroke.Mouse.State = Interception.MouseState.Button5Down;
                        break;
                }

                Interception.Send(Handler.mouse, deviceID, ref stroke, 1);
            }
        }

        protected void MouseUp(int deviceID, params MouseKey[] keys)
        {
            foreach (var key in keys)
            {
                var stroke = new Interception.Stroke();
                switch (key)
                {
                    case MouseKey.Left:
                        stroke.Mouse.State = Interception.MouseState.LeftButtonUp;
                        break;
                    case MouseKey.Right:
                        stroke.Mouse.State = Interception.MouseState.RightButtonUp;
                        break;
                    case MouseKey.Middle:
                        stroke.Mouse.State = Interception.MouseState.MiddleButtonUp;
                        break;
                    case MouseKey.Button1:
                        stroke.Mouse.State = Interception.MouseState.Button4Up;
                        break;
                    case MouseKey.Button2:
                        stroke.Mouse.State = Interception.MouseState.Button5Up;
                        break;
                }

                Interception.Send(Handler.mouse, deviceID, ref stroke, 1);
            }
        }

        protected void MouseMove(int deviceID, int x, int y)
        {
            var stroke = new Interception.Stroke();
            stroke.Mouse.X = x;
            stroke.Mouse.Y = y;
            stroke.Mouse.Flags = Interception.MouseFlag.MoveRelative;
            Interception.Send(Handler.mouse, deviceID, ref stroke, 1);
        }

        protected void MouseSet(int deviceID, int x, int y)
        {
            var stroke = new Interception.Stroke();
            stroke.Mouse.X = x;
            stroke.Mouse.Y = y;
            stroke.Mouse.Flags = Interception.MouseFlag.MoveAbsolute;
            Interception.Send(Handler.mouse, deviceID, ref stroke, 1);
        }

        protected void MouseMove(int x, int y)
        {
            MouseMove(Handler.mouseDeviceID, x, y);
        }

        protected void MouseSet(int x, int y)
        {
            MouseSet(Handler.mouseDeviceID, x, y);
        }

        protected void MouseDown(params MouseKey[] keys)
        {
            MouseDown(Handler.mouseDeviceID, keys);
        }

        protected void MouseUp(params MouseKey[] keys)
        {
            MouseUp(Handler.mouseDeviceID, keys);
        }

        #endregion

        protected Bitmap GetScreenShot(Process process)
        {
            var hwnd = process.MainWindowHandle;

            WinAPI.GetWindowRect(hwnd, out var rect);
            using (var image = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    var hdcBitmap = graphics.GetHdc();
                    WinAPI.PrintWindow(hwnd, hdcBitmap, 0);
                    graphics.ReleaseHdc(hdcBitmap);
                }

                return image;
            }
        }

        protected Process GetActiveProcess()
        {
            var h = WinAPI.GetForegroundWindow();
            var pid = 0;
            WinAPI.GetWindowThreadProcessId(h, ref pid);
            var p = Process.GetProcessById(pid);
            return p;
        }

        protected void PluginPostObject(object obj)
        {
            Handler.OnMacrosPostObjectMethod(this, obj);
        }

        #endregion

        #region WinAPI
        protected ushort GetKeyboardLayout()
        {
            return WinAPI.GetKeyboardLayout();
        }

        protected uint SetKeyboardLayout(ushort layout, KeyboardLayoutFlags flags)
        {
            return WinAPI.ActivateKeyboardLayout(layout, flags);
        }

        protected uint SetKeyboardLayout(KeyBoardLayouts layout, KeyboardLayoutFlags flags)
        {
            return WinAPI.ActivateKeyboardLayout(layout, flags);
        }

        #endregion
    }
}
