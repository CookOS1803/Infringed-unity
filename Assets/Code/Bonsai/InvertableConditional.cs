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
            base.Description(builder);
            builder.AppendFormat(", if condition is {0}", !_inverted);
        }
    }
}
