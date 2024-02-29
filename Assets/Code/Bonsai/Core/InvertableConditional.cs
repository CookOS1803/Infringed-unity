using UnityEngine;
using Bonsai;
using Bonsai.Core;
using System.Text;

namespace Bonsai.Core.User
{
    public abstract class InvertableConditional : ConditionalAbort
    {
        [SerializeField] protected bool _inverted = false;

        protected abstract bool _InvertableCondition();

        public override sealed bool Condition()
        {
            return _InvertableCondition() ^ _inverted;
        }

        public override void Description(StringBuilder builder)
        {
            builder.AppendFormat("Fails if condition is {0}\n", _inverted);
            base.Description(builder);
        }
    }
}
