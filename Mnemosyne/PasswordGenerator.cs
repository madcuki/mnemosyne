using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mnemosyne
{
    public class PasswordGenerator
    {
        public const int MIN_LENGTH = 4;
        public const int DEFAULT_LENGTH = 24;
        public const int MAX_LENGTH = 64;

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

        public char[] SpecialCharacters;

        private enum characterType
        {
            Letter,
            Digit,
            Special
        }

        private char[] letters;
        private char[] vowels;
        private char[] consonants;
        private char[] digits;

        private MnemRandom random;

        private Dictionary<char, char> digitVowelSubs;
        private Dictionary<char, char> digitConsonantSubs;
        private Dictionary<char, char> specialVowelSubs;
        private Dictionary<char, char> specialConsonantSubs;

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
            if (length < MIN_LENGTH)
            {
                throw new InvalidOperationException(
                    String.Format(
                        new ExceptionMessage(
                            "Generating password of Length: {0}",
                            "Length ({0}) is less than MinLength ({1})",
                            "Increase Length so that it's greater than or equal to MinLength ({1})"
                        ).ToString(), length, MIN_LENGTH
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
                ucaseRange = (UseUppercase ? letters.Length : 0),
                lcaseRange = (UseLowercase ? ucaseRange + letters.Length : ucaseRange),
                digitRange = (UseDigits ? lcaseRange + (MakePronounceable ? digitVowelSubs.Count + digitConsonantSubs.Count : this.digits.Length) : lcaseRange),
                sCharRange = (UseSpecialCharacters ? digitRange + (MakePronounceable ? specialVowelSubs.Count + specialConsonantSubs.Count : SpecialCharacters.Length) : digitRange),
                totalRange = sCharRange;

            bool vowelSwitch,
                prevSwitch,
                canRepeat,
                isUppercase;

            int random;

            void switchVowelSwitcher(int i, int length)
            {
                if (MakePronounceable)
                {
                    canRepeat = i == length - 1 ? false : prevSwitch != vowelSwitch;
                    prevSwitch = vowelSwitch;
                    vowelSwitch = canRepeat ? GetRandomBool() : !vowelSwitch;
                }
            }

            do
            {
                password.Clear();

                ucases = 0;
                lcases = 0;
                digits = 0;
                sChars = 0;

                prevSwitch = vowelSwitch = GetRandomBool();

                for (int i = 0; i < length;)
                {
                    random = this.random.Next(0, totalRange);

                    switch (random < lcaseRange ? characterType.Letter : random < digitRange ? characterType.Digit : characterType.Special)
                    {
                        case characterType.Letter:
                            isUppercase = random < ucaseRange;
                            if ((UseUppercase && isUppercase) || (UseLowercase && !isUppercase))
                            {
                                char c = GetRandomCharacter(MakePronounceable ? (vowelSwitch ? vowels : consonants) : letters);
                                password.Append(isUppercase ? Char.ToUpper(c) : Char.ToLower(c));
                                ucases = isUppercase ? ++ucases : ucases;
                                lcases = !isUppercase ? ++lcases : lcases;
                                i++;
                                switchVowelSwitcher(i, length);
                            }
                            break;
                        case characterType.Digit:
                            if (UseDigits)
                            {
                                password.Append(GetRandomCharacter(MakePronounceable ? (vowelSwitch ? digitVowelSubs.Values.ToArray() : digitConsonantSubs.Values.ToArray()) : this.digits));
                                digits++;
                                i++;
                                switchVowelSwitcher(i, length);
                            }
                            break;
                        default:
                            if (UseSpecialCharacters)
                            {
                                password.Append(GetRandomCharacter(MakePronounceable ? (vowelSwitch ? specialVowelSubs.Values.ToArray() : specialConsonantSubs.Values.ToArray()) : SpecialCharacters));
                                sChars++;
                                i++;
                                switchVowelSwitcher(i, length);
                            }
                            break;
                    }
                }

            } while (UseUppercase && ucases < 1 || UseLowercase && lcases < 1 || UseDigits && digits < 1 || UseSpecialCharacters && sChars < 1);

            return password.ToString();
        }

        public int[] GetRangeFromMinMax()
        {
            int[] range = new int[MAX_LENGTH - (MIN_LENGTH - 1)];

            for (int i = 0; i < MAX_LENGTH; i++)
            {
                range[i] = i + MIN_LENGTH;
            }

            return range;
        }

        private void Initialize(char[] sChars)
        {
            random = new MnemRandom();

            string vowels = "AEIOU",
                consonants = "BCDFGHJKLMNPQRSTVWXYZ";

            letters = (vowels + consonants).ToCharArray();
            this.vowels = vowels.ToCharArray();
            this.consonants = consonants.ToCharArray();
            digits = "1234567890".ToCharArray();

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
            SpecialCharacters = sChars.Length == 0 ? AllowableSpecialCharacters : sChars;

            digitVowelSubs = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableDigitVowelSubs, characterType.Digit));
            digitConsonantSubs = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableDigitConsonantSubs, characterType.Digit));
            specialVowelSubs = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableSpecialVowelSubs, characterType.Special));
            specialConsonantSubs = new Dictionary<char, char>(FilterSpecialSubstitutes(AllowableSpecialConsonantSubs, characterType.Special));

            Dictionary<char, char> FilterSpecialSubstitutes(Dictionary<char, char> substitutes, characterType type)
            {
                return substitutes.Where(kvp => (type == characterType.Special ? SpecialCharacters : digits).Contains(kvp.Value)).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private bool GetRandomBool()
        {
            return random.Next(0, 1) == 1;
        }

        private char GetRandomCharacter(char[] chars)
        {
            return chars[random.Next(0, chars.Length)];
        }
    }
}
