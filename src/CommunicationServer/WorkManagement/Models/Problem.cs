using System;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    public class Problem
    {
        public string ProblemType 
        { 
            get; 
            private set; 
        }
        public ulong SolvingTimeout 
        { 
            get; 
            private set; 
        }
        public byte[] Data 
        { 
            get; 
            private set; 
        }
        public ulong? Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id.HasValue)
                    throw new Exception("Value has already been set.");

                _id = value;
            }
        }

        public ulong? NumberOfParts 
        { 
            get; 
            set; 
        }

        private ulong? _id;


        public Problem(string type, ulong timeout, byte[] data)
        {
            ProblemType = type;
            SolvingTimeout = timeout;
            Data = data;
        }
    }
}
