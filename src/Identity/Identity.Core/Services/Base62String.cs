using System;
using System.Collections.Generic;
using System.Text;

/*
 * Derived from https://github.com/google/google-authenticator-android/blob/8509dca15f42825dbe560af1c13d7660542eb233/java/com/google/android/apps/authenticator/util/Base32String.java
 * 
 * Copyright 2019 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


namespace MagicMedia.Identity.Services;

/**
* Encodes arbitrary byte arrays as case-insensitive base-32 strings.
*
* <p> The implementation is slightly different than in RFC 4648. During encoding, padding is not
* added, and during decoding the last incomplete chunk is not taken into account. The result is
* that multiple strings decode to the same byte array, for example, string of sixteen 7s ("7...7")
* and seventeen 7s both decode to the same byte array.
*
* <p>to do: Revisit this encoding and whether this ambiguity needs fixing.
*/
internal static class Base32String
{
    private static readonly char[] DIGITS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
    private static readonly int MASK = DIGITS.Length - 1;
    private static readonly int SHIFT = DIGITS.Length.NumberOfTrailingZeros();
    private static Dictionary<char, int> CHAR_MAP = new Dictionary<char, int>();

    static Base32String()
    {
        for (int i = 0; i < DIGITS.Length; i++)
        {
            CHAR_MAP[DIGITS[i]] = i;
        }
    }

    public static string Encode(byte[] data)
    {
        if (data.Length == 0)
        {
            return "";
        }

        // SHIFT is the number of bits per output character, so the length of the
        // output is the length of the input multiplied by 8/SHIFT, rounded up.
        if (data.Length >= (1 << 28))
        {
            // The computation below will fail, so don't do it.
            throw new ArgumentOutOfRangeException("data");
        }

        int outputLength = (data.Length * 8 + SHIFT - 1) / SHIFT;
        StringBuilder result = new StringBuilder(outputLength);

        int buffer = data[0];
        int next = 1;
        int bitsLeft = 8;
        while (bitsLeft > 0 || next < data.Length)
        {
            if (bitsLeft < SHIFT)
            {
                if (next < data.Length)
                {
                    buffer <<= 8;
                    buffer |= (data[next++] & 0xff);
                    bitsLeft += 8;
                }
                else
                {
                    int pad = SHIFT - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            int index = MASK & (buffer >> (bitsLeft - SHIFT));
            bitsLeft -= SHIFT;
            result.Append(DIGITS[index]);
        }

        return result.ToString();
    }
}

internal static class Int32Extensions
{
    private static readonly int[] _lookup =
    {
            32, 0, 1, 26, 2, 23, 27, 0, 3, 16, 24, 30, 28, 11, 0, 13, 4, 7, 17,
            0, 25, 22, 31, 15, 29, 10, 12, 6, 0, 21, 14, 9, 5, 20, 8, 19, 18
        };

    /// <summary>
    /// http://graphics.stanford.edu/~seander/bithacks.html#ZerosOnRightModLookup
    /// </summary>
    public static int NumberOfTrailingZeros(this int value)
    {
        return _lookup[(value & -value) % 37];
    }
}
