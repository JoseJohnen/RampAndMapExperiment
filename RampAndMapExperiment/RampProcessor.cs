using RampAndMapExperiment.Models;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;

namespace RampAndMapExperiment
{
    public class RampProcessor : SyncScript
    {
        public Entity player;

        public Entity point1;
        public Entity point2;
        public Entity point3;
        public Entity point4;

        public Entity point5;
        public Entity point6;

        public Entity point7;

        public List<Entity> l_Points = new List<Entity>();

        public override void Start()
        {
            base.Start();
            l_Points = new List<Entity>()
            {
                point1,
                point2,
                point3,
                point4,
            };

            point5.Transform.Position = ObtenerPuntoMedio(point1, point2);
            point6.Transform.Position = ObtenerPuntoMedio(point3, point4);

            point7.Transform.Position = ObtenerPuntoMedio(point5, point6);
        }

        public override void Update()
        {
            if (Input.IsKeyDown(Stride.Input.Keys.K))
            {
                //player.Transform.Position.Y = NuevaAltura(
                //    player.Transform.Position.Z, 
                //    new Vector2(point2.Transform.Position.Z, point2.Transform.Position.Y),
                //    new Vector2(point3.Transform.Position.Z, point3.Transform.Position.Y)
                //    );
                //player.Transform.Position.Y = NuevaAltura3D(
                //    point5.Transform.Position,
                //    point6.Transform.Position,
                //    player.Transform.Position
                //    );
                //DebugText.Print("NuevaAltura: " + player.Transform.Position.Y, new Int2(100, 100));
                //UpperLeft Corner
                //UpperRight Corner
                //BottomRight Corner
                //BottomLeft Corner
                RampCalculation(point5.Transform.Position,point6.Transform.Position,ref player.Transform.Position,point4.Transform.Position,point3.Transform.Position,point2.Transform.Position,point1.Transform.Position);
                DebugText.Print("NuevaAltura: " + player.Transform.Position.Y, new Int2(100, 100));
            }
        }

        public bool RampCalculation(Vector3 pointa, Vector3 pointb, ref Vector3 characterPos, Vector3 UpperLeftCorner, Vector3 UpperRightCorner, Vector3 BottomRightCorner, Vector3 BottomLeftCorner)
        {
            Area areaRamp = new Area("AreaRamp", UpperLeftCorner, BottomLeftCorner, UpperRightCorner, BottomRightCorner);

            Vector3 fakeCharacter = characterPos;
            DebugText.Print("Distance: "
                    + Area.DistanciaEntreVectores(
                        characterPos,
                        fakeCharacter
                        )
                    , new Int2(100, 120));

            //If its not inside the area of the ramp, it doesn't change the height of the character
            DebugText.Print("Is In Area?: "
                + Area.IsActionFromEntityTrue(characterPos, areaRamp)
                , new Int2(100, 140));
            if (!Area.IsActionFromEntityTrue(characterPos, areaRamp))
            {
                return false;
            }

            //Evaluate if the character is over or under the ramp, if over it for one or less
            //it will have the ramp height
            fakeCharacter.Y = NuevaAltura3D(pointa, pointb, fakeCharacter);
            if (characterPos.Y >= fakeCharacter.Y)
            {
                characterPos.Y = NuevaAltura3D(pointa, pointb, characterPos);
            }

            return true;
        }

        //Metodo obtenido y adaptado desde ClosestPointFromTheLineExperiment: FUNCIONA PERFECTO
        public static float NuevaAltura3D(Vector3 pointa, Vector3 pointb, Vector3 characterPos)
        {
            // Calculate vectors AB and AC
            double[] AB = { pointb.X - pointa.X, pointb.Y - pointa.Y, pointb.Z - pointa.Z };
            double[] AC = { characterPos.X - pointa.X, characterPos.Y - pointa.Y, characterPos.Z - pointa.Z };

            // Calculate the dot product of AB
            double dotAB = DotProduct(AB, AB);

            // Calculate the t-parameter for the projection
            double t = DotProduct(AC, AB) / dotAB;

            // Ensure that t is clamped between 0 and 1
            t = Math.Clamp(t, 0, 1);

            // Calculate the closest point D on the line
            double[] D = { pointa.X + t * AB[0], pointa.Y + t * AB[1], pointa.Z + t * AB[2] };

            //return new Tuple<double, double, double>(D[0], D[1], D[2]);
            //return new Stride.Core.Mathematics.Vector3(
            Vector3 result = new Stride.Core.Mathematics.Vector3(
                Convert.ToSingle(D[0]),
                Convert.ToSingle(D[1]),
                Convert.ToSingle(D[2])
                );
            return result.Y;
        }

        //Metodo obtenido y adaptado desde ClosestPointFromTheLineExperiment: FUNCIONA PERFECTO
        public static double DotProduct(double[] vector1, double[] vector2)
        {
            double result = 0;
            for (int i = 0; i < vector1.Length; i++)
            {
                result += vector1[i] * vector2[i];
            }
            return result;
        }

        //Intentando con función lineal, probablemente lo que debí intentar desde el principio . . . ¡¡¡FUNCIONA PERFECTAMENTE!!!
        public static float NuevaAltura(float positionOnTheRamp, Vector2 pointa, Vector2 pointb)
        {
            //Pendiente
            //m = y2 - y1
            //    -------
            //    x2 - x1

            //Interseccion
            //b = y - (m * x)

            //Altura (La ecuación lineal por excelencia)
            //y = mx+b

            float pendiente = (pointb.Y - pointa.Y) / (pointb.X - pointa.X);
            float interseccion = (pointa.Y - (pendiente * pointa.X));
            float altura = (pendiente * positionOnTheRamp) + interseccion;
            return altura;
        }

        public static float DistanciaEntreVectores(Vector2 a, Vector2 b)
        {
            float valueX = MathF.Pow((a.X - b.X), 2);
            float valueY = MathF.Pow((a.Y - b.Y), 2);
            float valueToSquareRoot = MathF.Sqrt((valueX + valueY));
            return valueToSquareRoot;
        }

        public static Vector3 ObtenerPuntoMedio(Entity puntoA, Entity puntoB)
        {
            Vector3 valueToReturn = new Vector3();
            Vector3.Lerp(ref puntoA.Transform.Position, ref puntoB.Transform.Position, 0.5f, out valueToReturn);
            return valueToReturn;
        }
    }
}
