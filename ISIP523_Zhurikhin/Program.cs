using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
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
string longw = words.OrderByDescending(word => word.Length).First();
Console.WriteLine($"Самое длинное слово: '{longw}' (длина: {longw.Length})");
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
int glasCount = 0;
int soglasCount = 0;
string glas = "аеёиоуыэюя";
string soglas = "бвгджзйклмнпрстфхцчшщ";
foreach (char c in bebebe.ToLower())
{
    if (char.IsLetter(c))
    {
        if (glas.Contains(c))
        {
            glasCount++;
        }
        else if (soglas.Contains(c))
        {
            soglasCount++;
        }
    }
}
Console.WriteLine($"Гласных букв: {glasCount}");
Console.WriteLine($"Согласных букв: {soglasCount}");
Console.WriteLine($"Всего букв: {glasCount + soglasCount}");
Console.WriteLine("Статистика по частоте букв:");
char[] allLetters = new char[33];
int[] frequencies = new int[33];
int letterCount = 0;
string russianAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
for (int i = 0; i < russianAlphabet.Length; i++)
{
    allLetters[i] = russianAlphabet[i];
    frequencies[i] = 0;
    letterCount++;
}
foreach (char c in bebebe.ToLower())
{
    if (char.IsLetter(c) && russianAlphabet.Contains(c))
    {
        for (int i = 0; i < allLetters.Length; i++)
        {
            if (allLetters[i] == c)
            {
                frequencies[i]++;
                break;
            }
        }
    }
}
for (int i = 0; i < frequencies.Length - 1; i++)
{
    for (int j = 0; j < frequencies.Length - i - 1; j++)
    {
        if (frequencies[j] < frequencies[j + 1])
        {
            int tempFreq = frequencies[j];
            frequencies[j] = frequencies[j + 1];
            frequencies[j + 1] = tempFreq;
            char tempChar = allLetters[j];
            allLetters[j] = allLetters[j + 1];
            allLetters[j + 1] = tempChar;
        }
    }
}
int totalLetters = glasCount + soglasCount;
Console.WriteLine("Детальная статистика по буквам:");
for (int i = 0; i < allLetters.Length; i++)
{
    if (frequencies[i] > 0)
    {
        double percentage = (double)frequencies[i] / totalLetters * 100;
        Console.WriteLine($"Буква '{allLetters[i]}': {frequencies[i]} раз ({percentage:F1}%)");
    }
}
