
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;


public class move : MonoBehaviour
{
    [HideInInspector]
    public Vector2 _direction;
    public Transform food;
    public readWeights rw;
    private List<Transform> _segments;
    public Transform segmentPrefab;

    //Data

    List<float> data;
    List<List<float>> datalist = new List<List<float>>();
    private float prevX;
    private float time;
    private float prevY;  
    private Vector2 prevDir;

    private bool robot = true;
    
    private int inputKey;
    private int moveIndex;
    private Vector2 direction;
    private StreamWriter stream;
    private int maxBodyLength;
    public bool collect = false;
    private bool moved = false;
    private Vector3 prevPosition;

    private float[,] W1;
    private float[,] W2;
    private float[,] b1;
    private float[,] b2;
    private float[,] W3;
    private float[,] W4;
    private float[,] b3;
    private float[,] b4;

    private float[,] X_input;
    private List<float> decisions = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        InitValues();
        _segments = new List<Transform>();
        _segments.Add(this.transform);
        maxBodyLength = 10;
        this.prevPosition = new Vector3(0, 0, 0);
        b1 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\b1.txt");
        b2 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\b2.txt");
        b3 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\b3.txt");
        b4 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\b4.txt");
        W1 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\W1.txt");
        W2 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\W2.txt");
        W3 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\W3.txt");
        W4 = rw.getWeights("D:\\Unity\\Projects\\Snake\\SnakeData\\W4.txt");
        
    }
    void InitValues()
    {
        var xrand = UnityEngine.Random.Range(-5, 5);
        var yrand = UnityEngine.Random.Range(-5, 5);
        this.transform.position = new Vector3(xrand, yrand, 0);
        var picker = UnityEngine.Random.Range(0, 4);
        if (picker == 0)
        {
            _direction = Vector2.up;
        }
        else if (picker == 1)
        {
            _direction = Vector2.down;
        }
        else if (picker == 2)
        {
            _direction = Vector2.left;
        }
        else _direction = Vector2.right;
        
    }
    
    private float[,] forward(float[,] X_input, float[,] W1, float[,] W2, float[,] W3, float[,] W4, float[,] b1, float[,] b2, float[,] b3, float[,] b4) // SCALE WHEN CHANGING NETWORK
    {
        var Z1 = dot(W1, X_input);
        Z1 = add(Z1, b1);
        var A1 = ReLU(Z1);

        var Z2 = dot(W2, A1);
        Z2 = add(Z2, b2);
        var A2 = ReLU(Z2);

        var Z3 = dot(W3, A2);
        Z3 = add(Z3, b3);
        var A3 = ReLU(Z3);

        var Z4 = dot(W4, A3);
        Z4 = add(Z4, b4);
        var A4 = Softmax(Z4);

        return A4; // SCALE WHEN CHANGING NETWORK

    }
        

    private int GetDecision(float[,] lst)
    {
        decisions = new List<float>();  
        foreach (float dec in lst)
        {
            decisions.Add(dec);
             
        }
        decisions.Max();

        float maxIndex = -1, maxValue = int.MinValue;
        for (int i = 0; i < decisions.Count; i++)
        {
            if ((maxIndex < 0) || (decisions[i] > maxValue))
            {
                maxValue = decisions[i];
                maxIndex = i;
            }
        }
        return (int)maxIndex;
    }


    // Update is called once per frame
    void Update()
    {
        if (!robot)
        {
            if (moved)
            {
                if (_direction == Vector2.right || _direction == Vector2.left)
                {
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        _direction = Vector2.up;
                        moved = false;

                    }
                    else if (Input.GetKeyDown(KeyCode.S))
                    {
                        _direction = Vector2.down;
                        moved = false;

                    }
                    else
                    {

                    }
                }
                else if (_direction == Vector2.up || _direction == Vector2.down)
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        _direction = Vector2.left;
                        moved = false;


                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        _direction = Vector2.right;
                        moved = false;

                    }
                    else
                    {

                    }
                }
            }

        }
        
    }
    
    
    private void FixedUpdate()
    {
        X_input = CollectData(_direction, prevDir);
        var dec = forward(X_input,W1,W2,W3,W4,b1,b2,b3,b4);  // SCALE WHEN CHANGING NETWORK
        var dec_final = GetDecision(dec);
        //printMatrix(X_input);
        printMatrix(forward(X_input, W1, W2, W3,W4,b1,b2,b3,b4)); // SCALE WHEN CHANGING NETWORK
        print(dec_final);
            
                if (dec_final == 0)
                {
                    _direction = Vector2.up;
                    moved = false;

                }
                else if (dec_final == 1)
                {
                    _direction = Vector2.down;
                    moved = false;

                }
                else
                {

                }
            
            
                if (dec_final == 2)
                {
                    _direction = Vector2.left;
                    moved = false;


                }
                else if (dec_final == 3)
                {
                    _direction = Vector2.right;
                    moved = false;

                }
                else
                {

                }
            
        
        if (_segments.Count > 20)
        {
            ResetGame();
        }
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }
        
        
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x + _direction.x),
            Mathf.Round(this.transform.position.y + _direction.y),
            0.0f
            );
        moved = true;
        prevDir = _direction; 
    }

    private float[,] CollectData(Vector2 direction, Vector2 prevDir)
    {
        data = new List<float>();
        var bodyLength = (float)_segments.Count/maxBodyLength;
        var X = (this.transform.position.x + 19f) / 38f;
        var Y = (this.transform.position.y + 9f) / 18f;
        var foodX = (food.position.x+19f)/38f;
        var foodY = (food.position.y+9f)/18f;
        var foodDistance = Vector3.Distance(this.transform.position, food.position)/40f;
        var angle = Vector2.Angle(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(food.position.x, food.position.y))/180f;

        if (direction == Vector2.up)
        {
            moveIndex = 0;
        }
        else if (direction == Vector2.down)
        {
            moveIndex = 1;
        }
        else if (direction == Vector2.left)
        {
            moveIndex = 2;
        }
        else if (direction == Vector2.right)
        {
            moveIndex = 3;
        }
        if((prevDir.x == direction.x) && (prevDir.y == direction.y))
        {
            moveIndex = 4;
        }

        time += Time.deltaTime;

        
        if (robot)
        {
            data.Add((float)X);
            data.Add((float)Y);
            data.Add((float)prevX);
            data.Add((float)prevY);
            data.Add((float)foodX);
            data.Add((float)foodY);
            data.Add((float)foodDistance);
            data.Add((float)angle);
            data.Add((float)bodyLength);
            
            prevX = X;
            prevY = Y;
            float[,] X_input = new float[data.Count,1];
            

            for (int j = 0; j < data.Count; j++)
            {  
                X_input[j, 0] = data[j];

            }
            return X_input;
        }
        else
        {
            data.Add((float)X);
            data.Add((float)Y);
            data.Add((float)prevX);
            data.Add((float)prevY);
            data.Add((float)foodX);
            data.Add((float)foodY);
            data.Add((float)foodDistance);
            data.Add((float)angle);
            data.Add((float)bodyLength);
            data.Add((float)moveIndex);


            datalist.Add(data);
            prevX = X;
            prevY = Y;
            return null;
        }
    }

    void WriteMatrix(List<List<float>> floatMatrix)
    {
        string fileName = "data.txt";
        
            FileStream fs = File.Open(fileName, File.Exists(fileName)?FileMode.Append:FileMode.Create);
            stream = new StreamWriter(fs);
            if(floatMatrix.Count > 20)
            {
            for (int i = 0; i < floatMatrix.Count - 5; i++)
                {
                    if (i != 0) stream.WriteLine("");
                    for (int j = 0; j < floatMatrix[i].Count; j++)
                    {
                        stream.Write(floatMatrix[i][j] + " ");
                    }
                }
            stream.WriteLine("");
            

            }
            stream.Flush();
            stream.Close();
    }
    private void ResetGame()
    {
        if (collect) WriteMatrix(datalist);

        datalist = new List<List<float>>();
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        datalist.Clear();
        _segments.Clear();
        _segments.Add(this.transform);

        InitValues();

    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);
    }

    private string printMatrix(float[,] m, string decimals = "0.0000")
    {
        var matrixString = "";
        foreach (float f in m)
        {
            matrixString += f.ToString(decimals) + ", ";

        }

        print(matrixString);
        return matrixString;
    }

    private float[,] add(float[,] a,float[,] b)
    {
        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                a[i, j] = a[i, j] + b[i, j];


            }

        }
        return a;
    }

    private float[,] ReLU(float[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i,j] = Math.Max(0, matrix[i,j]);
             
            }

        }

        
        return matrix;
    }

    private float[,] Softmax(float[ ,] x)
    {
        var max = 0f;
        var sum = 0f;

        for (int i = 0; i < x.GetLength(0); i++)
        {
            for (int j = 0; j < x.GetLength(1); j++)
            {
                if(x[i,j] > max)
                {
                    max = x[i,j];   
                }
                sum += x[i, j];
                   


            }
    // exp = np.exp(x - np.max(x))

        }
        for (int i = 0; i < x.GetLength(0); i++)
        {
            for (int j = 0; j < x.GetLength(1); j++)
            {
                if (x[i, j] > max)
                {
                    x[i, j] = Mathf.Pow(2.71828f, x[i, j] - max)/sum;
                    
                }



            }
            // / exp.sum(axis=0)

        }


        return x;
    }

    private static float[,] dot(float[,] a, float[,] b)
    {
        float[,] dot = new float[a.GetLength(0), b.GetLength(1)];

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                // the next loop looks way slow according to the profiler
                for (int k = 0; k < b.GetLength(0); k++)
                    dot[i, j] += a[i, k] * b[k, j];
            }
        }
        return dot;
    }
    public void Recording()
    {
        collect = !collect;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "food")
        {
            Grow();

        } else if (other.tag == "obstacle")
        {
            ResetGame();
        }
    }
    public void exit()
    {
        Application.Quit();
    }
}


