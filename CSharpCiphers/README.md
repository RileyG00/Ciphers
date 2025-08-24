# Griffinere Cipher 🔐

The **Griffinere** cipher is a custom encryption algorithm in C# designed for reversible, base64-normalized encryption using a repeating key. Inspired by the Vigenère cipher, it adds configurable alphabet support, input validation, and padding-based encryption length enforcement.

---

## 📦 Installation

Install from the Nuget Library:

```
dotnet add package SubstitutionCiphers
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
const string key = "YourSecureKey";
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
const string customAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ12345";
const string key = "YOURKEY";
Griffinere cipher = new(customAlphabet, key);
``` 

### Alphabet Rules
1. Must not contain . (dot character)
2. All characters must be unique
3. All characters in the key must exist in the alphabet

2.1: Encrypt a String
```
Griffinere cipher = new("nGOvtaRXyMEtCOO9");
string plainText = "Hello World 123";
string encrypted = cipher.EncryptString(plainText);
//outputs: sjnyIdM trIKZ3v 0m7m
```

2.2: Encrypt a String with Minimum Length
```
Griffinere cipher = new("nGOvtaRXyMEtCOO9");
const string plainText = "Hello World 123";
string encrypted = cipher.EncryptString(plainText, minimumResponseLength: 24);
//outputs: w5WFeRLvJ1q.sjnyIdM trIKZ3v 0m7m.c
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
| Alphabet does not contain at least 3 unique characters      | ArgumentException     |
| Key does not contain at least 3 unique characters      | ArgumentException     |
| Key contains characters not in alphabet      | ArgumentException     |
| Specifying minimum length < 1      | ArgumentOutOfRangeException     |


## 📄 License
MIT License © 2025 Riley Griffin