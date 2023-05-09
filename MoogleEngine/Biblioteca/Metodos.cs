using System.Text.RegularExpressions;

namespace MoogleEngine;

public class Metodos
{
    //Metodos para trabajar con la lectura de informacion
    public static string Snipet(string query, string texto)
    {
        //Este metodo devuelve el string que funciona como snipet
        var regex = new Regex($"\\b({query})\\b", RegexOptions.IgnoreCase);
        var match = regex.Match(texto);
        if (match.Success)
        {
            var start = Math.Max(0, match.Index - 50);
            var end = Math.Min(texto.Length, match.Index + 50);
            return texto.Substring(start, end - start);
        }
        else
        {
            return texto.Substring(0, Math.Min(texto.Length, 100));
        }

    }
    public static string[] SepararPalabras(string texto)
    {
        //este metodo devuelve los documentos en un array de string separado en palabras 
        string[] words = texto.ToLower().Split(new char[] { ' ', ',', '.', ';', ':', '-', '!', '¡', '?', '¿', '(', ')', '[', ']', '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        return words;
    }
    public static string[] Lector(string[] Rutas)
    {
        //Este metodo lee el interior de cada archivo txt y lo asigna a un valor de el array como string
        string[] archivos = new string[Rutas.Length];
        for (int i = 0; i < archivos.Length; i++)
        {
            archivos[i] = File.ReadAllText(Rutas[i]);
        }
        return archivos;
    }
    public static string[] Nominador(string[] Rutas)
    {
        //este metodo retorna los titulos de cada archivo
        string[] titulos = new string[Rutas.Length];
        for (int i = 0; i < Rutas.Length; i++)
        {
            titulos[i] = Path.GetFileNameWithoutExtension(Rutas[i]);
        }
        return titulos;
    }
    public static List<string> ListaDePalabras(string[] Archivos)
    {
        //Este metodo crea la lista de palabras unicas
        HashSet<string> Listarep = new HashSet<string>();
        for (int i = 0; i < Archivos.Length; i++)
        {
            string[] temporal = Metodos.SepararPalabras(Archivos[i]);
            for (int j = 0; j < temporal.Length; j++)
            {
                Listarep.Add(temporal[j]);
            }
        }
        return Listarep.ToList();
    }

    //Metodos para trabajar con numeros
    public static double[,] ProductoTurbio(double[,] matriz, double[] array)
    {
        //Metodo de calculo que multiplica en un producto no usual analogo a la suma cada fila de la matriz con el array del argumento
        int m = matriz.GetLength(0);
        int n = matriz.GetLength(1);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matriz[i, j] *= array[j];
            }
        }
        return matriz;
    }
    public static double[] Similitud(double[,] matriz, double[] vectorQuery)
    {
        double[,] TheMatrix = matriz;
        //ahora vamos a determinar la similitud por coseno
        //primero calculamos el producto  punto de los vectores de la matriz y el vector del query
        double[] ProductoPunto = MatrizPorVector(TheMatrix, vectorQuery);
        //despues calculamos la norma de cada vector
        double[] Normas = new double[ProductoPunto.Length];
        double normaVector = Norma(vectorQuery);
        for (int i = 0; i < Normas.Length; i++)
        {
            double[] temp = new double[TheMatrix.GetLength(1)];
            for (int j = 0; j < temp.Length; j++)
            {
                temp[j] = TheMatrix[i, j];
            }
            Normas[i] = Norma(temp);
        }
        //Ahora debemos calcular el producto de las normas de los  vectores 
        for (int i = 0; i < Normas.Length; i++)
        {
            Normas[i] *= normaVector;
        }
        //Ahora debemos dividir el producto punto de los vectores entre el producto de sus normas
        double[] score = new double[ProductoPunto.Length];
        for (int i = 0; i < ProductoPunto.Length; i++)
        {
            if (Normas[i] != 0)
                score[i] = ProductoPunto[i] / (double)Normas[i];
            else
                score[i] = 0;
        }
        return score;
    }
    public static double Norma(double[] a)
    {
        //Este Metodo Calcula la norma de cada vector
        double suma = 0;
        for (int i = 0; i < a.Length; i++)
        {
            suma += Math.Pow(a[i], 2);
        }
        double norma = (Math.Sqrt(suma));
        return norma;
    }
    public static double[] MatrizPorVector(double[,] Matriz, double[] vectorQuery)
    {
        //Este metodo multiplica una matriz por un vector
        double[] result = new double[Matriz.GetLength(0)];
        for (int i = 0; i < Matriz.GetLength(0); i++)
        {
            double sum = 0;
            for (int j = 0; j < Matriz.GetLength(1); j++)
            {
                sum += Matriz[i, j] * vectorQuery[j];
            }
            result[i] = sum;
        }
        return result;
    }
    //Metodos para diccionarios
    public static Dictionary<string, double> CrearDiccionario(string[] titulos, double[] score)
    {
        //Crea un diccionario con los titulos de los documentos y sus indices
        Dictionary<string, double> diccionario = new Dictionary<string, double>();
        for (int i = 0; i < titulos.Length; i++)
            diccionario.Add(titulos[i], score[i]);
        return diccionario;
    }
    public static string[] MejoresTitulo(Dictionary<string, double> diccionario)
    {
        //Organiza los titutlos de acuerdo a su score
        string[] mejoresTitulos = new string[diccionario.Count];
        int count = 0;
        foreach (var item in diccionario.OrderByDescending(item => item.Value))
        {
            mejoresTitulos[count] = item.Key;
            count++;
        }
        return mejoresTitulos;
    }

    public static double DistanciaLevenshtein(string str1, string str2)
    {
        //Calculo de la distancia de Levenshtein
        int longitudStr1 = str1.Length;
        int longitudStr2 = str2.Length;
        int[,] matrizDistancia = new int[longitudStr1 + 1, longitudStr2 + 1];

        if (longitudStr1 == 0)
        {
            return longitudStr2;
        }

        if (longitudStr2 == 0)
        {
            return longitudStr1;
        }

        for (int i = 0; i <= longitudStr1; i++)
        {
            matrizDistancia[i, 0] = i;
        }

        for (int j = 0; j <= longitudStr2; j++)
        {
            matrizDistancia[0, j] = j;
        }

        for (int i = 1; i <= longitudStr1; i++)
        {
            for (int j = 1; j <= longitudStr2; j++)
            {
                int costo = (str2[j - 1] == str1[i - 1]) ? 0 : 1;
                matrizDistancia[i, j] = Math.Min(Math.Min(matrizDistancia[i - 1, j] + 1, matrizDistancia[i, j - 1] + 1), matrizDistancia[i - 1, j - 1] + costo);
            }
        }

        return (double)matrizDistancia[longitudStr1, longitudStr2];
    }


}
