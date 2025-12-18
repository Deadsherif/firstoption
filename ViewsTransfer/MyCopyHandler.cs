// Decompiled with JetBrains decompiler
// Type: ViewsTransfer.MyCopyHandler
// Assembly: ViewsTransfer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 097D957F-A3C8-4D58-A2D2-2E99F75DDB65
// Assembly location: C:\Users\ahmed\Downloads\ViewsTransfer\ViewsTransfer\bin\Debug\ViewsTransfer.dll

using Autodesk.Revit.DB;


namespace ViewsTransfer
{
  internal class MyCopyHandler : IDuplicateTypeNamesHandler
  {
    public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
    {
      return DuplicateTypeAction.UseDestinationTypes;
    }
  }
}
