namespace OA.Ultima.Input
{
    /// <summary>
    /// A single macro that performs one thing.
    /// </summary>
    public class Macro
    {
        public readonly MacroType Type;

        ValueTypes _valueType =  ValueTypes.None;
        int _valueInteger = -1;
        string _valueString;

        public Macro(MacroType type)
        {
            Type = type;
        }

        public Macro(MacroType type, int value)
            : this(type)
        {
            _valueInteger = value;
            _valueType = ValueTypes.Integer;
        }

        public Macro(MacroType type, string value)
            : this(type)
        {
            _valueString = value;
            _valueType = ValueTypes.String;
        }

        public int ValueInteger
        {
            set
            {
                _valueType =  ValueTypes.Integer;
                _valueInteger = value;
            }
            get
            {
                if (_valueType == ValueTypes.Integer)
                    return _valueInteger;
                return 0;
            }
        }

        public string ValueString
        {
            set
            {
                _valueType = ValueTypes.String;
                _valueString = value;
            }
            get
            {
                if (_valueType == ValueTypes.String)
                    return _valueString;
                return null;
            }
        }

        public ValueTypes ValueType
        {
            get
            {
                return _valueType;
            }
        }

        public override string ToString()
        {
            string value = (_valueType == ValueTypes.None ? string.Empty : (_valueType == ValueTypes.Integer ? _valueInteger.ToString() : _valueString));
            return string.Format("{0} ({1})", Type.ToString(), value);
        }

        public enum ValueTypes
        {
            None,
            Integer,
            String
        }
    }
}
