// Griffinere cipher – browser‑safe implementation
// Removed all Node‑only features (no Buffer import)
// Uses Web APIs: TextEncoder/TextDecoder and atob/btoa

export class Griffinere {
    private readonly alphabet: string[];
    private readonly alphabetPositionMap: Map<string, number>;
    private readonly key: string[];
    private readonly alphabetLength: number;

    constructor(key: string, alphabet?: string) {
        const defaultAlphabet: string = Griffinere.getDefaultAlphabet();
        const validatedAlphabet: string[] = Griffinere.validateAlphabet(alphabet ?? defaultAlphabet, key);

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

        const seen: Set<string> = new Set();
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
        const map: Map<string, number> = new Map();
        alphabet.forEach((char: string, index: number): void => {
            map.set(char, index);
        });
        return map;
    }

    private getKey(text: string[]): string[] {
        if (text.length === 0) throw new Error('Text cannot be empty');

        const repeatedKey: string[] = [];
        while (repeatedKey.length < text.length) {
            repeatedKey.push(...this.key);
        }
        return repeatedKey.slice(0, text.length);
    }

    private shiftCharacterPositive(keyChar: string, textChar: string): string {
        const keyPos: number | undefined = this.alphabetPositionMap.get(keyChar);
        const textPos: number | undefined = this.alphabetPositionMap.get(textChar);

        if (keyPos === undefined || textPos === undefined) return textChar;

        const shiftedIndex: number = (keyPos + textPos) % this.alphabetLength;
        return this.alphabet[shiftedIndex];
    }

    private shiftCharacterNegative(keyChar: string, textChar: string): string {
        const keyPos: number | undefined = this.alphabetPositionMap.get(keyChar);
        const textPos: number | undefined = this.alphabetPositionMap.get(textChar);

        if (keyPos === undefined || textPos === undefined) return textChar;

        const shiftedIndex: number = (textPos - keyPos + this.alphabetLength) % this.alphabetLength;
        return this.alphabet[shiftedIndex];
    }

    // --- Base64 helpers using Web APIs (TextEncoder/TextDecoder, atob/btoa) ---

    private static encodeBase64(text: string): string {
        const bytes: Uint8Array = new TextEncoder().encode(text);
        let binary: string = '';
        bytes.forEach((b: number): void => { binary += String.fromCharCode(b); });
        return btoa(binary);
    }

    private static decodeBase64(base64: string): string {
        const binary: string = atob(base64);
        const bytes: Uint8Array = Uint8Array.from(binary, (c: string): number => c.charCodeAt(0));
        return new TextDecoder().decode(bytes);
    }

    private static toBase64CharArray(text: string): string[] {
        if (!text.trim()) throw new Error('Text cannot be empty');
        const base64: string = Griffinere.encodeBase64(text).replace(/=/g, '');
        return base64.split('');
    }

    private static fromBase64CharArray(chars: string[]): string {
        if (chars.length === 0) throw new Error('Base64 input is empty');

        let base64: string = chars.join('');
        while (base64.length % 4 !== 0) base64 += '='; // restore padding

        return Griffinere.decodeBase64(base64);
    }

    // --- Public API ---

    public encryptString(plainText: string): string {
        if (!plainText.trim()) return '';

        const result: string[] = [];

        for (const segment of plainText.split(' ')) {
            if (!segment) {
                result.push(segment);
                continue;
            }

            const chars: string[] = Griffinere.toBase64CharArray(segment);
            const key: string[] = this.getKey(chars);
            const encrypted: string[] = chars.map((c: string, i: number): string => this.shiftCharacterPositive(key[i], c));
            result.push(encrypted.join(''));
        }

        return result.join(' ');
    }

    public encryptStringWithMinimumLength(plainText: string, minimumLength: number): string {
        if (!plainText.trim()) return '';
        if (minimumLength < 1) throw new Error('Minimum response length must be greater than zero.');

        let front: string = '';
        let back: string = '';

        if (plainText.length < minimumLength) {
            const stripped: string[] = plainText.replace(/ /g, '').split('');
            const toAdd: number = minimumLength - plainText.length;
            const frontCount: number = Math.ceil(toAdd / 1.25);
            const backCount: number = toAdd - frontCount;

            for (let i = 0; i < frontCount; i++) {
                front += stripped[i % stripped.length] ?? '';
            }
            for (let i = 0; i < backCount; i++) {
                back += stripped[stripped.length - 1 - (i % stripped.length)] ?? '';
            }
        }

        const reversedFront: string = front.split('').reverse().join('');
        const frontEncrypted: string = front ? `${this.encryptString(reversedFront)}.` : '';
        const backEncrypted: string = back ? `.${this.encryptString(back)}` : '';
        const mainEncrypted: string = this.encryptString(plainText);

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

            const chars: string[] = segment.split('');
            const key: string[] = this.getKey(chars);
            const decrypted: string[] = chars.map((c: string, i: number): string => this.shiftCharacterNegative(key[i], c));
            const plain: string = Griffinere.fromBase64CharArray(decrypted);
            result.push(plain);
        }

        return result.join(' ');
    }
}
