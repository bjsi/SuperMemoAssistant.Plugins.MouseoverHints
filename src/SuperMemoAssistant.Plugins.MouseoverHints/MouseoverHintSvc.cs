using mshtml;
using PluginManager.Interop.Sys;
using SuperMemoAssistant.Plugins.MouseoverHints.Interop;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverHints
{
  public class MouseoverHintSvc : PerpetualMarshalByRefObject, IMouseoverHintSvc
  {
    public bool CreateSingleHint(IHTMLTxtRange selObj)
    {
      return Svc<MouseoverHintsPlugin>.Plugin.CreateSingleHint(selObj);
    }

    public bool CreateGroupHint(IHTMLTxtRange selObj)
    {
      return Svc<MouseoverHintsPlugin>.Plugin.CreateGroupHint(selObj);
    }

    public bool HideContext(IHTMLTxtRange selObj)
    {
      return Svc<MouseoverHintsPlugin>.Plugin.HideContext(selObj);
    }
  }
}
