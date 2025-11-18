using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("RAIDOU Remastered Save Converter v1.0 by RikuKH3");
        Console.WriteLine("------------------------------------------------");

        if (args.Length < 1)
        {
            Console.WriteLine("Usage: "+Path.GetFileName(Environment.GetCommandLineArgs()[0])+" <input file>");
            Console.ReadKey();
            return;
        }

        string inputFile = args[0];

        byte[] key = Encoding.UTF8.GetBytes("O7IbWIUSRrpp7BRCzX6sKZ_St862urrb");
        byte[] iv  = Encoding.UTF8.GetBytes("Df-5wkTM8hsVkVSDKNXIXwzQy_REkonb");

        byte[] fileBytes = File.ReadAllBytes(inputFile);

        bool isVER = false;
        if (fileBytes.Length >= 7)
        {
            byte[] sig = new byte[3];
            Buffer.BlockCopy(fileBytes, 0x4, sig, 0, 3);
            isVER = Encoding.UTF8.GetString(sig) == "VER";
        }

        byte[] result;

        if (isVER)
        {
            Console.WriteLine("Signature = VER → Encrypting...");
            result = Encrypt(fileBytes, key, iv);
            File.WriteAllBytes(inputFile + "_steam", result);
            Console.WriteLine("Encrypted file written to: " + inputFile + "_steam");
        }
        else
        {
            Console.WriteLine("Signature != VER → Decrypting...");
            result = Decrypt(fileBytes, key, iv);
            File.WriteAllBytes(inputFile + "_switch", result);
            Console.WriteLine("Decrypted file written to: " + inputFile + "_switch");
        }
    }

    static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
    {
        using (var rij = new RijndaelManaged())
        {
            rij.KeySize = 256;
            rij.BlockSize = 256;
            rij.Mode = CipherMode.CBC;
            rij.Padding = PaddingMode.PKCS7;
            rij.Key = key;
            rij.IV = iv;

            using (ICryptoTransform encryptor = rij.CreateEncryptor())
            {
                return encryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }

    static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        using (var rij = new RijndaelManaged())
        {
            rij.KeySize = 256;
            rij.BlockSize = 256;
            rij.Mode = CipherMode.CBC;
            rij.Padding = PaddingMode.PKCS7;
            rij.Key = key;
            rij.IV = iv;

            using (ICryptoTransform decryptor = rij.CreateDecryptor())
            {
                return decryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }
}
