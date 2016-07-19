// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.FiltersPanel
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Workflows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
  public class FiltersPanel : RibbonPanel
  {
    public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
    {
      output.Write("<div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text("State:"));
      output.Write("</td><td>");
      output.Write("<select class=\"scWorkboxPageSizeCombobox\" id=\"States\" >");
      IWorkflowProvider workflowProvider = Context.ContentDatabase.WorkflowProvider;
      if (workflowProvider != null)
      {
        string string1 = Registry.GetString("/Current_User/Workbox/WorkflowID", ((IEnumerable<IWorkflow>) workflowProvider.GetWorkflows()).FirstOrDefault<IWorkflow>().WorkflowID);
        IWorkflow workflow = workflowProvider.GetWorkflow(string1);
        string str = string.Empty;
        if (Context.ClientPage.Session["StateID"] != null)
        {
          string string2 = Context.ClientPage.Session["StateID"].ToString();
          if (!string.IsNullOrWhiteSpace(string2) && workflow.GetState(string2) != null)
            str = string2;
        }
        foreach (WorkflowState workflowState in ((IEnumerable<WorkflowState>) workflow.GetStates()).Where<WorkflowState>((Func<WorkflowState, bool>) (state => !state.FinalState)))
        {
          if (WorkflowFilterer.FilterVisibleCommands(workflow.GetCommands(workflowState.StateID)).Length > 0)
            output.Write(string.Format("<option value=\"{0}\"" + (workflowState.StateID.Equals(str) ? " selected=\"selected\" " : string.Empty) + ">{1}</option>", (object) workflowState.StateID, (object) workflowState.DisplayName));
        }
      }
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");
      string str1 = string.Empty;
      if (Context.ClientPage.Session["Filter"] != null)
      {
        string @string = Context.ClientPage.Session["Filter"].ToString();
        if (!string.IsNullOrWhiteSpace(@string))
          str1 = @string;
      }
      output.Write("<div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text("Filter:"));
      output.Write("</td><td>");
      output.Write("<select class=\"scWorkboxPageSizeCombobox\" id=\"Filter\" >");
      output.Write("<option value=\"none\"" + (str1.Equals("none") ? " selected=\"selected\" " : string.Empty) + ">None</option>");
      output.Write("<option value=\"my\"" + (str1.Equals("my") ? " selected=\"selected\" " : string.Empty) + ">My Items</option>");
      output.Write("<option value=\"notmy\"" + (str1.Equals("notmy") ? " selected=\"selected\" " : string.Empty) + ">Not My Items</option>");
      output.Write("<option value=\"media\"" + (str1.Equals("media") ? " selected=\"selected\" " : string.Empty) + ">Media Library Items</option>");
      output.Write("<option value=\"notmedia\"" + (str1.Equals("notmedia") ? " selected=\"selected\" " : string.Empty) + ">Not Media Library Items</option>");
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");
      string str2 = string.Empty;
      if (Context.ClientPage.Session["Language"] != null)
      {
        string @string = Context.ClientPage.Session["Language"].ToString();
        if (!string.IsNullOrWhiteSpace(@string))
          str2 = @string;
          
      }
      output.Write("<div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text("Language:"));
      output.Write("</td><td>");
      output.Write("<select class=\"scWorkboxPageSizeCombobox\" id=\"Language\" >");
      output.Write("<option value=\"All\">All Languages</option>");
      foreach (Language language in (Collection<Language>) Context.ContentDatabase.GetLanguages())
        output.Write(string.Format("<option value=\"{0}\"" + (language.Name.Equals(str2) ? " selected=\"selected\" " : string.Empty) + ">{1}</option>", (object) language.Name, (object) language.CultureInfo.DisplayName));
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");
      int @int = Registry.GetInt("/Current_User/Workbox/PageSize", 10);
      output.Write("<div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text("Items per Page:"));
      output.Write("</td><td>");
      output.Write("<select class=\"scWorkboxPageSizeCombobox\" id=\"PageSize\" onchange='javascript:scForm.invoke(\"PageSize_Change\")'>");
      output.Write("<option value=\"10\"" + (@int == 10 ? " selected=\"selected\"" : string.Empty) + ">10</option>");
      output.Write("<option value=\"25\"" + (@int == 25 ? " selected=\"selected\"" : string.Empty) + ">25</option>");
      output.Write("<option value=\"50\"" + (@int == 50 ? " selected=\"selected\"" : string.Empty) + ">50</option>");
      output.Write("<option value=\"100\"" + (@int == 100 ? " selected=\"selected\"" : string.Empty) + ">100</option>");
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");

      //publishing sort <input type="text" name="firstname">
      string @order = "ASC";
      output.Write("<br/><br/><div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text("Sort by Published Date:"));
      output.Write("</td><td>");
      output.Write("<select class=\"scWorkboxPageSizeCombobox\" id=\"PublishedDate\" onchange='javascript:scForm.invoke(\"PublishedDate_Change\")'>");
      output.Write("<option value=\"ASC\"" + (@order == "ASC" ? " selected=\"selected\"" : string.Empty) + ">ASC</option>");
      output.Write("<option value=\"DESC\"" + (@order == "DESC" ? " selected=\"selected\"" : string.Empty) + ">DESC</option>");
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");

      //Item Path
      string itemPath;
      output.Write("<div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text(" Item Guid:"));
      output.Write("</td><td>");
      output.Write("<input type=\"text\" id=\"ItemPath\" name=\"ItemPath\" class=\"scWorkboxPageSizeCombobox\" height=\"20\" />");
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");

      //All versions or Latest
      string strVer = string.Empty;
      if (Context.ClientPage.Session["ShowVersions"] != null)
      {
          string @string = Context.ClientPage.Session["ShowVersions"].ToString();
          if (!string.IsNullOrWhiteSpace(@string))
              strVer = @string;
      }
      output.Write("<div class=\"scRibbonToolbarPanel\">");
      output.Write("<table class=\"scWorkboxPageSize\"><tr><td class=\"scWorkboxPageSizeLabel\">");
      output.Write(Translate.Text("Versions:"));
      output.Write("</td><td>");
      output.Write("<select class=\"scWorkboxPageSizeCombobox\" id=\"ShowVersions\" >");
      output.Write("<option value=\"Latest\"" + (strVer.Equals("Latest") ? " selected=\"selected\" " : string.Empty) + ">Latest</option>");
      output.Write("<option value=\"All\"" + (strVer.Equals("All") ? " selected=\"selected\" " : string.Empty) + ">All</option>");
      output.Write("</select>");
      output.Write("</td></tr></table>");
      output.Write("</div>");
    }
  }
}
