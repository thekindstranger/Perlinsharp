using System;
using System.Collections.Generic;

public class Grid
{
    //This class will generate and contain the grid, and output "random" values when given a position on the grid

    private float sizeX = 1f;
    private float sizeY = 1f;
    private float sizeZ = 1f;
    private random rng;
    public int seed;
    public bool thirdDimension;

    //This dict will determine the directions the corners of our grid can have
    private List<Vector> directions = new List<Vector>();

    //Base constructor will set up the grid with a random seed, overload allows for setting the seed
    public Grid(bool hasThirdDimension, Vector[] vects = null){
        rng = new random();
        seed = rng.next();
        thirdDimension = hasThirdDimension;

        //Setting the default directions if none given
        if(vects == null){
            if(thirdDimension){
                directions.add(new Vector(1f, 1f, 1f));
                directions.add(new Vector(1f, 1f, -1f));
                directions.add(new Vector(1f, -1f, 1f));
                directions.add(new Vector(1f, -1f, -1f));
                directions.add(new Vector(-1f, 1f, 1f));
                directions.add(new Vector(-1f, 1f, -1f));
                directions.add(new Vector(-1f, -1f, 1f));
                directions.add(new Vector(-1f, -1f, -1f));
            }else{
                directions.add(new Vector(1f, 1f));
                directions.add(new Vector(1f, -1f));
                directions.add(new Vector(-1f, 1f));
                directions.add(new Vector(-1f, -1f));
            }
        }else{
            foreach (Vector vect in vects){
                //Here we make sure that the vect has the correct number of dimensions
                if (vect.isThreeDimensional != thirdDimension){
                    throw new InvalidOperationException();
                }
                directions.add(vect);
            }
        }
    }

    public Grid(int _seed) : this(hasThirdDimension, vects) {
        this.seed = _seed;
    }

    //This method will allow setting a custom grid size
    public SetSize(float _sizeX, float _sizeY, float _sizeZ = 0){
        this.sizeX = _sizeX;
        this.sizeY = _sizeY;

        if (this.thirdDimension){
            this.sizeZ = _sizeZ;
        }
    }

    //Actual generator function, z value has default to avoid making an overload for 3D grid
    public float GetValue(float _x, float _y, float _z = 0){
        
    }

    private float interp(float valueOne, float valueTwo, float weight){
        return (valueTwo - valueOne) * weight + valueOne;
    }
}

public struct Vector
{
    public float x;
    public float y;
    public float z = 0;

    private bool isThreeDimensional = false;

    public Vector(float _x, float _y){
        this.x = _x;
        this.y = _y ;       
    }

    public Vector(float _z) : this(_x, _y){
        this.z = _z;
        this.isThreeDimensional = true;
    }

    //This will return the dot product of two vectors as long as they have the same number of dimensions
    public static float Dot(Vector vectOne, Vector vectTwo){

        //Check if the vects have the same number of dimensions
        if ((!vectOne.isThreeDimensional && vectTwo.isThreeDimensional) || (vectOne.isThreeDimensional && !vectTwo.isThreeDimensional)){
            throw new InvalidOperationException();
        }

        if (vectOne.isThreeDimensional){
            return (vectOne.x * vectTwo.x) + (vectOne.y * vectTwo.y) + (vectOne.z * vectTwo.z);
        }else{
            return (vectOne.x * vectTwo.x) + (vectOne.y * vectTwo.y);
        }
    }

    public static Vector operator -(Vector a, Vector b) => new Vector(a.x - b.x, a.y - b.y, a.z - b.z);
}