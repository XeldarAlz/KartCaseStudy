using System;
using Zenject;

namespace Game.Managers
{
    /// <summary>
    /// Manages the currency within the game.
    /// </summary>
    public class GameCurrencyManager
    {
        [Inject] private GameEventManager _gameEventManager;

        /// <summary>
        /// Gets the total currency.
        /// </summary>
        public int TotalCurrency => _totalCurrency;
        private int _totalCurrency;

        /// <summary>
        /// Sets the total currency to a specified value, ensuring it does not exceed the maximum allowed value.
        /// </summary>
        /// <param name="value">The new value for the total currency.</param>
        private void SetTotalCurrency(int value)
        {
            if (Math.Min(value, int.MaxValue) is var clampedValue && clampedValue == _totalCurrency)
            {
                return;
            }

            int previousValue = _totalCurrency;
            _totalCurrency = clampedValue;
            _gameEventManager.GameEvents.CurrencyChanged(previousValue, _totalCurrency);
        }

        /// <summary>
        /// Adds a specified amount to the total currency.
        /// </summary>
        /// <param name="amount">The amount to add.</param>
        public void AddTotalCurrency(int amount)
        {
            if (amount < 0) return;

            try
            {
                checked
                {
                    SetTotalCurrency(_totalCurrency + amount);
                }
            }
            catch (OverflowException)
            {
                SetTotalCurrency(int.MaxValue);
            }
        }

        /// <summary>
        /// Subtracts a specified amount from the total currency.
        /// </summary>
        /// <param name="amount">The amount to subtract.</param>
        public void SubtractTotalCurrency(int amount)
        {
            if (amount < 0) return;

            SetTotalCurrency(Math.Max(_totalCurrency - amount, 0));
        }
    }
}
