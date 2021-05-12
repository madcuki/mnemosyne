using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mnemosyne
{
    public class PasswordGenerator
    {
        private enum CharacterType
        {
            Letter,
            Digit,
            Special
        }

        public static readonly int    MinLength                     = 4;
        public static readonly char[] AllowableSpecialCharacters    = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~".ToCharArray();

        public static readonly Dictionary<char, char> AllowableDigitVowelSubs       = new Dictionary<char, char>
        {
            { 'a', '4' },
            { 'e', '3' },
            { 'o', '0' },
        };
        public static readonly Dictionary<char, char> AllowableDigitConsonantSubs   = new Dictionary<char, char>
        {
            { 'b', '8' },
            { 'l', '1' },
            { 's', '5' },
        };
        public static readonly Dictionary<char, char> AllowableSpecialVowelSubs     = new Dictionary<char, char>
        {
            { 'a', '@' },
            { 'i', '!' },
        };
        public static readonly Dictionary<char, char> AllowableSpecialConsonantSubs = new Dictionary<char, char>
        {
            { 'c', '<' },
            { 'l', '|' },
            { 's', '$' },
            { 't', '+' },
            { 'v', '^' },
        };

        public bool MakePronounceable = false;

        public bool UseUppercase            = false;
        public bool UseLowercase            = false;
        public bool UseDigits               = false;
        public bool UseSpecialCharacters    = false;

        protected char[] Letters;
        protected char[] Vowels;
        protected char[] Consonants;
        protected char[] Digits;
        
        public char[] SpecialCharacters;

        protected CryptoRandom CryptoRandom;

        private Dictionary<char, char> DigitVowelSubs;
        private Dictionary<char, char> DigitConsonantSubs;
        private Dictionary<char, char> SpecialVowelSubs;
        private Dictionary<char, char> SpecialConsonantSubs;

        public PasswordGenerator()
        {
            Initialize(AllowableSpecialCharacters);
        }

        public PasswordGenerator(char[] specialCharacters)
        {
            Initialize(specialCharacters);
        }

        public string GeneratePassword(int length)
        {
            if (length < MinLength)
            {
                throw new InvalidOperationException(
                    String.Format(
                        new ExceptionMessage(
                            "Generating password of Length: {0}",
                            "Length ({0}) is less than MinLength ({1})",
                            "Increase Length so that it's greater than or equal to MinLength ({1})"
                        ).ToString(), length, MinLength
                    )
                );
            }
            if (!UseUppercase && !UseLowercase && !UseDigits && !UseSpecialCharacters)
            {
                throw new InvalidOperationException(
                    String.Format(
                        new ExceptionMessage(
                            "Generating password",
                            "Not Allowed to use any types of characters",
                            "Allow some character type to be used"
                        ).ToString()
                    )
                );
            }
            char[] invalidSpecChars = SpecialCharacters.Except(AllowableSpecialCharacters).ToArray();
            if (UseSpecialCharacters && invalidSpecChars.Length > 0)
            {
                throw new InvalidOperationException(
                    String.Format(
                        new ExceptionMessage(
                            "Generating password with special characters",
                            "Invalid special characters where given: \"{1}\"",
                            "Limit special characters to: \"{2}\""
                        ).ToString(), GetType().Name, new string(invalidSpecChars), new string(AllowableSpecialCharacters)
                    )
                );
            }

            StringBuilder password = new StringBuilder(length, length);

            int ucases,
                lcases,
                digits,
                sChars,
                ucaseRange = (UseUppercase ? Letters.Length : 0),
                lcaseRange = (UseLowercase ? ucaseRange + Letters.Length : ucaseRange),
                digitRange = (UseDigits ? lcaseRange + (MakePronounceable ? DigitVowelSubs.Count + DigitConsonantSubs.Count : Digits.Length) : lcaseRange),
                sCharRange = (UseSpecialCharacters ? digitRange + (MakePronounceable ? SpecialVowelSubs.Count + SpecialConsonantSubs.Count : SpecialCharacters.Length) : digitRange),
                totalRange = sCharRange;

            bool vowelSwitch,
                lastVowelSwitch,
                lastVowelSwitchRepeated,
                isUppercase;

            int random;

            void switchVowelSwitcher()
            {
                if (MakePronounceable)
                {
                    lastVowelSwitchRepeated = lastVowelSwitch == vowelSwitch;
                    lastVowelSwitch = vowelSwitch;
                    vowelSwitch = lastVowelSwitchRepeated ? !vowelSwitch : GetRandomBool();
                }
            }

            do
            {
                password.Clear();

                ucases = 0;
                lcases = 0;
                digits = 0;
                sChars = 0;

                lastVowelSwitch = vowelSwitch = GetRandomBool();

                for (int i = 0; i < length;)
                {
                    random = CryptoRandom.Next(0, totalRange);

                    switch (random < lcaseRange ? CharacterType.Letter : random < digitRange ? CharacterType.Digit : CharacterType.Special)
                    {
                        case CharacterType.Letter:
                            isUppercase = random < ucaseRange;
                            if ((UseUppercase && isUppercase) || (UseLowercase && !isUppercase))
                            {
                                char c = GetRandomCharacter(MakePronounceable ? (vowelSwitch ? Vowels : Consonants) : Letters);
                                password.Append(isUppercase ? Char.ToUpper(c) : Char.ToLower(c));
                                ucases = isUppercase ? ++ucases : ucases;
                                lcases = !isUppercase ? ++lcases : lcases;
                                i++;
                                switchVowelSwitcher();
                            }
                            break;
                        case CharacterType.Digit:
                            if (UseDigits)
                            {
                                password.Append(GetRandomCharacter(MakePronounceable ? (vowelSwitch ? DigitVowelSubs.Values.ToArray() : DigitConsonantSubs.Values.ToArray()) : Digits));
                                digits++;
                                i++;
                                switchVowelSwitcher();
                            }
                            break;
                        default:
                            if (UseSpecialCharacters)
                            {
                                password.Append(GetRandomCharacter(MakePronounceable ? (vowelSwitch ? SpecialVowelSubs.Values.ToArray() : SpecialConsonantSubs.Values.ToArray()) : SpecialCharacters));
                                sChars++;
                                i++;
                                switchVowelSwitcher();
                            }
                            break;
                    }
                }

            } while (UseUppercase && ucases < 1 || UseLowercase && lcases < 1 || UseDigits && digits < 1 || UseSpecialCharacters && sChars < 1);

            return password.ToString();
        }

        protected bool GetRandomBool()
        {
            return CryptoRandom.Next(0, 1) == 1;
        }

        protected char GetRandomCharacter(char[] chars)
        {
            return chars[CryptoRandom.Next(0, chars.Length)];
        }

        private void Initialize(char[] sChars)
        {
            CryptoRandom        = new CryptoRandom();

            string vowels       = "AEIOU",
                consonants      = "BCDFGHJKLMNPQRSTVWXYZ";

            Letters             = (vowels + consonants).ToCharArray();
            Vowels              = vowels.ToCharArray();
            Consonants          = consonants.ToCharArray();
            Digits              = "1234567890".ToCharArray();

            if (sChars.Length < 0)
            {
                this.SpecialCharacters = new char[AllowableSpecialCharacters.Length];
                AllowableSpecialCharacters.CopyTo(this.SpecialCharacters, 0);
            }
            else
            {
                this.SpecialCharacters = new char[sChars.Length];
                sChars.CopyTo(this.SpecialCharacters, 0);
            }
            SpecialCharacters   = sChars.Length == 0 ? AllowableSpecialCharacters : sChars;
            
            DigitVowelSubs       = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableDigitVowelSubs, CharacterType.Digit));
            DigitConsonantSubs   = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableDigitConsonantSubs, CharacterType.Digit));
            SpecialVowelSubs     = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableSpecialVowelSubs, CharacterType.Special));
            SpecialConsonantSubs = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableSpecialConsonantSubs, CharacterType.Special));

            Dictionary<char, char> FilterSpecialSubstitutes(Dictionary<char, char> substitutes, CharacterType type)
            {
                return substitutes.Where(kvp => (type == CharacterType.Special ? SpecialCharacters : Digits).Contains(kvp.Value)).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
