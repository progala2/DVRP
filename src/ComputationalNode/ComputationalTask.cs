using System;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.ComputationalNode
{
    internal class ComputationalTask
    {
        private StatusThreadState _state;
        private Task _task;


        public DateTime LastStateChange { get; private set; }

        public StatusThreadState State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                    return;
                _state = value;
                LastStateChange = DateTime.UtcNow;
            }
        }

        public Task Task
        {
            get { return _task; }
            set
            {
                if (_task == value)
                    return;
                _task = value;
                State = _task == null ? StatusThreadState.Idle : StatusThreadState.Busy;
            }
        }

        public ComputationalTask()
        {
            LastStateChange = DateTime.UtcNow;
        }
    }
}
