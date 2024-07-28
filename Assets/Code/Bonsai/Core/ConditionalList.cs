using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonsai.Core.User
{
    public class ConditionalList : IEnumerable<ConditionalAbort>, ICollection<ConditionalAbort>
    {
        private HashSet<ConditionalAbort> _conditionals = new();

        public int Count => _conditionals.Count;

        public bool IsReadOnly => false;

        public void Add(ConditionalAbort item)
        {
            _conditionals.Add(item);
        }

        public void Clear()
        {
            _conditionals.Clear();
        }

        public bool Contains(ConditionalAbort item)
        {
            return _conditionals.Contains(item);
        }

        public void CopyTo(ConditionalAbort[] array, int arrayIndex)
        {
            _conditionals.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ConditionalAbort> GetEnumerator()
        {
            return _conditionals.GetEnumerator();
        }

        public bool Remove(ConditionalAbort item)
        {
            return _conditionals.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conditionals.GetEnumerator();
        }
    }
}
