    using System;
    using System.Security.Cryptography;

    public static class OtpUtility
    {
        public static string GenerateOtp(int length = 6)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").Substring(0, length);
            }
        }

        public static DateTime GetOtpExpiry(int minutes = 5) =>
            DateTime.UtcNow.AddMinutes(minutes);
    }
