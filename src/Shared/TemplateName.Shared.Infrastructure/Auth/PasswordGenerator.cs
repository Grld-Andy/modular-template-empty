using TemplateName.Shared.Abstractions.Auth;
using System;

namespace TemplateName.Shared.Infrastructure.Auth;

internal class PasswordGenerator : IPasswordGenerator
{
    public string GeneratePassword(int lengthOfPassword, bool includeLowercase = true, bool includeUppercase = true,
        bool includeNumeric = true, bool includeSpecial = false, bool includeSpaces = false)
    {
        const int MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS = 2;
        const string LOWERCASE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
        const string UPPERCASE_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMERIC_CHARACTERS = "0123456789";
        const string SPECIAL_CHARACTERS = @"!#$%&*@\";
        const string SPACE_CHARACTER = " ";
        const int PASSWORD_LENGTH_MIN = 8;
        const int PASSWORD_LENGTH_MAX = 128;

        if (lengthOfPassword < PASSWORD_LENGTH_MIN || lengthOfPassword > PASSWORD_LENGTH_MAX)
            return "Password length must be between 8 and 128.";

        var characterSet = "";

        if (includeLowercase) characterSet += LOWERCASE_CHARACTERS;

        if (includeUppercase) characterSet += UPPERCASE_CHARACTERS;

        if (includeNumeric) characterSet += NUMERIC_CHARACTERS;

        if (includeSpecial) characterSet += SPECIAL_CHARACTERS;

        if (includeSpaces) characterSet += SPACE_CHARACTER;

        var password = new char[lengthOfPassword];
        var characterSetLength = characterSet.Length;

        var random = new Random();
        for (var characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
        {
            password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];

            var moreThanTwoIdenticalInARow =
                characterPosition > MAXIMUM_IDENTICAL_CONSECUTIVE_CHARS
                && password[characterPosition] == password[characterPosition - 1]
                && password[characterPosition - 1] == password[characterPosition - 2];

            if (moreThanTwoIdenticalInARow) characterPosition--;
        }

        return string.Join(null, password);
    }
}