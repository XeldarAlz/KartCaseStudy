namespace Game.Events
{
    public class UiEvents
    {
        public delegate void UiDelegate();
        public event UiDelegate OnStartButtonPressed;
        public event UiDelegate OnSettingsButtonPressed;
        public event UiDelegate OnExitGameButtonPressed;
        public event UiDelegate OnQuitGameButtonPressed;
        public event UiDelegate OnBackButtonPressed;
        public event UiDelegate OnResumeButtonPressed;

        public void StartGameButtonPressed()
        {
            OnStartButtonPressed?.Invoke();
        }

        public void SettingsButtonPressed()
        {
            OnSettingsButtonPressed?.Invoke();
        }

        public void ExitGameButtonPressed()
        {
            OnExitGameButtonPressed?.Invoke();
        }

        public void QuitGameButtonPressed()
        {
            OnQuitGameButtonPressed?.Invoke();
        }

        public void BackButtonPressed()
        {
            OnBackButtonPressed?.Invoke();
        }

        public void ResumeButtonPressed()
        {
            OnResumeButtonPressed?.Invoke();
        }
    }
}