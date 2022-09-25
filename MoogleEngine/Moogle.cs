namespace MoogleEngine;


public static class Moogle
{
    public static SearchResult Query(string query) 
    {
        // Modifique este método para responder a la búsqueda
        
        float valor = 0;
        
        string[] rutas = preSearch.ObtenerRuta();

        string[] queryNormalizado = search.ObtenerQuery(query);

        List<string> listaDeOperadores = search.GuardarOperadores(queryNormalizado);

        List<string> querySinRepetir = search.querySinRepetir(queryNormalizado);

        double[] queryTfIDf = search.queryTfidf(preSearch.diccionarioTfIDf , querySinRepetir , queryNormalizado , rutas);

        search.OperadorDeImportancia(listaDeOperadores , querySinRepetir , queryTfIDf);

        double magnitudDeLaQuery = search.magnitudDeLaQuery(querySinRepetir , queryTfIDf);

        Dictionary<string, double> diccionarioSimilitudDeCoseno = search.diccionarioDeSimilitudDeCoseno(preSearch.documentosGuardados , querySinRepetir , queryTfIDf , preSearch.diccionarioTfIDf , magnitudDeLaQuery , rutas);

        search.Operadores(listaDeOperadores, rutas, preSearch.diccionarioTfIDf, diccionarioSimilitudDeCoseno, preSearch.documentosGuardados);

        List<string> similitudDeCosenoOrdenado = search.similitudDeCosenoOrdenado(diccionarioSimilitudDeCoseno);

        
        
            if (similitudDeCosenoOrdenado.Count == 0)
            {
                SearchItem[] items = new SearchItem[1];

                items[0] = new SearchItem("No se encontraron resultados", "Pruebe realizar otra búsqueda", 0);

                return new SearchResult(items);

            }

            else
            {
            SearchItem[] items = new SearchItem[similitudDeCosenoOrdenado.Count];

                for (int i = 0; i < similitudDeCosenoOrdenado.Count; i++)
                {
                    string snippets = search.Snippets(querySinRepetir, queryTfIDf, similitudDeCosenoOrdenado[i], preSearch.documentosGuardados, rutas);
                    items[i] = new SearchItem(Path.GetFileNameWithoutExtension(similitudDeCosenoOrdenado[i]),snippets, valor);

                }
                return new SearchResult(items);
            }  

    }
        
}

