using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class preSearch
    {

        public static Dictionary<string, double[]> diccionarioTfIDf;

        public static List<string[]> documentosGuardados;
        

        //metodo para obtener la ruta de cada txt
        public static string[] ObtenerRuta()
        {

            string[] arrayDeRutas = Directory.GetFiles(@"..\Content");

            return arrayDeRutas;

        }

        //metodo para normalizar los documentos

        public static string[] Normalizar(string aNormalizar, char[] separadores)
        {

            aNormalizar = aNormalizar.ToLower().Replace("\n", " ").Replace("\r", " ").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ü", "u");

            string[] normalizado = aNormalizar.Split(separadores, System.StringSplitOptions.RemoveEmptyEntries);

            return normalizado;

        }

        //metodo para leer los txt, normalizarlos y guardarlos

        public static List<string[]> GuardarTxt (string[] arrayDeRutas)
        {

            List<string[]> documentosGuardados = new List<string[]>();

            char[] separadoresTxt = { ' ', '.', ',', '!', '@', '#', '%', '^', '&', '*', '(', ')', '_', '-', '=', '+', ']', '[', '{', '}', ':', ';', '\'', '<', '>', '?', '/', '\\', '|', '`', '~', '"' };

            for (int i = 0; i < arrayDeRutas.Length; i++)
            {

                string stringCargado = File.ReadAllText(arrayDeRutas[i]);

                string[] normalizado = Normalizar(stringCargado, separadoresTxt);

                documentosGuardados.Add(normalizado);

            }

            return documentosGuardados;

        }

        //metodo para calcular tf*idf de los documentos

        public static  Dictionary<string , double[]> TfIdf (List<string[]> documentosGuardados , string[] arrayDeRutas)
        {
            
            Dictionary<string, double[]> diccionarioTfIdf = new Dictionary<string, double[]>();

            for (int i = 0; i < documentosGuardados.Count; i++)
            {
                for (int j = 0; j < documentosGuardados[i].Length; j++)
                {

                    if (!(diccionarioTfIdf.ContainsKey(documentosGuardados[i][j])))
                    {
                        double[] arrayTfIdf = new double[arrayDeRutas.Length + 1];

                        arrayTfIdf[i] = 1;

                        arrayTfIdf[arrayDeRutas.Length] = 1;

                        diccionarioTfIdf.Add(documentosGuardados[i][j], arrayTfIdf);

                    }

                    else
                    {

                        if (diccionarioTfIdf[documentosGuardados[i][j]][i] != 0)
                        {

                            diccionarioTfIdf[documentosGuardados[i][j]][i]++;

                        }

                        else
                        {

                            diccionarioTfIdf[documentosGuardados[i][j]][arrayDeRutas.Length]++;

                            diccionarioTfIdf[documentosGuardados[i][j]][i]++;

                        }

                    }

                }

            }


            foreach (string llave in diccionarioTfIdf.Keys)
            {

                diccionarioTfIdf[llave][arrayDeRutas.Length] = Math.Log10(documentosGuardados.Count / diccionarioTfIdf[llave][arrayDeRutas.Length]);

                for (int i = 0; i < documentosGuardados.Count; i++)
                {

                    diccionarioTfIdf[llave][i] = diccionarioTfIdf[llave][i] / documentosGuardados[i].Length;

                }
            }

            return diccionarioTfIdf;

        }

    }

}