﻿import { describe, test, expect } from "vitest";
import { Griffinere } from "../src/griffinere";

// Helper long constants copied from the original C# tests
const DEFAULT_KEY = "N3bhd1u6gh6Uh88H083envHwuUSec72i";
const DEFAULT_ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

/**
 *  1. Basic round‑trip encryption / decryption
 */
describe("Griffinere – core functionality", () => {
    test("encrypts and decrypts a string symmetrically", () => {
        const cipher = new Griffinere(DEFAULT_KEY);
        const plaintext = "This is a test of the encryption.";

        const encrypted = cipher.encryptString(plaintext);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(plaintext);
    });

    test("encrypts and decrypts with a custom alphabet", () => {
        const cipher = new Griffinere(DEFAULT_KEY, DEFAULT_ALPHABET);
        const plaintext = "Hello World 123!";

        const encrypted = cipher.encryptString(plaintext);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(plaintext);
    });
});

/**
 *  2. Minimum‑length padding
 */
describe("Griffinere – minimum‑length handling", () => {
    test("encryptStringWithMinimumLength() result is at least the requested size", () => {
        const cipher = new Griffinere(DEFAULT_KEY);
        const plaintext = "Short text";

        const encrypted = cipher.encryptStringWithMinimumLength(plaintext, 64);
        expect(encrypted.length).toBeGreaterThanOrEqual(64);
    });
});

/**
 *  3. Edge‑case inputs
 */
describe("Griffinere – edge cases", () => {
    test("encrypting an empty string returns an empty string", () => {
        const cipher = new Griffinere("VkiKMyvu7PT3UV08xZr9X1AA5WiZDzDm");
        expect(cipher.encryptString("")).toBe("");
    });

    test("handles really long key and message", () => {
        const cipher = new Griffinere(
            "LEWQcmPaCv9b8HNHJQFuqxDRDCJnQbcXmhQR3wwTuFhSPRUGBSJnj2GrTBSKj3tJTnnSrVC57DHhnik7EUVL8427EQRM6KHxJWenq1Jiy6qzRDchQt5B57izp744yZ0UtK5hngr9cq8kYDJnctwCc3TMk5awiw2HrhwyunyF3hEPk5bfhGmWZE61reeaC7SwH2iRZF9KYdHEwLQ8u1gV72KfPhMLvtca78ff4FcY7W5GeNZbMySUhU4GytTzU4PEHwtkQjRgcAqb7yxjaZT787t0wPZjTiyvdmVCreNm0C7exCFXpR6a4NC7QBQgimCaSWyj1cKZ9xTTML7Wrm6xZD0v5vHSiVKmN79tUpkPPD6TuV73RaTnPcHzqT8YpnujGtJ1jqvGVT6dRdLtbATth1wtLcmnMx5Mc0jLbp6hKicYjVEu7BJyv2mxYcaeyWQvXmj81zPEdnJ3wFz4ngXmT1XiRZwucAt2HMpxq3QaRaNGdA1y759dZqhueFbZn8G4"
        );

        const originalText =
            "ikdbr10dbLGm7xtMLkgVhBYVjmkrfAmARyNJXLLbUmvVSTnLMyFWw2vk4tZippWWJGJwhUq9dK6aD5FNJHyje4yzCTiMqjJ26wttnxSbgbNpXAuXKFUECNzDwFj5Dcf1JhqjeA9X6bfTBjY975jSYqrNNje1u1tBNTVjwq3qeMtWVFz9Bj2PxZhWuU99K1R8tedU48uRzjJWdvd18ZSVbwyrTMbGn77FPDAXQirbHiKwcwqXemMVq6tyec7Yc986KNVixV93Da4Z2jS3ERN66WHjhVwMm5yyb9KN81eiCNYWfJZdyp6mBAX2dNuNeBLQr4xP5LNdAFVWg2nn42t9aJNGh1Ep0yr1cGLBcYNXgMwPMqBtJnSLFphhi82zM3YhSeTLbSNchLzjJXu0A5ZhHqddPWc5BmnxtDeZ5tw6uTSy76au4MdTTqR3HcXeAVPuE9fxWSDwxEvh7gRCUBC3bkn7rdUtH8fRJFNLdyYNrNN2SM6C66rdHrhg71d6rGuG";

        const encrypted = cipher.encryptString(originalText);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(originalText);
    });

    test("handles really long key with custom alphabet", () => {
        const cipher = new Griffinere(
            "a{D{BhT(e&V{4zzpQ=Mjw(Hv5epZt;#wf,A!nNTbeMbdA2x%?NwD3kJ@@$)]/*-q/5x3)/T=_JTzRY$4(ggH!d45CK9R8Vm+y&i8N_Ki+PZ4DA[Cj[fxZ02w%:MV",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!'#$%&\"()*+,-/:;<=>?@[]^_`{|}~"
        );

        const originalText = "Testing encryption.";
        const encrypted = cipher.encryptString(originalText);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(originalText);
    });

    test("round‑trips text containing escaped characters", () => {
        const cipher = new Griffinere("EuMchtXtJFKhA5H8fGduYPXQEcZJKEAe");
        const originalText = "Testing\nSpecial\tCharacters";

        const encrypted = cipher.encryptString(originalText);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(originalText);
    });

    test("round‑trips text containing multiple spaces", () => {
        const cipher = new Griffinere("dHiNt8C8JY1RhZ26mtYCHByr0WzzfTLm");
        const originalText = "Testing  Double   Triple Space";

        const encrypted = cipher.encryptString(originalText);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(originalText);
    });
});

/**
 *  4. Constructor validation
 */
describe("Griffinere – constructor validation", () => {
    test("throws when alphabet contains a dot", () => {
        expect(
            () => new Griffinere("A39a3hiirMFAafY1iRBucZxY86AzCeMZ", "abc.defghijklmf")
        ).toThrowError();
    });

    test("throws when alphabet contains duplicate characters", () => {
        expect(() => new Griffinere("aabcdefg", "pseudorandom")).toThrowError(/Duplicate character/);
    });
});

/**
 *  5. Behaviour with dot‑prefixed ciphertext & invalid min length
 */
describe("Griffinere – miscellaneous behaviour", () => {
    test("decrypts text even when encryption added dot‑segments", () => {
        const cipher = new Griffinere("dShHPpUQTihcn7ju1wjYTAD1dvbrPKdT");
        const originalText = ".Padding test case.";

        const encrypted = cipher.encryptStringWithMinimumLength(originalText, 64);
        const decrypted = cipher.decryptString(encrypted);

        expect(decrypted).toBe(originalText);
    });

    test("encryptStringWithMinimumLength() throws when minimumLength < 1", () => {
        const cipher = new Griffinere("dShHPpUQTihcn7ju1wjYTAD1dvbrPKdT");
        expect(() => cipher.encryptStringWithMinimumLength("Random plain text.", 0)).toThrow();
    });
});
