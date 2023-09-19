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
                player.Transform.Position.Y = NuevaAltura(
                    player.Transform.Position.Z, 
                    new Vector2(point2.Transform.Position.Z, point2.Transform.Position.Y),
                    new Vector2(point3.Transform.Position.Z, point3.Transform.Position.Y)
                    );
                DebugText.Print("NuevaAltura: "+player.Transform.Position.Y,new Int2(100,100));
            }
        }

        //función lineal 3D (o Recta), Perfeccionaría la otra pues podríamos usar rampas diagonales y similar
        public static float NuevaAltura3D(float positionOnTheRamp, Vector3 pointa, Vector3 pointb)
        {
            //Recta (Obtiene el punto desconocido)
            //<x-x0,y-y0,z-z0> = t * <a,b,c>
            //<x-x0,y-y0,z-z0> = <a*t,b*t,c*t>
            //<x,y,z> = <x0+a*t,y0+b*t,z0+c*t>

            //Siendo: <x,y,z> un vector3
            //        PoP = <x-x0,y-y0,z-z0> también es un Vector3

            //Desde paralela
            Vector3 recta = new Vector3(pointa.X + (pointb.X * positionOnTheRamp), pointa.Y + (pointb.Y * positionOnTheRamp), pointa.Z + (pointb.Z * positionOnTheRamp));
            return recta.Y;
        }

        //Intentando con función lineal, probablemente lo que debí intentar desde el principio . . . ¡¡¡FUNCIONA PERFECTAMENTE!!!
        public static float NuevaAltura(float positionOnTheRamp,Vector2 pointa, Vector2 pointb)
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
