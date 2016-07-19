// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.SelectColumnsPage
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

using Sitecore.Controls;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.XamlSharp.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
  public class SelectColumnsPage : DialogPage
  {
    protected Scrollbox Columns;

    protected override void OK_Click()
    {
      HttpContext current = HttpContext.Current;
      Assert.IsNotNull((object) current, typeof (HttpContext));
      ListString listString = new ListString();
      foreach (string key in current.Request.Form.Keys)
      {
        if (!string.IsNullOrWhiteSpace(key) && key.StartsWith("Field_"))
          listString.Add(key.Split('_')[1]);
      }
      Registry.SetValue("/Current_User/Workbox/SelectColumns", listString.ToString());
      base.OK_Click();
    }

    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull((object) e, "e");
      base.OnLoad(e);
      if (XamlControl.AjaxScriptManager.IsEvent)
        return;
      HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter) new StringWriter());
      ListString source1 = new ListString(Registry.GetValue("/Current_User/Workbox/SelectColumns"));
      ListString source2 = new ListString(Registry.GetValue("/Current_User/Workbox/Columns"));
      if (source1 != null)
      {
        List<string> list1 = source1.ToList<string>();
        List<string> list2 = source2.ToList<string>();
        if (list1.Count > 0)
        {
          foreach (string str1 in list2)
          {
            string str2 = "Field_" + str1;
            string str3 = list1.Contains(str1) ? " checked=\"true\"" : string.Empty;
            htmlTextWriter.Write("<div style=\"padding:2px 0px 2px 0px\">");
            htmlTextWriter.Write("<input id=\"" + str2 + "\" name=\"" + str2 + "\" type=\"checkbox\"" + str3 + " />");
            htmlTextWriter.Write("<label for=\"" + str2.Split('_')[1] + "\">");
            htmlTextWriter.Write(Translate.Text(str2.Split('_')[1]));
            htmlTextWriter.Write("</label>");
            htmlTextWriter.Write("</div>");
          }
        }
      }
      this.Columns.InnerHtml = htmlTextWriter.InnerWriter.ToString();
    }
  }
}
