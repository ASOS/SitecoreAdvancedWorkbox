// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.WorkflowsPanel
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentManager.Panels;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Workflows;
using System.Web;
using System.Web.UI;

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
  public class WorkflowsPanel : DropDownPanel
  {
    public WorkflowsPanel()
    {
      this.Id = "WorkflowsPanel";
      this.Gallery = "Gallery.Workflows";
    }

    protected override void RenderPanel(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull((object) output, "output");
      IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
      if (workflowProvider == null)
        return;
      IWorkflow[] workflows = workflowProvider.GetWorkflows();
      string string1 = workflows.Length.ToString();
      for (int index = 0; index < workflows.Length; ++index)
      {
        IWorkflow workflow = workflows[index];
        string str = index == 0 ? "scRibbonToolbarSmallButtonDown" : "scRibbonToolbarSmallButton";
        string string2 = Registry.GetString("/Current_User/Workbox/WorkflowID", "");
        if (!string.IsNullOrWhiteSpace(string2))
          str = workflow.WorkflowID.Equals(string2) ? "scRibbonToolbarSmallButtonDown" : "scRibbonToolbarSmallButton";
        output.Write("<a href=\"#\" class=\"" + str + ("\" onclick=\"javascript:return scForm.postRequest('','','','wf:changewf(id=" + workflow.WorkflowID + ")')\"> "));
        output.Write("<span class=\"scRibbonToolbarSmallButtonPrefix header\">");
        output.Write('(');
        output.Write(index + 1);
        output.Write(' ');
        output.Write(Translate.Text("of"));
        output.Write(' ');
        output.Write(string1);
        output.Write(')');
        output.Write("</span>");
        this.RenderLabel(output, StringUtil.Clip(HttpUtility.HtmlEncode(workflow.Appearance.DisplayName).Replace("'", "&#39;"), 50, true));
        output.Write("</a>");
      }
    }

    private void RenderLabel(HtmlTextWriter output, string html)
    {
      Assert.ArgumentNotNull((object) output, "output");
      Assert.ArgumentNotNull((object) html, "html");
      Tag tag = new Tag("span", html);
      tag["class"] = "scRibbonToolbarSmallButtonLabel header";
      if (this.Width > 0)
        tag["style"] = "width: " + (object) (this.Width - 45) + "px";
      tag.ToString(output);
    }
  }
}
