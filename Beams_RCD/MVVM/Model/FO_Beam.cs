using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beams_RCD.MVVM.Model
{
    public class FO_Beam
    {

        #region Properties
        public Element Beam { get; private set; }
        public double BeamWidth { get; private set; }

        public Grid CenterGrid { get; private set; }

        public List<Grid> ParallelGrids { get; private set; }

        public List<Face> RightAndLeftFaces { get; private set; }
        public List<Face> TopAndBottomFaces { get; private set; }

        public Line BeamDimensionLine { get; private set; }
        public Line GridBeamDimensionLine { get; private set; }


        public XYZ Leader1_Position { get; private set; }
        public XYZ Leader2_Position { get; private set; }



        #endregion


        #region Constructor

        public FO_Beam(Element _Beam, List<Grid> All_Grids)
        {
            Beam = _Beam;
            ParallelGrids = GetParallelGrids(_Beam, All_Grids);
            CenterGrid = GetCenterGrid(_Beam, ParallelGrids);
            RightAndLeftFaces = GetRightAndLeftFaces(_Beam);

            TopAndBottomFaces = GetTopAndBotFaces(_Beam);
            BeamDimensionLine = GetDimensionLineForBeamDimension(_Beam);
            GridBeamDimensionLine = GetDimensionLineForGridDimension(_Beam);

            Leader1_Position = GetLeaderPosition(_Beam, -1.5);
            Leader2_Position = GetLeaderPosition(_Beam, 1.5);
            BeamWidth = GetWidth(_Beam);

        }

        #endregion


        #region Methods


        private double GetWidth(Element _Beam)
        {
            double Width = 0;
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            try
            {
                Width = Command.Doc.GetElement(_Beam.GetTypeId()).LookupParameter("b").AsDouble();

            }
            catch
            {
                Width = 0;
            }
            return Width;

        }

        private Grid GetCenterGrid(Element _Beam, List<Grid> Parallel_Grids)
        {
            Grid Result = null;

            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;

            double Depth = Command.Doc.GetElement(_Beam.GetTypeId()).LookupParameter("h").AsDouble();
            double Width = Command.Doc.GetElement(_Beam.GetTypeId()).LookupParameter("b").AsDouble();

            //Get The Grid that is inside the Beam (CenterGrid)
            double MinDistance = 0;
            if (Parallel_Grids != null && Parallel_Grids.Count > 0)
            {
                foreach (Grid item in Parallel_Grids)
                {
                    XYZ PointOnGrid = item.Curve.GetEndPoint(0);
                    MinDistance = Helper.GetDistanceBetweenPointAndLine(PointOnGrid, BeamCurve as Line);
                    if (MinDistance < Width + 0.00001)
                    {
                        Result = item;
                        break;
                    }
                }
            }

            return Result;
        }

        private List<Grid> GetParallelGrids(Element _Beam, List<Grid> All_Grids)
        {
            List<Grid> Result = new List<Grid>();

            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            XYZ P1_Curve = BeamCurve.GetEndPoint(0);
            XYZ P2_Curve = BeamCurve.GetEndPoint(1);

            double BeamSlope = Helper.CalculateSlope(P1_Curve, P2_Curve);

            foreach (Grid Grid in All_Grids)
            {
                Curve C = Grid.Curve;
                XYZ P1_Grd = C.GetEndPoint(0);
                XYZ P2_Grd = C.GetEndPoint(1);

                double GridSlope = Helper.CalculateSlope(P1_Grd, P2_Grd);

                if (Helper.AreLinesParallel(BeamSlope, GridSlope))
                {
                    Result.Add(Grid);


                }

            }

            return Result;

        }

        private List<Face> GetRightAndLeftFaces(Element _Beam)
        {
         


            List<Face> TempResult = new List<Face>();
            List<Face> Result = new List<Face>();
            XYZ BeamDirection = ((_Beam.Location as LocationCurve).Curve as Line).Direction;

            Solid BeamSolid = GetBeamSolid(_Beam);

            FaceArray BeamFaces = BeamSolid.Faces;

            List<Face> Faces = Helper.FaceArrayToList(BeamFaces).Where(e => e is PlanarFace).ToList();

            foreach (PlanarFace face in Faces)
            {
                XYZ FaceDirection = face.FaceNormal;

                double BeamDirection_X = BeamDirection.X;
                double BeamDirection_Y = BeamDirection.Y;

                if ((Math.Abs(FaceDirection.Y) - Math.Abs(BeamDirection_X) < 0.00001) && ((Math.Abs(FaceDirection.X) - Math.Abs(BeamDirection_Y)) < 0.00001))
                {
                    TempResult.Add(face);
                }
            }
            var Temp2 = TempResult.OrderByDescending(e => e.Area).ToList();
            if (Temp2.Count >= 2)
            {
                Result.Add(Temp2[0]);
                Result.Add(Temp2[1]);
            }


            return Result;


        }

        private List<Face> GetTopAndBotFaces(Element _Beam)
        {
         

            List<Face> TempResult = new List<Face>();
            List<Face> Result = new List<Face>();
            XYZ BeamDirection = ((_Beam.Location as LocationCurve).Curve as Line).Direction;

            Solid BeamSolid = GetBeamSolid(_Beam);

            FaceArray BeamFaces = BeamSolid.Faces;

            List<Face> Faces = Helper.FaceArrayToList(BeamFaces).Where(e => e is PlanarFace).ToList();

            foreach (PlanarFace face in Faces)
            {
                XYZ FaceDirection = face.FaceNormal;



                if (Math.Round(Math.Abs(FaceDirection.Z), 5) == 1)
                {
                    TempResult.Add(face);
                }
            }

            var Temp2 = TempResult.OrderByDescending(e => e.Area).ToList();
            if (Temp2.Count >= 2)
            {
                Result.Add(Temp2[0]);
                Result.Add(Temp2[1]);
            }


            return Result;


        }

        private Solid GetBeamSolid(Element _Beam)
        {

            // Get the geometry of the beam
            Options options = new Options();
            options.ComputeReferences = true;
            options.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement columnGeometry = _Beam.get_Geometry(options);
            Solid solid = default;

            foreach (GeometryObject geomObj in columnGeometry)
            {
                if ((geomObj as Solid) != null && (geomObj as Solid).Volume != 0)
                {
                    solid = geomObj as Solid;
                    break;
                }

            }

            return solid;
        }

        private Line GetDimensionLineForBeamDimension(Element _Beam)
        {
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            XYZ StartPoint = BeamCurve.GetEndPoint(0);
            XYZ EndPoint = BeamCurve.GetEndPoint(1);
            XYZ MidPoint = (StartPoint + EndPoint) / 2;
            double X1 = MidPoint.X;
            double Y1 = MidPoint.Y;
            double X = 0;
            double Y = 0;


            double TempSlope = Helper.CalculateSlope(StartPoint, EndPoint);
            double MainSlope;
            if (TempSlope == 1000000)
            {
                MainSlope = 0;
                X = 2 * X1;
                Y = Y1;
            }
            else if (Math.Abs(TempSlope) < 0.00001)
            {
                MainSlope = 1000000;
                X = X1;
                Y = 2 * Y1;
            }
            else
            {
                MainSlope = -1 / TempSlope;
                X = 2 * X1;
                Y = Y1 + MainSlope * (X - X1);
            }

            XYZ Point_2 = new XYZ(X, Y, MidPoint.Z);

            Line L = Line.CreateBound(MidPoint, Point_2);


            return L;



        }


        private Line GetDimensionLineForGridDimension(Element _Beam)
        {
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            XYZ StartPoint = BeamCurve.GetEndPoint(0);
            XYZ EndPoint = BeamCurve.GetEndPoint(1);
            XYZ MidPoint = (StartPoint + EndPoint) / 2;
            double X1 = MidPoint.X;
            double Y1 = MidPoint.Y;
            double X = 0;
            double Y = 0;


            double TempSlope = Helper.CalculateSlope(StartPoint, EndPoint);
            double MainSlope;
            if (TempSlope == 1000000)
            {
                MainSlope = 0;
                X = 2 * X1;
                Y = Y1;
            }
            else if (Math.Abs(TempSlope) < 0.00001)
            {
                MainSlope = 1000000;
                X = X1;
                Y = 2 * Y1;
            }
            else
            {
                MainSlope = -1 / TempSlope;
                X = 2 * X1;
                Y = Y1 + MainSlope * (X - X1);
            }

            XYZ Point_2 = new XYZ(X, Y, MidPoint.Z);

            Line L = Line.CreateBound(MidPoint, Point_2);


            Line Line_Trans = Helper.GetTransitionedLine(L, 500 * 0.00328084);


            return Line_Trans;



        }


        private XYZ GetLeaderPosition(Element _Beam, double Distance)
        {
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            XYZ StartPoint = BeamCurve.GetEndPoint(0);
            XYZ EndPoint = BeamCurve.GetEndPoint(1);
            XYZ MidPoint = (StartPoint + EndPoint) / 2;
            double X1 = MidPoint.X;
            double Y1 = MidPoint.Y;
            double X = 0;
            double Y = 0;


            double TempSlope = Helper.CalculateSlope(StartPoint, EndPoint);
            double MainSlope;
            if (TempSlope == 1000000)
            {
                MainSlope = 0;
                X = X1 + Distance;
                Y = Y1;
            }
            else if (Math.Abs(TempSlope) < 0.00001)
            {
                MainSlope = 1000000;
                X = X1;
                Y = Distance + Y1;
            }
            else
            {
                MainSlope = -1 / TempSlope;
                X = Distance + X1;
                Y = Y1 + MainSlope * (X - X1);
            }

            XYZ Point_2 = new XYZ(X, Y, MidPoint.Z);

            Line L = Line.CreateBound(MidPoint, Point_2);


            Line Line_Trans = Helper.GetTransitionedLine(L, 500 * 0.00328084);


            return Line_Trans.GetEndPoint(1);



        }



        #endregion


    }
}
