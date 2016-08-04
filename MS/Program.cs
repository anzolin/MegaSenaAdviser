using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MS
{
    enum TipoSequencia
    {
        Expressao = 1,
        Caracter = 2
    }

    class Jogo
    {
        public IEnumerable<int> Numeros { get; set; }

        public Jogo(IEnumerable<int> numeros)
        {
            Numeros = numeros;
        }
    }

    class Padrao
    {
        public TipoSequencia Tipo { get; set; }
        public string Sequencia { get; set; }
        public string Termino { get; set; }
        public string Break { get; set; }
    }

    public static class StringExtension
    {

        public static bool isValidNumber(this char number)
        {
            return isValidNumber(number.ToString());
        }

        /// <summary>
        /// Check if number is valid regarding Mega Sena pattern (between 1 and 60)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool isValidNumber(this string number)
        {
            int saida;
            if (!int.TryParse(number, out saida))
                return false;
            else
                return (saida >= 0 && saida <= 60);
        }

        public static string getOnlyNumbers(this string number)
        {
            var result = string.Empty;

            foreach (var item in number)
            {
                if (item.isValidNumber())
                    result += item;
            }

            return result;
        }
    }

    class Site
    {
        public IEnumerable<Padrao> Padroes { get; set; }
        public string Url { get; set; }

        public IEnumerable<string> getResultados()
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121";
            request.CookieContainer = new CookieContainer();

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var receiveStream = response.GetResponseStream())
                    {
                        using (var readStream = getNewStreamReader(response, receiveStream))
                        {
                            string data = readStream.ReadToEnd();
                            var matchesFound = new List<string>();
                            foreach (var padrao in Padroes)
                            {
                                switch (padrao.Tipo)
                                {
                                    case TipoSequencia.Expressao:
                                        var matches = Regex.Matches(data, padrao.Sequencia);
                                        foreach (var match in matches)
                                            matchesFound.Add(match.ToString());
                                        break;

                                    case TipoSequencia.Caracter:
                                        var indexOf = data.IndexOf(padrao.Sequencia);
                                        if (indexOf > 0)
                                        {
                                            var result = data.Substring(indexOf + padrao.Sequencia.Length, data.IndexOf(padrao.Termino, indexOf) - indexOf - padrao.Sequencia.Length);
                                            var res = Regex.Split(result, "<li>");
                                            var tempList = new List<int>();

                                            foreach (var item in res)
                                            {
                                                var itemWithoutTrash = item.getOnlyNumbers();

                                                if (!string.IsNullOrEmpty(itemWithoutTrash))
                                                    tempList.Add(int.Parse(itemWithoutTrash));
                                            }

                                            if (tempList.Count == 6)
                                            {
                                                result = string.Empty;
                                                tempList.ForEach(i => result += string.IsNullOrEmpty(result) ? i.ToString() : " " + i.ToString());
                                                matchesFound.Add(result);
                                            }                                                
                                        }

                                        break;

                                    default:
                                        throw new NotImplementedException();
                                }
                            }

                            if (matchesFound.Count > 0)
                                return matchesFound;
                        }
                    }
                }
            }

            return null;
        }

        #region Private Methods

        /// <summary>
        /// Construct a new stream for read Mega Sena informations.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        private StreamReader getNewStreamReader(HttpWebResponse response, Stream stream)
        {
            return response.CharacterSet == null ? new StreamReader(stream) : new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet));
        }
        #endregion

        class Program
        {
            public static List<Jogo> Jogos;
            static void Main(string[] args)
            {
                Jogos = new List<Jogo>();
                const string FILE_NAME = "C:\\dell\\mega sena.txt";
                const int NUMBER_LENGTH = 6;

                var site = new Site()
                {
                    Padroes = new List<Padrao>()
                    {
                        new Padrao
                        {
                            Tipo = TipoSequencia.Expressao,
                            Sequencia = "<ul class=\"numbers mega-sena\">(.*?)</ul>"
                        },
                        new Padrao
                        {
                            Tipo = TipoSequencia.Caracter,
                            Sequencia =  "<ul class=\"numbers mega-sena\">",
                            Termino = "</ul>",
                            Break = "<li>"
                        }
                    },
                    Url = "http://loterias.caixa.gov.br/wps/portal/loterias/landing/megasena/"
                };

                var result = site.getResultados();

                if (File.Exists(FILE_NAME))
                {
                    var arquivo = File.ReadAllLines(FILE_NAME);

                    foreach (var linha in arquivo)
                    {
                        var vetor = linha.Split(' ');
                        var tempNumbersSet = new List<int>();

                        foreach (var item in vetor)
                        {
                            if (item.isValidNumber())
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
        }
    }
}
