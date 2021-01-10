using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing2 : MonoBehaviour
{
    int[] myArray = { 3, 7, 1, 5, 9 }; // -> {1,3,5,7,9}

    // Start is called before the first frame update
    void Start()
    {
        SquareColor[] squares = FindObjectsOfType<SquareColor>();
        for (int j = 1; j < squares.Length; j += 1)
        {
            for (int i = 1; i < squares.Length; i += 1)
            {
                if (squares[i - 1].transform.position.y > squares[i].transform.position.y)
                {
                    SquareColor temp = squares[i];
                    squares[i] = squares[i - 1];
                    squares[i - 1] = temp;
                }
            }
        }

        for (int i = 0; i < squares.Length; i += 1)
        {
            squares[i].transform.position = new Vector3(0, 1.25f * i - 3, 0);
        }





        for(int j = 0; j < myArray.Length; j += 1)
        {
            for(int i = 1; i <myArray.Length; i += 1)
            {
                if(myArray[i-1] > myArray[i])
                {
                    int temp = myArray[i];
                    myArray[i] = myArray[i - 1];
                    myArray[i - 1] = temp;
                }
            }
        }

        
        foreach(int num in myArray)
        {
            print(num);
        }
    }
    
}
