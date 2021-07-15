using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace Mnemosyne.UnitTests
{
    [TestClass]
    public class PasswordGeneratorTests
    {
        private const int RUNS = 100;
        private const int PASSWORD_LENGTH = 20;

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GeneratePassword_NoLength()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseDigits = true;
            GeneratePasswords(generator, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GeneratePassword_NoCharactersAllowed()
        {
            GeneratePasswords(new PasswordGenerator());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GeneratePassword_SpecialCharactersRequiredButNoneGiven()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.SpecialCharacters = new char[0];
            generator.UseSpecialCharacters = true;
            GeneratePasswords(generator);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GeneratePassword_InvalidSpecialCharactersGiven()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.SpecialCharacters = new char[1] { ' ' };
            generator.UseSpecialCharacters = true;
            GeneratePasswords(generator);
        }

        [TestMethod]
        public void GeneratePassword_Length20()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseDigits = true;

            Assert.IsTrue(RegexPasswords(GeneratePasswords(generator, 20), @"^[0-9]{20}$"));
        }

        [TestMethod]
        public void GeneratePassword_AllCharacters()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseUppercase = true;
            generator.UseLowercase = true;
            generator.UseDigits = true;
            generator.UseSpecialCharacters = true;

            string[] passwords = GeneratePasswords(generator);

            string regex = new string(PasswordGenerator.AllSpecialCharacters);
            regex = regex.Insert(regex.IndexOf(']'), "\\\\");
            regex = regex.Insert(regex.IndexOf('/'), "\\");
            Assert.IsTrue(RegexPasswords(passwords, @"^[A-Za-z0-9" + regex + "]+$"));
        }

        [TestMethod]
        public void GeneratePassword_OnlyUppercase()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseUppercase = true;

            Assert.IsTrue(RegexPasswords(GeneratePasswords(generator), @"^[A-Z]+$"));
        }

        [TestMethod]
        public void GeneratePassword_OnlyLowercase()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseLowercase = true;

            Assert.IsTrue(RegexPasswords(GeneratePasswords(generator), @"^[a-z]+$"));
        }

        [TestMethod]
        public void GeneratePassword_OnlyDigits()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseDigits = true;

            Assert.IsTrue(RegexPasswords(GeneratePasswords(generator), @"^[0-9]+$"));
        }

        [TestMethod]
        public void GeneratePassword_OnlySpecialCharacters()
        {
            PasswordGenerator generator = new PasswordGenerator();
            generator.UseSpecialCharacters = true;

            Assert.IsTrue(RegexPasswords(GeneratePasswords(generator), @"^[!""#$%&'()*+,-.\/:;<=>?@[\\\]^_`{|}~]+$"));
        }

        private string[] GeneratePasswords(PasswordGenerator generator, int length = PASSWORD_LENGTH)
        {
            string[] passwords = new string[RUNS];
            for (int i = 0; i < passwords.Length; i++)
            {
                passwords[i] = generator.GeneratePassword(length);
            }

            return passwords;
        }

        private bool RegexPasswords(string[] passwords, string regex)
        {
            bool passwordsMatch = true;
            foreach (string password in passwords)
            {
                passwordsMatch = Regex.IsMatch(password, regex);
                if (!passwordsMatch)
                    break;
            }

            return passwordsMatch;
        }
    }
}
