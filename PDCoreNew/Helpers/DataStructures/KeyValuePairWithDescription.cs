using System.Text;

namespace PDCoreNew.Helpers.DataStructures
{
    public readonly struct KeyValuePairWithDescription<TKey, TValue, TDescription>
    {
        public KeyValuePairWithDescription(TKey key, TValue value, TDescription description)
        {
            Key = key;
            Value = value;
            Description = description;
        }

        public TKey Key { get; }

        public TValue Value { get; }

        public TDescription Description { get; }

        public void Deconstruct(out TKey key, out TValue value, out TDescription description)
        {
            key = Key;
            value = Value;
            description = Description;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append('[');

            if (Key != null)
            {
                stringBuilder.Append(Key.ToString());
            }

            stringBuilder.Append(", ");

            if (Value != null)
            {
                stringBuilder.Append(Value.ToString());
            }

            stringBuilder.Append(", ");

            if (Description != null)
            {
                stringBuilder.Append(Description.ToString());
            }

            stringBuilder.Append(']');

            return stringBuilder.ToString();
        }
    }
}
