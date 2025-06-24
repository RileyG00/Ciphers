# Griffinere Cipher 🔐

The **Griffinere** cipher is a custom encryption algorithm in C# designed for reversible, base64-normalized encryption using a repeating key. Inspired by the Vigenère cipher, it adds configurable alphabet support, input validation, and padding-based encryption length enforcement.

---

## 📦 Installation

Clone this repository or copy the `Griffinere.cs` file into your .NET project.

```
https://github.com/RileyG00/Ciphers
```
In your code:
```
using Ciphers;
```


## ✨ Features
🔐 Encrypts and decrypts alphanumeric or custom alphabet-based strings

🧩 Allows defining your own alphabet

📏 Supports minimum-length encrypted responses via padding

✅ Includes strong validation for alphabet and key integrity

🧪 Unit tested with xUnit

## 🧰 Usage

1.1: Creating the Cipher Using the default alphabet:
```
string key = "YourSecureKey123";
Griffinere cipher = new(key);
``` 
Default alphabet includes:
```
A-Z
a-z
0-9
```
    
1.2: Creating the Cipher Using a custom alphabet:
```
string customAlphabet = "abcdef123456";
string key = "a1c";
Griffinere cipher = new(customAlphabet, key);
``` 

### Alphabet Rules
1. Must not contain . (dot character)
2. All characters must be unique
3. All characters in the key must exist in the alphabet

2.1: Encrypt a String
```
string plainText = "Hello World 123";
string encrypted = cipher.EncryptString(plainText);
```

2.2: Encrypt a String with Minimum Length
```
string plainText = "Hello World 123";
string encrypted = cipher.EncryptString("Short text", minimumResponseLength: 24);
```

3.1: Decrypt a String
```
string decrypted = cipher.DecryptString(encrypted);
// Returns the original plain text
```

## ⚠️ Exceptions and Validations

## Tables

| Condition  | Exception |
| ------------- |:-------------:|
| Alphabet contains .      | ArgumentException     |
| Alphabet has duplicate characters      | ArgumentException     |
| Key contains characters not in alphabet      | ArgumentException     |
| Encrypting empty/null text      | ArgumentNullException     |
| Specifying minimum length < 1      | ArgumentOutOfRangeException     |


## 📄 License
MIT License © 2025 Riley Griffin
