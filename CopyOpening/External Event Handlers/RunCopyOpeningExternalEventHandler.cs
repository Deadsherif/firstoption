using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using CopyOpening.MVVM.ViewModel;
using System.Windows;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Xml.Linq;
using Autodesk.Revit.DB.Mechanical;

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using CopyOpening.MVVM.View;
using System.Threading;
using System.Windows.Documents;

using System.Data;
using Microsoft.Win32;
using System.IO.Packaging;
using System.Diagnostics;

using System.Security.Cryptography;
using System.Windows.Media.Media3D;

namespace CopyOpening.External_Event_Handlers
{
    internal class RunCopyOpeningExternalEventHandler : IExternalEventHandler
    {

        public MainWindowViewModel MainviewModel { get; set; }
        public MainWindow Mainview { get; set; }

        public RunCopyOpeningExternalEventHandler()
        {

        }
        public void Execute(UIApplication app)
        {


            try
            {
                var uidoc = app.ActiveUIDocument;
                var doc = app.ActiveUIDocument.Document;
                var activeView = doc.ActiveView;

                Transaction tr = new Transaction(doc, "Copy Opening");
                tr.Start();

                // Get the linked document
                Document linkedDoc = MainviewModel.SelectedLink.GetLinkDocument();
                if (linkedDoc == null) return;

                var openingIds = new FilteredElementCollector(linkedDoc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType().ToElementIds().ToList();

                CopyPasteOptions copyOptions = new CopyPasteOptions();

                copyOptions.SetDuplicateTypeNamesHandler(new CopyUseDestination());

                var copiedElements = ElementTransformUtils.CopyElements(linkedDoc, openingIds, doc, null, copyOptions);

                tr.Commit();


                Transaction tr2 = new Transaction(doc, "Cut Element");
                tr2.Start();

                foreach (var copiedElementId in copiedElements)
                {
                    var element = doc.GetElement(copiedElementId);
                    ElementMulticategoryFilter elementMulticategoryFilter = new ElementMulticategoryFilter(new List<BuiltInCategory> { BuiltInCategory.OST_Walls, BuiltInCategory.OST_Floors });


                    ElementIntersectsElementFilter elementIntersectsElementFilter = new ElementIntersectsElementFilter(element);

                    var wallsAndFloors = new FilteredElementCollector(doc).WherePasses(elementMulticategoryFilter).WherePasses(elementIntersectsElementFilter).ToList();

                    if (wallsAndFloors.FirstOrDefault() == null) continue;



                    try
                    {
                        InstanceVoidCutUtils.AddInstanceVoidCut(doc, wallsAndFloors.FirstOrDefault(), element);
                        
                    }
                    catch (Exception)
                    {

                       
                    }

                }



                tr2.Commit();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        public string GetName() => "Run Tool";



    }
    public class CopyUseDestination : IDuplicateTypeNamesHandler

    {

        public DuplicateTypeAction OnDuplicateTypeNamesFound(

        DuplicateTypeNamesHandlerArgs args)

        {

            return DuplicateTypeAction.UseDestinationTypes;

        }

    }



}




