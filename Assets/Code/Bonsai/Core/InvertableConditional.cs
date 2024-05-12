using UnityEngine;
using Bonsai;
using Bonsai.Core;
using System.Text;

namespace Bonsai.Core.User
{
    public abstract class InvertableConditional : ConditionalAbort
    {
        [field: SerializeField] public bool Inverted { get; protected set; } = false;

        protected abstract bool _InvertableCondition();

        public override sealed bool Condition()
        {
            return _InvertableCondition() ^ Inverted;
        }

        public override void Description(StringBuilder builder)
        {
            builder.AppendFormat("Fails if condition is {0}\n", Inverted);
            base.Description(builder);
        }
    }
}
