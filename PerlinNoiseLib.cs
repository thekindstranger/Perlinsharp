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
        if (thirdDimension){
            return ThreeDGenerate(_x, _y, _z);
        }else{
            return TwoDGenerate(_x, _y);
        }
    }

    private float TwoDGenerate(float xA, float yA){
        //Find pos inside grid square
        float relativePosX = xA % sizeX;
        float relativePosY = yA % sizeY;
        Vector relativeVect = new Vector(relativePosX, relativePosY);

        //Find square's pos on grid
        float leftLine = xA - relativePosX;
        float bottomLine = yA - relativePosY;
        float topLine = bottomLine + sizeY;
        float rightLine = leftLine + sizeX;

        //Find random vects at grid corners
        Vector topLeftVect = FindCornerVect(leftLine, topLine);
        Vector bottomLeftVect = FindCornerVect(leftLine, bottomLine);
        Vector topRightVect = FindCornerVect(rightLine, topLine);
        Vector bottomRightVect = FindCornerVect(rightLine, bottomLine);

        //Define vects from corner to point
        Vector vectToTopLeft = new Vector(sizeX - relativePosX, relativePosY);
        Vector vectToBottomLeft = relativeVect;
        Vector vectToTopRight = new Vector(sizeX - relativePosX, sizeY - relativePosY);
        Vector vectToBottomRight = new Vector(relativePosX, sizeY - relativePosY);

        //Generate random values
        float topLeftVal = Vector.Dot(topLeftVect, vectToTopLeft);
        float bottomLeftVal = Vector.Dot(bottomLeftVect, vectToBottomLeft);
        float topRightVal = Vector.Dot(topRightVect, vectToTopRight);
        float bottomRightVal = Vector.Dot(bottomRightVect, vectToBottomRight);

        //Interpolate the values using the pos as weight, generating the final value
        float topVal = Interp(topLeftVal, topRightVal, relativePosX);
        float bottomVal = Interp(bottomLeftVal, bottomRightVal, relativePosX);

        return Interp(bottomVal, topVal, relativePosY);
    }

    private float ThreeDGenerate(float xA, float yA, float zA){

    }

    private float Interp(float valueOne, float valueTwo, float weight){
        return (valueTwo - valueOne) * weight + valueOne;
    }

    //This paring function will combine the coords of the intersections of our grid, so we can set the seed for the rng and get consistent output for each seed
    private int Pair(int numA, int numB){
        return ((numA * numA) + (numA * 3) + (2 * (numA + numB)) + numB + (numB * numB)) / 2;
    }

    private Vector FindCornerVect(float coordX, float coordY, float coordZ = 0){
        int xIntersectionNumber = coordX % sizeX;
        int yIntersectionNumber = coordY % sizeY;
        if(this.thirdDimension){
            int zIntersectionNumber = coordZ % sizeZ;

            int _seed = Pair(zIntersectionNumber, Pair(xIntersectionNumber, yIntersectionNumber)) + seed;
            rng = new random(_seed);

            int vectNum = rng.Next(directions.Count());
            return directions[vectNum];
        }else{
            int _seed = Pair(xIntersectionNumber, yIntersectionNumber) + seed;
            rng = new random(_seed);

            int vectNum = rng.Next(directions.Count());
            return directions[vectNum];
        }
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