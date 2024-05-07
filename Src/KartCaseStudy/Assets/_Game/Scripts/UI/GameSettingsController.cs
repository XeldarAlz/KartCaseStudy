using System.Collections.Generic;
using System.Linq;
using Febucci.UI;
using Game.Managers;
using I2.Loc;
using Michsky.MUIP;
using UnityEngine;
using Zenject;

namespace Game.UserInterface.MainMenu
{
    /// <summary>
    /// Manages the game settings in the main menu, handling user interactions for graphics and language settings.
    /// Provides functionality to update and localize settings options, and to respond to user inputs.
    /// </summary>
    public class GameSettingsController : MonoBehaviour
    {
        [Header("Graphics Settings")]
        [SerializeField] private TypewriterByCharacter _feedbackText;

        [Header("Dropdowns Settings")]
        [SerializeField] private CustomDropdown _languageDropdown;
        [SerializeField] private CustomDropdown _graphicsQualityDropdown;

        [Inject] private readonly GameEventManager _gameEventManager;

        private Localize _feedbackLocalize;
        private List<string> _graphicsTerms;
        private List<string> _languageTerms;

        private void Awake()
        {
            InitializeSettings();
        }

        private void OnEnable()
        {
            LocalizationManager.OnLocalizeEvent += OnLocalizeEvent;
            LocalizeDropdowns();
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizeEvent -= OnLocalizeEvent;
        }

        /// <summary>
        /// Handles the back button press event.
        /// Triggers the appropriate UI event to navigate back in the menu.
        /// </summary>
        public void BackButtonPressed()
        {
            _gameEventManager.UiEvents.BackButtonPressed();
        }

        /// <summary>
        /// Initializes the settings for the game, including graphics and language options.
        /// Prepares the terms for graphics and language settings and sets up the corresponding dropdowns.
        /// </summary>
        private void InitializeSettings()
        {
            _graphicsTerms = QualitySettings.names.ToList();
            _languageTerms = LocalizationManager.GetAllLanguages();
            _feedbackLocalize = _feedbackText.gameObject.GetComponent<Localize>();

            SetupDropdowns();
        }

        /// <summary>
        /// Sets up the dropdown menus for language and graphics quality settings.
        /// Responsible for populating the dropdowns with the available options for each setting.
        /// </summary>
        private void SetupDropdowns()
        {
            SetupLanguageDropdown();
            SetupGraphicsQualityDropdown();
        }

        /// <summary>
        /// Handles the event triggered when localization settings are changed.
        /// Called to update the dropdowns to reflect the new localization settings.
        /// </summary>
        private void OnLocalizeEvent()
        {
            LocalizeDropdowns();
        }

        /// <summary>
        /// Localizes the content of dropdown menus based on the current language settings.
        /// This method updates the items in both the graphics and language dropdowns to display the correct translations.
        /// </summary>
        private void LocalizeDropdowns()
        {
            LocalizeGraphicsDropdown();
            LocalizeLanguageDropdown();
        }

        /// <summary>
        /// Sets up the language dropdown, populating it with available languages and setting the current language.
        /// </summary>
        private void SetupLanguageDropdown()
        {
            if (_languageDropdown == null) return;
          
            string currentLanguage = LocalizationManager.CurrentLanguage;
            if (LocalizationManager.Sources.Count == 0) LocalizationManager.UpdateSources();
            
            foreach (string language in _languageTerms)
            {
                _languageDropdown.CreateNewItem(language);
            }

            _languageDropdown.ChangeDropdownInfo(_languageTerms.IndexOf(currentLanguage));
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageValueChanged);
            _languageDropdown.onValueChanged.AddListener(OnLanguageValueChanged);
        }

        /// <summary>
        /// Sets up the graphics quality dropdown, populating it with available quality settings.
        /// </summary>
        private void SetupGraphicsQualityDropdown()
        {
            if (_graphicsQualityDropdown == null) return;

            foreach (string graphics in _graphicsTerms)
            {
                _graphicsQualityDropdown.CreateNewItem(graphics);
            }

            _graphicsQualityDropdown.ChangeDropdownInfo(QualitySettings.GetQualityLevel());
            _graphicsQualityDropdown.onValueChanged.RemoveListener(OnGraphicsValueChanged);
            _graphicsQualityDropdown.onValueChanged.AddListener(OnGraphicsValueChanged);
        }

        /// <summary>
        /// Localizes the graphics quality dropdown items.
        /// Translates each graphics quality option in the dropdown to the current language.
        /// </summary>
        private void LocalizeGraphicsDropdown()
        {
            foreach (var term in _graphicsTerms)
            {
                string translation = LocalizationManager.GetTranslation($"Graphics Settings/{term}");
                _graphicsQualityDropdown.items[_graphicsTerms.IndexOf(term)].itemName = translation;
            }

            _graphicsQualityDropdown.SetupDropdown();
            _graphicsQualityDropdown.ChangeDropdownInfo(_graphicsQualityDropdown.selectedItemIndex);
        }

        /// <summary>
        /// Localizes the language dropdown items.
        /// Translates each language option in the dropdown to the current language.
        /// </summary>
        private void LocalizeLanguageDropdown()
        {
            foreach (var term in _languageTerms)
            {
                string translation = LocalizationManager.GetTranslation($"Language/{term}");
                _languageDropdown.items[_languageTerms.IndexOf(term)].itemName = translation;
            }

            _languageDropdown.SetupDropdown();
            _languageDropdown.ChangeDropdownInfo(_languageDropdown.selectedItemIndex);
        }

        /// <summary>
        /// Handles changes in the language setting.
        /// Updates the game's language based on the selected option in the dropdown.
        /// </summary>
        /// <param name="index">The index of the selected language in the dropdown.</param>
        private void OnLanguageValueChanged(int index)
        {
            if (index < 0)
            {
                index = 0;
                _languageDropdown.ChangeDropdownInfo(index);
            }

            LocalizationManager.CurrentLanguage = _languageTerms[index];
            UpdateLanguageFeedback();
        }

        /// <summary>
        /// Handles changes in the graphics quality setting.
        /// Updates the game's graphics quality based on the selected option in the dropdown.
        /// </summary>
        /// <param name="index">The index of the selected graphics quality in the dropdown.</param>
        private void OnGraphicsValueChanged(int index)
        {
            if (index < 0)
            {
                index = 0;
                _graphicsQualityDropdown.ChangeDropdownInfo(index);
            }

            QualitySettings.SetQualityLevel(index);
            UpdateGraphicsFeedback();
        }

        /// <summary>
        /// Updates the feedback text to indicate that the graphics settings have been changed.
        /// </summary>
        private void UpdateGraphicsFeedback()
        {
            _feedbackLocalize.Term = "Settings/Graphics settings changed.";
        }

        /// <summary>
        /// Updates the feedback text to indicate that the language settings have been changed.
        /// </summary>
        private void UpdateLanguageFeedback()
        {
            _feedbackLocalize.Term = "Settings/Language settings changed.";
        }
    }
}