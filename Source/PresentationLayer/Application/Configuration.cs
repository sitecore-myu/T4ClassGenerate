using Sitecore.Controls;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.T4Class.Generate.Module.BusinessLayer;
using Sitecore.Text;
using Sitecore.Web;
using System;
using System.Web.UI.WebControls;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace Sitecore.T4Class.Generate.Module.PresentationLayer.Application
{
  public class Configuration : DialogPage
  {
    protected TextBox tbDefaultFileName;
    protected TextBox tbDefaultNamespace;
    protected TextBox tbProjectRootPath;
    protected TextBox tbUsingStatements;
    protected DropDownList lstbxClassType;

    protected override void OK_Click()
    {
      var result = new UrlString();
      result.Add("DefaultProjectPath", tbProjectRootPath.Text);
      result.Add("DefaultFileName", tbDefaultFileName.Text);
      result.Add("DefaultNamespace", tbDefaultNamespace.Text);
      result.Add("UsingStatement", tbUsingStatements.Text);
      result.Add("ClassType", lstbxClassType.SelectedValue);
      AjaxScriptManager.SetDialogValue(result.ToString());
      base.OK_Click();
    }

    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
      if (!AjaxScriptManager.IsEvent)
      {
        // Init Using Statement
        tbUsingStatements.Text = WebUtil.GetQueryString("UsingStatement");

        // Init Class Type ListBox
        var masterDb = Sitecore.Configuration.Factory.GetDatabase(T4ClassConsts.Configurations.DefaultDataBase.ToLower());
        var classTypesItems = masterDb.Items[T4ClassConsts.IDs.ClassTypes].Children;
        foreach (Item classTypesItem in classTypesItems)
        {
          var typeNameField = classTypesItem.Fields[T4ClassConsts.IDs.TypeNameField];
          if (typeNameField != null)
          {
            lstbxClassType.Items.Add(new ListItem { Text = typeNameField.Value, Value = classTypesItem.ID.ToString() });
          }
        }
        lstbxClassType.SelectedValue = WebUtil.GetQueryString("ClassType");

        // Init SolutionPath
        var projectPath = WebUtil.GetQueryString("SolutionPath");
        if (!string.IsNullOrEmpty(projectPath))
          tbProjectRootPath.Text = projectPath;
        else
          tbProjectRootPath.Text = AppDomain.CurrentDomain.BaseDirectory;

        // Init FileName
        var fileName = WebUtil.GetQueryString("DefaultFileName");
        if (!string.IsNullOrEmpty(fileName))
          tbDefaultFileName.Text = fileName;
        else
          tbDefaultFileName.Text = WebUtil.GetQueryString("CurrentTreeviewItemName");

        // Init Namespace
        var defaultNamespace = WebUtil.GetQueryString("DefaultNamespace");
        if (!string.IsNullOrEmpty(defaultNamespace))
          tbDefaultNamespace.Text = defaultNamespace;
        else
          tbDefaultNamespace.Text = T4ClassConsts.Texts.DefaultNamespaceText;
      }
    }
  }
}