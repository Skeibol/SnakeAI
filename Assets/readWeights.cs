
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ReadWeights : MonoBehaviour 
{
    public List<float> row;
    public List<List<float>> table = new List<List<float>>();



    public float[,] getWeights(string filepath)
    {
        table = new List<List<float>>();
        var txt = File.ReadAllText(filepath);
        foreach(string line in txt.Split("\n"))
        {
            print(line);
            row = new List<float>(); 
            foreach (string element in line.Split(","))
            {
               
                var x = float.Parse(element);
               
                row.Add(x);

            }
            table.Add(row);
           
        }
        float[,] array = new float[table.Count, table[0].Count];
        
        for (int j = 0; j < table.Count; j++)
        {
            for (int i = 0; i < table[j].Count; i++)
            {
                array[j, i] = table[j][i];
            }
        }
        return array;
    }

    
    
}
