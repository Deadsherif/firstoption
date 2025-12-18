// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.EV
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace MEP_Fabricator
{
    internal class EV : IExternalEventHandler
    {
        public static bool ff = true;
        public static DefinitionFile spFile = (DefinitionFile)null;
        public static int startIndex = 0;
        public static double splitter = 3.2808399;
        public static int fabElem = 0;

        public void Execute(UIApplication app)
        {
            UIDocument activeUiDocument = app.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
            Enum.GetValues(typeof(BuiltInCategory));
            Category category1 = document.Settings.Categories.get_Item((BuiltInCategory)(-2008000));
            Category category2 = document.Settings.Categories.get_Item((BuiltInCategory)(-2008044));
            Category category3 = document.Settings.Categories.get_Item((BuiltInCategory)(-2008130));
            Category category4 = document.Settings.Categories.get_Item((BuiltInCategory)(-2008132));
            CategorySet categorySet = app.Application.Create.NewCategorySet();
            categorySet.Insert(category1);
            categorySet.Insert(category2);
            categorySet.Insert(category3);
            categorySet.Insert(category4);
            List<BuiltInCategory> builtInCategoryList = new List<BuiltInCategory>();
            try
            {
                EV.spFile = app.Application.OpenSharedParameterFile();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", "not valid shared parameter file!");
                EV.ff = false;
            }
            if (!EV.ff)
                return;
            if (EV.spFile != null)
            {
                if (!EV.Get_TXT_SearchResult(EV.spFile.Filename, "49994AD6-E279-4114-A09B-946894590B8A"))
                {
                    EV.AddNewLine(EV.spFile.Filename, EV.spFile.Groups.Size, "49994AD6-E279-4114-A09B-946894590B8A", "FabricatorIndex", "mepFabricator", "TEXT", "the serialized index produced by MEP Fabricator add-in");
                    EV.spFile = app.Application.OpenSharedParameterFile();
                }
                if (!EV.Get_TXT_SearchResult(EV.spFile.Filename, "49994AD6-E279-4114-A09B-946894590B8B"))
                {
                    EV.AddNewLine(EV.spFile.Filename, EV.spFile.Groups.Size, "49994AD6-E279-4114-A09B-946894590B8B", "Fabricated", "FabricatedParts", "TEXT", "Is this element created as a fabrication part ?");
                    EV.spFile = app.Application.OpenSharedParameterFile();
                }
            }
            else
                TaskDialog.Show("not found", "cant find the shared parameter file");
            foreach (DefinitionGroup group in EV.spFile.Groups)
            {
                string name = group.Name;
                if (group.Name == "mepFabricator")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add FabricatorIndex Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "FabricatorIndex")
                            {
                                InstanceBinding instanceBinding = app.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, (BuiltInParameterGroup)(-5000123));
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
                if (group.Name == "FabricatedParts")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add Fabricated Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "Fabricated")
                            {
                                InstanceBinding instanceBinding = app.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, (BuiltInParameterGroup)(-5000123));
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            string versionNumber = document.Application.VersionNumber;
            string versionName = document.Application.VersionName;
            if (document.Application.VersionNumber == "2023")
            {
                int num = (int)new CatigoryPick().ShowDialog();
                switch (EV.fabElem)
                {
                    case 0:
                        this.FabricateDucts(activeUiDocument, document);
                        break;
                    case 1:
                        this.FabricatePipes(activeUiDocument, document);
                        break;
                    case 2:
                        this.FabricateCableTrays(activeUiDocument, document);
                        break;
                    case 3:
                        this.FabricateConduits(activeUiDocument, document);
                        break;
                }
            }
        }

        public string GetName() => "aa";

        public static void AddNewLine(
          string pathToFile,
          int size,
          string paramID,
          string paramName,
          string groupName,
          string paramType,
          string paramDiscription)
        {
            string[] strArray = File.ReadAllLines(pathToFile);
            string str1 = pathToFile;
            string str2 = str1.Remove(str1.Length - 4, 4) + "n.txt";
            File.Create(str2).Dispose();
            string str3 = "";
            for (int index = 0; index < strArray.Length; ++index)
                str3 = str3 + strArray[index] + Environment.NewLine;
            string contents = str3 + string.Format("GROUP\t{0}\t{1}", (object)(size + 1), (object)groupName) + Environment.NewLine + string.Format("PARAM\t{0}\t{1}\t{2}\t\t{3}\t1\t{4}\t1\t0", (object)paramID, (object)paramName, (object)paramType, (object)(size + 1), (object)paramDiscription);
            File.AppendAllText(str2, contents);
            File.Delete(pathToFile);
            File.Move(str2, pathToFile);
        }

        public static bool Get_TXT_SearchResult(string pathToFile, string paramID)
        {
            bool txtSearchResult = false;
            foreach (string readAllLine in File.ReadAllLines(pathToFile))
            {
                if (readAllLine.Contains(paramID))
                {
                    txtSearchResult = true;
                    break;
                }
            }
            return txtSearchResult;
        }

        public double GetPipeUnionThickness(Document doc, Pipe d)
        {
            Curve curve = (((Element)d).Location as LocationCurve).Curve;
            XYZ endPoint1 = curve.GetEndPoint(0);
            XYZ endPoint2 = curve.GetEndPoint(1);
            XYZ xyz = new XYZ((endPoint1.X + endPoint2.X) / 2.0, (endPoint1.Y + endPoint2.Y) / 2.0, (endPoint1.Z + endPoint2.Z) / 2.0);
            Transaction transaction = new Transaction(doc);
            transaction.Start("getThickness");
            Pipe pipe1 = Pipe.Create(doc, ((Element)((MEPCurve)d).MEPSystem).GetTypeId(), ((Element)d.PipeType).Id, ((Element)((MEPCurve)d).ReferenceLevel).Id, endPoint1, xyz);
            Pipe pipe2 = Pipe.Create(doc, ((Element)((MEPCurve)d).MEPSystem).GetTypeId(), ((Element)d.PipeType).Id, ((Element)((MEPCurve)d).ReferenceLevel).Id, xyz, endPoint2);
            ConnectorSet connectors1 = ((MEPCurve)pipe1).ConnectorManager.Connectors;
            List<Connector> A = new List<Connector>();
            foreach (Connector connector in connectors1)
                A.Add(connector);
            ConnectorSet connectors2 = ((MEPCurve)pipe2).ConnectorManager.Connectors;
            List<Connector> B = new List<Connector>();
            foreach (Connector connector in connectors2)
                B.Add(connector);
            ((Element)pipe1).LookupParameter("Diameter").Set(((MEPCurve)d).Diameter);
            ((Element)pipe2).LookupParameter("Diameter").Set(((MEPCurve)d).Diameter);
            ConnectorSet connectors3 = this.createUnion(doc, A, B).MEPModel.ConnectorManager.Connectors;
            List<Connector> connectorList = new List<Connector>();
            foreach (Connector connector in connectors3)
                connectorList.Add(connector);
            double pipeUnionThickness = connectorList[0].Origin.DistanceTo(connectorList[1].Origin);
            transaction.RollBack();
            return pipeUnionThickness;
        }

        public double GetDuctUnionThickness(Document doc, Duct d)
        {
            Curve curve = (((Element)d).Location as LocationCurve).Curve;
            XYZ endPoint1 = curve.GetEndPoint(0);
            XYZ endPoint2 = curve.GetEndPoint(1);
            XYZ xyz = new XYZ((endPoint1.X + endPoint2.X) / 2.0, (endPoint1.Y + endPoint2.Y) / 2.0, (endPoint1.Z + endPoint2.Z) / 2.0);
            Transaction transaction = new Transaction(doc);
            transaction.Start("getThickness");
            Duct duct1 = Duct.Create(doc, ((Element)((MEPCurve)d).MEPSystem).GetTypeId(), ((Element)d.DuctType).Id, ((Element)((MEPCurve)d).ReferenceLevel).Id, endPoint1, xyz);
            Duct duct2 = Duct.Create(doc, ((Element)((MEPCurve)d).MEPSystem).GetTypeId(), ((Element)d.DuctType).Id, ((Element)((MEPCurve)d).ReferenceLevel).Id, xyz, endPoint2);
            ConnectorSet connectors1 = ((MEPCurve)duct1).ConnectorManager.Connectors;
            List<Connector> A = new List<Connector>();
            foreach (Connector connector in connectors1)
                A.Add(connector);
            ConnectorSet connectors2 = ((MEPCurve)duct2).ConnectorManager.Connectors;
            List<Connector> B = new List<Connector>();
            foreach (Connector connector in connectors2)
                B.Add(connector);
            try
            {
                ((Element)duct1).LookupParameter("Width").Set(((MEPCurve)d).Width);
                ((Element)duct1).LookupParameter("Height").Set(((MEPCurve)d).Height);
                ((Element)duct2).LookupParameter("Width").Set(((MEPCurve)d).Width);
                ((Element)duct2).LookupParameter("Height").Set(((MEPCurve)d).Height);
            }
            catch (Exception ex)
            {
            }
            ConnectorSet connectors3 = this.createUnion(doc, A, B).MEPModel.ConnectorManager.Connectors;
            List<Connector> connectorList = new List<Connector>();
            foreach (Connector connector in connectors3)
                connectorList.Add(connector);
            double ductUnionThickness = connectorList[0].Origin.DistanceTo(connectorList[1].Origin);
            transaction.RollBack();
            return ductUnionThickness;
        }

        public double GetTrayUnionThickness(Document doc, CableTray d)
        {
            Curve curve = (((Element)d).Location as LocationCurve).Curve;
            XYZ endPoint1 = curve.GetEndPoint(0);
            XYZ endPoint2 = curve.GetEndPoint(1);
            XYZ xyz = new XYZ((endPoint1.X + endPoint2.X) / 2.0, (endPoint1.Y + endPoint2.Y) / 2.0, (endPoint1.Z + endPoint2.Z) / 2.0);
            Transaction transaction = new Transaction(doc);
            transaction.Start("getThickness");
            CableTray cableTray1 = CableTray.Create(doc, ((Element)d).GetTypeId(), endPoint1, xyz, ((Element)((MEPCurve)d).ReferenceLevel).Id);
            CableTray cableTray2 = CableTray.Create(doc, ((Element)d).GetTypeId(), xyz, endPoint2, ((Element)((MEPCurve)d).ReferenceLevel).Id);
            ConnectorSet connectors1 = ((MEPCurve)cableTray1).ConnectorManager.Connectors;
            List<Connector> A = new List<Connector>();
            foreach (Connector connector in connectors1)
                A.Add(connector);
            ConnectorSet connectors2 = ((MEPCurve)cableTray2).ConnectorManager.Connectors;
            List<Connector> B = new List<Connector>();
            foreach (Connector connector in connectors2)
                B.Add(connector);
            try
            {
                ((Element)cableTray1).LookupParameter("Width").Set(((MEPCurve)d).Width);
                ((Element)cableTray1).LookupParameter("Height").Set(((MEPCurve)d).Height);
                ((Element)cableTray2).LookupParameter("Width").Set(((MEPCurve)d).Width);
                ((Element)cableTray2).LookupParameter("Height").Set(((MEPCurve)d).Height);
            }
            catch (Exception ex)
            {
            }
            ConnectorSet connectors3 = this.createUnion(doc, A, B).MEPModel.ConnectorManager.Connectors;
            List<Connector> connectorList = new List<Connector>();
            foreach (Connector connector in connectors3)
                connectorList.Add(connector);
            double trayUnionThickness = connectorList[0].Origin.DistanceTo(connectorList[1].Origin);
            transaction.RollBack();
            return trayUnionThickness;
        }

        public double GetConduitUnionThickness(Document doc, Conduit d)
        {
            Curve curve = (((Element)d).Location as LocationCurve).Curve;
            XYZ endPoint1 = curve.GetEndPoint(0);
            XYZ endPoint2 = curve.GetEndPoint(1);
            XYZ xyz = new XYZ((endPoint1.X + endPoint2.X) / 2.0, (endPoint1.Y + endPoint2.Y) / 2.0, (endPoint1.Z + endPoint2.Z) / 2.0);
            Transaction transaction = new Transaction(doc);
            transaction.Start("getThickness");
            Conduit conduit1 = Conduit.Create(doc, ((Element)d).GetTypeId(), endPoint1, xyz, ((Element)((MEPCurve)d).ReferenceLevel).Id);
            Conduit conduit2 = Conduit.Create(doc, ((Element)d).GetTypeId(), xyz, endPoint2, ((Element)((MEPCurve)d).ReferenceLevel).Id);
            ConnectorSet connectors1 = ((MEPCurve)conduit1).ConnectorManager.Connectors;
            List<Connector> A = new List<Connector>();
            foreach (Connector connector in connectors1)
                A.Add(connector);
            ConnectorSet connectors2 = ((MEPCurve)conduit2).ConnectorManager.Connectors;
            List<Connector> B = new List<Connector>();
            foreach (Connector connector in connectors2)
                B.Add(connector);
            ((Element)conduit1).LookupParameter("Diameter(Trade Size)").Set(((MEPCurve)d).Diameter);
            ((Element)conduit2).LookupParameter("Diameter(Trade Size)").Set(((MEPCurve)d).Diameter);
            ConnectorSet connectors3 = this.createUnion(doc, A, B).MEPModel.ConnectorManager.Connectors;
            List<Connector> connectorList = new List<Connector>();
            foreach (Connector connector in connectors3)
                connectorList.Add(connector);
            double conduitUnionThickness = connectorList[0].Origin.DistanceTo(connectorList[1].Origin);
            transaction.RollBack();
            return conduitUnionThickness;
        }

        public void TestDuctConnector(Duct d)
        {
            ConnectorSet connectors = ((MEPCurve)d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            ConnectorSet allRefs = connectorList1[0].AllRefs;
            List<Connector> connectorList2 = new List<Connector>();
            foreach (Connector connector in allRefs)
                connectorList2.Add(connector);
        }

        public List<List<Connector>> GetDuctConnectorsCouples(Duct d)
        {
            ConnectorSet connectors = ((MEPCurve)d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = connectorList1[0];
            ConnectorSet allRefs1 = connector1.AllRefs;
            List<Connector> connectorList2 = new List<Connector>();
            foreach (Connector connector2 in allRefs1)
                connectorList2.Add(connector2);
            for (int index = 0; index < connectorList2.Count; ++index)
            {
                if (connectorList2[index].ConnectorType != ConnectorType.Logical && connectorList2[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList2[index] = connector1;
            }
            try
            {
                connectorList2[0].DisconnectFrom(connectorList2[1]);
            }
            catch (Exception ex)
            {
            }
            Connector connector3 = connectorList1[1];
            ConnectorSet allRefs2 = connector3.AllRefs;
            List<Connector> connectorList3 = new List<Connector>();
            foreach (Connector connector4 in allRefs2)
                connectorList3.Add(connector4);
            for (int index = 0; index < connectorList3.Count; ++index)
            {
                if (connectorList3[index].ConnectorType != ConnectorType.Logical && connectorList3[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList3[index] = connector3;
            }
            try
            {
                connectorList3[0].DisconnectFrom(connectorList3[1]);
            }
            catch (Exception ex)
            {
            }
            return new List<List<Connector>>()
      {
        connectorList2,
        connectorList3
      };
        }

        public List<List<Connector>> GetPipeConnectorsCouples(Pipe d)
        {
            ConnectorSet connectors = ((MEPCurve)d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = connectorList1[0];
            ConnectorSet allRefs1 = connector1.AllRefs;
            List<Connector> connectorList2 = new List<Connector>();
            foreach (Connector connector2 in allRefs1)
                connectorList2.Add(connector2);
            for (int index = 0; index < connectorList2.Count; ++index)
            {
                if (connectorList2[index].ConnectorType != ConnectorType.Logical && connectorList2[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList2[index] = connector1;
            }
            try
            {
                connectorList2[0].DisconnectFrom(connectorList2[1]);
            }
            catch (Exception ex)
            {
            }
            Connector connector3 = connectorList1[1];
            ConnectorSet allRefs2 = connector3.AllRefs;
            List<Connector> connectorList3 = new List<Connector>();
            foreach (Connector connector4 in allRefs2)
                connectorList3.Add(connector4);
            for (int index = 0; index < connectorList3.Count; ++index)
            {
                if (connectorList3[index].ConnectorType != ConnectorType.Logical && connectorList3[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList3[index] = connector3;
            }
            try
            {
                connectorList3[0].DisconnectFrom(connectorList3[1]);
            }
            catch (Exception ex)
            {
            }
            return new List<List<Connector>>()
      {
        connectorList2,
        connectorList3
      };
        }

        public List<List<Connector>> GetTrayConnectorsCouples(CableTray d)
        {
            ConnectorSet connectors = ((MEPCurve)d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = connectorList1[0];
            ConnectorSet allRefs1 = connector1.AllRefs;
            List<Connector> connectorList2 = new List<Connector>();
            foreach (Connector connector2 in allRefs1)
                connectorList2.Add(connector2);
            for (int index = 0; index < connectorList2.Count; ++index)
            {
                if (connectorList2[index].ConnectorType != ConnectorType.Logical && connectorList2[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList2[index] = connector1;
            }
            try
            {
                connectorList2[0].DisconnectFrom(connectorList2[1]);
            }
            catch (Exception ex)
            {
            }
            Connector connector3 = connectorList1[1];
            ConnectorSet allRefs2 = connector3.AllRefs;
            List<Connector> connectorList3 = new List<Connector>();
            foreach (Connector connector4 in allRefs2)
                connectorList3.Add(connector4);
            for (int index = 0; index < connectorList3.Count; ++index)
            {
                if (connectorList3[index].ConnectorType != ConnectorType.Logical && connectorList3[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList3[index] = connector3;
            }
            try
            {
                connectorList3[0].DisconnectFrom(connectorList3[1]);
            }
            catch (Exception ex)
            {
            }
            return new List<List<Connector>>()
      {
        connectorList2,
        connectorList3
      };
        }

        public List<List<Connector>> GetConduitConnectorsCouples(Conduit d)
        {
            ConnectorSet connectors = ((MEPCurve)d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = connectorList1[0];
            ConnectorSet allRefs1 = connector1.AllRefs;
            List<Connector> connectorList2 = new List<Connector>();
            foreach (Connector connector2 in allRefs1)
                connectorList2.Add(connector2);
            for (int index = 0; index < connectorList2.Count; ++index)
            {
                if (connectorList2[index].ConnectorType != ConnectorType.Logical && connectorList2[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList2[index] = connector1;
            }
            try
            {
                connectorList2[0].DisconnectFrom(connectorList2[1]);
            }
            catch (Exception ex)
            {
            }
            Connector connector3 = connectorList1[1];
            ConnectorSet allRefs2 = connector3.AllRefs;
            List<Connector> connectorList3 = new List<Connector>();
            foreach (Connector connector4 in allRefs2)
                connectorList3.Add(connector4);
            for (int index = 0; index < connectorList3.Count; ++index)
            {
                if (connectorList3[index].ConnectorType != ConnectorType.Logical && connectorList3[index].Owner.Category.Name == ((Element)d).Category.Name)
                    connectorList3[index] = connector3;
            }
            try
            {
                connectorList3[0].DisconnectFrom(connectorList3[1]);
            }
            catch (Exception ex)
            {
            }
            return new List<List<Connector>>()
      {
        connectorList2,
        connectorList3
      };
        }

        public void ConnectDuctChildAsParent(Duct ch_d, List<List<Connector>> ress)
        {
            ConnectorSet connectors = ((MEPCurve)ch_d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = (Connector)null;
            List<Connector> connectorList2 = (List<Connector>)null;
            foreach (Connector connector2 in connectorList1)
            {
                XYZ origin = connector2.Origin;
                foreach (List<Connector> connectorList3 in ress)
                {
                    foreach (Connector connector3 in connectorList3)
                    {
                        if (connector3.ConnectorType != ConnectorType.Logical && connector3.Origin.DistanceTo(origin) < 0.0001)
                        {
                            connector1 = connector2;
                            connectorList2 = connectorList3;
                            break;
                        }
                    }
                    if (connectorList2 != null)
                        break;
                }
                if (connectorList2 != null)
                    break;
            }
            foreach (Connector connector4 in connectorList2)
            {
                if (connector4.Owner.Category.Name != ((Element)ch_d).Category.Name)
                {
                    try
                    {
                        connector1.ConnectTo(connector4);
                        break;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void ConnectPipeChildAsParent(Pipe ch_d, List<List<Connector>> ress)
        {
            ConnectorSet connectors = ((MEPCurve)ch_d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = (Connector)null;
            List<Connector> connectorList2 = (List<Connector>)null;
            foreach (Connector connector2 in connectorList1)
            {
                XYZ origin = connector2.Origin;
                foreach (List<Connector> connectorList3 in ress)
                {
                    foreach (Connector connector3 in connectorList3)
                    {
                        if (connector3.ConnectorType != ConnectorType.Logical && connector3.Origin.DistanceTo(origin) < 0.0001)
                        {
                            connector1 = connector2;
                            connectorList2 = connectorList3;
                            break;
                        }
                    }
                    if (connectorList2 != null)
                        break;
                }
                if (connectorList2 != null)
                    break;
            }
            foreach (Connector connector4 in connectorList2)
            {
                if (connector4.Owner.Category.Name != ((Element)ch_d).Category.Name)
                {
                    try
                    {
                        connector1.ConnectTo(connector4);
                        break;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void ConnectTrayChildAsParent(CableTray ch_d, List<List<Connector>> ress)
        {
            ConnectorSet connectors = ((MEPCurve)ch_d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = (Connector)null;
            List<Connector> connectorList2 = (List<Connector>)null;
            foreach (Connector connector2 in connectorList1)
            {
                XYZ origin = connector2.Origin;
                foreach (List<Connector> connectorList3 in ress)
                {
                    foreach (Connector connector3 in connectorList3)
                    {
                        if (connector3.ConnectorType != ConnectorType.Logical && connector3.Origin.DistanceTo(origin) < 0.0001)
                        {
                            connector1 = connector2;
                            connectorList2 = connectorList3;
                            break;
                        }
                    }
                    if (connectorList2 != null)
                        break;
                }
                if (connectorList2 != null)
                    break;
            }
            foreach (Connector connector4 in connectorList2)
            {
                if (connector4.Owner.Category.Name != ((Element)ch_d).Category.Name)
                {
                    try
                    {
                        connector1.ConnectTo(connector4);
                        break;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void ConnectConduitChildAsParent(Conduit ch_d, List<List<Connector>> ress)
        {
            ConnectorSet connectors = ((MEPCurve)ch_d).ConnectorManager.Connectors;
            List<Connector> connectorList1 = new List<Connector>();
            foreach (Connector connector in connectors)
                connectorList1.Add(connector);
            Connector connector1 = (Connector)null;
            List<Connector> connectorList2 = (List<Connector>)null;
            foreach (Connector connector2 in connectorList1)
            {
                XYZ origin = connector2.Origin;
                foreach (List<Connector> connectorList3 in ress)
                {
                    foreach (Connector connector3 in connectorList3)
                    {
                        if (connector3.ConnectorType != ConnectorType.Logical && connector3.Origin.DistanceTo(origin) < 0.0001)
                        {
                            connector1 = connector2;
                            connectorList2 = connectorList3;
                            break;
                        }
                    }
                    if (connectorList2 != null)
                        break;
                }
                if (connectorList2 != null)
                    break;
            }
            foreach (Connector connector4 in connectorList2)
            {
                if (connector4.Owner.Category.Name != ((Element)ch_d).Category.Name)
                {
                    try
                    {
                        connector1.ConnectTo(connector4);
                        break;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void FabricateDucts(UIDocument uidoc, Document doc)
        {
            List<Reference> referenceList = new List<Reference>();
            int num1 = 0;
            try
            {
                var references = uidoc.Selection.PickObjects((ObjectType)1, string.Format("COUNT:  {0}  Ducts", (object)num1));
                foreach (var reference in references)
                {
                    if (doc.GetElement(reference.ElementId).Category.Name == "Ducts")
                    {
                        referenceList.Add(reference);
                        ++num1;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            int num2 = (int)new StartIndex().ShowDialog();
            List<Element> elementList = new List<Element>();
            int startIndex = EV.startIndex;
            foreach (Reference reference in referenceList)
            {
                List<XYZ> xyzList = new List<XYZ>();
                Duct element = doc.GetElement(reference.ElementId) as Duct;
                List<List<Connector>> connectorsCouples;
                try
                {
                    this.TestDuctConnector(element);
                    connectorsCouples = this.GetDuctConnectorsCouples(element);
                    double ductUnionThickness = this.GetDuctUnionThickness(doc, element);
                    Curve curve = (((Element)element).Location as LocationCurve).Curve;
                    XYZ endPoint1 = curve.GetEndPoint(0);
                    XYZ endPoint2 = curve.GetEndPoint(1);
                    double num3 = EV.splitter + ductUnionThickness;
                    double num4 = num3;
                    double num5 = endPoint1.X - endPoint2.X;
                    double num6 = endPoint1.Y - endPoint2.Y;
                    double num7 = endPoint1.Z - endPoint2.Z;
                    Math.Pow(num5 * num5 + num6 * num6 + num7 * num7, 0.5);
                    double num8 = endPoint1.DistanceTo(endPoint2);
                    double num9 = num8 / num3;
                    int.Parse(Math.Round(num9, 0).ToString());
                    xyzList.Add(endPoint1);
                    for (int index = 1; (double)index < num9; ++index)
                    {
                        XYZ xyz1 = new XYZ(endPoint1.X + (num4 - ductUnionThickness / 2.0) / num8 * (endPoint2.X - endPoint1.X), endPoint1.Y + (num4 - ductUnionThickness / 2.0) / num8 * (endPoint2.Y - endPoint1.Y), endPoint1.Z + (num4 - ductUnionThickness / 2.0) / num8 * (endPoint2.Z - endPoint1.Z));
                        double num10 = endPoint2.DistanceTo(xyz1);
                        if (((double)index < num9 - 1.0 || num10 >= 625.0 / 381.0) && false)
                        {
                            XYZ xyz2 = new XYZ(endPoint1.X + (num4 - (625.0 / 381.0 - num10) - ductUnionThickness) / num8 * (endPoint2.X - endPoint1.X), endPoint1.Y + (num4 - (625.0 / 381.0 - num10) - ductUnionThickness) / num8 * (endPoint2.Y - endPoint1.Y), endPoint1.Z + (num4 - (625.0 / 381.0 - num10) - ductUnionThickness) / num8 * (endPoint2.Z - endPoint1.Z));
                            xyzList.Add(xyz2);
                            num4 += num3;
                        }
                        else
                        {
                            xyzList.Add(xyz1);
                            num4 += num3;
                        }
                    }
                    xyzList.Add(endPoint2);
                }
                catch (Exception ex)
                {
                    continue;
                }
                Duct duct = (Duct)null;
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Fabricate Ducts");
                    try
                    {
                        if (xyzList.Count > 2)
                        {
                            for (int index = 0; index < xyzList.Count - 1; ++index)
                            {
                                Duct ch_d = Duct.Create(doc, ((Element)((MEPCurve)element).MEPSystem).GetTypeId(), ((Element)element.DuctType).Id, ((Element)((MEPCurve)element).ReferenceLevel).Id, xyzList[index], xyzList[index + 1]);
                                Curve curve1 = (((Element)ch_d).Location as LocationCurve).Curve;
                                XYZ endPoint3 = curve1.GetEndPoint(0);
                                XYZ endPoint4 = curve1.GetEndPoint(1);
                                double num11 = 304.8 * endPoint3.DistanceTo(endPoint4);
                                double num12 = 304.8 * ((Element)ch_d).LookupParameter("Length").AsDouble();
                                if (duct != null)
                                {
                                    Curve curve2 = (((Element)duct).Location as LocationCurve).Curve;
                                    XYZ endPoint5 = curve2.GetEndPoint(0);
                                    curve2.GetEndPoint(1);
                                    double num13 = 304.8 * endPoint5.DistanceTo(endPoint4);
                                    double num14 = 304.8 * ((Element)duct).LookupParameter("Length").AsDouble();
                                }
                              ((Element)ch_d).LookupParameter("Width").Set(((MEPCurve)element).Width);
                                ((Element)ch_d).LookupParameter("Height").Set(((MEPCurve)element).Height);
                                ((Element)ch_d).LookupParameter("Fabricated").Set("true");
                                ((Element)ch_d).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                                ++EV.startIndex;
                                if (duct == null)
                                {
                                    duct = ch_d;
                                }
                                else
                                {
                                    ConnectorSet connectors1 = ((MEPCurve)duct).ConnectorManager.Connectors;
                                    List<Connector> A = new List<Connector>();
                                    foreach (Connector connector in connectors1)
                                        A.Add(connector);
                                    ConnectorSet connectors2 = ((MEPCurve)ch_d).ConnectorManager.Connectors;
                                    List<Connector> B = new List<Connector>();
                                    foreach (Connector connector in connectors2)
                                        B.Add(connector);
                                    this.createUnion(doc, A, B);
                                    duct = ch_d;
                                }
                                if (index == 0 || index == xyzList.Count - 2)
                                    this.ConnectDuctChildAsParent(ch_d, connectorsCouples);
                            }
                            doc.Delete(((Element)element).Id);
                        }
                        else
                        {
                            ((Element)element).LookupParameter("Fabricated").Set("true");
                            ((Element)element).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                            ++EV.startIndex;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.RollBack();
                        continue;
                    }
                    transaction.Commit();
                }
            }
        }

        public FamilyInstance createUnion(Document doc, List<Connector> A, List<Connector> B)
        {
            Connector connector1 = (Connector)null;
            Connector connector2 = (Connector)null;
            double num1 = -1.0;
            foreach (Connector connector3 in A)
            {
                XYZ origin1 = connector3.Origin;
                foreach (Connector connector4 in B)
                {
                    XYZ origin2 = connector4.Origin;
                    double num2 = origin1.DistanceTo(origin2);
                    if (num1 == -1.0)
                    {
                        num1 = num2;
                        connector1 = connector3;
                        connector2 = connector4;
                    }
                    else if (num2 < num1)
                    {
                        num1 = num2;
                        connector1 = connector3;
                        connector2 = connector4;
                    }
                }
            }
            try
            {
                return doc.Create.NewUnionFitting(connector1, connector2);
            }
            catch (Exception ex)
            {
                return (FamilyInstance)null;
            }
        }

        public void connectAny(Document doc, List<Connector> A, List<Connector> B)
        {
            Connector connector1 = (Connector)null;
            Connector connector2 = (Connector)null;
            double num1 = -1.0;
            foreach (Connector connector3 in A)
            {
                XYZ origin1 = connector3.Origin;
                foreach (Connector connector4 in B)
                {
                    XYZ origin2 = connector4.Origin;
                    double num2 = origin1.DistanceTo(origin2);
                    if (num1 == -1.0)
                    {
                        num1 = num2;
                        connector1 = connector3;
                        connector2 = connector4;
                    }
                    else if (num2 < num1)
                    {
                        num1 = num2;
                        connector1 = connector3;
                        connector2 = connector4;
                    }
                }
            }
            try
            {
                connector1.ConnectTo(connector2);
            }
            catch (Exception ex)
            {
            }
        }

        public void FabricatePipes(UIDocument uidoc, Document doc)
        {
            List<Reference> referenceList = new List<Reference>();
            int num1 = 0;


            try
            {
                var references = uidoc.Selection.PickObjects((ObjectType)1, string.Format("COUNT:  {0}  Pipes", (object)num1));
                foreach (Reference reference in references)
                {
                    if (doc.GetElement(reference.ElementId).Category.Name == "Pipes")
                    {
                        referenceList.Add(reference);
                        ++num1;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            int num2 = (int)new StartIndex().ShowDialog();
            List<Element> elementList = new List<Element>();
            int startIndex = EV.startIndex;
            foreach (Reference reference in referenceList)
            {
                Pipe element = doc.GetElement(reference.ElementId) as Pipe;
                List<List<Connector>> connectorsCouples = this.GetPipeConnectorsCouples(element);
                double pipeUnionThickness = this.GetPipeUnionThickness(doc, element);
                Curve curve = (((Element)element).Location as LocationCurve).Curve;
                List<XYZ> xyzList = new List<XYZ>();
                XYZ endPoint1 = curve.GetEndPoint(0);
                XYZ endPoint2 = curve.GetEndPoint(1);
                double num3 = EV.splitter + pipeUnionThickness;
                double num4 = num3;
                double num5 = endPoint1.X - endPoint2.X;
                double num6 = endPoint1.Y - endPoint2.Y;
                double num7 = endPoint1.Z - endPoint2.Z;
                double num8 = Math.Pow(num5 * num5 + num6 * num6 + num7 * num7, 0.5);
                double num9 = num8 / num3;
                int.Parse(Math.Round(num9, 0).ToString());
                xyzList.Add(endPoint1);
                for (int index = 1; (double)index < num9; ++index)
                {
                    double num10 = endPoint1.X + (num4 - pipeUnionThickness / 2.0) / num8 * (endPoint2.X - endPoint1.X);
                    double num11 = endPoint1.Y + (num4 - pipeUnionThickness / 2.0) / num8 * (endPoint2.Y - endPoint1.Y);
                    double num12 = endPoint1.Z + (num4 - pipeUnionThickness / 2.0) / num8 * (endPoint2.Z - endPoint1.Z);
                    xyzList.Add(new XYZ(num10, num11, num12));
                    num4 += num3;
                }
                xyzList.Add(endPoint2);
                Pipe pipe = (Pipe)null;
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Fabricate Pipes");
                    if (xyzList.Count > 2)
                    {
                        for (int index = 0; index < xyzList.Count - 1; ++index)
                        {
                            Pipe ch_d = Pipe.Create(doc, ((Element)((MEPCurve)element).MEPSystem).GetTypeId(), ((Element)element.PipeType).Id, ((Element)((MEPCurve)element).ReferenceLevel).Id, xyzList[index], xyzList[index + 1]);
                            ((Element)ch_d).LookupParameter("Diameter").Set(((MEPCurve)element).Diameter);
                            ((Element)ch_d).LookupParameter("Fabricated").Set("true");
                            ((Element)ch_d).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                            ++EV.startIndex;
                            if (pipe == null)
                            {
                                pipe = ch_d;
                            }
                            else
                            {
                                ConnectorSet connectors1 = ((MEPCurve)pipe).ConnectorManager.Connectors;
                                List<Connector> A = new List<Connector>();
                                foreach (Connector connector in connectors1)
                                    A.Add(connector);
                                ConnectorSet connectors2 = ((MEPCurve)ch_d).ConnectorManager.Connectors;
                                List<Connector> B = new List<Connector>();
                                foreach (Connector connector in connectors2)
                                    B.Add(connector);
                                this.createUnion(doc, A, B);
                                pipe = ch_d;
                            }
                            if (index == 0 || index == xyzList.Count - 2)
                                this.ConnectPipeChildAsParent(ch_d, connectorsCouples);
                        }
                        doc.Delete(((Element)element).Id);
                    }
                    else
                    {
                        ((Element)element).LookupParameter("Fabricated").Set("true");
                        ((Element)element).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                        ++EV.startIndex;
                    }
                    transaction.Commit();
                }
            }
        }

        public void FabricateCableTrays(UIDocument uidoc, Document doc)
        {
            List<Reference> referenceList = new List<Reference>();
            int num1 = 0;

            try
            {
                var references = uidoc.Selection.PickObjects((ObjectType)1, string.Format("COUNT:  {0}  Cable Trays", (object)num1));
                foreach (var reference in references)
                {
                    if (doc.GetElement(reference.ElementId).Category.Name == "Cable Trays")
                    {
                        referenceList.Add(reference);
                        ++num1;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            int num2 = (int)new StartIndex().ShowDialog();
            List<Element> elementList = new List<Element>();
            int startIndex = EV.startIndex;
            foreach (Reference reference in referenceList)
            {
                CableTray element = doc.GetElement(reference.ElementId) as CableTray;
                List<List<Connector>> connectorsCouples = this.GetTrayConnectorsCouples(element);
                Curve curve = (((Element)element).Location as LocationCurve).Curve;
                List<XYZ> xyzList = new List<XYZ>();
                XYZ endPoint1 = curve.GetEndPoint(0);
                XYZ endPoint2 = curve.GetEndPoint(1);
                double trayUnionThickness = this.GetTrayUnionThickness(doc, element);
                double num3 = EV.splitter + trayUnionThickness;
                double num4 = num3;
                double num5 = endPoint1.X - endPoint2.X;
                double num6 = endPoint1.Y - endPoint2.Y;
                double num7 = endPoint1.Z - endPoint2.Z;
                double num8 = Math.Pow(num5 * num5 + num6 * num6 + num7 * num7, 0.5);
                double num9 = num8 / num3;
                int.Parse(Math.Round(num9, 0).ToString());
                xyzList.Add(endPoint1);
                for (int index = 1; (double)index < num9; ++index)
                {
                    double num10 = endPoint1.X + num4 / num8 * (endPoint2.X - endPoint1.X);
                    double num11 = endPoint1.Y + num4 / num8 * (endPoint2.Y - endPoint1.Y);
                    double num12 = endPoint1.Z + num4 / num8 * (endPoint2.Z - endPoint1.Z);
                    xyzList.Add(new XYZ(num10, num11, num12));
                    num4 += num3;
                }
                xyzList.Add(endPoint2);
                CableTray cableTray = (CableTray)null;
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Fabricate Cable Trays");
                    if (xyzList.Count > 2)
                    {
                        for (int index = 0; index < xyzList.Count - 1; ++index)
                        {
                            CableTray ch_d = CableTray.Create(doc, ((Element)element).GetTypeId(), xyzList[index], xyzList[index + 1], ((Element)((MEPCurve)element).ReferenceLevel).Id);
                            ((Element)ch_d).LookupParameter("Width").Set(((MEPCurve)element).Width);
                            ((Element)ch_d).LookupParameter("Height").Set(((MEPCurve)element).Height);
                            ((Element)ch_d).LookupParameter("Fabricated").Set("true");
                            ((Element)ch_d).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                            ++EV.startIndex;
                            if (cableTray == null)
                            {
                                cableTray = ch_d;
                            }
                            else
                            {
                                ConnectorSet connectors1 = ((MEPCurve)cableTray).ConnectorManager.Connectors;
                                List<Connector> A = new List<Connector>();
                                foreach (Connector connector in connectors1)
                                    A.Add(connector);
                                ConnectorSet connectors2 = ((MEPCurve)ch_d).ConnectorManager.Connectors;
                                List<Connector> B = new List<Connector>();
                                foreach (Connector connector in connectors2)
                                    B.Add(connector);
                                this.createUnion(doc, A, B);
                                cableTray = ch_d;
                            }
                            if (index == 0 || index == xyzList.Count - 2)
                                this.ConnectTrayChildAsParent(ch_d, connectorsCouples);
                        }
                        doc.Delete(((Element)element).Id);
                    }
                    else
                    {
                        ((Element)element).LookupParameter("Fabricated").Set("true");
                        ((Element)element).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                        ++EV.startIndex;
                    }
                    transaction.Commit();
                }
            }
        }

        public void FabricateConduits(UIDocument uidoc, Document doc)
        {
            List<Reference> referenceList = new List<Reference>();
            int num1 = 0;

            try
            {
                var references = uidoc.Selection.PickObjects((ObjectType)1, string.Format("COUNT:  {0}  Conduits", (object)num1));
                foreach (Reference reference in references)
                {
                    if (doc.GetElement(reference.ElementId).Category.Name == "Conduits")
                    {
                        referenceList.Add(reference);
                        ++num1;
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            int num2 = (int)new StartIndex().ShowDialog();
            List<Element> elementList = new List<Element>();
            int startIndex = EV.startIndex;
            foreach (Reference reference in referenceList)
            {
                Conduit element = doc.GetElement(reference.ElementId) as Conduit;
                List<List<Connector>> connectorsCouples = this.GetConduitConnectorsCouples(element);
                Curve curve = (((Element)element).Location as LocationCurve).Curve;
                List<XYZ> xyzList = new List<XYZ>();
                XYZ endPoint1 = curve.GetEndPoint(0);
                XYZ endPoint2 = curve.GetEndPoint(1);
                double num3 = EV.splitter + this.GetConduitUnionThickness(doc, element);
                double num4 = num3;
                double num5 = endPoint1.X - endPoint2.X;
                double num6 = endPoint1.Y - endPoint2.Y;
                double num7 = endPoint1.Z - endPoint2.Z;
                double num8 = Math.Pow(num5 * num5 + num6 * num6 + num7 * num7, 0.5);
                double num9 = num8 / num3;
                int.Parse(Math.Round(num9, 0).ToString());
                xyzList.Add(endPoint1);
                for (int index = 1; (double)index < num9; ++index)
                {
                    double num10 = endPoint1.X + num4 / num8 * (endPoint2.X - endPoint1.X);
                    double num11 = endPoint1.Y + num4 / num8 * (endPoint2.Y - endPoint1.Y);
                    double num12 = endPoint1.Z + num4 / num8 * (endPoint2.Z - endPoint1.Z);
                    xyzList.Add(new XYZ(num10, num11, num12));
                    num4 += num3;
                }
                xyzList.Add(endPoint2);
                Conduit conduit = (Conduit)null;
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Fabricate Conduits");
                    if (xyzList.Count > 2)
                    {
                        for (int index = 0; index < xyzList.Count - 1; ++index)
                        {
                            Conduit ch_d = Conduit.Create(doc, ((Element)element).GetTypeId(), xyzList[index], xyzList[index + 1], ((Element)((MEPCurve)element).ReferenceLevel).Id);
                            ((Element)ch_d).LookupParameter("Diameter(Trade Size)").Set(((MEPCurve)element).Diameter);
                            ((Element)ch_d).LookupParameter("Fabricated").Set("true");
                            ((Element)ch_d).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                            ++EV.startIndex;
                            if (conduit == null)
                            {
                                conduit = ch_d;
                            }
                            else
                            {
                                ConnectorSet connectors1 = ((MEPCurve)conduit).ConnectorManager.Connectors;
                                List<Connector> A = new List<Connector>();
                                foreach (Connector connector in connectors1)
                                    A.Add(connector);
                                ConnectorSet connectors2 = ((MEPCurve)ch_d).ConnectorManager.Connectors;
                                List<Connector> B = new List<Connector>();
                                foreach (Connector connector in connectors2)
                                    B.Add(connector);
                                this.createUnion(doc, A, B);
                                conduit = ch_d;
                            }
                            if (index == 0 || index == xyzList.Count - 2)
                                this.ConnectConduitChildAsParent(ch_d, connectorsCouples);
                        }
                        doc.Delete(((Element)element).Id);
                    }
                    else
                    {
                        ((Element)element).LookupParameter("Fabricated").Set("true");
                        ((Element)element).LookupParameter("FabricatorIndex").Set(EV.startIndex.ToString());
                        ++EV.startIndex;
                    }
                    transaction.Commit();
                }
            }
        }
    }
}
