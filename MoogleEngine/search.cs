namespace MoogleEngine
{
    public static class search
    {

        public static Dictionary<string, double[]> diccionarioTfIdf = preSearch.TfIdf(preSearch.GuardarTxt(preSearch.ObtenerRuta()), preSearch.ObtenerRuta());

        // metodo para obtener la query y normalizarla excpeto los operadores 
        public static string[] ObtenerQuery(string query)
        {
            char[] separadoresQuery = { ' ', '.', ',', '@', '#', '%', '&', '(', ')', '_', '-', '=', '+', ']', '[', '{', '}', ':', ';', '\'', '<', '>', '?', '/', '\\', '|', '`', '"' };

            string[] queryNormalizado = preSearch.Normalizar(query, separadoresQuery);

            return queryNormalizado;

        }

        //metodo para eliminar los operadores y terminar de normalizar la query

        public static List<string> GuardarOperadores(string[] queryNormalizado)
        {

            List<string> listaDeOperadores = new List<string>();

            int contadorDeOperadorDeCercania = 0;

            for (int i = 0; i < queryNormalizado.Length; i++)
            {

                if (queryNormalizado[i][0] == '!' || queryNormalizado[i][0] == '^')
                {

                    listaDeOperadores.Add(queryNormalizado[i]);

                    queryNormalizado[i] = queryNormalizado[i].Substring(1);

                }

                if (queryNormalizado[i][0] == '*')
                {

                    listaDeOperadores.Add(queryNormalizado[i]);

                    int contador = 0;

                    for (int j = 0; j < queryNormalizado[i].Length; j++)
                    {

                        if (queryNormalizado[i][j] == '*')
                        {

                            contador++;

                        }

                    }

                    queryNormalizado[i] = queryNormalizado[i].Substring(contador);

                }

                if (queryNormalizado[i] == "~")
                {

                    listaDeOperadores.Add(queryNormalizado[i - 1] + " " + queryNormalizado[i + 1]);
                    contadorDeOperadorDeCercania++;

                }
            }

            List<string> listaAuxiliarParaConvertir = queryNormalizado.ToList();

            for (int i = 0; i < contadorDeOperadorDeCercania; i++)
            {

                listaAuxiliarParaConvertir.Remove("~");

            }

            queryNormalizado = listaAuxiliarParaConvertir.ToArray();

            return listaDeOperadores;

        }

        //metodo para calcular el tf de la query

        public static double TfQuery(string[] queryNormalizado, string aNormalizar)
        {
            double contador = 0;

            for (int i = 0; i < queryNormalizado.Length; i++)
            {
                if (aNormalizar == queryNormalizado[i])
                {

                    contador++;

                }

            }

            return contador / queryNormalizado.Length;
        }

        //metodo para guardar la query sin repetir ninguna palabra

        public static List<string> querySinRepetir(string[] queryNormalizado)
        {

            List<string> querySinRepetir = new List<string>();

            for (int i = 0; i < queryNormalizado.Length; i++)
            {

                if (!(querySinRepetir.Contains(queryNormalizado[i])))
                {

                    querySinRepetir.Add(queryNormalizado[i]);

                }

            }
            return querySinRepetir;

        }

        //metodo para crear el vector query (tf*idf)

        public static double[] queryTfidf(Dictionary<string, double[]> diccionarioTfIdf, List<string> querySinRepetir, string[] queryNormalizado, string[] arrayDeRutas)
        {


            double[] queryTfIDf = new double[querySinRepetir.Count];

            for (int i = 0; i < querySinRepetir.Count; i++)
            {
                if (diccionarioTfIdf.ContainsKey(querySinRepetir[i]))
                {

                    queryTfIDf[i] = TfQuery(queryNormalizado, querySinRepetir[i]) * diccionarioTfIdf[querySinRepetir[i]][arrayDeRutas.Length];

                }

                else
                {

                    queryTfIDf[i] = 0;

                }

            }

            return queryTfIDf;

        }

        //metodo para el operador de importancia

        public static void OperadorDeImportancia(List<string> listaDeOperadores, List<string> querySinRepetir, double[] queryTfIdf)
        {

            for (int i = 0; i < listaDeOperadores.Count; i++)
            {

                if (listaDeOperadores[i][0] == '*')
                {

                    int contador = 0;

                    for (int j = 0; j < listaDeOperadores[i].Length; j++)
                    {

                        if (listaDeOperadores[i][j] == '*')
                        {

                            contador++;

                        }

                    }

                    string palabraSinOperadorDeImportancia = listaDeOperadores[i].Substring(contador);

                    int posicion = 0;

                    for (int k = 0; k < querySinRepetir.Count; k++)
                    {

                        if (palabraSinOperadorDeImportancia == querySinRepetir[k])
                        {

                            posicion = k;

                        }

                    }

                    queryTfIdf[posicion] = (1 + contador) * queryTfIdf[posicion];

                }

            }

        }

        //metodo apra calcular la magnitud de la query

        public static double magnitudDeLaQuery(List<string> querySinRepetir, double[] queryTfIDf)
        {

            double magnitudDeLaQuery = 0;

            for (int i = 0; i < querySinRepetir.Count; i++)
            {
                magnitudDeLaQuery += Math.Pow(queryTfIDf[i], 2);

            }

            return magnitudDeLaQuery = Math.Sqrt(magnitudDeLaQuery);

        }

        //metodo para calcular la similitud de coseno

        public static Dictionary<string, double> diccionarioDeSimilitudDeCoseno(List<string[]> documentosGuardados, List<string> querySinRepetir, double[] queryTfIDf, Dictionary<string, double[]> diccionarioTfIdf, double magnitudDeLaQuery, string[] arrayDeRutas)
        {

            Dictionary<string, double> diccionarioSimilitudDeCoseno = new Dictionary<string, double>();


            //ciclo para crear el vector documento y multiplicarlo con el vector query 

            for (int i = 0; i < documentosGuardados.Count; i++)
            {

                double[] documentoTfIdf = new double[queryTfIDf.Length];

                for (int j = 0; j < querySinRepetir.Count; j++)
                {

                    if (diccionarioTfIdf.ContainsKey(querySinRepetir[j]))
                    {

                        documentoTfIdf[j] = diccionarioTfIdf[querySinRepetir[j]][i] * diccionarioTfIdf[querySinRepetir[j]][arrayDeRutas.Length]; //vector documento

                    }

                    else
                    {

                        documentoTfIdf[j] = 0;

                    }

                }

                double sumaPunto = 0;

                for (int k = 0; k < queryTfIDf.Length; k++)
                {

                    sumaPunto += queryTfIDf[k] * documentoTfIdf[k];  //sumapunto

                }

                //magnitud del documento

                double magnitudDelDocumento = 0;

                foreach (string palabraBuscada in diccionarioTfIdf.Keys)
                {

                    magnitudDelDocumento += Math.Pow(diccionarioTfIdf[palabraBuscada][i] * diccionarioTfIdf[palabraBuscada][arrayDeRutas.Length], 2);

                }

                magnitudDelDocumento = Math.Sqrt(magnitudDelDocumento);

                // formula de similitud de coseno

                double similitudDeCos = sumaPunto / (magnitudDeLaQuery * magnitudDelDocumento);

                diccionarioSimilitudDeCoseno.Add(arrayDeRutas[i], similitudDeCos);

            }

            return diccionarioSimilitudDeCoseno;

        }

        //metodo para ordenar la similitud de coseno

        public static List<string> similitudDeCosenoOrdenado(Dictionary<string, double> diccionarioSimilitudDeCoseno)
        {

            List<string> similitudDeCosenoOrdenado = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                string rutaGuardada = " ";

                double valorDerutaGuardada = 0;

                foreach (string ruta in diccionarioSimilitudDeCoseno.Keys)
                {
                    if (diccionarioSimilitudDeCoseno[ruta] > valorDerutaGuardada)
                    {

                        valorDerutaGuardada = diccionarioSimilitudDeCoseno[ruta];

                        rutaGuardada = ruta;

                    }

                }

                if (valorDerutaGuardada != 0)
                {

                    similitudDeCosenoOrdenado.Add(rutaGuardada);

                    diccionarioSimilitudDeCoseno.Remove(rutaGuardada);
                }

            }

            return similitudDeCosenoOrdenado;
        }

        //metodo para trabajar con los operadores de aparece, no aparece y cercania

        public static void Operadores(List<string> listaDeOperadores, string[] arrayDeRutas, Dictionary<string, double[]> diccionarioTfIDf, Dictionary<string, double> diccionarioSimilitudDeCoseno, List<string[]> documentosGuardados)
        {

            for (int i = 0; i < listaDeOperadores.Count; i++)
            {

                //operador de no aparece

                if (listaDeOperadores[i][0] == '!')
                {

                    string palabraSubString = listaDeOperadores[i].Substring(1);

                    for (int j = 0; j < arrayDeRutas.Length; j++)
                    {

                        if (diccionarioTfIdf[palabraSubString][j] != 0)
                        {

                            diccionarioSimilitudDeCoseno.Remove(arrayDeRutas[j]);

                        }

                    }

                }


                //operador de aparece

                if (listaDeOperadores[i][0] == '^')
                {

                    string palabraSubString = listaDeOperadores[i].Substring(1);

                    for (int j = 0; j < arrayDeRutas.Length; j++)
                    {

                        if (diccionarioTfIdf[palabraSubString][j] == 0)
                        {

                            diccionarioSimilitudDeCoseno.Remove(arrayDeRutas[j]);

                        }
                    }

                }


                //operador de cercania

                if (listaDeOperadores[i][0] != '!' && listaDeOperadores[i][0] != '^' && listaDeOperadores[i][0] != '*')
                {

                    string[] arraydeCercania = listaDeOperadores[i].Split(' ');

                    if (diccionarioTfIdf.ContainsKey(arraydeCercania[0]) && diccionarioTfIdf.ContainsKey(arraydeCercania[1]))
                    {

                        for (int j = 0; j < arrayDeRutas.Length; j++)
                        {

                            if (diccionarioTfIdf[arraydeCercania[0]][j] != 0 && diccionarioTfIdf[arraydeCercania[1]][j] != 0)
                            {

                                int contador = 0;

                                string marcador = " ";

                                int resultado = int.MaxValue;

                                int cambioDeString = 0;

                                for (int k = 0; k < documentosGuardados[j].Length; k++)
                                {

                                    if (documentosGuardados[j][k] == arraydeCercania[0] || documentosGuardados[j][k] == arraydeCercania[1])
                                    {

                                        if (marcador != documentosGuardados[j][k])
                                        {
                                            marcador = documentosGuardados[j][k];

                                            cambioDeString++;

                                            if (cambioDeString != 1)
                                            {

                                                if (k - contador <= resultado) resultado = k - contador;

                                            }

                                            contador = k;

                                        }

                                        else
                                        {
                                            cambioDeString++;

                                            contador = k;
                                        }

                                    }

                                }

                                diccionarioSimilitudDeCoseno[arrayDeRutas[j]] = diccionarioSimilitudDeCoseno[arrayDeRutas[j]] * (1 + (1 / resultado));

                            }

                        }

                    }

                }

            }


        }


        //ordenar las palabras de la query segun su tfidf de mayor a menor

        public static string[] OrdenarQuerySegunTfIdf(List<string> querySinRepetir, double[] queryTfIDf)
        {

            string[] querySinRepetirAuxiliar = new string[querySinRepetir.Count];

            for (int i = 0; i < queryTfIDf.Length; i++)
            {
                double comparador = double.MinValue;

                int posicion = -1;

                for(int j = 0; j < querySinRepetir.Count; j++)
                {

                    if(queryTfIDf[j] > comparador)
                    {

                        posicion = j;

                        comparador = queryTfIDf[j];

                    }

                }

                querySinRepetirAuxiliar[i] = querySinRepetir[posicion];

                queryTfIDf[posicion] = -1;

            }


            /* string palabra = " ";

             string[] querySinRepetirAuxiliar = querySinRepetir.ToArray();

             double[] queryTfIdfAuxiliar = queryTfIDf;

             for (int i = 0; i < querySinRepetir.Count; i++)
             {
                 double comparador = double.MinValue;

                 for (int j = 0; j < queryTfIDf.Length; j++)
                 {
                     if (queryTfIdfAuxiliar[j] > comparador)
                     {
                         palabra = querySinRepetirAuxiliar[j];

                         comparador = queryTfIdfAuxiliar[j];

                     }

                 }

                 querySinRepetirAuxiliar[i] = palabra;

             }

             return querySinRepetirAuxiliar;
             */

            return querySinRepetirAuxiliar;
        }


        //snippets

        public static string Snippets(List<string> querySinRepetir, double[] queryTfIDf, string ruta, List<string[]> documentosGuardados, string[] arrayDeRutas)
        {

            string[] snippets = new string[20];

            string snippetsFinal;

            int indiceDeLaRuta = -1;

            int count = 0;

            //metodo para buscar en que posicion del array de rutas esta la ruta buscada

            for (int i = 0; i < arrayDeRutas.Length; i++)
            {

                if (ruta == arrayDeRutas[i])
                {

                    indiceDeLaRuta = i;

                    break;

                }

            }

            string[] documento = documentosGuardados[indiceDeLaRuta];

            string[] querySinRepetirAuxiliar = OrdenarQuerySegunTfIdf(querySinRepetir, queryTfIDf);

            //buscar la palabra de la query con mayor tfidf en el documento mas importante y crear el snippet

            for (int a = 0; a < querySinRepetirAuxiliar.Length; a++)
            {

                if (documento.Contains(querySinRepetirAuxiliar[a]))
                {

                    for (int j = 0; j < documento.Length; j++)
                    {

                        if (documento[j] == querySinRepetirAuxiliar[a])
                        {

                            for (int k = j; k < j + 20; k++)
                            {
                                try
                                {

                                    snippets[count] = documento[k];

                                    count++;



                                }

                                catch { };

                            }

                            break;

                        }

                    }

                   break;

                }

            }
            snippetsFinal = string.Join(" ", snippets);

            return snippetsFinal;

        }
    }
}




