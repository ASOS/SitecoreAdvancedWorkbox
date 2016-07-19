// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.WorkflowItems
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
  public class WorkflowItems
  {
    public string SerialNumber { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }

    public string ModifiedDate { get; set; }

    public string Language { get; set; }

    public string Editor { get; set; }

    public string Version { get; set; }

    public string Comment { get; set; }

    public string PublishingStartDate { get; set; }

    public string RelatedPath { get; set; }

    public string ItemGuid { get; set; }

    public WorkflowItems()
    {
    }

      public WorkflowItems(string sno, string name, string path, string modifiedDate, string language, string editor,
          string version, string comment, string publishingStartDate, string relatedPath, string temGuid)
      {
          this.SerialNumber = sno;
          this.Name = name;
          this.Path = path;
          this.ModifiedDate = modifiedDate;
          this.Language = language;
          this.Editor = editor;
          this.Version = version;
          this.Comment = comment;
          this.PublishingStartDate = publishingStartDate;
          this.RelatedPath = relatedPath;
          this.ItemGuid = ItemGuid;
      }
  }
}
