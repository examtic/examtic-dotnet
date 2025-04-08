using System.Text;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace Examtic;

public class ExamticService(IConfiguration configuration)
{
    public string GetLoginToken(string userId, bool autoCreate = false)
    {
        var secret = configuration["Examtic:Secret"];

        if (string.IsNullOrEmpty(secret))
        {
            throw new Exception("Secret missing in configuration.");
        }

        return GetLoginToken(userId, secret, autoCreate);
    }

    public static string GetLoginToken(string userId, string secret, bool autoCreate = false)
    {
        var json = new JsonObject
        {
            ["userId"] = userId,
            ["autoCreate"] = autoCreate
        };

        var aes = Aes.Create();
        var encryptor = aes.CreateEncryptor(Encoding.UTF8.GetBytes(secret), aes.IV);

        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(json.ToJsonString());
        }

        var iv = aes.IV;
        var encrypted = memoryStream.ToArray();

        return Convert.ToBase64String(iv.Concat(encrypted).ToArray());
    }
}

