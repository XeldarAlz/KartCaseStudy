namespace Game.Events
{
    public class SettingsEvents
    {
        public delegate void GraphicsSettingsAppliedDelegate(int graphicsQualityIndex = 0);
        public event GraphicsSettingsAppliedDelegate OnGraphicsSettingsApplied;

        public void GraphicsSettingsApplied(int graphicsQualityIndex = 0)
        {
            OnGraphicsSettingsApplied?.Invoke(graphicsQualityIndex);
        }
    }
}