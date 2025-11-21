using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("RAIDOU Remastered Save Converter v1.1 by RikuKH3");
        Console.WriteLine("------------------------------------------------");

        if (args.Length < 1)
        {
            Console.WriteLine("Usage: " + Path.GetFileName(Environment.GetCommandLineArgs()[0]) + " <input file>");
            Console.ReadKey();
            return;
        }

        string inputFile = args[0];

        byte[] key = Encoding.UTF8.GetBytes("O7IbWIUSRrpp7BRCzX6sKZ_St862urrb");
        byte[] iv = Encoding.UTF8.GetBytes("Df-5wkTM8hsVkVSDKNXIXwzQy_REkonb");

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
        return ProcessCipher(true, data, key, iv);
    }

    static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        return ProcessCipher(false, data, key, iv);
    }

    static byte[] ProcessCipher(bool encrypt, byte[] data, byte[] key, byte[] iv)
    {
        var engine = new RijndaelEngine(256);
        var blockCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
        var cipherParams = new ParametersWithIV(new KeyParameter(key), iv);
        blockCipher.Init(encrypt, cipherParams);
        return blockCipher.DoFinal(data);
    }
}
