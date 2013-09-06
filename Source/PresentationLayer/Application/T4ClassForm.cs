using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.T4Class.Generate.Module.BusinessLayer;
using Sitecore.T4Class.Generate.Module.BusinessLayer.Extentions;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;
using Sitecore.Web.UI.XamlSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace Sitecore.T4Class.Generate.Module.PresentationLayer.Application
{
  public class T4ClassForm : ApplicationForm
  {
    private readonly T4ClassConfig _configT4Class;
    private readonly Database source;

    protected GridPanel GpMainLayout;
    protected GridPanel GpEmpty;
    protected GridPanel GpFields;
    protected Border BuildListBorder;
    protected Border RibbonPanel;
    protected Checklist ClFields;
    protected DataContext DataContext;
    protected DataTreeview Treeview;
    protected Literal LSelectedItemName;

    private TemplateItem CurrentTreeviewItem
    {
      get
      {
        Item currentItem = Treeview.GetSelectionItem();
        return currentItem.TemplateID == TemplateIDs.Template ? currentItem : null;
      }
    }

    private string T4TemplateClass
    {
      get
      {
        ChecklistItem[] items = ClFields.Items;
        var t4ClassTemplate = new List<TemplateFieldItem>();

        for (int i = 0; i < items.Length; i++)
        {
          ChecklistItem child = items[i];
          if (child.Checked)
          {
            TemplateFieldItem ownField = CurrentTreeviewItem.OwnFields.SingleOrDefault(t => t.ID == ID.Parse(child.ID));
            if (ownField != null)
              t4ClassTemplate.Add(ownField);
          }
        }
        return t4ClassTemplate.RunT4Template(_configT4Class, CurrentTreeviewItem);
      }
    }

    public T4ClassForm()
    {
      source = Context.ContentDatabase;
      if (!source.Name.Contains("core"))
        _configT4Class = ConfigManager.GetT4ClassConfig();
    }

    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);

      Load();
    }

    private void Load()
    {
      if (!Context.ClientPage.IsEvent)
      {
        DataContext.DataViewName = T4ClassConsts.Configurations.DefaultDataBase;
        DataContext.Root = source.Items[T4ClassConsts.Configurations.RootTreeview].ID.ToString();
      }

      if (!source.Name.Contains("core"))
      {
        if (!Context.ClientPage.IsEvent)
        {
          BuildListBorder.Visible = false;
          UpdateRibbon();

        }

        Treeview.OnClick += Treeview_OnClick;

        GpEmpty.Visible = false;
        GpMainLayout.Visible = true;
      }
      else
      {
        GpEmpty.Visible = true;
        GpMainLayout.Visible = false;
      }
    }

    private void UpdateRibbon()
    {
      var ctl = new Ribbon
      {
        ID = "Ribbon"
      };
      var context = new CommandContext(DataContext.GetFolder());
      ctl.CommandContext = context;
      ctl.ShowContextualTabs = false;
      Item item = Context.Database.GetItem(T4ClassConsts.Configurations.Ribbon);
      Assert.IsNotNull(item, T4ClassConsts.Configurations.Ribbon);
      context.RibbonSourceUri = item.Uri;
      context.Folder = DataContext.GetFolder();
      RibbonPanel.InnerHtml = HtmlUtil.RenderControl(ctl);
    }

    protected void OnBtnCheckAll()
    {
      ClFields.CheckAll();
    }

    protected void OnBtnUncheckAll()
    {
      ClFields.UncheckAll();
    }

    private void Treeview_OnClick(object sender, EventArgs e)
    {
      InitViewItem();
    }

    private void InitViewItem()
    {
      if (CurrentTreeviewItem != null)
      {
        LSelectedItemName.Text = DataContext.CurrentItem.Name;
        GpFields.Controls.Clear();
        ClFields.Controls.Clear();
        foreach (TemplateFieldItem field in CurrentTreeviewItem.OwnFields)
        {
          var checklistItem = new ChecklistItem
                                {
                                  Header =field.Name.FieldNameFormat(),
                                  ID = field.ID.ToString(),
                                  Checked = true
                                };
          checklistItem.Margin = "10px; 15px; 15px; 10px;";
          ClFields.Controls.Add(checklistItem);
          ClFields.Margin = "5px; 5px; 5px; 5px;";
          BuildListBorder.Visible = true;
        }
        GpFields.Controls.Add(ClFields);
        SheerResponse.Refresh(BuildListBorder);
      }
    }

    public override void HandleMessage(Message message)
    {
      Assert.ArgumentNotNull(message, "message");
      base.HandleMessage(message);

      if (message.Name.Contains("sct4class:preview"))
      {
        if (CurrentTreeviewItem != null)
        {
          var dialogUrl =
            new UrlString(
              ControlManager.GetControlUrl(
                new ControlName("Sitecore.T4Class.Generate.Module.PresentationLayer.Application.T4ClassPreview")));
          var handle = new UrlHandle();

          handle["class"] = T4TemplateClass;
          handle.Add(dialogUrl);
          SheerResponse.ShowModalDialog(dialogUrl.ToString(), "800");
        }
        else
        {
          SheerResponse.Alert("Please, select item!");
        }
      }
      if (message.Name.Contains("sct4class:config"))
      {
        var parameters = new NameValueCollection();
        Context.ClientPage.Start(this, "ConfigMethod", parameters);
      }
      if (message.Name.Contains("sct4class:generate"))
      {
        if (CurrentTreeviewItem != null)
        {
          string fileName = string.Format("{0}.cs", string.IsNullOrEmpty(_configT4Class.DefaultFileName)
                                                      ? CurrentTreeviewItem.Name
                                                      : _configT4Class.DefaultFileName);

          string projectPath = WebUtil.GetQueryString("SolutionPath");
          if (string.IsNullOrEmpty(projectPath))
            projectPath = AppDomain.CurrentDomain.BaseDirectory;

          File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + fileName, T4TemplateClass);
          SheerResponse.Alert(string.Format("Class '{0}' has been generated! \n\nFile are located in '{1}{0}'", fileName,
                                            projectPath));
        }
        else
        {
          SheerResponse.Alert("Please, select item!");
        }
      }
    }

    protected void ConfigMethod(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (args.IsPostBack)
      {
        if (args.HasResult)
        {
          var results = new UrlString(args.Result);
          Assert.IsNotNull(_configT4Class.InnerItem,
                           string.Format("'{0}' not found", T4ClassConsts.Configurations.ScT4ClassConfig));
          using (new EditContext(_configT4Class.InnerItem))
          {
            _configT4Class.UsingStatement = HttpUtility.UrlDecode(results["UsingStatement"]);
            _configT4Class.DefaultNamespace = HttpUtility.UrlDecode(results["DefaultNamespace"]);
            _configT4Class.DefaultFileName = HttpUtility.UrlDecode(results["DefaultFileName"]);
            _configT4Class.DefaultProjectPath = HttpUtility.UrlDecode(results["DefaultProjectPath"]);
            _configT4Class.ClassType = HttpUtility.UrlDecode(results["ClassType"]);
          }
        }
      }
      else
      {
        var dialogUrl =
          new UrlString(
            ControlManager.GetControlUrl(
              new ControlName("Sitecore.T4Class.Generate.Module.PresentationLayer.Application.Configuration")));

        Assert.IsNotNull(_configT4Class.InnerItem,
                         string.Format("'{0}' not found", T4ClassConsts.Configurations.ScT4ClassConfig));
        dialogUrl.Add("DefaultProjectPath", _configT4Class.DefaultProjectPath);
        dialogUrl.Add("DefaultFileName", _configT4Class.DefaultFileName);
        dialogUrl.Add("DefaultNamespace", _configT4Class.DefaultNamespace);
        dialogUrl.Add("UsingStatement", _configT4Class.UsingStatement);
        dialogUrl.Add("ClassType", _configT4Class.ClassType);
        if (string.IsNullOrEmpty(_configT4Class.DefaultFileName))
          dialogUrl.Add("CurrentTreeviewItemName", CurrentTreeviewItem.Name);
        SheerResponse.ShowModalDialog(dialogUrl.ToString(), true);
        args.WaitForPostBack();
      }
    }
  }
}