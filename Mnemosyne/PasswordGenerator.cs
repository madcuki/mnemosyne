using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mnemosyne
{
    public class PasswordGenerator
    {
        public const int MIN_LENGTH = 4;
        public const int MAX_LENGTH = 64;

        public static readonly char[] AllSpecialCharacters = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~".ToCharArray();

        public char[] SpecialCharacters
        {
            private get
            {
                return _special_characters;
            }
            set
            {
                _special_characters = new char[value.Length];
                value.CopyTo(_special_characters, 0);

                _special_vowel_subs = FilterSpecialSubstitutes(_all_special_vowel_subs);
                _special_consonantSubs = FilterSpecialSubstitutes(_all_special_consonant_subs);

                Dictionary<char, char> FilterSpecialSubstitutes(Dictionary<char, char> possibleSubstitutes)
                {
                    return possibleSubstitutes.Where(kvp => SpecialCharacters.Contains(kvp.Value)).ToDictionary(x => x.Key, x => x.Value);
                }
            }
        }

        public bool MakePronounceable;
        public bool UseUppercase;
        public bool UseLowercase;
        public bool UseDigits;
        public bool UseSpecialCharacters;

        private readonly Dictionary<char, char> _all_digit_vowel_subs;
        private readonly Dictionary<char, char> _all_digit_consonant_subs;
        private readonly Dictionary<char, char> _all_special_vowel_subs;
        private readonly Dictionary<char, char> _all_special_consonant_subs;

        private Dictionary<char, char> _special_vowel_subs;
        private Dictionary<char, char> _special_consonantSubs;

        private char[] _special_characters;

        private readonly char[] _letters;
        private readonly char[] _vowels;
        private readonly char[] _consonants;
        private readonly char[] _digits;

        private MnemRandom _random;

        private enum _CharType
        {
            Letter = 0,
            Digit = 1,
            Special = 2,
        }

        public PasswordGenerator()
        {
            _all_digit_vowel_subs = new Dictionary<char, char>()
            {
                { 'a', '4' },
                { 'e', '3' },
                { 'i', '1' },
                { 'o', '0' },
            };

            _all_digit_consonant_subs = new Dictionary<char, char>()
            {
                { 'b', '8' },
                { 's', '5' },
                { 'z', '2' },
            };

            _all_special_vowel_subs = new Dictionary<char, char>()
            {
                { 'a', '@' },
                { 'i', '!' },
            };

            _all_special_consonant_subs = new Dictionary<char, char>()
            {
                { 'c', '<' },
                { 'l', '|' },
                { 's', '$' },
                { 't', '+' },
                { 'v', '^' },
            };

            SpecialCharacters = AllSpecialCharacters;

            MakePronounceable = false;
            UseUppercase = false;
            UseLowercase = false;
            UseDigits = false;
            UseSpecialCharacters = false;

            _letters    = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            _vowels     = "AEIOU".ToCharArray();
            _consonants = "BCDFGHJKLMNPQRSTVWXYZ".ToCharArray();
            _digits     = "0123456789".ToCharArray();

            _random = new MnemRandom();
        }

        public string GeneratePassword(int length)
        {
            if (length < MIN_LENGTH || length > MAX_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            if (!UseUppercase && !UseLowercase && !UseDigits && !UseSpecialCharacters)
            {
                throw new InvalidOperationException("Cannot generate password without some type of character");
            }
            if (UseSpecialCharacters)
            {
                if (SpecialCharacters.Length == 0)
                {
                    throw new InvalidOperationException("Cannot generate password with special characters: none given");
                }
                char[] invalid_special_characters = SpecialCharacters.Except(AllSpecialCharacters).ToArray();
                if (invalid_special_characters.Length > 0)
                {
                    throw new InvalidOperationException(string.Format("Invalid special characters given: \"{0}\"", new string(invalid_special_characters)));
                }
            }

            StringBuilder password = new StringBuilder(length, length);

            int uppercase_counter,
                lowercase_counter,
                digit_counter,
                special_character_counter,
                uppercase_range = UseUppercase ? _letters.Length : 0,
                lowercase_range = UseLowercase ? uppercase_range + _letters.Length : uppercase_range,
                digit_range = UseDigits ? lowercase_range + (MakePronounceable ? _all_digit_vowel_subs.Count + _all_digit_consonant_subs.Count : this._digits.Length) : lowercase_range,
                special_character_range = UseSpecialCharacters ? digit_range + (MakePronounceable ? _special_vowel_subs.Count + _special_consonantSubs.Count : SpecialCharacters.Length) : digit_range,
                total_range = special_character_range;

            bool vowel_switcher,
                previous_vowel_switcher,
                can_repeat,
                uppercase;

            int random;

            (int, int) IterateBuilderLoop(int loop_counter, int character_type_counter)
            {
                if (MakePronounceable)
                {
                    can_repeat = loop_counter == 0 || loop_counter == length - 2 ? false : previous_vowel_switcher != vowel_switcher;
                    previous_vowel_switcher = vowel_switcher;
                    vowel_switcher = can_repeat ? GetRandomBool() : !vowel_switcher;
                }

                return (++loop_counter, ++character_type_counter);
            }

            do
            {
                password.Clear();

                uppercase_counter = 0;
                lowercase_counter = 0;
                digit_counter = 0;
                special_character_counter = 0;

                previous_vowel_switcher = vowel_switcher = GetRandomBool();

                for (int i = 0; i < length;)
                {
                    random = _random.Next(0, total_range);

                    switch (random < lowercase_range ? _CharType.Letter : random < digit_range ? _CharType.Digit : _CharType.Special)
                    {
                        case _CharType.Letter:
                            uppercase = random < uppercase_range;
                            if ((UseUppercase && uppercase) || (UseLowercase && !uppercase))
                            {
                                char c = GetRandomCharacter(MakePronounceable ? (vowel_switcher ? _vowels : _consonants) : _letters);
                                password.Append(uppercase ? char.ToUpper(c) : char.ToLower(c));
                                if (uppercase)
                                {
                                    (i, uppercase_counter) = IterateBuilderLoop(i, uppercase_counter);
                                }
                                else
                                {
                                    (i, lowercase_counter) = IterateBuilderLoop(i, lowercase_counter);
                                }
                            }
                            break;
                        case _CharType.Digit:
                            if (UseDigits)
                            {
                                password.Append(GetRandomCharacter(MakePronounceable ? (vowel_switcher ? _all_digit_vowel_subs.Values.ToArray() : _all_digit_consonant_subs.Values.ToArray()) : _digits));
                                (i, digit_counter) = IterateBuilderLoop(i, digit_counter);
                            }
                            break;
                        default:
                            if (UseSpecialCharacters)
                            {
                                if (MakePronounceable)
                                {
                                    if (vowel_switcher && _special_vowel_subs.Count > 0)
                                    {
                                        password.Append(GetRandomCharacter(_special_vowel_subs.Values.ToArray()));
                                    }
                                    else if (!vowel_switcher && _special_consonantSubs.Count > 0)
                                    {
                                        password.Append(GetRandomCharacter(_special_consonantSubs.Values.ToArray()));
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    password.Append(GetRandomCharacter(SpecialCharacters));
                                }
                                (i, special_character_counter) = IterateBuilderLoop(i, special_character_counter);
                            }
                            break;
                    }
                }

            } while (UseUppercase && uppercase_counter < 1 || UseLowercase && lowercase_counter < 1 || UseDigits && digit_counter < 1 || UseSpecialCharacters && special_character_counter < 1);

            return password.ToString();
        }

        private bool GetRandomBool()
        {
            return _random.Next(0, 2) == 1;
        }

        private char GetRandomCharacter(char[] characters)
        {
            return characters[_random.Next(0, characters.Length)];
        }
    }
}
