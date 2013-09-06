using Sitecore.T4Class.Generate.Module.BusinessLayer.CSharpFormat;

namespace Sitecore.T4Class.Generate.Module.PresentationLayer.Application
{
  using Sitecore.Controls;
  using Sitecore.Diagnostics;
  using Sitecore.Web;
  using Sitecore.Web.UI.HtmlControls;
  using System;

  public class PreviewClass : DialogPage
  {
    protected Border OutputDiv;

    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
        UrlHandle handle = UrlHandle.Get();      
        base.Header=handle["templatename"];
        var cSharpFormat = new CSharpFormat();
        string t4TemplateClassFormated = cSharpFormat.FormatCode(handle["class"]);

        OutputDiv.InnerHtml = t4TemplateClassFormated;
    }
  }
}