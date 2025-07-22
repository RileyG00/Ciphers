import { describe, test, expect } from "vitest";
import { Griffinere } from "../src/griffinere";
describe('Griffinere', () => {
    test('encrypts and decrypts a string symmetrically', () => {
        const cipher = new Griffinere('KEY');
        const plaintext = 'Hello123';
        const encrypted = cipher.encryptString(plaintext);
        const decrypted = cipher.decryptString(encrypted);
        expect(decrypted).toBe(plaintext);
    });
});
