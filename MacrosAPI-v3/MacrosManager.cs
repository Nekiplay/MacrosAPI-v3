using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeyList = System.Collections.Generic.List<MacrosAPI_v3.Key>;

namespace MacrosAPI_v3
{
    public class MacrosManager
    {
        private readonly List<Macros> plugins = new List<Macros>();

        private readonly Dictionary<string, List<Macros>> registeredPluginsPluginChannels =
            new Dictionary<string, List<Macros>>();
        
        public Dictionary<int, KeyList> downedKeys = new Dictionary<int, KeyList>();

        public IntPtr keyboard;
        public int keyboardDeviceID = 1;

        public IntPtr mouse;
        public int mouseDeviceID = 1;

        private readonly MacrosUpdater updater;

        public MacrosManager(MacrosUpdater updater)
        {
            keyboard = Interception.CreateContext();
            Interception.SetFilter(keyboard, Interception.IsKeyboard, Interception.Filter.All);
            mouse = Interception.CreateContext();
            Interception.SetFilter(mouse, Interception.IsMouse, Interception.Filter.All);
            this.updater = updater;
            updater.SetHandler(this);
            updater.StartUpdater();
        }

        private KeyList GetOrCreateKeyList(Dictionary<int, KeyList> dictionary, int deviceID)
        {
            KeyList result;
            if (!dictionary.TryGetValue(deviceID, out result))
            {
                result = new KeyList();
                dictionary[deviceID] = result;
            }

            return result;
        }

        public Key ToKey(Interception.KeyStroke keyStroke)
        {
            var result = keyStroke.Code;

            if ((keyStroke.State & Interception.KeyState.E0) != 0)
                result += 0x100;

            return (Key)result;
        }

        public Interception.KeyStroke ToKeyStroke(Key key, bool down)
        {
            var result = new Interception.KeyStroke();

            if (!down)
                result.State = Interception.KeyState.Up;

            var code = (ushort)key;
            if (code >= 0x100)
            {
                code -= 0x100;
                result.State |= Interception.KeyState.E0;
            }

            result.Code = code;

            return result;
        }

        public void DriverUpdaterMouse()
        {

            var mousedeviceID = Interception.WaitWithTimeout(mouse, 0);
            switch (mousedeviceID)
            {
                case 0:

                    break;
                default:
                    mouseDeviceID = mousedeviceID;
                    break;
            }


            var stroke = new Interception.Stroke();
            while (Interception.Receive(mouse, mousedeviceID, ref stroke, 1) > 0)
            {
                var processed = false;
                switch (stroke.Mouse.State)
                {
                    case Interception.MouseState.LeftButtonDown:
                        processed = OnMouseDown(MouseKey.Left);
                        break;
                    case Interception.MouseState.RightButtonDown:
                        processed = OnMouseDown(MouseKey.Right);
                        break;
                    case Interception.MouseState.MiddleButtonDown:
                        processed = OnMouseDown(MouseKey.Middle);
                        break;
                    case Interception.MouseState.Button4Down:
                        processed = OnMouseDown(MouseKey.Button1);
                        break;
                    case Interception.MouseState.Button5Down:
                        processed = OnMouseDown(MouseKey.Button2);
                        break;


                    case Interception.MouseState.LeftButtonUp:
                        processed = OnMouseUp(MouseKey.Left);
                        break;
                    case Interception.MouseState.RightButtonUp:
                        processed = OnMouseUp(MouseKey.Right);
                        break;
                    case Interception.MouseState.MiddleButtonUp:
                        processed = OnMouseUp(MouseKey.Middle);
                        break;
                    case Interception.MouseState.Button4Up:
                        processed = OnMouseUp(MouseKey.Button1);
                        break;
                    case Interception.MouseState.Button5Up:
                        processed = OnMouseUp(MouseKey.Button2);
                        break;
                    case Interception.MouseState.Wheel:
                        processed = OnMouseWheel(stroke.Mouse.Rolling);
                        break;
                    default:
                        processed = OnMouseMove(stroke.Mouse.X, stroke.Mouse.Y);
                        break;
                }

                if (!processed)
                    Interception.Send(mouse, mousedeviceID, ref stroke, 1);
            }
        }

        public void DriverUpdaterKeyBoard()
        {
            var keyboardDeviceIDdeviceID = Interception.WaitWithTimeout(keyboard, 0);
            switch (keyboardDeviceIDdeviceID)
            {
                case 0:

                    break;
                default:
                    keyboardDeviceID = keyboardDeviceIDdeviceID;
                    break;
            }

            Interception.Stroke stroke = new Interception.Stroke();
            while (Interception.Receive(keyboard, keyboardDeviceIDdeviceID, ref stroke, 1) > 0)
            {
                var key = ToKey(stroke.Key);
                var processed = false;

                var deviceDownedKeys = GetOrCreateKeyList(downedKeys, keyboardDeviceIDdeviceID);
                switch (stroke.Key.State.IsKeyDown())
                {
                    case true:
                        switch (!deviceDownedKeys.Contains(key))
                        {
                            case true:
                                deviceDownedKeys.Add(key);
                                processed = OnKeyDown(key, false);
                                break;
                            case false:
                                processed = OnKeyDown(key, true);
                                break;
                        }

                        break;
                    case false:
                        deviceDownedKeys.Remove(key);
                        processed = OnKeyUp(key);
                        break;
                }

                if (!processed)
                    Interception.Send(keyboard, keyboardDeviceIDdeviceID, ref stroke, 1);
            }
        }

        public void Quit()
        {
            if (updater != null) updater.Stop();
            Interception.DestroyContext(keyboard);
            Interception.DestroyContext(mouse);
            foreach (var p in plugins.ToArray()) UnLoadMacros(p);
        }

        #region Системное

        public static bool isUsingMono => Type.GetType("Mono.Runtime") != null;

        public void OnUpdate()
        {
            foreach (var bot in plugins.ToArray())
                try { bot.Update(); } catch { }
        }

        #endregion

        #region Получение и отправка данных от плагина

        public Action<Macros> OnUnloadPlugin { set; get; }
        public Action<Macros, object> OnMacrosPostObject { set; get; }
        public Action<Macros> OnMacrosLoad { set; get; }

        public void OnMacrosPostObjectMethod(Macros macros, object ob)
        {
            if (OnMacrosPostObject != null) OnMacrosPostObject(macros, ob);
        }

        #endregion

        #region Управление плагином

        public void LoadMacros(Macros macros, bool init = true)
        {
            macros.SetHandler(this);
            plugins.Add(macros);
            if (init)
            {
                var temp = new List<Macros>();
                temp.Add(macros);
                //new Plugin[] { b }
                DispatchPluginEvent(bot => bot.Initialize(), temp);
                if (OnMacrosLoad != null) OnMacrosLoad(macros);
            }
        }

        private bool OnMouseMove(int x, int y)
        {
            bool blocked = false;
            foreach (var macros in plugins.ToArray())
                try 
                {
                    bool temp = macros.OnMouseMove(x, y);
                    if (temp)
                    {
                        blocked = true;
                    }
                } 
                catch { return blocked; }
            return blocked;
        }

        private bool OnMouseDown(MouseKey key)
        {
            bool blocked = false;
            foreach (var macros in plugins.ToArray())
                try
                {
                    bool temp = macros.OnMouseDown(key);
                    if (temp)
                    {
                        blocked = true;
                    }
                } 
                catch { return blocked; }
            return blocked;
        }

        private bool OnMouseUp(MouseKey key)
        {
            bool blocked = false;
            foreach (var macros in plugins.ToArray())
                try 
                {
                    bool temp = macros.OnMouseUp(key);
                    if (temp)
                    {
                        blocked = true;
                    }
                }
                catch { return blocked; }
            return blocked;
        }

        private bool OnMouseWheel(int rolling)
        {
            bool blocked = false;
            foreach (var macros in plugins.ToArray())
                try 
                {
                    bool temp = macros.OnMouseWheel(rolling);
                    if (temp)
                    {
                        blocked = true;
                    }
                } 
                catch { return blocked; }
            return blocked;
        }

        private bool OnKeyDown(Key key, bool repeat)
        {
            bool blocked = false;
            foreach (var macros in plugins.ToArray())
                try 
                {  
                    bool temp = macros.OnKeyDown(key, repeat); 
                    if (temp) 
                    { 
                        blocked = true; 
                    } 
                } 
                catch { return blocked; }
            return blocked;
        }



        private bool OnKeyUp(Key key)
        {
            bool blocked = false;
            foreach (var macros in plugins.ToArray())
                try 
                {
                    bool temp = macros.OnKeyUp(key);
                    if (temp)
                    {
                        blocked = true;
                    }
                } 
                catch { return blocked; }
            return blocked;
        }

        public void MacrosPostObject(Macros macros, object obj)
        {
            foreach (var smacros in plugins.ToArray())
                try { if (macros == smacros) smacros.ReceivedObject(obj); } catch { }
        }

        public void UnLoadMacros(Macros m)
        {
            plugins.RemoveAll(item => ReferenceEquals(item, m));

            var botRegistrations = registeredPluginsPluginChannels.Where(entry => entry.Value.Contains(m)).ToList();
            foreach (var entry in botRegistrations) UnregisterPluginChannel(entry.Key, m);
        }

        #endregion

        #region Регистрация плагинов

        private void DispatchPluginEvent(Action<Macros> action, IEnumerable<Macros> botList = null)
        {
            Macros[] selectedBots;

            if (botList != null)
                selectedBots = botList.ToArray();
            else
                selectedBots = plugins.ToArray();

            foreach (var bot in selectedBots)
                try
                {
                    action(bot);
                }
                catch (Exception e)
                {
                    if (!(e is ThreadAbortException))
                    {
                        //Retrieve parent method name to determine which event caused the exception
                        var frame = new StackFrame(1);
                        var method = frame.GetMethod();
                        var parentMethodName = method.Name;

                        //Display a meaningful error message to help debugging the ChatBot
                        Console.WriteLine(parentMethodName + ": Got error from " + bot + ": " + e);
                    }
                    else
                    {
                        throw;
                    }
                }
        }

        public void UnregisterPluginChannel(string channel, Macros bot)
        {
            if (registeredPluginsPluginChannels.ContainsKey(channel))
            {
                var registeredBots = registeredPluginsPluginChannels[channel];
                registeredBots.RemoveAll(item => ReferenceEquals(item, bot));
                if (registeredBots.Count == 0) registeredPluginsPluginChannels.Remove(channel);
            }
        }

        #endregion
    }
}
