namespace Game.Events
{
    public class PlayerEvents
    {
        public delegate void PlayerDelegate();
        public event PlayerDelegate OnPlayerFallFromMap;
        public event PlayerDelegate OnPlayerJumped;
        public event PlayerDelegate OnPlayerLanded;
        public event PlayerDelegate OnPlayerInteracted;

        public void PlayerFallFromMap()
        {
            OnPlayerFallFromMap?.Invoke();
        }

        public void PlayerJumped()
        {
            OnPlayerJumped?.Invoke();
        }

        public void PlayerLanded()
        {
            OnPlayerLanded?.Invoke();
        }

        public void PlayerInteracted()
        {
            OnPlayerInteracted?.Invoke();
        }
    }
}