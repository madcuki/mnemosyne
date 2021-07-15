using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Mnemosyne
{
    public partial class PasswordGeneratorWindow : Window, INotifyPropertyChanged
    {
        public const int DEFAULT_LENGTH = 24;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password == value)
                {
                    return;
                }
                _password = value;
                _RaisePropertyChanged("Password");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private PasswordGenerator _generator;

        private List<CheckBox> _option_checkboxes;
        private List<CheckBox> _all_special_character_checkboxes;
        private List<CheckBox> _pronounceable_special_character_checkboxes;
        private List<CheckBox> _unpronounceable_special_character_checkboxes;

        private bool _check_all_special_characters;
        private bool _uncheck_defaults_checkbox;

        private string _password;

        public PasswordGeneratorWindow(string password = null)
        {
            InitializeComponent();
            _InitializeControls();
            _InitializeSlider();

            DataContext = this;
            _generator = new PasswordGenerator();
            Password = password;

            _check_all_special_characters = true;
            _uncheck_defaults_checkbox = true;

            _chk_Defaults.IsChecked = true;

            if (Password == null || Password == "")
            {
                Randomize();
            }
        }

        public void Randomize()
        {
            StringBuilder special_characters = new StringBuilder();

            foreach (CheckBox box in _all_special_character_checkboxes)
            {
                if ((bool)box.IsChecked)
                {
                    special_characters.Append(box.Content);
                }
            }

            _generator.SpecialCharacters     = special_characters.ToString().ToCharArray();
            _generator.MakePronounceable     = (bool)_chk_MakePronounceable.IsChecked;
            _generator.UseUppercase          = (bool)_chk_UseUppercases.IsChecked;
            _generator.UseLowercase          = (bool)_chk_UseLowercases.IsChecked;
            _generator.UseDigits             = (bool)_chk_UseDigits.IsChecked;
            _generator.UseSpecialCharacters  = (bool)_chk_UseSpecialCharacters.IsChecked;

            Password = _generator.GeneratePassword((int)_sld_Length.Value);
        }

        private void _RaisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void _InitializeControls()
        {
            _option_checkboxes = new List<CheckBox>();
            _pronounceable_special_character_checkboxes = new List<CheckBox>();
            _unpronounceable_special_character_checkboxes = new List<CheckBox>();

            _option_checkboxes.Add(_chk_UseUppercases);
            _option_checkboxes.Add(_chk_UseLowercases);
            _option_checkboxes.Add(_chk_UseDigits);
            _option_checkboxes.Add(_chk_UseSpecialCharacters);
            _option_checkboxes.Add(_chk_MakePronounceable);

            foreach (CheckBox checkbox in _option_checkboxes)
            {
                checkbox.Unchecked += DefaultCheckboxes_Unchecked;
            }

            _sld_Length.ValueChanged += _sld_Length_ValueChanged;

            _pronounceable_special_character_checkboxes.Add(_chk_AddressSign);
            _pronounceable_special_character_checkboxes.Add(_chk_Exclamation);
            _pronounceable_special_character_checkboxes.Add(_chk_LessThan);
            _pronounceable_special_character_checkboxes.Add(_chk_Pipe);
            _pronounceable_special_character_checkboxes.Add(_chk_DollarSign);
            _pronounceable_special_character_checkboxes.Add(_chk_PlusSign);
            _pronounceable_special_character_checkboxes.Add(_chk_Caret);

            foreach (CheckBox checkbox in _pronounceable_special_character_checkboxes)
            {
                checkbox.Unchecked += DefaultCheckboxes_Unchecked;
            }

            _unpronounceable_special_character_checkboxes.Add(_chk_Quotation);
            _unpronounceable_special_character_checkboxes.Add(_chk_Hashtag);
            _unpronounceable_special_character_checkboxes.Add(_chk_PercentageSign);
            _unpronounceable_special_character_checkboxes.Add(_chk_Ampersand);
            _unpronounceable_special_character_checkboxes.Add(_chk_Apostrophe);
            _unpronounceable_special_character_checkboxes.Add(_chk_OpeningParenthesis);
            _unpronounceable_special_character_checkboxes.Add(_chk_ClosingParenthesis);
            _unpronounceable_special_character_checkboxes.Add(_chk_Asterisk);
            _unpronounceable_special_character_checkboxes.Add(_chk_Comma);
            _unpronounceable_special_character_checkboxes.Add(_chk_Dash);
            _unpronounceable_special_character_checkboxes.Add(_chk_Period);
            _unpronounceable_special_character_checkboxes.Add(_chk_ForwardSlash);
            _unpronounceable_special_character_checkboxes.Add(_chk_Colon);
            _unpronounceable_special_character_checkboxes.Add(_chk_Semicolon);
            _unpronounceable_special_character_checkboxes.Add(_chk_EqualSign);
            _unpronounceable_special_character_checkboxes.Add(_chk_GreaterThan);
            _unpronounceable_special_character_checkboxes.Add(_chk_QuestionMark);
            _unpronounceable_special_character_checkboxes.Add(_chk_OpeningBracket);
            _unpronounceable_special_character_checkboxes.Add(_chk_BackSlash);
            _unpronounceable_special_character_checkboxes.Add(_chk_ClosingBracket);
            _unpronounceable_special_character_checkboxes.Add(_chk_Underscore);
            _unpronounceable_special_character_checkboxes.Add(_chk_Backtick);
            _unpronounceable_special_character_checkboxes.Add(_chk_OpeningCurlyBrace);
            _unpronounceable_special_character_checkboxes.Add(_chk_ClosingCurlyBrace);
            _unpronounceable_special_character_checkboxes.Add(_chk_Tilde);

            _all_special_character_checkboxes = _pronounceable_special_character_checkboxes.Concat(_unpronounceable_special_character_checkboxes).ToList();

            foreach (CheckBox checkbox in _all_special_character_checkboxes)
            {
                checkbox.Checked += _chk_AnySpecialCharacter_Checked;
                checkbox.Unchecked += _chk_AnySpecialCharacter_Unchecked;
            }
        }

        private void _InitializeSlider()
        {
            _sld_Length.Minimum = PasswordGenerator.MIN_LENGTH;
            _sld_Length.Maximum = PasswordGenerator.MAX_LENGTH;
        }

        private void _CheckCheckboxes(List<CheckBox> checkboxes, bool check)
        {
            foreach (CheckBox checkbox in checkboxes)
            {
                checkbox.IsChecked = check;
            }
        }

        private bool _AnyCheckboxesChecked(List<CheckBox> checkboxes)
        {
            bool any_checked = false;
            foreach (CheckBox checkbox in checkboxes)
            {
                any_checked = (bool)checkbox.IsChecked ? true : any_checked;
            }
            return any_checked;
        }

        private void _EnableCheckboxes(List<CheckBox> checkboxes, bool enable)
        {
            foreach (CheckBox checkbox in checkboxes)
            {
                checkbox.IsEnabled = enable;
            }
        }

        private void _UncheckDefaultsCheckbox()
        {
            _chk_Defaults.IsChecked = false;
        }

        private void _btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_txt_Password.Text);
        }

        private void _btn_Randomize_Click(object sender, RoutedEventArgs e)
        {
            Randomize();
        }

        private void _btn_Use_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void _chk_Defaults_Checked(object sender, RoutedEventArgs e)
        {
            _uncheck_defaults_checkbox = false;
            _chk_UseSpecialCharacters.IsChecked = false;
            _uncheck_defaults_checkbox = true;

            _CheckCheckboxes(_option_checkboxes, true);
            _sld_Length.Value = DEFAULT_LENGTH;
        }

        private void DefaultCheckboxes_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_uncheck_defaults_checkbox)
            {
                _UncheckDefaultsCheckbox();
            }
        }

        private void _sld_Length_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_sld_Length.Value != DEFAULT_LENGTH)
            {
                _UncheckDefaultsCheckbox();
            }
        }

        private void _chk_MakePronounceable_Checked(object sender, RoutedEventArgs e)
        {
            _CheckCheckboxes(_unpronounceable_special_character_checkboxes, false);
            _EnableCheckboxes(_unpronounceable_special_character_checkboxes, false);
        }

        private void _chk_MakePronounceable_Unchecked(object sender, RoutedEventArgs e)
        {
            _EnableCheckboxes(_unpronounceable_special_character_checkboxes, true);
        }

        private void _chk_AnySpecialCharacter_Checked(object sender, RoutedEventArgs e)
        {
            _check_all_special_characters = false;
            _chk_UseSpecialCharacters.IsChecked = _AnyCheckboxesChecked(_all_special_character_checkboxes) ? true : _chk_UseSpecialCharacters.IsChecked;
            _check_all_special_characters = true;
        }

        private void _chk_AnySpecialCharacter_Unchecked(object sender, RoutedEventArgs e)
        {
            _chk_UseSpecialCharacters.IsChecked = !_AnyCheckboxesChecked(_all_special_character_checkboxes) ? false : _chk_UseSpecialCharacters.IsChecked;
        }

        private void _chk_UseSpecialCharacters_Unchecked(object sender, RoutedEventArgs e)
        {
            _CheckCheckboxes(_all_special_character_checkboxes, false);
        }

        private void _chk_UseSpecialCharacters_Checked(object sender, RoutedEventArgs e)
        {
            if (_check_all_special_characters)
            {
                _CheckCheckboxes((bool)_chk_MakePronounceable.IsChecked ? _pronounceable_special_character_checkboxes : _all_special_character_checkboxes, true);
            }
        }
    }
}
