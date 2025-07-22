using Ciphers;

const string key = "dHiNt8C8JY1RhZ26mtYCHByr0WzzfTLm";
const string plainText = "BEGIN:VCARD\nVERSION:3.0\nFN:Riley  Griffin\nN:Griffin;Riley;;;\nORG:\nTITLE:\nTEL;TYPE=work,voice:\nEMAIL;TYPE=internet:\nURL:\nBDAY:2025-05-15\nEND:VCARD";
//const string plainText = "Testing  Double   Triple Space";


Griffinere griffinere = new(key);


string encrypted = griffinere.EncryptString(plainText);
string decrypted = griffinere.DecryptString(encrypted);

Console.WriteLine(encrypted);
Console.WriteLine(decrypted);






