using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalEngine
{
    public class ACDData
    {
        public Guid AgentID { get; set; }

        private int _AcdAvailableCount;
        private int _AcdTakenCount;
        private bool _AcdEnabled;
        private string _Reason;
        public bool MarkSelected;

        public int AcdAvailableCount
        {
            get
            {
                return _AcdAvailableCount;
            }
            set
            {
                if (_AcdAvailableCount != value)
                {
                    MarkSelected = true;
                }
                _AcdAvailableCount = value;
            }
        }
        public int AcdTakenCount
        {
            get
            {
                return _AcdTakenCount;
            }
            set
            {
                if (_AcdTakenCount != value)
                {
                    MarkSelected = true;
                }
                _AcdTakenCount = value;
            }
        }
        public bool AcdEnabled
        {
            get
            {
                return _AcdEnabled;
            }
            set
            {
                if (_AcdEnabled != value)
                {
                    MarkSelected = true;
                }
                _AcdEnabled = value;
            }
        }

        public string Reason
        {
            get
            {
                return _Reason;
            }
            set
            {
                if (_Reason != value)
                {
                    MarkSelected = true;
                }
                _Reason = value;
            }
        }
    }
}