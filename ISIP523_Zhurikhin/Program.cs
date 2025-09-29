using System.Runtime.InteropServices;

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
Console.WriteLine($"Всего слов в тексте: {a+1}");