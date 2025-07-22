export declare class Griffinere {
    private readonly alphabet;
    private readonly alphabetPositionMap;
    private readonly key;
    private readonly alphabetLength;
    constructor(key: string, alphabet?: string);
    private static getDefaultAlphabet;
    private static validateAlphabet;
    private static createAlphabetPositionMap;
    private getKey;
    private shiftCharacterPositive;
    private shiftCharacterNegative;
    private static toBase64CharArray;
    private static fromBase64CharArray;
    encryptString(plainText: string): string;
    encryptStringWithMinimumLength(plainText: string, minimumLength: number): string;
    decryptString(cipherText: string): string;
}
