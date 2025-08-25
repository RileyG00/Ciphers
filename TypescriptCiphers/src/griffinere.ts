export class Griffinere {
	private readonly alphabet: string[];
	private readonly alphabetPositionMap: Map<string, number>;
	private readonly key: string[];
	private readonly alphabetLength: number;

	constructor(key: string, alphabet?: string) {
		const defaultAlphabet: string = Griffinere.getDefaultAlphabet();
		const validatedAlphabet: string[] = Griffinere.validateAlphabet(
			alphabet ?? defaultAlphabet,
			key,
		);

		this.alphabet = validatedAlphabet;
		this.alphabetPositionMap =
			Griffinere.createAlphabetPositionMap(validatedAlphabet);
		this.key = Griffinere.validateKey(key);
		this.alphabetLength = validatedAlphabet.length;
	}

	private static getDefaultAlphabet(): string {
		return "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	}

	private static validateKey = (key: string): string[] => {
		if (key.length < 3) {
			throw new Error("Key must be at least 3 characters long.");
		} else {
			return key.split("");
		}
	};

	private static validateAlphabet = (
		alphabet: string,
		key: string,
	): string[] => {
		if (alphabet.includes(".")) {
			throw new Error("Alphabet must not contain '.'");
		}

		const seen: Set<string> = new Set();
		const uniqueList: string[] = [];

		for (const c of alphabet) {
			if (seen.has(c)) {
				throw new Error(
					`Duplicate character '${c}' in provided alphabet.`,
				);
			}
			seen.add(c);
			uniqueList.push(c);
		}

		if (uniqueList.length < 3) {
			throw new Error(
				"Alphabet must contain at least 3 unique characters.",
			);
		}

		for (const c of key) {
			if (!uniqueList.includes(c)) {
				throw new Error(
					`Alphabet does not contain the character '${c}' supplied in the key.`,
				);
			}
		}

		return uniqueList;
	};

	private static createAlphabetPositionMap(
		alphabet: string[],
	): Map<string, number> {
		const map: Map<string, number> = new Map();
		alphabet.forEach((char: string, index: number): void => {
			map.set(char, index);
		});
		return map;
	}

	private getKey(text: string[]): string[] {
		if (text.length === 0) throw new Error("Text cannot be empty");

		const repeatedKey: string[] = [];
		while (repeatedKey.length < text.length) {
			repeatedKey.push(...this.key);
		}
		return repeatedKey.slice(0, text.length);
	}

	private shiftCharacterPositive(keyChar: string, textChar: string): string {
		const keyPos: number | undefined =
			this.alphabetPositionMap.get(keyChar);
		const textPos: number | undefined =
			this.alphabetPositionMap.get(textChar);

		if (keyPos === undefined || textPos === undefined) return textChar; // shouldn't happen with alphabet-only input

		const shiftedIndex: number = (keyPos + textPos) % this.alphabetLength;
		return this.alphabet[shiftedIndex];
	}

	private shiftCharacterNegative(keyChar: string, textChar: string): string {
		const keyPos: number | undefined =
			this.alphabetPositionMap.get(keyChar);
		const textPos: number | undefined =
			this.alphabetPositionMap.get(textChar);

		if (keyPos === undefined || textPos === undefined) return textChar; // shouldn't happen with alphabet-only input

		const shiftedIndex: number =
			(textPos - keyPos + this.alphabetLength) % this.alphabetLength;
		return this.alphabet[shiftedIndex];
	}

	// -------------------------
	// Alphabet-only Base-N codec
	// -------------------------

	private static utf8ToBytes(text: string): Uint8Array {
		return new TextEncoder().encode(text);
	}

	private static bytesToUtf8(bytes: Uint8Array): string {
		return new TextDecoder().decode(bytes);
	}

	/**
	 * Encode arbitrary bytes into the configured alphabet (base-N).
	 * Preserves leading zero bytes by prefixing alphabet[0] for each leading zero.
	 */
	private encodeBytesToAlphabet(bytes: Uint8Array): string {
		if (bytes.length === 0) return "";

		// Count leading zero bytes
		let zeroCount = 0;
		while (zeroCount < bytes.length && bytes[zeroCount] === 0) zeroCount++;

		// Convert bytes (big-endian) to BigInt
		let value = 0n;
		for (const b of bytes) {
			value = (value << 8n) + BigInt(b);
		}

		const base = BigInt(this.alphabetLength);

		if (value === 0n) {
			// All zeros: represent at least one zero digit
			return this.alphabet[0].repeat(Math.max(1, zeroCount));
		}

		const digits: string[] = [];
		while (value > 0n) {
			const rem = Number(value % base);
			digits.push(this.alphabet[rem]);
			value = value / base;
		}

		// Add prefix zeros and reverse to big-endian digit order
		const zeroPrefix = this.alphabet[0].repeat(zeroCount);
		return zeroPrefix + digits.reverse().join("");
	}

	/**
	 * Decode text composed only of alphabet characters back to bytes.
	 * Restores leading zeros encoded as prefix alphabet[0].
	 */
	private decodeAlphabetToBytes(text: string): Uint8Array {
		if (text.length === 0) return new Uint8Array(0);

		// Count prefix zeros (alphabet[0])
		let zeroPrefix = 0;
		while (
			zeroPrefix < text.length &&
			text[zeroPrefix] === this.alphabet[0]
		) {
			zeroPrefix++;
		}

		const base = BigInt(this.alphabetLength);
		let value = 0n;
		for (const c of text) {
			const digit = this.alphabetPositionMap.get(c);
			if (digit === undefined) {
				throw new Error(
					`Cipher text contains character '${c}' not in the alphabet.`,
				);
			}
			value = value * base + BigInt(digit);
		}

		// Convert BigInt to big-endian bytes
		const bytesRev: number[] = [];
		while (value > 0n) {
			bytesRev.push(Number(value & 0xffn));
			value >>= 8n;
		}
		const core = Uint8Array.from(bytesRev.reverse());

		// Restore encoded leading zeros
		if (zeroPrefix > 0) {
			const out = new Uint8Array(zeroPrefix + core.length);
			// first zeroPrefix bytes are already zero
			out.set(core, zeroPrefix);
			return out;
		}

		return core;
	}

	private toAlphabetCharArray(text: string): string[] {
		if (!text.trim()) throw new Error("Text cannot be empty");
		const bytes: Uint8Array = Griffinere.utf8ToBytes(text);
		const encoded: string = this.encodeBytesToAlphabet(bytes);
		return encoded.split("");
	}

	private fromAlphabetCharArray(chars: string[]): string {
		if (chars.length === 0) throw new Error("Encoded input is empty");
		const text: string = chars.join("");
		const bytes: Uint8Array = this.decodeAlphabetToBytes(text);
		return Griffinere.bytesToUtf8(bytes);
	}

	// --- Public API ---

	public encryptString(plainText: string): string {
		if (!plainText.trim()) return "";

		const result: string[] = [];

		for (const segment of plainText.split(" ")) {
			if (!segment) {
				result.push(segment);
				continue;
			}

			const chars: string[] = this.toAlphabetCharArray(segment);
			const key: string[] = this.getKey(chars);
			const encrypted: string[] = chars.map(
				(c: string, i: number): string =>
					this.shiftCharacterPositive(key[i], c),
			);
			result.push(encrypted.join(""));
		}

		return result.join(" ");
	}

	public encryptStringWithMinimumLength(
		plainText: string,
		minimumLength: number,
	): string {
		if (!plainText.trim()) return "";
		if (minimumLength < 1)
			throw new Error(
				"Minimum response length must be greater than zero.",
			);

		let front = "";
		let back = "";

		if (plainText.length < minimumLength) {
			const stripped: string[] = plainText.replace(/ /g, "").split("");
			const toAdd: number = minimumLength - plainText.length;
			const frontCount: number = Math.ceil(toAdd / 1.25);
			const backCount: number = toAdd - frontCount;

			for (let i = 0; i < frontCount; i++) {
				front += stripped[i % stripped.length] ?? "";
			}
			for (let i = 0; i < backCount; i++) {
				back +=
					stripped[stripped.length - 1 - (i % stripped.length)] ?? "";
			}
		}

		const reversedFront: string = front.split("").reverse().join("");
		const frontEncrypted: string = front
			? `${this.encryptString(reversedFront)}.`
			: "";
		const backEncrypted: string = back
			? `.${this.encryptString(back)}`
			: "";
		const mainEncrypted: string = this.encryptString(plainText);

		return `${frontEncrypted}${mainEncrypted}${backEncrypted}`;
	}

	public decryptString(cipherText: string): string {
		if (!cipherText.trim()) return "";

		if (cipherText.includes(".")) {
			cipherText = cipherText.split(".")[1];
		}

		const result: string[] = [];

		for (const segment of cipherText.split(" ")) {
			if (!segment) {
				result.push("");
				continue;
			}

			const chars: string[] = segment.split("");
			const key: string[] = this.getKey(chars);
			const decrypted: string[] = chars.map(
				(c: string, i: number): string =>
					this.shiftCharacterNegative(key[i], c),
			);
			const plain: string = this.fromAlphabetCharArray(decrypted);
			result.push(plain);
		}

		return result.join(" ");
	}
}
