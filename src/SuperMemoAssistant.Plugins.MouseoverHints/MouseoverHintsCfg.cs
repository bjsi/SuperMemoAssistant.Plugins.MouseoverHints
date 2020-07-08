using Forge.Forms.Annotations;
using Newtonsoft.Json;
using SuperMemoAssistant.Services.UI.Configuration;
using SuperMemoAssistant.Sys.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverHints
{

  [Form(Mode = DefaultFields.None)]
  [Title("Dictionary Settings",
         IsVisible = "{Env DialogHostContext}")]
  [DialogAction("cancel",
        "Cancel",
        IsCancel = true)]
  [DialogAction("save",
        "Save",
        IsDefault = true,
        Validates = true)]
  public class MouseoverHintsCfg : CfgBase<MouseoverHintsCfg>, INotifyPropertyChangedEx
  {

    //[Field(Name = "Single Hint CSS")]
    //[MultiLine]
    //public string SingleHintCSS { get; set; }

    //[Field(Name = "Group Hint CSS")]
    //[MultiLine]
    //public string GroupHintCSS { get; set; }

    [JsonIgnore]
    public bool IsChanged { get; set; }

    public override string ToString()
    {
      return "MouseoverHints";
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
