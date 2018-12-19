using System;

namespace EncryctionDES
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Console.WriteLine("Enter text: ");
            string text = Console.ReadLine();
            Console.WriteLine("Enter key: ");
            string key = Console.ReadLine();

            DES encryptor = new DES(text, key);


            string encryptedText = encryptor.Encryct();
            Console.WriteLine(encryptedText);
            Console.ReadKey();
        }
    }
}
