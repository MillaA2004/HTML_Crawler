using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLfile.CustomFeatures
{
    public class NewStack<T>
    {
        private T[] _items = new T[DefaultCapacity];

        private const int DefaultCapacity = 4;

        private bool IsEmpty => Count == 0;
        public int Count { get; private set; } = 0;

        public void Push(T item)
        {
            if (Count == _items.Length)
            {
                Resize(_items.Length * 2);
            }

            _items[Count++] = item;
        }

        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty.");

            var item = _items[--Count];
            _items[Count] = default!;

            if (Count > 0 && Count < _items.Length / 4)
            {
                Resize(_items.Length / 2);
            }

            return item;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty.");

            return _items[Count - 1];
        }

        private void Resize(int newSize)
        {
            var newArray = new T[newSize];
            Array.Copy(_items, newArray, Count);
            _items = newArray;
        }
    }
}
