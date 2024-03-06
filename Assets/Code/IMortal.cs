namespace Infringed
{
    public interface IMortal
    {
        /// <summary>
        /// Called by DeathState
        /// </summary>
        void OnDeathEnd();
    }
}
