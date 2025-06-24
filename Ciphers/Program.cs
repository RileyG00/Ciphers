using Ciphers;

const string key = "N3bhd1u6gh6Uh88H083envHwuUSec72i";
const string plainText = "Short text";

Griffinere griffinere = new(key);

griffinere.EncryptString(plainText, 64);







