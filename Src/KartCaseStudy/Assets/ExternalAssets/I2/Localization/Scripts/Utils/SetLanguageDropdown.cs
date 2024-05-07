using Michsky.MUIP;
using UnityEngine;

namespace I2.Loc
{
	[AddComponentMenu("I2/Localization/SetLanguage Dropdown")]
	public class SetLanguageDropdown : MonoBehaviour
    {
        [SerializeField] private CustomDropdown _dropdown;

        private void OnEnable()
		{
			if (_dropdown == null)
				return;

			var currentLanguage = LocalizationManager.CurrentLanguage;
			if (LocalizationManager.Sources.Count==0) LocalizationManager.UpdateSources();
			var languages = LocalizationManager.GetAllLanguages();

            foreach (string language in languages)
            {
                _dropdown.CreateNewItem(language);
            }
            
            _dropdown.ChangeDropdownInfo(languages.IndexOf(currentLanguage));
            _dropdown.onValueChanged.RemoveListener( OnValueChanged );
            _dropdown.onValueChanged.AddListener( OnValueChanged );
		}

		
		private void OnValueChanged(int index)
		{
			if (index<0)
			{
				index = 0;
                _dropdown.ChangeDropdownInfo(index);
			}

            LocalizationManager.CurrentLanguage = _dropdown.items[index].itemName;
        }
    }
}