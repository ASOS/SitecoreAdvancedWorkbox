// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.GalleryWorkflowsForm
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentManager.Galleries;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Workflows;
using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
  public class GalleryWorkflowsForm : GalleryForm
  {
    protected Scrollbox Options;

    public override void HandleMessage(Message message)
    {
      Assert.ArgumentNotNull((object) message, "message");
      if (!message.Name.StartsWith("wf:"))
        return;
      this.Invoke(message, true);
      message.CancelBubble = true;
      message.CancelDispatch = true;
    }

    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull((object) e, "e");
      base.OnLoad(e);
      if (Context.ClientPage.IsEvent)
        return;
      IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
      HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter) new StringWriter());
      if (workflowProvider != null)
      {
        IWorkflow[] workflows = workflowProvider.GetWorkflows();
        string string1 = workflows.Length.ToString();
        for (int index = 0; index < workflows.Length; ++index)
        {
          IWorkflow workflow = workflows[index];
          string str = index == 0 ? "scRibbonToolbarSmallButtonDown" : "scRibbonToolbarSmallButton";
          string string2 = Registry.GetString("/Current_User/Workbox/WorkflowID", "");
          if (!string.IsNullOrWhiteSpace(string2))
            str = workflow.WorkflowID.Equals(string2) ? "scRibbonToolbarSmallButtonDown" : "scRibbonToolbarSmallButton";
          htmlTextWriter.Write("<a href=\"#\" class=\"" + str + ("\" onclick=\"javascript:return scForm.postRequest('','','','wf:changewf(id=" + workflow.WorkflowID + ")')\"> "));
          htmlTextWriter.Write("<span class=\"scRibbonToolbarSmallButtonPrefix header\">");
          htmlTextWriter.Write('(');
          htmlTextWriter.Write(index + 1);
          htmlTextWriter.Write(' ');
          htmlTextWriter.Write(Translate.Text("of"));
          htmlTextWriter.Write(' ');
          htmlTextWriter.Write(string1);
          htmlTextWriter.Write(')');
          htmlTextWriter.Write("</span>");
          htmlTextWriter.Write(StringUtil.Clip(HttpUtility.HtmlEncode(workflow.Appearance.DisplayName).Replace("'", "&#39;"), 50, true));
          htmlTextWriter.Write("</a>");
        }
      }
      this.Options.Controls.Add((System.Web.UI.Control) new LiteralControl(htmlTextWriter.InnerWriter.ToString()));
    }
  }
}
