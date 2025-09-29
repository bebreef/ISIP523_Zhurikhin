using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

Console.WriteLine("Введите текст. (если там будет менее 100 символов...)");
string bebebe;
bebebe = Console.ReadLine();
if (bebebe.Length < 100)
{
    Console.WriteLine("В начале сотворил Бог небо и землю. Земля же была безвидна и пуста, и тьма над бездною, и Дух Божий носился над водою. И сказал Бог: да будет свет. И стал свет. И увидел Бог свет, что он хорош, и отделил Бог свет от тьмы. И назвал Бог свет днем, а тьму ночью. И был вечер, и было утро: день один. дальше я устал.");
}
int a = 0;
for (int i = 0; i<bebebe.Length; i++)
{
    if (bebebe[i] == ' ')
    {
        a++;
    }
}
Console.WriteLine($"-------------------------------------");
Console.WriteLine($"Всего слов в тексте: {a+1}");
string[] words = bebebe.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
string shortw = words.OrderBy(word => word.Length).First();
Console.WriteLine($"Самое короткое слово: '{shortw}' (длина: {shortw.Length})");
string[] sentences = bebebe.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
int sentenceCount = 0;
foreach (string sentence in sentences)
{
    string trimmed = sentence.Trim();
    if (!string.IsNullOrEmpty(trimmed) && !IsInitial(trimmed))
    {
        sentenceCount++;
    }
}
Console.WriteLine($"Всего предложений в тексте: {sentenceCount-1}");
static bool IsInitial(string text)
{
    if (text.Length <= 2 && text.EndsWith("."))
        return true;
    return false;
}