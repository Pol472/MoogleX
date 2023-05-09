namespace MoogleEngine;

public class Query
{
    public string[] TerminosQuery;
    public Query(string query)
    {
        TerminosQuery = Metodos.SepararPalabras(query);
    }
}