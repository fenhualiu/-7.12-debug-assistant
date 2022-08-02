using System;
using System.Collections.Generic;
using System.Text;

namespace _7._12_debug_assistant.Service
{
    public struct BoolEdge
    {
        public bool ValueChanged { get; private set; }
        private bool _currentValue;
        public bool CurrentValue
        {
            get => _currentValue;
            set
            {
                ValueChanged = ((_currentValue) != (value));
                _currentValue = value;
            }
        }
    }
    public struct StringEdge
    {
        public bool ValueChanged { get; private set; }
        public string OldValue;
        private string _currentValue;
        public string CurrentValue
        {
            get => _currentValue;
            set
            {
                ValueChanged = ((_currentValue) != (value));
                OldValue = _currentValue;
                _currentValue = value;
            }
        }
    }
    public struct ShortEdge
    {
        public bool ValueChanged { get; private set; }
        private short _currentValue;
        public short CurrentValue
        {
            get => _currentValue;
            set
            {
                ValueChanged = ((_currentValue) != (value));
                _currentValue = value;
            }
        }
    }
    public struct IntEdge
    {
        public bool ValueChanged { get; private set; }
        public int OldValue { get; private set; }
        private int _currentValue;
        public int CurrentValue
        {
            get => _currentValue;
            set
            {
                ValueChanged = ((_currentValue) != (value));
                OldValue = _currentValue;
                _currentValue = value;
            }
        }
    }
}
