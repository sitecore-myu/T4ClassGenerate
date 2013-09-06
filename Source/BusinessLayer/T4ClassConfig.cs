using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.T4Class.Generate.Module.BusinessLayer
{
  public class T4ClassConfig : CustomItemBase
  {
    public T4ClassConfig(Item innerItem) : base(innerItem)
    {
    }

    public string DefaultProjectPath
    {
      get { return base.InnerItem["{2B4B2244-94F0-4534-8E80-184D984EE853}"]; }
      set { base.InnerItem["{2B4B2244-94F0-4534-8E80-184D984EE853}"] = value; }
    }

    public string DefaultFileName
    {
      get { return base.InnerItem["{334B59E2-FFFD-4BB2-BBFC-A54BD9EA2CC0}"]; }
      set { base.InnerItem["{334B59E2-FFFD-4BB2-BBFC-A54BD9EA2CC0}"] = value; }
    }

    public string DefaultNamespace
    {
      get { return base.InnerItem["{CCA71E4B-3EA2-4E2B-8DE0-65F223E46DA1}"]; }
      set { base.InnerItem["{CCA71E4B-3EA2-4E2B-8DE0-65F223E46DA1}"] = value; }
    }

    public string UsingStatement
    {
      get { return base.InnerItem["{BB091E72-36FC-4170-9728-A7FB013A5A1B}"]; }
      set { base.InnerItem["{BB091E72-36FC-4170-9728-A7FB013A5A1B}"] = value; }
    }

    public string ClassType
    {
      get { return base.InnerItem["{6187D76A-39FF-470C-8999-46FA0B5B2F07}"]; }
      set { base.InnerItem["{6187D76A-39FF-470C-8999-46FA0B5B2F07}"] = value; }
    }

  }
}