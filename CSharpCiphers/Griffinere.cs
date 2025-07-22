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
		this._alphabet = alphabetUnique;
        this._alphabetPositionMap = CreateAlphabetPositionMap(this._alphabet);
        this._key = key.ToCharArray();
        this._alphabetLength = this._alphabet.Length;
    }

    public Griffinere(string alphabet, string key)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(alphabet);
		char[] alphabetUnique = ValidateAlphabet(alphabet, key);

        this._alphabet = alphabetUnique;
        this._alphabetPositionMap = CreateAlphabetPositionMap(this._alphabet);
        this._key = key.ToCharArray();
        this._alphabetLength = this._alphabet.Length;
    }

    private static char[] GetDefaultAlphabet() =>
        [
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9'
		];

    private static char[] ValidateAlphabet(string alphabet, string key)
    {
        HashSet<char> seen = new();
        List<char> uniqueList = new();

        if (alphabet.Contains('.'))
        {
            throw new ArgumentException("Alphabet must not contain '.'");
        }

        foreach (char c in alphabet)
        {
            if (!seen.Add(c))
            {
                throw new ArgumentException($"Duplicate character '{c}' in provided alphabet.");    
            }
            
            uniqueList.Add(c);
        }

        foreach (char c in key)
        {
            if (!uniqueList.Contains(c))
            {
                throw new ArgumentException($"Alphabet does not contain the character '{c}' supplied in the key.");
            }
        }

        return uniqueList.ToArray();
    }

	private static char[] ValidateAlphabet(char[] alphabetChar, string key)
	{
        string alphabet = string.Join("", alphabetChar);
    
        HashSet<char> seen = new();
		List<char> uniqueList = new();

		if (alphabet.Contains('.'))
		{
			throw new ArgumentException("Alphabet must not contain '.'");
		}

		foreach (char c in alphabet)
		{
			if (!seen.Add(c))
				throw new ArgumentException($"Duplicate character '{c}' in provided alphabet.");

			uniqueList.Add(c);
		}
        
        foreach (char c in key)
        {
            if (!uniqueList.Contains(c))
            {
                throw new ArgumentException($"Alphabet does not contain the character '{c}' supplied in the key.");
            }
        }
        
		return uniqueList.ToArray();
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

        List<char> key = this._key.ToList();

        while (key.Count < encodedText.Length)
        {
            key.AddRange(this._key);
        }

        return key.Take(encodedText.Length).ToArray();
    }

    private char ShiftCharacterPositive(char keyChar, char textChar)
    {
        if (!this.TryGetCharPlacement(keyChar, out int keyPos) || !this.TryGetCharPlacement(textChar, out int textPos))
            return textChar;

        int shiftedIndex = (keyPos + textPos) % this._alphabetLength;
        return this._alphabet[shiftedIndex];
    }

    private char ShiftCharacterNegative(char keyChar, char textChar)
    {
        if (!this.TryGetCharPlacement(keyChar, out int keyPos) || !this.TryGetCharPlacement(textChar, out int textPos))
            return textChar;

        int shiftedIndex = (textPos - keyPos + this._alphabetLength) % this._alphabetLength;
        return this._alphabet[shiftedIndex];
    }

    private bool TryGetCharPlacement(char c, out int placement)
    {
        return this._alphabetPositionMap.TryGetValue(c, out placement);
    }

    private static char[] ToBase64CharArray(string text)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(text);
        
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        string base64String = Convert.ToBase64String(bytes).Replace("=", string.Empty);
        
        return base64String.ToCharArray();
    }
    
    private static string FromBase64CharArray(char[] charArray)
    {
        ArgumentOutOfRangeException.ThrowIfZero(charArray.Length);

        string base64String = new(charArray);

        if (base64String.Length % 4 != 0)
        {
            if (base64String.Length % 4 == 3)
            {
                base64String += "=";
            } else if (base64String.Length % 4 == 2)
            {
                base64String += "==";
            }
        }

        byte[] result = Convert.FromBase64String(base64String);

        return Encoding.Default.GetString(result);
    }
    
    public string EncryptString(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return string.Empty;
        }
        
        List<string> result = new();

        foreach (string segment in plainText.Split(' '))
        {
            if (segment == "")
            {
				result.Add(segment);
                continue;
			}

            char[] segmentChars = ToBase64CharArray(segment);
            char[] key = this.GetKey(segmentChars);

            char[] encrypted = segmentChars.Select((c, i) => this.ShiftCharacterPositive(key[i], c)).ToArray();
            result.Add(new string(encrypted));
        }

        string response = string.Join(" ", result);
        
        return response;
    }

    public string EncryptString(string plainText, int minimumResponseLength)
    {
		if (string.IsNullOrWhiteSpace(plainText))
		{
			return string.Empty;
		}

		if (minimumResponseLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumResponseLength), "Minimum response length must be greater than zero.");
        }

        string stringToFront = string.Empty;
        string stringToBack = string.Empty;
        
        if (plainText.Length < minimumResponseLength)
        {
            char[] charArray = plainText.Replace(" ","").ToCharArray();
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

            
            stringToFront = this.EncryptString(reversedString) + ".";
        }

        if (!string.IsNullOrEmpty(stringToBack))
        {
            stringToBack = "." + this.EncryptString(stringToBack);
        }

        string encryptedString = this.EncryptString(plainText);

        string response = string.Concat(stringToFront, encryptedString, stringToBack);
                    
        return response;
    }

    
    public string DecryptString(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return string.Empty;
        }

        List<string> result = new();

        if (cipherText.Contains('.'))
        {
            cipherText = cipherText.Split('.')[1];
        }

        foreach (string segment in cipherText.Split(' '))
        {
            if (segment == string.Empty)
            {
                result.Add(string.Empty);
                continue;
            }
            

            char[] segmentChars = segment.ToCharArray();
            char[] key = this.GetKey(segmentChars);

            char[] decrypted = segmentChars.Select((c, i) => ShiftCharacterNegative(key[i], c)).ToArray();
            
            string decryptedBase64 = FromBase64CharArray(decrypted);
            result.Add(new string(decryptedBase64));
        }

        string response = string.Join(" ", result);
        
        return response;
    }
}
