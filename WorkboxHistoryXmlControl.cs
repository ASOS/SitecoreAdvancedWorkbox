// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.WorkboxHistoryXmlControl
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Workflows;
using System;
using System.Collections.Specialized;

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
  public class WorkboxHistoryXmlControl : XmlControl
  {
    private string m_itemID;
    private string m_language;
    private string m_version;
    private string m_workflowID;
    protected Border History;

    public string ItemID
    {
      get
      {
        return this.m_itemID;
      }
      set
      {
        this.m_itemID = value;
      }
    }

    public string Language
    {
      get
      {
        return this.m_language;
      }
      set
      {
        this.m_language = value;
      }
    }

    public string Version
    {
      get
      {
        return this.m_version;
      }
      set
      {
        this.m_version = value;
      }
    }

    public string WorkflowID
    {
      get
      {
        return this.m_workflowID;
      }
      set
      {
        this.m_workflowID = value;
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      IWorkflowProvider workflowProvider = Sitecore.Context.ContentDatabase.WorkflowProvider;
      if (workflowProvider == null)
        return;
      IWorkflow workflow = workflowProvider.GetWorkflow(this.WorkflowID);
      Error.Assert(workflow != null, "Workflow \"" + this.WorkflowID + "\" not found.");
      Item obj = Sitecore.Context.ContentDatabase.Items[this.ItemID, Sitecore.Globalization.Language.Parse(this.Language), Sitecore.Data.Version.Parse(this.Version)];
      if (obj == null)
        return;
      NameValueCollection nameValueCollection1 = new NameValueCollection();
      NameValueCollection nameValueCollection2 = new NameValueCollection();
      foreach (WorkflowState state in workflow.GetStates())
      {
        nameValueCollection1.Add(state.StateID, state.DisplayName);
        nameValueCollection2.Add(state.StateID, state.Icon);
      }
      WorkflowEvent[] history = workflow.GetHistory(obj);
      string name = Sitecore.Context.Domain.Name;
      foreach (WorkflowEvent workflowEvent in history)
      {
        string text = workflowEvent.User;
        if (text.StartsWith(name + "\\", StringComparison.OrdinalIgnoreCase))
          text = StringUtil.Mid(text, name.Length + 1);
        string string1 = StringUtil.GetString(new string[2]
        {
          text,
          Translate.Text("Unknown")
        });
        string str1 = nameValueCollection2[workflowEvent.NewState];
        string string2 = StringUtil.GetString(new string[2]
        {
          nameValueCollection1[workflowEvent.OldState],
          "?"
        });
        string string3 = StringUtil.GetString(new string[2]
        {
          nameValueCollection1[workflowEvent.NewState],
          "?"
        });
        string str2 = DateUtil.FormatDateTime(workflowEvent.Date, "D", Sitecore.Context.User.Profile.Culture);
        XmlControl xmlControl = Resource.GetWebControl("WorkboxHistoryEntryCustom") as XmlControl;
        this.History.Controls.Add((System.Web.UI.Control) xmlControl);
        xmlControl["User"] = (object) string1;
        xmlControl["Icon"] = (object) str1;
        xmlControl["Date"] = (object) str2;
        xmlControl["Action"] = (object) string.Format(Translate.Text("Changed from <b>{0}</b> to <b>{1}</b>."), (object) string2, (object) string3);
        xmlControl["Text"] = (object) workflowEvent.Text;
      }
    }
  }
}
