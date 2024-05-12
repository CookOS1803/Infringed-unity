namespace Infringed
{
    public interface IAttacker
    {
        /// <summary>
        /// Called by AttackState
        /// </summary>
        public void AttackStateStarted();
        /// <summary>
        /// Called by AttackState
        /// </summary>
        public void AttackStateEnded();
    }
}
