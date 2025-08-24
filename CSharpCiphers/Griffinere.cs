using System.Numerics;
using System.Text;

namespace Ciphers;

public class Griffinere
{
	private readonly Dictionary<char, int> _alphabetPositionMap;
	private readonly char[] _alphabet;
	private readonly char[] _key;
	private readonly int _alphabetLength;

	public Griffinere(string key)
	{
		char[] alphabetUnique = ValidateAlphabet(GetDefaultAlphabet(), key);
		_alphabet = alphabetUnique;
		_alphabetPositionMap = CreateAlphabetPositionMap(_alphabet);
		_key = ValidateKey(key);
		_alphabetLength = _alphabet.Length;
	}

	public Griffinere(string alphabet, string key)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(alphabet);
		char[] alphabetUnique = ValidateAlphabet(alphabet, key);

		_alphabet = alphabetUnique;
		_alphabetPositionMap = CreateAlphabetPositionMap(_alphabet);
		_key = ValidateKey(key);
		_alphabetLength = _alphabet.Length;
	}

	private static char[] GetDefaultAlphabet() =>
		[
			'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
			'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
			'0','1','2','3','4','5','6','7','8','9'
		];

	/// <summary>
	/// Validates a string alphabet (unique chars, no '.'), and ensures all key chars exist in the alphabet.
	/// </summary>
	private static char[] ValidateAlphabet(string alphabet, string key)
	{
		HashSet<char> seen = new();
		List<char> uniqueList = new();

		if (alphabet.Contains('.'))
			throw new ArgumentException("Alphabet must not contain '.'");

		foreach (char c in alphabet)
		{
			if (!seen.Add(c))
				throw new ArgumentException($"Duplicate character '{c}' in provided alphabet.");
			uniqueList.Add(c);
		}

		if (uniqueList.Count < 3)
			throw new ArgumentException("Alphabet must contain at least 3 unique characters.");

		foreach (char c in key)
		{
			if (!uniqueList.Contains(c))
				throw new ArgumentException($"Alphabet does not contain the character '{c}' supplied in the key.");
		}

		return uniqueList.ToArray();
	}

	private static char[] ValidateKey(string key)
	{
		if (key.Length < 3)
			throw new ArgumentException("Key must be at least 3 characters long");

		return key.ToCharArray();
	}

	/// <summary>
	/// Validates a char[] alphabet (unique chars, no '.'), and ensures all key chars exist in the alphabet.
	/// </summary>
	private static char[] ValidateAlphabet(char[] alphabetChar, string key)
	{
		string alphabet = string.Join("", alphabetChar);
		return ValidateAlphabet(alphabet, key);
	}

	private static Dictionary<char, int> CreateAlphabetPositionMap(char[] alphabet)
	{
		Dictionary<char, int> map = new();
		for (int i = 0; i < alphabet.Length; i++)
		{
			map[alphabet[i]] = i;
		}
		return map;
	}

	private char[] GetKey(char[] encodedText)
	{
		ArgumentOutOfRangeException.ThrowIfZero(encodedText.Length);

		List<char> key = _key.ToList();
		while (key.Count < encodedText.Length)
			key.AddRange(_key);

		return key.Take(encodedText.Length).ToArray();
	}

	private char ShiftCharacterPositive(char keyChar, char textChar)
	{
		if (!TryGetCharPlacement(keyChar, out int keyPos) || !TryGetCharPlacement(textChar, out int textPos))
			return textChar; // should not trigger now that pre-encoding guarantees alphabet-only

		int shiftedIndex = (keyPos + textPos) % _alphabetLength;
		return _alphabet[shiftedIndex];
	}

	private char ShiftCharacterNegative(char keyChar, char textChar)
	{
		if (!TryGetCharPlacement(keyChar, out int keyPos) || !TryGetCharPlacement(textChar, out int textPos))
			return textChar; // should not trigger now that pre-encoding guarantees alphabet-only

		int shiftedIndex = (textPos - keyPos + _alphabetLength) % _alphabetLength;
		return _alphabet[shiftedIndex];
	}

	private bool TryGetCharPlacement(char c, out int placement) =>
		_alphabetPositionMap.TryGetValue(c, out placement);


	/// <summary>
	/// Encodes arbitrary bytes into a string using ONLY characters from the configured alphabet.
	/// Works for any alphabet length >= 2 (binary and up). Preserves leading zero bytes by
	/// prefixing the first alphabet character for each leading zero.
	/// </summary>
	private string EncodeBytesToAlphabet(ReadOnlySpan<byte> bytes)
	{
		if (bytes.Length == 0) return string.Empty;

		// Count leading zero bytes to restore later.
		int zeroCount = 0;
		for (int i = 0; i < bytes.Length && bytes[i] == 0; i++) zeroCount++;

		// BigInteger expects little-endian two's complement. Ensure positive by appending 0x00.
		byte[] littleEndian = bytes.ToArray().Reverse().Concat(new byte[] { 0 }).ToArray();
		BigInteger value = new(littleEndian);

		// Convert to base-N digits
		if (value.IsZero)
		{
			// All zeros: return zeroCount copies of alphabet[0]
			return new string(_alphabet[0], Math.Max(1, zeroCount));
		}

		List<char> encoded = new();
		BigInteger n = _alphabetLength;
		while (value > 0)
		{
			value = BigInteger.DivRem(value, n, out BigInteger rem);
			encoded.Add(_alphabet[(int)rem]);
		}

		// Append leading zeros as alphabet[0]
		for (int i = 0; i < zeroCount; i++) encoded.Add(_alphabet[0]);

		encoded.Reverse();
		return new string(encoded.ToArray());
	}

	/// <summary>
	/// Decodes a string composed ONLY of alphabet characters back into the original bytes.
	/// Restores leading zeros encoded as prefix alphabet[0] characters.
	/// </summary>
	private byte[] DecodeAlphabetToBytes(ReadOnlySpan<char> text)
	{
		if (text.Length == 0) return Array.Empty<byte>();

		// Count prefix zeros
		int zeroPrefix = 0;
		for (int i = 0; i < text.Length && text[i] == _alphabet[0]; i++) zeroPrefix++;

		BigInteger n = _alphabetLength;
		BigInteger value = BigInteger.Zero;
		foreach (char c in text)
		{
			if (!TryGetCharPlacement(c, out int digit))
				throw new ArgumentException($"Cipher text contains character '{c}' not in the alphabet.");
			value = value * n + digit;
		}

		// Convert BigInteger to big-endian bytes
		byte[] little = value.ToByteArray(); // little-endian two's complement
		if (little.Length > 1 && little[^1] == 0) // remove sign byte if present
			little = little.Take(little.Length - 1).ToArray();
		Array.Reverse(little);

		// Trim potential leading zero introduced by sign handling (not the encoded zeros)
		int trim = 0;
		while (trim < little.Length && little[trim] == 0) trim++;
		byte[] nonPadded = little.Skip(trim).ToArray();

		// Restore encoded leading zeros
		byte[] restored = new byte[zeroPrefix + nonPadded.Length];
		// zeroPrefix bytes already zero-initialized
		Buffer.BlockCopy(nonPadded, 0, restored, zeroPrefix, nonPadded.Length);
		return restored;
	}

	private char[] ToAlphabetCharArray(string text)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(text);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		string encoded = EncodeBytesToAlphabet(bytes);
		return encoded.ToCharArray();
	}

	private static string FromUtf8Bytes(byte[] bytes) => Encoding.UTF8.GetString(bytes);

	private string FromAlphabetCharArray(char[] charArray)
	{
		ArgumentOutOfRangeException.ThrowIfZero(charArray.Length);
		byte[] bytes = DecodeAlphabetToBytes(charArray);
		return FromUtf8Bytes(bytes);
	}


	public string EncryptString(string plainText)
	{
		if (string.IsNullOrWhiteSpace(plainText))
			return string.Empty;

		List<string> result = new();

		foreach (string segment in plainText.Split(' '))
		{
			if (segment == "")
			{
				result.Add(segment);
				continue;
			}

			char[] segmentChars = ToAlphabetCharArray(segment);
			char[] key = GetKey(segmentChars);
			char[] encrypted = segmentChars.Select((c, i) => ShiftCharacterPositive(key[i], c)).ToArray();
			result.Add(new string(encrypted));
		}

		string response = string.Join(" ", result);
		return response;
	}

	public string EncryptString(string plainText, int minimumResponseLength)
	{
		if (string.IsNullOrWhiteSpace(plainText))
			return string.Empty;

		if (minimumResponseLength < 1)
			throw new ArgumentOutOfRangeException(nameof(minimumResponseLength), "Minimum response length must be greater than zero.");

		string stringToFront = string.Empty;
		string stringToBack = string.Empty;

		if (plainText.Length < minimumResponseLength)
		{
			char[] charArray = plainText.Replace(" ", "").ToCharArray();
			int charArrayLength = charArray.Length;
			int needToAdd = minimumResponseLength - plainText.Length;
			int pullFromFront = (int)Math.Ceiling(Convert.ToDouble(needToAdd) / 1.25);
			int pullFromBack = needToAdd - pullFromFront;

			int addedToFront = 0;
			for (int i = 0; i < pullFromFront && addedToFront < pullFromFront; i++)
			{
				if (i < charArrayLength)
				{
					stringToFront += charArray[i];
					addedToFront++;
				}
				else
				{
					i = -1;
				}
			}

			int addedToBack = 0;
			for (int i = 0; i < pullFromBack && addedToBack < pullFromBack; i++)
			{
				if (i < charArrayLength)
				{
					stringToBack += charArray[charArray.Length - 1 - i];
					addedToBack++;
				}
				else
				{
					i = -1;
				}
			}
		}

		if (!string.IsNullOrWhiteSpace(stringToFront))
		{
			string reversedString = string.Create(stringToFront.Length, stringToFront, (span, str) =>
			{
				str.AsSpan().CopyTo(span);
				span.Reverse();
			});

			stringToFront = EncryptString(reversedString) + ".";
		}

		if (!string.IsNullOrEmpty(stringToBack))
		{
			stringToBack = "." + EncryptString(stringToBack);
		}

		string encryptedString = EncryptString(plainText);
		string response = string.Concat(stringToFront, encryptedString, stringToBack);
		return response;
	}

	public string DecryptString(string cipherText)
	{
		if (string.IsNullOrWhiteSpace(cipherText))
			return string.Empty;

		List<string> result = new();

		if (cipherText.Contains('.'))
			cipherText = cipherText.Split('.')[1];

		foreach (string segment in cipherText.Split(' '))
		{
			if (segment == string.Empty)
			{
				result.Add(string.Empty);
				continue;
			}

			char[] segmentChars = segment.ToCharArray();
			char[] key = GetKey(segmentChars);
			char[] decrypted = segmentChars.Select((c, i) => ShiftCharacterNegative(key[i], c)).ToArray();

			string decoded = FromAlphabetCharArray(decrypted);
			result.Add(decoded);
		}

		string response = string.Join(" ", result);
		return response;
	}
}
