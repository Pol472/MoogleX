namespace MoogleEngine;


public static class Moogle
{
    public static Coleccion coleccion;
    public static SearchResult Query(string query) 
    {
        SearchItem[] searchItem = coleccion.ObtenerSearchItems(query);
        string sugerencia = coleccion.Sugerencia(query);
        return new SearchResult(searchItem, sugerencia);
    }

    public static void Iniciar()
    {
        coleccion = new Coleccion();
    }
}
            