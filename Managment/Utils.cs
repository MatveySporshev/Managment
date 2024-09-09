namespace ProjectManagementSystem
{
    public static class Utils
    {
        private static int step = 4;

        public static string Encrypt(string text)
        {
            var newText = string.Empty;

            foreach (var charText in text)
            {
                newText += Convert.ToChar(charText + step);
            }

            return newText;
        }

        public static string Decrypt(string encryptedText)
        {
            var newText = string.Empty;

            foreach (var charText in encryptedText)
            {
                newText += Convert.ToChar(charText - step);
            }

            return newText;
        }
    }
}
