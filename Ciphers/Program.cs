using Ciphers;

const string key = "3Gg0V6Ld2ey0pRNaukgbTqAjimmZFK2M";
const string plainText = "1000";

Griffinere griffinere = new(key);

Console.WriteLine(griffinere.EncryptString(plainText, 12));







