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
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (password == value)
                {
                    return;
                }
                password = value;
                raisePropertyChanged("Password");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private PasswordGenerator generator;

        private List<CheckBox> options;
        private List<CheckBox> allChars;
        private List<CheckBox> readableChars;
        private List<CheckBox> nonreadableChars;

        private string password;

        public PasswordGeneratorWindow(string password = null)
        {
            InitializeComponent();
            initializeControls();
            initializeSlider();

            DataContext = this;
            generator = new PasswordGenerator();
            Password = password;

            chk_defaults.IsChecked = true;

            if (Password == null || Password == "")
            {
                Randomize();
            }
        }

        public void Randomize()
        {
            StringBuilder specialCharacters = new StringBuilder();

            foreach (CheckBox box in allChars)
            {
                if ((bool)box.IsChecked)
                {
                    specialCharacters.Append(box.Content);
                }
            }

            generator.SpecialCharacters = specialCharacters.ToString().ToCharArray();

            generator.UseUppercase = (bool)chk_ucases.IsChecked;
            generator.UseLowercase = (bool)chk_lcases.IsChecked;
            generator.UseDigits = (bool)chk_digits.IsChecked;
            generator.UseSpecialCharacters = (bool)chk_sChars.IsChecked;
            generator.MakePronounceable = (bool)chk_pronounceable.IsChecked;

            Password = generator.GeneratePassword((int)sld_length.Value);
        }

        private void raisePropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void initializeControls()
        {
            options = new List<CheckBox>();
            readableChars = new List<CheckBox>();
            nonreadableChars = new List<CheckBox>();

            options.Add(chk_ucases);
            options.Add(chk_lcases);
            options.Add(chk_digits);
            options.Add(chk_sChars);
            options.Add(chk_pronounceable);
            options.Add(chk_all);

            foreach (CheckBox chk in options)
            {
                chk.Unchecked += chk_options_Unchecked;
            }

            sld_length.ValueChanged += sld_length_ValueChanged;

            readableChars.Add(chk_addressSign);
            readableChars.Add(chk_exclamation);
            readableChars.Add(chk_lessThan);
            readableChars.Add(chk_pipe);
            readableChars.Add(chk_dollarSign);
            readableChars.Add(chk_plusSign);
            readableChars.Add(chk_caret);

            nonreadableChars.Add(chk_quotation);
            nonreadableChars.Add(chk_hashtag);
            nonreadableChars.Add(chk_percentageSign);
            nonreadableChars.Add(chk_ampersand);
            nonreadableChars.Add(chk_apostrophe);
            nonreadableChars.Add(chk_openingParenthesis);
            nonreadableChars.Add(chk_closingParenthesis);
            nonreadableChars.Add(chk_asterisk);
            nonreadableChars.Add(chk_comma);
            nonreadableChars.Add(chk_dash);
            nonreadableChars.Add(chk_period);
            nonreadableChars.Add(chk_forwardSlash);
            nonreadableChars.Add(chk_colon);
            nonreadableChars.Add(chk_semicolon);
            nonreadableChars.Add(chk_equalSign);
            nonreadableChars.Add(chk_greaterThan);
            nonreadableChars.Add(chk_questionMark);
            nonreadableChars.Add(chk_openingBracket);
            nonreadableChars.Add(chk_backSlash);
            nonreadableChars.Add(chk_closingBracket);
            nonreadableChars.Add(chk_underscore);
            nonreadableChars.Add(chk_backtick);
            nonreadableChars.Add(chk_openingCurlyBrace);
            nonreadableChars.Add(chk_closingCurlyBrace);
            nonreadableChars.Add(chk_tilde);

            allChars = readableChars.Concat(nonreadableChars).ToList();

            foreach (CheckBox chk in allChars)
            {
                chk.Checked += chk_allCharsChecked;
                chk.Unchecked += chk_allCharsUnchecked;
            }
        }

        private void initializeSlider()
        {
            sld_length.Minimum = PasswordGenerator.MIN_LENGTH;
            sld_length.Maximum = PasswordGenerator.MAX_LENGTH;
        }

        private void checkCheckboxes(List<CheckBox> checkboxes, bool check)
        {
            foreach (CheckBox chk in checkboxes)
            {
                chk.IsChecked = check;
            }
        }

        private void enableCheckboxes(List<CheckBox> checkboxes, bool enable)
        {
            foreach (CheckBox chk in checkboxes)
            {
                chk.IsEnabled = enable;
            }
        }

        private void uncheckDefaults()
        {
            chk_defaults.IsChecked = false;
        }

        private bool anyBoxesChecked(List<CheckBox> checkboxes)
        {
            bool anyChecked = false;
            foreach (CheckBox chk in checkboxes)
            {
                anyChecked = (bool)chk.IsChecked ? true : anyChecked;
            }
            return anyChecked;
        }

        private bool anyBoxesUnchecked(List<CheckBox> checkboxes)
        {
            bool anyUnchecked = false;
            foreach (CheckBox chk in checkboxes)
            {
                anyUnchecked = (bool)chk.IsChecked ? anyUnchecked : true;
            }
            return anyUnchecked;
        }

        private void btn_copyClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txt_password.Text);
        }

        private void btn_rerollClick(object sender, RoutedEventArgs e)
        {
            Randomize();
        }

        private void btn_useClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void chk_defaults_Checked(object sender, RoutedEventArgs e)
        {
            checkCheckboxes(options, true);
            sld_length.Value = PasswordGenerator.DEFAULT_LENGTH;
        }

        private void chk_options_Unchecked(object sender, RoutedEventArgs e)
        {
            uncheckDefaults();
        }

        private void sld_length_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (sld_length.Value != PasswordGenerator.DEFAULT_LENGTH)
            {
                uncheckDefaults();
            }
        }

        private void chk_pronChecked(object sender, RoutedEventArgs e)
        {
            checkCheckboxes(nonreadableChars, false);
            enableCheckboxes(nonreadableChars, false);
        }

        private void chk_pronUnchecked(object sender, RoutedEventArgs e)
        {
            chk_all.IsChecked = false;
            enableCheckboxes(nonreadableChars, true);
        }

        private void chk_allCharsChecked(object sender, RoutedEventArgs e)
        {
            chk_sChars.IsChecked = anyBoxesChecked(allChars) ? true : chk_sChars.IsChecked;
        }

        private void chk_allCharsUnchecked(object sender, RoutedEventArgs e)
        {
            chk_sChars.IsChecked = !anyBoxesChecked(allChars) ? false : chk_sChars.IsChecked;
            if ((bool)chk_pronounceable.IsChecked)
            {
                chk_all.IsChecked = anyBoxesUnchecked(readableChars) ? false : chk_all.IsChecked;
            }
            else
            {
                chk_all.IsChecked = anyBoxesUnchecked(allChars) ? false : chk_all.IsChecked;
            }
        }

        private void chk_allChecked(object sender, RoutedEventArgs e)
        {
            checkCheckboxes(readableChars, true);
            checkCheckboxes(nonreadableChars, !(bool)chk_pronounceable.IsChecked);
        }

        private void chk_sCharsUnchecked(object sender, RoutedEventArgs e)
        {
            checkCheckboxes(allChars, false);
        }
    }
}
