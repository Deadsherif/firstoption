using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModelHealthChecker.ExternalEventHandlers
{
    public class DeleteLinksEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            Element Link = Command.VM.SelectedRevitLink;


            MessageBoxResult dialog = MessageBox.Show($"Are you sure you want to delete {Link.Name}?\nIf deleted, this operation can't be undone.", "FirstOpion", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (dialog == MessageBoxResult.Yes)
            {

                using (Transaction trans = new Transaction(doc, "Delete Link"))
                {
                    trans.Start();
                    try
                    {

                        int Mode = 1;
                        int index = 0;

                        string Name = Link.Name;

                        if (Command.VM.ImportedLinks.Contains(Link))
                        {
                            Mode = 1;
                            index = Command.VM.ImportedLinks.IndexOf(Link);
                        }
                        else
                        {
                            Mode = 2;
                            index = Command.VM.ImportedCAD.IndexOf(Link);
                        }

                        doc.Delete(Link.Id);

                        if (Mode == 1)
                        {
                            Command.VM.ImportedLinks.RemoveAt(index);
                        }
                        else
                        {
                            Command.VM.ImportedCAD.RemoveAt(index);

                        }

                        MessageBox.Show($"{Name} has been deleted.", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    trans.Commit();
                }

            }



        }

        public string GetName()
        {
            return "A";
        }
    }
}
