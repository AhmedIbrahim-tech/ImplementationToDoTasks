using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TextCopy;

const string EncryptionKey = "E)H@McQfThWmZq4t7w!z%C*F-JaNdRgUkXn2r5u8x/A?D(G+KbPeShVmYq3s6v9y";

bool continueProgram = true;

while (continueProgram)
{
    Console.WriteLine("Enter '1' for encrypt or '2' for decrypt:");
    char choice = Console.ReadKey().KeyChar;
    Console.WriteLine();

    Console.WriteLine("\n");

    Console.WriteLine("Enter the text:");
    string inputText = Console.ReadLine();

    Console.WriteLine("\n");

    string result = string.Empty;

    if (choice == '1')
    {
        result = Encrypt(inputText);
        Console.WriteLine($"Encrypted Text: {result}");
    }
    else if (choice == '2')
    {
        result = Decrypt(inputText);
        Console.WriteLine($"Decrypted Text: {result}");
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }

    if (!string.IsNullOrEmpty(result))
    {
        Console.WriteLine("Enter 'c' to copy the result to clipboard:");
        char copyChoice = Console.ReadKey().KeyChar;
        Console.WriteLine();

        if (copyChoice == 'c')
        {
            ClipboardService.SetText(result);
            Console.WriteLine("The result has been copied to the clipboard.");
        }
    }

    Console.WriteLine("Do you want to perform another operation? (y/n):");
    char continueChoice = Console.ReadKey().KeyChar;
    Console.WriteLine();

    if (continueChoice != 'y')
    {
        continueProgram = false;
    }
}

#region Hash
string Encrypt(string plainText)
{
    byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
    using (Aes encryptor = Aes.Create())
    {
        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }
            plainText = Convert.ToBase64String(ms.ToArray());
        }
    }
    return plainText;
}

string Decrypt(string textHash)
{
    textHash = textHash.Replace(" ", "+");
    byte[] cipherBytes = Convert.FromBase64String(textHash);
    using (Aes encryptor = Aes.Create())
    {
        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
            }
            textHash = Encoding.Unicode.GetString(ms.ToArray());
        }
    }
    return textHash;
}

string Hash(string plainText)
{
    byte[] salt;
    byte[] buffer2;
    if (string.IsNullOrEmpty(plainText))
    {
        throw new ArgumentNullException("password");
    }
    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(plainText, 0x10, 0x3e8))
    {
        salt = bytes.Salt;
        buffer2 = bytes.GetBytes(0x20);
    }
    byte[] dst = new byte[0x31];
    Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
    Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
    return Convert.ToBase64String(dst);
}

bool IsMatch(string plainText, string textHash)
{
    byte[] buffer4;
    if (textHash == null)
    {
        return false;
    }
    if (plainText == null)
    {
        throw new ArgumentNullException("providedPassword");
    }
    byte[] src = Convert.FromBase64String(textHash);
    if ((src.Length != 0x31) || (src[0] != 0))
    {
        return false;
    }
    byte[] dst = new byte[0x10];
    Buffer.BlockCopy(src, 1, dst, 0, 0x10);
    byte[] buffer3 = new byte[0x20];
    Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(plainText, dst, 0x3e8))
    {
        buffer4 = bytes.GetBytes(0x20);
    }
    return ByteArraysEqual(buffer3, buffer4);
}

[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
bool ByteArraysEqual(byte[] a, byte[] b)
{
    if (ReferenceEquals(a, b))
    {
        return true;
    }

    if (a == null || b == null || a.Length != b.Length)
    {
        return false;
    }

    bool areSame = true;
    for (int i = 0; i < a.Length; i++)
    {
        areSame &= (a[i] == b[i]);
    }
    return areSame;
}

#endregion