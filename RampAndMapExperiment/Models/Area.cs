using BaseExperimento.Auxiliary;
using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RampAndMapExperiment.Models
{
    public enum TypeOfArea { Floor = 0, InsideLimit = 1, OuterLimit = 2, InsideActivate = 3 }

    public enum FloorIs { NoneFloor = 0, FloorIsXZ = 1, FloorIsXY = 2 }

    public class Area
    {
        #region Functional Attributes
        public static List<Area> l_areas = new List<Area>();
        #endregion

        #region Attributes
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (l_areas.Where(c => c.Name == value).ToList().Count == 0)
                {
                    l_areas.Add(this);
                }
                name = value;
            }
        }

        private Dictionary<string, Vector3> dic_points = new Dictionary<string, Vector3>();
        [JsonConverter(typeof(DictionaryConverterStringVector3))]
        public Dictionary<string, Vector3> Dic_Points { get => dic_points; set => dic_points = value; }

        public FloorIs isFloor = FloorIs.NoneFloor;
        #endregion

        #region Constructores
        [JsonConstructor]
        public Area(string name = "", Vector3 nw = new Vector3(), Vector3 sw = new Vector3(), Vector3 ne = new Vector3(), Vector3 se = new Vector3(), FloorIs isFloor = FloorIs.FloorIsXZ)
        {
            this.Name = name;
            this.isFloor = isFloor;
            this.Dic_Points = new Dictionary<string, Vector3>
            {
                { "NW", nw },
                { "SW", sw },
                { "NE", ne },
                { "SE", se }
            };
        }

        public Area(Dictionary<string, Vector3> collection)
        {
            this.Dic_Points = collection;
        }
        #endregion

        #region Functions
        public bool MoveArea(Vector3 modifier)
        {
            try
            {
                this.Dic_Points["NW"] += modifier;
                this.Dic_Points["SW"] += modifier;
                this.Dic_Points["NE"] += modifier;
                this.Dic_Points["SE"] += modifier;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error bool MoveArea(Vector3): " + ex.Message);
                return false;
            }
        }

        public static float DistanciaEntreVectores(Vector3 a, Vector3 b)
        {
            float valueX = MathF.Pow((a.X - b.X), 2);
            float valueY = MathF.Pow((a.Y - b.Y), 2);
            float valueZ = MathF.Pow((a.Z - b.Z), 2);
            float valueToSquareRoot = MathF.Sqrt((valueX + valueY + valueZ));
            return valueToSquareRoot;
        }

        public static bool IsActionFromEntityTrue(Vector3 entityPosition, Area areaChecked, TypeOfArea toa = TypeOfArea.Floor)
        {
            try
            {
                switch (toa)
                {
                    case TypeOfArea.Floor:
                    case TypeOfArea.InsideLimit:
                    case TypeOfArea.InsideActivate:
                        return IsInside(entityPosition, areaChecked);
                        break;
                    case TypeOfArea.OuterLimit:
                        return !IsInside(entityPosition, areaChecked);
                        break;
                    default:
                        Console.Out.WriteLine("Error IsActionFromEntityTrue(Vector3, TypeOfArea): Switch Default Triggered toa: " + toa + " entityPosition: " + entityPosition.ToString());
                        return false;
                        //The break is there, but is not accesed anyway
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error IsActionFromEntityTrue(Vector3, TypeOfArea): " + ex.Message);
                return false;
            }
        }

        public static bool IsInside(Vector3 entityPosition, Area areaChecked)
        {
            Vector2 CornerNW;
            Vector2 CornerSE;
            float entityPositionY;

            if (areaChecked.isFloor == FloorIs.FloorIsXZ)
            {
                CornerNW = new Vector2(areaChecked.dic_points["NW"].X, areaChecked.dic_points["NW"].Z);
                CornerSE = new Vector2(areaChecked.dic_points["SE"].X, areaChecked.dic_points["SE"].Z);
                entityPositionY = entityPosition.Z;
            }
            else if (areaChecked.isFloor == FloorIs.FloorIsXY)
            {
                CornerNW = new Vector2(areaChecked.dic_points["NW"].X, areaChecked.dic_points["NW"].Y);
                CornerSE = new Vector2(areaChecked.dic_points["SE"].X, areaChecked.dic_points["SE"].Y);
                entityPositionY = entityPosition.Y;
            }
            else
            {
                return false;
            }

            float W = CornerNW.X;
            float E = CornerSE.X;
            float N = CornerNW.Y;
            float S = CornerSE.Y;

            //Todo esto es válido SOLO cuando Z>0 es Norte y X<0 es Oeste
            bool isWLocTrue = false;
            if (entityPosition.X < W)
            {
                isWLocTrue = true;
            }


            bool isELocTrue = false;
            if (entityPosition.X > E)
            {
                isELocTrue = true;
            }


            bool isNLocTrue = false;
            if (entityPositionY < N)
            {
                isNLocTrue = true;
            }


            bool isSLocTrue = false;
            if (entityPositionY > S)
            {
                isSLocTrue = true;
            }


            if (isWLocTrue && isELocTrue && isNLocTrue && isSLocTrue)
            {
                return true;
            }
            return false;
        }


        #region Math Function (Do not detect for some reason) TODO: Retest in the future THEY ARE MAYBIE GOOD, BUT TRY WITH WORLDPOSITION INSTEAD OF LOCALPOSITION
        // Function to check if a point is inside a 3D polygon.
        //public static bool IsPointInPolygon(Vector3 point, List<Vector3> polygonVertices)
        public static bool IsPointIn3DArea(Vector3 point, List<Vector3> polygonVertices)
        {
            // Calculate the normal vector of the polygon's plane.
            Vector3 normal = CalculatePolygonNormal(polygonVertices);

            // Calculate vectors from the point to each polygon vertex.
            foreach (Vector3 vertex in polygonVertices)
            {
                Vector3 vectorToVertex = vertex - point;

                // Check the dot product between the normal vector and the vector to the vertex.
                float dotProduct = Vector3.Dot(normal, vectorToVertex);

                // If any dot product has a different sign, the point is outside the polygon.
                if (dotProduct < 0)
                {
                    return false;
                }
            }

            // If all dot products have the same sign, the point is inside or on the polygon.
            return true;
        }

        // Function to calculate the normal vector of a 3D polygon.
        private static Vector3 CalculatePolygonNormal(List<Vector3> vertices)
        {
            if (vertices.Count < 3)
            {
                throw new ArgumentException("Polygon must have at least three vertices.");
            }

            // Calculate the cross product of two edges of the polygon.
            Vector3 edge1 = vertices[1] - vertices[0];
            Vector3 edge2 = vertices[2] - vertices[0];
            Vector3 normal = Vector3.Cross(edge1, edge2);
            normal.Normalize();
            return normal;
        }

        public static bool IsPointIn2DArea(Vector3 position, Area areaChecked, FloorIs isFloor = FloorIs.FloorIsXZ)
        {
            // Define the coordinates of the rectangle vertices and point P
            double x1 = 0, y1 = 0;
            double x2 = 5, y2 = 0;
            double x3 = 5, y3 = 3;
            double x4 = 0, y4 = 3;
            double x = 2, y = 2; // Change these to the coordinates of your point P

            if (isFloor == FloorIs.FloorIsXZ)
            {
                // Define the coordinates of the rectangle vertices and point P
                x1 = areaChecked.dic_points["NW"].X;
                y1 = areaChecked.dic_points["NW"].Z;

                x2 = areaChecked.dic_points["NE"].X;
                y2 = areaChecked.dic_points["NE"].Z;

                x3 = areaChecked.dic_points["SW"].X;
                y3 = areaChecked.dic_points["SW"].Z;

                x4 = areaChecked.dic_points["SE"].X;
                y4 = areaChecked.dic_points["SE"].Z;

                x = position.X;
                y = position.Z; // Change these to the coordinates of your point P
            }
            else if (isFloor == FloorIs.FloorIsXY)
            {
                // Define the coordinates of the rectangle vertices and point P
                x1 = areaChecked.dic_points["NW"].X;
                y1 = areaChecked.dic_points["NW"].Y;

                x2 = areaChecked.dic_points["NE"].X;
                y2 = areaChecked.dic_points["NE"].Y;

                x3 = areaChecked.dic_points["SW"].X;
                y3 = areaChecked.dic_points["SW"].Y;

                x4 = areaChecked.dic_points["SE"].X;
                y4 = areaChecked.dic_points["SE"].Y;

                x = position.X;
                y = position.Y; // Change these to the coordinates of your point P
            }
            else
            {
                return false;
            }

            // Calculate the sum of triangle areas
            double areaAPD = CalculateTriangleArea(x1, y1, x4, y4, x, y);
            double areaDPC = CalculateTriangleArea(x4, y4, x3, y3, x, y);
            double areaCPB = CalculateTriangleArea(x3, y3, x2, y2, x, y);
            double areaPBA = CalculateTriangleArea(x, y, x1, y1, x2, y2);

            // Calculate the area of the rectangle
            double rectangleArea = CalculateRectangleArea(x1, y1, x2, y2, x3, y3, x4, y4);

            // Check if the point is inside, on, or outside the rectangle
            if (Math.Abs(areaAPD + areaDPC + areaCPB + areaPBA - rectangleArea) < 1e-9)
            {
                if (areaAPD == 0 || areaDPC == 0 || areaCPB == 0 || areaPBA == 0)
                {
                    Console.WriteLine("Point P is on the rectangle.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Point P is inside the rectangle.");
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Point P is outside the rectangle.");
            }
            return false;
        }

        static double CalculateTriangleArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Math.Abs(0.5 * ((x1 * (y2 - y3)) + (x2 * (y3 - y1)) + (x3 * (y1 - y2))));
        }

        static double CalculateRectangleArea(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            return Math.Abs((x1 * y2 - x2 * y1 + x2 * y3 - x3 * y2 + x3 * y4 - x4 * y3 + x4 * y1 - x1 * y4) * 0.5);
        }

        public static bool InsideArea(Vector3 position, Area areaChecked)
        {
            try
            {
                Vector3 NW = Vector3.Zero;
                Vector3 NE = Vector3.Zero;
                Vector3 SW = Vector3.Zero;
                Vector3 SE = Vector3.Zero;

                foreach (KeyValuePair<string, Vector3> point in areaChecked.Dic_Points)
                {
                    if (point.Key == "NW")
                    {
                        NW = point.Value;
                    }

                    if (point.Key == "NE")
                    {
                        NE = point.Value;
                    }

                    if (point.Key == "SW")
                    {
                        SW = point.Value;
                    }

                    if (point.Key == "SE")
                    {
                        SE = point.Value;
                    }
                }

                Vector3 SouthLine = new Vector3(); //South + SouthVariance;
                Vector3 NorthLine = new Vector3(); //North + NorthVariance;
                Vector3 EastLine = new Vector3(); //East + EastVariance;
                Vector3 WestLine = new Vector3(); //West + WestVariance;

                //EastLine.X = (((NE.X - SE.X) / 2) + SE.X);
                //WestLine.X = (((NW.X - SW.X) / 2) + SW.X);

                EastLine = ((NE + SE) / 2);
                WestLine = ((NW + SW) / 2);
                SouthLine = ((SW + SE) / 2);
                NorthLine = ((NW + NE) / 2);

                bool sur = false;
                bool norte = false;
                bool este = false;
                bool oeste = false;

                if (areaChecked.isFloor == FloorIs.FloorIsXZ)
                {
                    //SouthLine.Z = (((SE.Z - SW.Z) / 2) + SW.Z);
                    //NorthLine.Z = (((NE.Z - NW.Z) / 2) + NW.Z);

                    //Si cae dentro de la zona, no se mueve
                    if (position.Z >= SouthLine.Z) //South Limit
                    {
                        if ((position).Z < NorthLine.Z)
                        {
                            //Console.WriteLine("Desde el sur entro");
                            sur = true;

                        }
                    }

                    if ((position).Z <= NorthLine.Z) //North Limit
                    {
                        if ((position).Z > SouthLine.Z)
                        {
                            //Console.WriteLine("Desde el norte entro");
                            norte = true;
                        }
                    }
                }

                if (areaChecked.isFloor == FloorIs.FloorIsXY)
                {
                    //SouthLine.Y = (((SE.Y - SW.Y) / 2) + SW.Y);
                    //NorthLine.Y = (((NE.Y - NW.Y) / 2) + NW.Y);

                    //Si cae dentro de la zona, no se mueve
                    if (position.Y >= SouthLine.Y) //South Limit
                    {
                        if ((position).Y < NorthLine.Y)
                        {
                            //Console.WriteLine("Desde el sur entro");
                            sur = true;

                        }
                    }

                    if ((position).Y <= NorthLine.Y) //North Limit
                    {
                        if ((position).Y > SouthLine.Y)
                        {
                            //Console.WriteLine("Desde el norte entro");
                            norte = true;
                        }
                    }
                }

                if ((position).X <= EastLine.X) //East Limit
                {
                    if ((position).X > WestLine.X)
                    {
                        //Console.WriteLine("Desde el este entro");
                        este = true;
                    }
                }

                if ((position).X >= WestLine.X) //West Limit
                {
                    if ((position).X < EastLine.X)
                    {
                        //Console.WriteLine("Desde el oeste entro");
                        oeste = true;
                    }
                }

                if (norte && sur && este && oeste)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error InsideArea(Vector3, Area): " + ex.Message);
                return false;
            }
        }
        #endregion
        #endregion
    }
}
