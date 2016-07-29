using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MS
{
    class Jogo
    {
        public IEnumerable<int> Numeros { get; set; }

        public Jogo(IEnumerable<int> numeros)
        {
            Numeros = numeros;
        }
    }

    class Site
    {
        public IEnumerable<string> Padroes { get; set; }
        public string Url { get; set; }
    }

    class Program
    {
        public static List<Jogo> Jogos;
        static void Main(string[] args)
        {
            Jogos = new List<Jogo>();
            const string FILE_NAME = "C:\\dell\\mega sena.txt";
            const int NUMBER_LENGTH = 6;

            if (File.Exists(FILE_NAME))
            {
                var arquivo = File.ReadAllLines(FILE_NAME);

                foreach (var linha in arquivo)
                {
                    var vetor = linha.Split(' ');
                    var tempNumbersSet = new List<int>();

                    foreach (var item in vetor)
                    {                       
                        if (isValidNumber(item))
                            tempNumbersSet.Add(int.Parse(item));                        
                    }

                    //Adding valid numbers set.
                    if (tempNumbersSet.Count == NUMBER_LENGTH)
                        Jogos.Add(new Jogo(tempNumbersSet));

                    Console.WriteLine();
                }

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Check if number is valid regarding Mega Sena pattern (between 1 and 60)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool isValidNumber(string number)
        {
            int saida;
            if (!int.TryParse(number, out saida))
                return false;
            else
                return (saida >= 1 && saida <= 60);
        }
    }
}
