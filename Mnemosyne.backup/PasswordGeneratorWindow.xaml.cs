using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mnemosyne
{
    public partial class PasswordGeneratorWindow : Window
    {
        private PasswordGenerator generator;

        private MnemosyneCipherFile file;

        private List<CheckBox> pptions;
        private List<CheckBox> allChars;
        private List<CheckBox> readableChars;
        private List<CheckBox> nonreadableChars;

        public PasswordGeneratorWindow()
        {
            InitializeComponent();

            file = new MnemosyneCipherFile();

            generator = new PasswordGenerator();

            pptions = new List<CheckBox>();
            readableChars = new List<CheckBox>();
            nonreadableChars = new List<CheckBox>();

            pptions.Add(chk_ucases);
            pptions.Add(chk_lcases);
            pptions.Add(chk_digits);
            pptions.Add(chk_sChars);
            pptions.Add(chk_pronounceable);
            pptions.Add(chk_all);

            foreach (CheckBox chk in pptions)
            {
                chk.Unchecked += chk_OptionsUnchecked;
            }

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

            chk_defaults.IsChecked = true;
            randomize();
        }

        private void randomize()
        {
            StringBuilder sb = new StringBuilder();

            if ((bool)chk_addressSign.IsChecked)
                sb.Append('@');

            if ((bool)chk_exclamation.IsChecked)
                sb.Append('!');

            if ((bool)chk_lessThan.IsChecked)
                sb.Append('<');

            if ((bool)chk_pipe.IsChecked)
                sb.Append('|');

            if ((bool)chk_dollarSign.IsChecked)
                sb.Append('$');

            if ((bool)chk_plusSign.IsChecked)
                sb.Append('+');

            if ((bool)chk_caret.IsChecked)
                sb.Append('^');

            if ((bool)chk_questionMark.IsChecked)
                sb.Append('"');

            if ((bool)chk_hashtag.IsChecked)
                sb.Append('#');

            if ((bool)chk_percentageSign.IsChecked)
                sb.Append('%');

            if ((bool)chk_ampersand.IsChecked)
                sb.Append('&');

            if ((bool)chk_apostrophe.IsChecked)
                sb.Append('\'');

            if ((bool)chk_openingParenthesis.IsChecked)
                sb.Append('(');

            if ((bool)chk_closingParenthesis.IsChecked)
                sb.Append(')');

            if ((bool)chk_asterisk.IsChecked)
                sb.Append('*');

            if ((bool)chk_plusSign.IsChecked)
                sb.Append('+');

            if ((bool)chk_comma.IsChecked)
                sb.Append(',');

            if ((bool)chk_dash.IsChecked)
                sb.Append('-');

            if ((bool)chk_period.IsChecked)
                sb.Append('.');

            if ((bool)chk_forwardSlash.IsChecked)
                sb.Append('/');

            if ((bool)chk_colon.IsChecked)
                sb.Append(':');

            if ((bool)chk_semicolon.IsChecked)
                sb.Append(';');

            if ((bool)chk_equalSign.IsChecked)
                sb.Append('=');

            if ((bool)chk_greaterThan.IsChecked)
                sb.Append('>');

            if ((bool)chk_questionMark.IsChecked)
                sb.Append('?');

            if ((bool)chk_openingBracket.IsChecked)
                sb.Append('[');

            if ((bool)chk_backSlash.IsChecked)
                sb.Append('\\');

            if ((bool)chk_closingBracket.IsChecked)
                sb.Append(']');

            if ((bool)chk_underscore.IsChecked)
                sb.Append('_');

            if ((bool)chk_backtick.IsChecked)
                sb.Append('`');

            if ((bool)chk_openingCurlyBrace.IsChecked)
                sb.Append('{');

            if ((bool)chk_closingCurlyBrace.IsChecked)
                sb.Append('}');

            if ((bool)chk_tilde.IsChecked)
                sb.Append('~');

            generator.SpecialCharacters = sb.ToString().ToCharArray();

            generator.UseUppercase = (bool)chk_ucases.IsChecked;
            generator.UseLowercase = (bool)chk_lcases.IsChecked;
            generator.UseDigits = (bool)chk_digits.IsChecked;
            generator.UseSpecialCharacters = (bool)chk_sChars.IsChecked;
            generator.MakePronounceable = (bool)chk_pronounceable.IsChecked;

            txt_password.Text = generator.GeneratePassword(20);
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
            randomize();
        }

        private void btn_saveClick(object sender, RoutedEventArgs e)
        {
            if (file.Save("test"))
            {
                MessageBox.Show("Saved!");
            }
            else
            {
                MessageBox.Show("Something went wrong. Credential was NOT saved.");
            }
        }

        private void chk_defaultsChecked(object sender, RoutedEventArgs e)
        {
            checkCheckboxes(pptions, true);
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

        private void chk_OptionsUnchecked(object sender, RoutedEventArgs e)
        {
            chk_defaults.IsChecked = false;
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
