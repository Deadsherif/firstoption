using AlignTags.Commands;
using AlignTags.SelectionFilters;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AlignTags.MVVM.ViewModel
{
   public class AlignTagsViewModel : ViewModelBase
   {
      #region Fields
      int selectedTagsNumber;
      bool d1;
      bool d2;
      bool d3;
      bool d4;
      bool isAuto;
      bool isMan;
      double sliderValue;
      double dimSpacing;
      bool orderX;

      #endregion

      #region Properties
      public int SelectedTagsNumber
      {
         get { return selectedTagsNumber; }
         set
         {
            selectedTagsNumber = value;
            OnPropertyChanged();
         }
      }
      public bool D1
      {
         get { return d1; }
         set
         {
            d1 = value; OnPropertyChanged();
         }
      }
      public bool D2
      {
         get { return d2; }
         set
         {
            d2 = value; OnPropertyChanged();
         }
      }
      public bool D3
      {
         get { return d3; }
         set
         {
            d3 = value; OnPropertyChanged();
         }
      }
      public bool D4
      {
         get { return d4; }
         set
         {
            d4 = value; OnPropertyChanged();
         }
      }
      public bool IsAuto
      {

         get { return isAuto; }
         set
         {
            isAuto = value;
            IsMan = !isAuto;
            OnPropertyChanged();
         }
      }
      public bool IsMan
      {

         get { return isMan; }
         set
         {
            isMan = value; OnPropertyChanged();
         }
      }
      public double SliderValue
      {
         get { return sliderValue; }
         set
         {
            // Round the value to the nearest multiple of 5
            sliderValue = 5 * (int)Math.Round(value / 5);
            Command.SavedAngle = sliderValue;
            OnPropertyChanged(nameof(SliderValue));
         }
      }
      public double DimSpacing
      {
         get
         {
            return dimSpacing;
         }
         set
         {
            dimSpacing = value;
            OnPropertyChanged();
         }
      }
      public bool OrderX
      {
         get
         {
            return orderX;
         }
         set
         {
            orderX = value;
            OnPropertyChanged();
         }

      }
      public ICommand AlignBTN { get; set; }
      public ICommand selectBTN { get; set; }

      #endregion

      #region Constructor
      public AlignTagsViewModel()
      {
         IsAuto = true;
         D1 = true;
         AlignBTN = new RelayCommand(P => AlignCommand(P));
         selectBTN = new RelayCommand(P => SelectTagsCommand(P));
         DimSpacing = 1000;
         SliderValue = Command.SavedAngle;
         OrderX = true;
      }
      #endregion


      #region ButtonActions
      public void SelectTagsCommand(object parameter)
      {
         try
         {
            Document doc = Command.Doc;
            UIDocument Uidoc = Command.UiDoc;
            TagsSelectionFilter tagsFilter = new TagsSelectionFilter();
            List<Reference> tagsReffs = new List<Reference>();
            Command.frm.Hide();
            try
            {
               tagsReffs = Uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, tagsFilter).ToList();
               Command.frm.VM.SelectedTagsNumber = tagsReffs.Count;
            }
            catch
            {

            }
            Command.tags = new List<Element>();

            foreach (Reference reff in tagsReffs)
            {
               Element ele = doc.GetElement(reff);
               if (ele is IndependentTag)
               {
                  Command.tags.Add(ele);

               }

            }
            Command.frm.VM.SelectedTagsNumber = Command.tags.Count;
            Command.frm.ShowDialog();


         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "First Option", MessageBoxButton.OK, MessageBoxImage.Error);
         }

      }
      public void AlignCommand(object parameter)
      {
         Document doc = Command.Doc;
         UIDocument UiDoc = Command.UiDoc;
         double degree = Command.frm.VM.SliderValue;
         List<Element> tagsToAlign = Command.tags;
         try
         {

            if (tagsToAlign != null && tagsToAlign.Count != 0)
            {
               Command.frm.Hide();
               double Spacing = 1000;

               using (Transaction trans = new Transaction(doc, "Change"))
               {
                  trans.Start();

                  foreach (Element element in tagsToAlign)
                  {

                     if (element is IndependentTag)
                     {
                        (element as IndependentTag).HasLeader = true;
                        (element as IndependentTag).LeaderEndCondition = LeaderEndCondition.Free;
                     }

                  }
                  trans.Commit();
               }



               Autodesk.Revit.DB.View view = doc.ActiveView;
               XYZ RightDirection = view.RightDirection;
               XYZ TopDirection = view.UpDirection;
               List<Element> finalTags = new List<Element>();
               if (Command.frm.VM.D1)
               {
                  if (Command.frm.VM.OrderX)
                  {
                     finalTags = Helper.OrderPointsFromLeftToRight(view, tagsToAlign, false);
                  }
                  else
                  {
                     finalTags = Helper.OrderPointsFromTopToBot(view, tagsToAlign, true);
                  }
                  degree = 180 - Command.frm.VM.SliderValue;
               }
               else if (Command.frm.VM.D2)
               {
                  if (Command.frm.VM.OrderX)
                  {
                     finalTags = Helper.OrderPointsFromLeftToRight(view, tagsToAlign, true);
                  }
                  else
                  {
                     finalTags = Helper.OrderPointsFromTopToBot(view, tagsToAlign, true);
                  }

                  degree = Command.frm.VM.SliderValue;
               }
               else if (Command.frm.VM.D3)
               {
                  if (Command.frm.VM.OrderX)
                  {
                     finalTags = Helper.OrderPointsFromLeftToRight(view, tagsToAlign, true);
                  }
                  else
                  {
                     finalTags = Helper.OrderPointsFromTopToBot(view, tagsToAlign, false);
                  }
                  degree = 180 + Command.frm.VM.SliderValue;
               }
               else if (Command.frm.VM.D4)
               {
                  if (Command.frm.VM.OrderX)
                  {
                     finalTags = Helper.OrderPointsFromLeftToRight(view, tagsToAlign, false);
                  }
                  else
                  {
                     finalTags = Helper.OrderPointsFromTopToBot(view, tagsToAlign, true);
                  }
                  degree = 360 - Command.frm.VM.SliderValue;
               }

               Command.FinalTags = finalTags;


               if (Command.frm.VM.IsAuto)
               {
                  //Spacing = 1000 * 0.00328084; //
                  Spacing = Helper.GetTagTextHeight(doc.ActiveView, Command.FinalTags?.FirstOrDefault()); //
               }
               else
               {
                  Spacing = Command.frm.VM.DimSpacing * 0.00328084;
               }

               using (Transaction trans = new Transaction(doc, "Change"))
               {
                  trans.Start();
                  SketchPlane sp = SketchPlane.Create(doc, Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin));

                  doc.ActiveView.SketchPlane = sp;

                  // Finally, we are able to PickPoint()
                  XYZ NewTargetHeadPoint = UiDoc.Selection.PickPoint();
                  //var NewTargetHeadPoint = UiDoc.Selection.PickPoint("Pick Tag Point");

                  double i = 0;
                  if (view is ViewPlan)
                  {
                     foreach (IndependentTag tag in Command.FinalTags)
                     {
                        tag.TagHeadPosition = new XYZ(NewTargetHeadPoint.X, NewTargetHeadPoint.Y + i, NewTargetHeadPoint.Z);
#if (R2020 || R2021)
                        var reference = tag.GetTaggedReference();

                        XYZ point = tag.LeaderEnd;
                         Line SlopedLine = Helper.GetLeaderLine(view, point, degree, 1000);
                        XYZ NewElbowPoint = Helper.GetElbowPoint(view, tag.TagHeadPosition, SlopedLine);
                        tag.LeaderElbow = NewElbowPoint;
                        
#else
                        var references = tag.GetTaggedReferences();
                        var reference = references.FirstOrDefault();
                        XYZ point = tag.GetLeaderEnd(reference);
                        Line SlopedLine = Helper.GetLeaderLine(view, point, degree, 1000);
                        XYZ NewElbowPoint = Helper.GetElbowPoint(view, tag.TagHeadPosition, SlopedLine);
                        tag.SetLeaderElbow(reference, NewElbowPoint);
#endif



                        i = i - Spacing;
                     }
                  }
                  else
                  {

                     foreach (IndependentTag tag in Command.FinalTags)
                     {
                        tag.TagHeadPosition = new XYZ(NewTargetHeadPoint.X, NewTargetHeadPoint.Y, NewTargetHeadPoint.Z + i);

#if (R2020 || R2021)
                        var reference = tag.GetTaggedReference();

                        XYZ point = tag.LeaderEnd;
                         Line SlopedLine = Helper.GetLeaderLine(view, point, degree, 1000);
                        XYZ NewElbowPoint = Helper.GetElbowPoint(view, tag.TagHeadPosition, SlopedLine);
                        tag.LeaderElbow = NewElbowPoint;
                        
#else
                        var references = tag.GetTaggedReferences();
                        var reference = references.FirstOrDefault();
                        XYZ point = tag.GetLeaderEnd(reference);
                        Line SlopedLine = Helper.GetLeaderLine(view, point, degree, 1000);
                        XYZ NewElbowPoint = Helper.GetElbowPoint(view, tag.TagHeadPosition, SlopedLine);
                        tag.SetLeaderElbow(reference, NewElbowPoint);
#endif

                        i = i - Spacing;
                     }
                  }
                  trans.Commit();
               }


               Command.frm.ShowDialog();

            }
            else
            {
               MessageBox.Show("No Tags Selected", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
         }
         catch (Exception ex)
         {

            MessageBox.Show(ex.Message, "First Option", MessageBoxButton.OK, MessageBoxImage.Error);
         }
      }
      #endregion
   }
}
