namespace MoogleEngine;
public class Coleccion
{
    public static char SeparardorDelSistema = Path.DirectorySeparatorChar;
    private string[] Rutas = Directory.GetFiles(".."+SeparardorDelSistema+"Content");
    public string[] Archivos;
    public string[] Titulos;
    public List<string> ListaPalabrasSinRep;
    public double[,] TheMatrix;
    double[] IDF;
    public Dictionary<string, int> Lista;

    public Coleccion()
    {
      
      Archivos = Metodos.Lector(Rutas);
      ListaPalabrasSinRep = Metodos.ListaDePalabras(Archivos);
      IDF = new double[ListaPalabrasSinRep.Count];
      Titulos = Metodos.Nominador(Rutas);
      TheMatrix = Matriz();
      Lista = llenarDic();
      
    }
    //Metodos auxiliares de la clase
    public double[,] Matriz()
    {
        //Metodo para generar la matriz numerica
        Dictionary<string, int>[] diccionario =  new Dictionary<string,int>[Archivos.Length];
        //Vamos a generar el diccionario de las palabras con las frecuencias brutas
        for(int i = 0; i < diccionario.Length ; i++)
        {
            string[] temporal = Metodos.SepararPalabras(Archivos[i]);
            Dictionary<string,int> temp = new Dictionary<string, int>();
                foreach(string palabra in temporal)
                {
                    if(temp.ContainsKey(palabra))
                    temp[palabra]++;
                    else
                    temp[palabra]=1;
                }
                diccionario[i] = temp;
        }
        //Vamos a construir la matriz con los vectores TF-IDF
        //Primero TF
       double[,] TheMatrix = new double[Archivos.Length, ListaPalabrasSinRep.Count];
       for(int i = 0;i< Archivos.Length; i++) 
       {
        double FrecuenciaAbsoluta=diccionario[i].Values.Sum();
        for(int j =0; j< ListaPalabrasSinRep.Count;j++)
        {
            if(diccionario[i].ContainsKey(ListaPalabrasSinRep[j]))
            TheMatrix[i,j]= diccionario[i][ListaPalabrasSinRep[j]];
            TheMatrix[i,j]= TheMatrix[i,j]/FrecuenciaAbsoluta;
        }
       }   
       //Vamos a calcular el idf de la matriz, primero determinando el numero de documentos en los que cada palabra aparece
        
        double docs = Archivos.Length;
        for (int i = 0; i < TheMatrix.GetLength(1); i++)
        {
            for (int j = 0; j < TheMatrix.GetLength(0); j++)
            {
                if (TheMatrix[j, i] > 0)
                    IDF[i]++;
            }
        }
        //Vamos a incorporar la formula de IDF a cada palabra
        for (int i = 0; i < IDF.Length; i++)
        {
            if (IDF[i] != 0)
                IDF[i] = Math.Log((double)docs / (double)IDF[i]);
        }
        //Ahora crearemos la matriz TF-IDF
        TheMatrix = Metodos.ProductoTurbio(TheMatrix, IDF);
      return TheMatrix;
    }
    public double[] VectorQuerys(string query, double CardinalColeccion)
    {
        //Este metodo crea el vector TF-IDF del query
        List<string> CambiodeBase = ListaPalabrasSinRep;

        double docs = CardinalColeccion;
        string[] vector = Metodos.SepararPalabras(query);
        double[] VectorQuery = new double[CambiodeBase.Count];
        //Determinar Frecuencia Bruta
        for (int i = 0; i < VectorQuery.Length; i++)
        {
            for (int j = 0; j < vector.Length; j++)
            {
                if (CambiodeBase[i] == vector[j])
                    VectorQuery[i]++;
            }
        }

        //Vamos a calcular TF-IDF
        for (int i = 0; i < VectorQuery.Length; i++)
        {
            VectorQuery[i] = (double)VectorQuery[i] / (double)vector.Length;
            VectorQuery[i] *= (double)IDF[i];
        }
        return VectorQuery;
    }
    

    

    public SearchItem[] ObtenerSearchItems(string query)
    {
        //Metodo para retornar el Array con los SearchItems que se ofreceran al usuario
        Query objetoQuery = new Query(query);
        double[] vectorScore = Metodos.Similitud(TheMatrix,VectorQuerys(query, Archivos.Length));
        int[] indicesDeTextos = Enumerable.Range(0, vectorScore.Length).ToArray();
        string[] mejoresTitulos = Metodos.MejoresTitulo(Metodos.CrearDiccionario(Titulos,vectorScore));
        Array.Sort(vectorScore,indicesDeTextos);

        Array.Reverse(indicesDeTextos);

        Array.Reverse(vectorScore);
        
        List<SearchItem> searchItems = new List<SearchItem>();
        //Ordenar el query de acuerdo a la relevancia de cada una de sus palabras
        string[] terminosQuery = OrdenarQuery(objetoQuery.TerminosQuery, VectorQuerys(query,Archivos.Length));
     
        for (int i = 0; i < vectorScore.Length; i++)
        {
            //solo se devolvera aquellos searchitems cuyo score es mayor que cerp
            if (vectorScore[i] > 0)
            {
                string snippet = "";
                if(ListaPalabrasSinRep.Contains(terminosQuery[0]))
                {
                 snippet = Metodos.Snipet(terminosQuery[0],Archivos[indicesDeTextos[i]]);
                }
                else if(ListaPalabrasSinRep.Contains(terminosQuery[1]))
                {
                 snippet = Metodos.Snipet(terminosQuery[1],Archivos[indicesDeTextos[i]]);   
                }
                else
                {
                  snippet = Metodos.Snipet(terminosQuery[2],Archivos[indicesDeTextos[i]]);   
                }
                searchItems.Add(new SearchItem(mejoresTitulos[i],snippet,(float)vectorScore[i]));
            }
            else break;
        }
      return searchItems.ToArray();
    }

    private Dictionary<string, int>  llenarDic ()
    {
        //Este metodo crea el diccionario que sera usado para organizar mi query por la relevancia de sus palabras
        Dictionary<string,int> diccionario = new Dictionary<string, int>();
        int contador = 0;
        foreach(string palabra in ListaPalabrasSinRep)
        {
            if(!diccionario.ContainsKey(palabra))
            {
                diccionario.Add(palabra,contador);
                contador++;
            }
        }
        return diccionario;
    }
    private string[] OrdenarQuery(string[] terminosQuery, double[] vectorQuery)
    {
        //Este metodo ordena al query por la relevancia de sus palabras(la primera seria la de mayor tf-idf)
        for (int i = 0; i < terminosQuery.Length - 1; i++)
        {
            for (int j = i + 1; j < terminosQuery.Length; j++)
            {
                double valorA = Lista.ContainsKey(terminosQuery[i]) ? vectorQuery[Lista[terminosQuery[i]]] : 0;
                double valorB = Lista.ContainsKey(terminosQuery[j]) ? vectorQuery[Lista[terminosQuery[j]]] : 0;

                if (valorA < valorB)
                {
                    Intercambiar(terminosQuery, i, j);
                }
            }
        }
        return terminosQuery;
    }

    private void Intercambiar(string[] terminosQuery, int i, int j)
    {
        string temp = terminosQuery[i];
        terminosQuery[i]=terminosQuery[j];
        terminosQuery[j]= temp;
    }
    //Esta serie de metodos son utilizados para crear la sugerencia en caso de ser necesaria 
    public bool NecesidadSugerencia(string[] query)
    {
        //este metodo determina si una sugerencia es necesaria y lo sera si al menos una de las palabras del query no esta en la coleccion
        int contador = 0;
        for(int i = 0; i<query.Length;i++)
        {
            if(ListaPalabrasSinRep.Contains(query[i]))
            {
                contador++;
            }
        }
        if(contador < query.Length) 
            return true;
        return false;
    }
    public string ConstructorSugerencia(string query)
    {
        // este metodo construye la sugerencia utilizando el metodo Levenshtein
        Query objetoQuery = new Query(query);
        string[] palabra = objetoQuery.TerminosQuery;
        
        string Sugerencia = "";
        for (int i = 0;i<palabra.Length;i++)
        {
            if(ListaPalabrasSinRep.Contains(palabra[i]))
            {
                Sugerencia+= palabra[i] + " ";
            }
            else
            {
                Sugerencia += Levenshtein(palabra[i]) + " ";
            }
        }

         return Sugerencia;
    }
    public string Levenshtein (string palabra)
    {
        //este metodo selecciona cual es la palabra mejor para ser sugerida en lugar de la que no esta en la coleccion y apela al calculo de la distancia de Levensthein que se implementa
        //en el metodo DistanciaLevenshtein que esta en la clase Metodos
       double Indicador = int.MaxValue;
       string palabras = palabra ;
       for(int i =0; i<Lista.Count; i++)
       {
        double temporal = Metodos.DistanciaLevenshtein(ListaPalabrasSinRep[i],palabra);
         if(temporal < Indicador)
         {
            Indicador = temporal; 
            palabras = ListaPalabrasSinRep[i];
         }
       }
     return palabras;
    }
    public string Sugerencia( string query)
    {
        //Este Metodo es que retorna la sugerencia final implementando todos los anteriores y es llamado desde moogle
        Query objetoQuery = new Query(query);
        string[] palabras = objetoQuery.TerminosQuery;
        if(NecesidadSugerencia(palabras))
        {
          return ConstructorSugerencia(query); 
        }
        else
        return "";
    }

}