using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;

namespace Mnemosyne.UnitTests
{
    [TestClass]
    public class PasswordGeneratorTests
    {
        public int Runs = 100;
        public int Length = 20;

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GeneratePassword_NoLength()
        {
            Length = 0;
            try
            {
                string[] passwords = GeneratePasswords(new PasswordGenerator());
            }
            catch (InvalidOperationException exception)
            {
                Length = 20;
                throw exception;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GeneratePassword_NoCharactersAllowed()
        {
            string[] passwords = GeneratePasswords(new PasswordGenerator());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GeneratePassword_NoSpecialCharactersPassedButRequired()
        {
            PasswordGenerator generator = new PasswordGenerator(new char[1] {' '});
            generator.UseSpecialCharacters = true;
            string[] passwords = GeneratePasswords(generator);
        }

        [TestMethod]
        public void GeneratePassword_Length20()
        {
            int prevLength = Length;
            Length = 20;

            PasswordGenerator generator = new PasswordGenerator();
            generator.UseDigits = true;

            Assert.IsTrue(RegexPasswords(GeneratePasswords(generator), @"^[0-9]{20}$"));

            Length = prevLength;
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

            string specialCharacters = new string(PasswordGenerator.AllowableSpecialCharacters);
            specialCharacters = specialCharacters.Insert(specialCharacters.IndexOf(']'), "\\\\");
            specialCharacters = specialCharacters.Insert(specialCharacters.IndexOf('/'), "\\");
            Assert.IsTrue(RegexPasswords(passwords, @"^[A-Za-z0-9" + specialCharacters + "]+$"));
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

        private string[] GeneratePasswords(PasswordGenerator generator)
        {
            string[] passwords = new string[Runs];
            for (int i = 0; i < passwords.Length; i++)
            {
                passwords[i] = generator.GeneratePassword(Length);
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
