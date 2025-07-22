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
        public void EncryptString_WithEmptyText_ShouldReturnEmptyString()
        {
            Griffinere cipher = new("VkiKMyvu7PT3UV08xZr9X1AA5WiZDzDm");
        
            string encrypted = cipher.EncryptString("");
        
            Assert.Equal("", encrypted);
        }
        
        [Fact]
        public void EncryptString_WithReallyLongKeyAndMessage_ShouldReturnOriginalText()
        {
            Griffinere cipher = new("LEWQcmPaCv9b8HNHJQFuqxDRDCJnQbcXmhQR3wwTuFhSPRUGBSJnj2GrTBSKj3tJTnnSrVC57DHhnik7EUVL8427EQRM6KHxJWenq1Jiy6qzRDchQt5B57izp744yZ0UtK5hngr9cq8kYDJnctwCc3TMk5awiw2HrhwyunyF3hEPk5bfhGmWZE61reeaC7SwH2iRZF9KYdHEwLQ8u1gV72KfPhMLvtca78ff4FcY7W5GeNZbMySUhU4GytTzU4PEHwtkQjRgcAqb7yxjaZT787t0wPZjTiyvdmVCreNm0C7exCFXpR6a4NC7QBQgimCaSWyj1cKZ9xTTML7Wrm6xZD0v5vHSiVKmN79tUpkPPD6TuV73RaTnPcHzqT8YpnujGtJ1jqvGVT6dRdLtbATth1wtLcmnMx5Mc0jLbp6hKicYjVEu7BJyv2mxYcaeyWQvXmj81zPEdnJ3wFz4ngXmT1XiRZwucAt2HMpxq3QaRaNGdA1y759dZqhueFbZn8G4");
        
            const string originalText = "ikdbr10dbLGm7xtMLkgVhBYVjmkrfAmARyNJXLLbUmvVSTnLMyFWw2vk4tZippWWJGJwhUq9dK6aD5FNJHyje4yzCTiMqjJ26wttnxSbgbNpXAuXKFUECNzDwFj5Dcf1JhqjeA9X6bfTBjY975jSYqrNNje1u1tBNTVjwq3qeMtWVFz9Bj2PxZhWuU99K1R8tedU48uRzjJWdvd18ZSVbwyrTMbGn77FPDAXQirbHiKwcwqXemMVq6tyec7Yc986KNVixV93Da4Z2jS3ERN66WHjhVwMm5yyb9KN81eiCNYWfJZdyp6mBAX2dNuNeBLQr4xP5LNdAFVWg2nn42t9aJNGh1Ep0yr1cGLBcYNXgMwPMqBtJnSLFphhi82zM3YhSeTLbSNchLzjJXu0A5ZhHqddPWc5BmnxtDeZ5tw6uTSy76au4MdTTqR3HcXeAVPuE9fxWSDwxEvh7gRCUBC3bkn7rdUtH8fRJFNLdyYNrNN2SM6C66rdHrhg71d6rGuGikdbr10dbLGm7xtMLkgVhBYVjmkrfAmARyNJXLLbUmvVSTnLMyFWw2vk4tZippWWJGJwhUq9dK6aD5FNJHyje4yzCTiMqjJ26wttnxSbgbNpXAuXKFUECNzDwFj5Dcf1JhqjeA9X6bfTBjY975jSYqrNNje1u1tBNTVjwq3qeMtWVFz9Bj2PxZhWuU99K1R8tedU48uRzjJWdvd18ZSVbwyrTMbGn77FPDAXQirbHiKwcwqXemMVq6tyec7Yc986KNVixV93Da4Z2jS3ERN66WHjhVwMm5yyb9KN81eiCNYWfJZdyp6mBAX2dNuNeBLQr4xP5LNdAFVWg2nn42t9aJNGh1Ep0yr1cGLBcYNXgMwPMqBtJnSLFphhi82zM3YhSeTLbSNchLzjJXu0A5ZhHqddPWc5BmnxtDeZ5tw6uTSy76au4MdTTqR3HcXeAVPuE9fxWSDwxEvh7gRCUBC3bkn7rdUtH8fRJFNLdyYNrNN2SM6C66rdHrhg71d6rGuG";
            string encrypted = cipher.EncryptString(originalText);
            string decrypted = cipher.DecryptString(encrypted);

            
            Assert.Equal(originalText, decrypted);
        }
        
        [Fact]
        public void EncryptString_WithReallyLongKeyAndCustomAlphabet_ShouldReturnOriginalText()
        {
            Griffinere cipher = new("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"#$%&'()*+,-/:;<=>?@[]^_`{|}~","a{D{BhT(e&V{4zzpQ=Mjw(Hv5epZt;#wf,A!nNTbeMbdA2x%?NwD3kJ@@$)]/*-q/5x3)/T=_JTzRY$4(ggH!d45CK9R8Vm+y&i8N_Ki+PZ4DA[Cj[fxZ02w%:MV");
        
            const string originalText = "Testing encryption.";
            string encrypted = cipher.EncryptString(originalText);
            string decrypted = cipher.DecryptString(encrypted);
            
            Assert.Equal(originalText, decrypted);
        }

		[Fact]
		public void EncryptString_WithEscapedCharacters_ShouldReturnOriginalText()
		{
			Griffinere cipher = new("EuMchtXtJFKhA5H8fGduYPXQEcZJKEAe");

			const string originalText = "Testing\nSpecial\tCharacters";
			string encrypted = cipher.EncryptString(originalText);
			string decrypted = cipher.DecryptString(encrypted);

			Assert.Equal(originalText, decrypted);
		}

		[Fact]
		public void EncryptString_WithDoubleSpaceCharacter_ShouldReturnOriginalText()
		{
			Griffinere cipher = new("dHiNt8C8JY1RhZ26mtYCHByr0WzzfTLm");

			const string originalText = "Testing  Double   Triple Space";
			string encrypted = cipher.EncryptString(originalText);
			string decrypted = cipher.DecryptString(encrypted);

			Assert.Equal(originalText, decrypted);
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


