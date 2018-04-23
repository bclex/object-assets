using OA.Core;
using System;
using System.Threading;

namespace OA.Ultima.Core
{
    public class DelayedAction
    {
        volatile Action _action;

        private DelayedAction(Action action, int msDelay)
        {
            _action = action;
            dynamic timer = new Timer(TimerProc);
            timer.Change(msDelay, Timeout.Infinite);
        }

        private void TimerProc(object state)
        {
            try
            {
                // The state object is the Timer object. 
                ((Timer)state).Dispose();
                _action.Invoke();
            }
            catch (Exception ex) { Utils.Exception(ex); }
        }

        public static DelayedAction Start(Action callback, int msDelay)
        {
            return new DelayedAction(callback, msDelay);
        }
    }
}
