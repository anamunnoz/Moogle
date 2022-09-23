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

        List<string> similitudDeCosenoOrdenado = search.similitudDeCosenoOrdenado(diccionarioSimilitudDeCoseno);

        search.Operadores(listaDeOperadores, rutas , preSearch.diccionarioTfIDf , diccionarioSimilitudDeCoseno, preSearch.documentosGuardados);

        //string[] querySinRepetirAuxiliar = search.OrdenarQuerySegunTfIdf(querySinRepetir, queryTfIDf);


        SearchItem[] items = new SearchItem[6];

        for (int i = 0; i < 6; i++)
        {
            string snippets= search.Snippets(querySinRepetir, queryTfIDf, similitudDeCosenoOrdenado[i],preSearch.documentosGuardados , rutas);
            items[i] = new SearchItem (similitudDeCosenoOrdenado[i], snippets, valor);

        }

        return new SearchResult(items, query);
    }
}
