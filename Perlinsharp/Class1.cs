using System;
using System.Collections.Generic;

public class Grid
{
    //This class will generate and contain the grid, and output "random" values when given a position on the grid

    private float sizeX = 1f;
    private float sizeY = 1f;
    private float sizeZ = 1f;
    private Random rng;
    public int seed;
    public bool thirdDimension;

    //This dict will determine the directions the corners of our grid can have
    private List<Vector> directions = new List<Vector>();

    //Base constructor will set up the grid with a random seed, overload allows for setting the seed
    public Grid(bool hasThirdDimension, Vector[] vects = null){
        rng = new Random();
        seed = rng.Next();
        thirdDimension = hasThirdDimension;

        //Setting the default directions if none given
        if(vects == null){
            if(thirdDimension){
                directions.Add(new Vector(1f, 1f, 1f));
                directions.Add(new Vector(1f, 1f, -1f));
                directions.Add(new Vector(1f, -1f, 1f));
                directions.Add(new Vector(1f, -1f, -1f));
                directions.Add(new Vector(-1f, 1f, 1f));
                directions.Add(new Vector(-1f, 1f, -1f));
                directions.Add(new Vector(-1f, -1f, 1f));
                directions.Add(new Vector(-1f, -1f, -1f));
            }else{
                directions.Add(new Vector(1f, 1f));
                directions.Add(new Vector(1f, -1f));
                directions.Add(new Vector(-1f, 1f));
                directions.Add(new Vector(-1f, -1f));
            }
        }else{
            foreach (Vector vect in vects){
                //Here we make sure that the vect has the correct number of dimensions
                if (vect.isThreeDimensional != thirdDimension){
                    throw new InvalidOperationException();
                }
                directions.Add(vect);
            }
        }
    }

    public Grid(bool hasThirdDimension, int _seed, Vector[] vects = null) : this(hasThirdDimension, vects) {
        this.seed = _seed;
    }

    //This method will allow setting a custom grid size
    public void SetSize(float _sizeX, float _sizeY, float _sizeZ = 0){
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
        Vector vectToTopLeft = new Vector(relativePosX, sizeY - relativePosY);
        Vector vectToBottomLeft = relativeVect;
        Vector vectToTopRight = new Vector(sizeX - relativePosX, sizeY - relativePosY);
        Vector vectToBottomRight = new Vector(sizeX - relativePosX, relativePosY);

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
        //Find pos inside grid square
        float relativePosX = xA % sizeX;
        float relativePosY = yA % sizeY;
        float relativePosZ = zA % sizeZ;
        Vector relativeVect = new Vector(relativePosX, relativePosY, relativePosZ);

        //Find square's pos on grid
        float leftLine = xA - relativePosX;
        float bottomLine = yA - relativePosY;
        float frontLine = zA - relativePosZ;
        float topLine = bottomLine + sizeY;
        float rightLine = leftLine + sizeX;
        float backLine = frontLine + sizeZ;

        //Find random vects at grid corners
        Vector topLeftBackVect = FindCornerVect(leftLine, topLine, backLine);
        Vector bottomLeftBackVect = FindCornerVect(leftLine, bottomLine, backLine);
        Vector topRightBackVect = FindCornerVect(rightLine, topLine, backLine);
        Vector bottomRightBackVect = FindCornerVect(rightLine, bottomLine, backLine);

        Vector topLeftFrontVEct = FindCornerVect(leftLine, topLine, frontLine);
        Vector bottomLeftFrontVect = FindCornerVect(leftLine, bottomLine, frontLine);
        Vector topRightFrontVect = FindCornerVect(rightLine, topLine, frontLine);
        Vector bottomRightFrontVect = FindCornerVect(rightLine, bottomLine, frontLine);

        //Define vects from corner to point
        Vector vectToTopLeftBack = new Vector(relativePosX, sizeY - relativePosY, sizeZ - relativePosZ);
        Vector vectToBottomLeftBack = (relativePosX, relativePosY, sizeZ - relativePosZ);
        Vector vectToTopRightBack = new Vector(sizeX - relativePosX, sizeY - relativePosY, sizeZ - relativePosZ);
        Vector vectToBottomRightBack = new Vector(sizeX - relativePosX, relativePosY, sizeZ - relativePosZ);

        Vector vectToTopLeftFront = new Vector(relativePosX, sizeY - relativePosY, relativePosZ);
        Vector vectToBottomLeftFront = (relativePosX, relativePosY, relativePosZ);
        Vector vectToTopRightFront = new Vector(sizeX - relativePosX, sizeY - relativePosY, relativePosZ);
        Vector vectToBottomRightFront = new Vector(sizeX - relativePosX, relativePosY, relativePosZ);

        //Generate random values
        float topLeftBackVal = Vector.Dot(topLeftBackVect, vectToTopLeftBack);
        float bottomLeftBackVal = Vector.Dot(bottomLeftBackVect, vectToBottomLeftBack);
        float topRightBackVal = Vector.Dot(topRightBackVect, vectToTopRightBack);
        float bottomRightBackVal = Vector.Dot(bottomRightBackVect, vectToBottomRightBack);

        float topLeftFrontVal = Vector.Dot(topLeftFrontVect, vectToTopLeftFront);
        float bottomLeftFrontVal = Vector.Dot(bottomLeftFrontVect, vectToBottomLeftFront);
        float topRightFrontVal = Vector.Dot(topRightFrontVect, vectToTopRightFront);
        float bottomRightFrontVal = Vector.Dot(bottomRightFrontVect, vectToBottomRightFront);

        //Interpolate the values using the pos as weight, generating the final value
        float topBackVal = Interp(topLeftBackVal, topRightBackVal, relativePosX);
        float bottomBackVal = Interp(bottomLeftBackVal, bottomRightBackVal, relativePosX);
        float backVal = Interp(bottomBackVal, topBackVal, relativePosY);

        float topFrontVal = Interp(topLeftFrontVal, topRightFrontVal, relativePosX);
        float bottomFrontVal = Interp(bottomLeftFrontVal, bottomRightFrontVal, relativePosX);
        float frontVal = Interp(bottomBackVal, topBackVal, relativePosY);

        return Interp(frontVal, backVal, relativePosZ);
    }

    private float Interp(float valueOne, float valueTwo, float weight){
        return (valueTwo - valueOne) * weight + valueOne;
    }

    //This paring function will combine the coords of the intersections of our grid, so we can set the seed for the rng and get consistent output for each seed
    private int Pair(int numA, int numB){
        return ((numA * numA) + (numA * 3) + (2 * (numA + numB)) + numB + (numB * numB)) / 2;
    }

    private Vector FindCornerVect(float coordX, float coordY, float coordZ = 0){
        int xIntersectionNumber = (int)coordX / (int)sizeX;
        int yIntersectionNumber = (int)coordY / (int)sizeY;
        if(this.thirdDimension){
            int zIntersectionNumber = (int)coordZ / (int)sizeZ;

            int _seed = Pair(zIntersectionNumber, Pair(xIntersectionNumber, yIntersectionNumber)) + seed;
            rng = new Random(_seed);

            int vectNum = rng.Next(directions.Count);
            return directions[vectNum];
        }else{
            int _seed = Pair(xIntersectionNumber, yIntersectionNumber) + seed;
            rng = new Random(_seed);

            int vectNum = rng.Next(directions.Count);
            return directions[vectNum];
        }
    }
}

public struct Vector
{
    public float x;
    public float y;
    public float z;

    public bool isThreeDimensional;

    public Vector(float _x, float _y){
        this.x = _x;
        this.y = _y;
        this.z = 0;
        this.isThreeDimensional = false;      
    }

    public Vector(float _x, float _y, float _z) : this(_x, _y){
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