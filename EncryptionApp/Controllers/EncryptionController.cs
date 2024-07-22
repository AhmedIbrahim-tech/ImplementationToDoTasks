using Microsoft.AspNetCore.Mvc;
using ImplementationToDoTasks.Models;
using System.Security.Cryptography;
using System.Text;
using TextCopy;

namespace ImplementationToDoTasks.Controllers;

public class EncryptionController : Controller
{
    private const string EncryptionKey = "E)H@McQfThWmZq4t7w!z%C*F-JaNdRgUkXn2r5u8x/A?D(G+KbPeShVmYq3s6v9y";

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Encrypt(EncryptionModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                model.Result = EncryptText(model.InputText);
                ClipboardService.SetText(model.Result);
                return Json(new { success = true, result = model.Result, message = "The result has been copied to the clipboard." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Invalid Input" });
            }
        }
        return Json(new { success = false, message = "Invalid input." });
    }

    [HttpPost]
    public IActionResult Decrypt(EncryptionModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                model.Result = DecryptText(model.InputText);
                ClipboardService.SetText(model.Result);
                return Json(new { success = true, result = model.Result, message = "The result has been copied to the clipboard." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Invalid Input" });
            }
        }
        return Json(new { success = false, message = "Invalid input." });
    }

    private string EncryptText(string plainText)
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

    private string DecryptText(string textHash)
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
}
