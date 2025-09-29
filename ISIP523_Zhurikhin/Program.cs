using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

List<TextStatistics> allStatistics = new List<TextStatistics>();
while (true)
{
Console.Clear();
Console.WriteLine("=== АНАЛИЗАТОР ТЕКСТА ===");
Console.WriteLine("1. Анализ нового текста");
Console.WriteLine("2. Просмотр статистики по прошлым текстам");
Console.WriteLine("3. Выход");
Console.Write("Выберите пункт меню: ");
string choice = Console.ReadLine();
switch (choice)
{
case "1":
AnalyzeNewText();
break;
case "2":
ShowPreviousStatistics();
break;
case "3":
Console.WriteLine("УЭЭЭЭЭЭЭЭЭЭЭЭЭ");
return;
default:
Console.WriteLine("Неверный выбор! Нажмите любую клавишу для продолжения...");
Console.ReadKey();
break;
}
}
void AnalyzeNewText()
{
Console.Clear();
Console.WriteLine("=== АНАЛИЗ НОВОГО ТЕКСТА ===");
Console.WriteLine("Введите текст:");
string bebebe = Console.ReadLine();
if (bebebe.Length < 100)
{
Console.WriteLine("В начале сотворил Бог небо и землю. Земля же была безвидна и пуста, и тьма над бездною, и Дух Божий носился над водою. И сказал Бог: да будет свет. И стал свет. И увидел Бог свет, что он хорош, и отделил Бог свет от тьмы. И назвал Бог свет днем, а тьму ночью. И был вечер, и было утро: день один. дальше я устал.");
}
TextStatistics currentStats = new TextStatistics();
int spaceCount = 0;
for (int i = 0; i < bebebe.Length; i++)
{
if (bebebe[i] == ' ')
{
spaceCount++;
}
}
currentStats.WordCount = spaceCount + 1;
string[] words = bebebe.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
string shortw = words.OrderBy(word => word.Length).First();
currentStats.ShortestWord = shortw;
currentStats.ShortestWordLength = shortw.Length;
string longw = words.OrderByDescending(word => word.Length).First();
currentStats.LongestWord = longw;
currentStats.LongestWordLength = longw.Length;
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
currentStats.SentenceCount = sentenceCount - 1;
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
currentStats.VowelCount = glasCount;
currentStats.ConsonantCount = soglasCount;
currentStats.TotalLetters = glasCount + soglasCount;
char[] allLetters = new char[33];
int[] frequencies = new int[33];
string russianAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
for (int i = 0; i < russianAlphabet.Length; i++)
{
allLetters[i] = russianAlphabet[i];
frequencies[i] = 0;
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
currentStats.LetterFrequencies = new int[33];
Array.Copy(frequencies, currentStats.LetterFrequencies, 33);
currentStats.Letters = new char[33];
Array.Copy(allLetters, currentStats.Letters, 33);
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
Console.WriteLine($"-------------------------------------");
Console.WriteLine($"Всего слов в тексте: {currentStats.WordCount}");
Console.WriteLine($"Самое короткое слово: '{currentStats.ShortestWord}' (длина: {currentStats.ShortestWordLength})");
Console.WriteLine($"Самое длинное слово: '{currentStats.LongestWord}' (длина: {currentStats.LongestWordLength})");
Console.WriteLine($"Всего предложений в тексте: {currentStats.SentenceCount}");
Console.WriteLine($"Гласных букв: {currentStats.VowelCount}");
Console.WriteLine($"Согласных букв: {currentStats.ConsonantCount}");
Console.WriteLine($"Всего букв: {currentStats.TotalLetters}");
Console.WriteLine("Статистика по частоте букв:");
int totalLetters = currentStats.TotalLetters;
for (int i = 0; i < allLetters.Length; i++)
{
if (frequencies[i] > 0)
{
double percentage = (double)frequencies[i] / totalLetters * 100;
Console.WriteLine($"Буква '{allLetters[i]}': {frequencies[i]} раз ({percentage:F1}%)");
}
}
allStatistics.Add(currentStats);
Console.WriteLine($"Статистика сохранена! Всего проанализировано текстов: {allStatistics.Count}");
Console.WriteLine("Нажмите любую клавишу для продолжения...");
Console.ReadKey();
}
void ShowPreviousStatistics()
{
Console.Clear();
Console.WriteLine("=== СТАТИСТИКА ПО ПРОШЛЫМ ТЕКСТАМ ===");
if (allStatistics.Count == 0)
{
Console.WriteLine("Статистика отсутствует. Сначала проанализируйте тексты.");
Console.WriteLine("Нажмите любую клавишу для продолжения...");
Console.ReadKey();
return;
}
Console.WriteLine($"Всего проанализировано текстов: {allStatistics.Count}");
for (int i = 0; i < allStatistics.Count; i++)
{
Console.WriteLine($"=== Текст #{i + 1} ===");
Console.WriteLine($"Предпросмотр: {allStatistics[i].TextPreview}");
Console.WriteLine($"Слов: {allStatistics[i].WordCount}");
Console.WriteLine($"Предложений: {allStatistics[i].SentenceCount}");
Console.WriteLine($"Самое короткое слово: '{allStatistics[i].ShortestWord}' ({allStatistics[i].ShortestWordLength} симв.)");
Console.WriteLine($"Самое длинное слово: '{allStatistics[i].LongestWord}' ({allStatistics[i].LongestWordLength} симв.)");
Console.WriteLine($"Букв: {allStatistics[i].TotalLetters} (гласных: {allStatistics[i].VowelCount}, согласных: {allStatistics[i].ConsonantCount})");
Console.Write("Самые частые буквы: ");
int count = 0;
for (int j = 0; j < allStatistics[i].Letters.Length && count < 3; j++)
{
if (allStatistics[i].LetterFrequencies[j] > 0)
{
Console.Write($"'{allStatistics[i].Letters[j]}'({allStatistics[i].LetterFrequencies[j]}) ");
count++;
}
}
Console.WriteLine("\n");
}

Console.WriteLine("Нажмите любую клавишу для продолжения...");
Console.ReadKey();
}
static bool IsInitial(string text)
{
if (text.Length <= 2 && text.EndsWith("."))
return true;
return false;
}
class TextStatistics
{
    public string TextPreview { get; set; } 
    public int WordCount { get; set; }
    public string ShortestWord { get; set; }
    public int ShortestWordLength { get; set; }
    public string LongestWord { get; set; } 
    public int LongestWordLength { get; set; }
    public int SentenceCount { get; set; }
    public int VowelCount { get; set; }
    public int ConsonantCount { get; set; }
    public int TotalLetters { get; set; }
    public char[] Letters { get; set; } = new char[33];
    public int[] LetterFrequencies { get; set; } = new int[33];
}