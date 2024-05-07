namespace Game.Events
{
    public class GameEvents
    {
        public delegate void CurrencyChangedDelegate(int previousCount, int newCount);
        public event CurrencyChangedDelegate OnCurrencyChanged;

        public void CurrencyChanged(int previousCount, int newCount)
        {
            OnCurrencyChanged?.Invoke(previousCount, newCount);
        }
    }
}