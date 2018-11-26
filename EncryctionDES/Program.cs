using System;

namespace EncryctionDES
{
    class Program
    {
        

        static void Main(string[] args)
        {

            string text = "Член";
            string key = "1234567";

            DES encryptor = new DES(text, key);


            string encryptedText = encryptor.Encryct();
            Console.WriteLine(encryptedText);
            Console.ReadKey();
        }
    }
}
