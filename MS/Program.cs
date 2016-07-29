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
            const string nomeArquivo = "C:\\dell\\mega sena.txt";

            if (File.Exists(nomeArquivo))
            {
                var arquivo = File.ReadAllLines(nomeArquivo);

                foreach (var linha in arquivo)
                {
                    var vetor = linha.Split(' ');
                    var temp = new List<int>();

                    foreach (var item in vetor)
                    {                       
                        if (isValidNumber(item))
                            temp.Add(int.Parse(item));                        
                    }

                    if (temp.Count == 6)
                        Jogos.Add(new Jogo(temp));

                    Console.WriteLine();
                }

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Check if number is a valid name (between 1 and 60)
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
