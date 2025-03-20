using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalEngine
{
    public class WebData
    {
        public Guid AgentID { get; set; }

        private int _WebAvailableCount;
        private int _WebDialedCount;
        private bool _WebEnabled;
        private string _Reason;
        public bool MarkSelected;

        public int WebAvailableCount
        {
            get
            {
                return _WebAvailableCount;
            }
            set
            {
                if (_WebAvailableCount != value)
                {
                    MarkSelected = true;
                }
                _WebAvailableCount = value;
            }
        }
        public int WebDialedCount
        {
            get
            {
                return _WebDialedCount;
            }
            set
            {
                if (_WebDialedCount != value)
                {
                    MarkSelected = true;
                }
                _WebDialedCount = value;
            }
        }
        public bool WebEnabled
        {
            get
            {
                return _WebEnabled;
            }
            set
            {
                if (_WebEnabled != value)
                {
                    MarkSelected = true;
                }
                _WebEnabled = value;
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