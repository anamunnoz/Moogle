namespace MoogleEngine;


public static class Moogle
{
    public static SearchResult Query(string query)
    {
        // Modifique este método para responder a la búsqueda

        float valor = 0;

        string[] rutas = preSearch.ObtenerRuta();

        string sugerencia = " ";

        char[] separadoresOperadores = { '!', '^', '*' };

        string[] queryNormalizado = search.ObtenerQuery(query);

        List<string> listaDeOperadores = search.GuardarOperadores(queryNormalizado);

        List<string> querySinRepetir = search.querySinRepetir(queryNormalizado);

        //quitar los operadores a la query para realizar sugerencia 

        foreach (string palabra in querySinRepetir)
        {

            preSearch.Normalizar(palabra, separadoresOperadores);

        }

        if (querySinRepetir.Contains("~"))
        {

            querySinRepetir.Remove("~");

        }

        double[] queryTfIDf = search.queryTfidf(preSearch.diccionarioTfIDf, querySinRepetir, queryNormalizado, rutas);

        search.OperadorDeImportancia(listaDeOperadores, querySinRepetir, queryTfIDf);

        double magnitudDeLaQuery = search.magnitudDeLaQuery(querySinRepetir, queryTfIDf);

        Dictionary<string, double> diccionarioSimilitudDeCoseno = search.diccionarioDeSimilitudDeCoseno(preSearch.documentosGuardados, querySinRepetir, queryTfIDf, preSearch.diccionarioTfIDf, magnitudDeLaQuery, rutas);

        search.Operadores(listaDeOperadores, rutas, preSearch.diccionarioTfIDf, diccionarioSimilitudDeCoseno, preSearch.documentosGuardados);

        List<string> similitudDeCosenoOrdenado = search.similitudDeCosenoOrdenado(diccionarioSimilitudDeCoseno);

        

        //recorrer la query para crear la nueva query con las palabras sugeridas

        for (int i = 0; i < querySinRepetir.Count;  i++)
        {

            if (preSearch.diccionarioTfIDf.ContainsKey(querySinRepetir[i]))
            {

                sugerencia += " " + querySinRepetir[i];

            }

            else
            {
                string palabrasDespuesDeLaSugerenia = " ";

                int contador = int.MaxValue;

                foreach(string palabraAComparar in preSearch.diccionarioTfIDf.Keys)
                {

                    if(search.DistanciaHamill(querySinRepetir[i] , palabraAComparar) < contador)
                    {

                        contador = search.DistanciaHamill(querySinRepetir[i], palabraAComparar);

                        palabrasDespuesDeLaSugerenia = palabraAComparar;


                    }

                }

                sugerencia += " " + palabrasDespuesDeLaSugerenia;

            }

        }

       
        if (similitudDeCosenoOrdenado.Count == 0)
            {
                SearchItem[] items = new SearchItem[1];

                items[0] = new SearchItem("No se encontraron resultados", "Pruebe realizar otra búsqueda", 0);

                return new SearchResult(items , sugerencia);

            }

            else
            {
             SearchItem[] items = new SearchItem[similitudDeCosenoOrdenado.Count];
            
             string[] querySinRepetirAuxiliar = search.OrdenarQuerySegunTfIdf(querySinRepetir, queryTfIDf);


                for (int i = 0; i < similitudDeCosenoOrdenado.Count; i++)
                {
                    string snippets = search.Snippets(similitudDeCosenoOrdenado[i], preSearch.documentosGuardados, rutas , querySinRepetirAuxiliar);
                    items[i] = new SearchItem(Path.GetFileNameWithoutExtension(similitudDeCosenoOrdenado[i]),snippets, valor);

                }
                return new SearchResult(items , sugerencia);
            }  

    }
        
}

