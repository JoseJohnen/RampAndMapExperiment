using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Media3D;

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
                point2 ,
                point3 ,
                point4 ,
            };

            point5.Transform.Position = ObtenerPuntoMedio(point1, point2);
            point6.Transform.Position = ObtenerPuntoMedio(point3, point4);

            point7.Transform.Position = ObtenerPuntoMedio(point5, point6);
        }

        public override void Update()
        {
            //RampProcessor.NuevaPosicionEnRampa(ref player, point6.Transform.Position, point5.Transform.Position);
            if (Input.HasPressedKeys)
            {
                //player.Transform.Position = RampProcessor.Interpolate(point5.Transform.Position, point6.Transform.Position, player.Transform.Position.Y);
                RampProcessor.NuevaPosicionEnRampa(ref player, point6.Transform.Position, point5.Transform.Position);
                //RampProcessor.CalculateY(point5.Transform.Position, point6.Transform.Position, ref player.Transform.Position);
                //RampProcessor.CalculateYImproved(point5.Transform.Position, point6.Transform.Position, ref player.Transform.Position);
                //RampProcessor.CalculateYInSquaredRamp(point1.Transform.Position, point2.Transform.Position, point3.Transform.Position, point4.Transform.Position, ref player.Transform.Position);
            }
        }

        //Doesn't seem to work properly
        public static float CalculateYInSquaredRamp(Vector3 beginning1, Vector3 beginning2, Vector3 end1, Vector3 end2, ref Vector3 between)
        {
            // Check if the provided vectors define a valid squared ramp
            /*if (!IsSquaredRampValid(beginning1, beginning2, end1, end2))
            {
                throw new ArgumentException("Invalid squared ramp definition.");
            }*/

            // Calculate the Y value between "Beginning1" and "Beginning2" based on the X-coordinate of "between"
            float tX = (between.X - beginning1.X) / (beginning2.X - beginning1.X);
            float yInRamp = MathUtil.Lerp(beginning1.Y, beginning2.Y, tX);

            // Calculate the Y value within the squared ramp by interpolating along the Z-axis
            float tZ = (between.Z - beginning1.Z) / (end1.Z - beginning1.Z);
            //float interpolatedY = MathUtil.Lerp(yInRamp, MathUtil.Lerp(end1.Y, end2.Y, tX), tZ);
            between.Y = MathUtil.Lerp(yInRamp, MathUtil.Lerp(end1.Y, end2.Y, tX), tZ);

            //return interpolatedY;
            return between.Y;
        }

        // Helper function to check if the provided vectors define a valid squared ramp
        private static bool IsSquaredRampValid(Vector3 beginning1, Vector3 beginning2, Vector3 end1, Vector3 end2)
        {
            return beginning1.X == beginning2.X && end1.X == end2.X &&
                   beginning1.Z == end1.Z && beginning2.Z == end2.Z &&
                   beginning1.Y <= beginning2.Y && end1.Y >= end2.Y;
        }

        public static void NuevaPosicionEnRampa(ref Entity plyr, Vector3 topPosition, Vector3 bottomPosition)
        {
            //TODO, si el resultado es negativo, le inviertes el signo
            //TODO2, Evaluar si con Elevación a 2 y posterior raíz cuadrada de dos se soluciona el tema de "el orden de los factores no altera el producto" en este caso
            //Obtienes la distancia de la altura entre el punto mas alto y el punto mas bajo de la rampa
            float distanceYTopBottom = topPosition.Y - bottomPosition.Y;
            if(distanceYTopBottom < 0)
            {
                distanceYTopBottom *= -1;
            }

            //Preparas los Top, Player y Bottom como Vectores 2 para el resto de los puntos de comparación
            Vector2 topVector2 = new Vector2(topPosition.X, topPosition.Z);
            Vector2 plyrVector2 = new Vector2(plyr.Transform.Position.X, plyr.Transform.Position.Z);
            Vector2 bottomVector2 = new Vector2(bottomPosition.X, bottomPosition.Z);

            //Averiguas a cuantas distancia en las dimensiones restantes se encuentra el player con respecto al punto mas alto
            float distanciaTopPlayer = RampProcessor.DistanciaEntreVectores(topVector2,plyrVector2);

            //Averiguas a cuantas distancia en las dimensiones restantes se encuentra el player con respecto al punto mas bajo
            float distanciaBottomPlayer = RampProcessor.DistanciaEntreVectores(bottomVector2, plyrVector2);

            //Con ambas distancias, las sumas y sacas el Total Variable, que representa cuanto hay entre el punto mas bajo y el mas alto pasando por el player
            float TotalVariable = distanciaTopPlayer + distanciaBottomPlayer;

            //Tomas distanciaBottomPlayer, y sobre ese sacas el porcentaje con respecto a TotalVariable (Regla de tres)
            float PorcentualTotalVariableDistanciaBottomPlayer = ((distanciaBottomPlayer * 100) / TotalVariable);

            //Teniendo el Porcentual anteriormente ya mencionado, sacas cuanto es el valor en sólido del distanceYTopBottom, con regla de 3
            float distanceYPlayerBetween = ((PorcentualTotalVariableDistanciaBottomPlayer * distanceYTopBottom) / 100);

            //Teniendo este nuevo valor, procedes a utilizarlo y ver
            plyr.Transform.Position.Y = (bottomPosition.Y+distanceYPlayerBetween);
        }

        public static float DistanciaEntreVectores(Vector2 a, Vector2 b)
        {
            float valueX = MathF.Pow((a.X - b.X), 2);
            float valueY = MathF.Pow((a.Y - b.Y), 2);
            float valueToSquareRoot = MathF.Sqrt((valueX + valueY));
            return valueToSquareRoot;
        }

        //Malo
        public static void NuevaPosicionEnRampa2(ref Entity plyr, Vector3 topPosition, Vector3 bottomPosition)
        {
            //Si la rampa esta en nivel 0, (y player Y también) debe estar un poco mas arriba de la misma, player Y NUNCA DEBE SER 0
            if(plyr.Transform.Position.Y == 0)
            {
                plyr.Transform.Position.Y = (bottomPosition.Y + 0.2f);
            }

            Vector3 position = topPosition - bottomPosition;
            float temp = ((plyr.Transform.Position.Y * 100) / position.Y);
            float temp2 = temp / 100;
            plyr.Transform.Position.Y *= temp2;
        }

        //Insuficiente
        public static Vector3 Interpolate(Vector3 beginning, Vector3 end, float t)
        {
            // Ensure t is within the range [0, 1]
            t = MathUtil.Clamp(t, 0f, 1f);

            // Use linear interpolation to calculate the intermediate point
            return Vector3.Lerp(beginning, end, t);
        }

        //Funciona
        public static float CalculateY(Vector3 beginning, Vector3 end, ref Vector3 between)
        {
            // Calculate the total distance between Beginning and End
            float totalDistance = Vector3.Distance(beginning, end);

            // Calculate the distance between Beginning and Between
            float distanceToBetween = Vector3.Distance(beginning, between);

            // Calculate the ratio of the distance to Between over the total distance
            float t = distanceToBetween / totalDistance;

            // Use linear interpolation to calculate the intermediate Y value
            //float interpolatedY = MathUtil.Lerp(beginning.Y, end.Y, t);
            between.Y = MathUtil.Lerp(beginning.Y, end.Y, t);

            //return interpolatedY;
            return between.Y;
        }

        //Funciona
        public static float CalculateYImproved(Vector3 beginning, Vector3 end, ref Vector3 between)
        {
            // Calculate the total distance between Beginning and End
            float totalDistance = Vector3.Distance(beginning, end);

            // Check if the totalDistance is close to zero (avoid division by zero)
            if (Math.Abs(totalDistance) < float.Epsilon)
            {
                throw new ArgumentException("Beginning and End are too close to each other.");
            }

            // Calculate the distance between Beginning and Between
            float distanceToBetween = Vector3.Distance(beginning, between);

            // Calculate the ratio of the distance to Between over the total distance
            float t = distanceToBetween / totalDistance;

            // Use linear interpolation to calculate the intermediate Y value
            //float interpolatedY = MathUtil.Lerp(beginning.Y, end.Y, t);
            between.Y = MathUtil.Lerp(beginning.Y, end.Y, t);

            //return interpolatedY;
            return between.Y;
        }

        public static Vector3 ObtenerPuntoMedio(Entity puntoA, Entity puntoB)
        {
            Vector3 valueToReturn = new Vector3();
            /*if (puntoA.Transform.Position.X < 0 || puntoB.Transform.Position.X < 0)
            {
                valueToReturn.X = (puntoA.Transform.Position.X + puntoB.Transform.Position.X);
            }
            else
            {
                valueToReturn.X = (puntoA.Transform.Position.X - puntoB.Transform.Position.X);
            }
            
            if (puntoA.Transform.Position.Y < 0 || puntoB.Transform.Position.Y < 0)
            {
                valueToReturn.Y = (puntoA.Transform.Position.Y + puntoB.Transform.Position.Y);
            }
            else
            {
                valueToReturn.Y = (puntoA.Transform.Position.Y - puntoB.Transform.Position.Y);
            }

            if (puntoA.Transform.Position.Z < 0 || puntoB.Transform.Position.Z < 0)
            {
                valueToReturn.Z = (puntoA.Transform.Position.Z + puntoB.Transform.Position.Z);
            }
            else
            {
                valueToReturn.Z = (puntoA.Transform.Position.Z - puntoB.Transform.Position.Z);
            }*/

            Vector3.Lerp(ref puntoA.Transform.Position, ref puntoB.Transform.Position, 0.5f, out valueToReturn);
            return valueToReturn;
        }

    }
}
