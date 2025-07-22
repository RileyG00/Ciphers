# Griffinere Cipher 🔐

The **Griffinere** cipher is a custom encryption algorithm in C# designed for reversible, base64-normalized encryption using a repeating key. Inspired by the Vigenère cipher, it adds configurable alphabet support, input validation, and padding-based encryption length enforcement.

---

## 📦 Installation

Install from the NPM Registry:

```
npm i substitution-ciphers@latest
```
In your code:
```
import { Griffinere } from "substitution-ciphers";
```


## ✨ Features
🔐 Encrypts and decrypts alphanumeric or custom alphabet-based strings

🧩 Allows defining your own alphabet

📏 Supports minimum-length encrypted responses via padding

✅ Includes strong validation for alphabet and key integrity

🧪 Unit tested with Vitest

## 🧰 Usage

1.1: Creating the Cipher Using the default alphabet:
```
const key: string = "YOURKEY";
const griffinereDefault: Griffinere = new Griffinere(key);
``` 
Default alphabet includes:
```
A-Z
a-z
0-9
```
    
1.2: Creating the Cipher Using a custom alphabet:
```
const alphabet: "ABCDEFGHIJKLMNOPQRSTUVWXYZ12345";
const key: string = "YOURKEY";
const griffinereDefault: Griffinere = new Griffinere(key, alphabet);
``` 

### Alphabet Rules
1. Must not contain . (dot character)
2. All characters must be unique
3. All characters in the key must exist in the alphabet

2.1: Encrypt a String
```
const encryptedString: string = griffinere.encryptString("Hello World 123");
//outputs: LUKsbK8 OK9ybKJ FC3z
```

2.2: Encrypt a String with Minimum Length
```
const encryptedString: string = griffinere.encryptStringWithMinimumLength("Hello World 123", 24);
//outputs: cm9JbAxsIJg.LUKsbK8 OK9ybKJ FC3z.Fw
```

3.1: Decrypt a String
```
const decryptedString: string = griffinere.decryptString(encryptedString);
// Returns the original plain text
```

## ⚠️ Exceptions and Validations

## Tables

| Condition  | Exception |
| ------------- |:-------------:|
| Alphabet contains .      | Error      |
| Alphabet has duplicate characters      | Error      |
| Key contains characters not in alphabet      | Error      |
| Specifying minimum length < 1      | RangeError     |


## 📄 License
MIT License © 2025 Riley Griffin