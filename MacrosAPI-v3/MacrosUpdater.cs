using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MacrosAPI_v3
{
    public class MacrosUpdater
    {
        private Thread driverupdaterkeyboard;
        private Thread driverupdatermouse;
        private Thread updater;

        #region Системное

        private MacrosManager _handler;

        public void SetHandler(MacrosManager _handler)
        {
            this._handler = _handler;
        }

        public void Stop()
        {
            if (updater != null)
            {
                updater.Abort();
                updater = null;
            }

            if (driverupdaterkeyboard != null)
            {
                driverupdaterkeyboard.Abort();
                driverupdaterkeyboard = null;
            }

            if (driverupdatermouse != null)
            {
                driverupdatermouse.Abort();
                driverupdatermouse = null;
            }
        }

        public void StartUpdater()
        {
            if (updater == null)
            {
                updater = new Thread(Updater);
                updater.Name = "Updater";
                updater.Start();
            }

            if (driverupdaterkeyboard == null)
            {
                driverupdaterkeyboard = new Thread(DriverUpdaterKB);
                driverupdaterkeyboard.Name = "DriverUpdaterKB";
                driverupdaterkeyboard.Priority = ThreadPriority.Highest;
                driverupdaterkeyboard.Start();
            }

            if (driverupdatermouse == null)
            {
                driverupdatermouse = new Thread(DriverUpdaterMS);
                driverupdatermouse.Name = "DriverUpdaterMS";
                driverupdatermouse.Priority = ThreadPriority.Highest;
                driverupdatermouse.Start();
            }
        }

        private void Updater()
        {
            try
            {
                var keepUpdating = true;
                var stopWatch = new Stopwatch();
                while (keepUpdating)
                {
                    stopWatch.Start();
                    try
                    {
                        _handler.OnUpdate();
                    }
                    catch
                    {
                    }

                    stopWatch.Stop();
                    var elapsed = stopWatch.Elapsed.Milliseconds;
                    stopWatch.Reset();
                    if (elapsed < 1) Thread.Sleep(1 - elapsed);
                }
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void DriverUpdaterKB()
        {
            try
            {
                var keepUpdating = true;
                var stopWatch = new Stopwatch();
                while (keepUpdating)
                {
                    stopWatch.Start();
                    _handler.DriverUpdaterKeyBoard();
                    stopWatch.Stop();
                    var elapsed = stopWatch.Elapsed.Milliseconds;
                    stopWatch.Reset();
                    if (elapsed < 1) Thread.Sleep(1 - elapsed);
                }
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void DriverUpdaterMS()
        {
            try
            {
                var keepUpdating = true;
                var stopWatch = new Stopwatch();
                while (keepUpdating)
                {
                    stopWatch.Start();
                    _handler.DriverUpdaterMouse();
                    stopWatch.Stop();
                    var elapsed = stopWatch.Elapsed.Milliseconds;
                    stopWatch.Reset();
                    if (elapsed < 1) Thread.Sleep(1 - elapsed);
                }
            }
            catch (IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        #endregion
    }
}
