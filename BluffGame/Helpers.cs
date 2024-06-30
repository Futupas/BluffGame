using System.Text;
using QRCoder;

namespace BluffGame;

public static class Helpers
{
    public static void ShuffleList<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Shared.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    public static string GenerateQRCode(string url)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        var base64Image = Convert.ToBase64String(qrCodeImage);
        return $"data:image/png;base64,{base64Image}";
    }

    public static T RandomChoose<T>(IReadOnlyList<T> list)
    {
        var index = Random.Shared.Next(list.Count);
        return list[index];
    }
    
    
    public static (T option1, T option2) RandomChoose2<T>(IReadOnlyList<T> list)
    {
        if (list.Distinct().Count() < 2) return (list[0], list[0]);
        
        var option1 = RandomChoose(list)!;
        var option2 = RandomChoose(list)!;
        while(option1.Equals(option2)) option2 = RandomChoose(list);

        return (option1, option2);
    }

    public static string GenerateShortId(int length = 5)
    {
        var chars = "abcdefghijkmnpqrstuvwxyz23456789".ToCharArray(); // excluding 'l', '1', 'i', '0', 'o' for clarity

        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            sb.Append(RandomChoose(chars));
        }
        return sb.ToString();
    }

}
