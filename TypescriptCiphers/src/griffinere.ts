// src/griffinere.ts

export class Griffinere {
    private readonly alphabet: string[];
    private readonly alphabetPositionMap: Map<string, number>;
    private readonly key: string[];
    private readonly alphabetLength: number;

    constructor(key: string, alphabet?: string) {
        const defaultAlphabet = Griffinere.getDefaultAlphabet();
        const validatedAlphabet = Griffinere.validateAlphabet(alphabet ?? defaultAlphabet, key);

        this.alphabet = validatedAlphabet;
        this.alphabetPositionMap = Griffinere.createAlphabetPositionMap(validatedAlphabet);
        this.key = key.split('');
        this.alphabetLength = validatedAlphabet.length;
    }

    private static getDefaultAlphabet(): string {
        return 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    }

    private static validateAlphabet(alphabet: string, key: string): string[] {
        if (alphabet.includes('.')) {
            throw new Error("Alphabet must not contain '.'");
        }

        const seen = new Set<string>();
        const uniqueList: string[] = [];

        for (const c of alphabet) {
            if (seen.has(c)) {
                throw new Error(`Duplicate character '${c}' in provided alphabet.`);
            }
            seen.add(c);
            uniqueList.push(c);
        }

        for (const c of key) {
            if (!uniqueList.includes(c)) {
                throw new Error(`Alphabet does not contain the character '${c}' supplied in the key.`);
            }
        }

        return uniqueList;
    }

    private static createAlphabetPositionMap(alphabet: string[]): Map<string, number> {
        const map = new Map<string, number>();
        alphabet.forEach((char, index) => map.set(char, index));
        return map;
    }

    private getKey(text: string[]): string[] {
        if (text.length === 0) throw new Error('Text cannot be empty');

        const repeatedKey = [];
        while (repeatedKey.length < text.length) {
            repeatedKey.push(...this.key);
        }
        return repeatedKey.slice(0, text.length);
    }

    private shiftCharacterPositive(keyChar: string, textChar: string): string {
        const keyPos = this.alphabetPositionMap.get(keyChar);
        const textPos = this.alphabetPositionMap.get(textChar);

        if (keyPos === undefined || textPos === undefined) return textChar;

        const shiftedIndex = (keyPos + textPos) % this.alphabetLength;
        return this.alphabet[shiftedIndex];
    }

    private shiftCharacterNegative(keyChar: string, textChar: string): string {
        const keyPos = this.alphabetPositionMap.get(keyChar);
        const textPos = this.alphabetPositionMap.get(textChar);

        if (keyPos === undefined || textPos === undefined) return textChar;

        const shiftedIndex = (textPos - keyPos + this.alphabetLength) % this.alphabetLength;
        return this.alphabet[shiftedIndex];
    }

    private static toBase64CharArray(text: string): string[] {
        if (!text.trim()) throw new Error('Text cannot be empty');
        const base64 = Buffer.from(text, 'utf8').toString('base64').replace(/=/g, '');
        return base64.split('');
    }

    private static fromBase64CharArray(chars: string[]): string {
        if (chars.length === 0) throw new Error('Base64 input is empty');

        let base64 = chars.join('');
        while (base64.length % 4 !== 0) base64 += '=';

        return Buffer.from(base64, 'base64').toString('utf8');
    }

    public encryptString(plainText: string): string {
        if (!plainText.trim()) return '';

        const result: string[] = [];

        for (const segment of plainText.split(' ')) {
            if (!segment) {
                result.push(segment);
                continue;
            }

            const chars = Griffinere.toBase64CharArray(segment);
            const key = this.getKey(chars);
            const encrypted = chars.map((c, i) => this.shiftCharacterPositive(key[i], c));
            result.push(encrypted.join(''));
        }

        return result.join(' ');
    }

    public encryptStringWithMinimumLength(plainText: string, minimumLength: number): string {
        if (!plainText.trim()) return '';
        if (minimumLength < 1) throw new Error('Minimum response length must be greater than zero.');

        let front = '';
        let back = '';

        if (plainText.length < minimumLength) {
            const stripped = plainText.replace(/ /g, '').split('');
            const toAdd = minimumLength - plainText.length;
            const frontCount = Math.ceil(toAdd / 1.25);
            const backCount = toAdd - frontCount;

            for (let i = 0; i < frontCount; i++) {
                front += stripped[i % stripped.length] || '';
            }
            for (let i = 0; i < backCount; i++) {
                back += stripped[stripped.length - 1 - (i % stripped.length)] || '';
            }
        }

        const reversedFront = front.split('').reverse().join('');
        const frontEncrypted = front ? `${this.encryptString(reversedFront)}.` : '';
        const backEncrypted = back ? `.${this.encryptString(back)}` : '';
        const mainEncrypted = this.encryptString(plainText);

        return `${frontEncrypted}${mainEncrypted}${backEncrypted}`;
    }

    public decryptString(cipherText: string): string {
        if (!cipherText.trim()) return '';

        if (cipherText.includes('.')) {
            cipherText = cipherText.split('.')[1];
        }

        const result: string[] = [];

        for (const segment of cipherText.split(' ')) {
            if (!segment) {
                result.push('');
                continue;
            }

            const chars = segment.split('');
            const key = this.getKey(chars);
            const decrypted = chars.map((c, i) => this.shiftCharacterNegative(key[i], c));
            const plain = Griffinere.fromBase64CharArray(decrypted);
            result.push(plain);
        }

        return result.join(' ');
    }
}
