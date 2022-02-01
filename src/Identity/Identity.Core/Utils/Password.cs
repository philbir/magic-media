using System;
using System.Security.Cryptography;
using System.Text;

namespace MagicMedia.Identity.Services;

/// <summary>
/// Provides helper functions to work with passwords
/// </summary>
public class Password
{
    private static RandomNumberGenerator rngCsp = RandomNumberGenerator.Create();

    /// <summary>
    /// Generates a Random password
    /// </summary>
    /// <param name="length">Number of chars</param>
    /// <param name="includeSpecialChars">Include spaecial chars (@-.'$!?*)</param>
    /// <returns>the generated password</returns>
    public static string GenerateRandomPassword(
        int length,
        bool includeSpecialChars = false)
    {
        var possible = "abcdefghjklmnpqrstuvwyzABCDEFGHJKMNOPQRSTUVWYZ123456789";
        if (includeSpecialChars)
        {
            possible += "@-.'$!?*";
        }
        return GetRandomString(possible, length);
    }

    private static string GetRandomString(string possible, int length)
    {
        var possibleChars = possible.ToCharArray();
        var result = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            result.Append(possibleChars[RollDice((byte)possibleChars.Length)]);

        }
        return result.ToString();
    }


    /// <summary>
    /// This method simulates a roll of the dice.
    /// The input parameter is the number of sides of the dice.
    /// </summary>
    public static byte RollDice(byte numberSides, bool zeroBased = true)
    {
        if (numberSides <= 0)
            throw new ArgumentOutOfRangeException("numberSides");
        // Create a byte array to hold the random value. 
        var randomNumber = new byte[1];
        do
        {
            // Fill the array with a random value.
            rngCsp.GetBytes(randomNumber);
        }
        while (!IsFairRoll(randomNumber[0], numberSides));
        // Return the random number mod the number 
        // of sides.  The possible values are zero- 
        // based 
        return (byte)(randomNumber[0] % numberSides + (zeroBased ? 0 : 1));
    }

    private static bool IsFairRoll(byte roll, byte numSides)
    {
        // There are MaxValue / numSides full sets of numbers that can come up 
        // in a single byte.  For instance, if we have a 6 sided die, there are 
        // 42 full sets of 1-6 that come up.  The 43rd set is incomplete. 
        var fullSetsOfValues = byte.MaxValue / numSides;

        // If the roll is within this range of fair values, then we let it continue. 
        // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use 
        // < rather than <= since the = portion allows through an extra 0 value). 
        // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair 
        // to use. 
        return roll < numSides * fullSetsOfValues;
    }
}
