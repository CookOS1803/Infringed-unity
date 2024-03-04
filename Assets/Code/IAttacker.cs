namespace Infringed
{
    public interface IAttacker
    {
        /// <summary>
        /// Called by AttackState
        /// </summary>
        public void AttackStarted();
        /// <summary>
        /// Called by AttackState
        /// </summary>
        public void AttackEnded();
    }
}
