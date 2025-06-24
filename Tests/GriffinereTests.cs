using System;
using Xunit;
using Ciphers;

namespace Tests
{
    public class GriffinereTests
    {
        [Fact]
        public void EncryptAndDecrypt_ShouldReturnOriginalText()
        {
            const string key = "N3bhd1u6gh6Uh88H083envHwuUSec72i";
            const string plainText = "This is a test of the encryption.";
            Griffinere cipher = new(key);

            string encrypted = cipher.EncryptString(plainText);
            string decrypted = cipher.DecryptString(encrypted);

            Assert.Equal(plainText, decrypted);
        }

        [Fact]
        public void EncryptAndDecrypt_WithCustomAlphabet_ShouldReturnOriginalText()
        {
            const string key = "N3bhd1u6gh6Uh88H083envHwuUSec72i";
            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const string plainText = "Hello World 123!";
            Griffinere cipher = new(alphabet, key);
        
            string encrypted = cipher.EncryptString(plainText);
            string decrypted = cipher.DecryptString(encrypted);
        
            Assert.Equal(plainText, decrypted);
        }
        
        [Fact]
        public void EncryptString_WithMinimumLength_ShouldBeAtLeastThatLong()
        {
            const string key = "N3bhd1u6gh6Uh88H083envHwuUSec72i";
            const string plainText = "Short text";
            Griffinere cipher = new(key);
        
            string encrypted = cipher.EncryptString(plainText, 64);
        
            Assert.True(encrypted.Length >= 64);
        }
        
        [Fact]
        public void Constructor_WithInvalidAlphabet_ShouldThrow()
        {
            const string invalidAlphabet = "abc.defghijklmf"; // contains '.'
            const string key = "A39a3hiirMFAafY1iRBucZxY86AzCeMZ";
        
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new Griffinere(invalidAlphabet, key));
            Assert.Contains("must not contain '.'", ex.Message);
        }
        
        [Fact]
        public void Constructor_WithDuplicateAlphabetChars_ShouldThrow()
        {
            const string invalidAlphabet = "aabcdefg"; // duplicate 'a'
            const string key = "pseudorandom";
        
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new Griffinere(invalidAlphabet, key));
            Assert.Contains("Duplicate character", ex.Message);
        }
        
        [Fact]
        public void EncryptString_WithEmptyText_ShouldThrow()
        {
            Griffinere cipher = new("VkiKMyvu7PT3UV08xZr9X1AA5WiZDzDm");
        
            Assert.Throws<ArgumentNullException>(() => cipher.EncryptString(string.Empty));
        }
        
        [Fact]
        public void DecryptString_WithDotPrefix_ShouldStillReturnPlaintext()
        {
            const string key = "dShHPpUQTihcn7ju1wjYTAD1dvbrPKdT";
            const string plainText = ".Padding test case.";
            Griffinere cipher = new(key);
        
            string encrypted = cipher.EncryptString(plainText, 64);
            string decrypted = cipher.DecryptString(encrypted);
        
            Assert.Equal(plainText, decrypted);
        }
        
        [Fact]
        public void DecryptString_WithInvalidLength_ShouldThrow()
        {
            const string key = "dShHPpUQTihcn7ju1wjYTAD1dvbrPKdT";
            const string plainText = "Random plain text.";
            Griffinere cipher = new(key);
            
            Assert.Throws<ArgumentOutOfRangeException>(() => cipher.EncryptString(plainText, 0));
        }
    }
}


