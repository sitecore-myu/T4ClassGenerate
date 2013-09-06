using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TextTemplating;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.T4Class.Generate.Module.PresentationLayer.T4Template;

namespace Sitecore.T4Class.Generate.Module.BusinessLayer.Extentions
{
  public static class T4ClassExtentions
  {
    public static Type GetFieldType(this TemplateFieldItem field)
    {
      FieldType fieldType = FieldTypeManager.GetFieldType(field.Type);
      Type result;
      if (field.Type == "Number")
      {
        result = typeof (double);
      }
      else
      {
        if (field.Type == "Integer")
        {
          result = typeof (int);
        }
        else
        {
          if (fieldType != null)
          {
            string fullName = fieldType.Type.FullName;
            switch (fullName)
            {
              case "Sitecore.Data.Fields.TextField":
              case "Sitecore.Data.Fields.HtmlField":
              case "Sitecore.Data.Fields.ValueLookupField":
                result = typeof (string);
                return result;
              case "Sitecore.Data.Fields.DateField":
                result = typeof (DateTime);
                return result;
              case "Sitecore.Data.Fields.LookupField":
              case "Sitecore.Data.Fields.ReferenceField":
              case "Sitecore.Data.Fields.GroupedDroplinkField":
              case "Sitecore.Data.Fields.GroupedDroplistField":
                result = typeof (Item);
                return result;
              case "Sitecore.Data.Fields.MultilistField":
                result = typeof (IEnumerable<Item>);
                return result;
              case "Sitecore.Data.Fields.CheckboxField":
                result = typeof (bool);
                return result;
            }
            result = fieldType.Type;
          }
          else
          {
            result = typeof (string);
          }
        }
      }
      return result;
    }

    public static string GetClassTypeName(this string itemId)
    {
      if (itemId.Equals(T4ClassConsts.IDs.GlassClassType))
        return "GlassClassType";
      if (itemId.Equals(T4ClassConsts.IDs.DefaultClassType))
        return "DefaultClassType";
      return string.Empty;
    }

    public static string TypeFormat(this string text)
    {
      var typeName = text.
        Replace("String", "string").
        Replace("Int32", "int").
        Replace("Double", "double").
        Replace("Boolean", "bool").
        Replace("IEnumerable`1", "List");
      if (typeName.Contains("List"))
      {
        typeName = string.Format("{0}<Item>", typeName);
      }
      return typeName;
    }

    public static string FieldNameFormat(this string field)
    {
      return field.Replace(" ", string.Empty);
    }

    public static string RunT4Template(this List<TemplateFieldItem> templateFields, T4ClassConfig configT4Class,
                                       TemplateItem currentTreeviewItem)
    {
      var template = new T4TemplateClass();
      var session = new TextTemplatingSession();
      session["namespaceName"] = !configT4Class.DefaultNamespace.Any()
                                   ? T4ClassConsts.Texts.DefaultNamespaceText
                                   : configT4Class.DefaultNamespace;
      session["usingNamespaces"] = configT4Class.UsingStatement;
      session["classType"] = configT4Class.ClassType.GetClassTypeName();
      session["classFields"] = templateFields;
      session["className"] = currentTreeviewItem.Name.Replace(" ", string.Empty);
      template.Session = session;
      template.Initialize();
      string textTemplateOutput = template.TransformText();
      return textTemplateOutput.Remove(0, 15);
    }
  }
}