
// Decompiled with JetBrains decompiler
// Type: SitecoreAdvancedWorkbox.Shell.Applications.Workbox
// Assembly: SitecoreAdvancedWorkbox, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D7941FCF-7B2D-4C88-BFFF-23797D3F62A0
// Assembly location: C:\Users\rohit.gautam\Downloads\Advanced Workbox-1.0\package\files\bin\SitecoreAdvancedWorkbox.dll

using ComponentArt.Web.UI;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Sitecore.Extensions;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell.Data;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Sites;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Grids;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Web.UI.XamlSharp.Ajax;
using Sitecore.Web.UI.XmlControls;
using Sitecore.Workflows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.HtmlControls;

namespace SitecoreAdvancedWorkbox.Shell.Applications
{
    public class Workbox : Sitecore.Web.UI.HtmlControls.Page, IHasCommandContext
    {
        protected HtmlForm WorkboxForm;
        protected Ribbon RibbonPanel;
        protected Border ItemsContainer;
        protected Grid ItemsGrid;
        protected Panel PanelContainer;
        protected ClientTemplate LoadingFeedbackTemplate;
        protected ClientTemplate SliderTemplate;

        protected IWorkflowProvider WFProvider
        {
            get { return Sitecore.Context.ContentDatabase.WorkflowProvider; }
        }

        CommandContext IHasCommandContext.GetCommandContext()
        {
            CommandContext commandContext = new CommandContext();
            Item itemNotNull = Sitecore.Client.GetItemNotNull("/sitecore/content/Applications/Advanced Workbox/Ribbon",
                Sitecore.Client.CoreDatabase);
            commandContext.Parameters["item"] = GridUtil.GetSelectedValue("ItemsGrid");
            commandContext.RibbonSourceUri = itemNotNull.Uri;
            return commandContext;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Sitecore.Context.State.DataBind = false;
            Sitecore.Client.AjaxScriptManager.OnExecute += new AjaxScriptManager.ExecuteDelegate(this.Current_OnExecute);
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object) e, "e");
            base.OnLoad(e);
            Assert.CanRunApplication("Advanced Workbox");
            if (!this.Page.IsPostBack)
                this.Bind_Grid();
            if (!Sitecore.Context.ClientPage.IsEvent && !this.ItemsGrid.IsCallback)
            {
                GridColumnCollection columns = this.ItemsGrid.Levels[(object) 0].Columns;
                ListString listString = new ListString();
                foreach (GridColumn gridColumn in (CollectionBase) columns)
                {
                    if (!gridColumn.DataField.Equals("scGridID"))
                        listString.Add(gridColumn.DataField);
                }
                Registry.SetValue("/Current_User/Workbox/Columns", listString.ToString());
            }
            this.ItemsGrid.PageSize = Registry.GetInt("/Current_User/Workbox/PageSize", 10);
            this.ItemsGrid.LocalizeGrid();
        }

        private void Bind_Grid()
        {
            if (!string.IsNullOrWhiteSpace(Registry.GetValue("/Current_User/Workbox/SelectColumns")))
            {
                List<string> list =
                    new ListString(Registry.GetValue("/Current_User/Workbox/SelectColumns")).ToList<string>();
                GridColumnCollection columns = this.ItemsGrid.Levels[(object) 0].Columns;
                ListString listString = new ListString();
                int num = 0;
                foreach (GridColumn gridColumn in (CollectionBase) columns)
                {
                    if (!list.Contains(gridColumn.DataField) && !gridColumn.DataField.Equals("sc_GridId"))
                    {
                        gridColumn.Visible = false;
                        gridColumn.DataCellCssClass = "invisible";
                        gridColumn.HeadingCellCssClass = "invisible";
                    }
                    ++num;
                }
            }
            IWorkflow workflow =
                this.WFProvider.GetWorkflow(Registry.GetString("/Current_User/Workbox/WorkflowID",
                    ((IEnumerable<IWorkflow>) this.WFProvider.GetWorkflows()).FirstOrDefault<IWorkflow>().WorkflowID));
            WorkflowState state = ((IEnumerable<WorkflowState>) workflow.GetStates()).FirstOrDefault<WorkflowState>();
            if (Sitecore.Context.ClientPage.Session["StateID"] != null)
            {
                string @string = Sitecore.Context.ClientPage.Session["StateID"].ToString();
                if (workflow.GetState(@string) != null)
                    state = workflow.GetState(@string);
            }
            IEnumerable<WorkflowItems> workflowItemses = this.GetItems(state, workflow);
            if (workflowItemses == null)
                return;
            if (Sitecore.Context.ClientPage.Session["Language"] != null)
            {
                object languageFilter = Sitecore.Context.ClientPage.Session["Language"];
                if (!string.IsNullOrWhiteSpace(languageFilter.ToString()) && !languageFilter.ToString().Equals("All"))
                    workflowItemses =
                        workflowItemses.Where<WorkflowItems>(
                            (Func<WorkflowItems, bool>) (wf => wf.Language.Equals(languageFilter.ToString())));
            }
            if (Sitecore.Context.ClientPage.Session["ItemPath"] != null)
            {
                object itemPath = Sitecore.Context.ClientPage.Session["ItemPath"];
                if (!string.IsNullOrWhiteSpace(itemPath.ToString()))
                    workflowItemses =
                        workflowItemses.Where<WorkflowItems>(
                            (Func<WorkflowItems, bool>) (wf => wf.ItemGuid == itemPath.ToString().Trim() || wf.RelatedPath == itemPath.ToString().Trim()));
            }
            if (Sitecore.Context.ClientPage.Session["PublishedDate"] != null)
            {
                object publishedOrder= Sitecore.Context.ClientPage.Session["PublishedDate"];
                if (!string.IsNullOrWhiteSpace(publishedOrder.ToString()))
                {
                    if (publishedOrder.ToString().ToUpper().Trim() == "DESC")
                    {
                        workflowItemses =
                            workflowItemses.OrderByDescending(wf => wf.PublishingStartDate);
                    }
                    else
                    {

                        workflowItemses =
                               workflowItemses.OrderBy(wf => wf.PublishingStartDate);
                    }
                    
                }
            }

            ComponentArtGridHandler<WorkflowItems>.Manage(this.ItemsGrid,
                (IGridSource<WorkflowItems>) new GridSource<WorkflowItems>(workflowItemses), true);
        }

        private void Current_OnExecute(object sender, AjaxCommandEventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull((object) args, "args");
            switch (args.Name)
            {
                case "wf:changewf":
                    Sitecore.Context.ClientPage.Session.Clear();
                    Registry.SetString("/Current_User/Workbox/WorkflowID", args.Parameters["id"]);
                    this.Reload();
                    break;
                case "wf:refresh":
                    SheerResponse.Eval("filters()");
                    break;
                case "wf:filters":
                    Sitecore.Context.ClientPage.Session["StateID"] = (object) args.Parameters["state"];
                    Sitecore.Context.ClientPage.Session["Filter"] = (object) args.Parameters["filter"];
                    Sitecore.Context.ClientPage.Session["Language"] = (object) args.Parameters["language"];
                    Sitecore.Context.ClientPage.Session["ItemPath"] = (object) args.Parameters["itempath"];
                    Sitecore.Context.ClientPage.Session["PublishedDate"] = (object) args.Parameters["PublishedDate"];
                    Sitecore.Context.ClientPage.Session["ShowVersions"] = (object)args.Parameters["ShowVersions"];
                    Registry.SetInt("/Current_User/Workbox/PageSize", int.Parse(args.Parameters["pagesize"]));
                    this.Reload();
                    break;
                case "wf:sendselected":
                    this.Send(args.Parameters, args.Name);
                    break;
                case "wf:sendall":
                    this.SendAll(args.Parameters, args.Name);
                    break;
                case "wf:open":
                case "wf:preview":
                case "wf:diff":
                case "wf:history":
                    WorkflowItems workflowItems = this.GridSelectedItem();
                    if (workflowItems != null)
                    {
                        string language = workflowItems.Language;
                        string version = workflowItems.Version;
                        string string1 =
                            Sitecore.Context.ContentDatabase.Items[
                                workflowItems.Path, Language.Parse(language), Sitecore.Data.Version.Parse(version)].ID
                                .ToString();
                        string string2 = Registry.GetString("/Current_User/Workbox/WorkflowID");
                        if (args.Name == "wf:open")
                            this.Open(string1, language, version);
                        if (args.Name == "wf:preview")
                            this.Preview(string1, language, version);
                        if (args.Name == "wf:diff")
                            this.Diff(string1, language, version);
                        if (!(args.Name == "wf:history"))
                            break;
                        Workbox.ShowHistory(string1, language, version, string2,
                            Sitecore.Context.ClientPage.ClientRequest.Control);
                        break;
                    }
                    SheerResponse.Alert("There are no selected items.");
                    break;
                case "wf:selectcolumns":
                    GridColumnCollection columns = this.ItemsGrid.Levels[(object) 0].Columns;
                    ListString listString = new ListString();
                    foreach (GridColumn gridColumn in (CollectionBase) columns)
                    {
                        if (gridColumn.Visible)
                            listString.Add(gridColumn.DataField);
                    }
                    Registry.SetValue("/Current_User/Workbox/SelectColumns", listString.ToString());
                    SheerResponse.ShowModalDialog(
                        new UrlString("/sitecore/shell/~/xaml/Sitecore.Shell.Applications.NewWorkbox.SelectColumns.aspx")
                            .ToString(), true);
                    this.Reload();
                    break;
            }
        }

        private IEnumerable<WorkflowItems> GetItems(WorkflowState state, IWorkflow workflow)
        {
            Assert.ArgumentNotNull((object) state, "state");
            Assert.ArgumentNotNull((object) workflow, "workflow");
            string str = string.Empty;
            if (Sitecore.Context.ClientPage.Session["Filter"] != null)
                str = Sitecore.Context.ClientPage.Session["Filter"].ToString();

            string strVer = string.Empty;
            if (Sitecore.Context.ClientPage.Session["ShowVersions"] != null)
                strVer = Sitecore.Context.ClientPage.Session["ShowVersions"].ToString();

            List<WorkflowItems> workflowItemsList = new List<WorkflowItems>();
            DataUri[] items = workflow.GetItems(state.StateID);
            if (items != null)
            {
                int itemCount = 1;
                foreach (DataUri index in items)
                {
                    Item obj1 = Sitecore.Context.ContentDatabase.Items[index];
                    if (obj1 != null && obj1.Access.CanRead() &&
                        (obj1.Access.CanReadLanguage() && obj1.Access.CanWriteLanguage()) &&
                        (Sitecore.Context.IsAdministrator || obj1.Locking.CanLock() || obj1.Locking.HasLock()))
                    {
                        Item obj2 = (Item) null;
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            if (str.Equals("none"))
                                obj2 = obj1;
                            else if (str.Equals("my"))
                            {
                                if (obj1.Locking.HasLock())
                                    obj2 = obj1;
                            }
                            else if (str.Equals("notmy"))
                            {
                                if (!obj1.Locking.HasLock())
                                    obj2 = obj1;
                            }
                            else if (str.Equals("media"))
                            {
                                if (obj1.Paths.IsMediaItem)
                                    obj2 = obj1;
                            }
                            else if (str.Equals("notmedia") && !obj1.Paths.IsMediaItem)
                                obj2 = obj1;
                        }
                        else
                            obj2 = obj1;
                        if (obj2 != null)
                        {
                            if (strVer.ToUpper() == "ALL")
                            {
                                foreach (var versionedItem in obj2.Versions.GetVersions())
                                {
                                    WorkflowItems workflowItem = this.GetWorkflowItem(versionedItem, workflow, itemCount);
                                    workflowItemsList.Add(workflowItem);


                                }
                            }
                            else
                            {
                                WorkflowItems workflowItem = this.GetWorkflowItem(obj2, workflow, itemCount);
                                workflowItemsList.Add(workflowItem);

                            }

                            var dsItems = GetRenderingDataSourceItems(obj2);
                            foreach (var item in dsItems)
                            {
                                WorkflowItems workflowItem = this.GetWorkflowItem(item, workflow, itemCount, obj2.ID.ToString());
                                workflowItemsList.Add(workflowItem);
                            }
                            
                            
                        }
                    }
                    ++itemCount;
                }
            }
            return (IEnumerable<WorkflowItems>) workflowItemsList;
        }

        private WorkflowItems GetWorkflowItem(Item item, IWorkflow workflow, int itemCount, string relatedPath = "")
        {
            
            WorkflowItems workflowItems = new WorkflowItems();
            workflowItems.SerialNumber = itemCount.ToString();
            workflowItems.Name = item.Name;
            workflowItems.Path = item.Paths.FullPath;
            DateTime dateTime = DateUtil.ParseDateTime(item.Fields["__Updated"].Value, DateTime.Today);
            workflowItems.ModifiedDate = dateTime.ToString("yyyy/MM/dd");
            workflowItems.Language = item.Language.ToString();
            workflowItems.Editor = item.Fields["__Updated by"].Value;
            WorkflowEvent[] history = workflow.GetHistory(item);
            workflowItems.Comment = history.Length <= 0 ? "" : history[history.Length - 1].Text;
            workflowItems.Version = item.Version.Number.ToString();
            workflowItems.PublishingStartDate = item.Fields["__Valid from"].Value == String.Empty
                ? ""
                : DateUtil.IsoDateToDateTime(DateUtil.IsoDateToUtcIsoDate(item.Fields["__Valid from"].Value))
                    .ToShortDateString() + " "
                  +
                  DateUtil.ToServerTime(DateUtil.IsoDateToDateTime(item.Fields["__Valid from"].Value)).ToShortTimeString();
            workflowItems.RelatedPath = relatedPath;
            workflowItems.ItemGuid = item.ID.ToString();

            return workflowItems;
        }

        private void Reload()
        {
            UrlString urlString = new UrlString(WebUtil.GetRawUrl());
            urlString["reload"] = "1";
            urlString["time"] = DateTime.Now.Ticks.ToString();
            SheerResponse.SetLocation(urlString.ToString());
        }

        private int FindColumnIndex(string name)
        {
            GridColumnCollection columns = this.ItemsGrid.Levels[(object) 0].Columns;
            int num1 = -1;
            for (int index = 0; index < columns.Count; ++index)
            {
                if (columns[(object) index].DataField == name)
                {
                    int num2;
                    return num2 = index;
                }
            }
            return num1;
        }

        private void Send(NameValueCollection parameters, string commandName)
        {
            Assert.ArgumentNotNull((object) parameters, "message");
            if (this.WFProvider == null)
                return;
            string workflowID = parameters["wf"];
            string str = parameters["ws"];
            IWorkflow workflow = this.WFProvider.GetWorkflow(workflowID);
            if (workflow == null)
                return;
            GridItemCollection selectedItems = this.ItemsGrid.SelectedItems;
            int num = 0;
            bool flag = false;
            if (selectedItems != null)
            {
                foreach (GridItem gridItem in (CollectionBase) selectedItems)
                {
                    int columnIndex1 = this.FindColumnIndex("Path");
                    int columnIndex2 = this.FindColumnIndex("Language");
                    int columnIndex3 = this.FindColumnIndex("Version");
                    Item obj =
                        Sitecore.Context.ContentDatabase.Items[
                            ((Array) gridItem.DataItem).GetValue(columnIndex1).ToString(),
                            Language.Parse(((Array) gridItem.DataItem).GetValue(columnIndex2).ToString()),
                            Sitecore.Data.Version.Parse(((Array) gridItem.DataItem).GetValue(columnIndex3).ToString())];
                    if (obj != null)
                    {
                        WorkflowState state = workflow.GetState(obj);
                        if (state.StateID.Equals(str))
                        {
                            try
                            {
                                workflow.Execute(parameters["command"], obj, state.DisplayName, true);
                            }
                            catch (WorkflowStateMissingException ex)
                            {
                                flag = true;
                            }
                        }
                        ++num;
                    }
                }
            }
            if (flag)
                SheerResponse.Alert(
                    "One or more items could not be processed because their workflow state does not specify the next step.");
            if (num == 0)
                SheerResponse.Alert("There are no selected items.", true);
            else
                SheerResponse.Eval("refresh()");
        }

        private void SendAll(NameValueCollection parameters, string commandName)
        {
            Assert.ArgumentNotNull((object) parameters, "message");
            if (this.WFProvider == null)
                return;
            string workflowID = parameters["wf"];
            string stateID = parameters["ws"];
            IWorkflow workflow = this.WFProvider.GetWorkflow(workflowID);
            if (workflow == null)
                return;
            IEnumerable<WorkflowItems> source = this.GetItems(workflow.GetState(stateID), workflow);
            if (source != null && Sitecore.Context.ClientPage.Session["Language"] != null)
            {
                object languageFilter = Sitecore.Context.ClientPage.Session["Language"];
                if (!string.IsNullOrWhiteSpace(languageFilter.ToString()) && !languageFilter.ToString().Equals("All"))
                    source =
                        source.Where<WorkflowItems>(
                            (Func<WorkflowItems, bool>) (wf => wf.Language.Equals(languageFilter.ToString())));
            }
            int num = 0;
            bool flag = false;
            if (source != null)
            {
                foreach (WorkflowItems workflowItems in source)
                {
                    Item obj =
                        Sitecore.Context.ContentDatabase.Items[
                            workflowItems.Path, Language.Parse(workflowItems.Language),
                            Sitecore.Data.Version.Parse(workflowItems.Version)];
                    if (obj != null)
                    {
                        WorkflowState state = workflow.GetState(obj);
                        if (state.StateID.Equals(stateID))
                        {
                            try
                            {
                                workflow.Execute(parameters["command"], obj, state.DisplayName, true);
                            }
                            catch (WorkflowStateMissingException ex)
                            {
                                flag = true;
                            }
                        }
                        ++num;
                    }
                }
            }
            if (flag)
                SheerResponse.Alert(
                    "One or more items could not be processed because their workflow state does not specify the next step.");
            if (num == 0)
                SheerResponse.Alert("There are no selected items.", true);
            else
                SheerResponse.Eval("refresh()");
        }

        private WorkflowItems GridSelectedItem()
        {
            int columnIndex1 = this.FindColumnIndex("SerialNumber");
            int columnIndex2 = this.FindColumnIndex("Name");
            int columnIndex3 = this.FindColumnIndex("Path");
            int columnIndex4 = this.FindColumnIndex("Language");
            int columnIndex5 = this.FindColumnIndex("ModifiedDate");
            int columnIndex6 = this.FindColumnIndex("Editor");
            int columnIndex7 = this.FindColumnIndex("Version");
            int columnIndex8 = this.FindColumnIndex("Comment");
            int columnIndex9 = this.FindColumnIndex("PublishingStartDate");
            int columnIndex10 = this.FindColumnIndex("RelatedPath");
            int columnIndex11 = this.FindColumnIndex("ItemGuid");

            GridItemCollection selectedItems = this.ItemsGrid.SelectedItems;
            if (selectedItems.Count > 0)
            {
                IEnumerator enumerator = selectedItems.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        GridItem gridItem = (GridItem) enumerator.Current;
                        string string1 = ((Array) gridItem.DataItem).GetValue(columnIndex1).ToString();
                        string string2 = ((Array) gridItem.DataItem).GetValue(columnIndex2).ToString();
                        string string3 = ((Array) gridItem.DataItem).GetValue(columnIndex3).ToString();
                        string string4 = ((Array) gridItem.DataItem).GetValue(columnIndex4).ToString();
                        string string5 = ((Array) gridItem.DataItem).GetValue(columnIndex5).ToString();
                        string string6 = ((Array) gridItem.DataItem).GetValue(columnIndex7).ToString();
                        string string7 = ((Array) gridItem.DataItem).GetValue(columnIndex6).ToString();
                        string string8 = ((Array) gridItem.DataItem).GetValue(columnIndex8).ToString();
                        string string9 = ((Array) gridItem.DataItem).GetValue(columnIndex9).ToString();
                        string string10 = ((Array)gridItem.DataItem).GetValue(columnIndex10).ToString();
                        string string11 = ((Array)gridItem.DataItem).GetValue(columnIndex11).ToString();

                        return new WorkflowItems(string1, string2, string3, string5, string4, string7, string6, string8,
                            string9, string10, string11);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }
            }
            return new WorkflowItems();
        }

        protected void Diff(string id, string language, string version)
        {
            Assert.ArgumentNotNull((object) id, "id");
            Assert.ArgumentNotNull((object) language, "language");
            Assert.ArgumentNotNull((object) version, "version");
            UrlString urlString = new UrlString(UIUtil.GetUri("control:Diff"));
            urlString.Append("id", id);
            urlString.Append("la", language);
            urlString.Append("vs", version);
            urlString.Append("wb", "1");
            SheerResponse.ShowModalDialog(urlString.ToString());
        }

        protected void Open(string id, string language, string version)
        {
            Assert.ArgumentNotNull((object) id, "id");
            Assert.ArgumentNotNull((object) language, "language");
            Assert.ArgumentNotNull((object) version, "version");
            string sectionId = RootSections.GetSectionID(id);
            UrlString urlString = new UrlString();
            urlString.Append("ro", sectionId);
            urlString.Append("fo", id);
            urlString.Append("id", id);
            urlString.Append("la", language);
            urlString.Append("vs", version);
            Windows.RunApplication("Content editor", urlString.ToString());
        }

        protected void Preview(string id, string language, string version)
        {
            Assert.ArgumentNotNull((object) id, "id");
            Assert.ArgumentNotNull((object) language, "language");
            Assert.ArgumentNotNull((object) version, "version");
            Assert.IsNotNull((object) Factory.GetSite(Settings.Preview.DefaultSite), "Site \"{0}\" not found",
                (object) Settings.Preview.DefaultSite);
            UrlString webSiteUrl = SiteContext.GetWebSiteUrl();
            webSiteUrl["sc_itemid"] = id;
            webSiteUrl["sc_mode"] = "preview";
            webSiteUrl["sc_lang"] = language;
            SheerResponse.Eval("window.open('" + (object) webSiteUrl + "', '_blank')");
        }

        private static void ShowHistory(string id, string language, string version, string workflowID, string control)
        {
            Assert.ArgumentNotNull((object) id, "id");
            Assert.ArgumentNotNull((object) language, "language");
            Assert.ArgumentNotNull((object) version, "version");
            Assert.ArgumentNotNull((object) control, "control");
            XmlControl xmlControl = Resource.GetWebControl("WorkboxHistoryCustom") as XmlControl;
            Assert.IsNotNull((object) xmlControl, "history is null");
            xmlControl["ItemID"] = (object) id;
            xmlControl["Language"] = (object) language;
            xmlControl["Version"] = (object) version;
            xmlControl["WorkflowID"] = (object) workflowID;
            if (AjaxScriptManager.Current != null)
                AjaxScriptManager.Current.ShowPopup("CD7F9A6F798D14B08B16018AF250E4EFA", "below",
                    (System.Web.UI.Control) xmlControl);
            else
                Sitecore.Context.ClientPage.ClientResponse.ShowPopup("CD7F9A6F798D14B08B16018AF250E4EFA", "below",
                    (System.Web.UI.Control) xmlControl);
        }

        private List<Item> GetRenderingDataSourceItems(Item item)
        {
            var items = new List<Item>();
            var renderings = item.Visualization.GetRenderings(Sitecore.Context.Device, true);
            
            foreach (var rendering in renderings)
            {
                //This check ensures only items are added, not queries
                if (Sitecore.Data.ID.IsID(rendering.Settings.DataSource))
                {
                    var dsItem = item.Database.SelectSingleItem(rendering.Settings.DataSource);
                    if (dsItem != null)
                    {
                        //Add the datasource item
                        items.Add(dsItem);
                    }
                }
            }
            return items;
        }
    }
}
