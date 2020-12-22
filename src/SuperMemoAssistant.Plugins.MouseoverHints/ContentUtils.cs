using mshtml;
using SuperMemoAssistant.Extensions;
using SuperMemoAssistant.Interop.SuperMemo.Content.Controls;
using SuperMemoAssistant.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMemoAssistant.Plugins.MouseoverHints
{
  public static class ContentUtils
  {
    /// <summary>
    /// Get the selection object representing the currently highlighted text in SM.
    /// </summary>
    /// <returns>IHTMLTxtRange object or null</returns>
    public static IHTMLTxtRange GetSelectionObject()
    {

      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      var htmlCtrl = ctrlGroup?.FocusedControl?.AsHtml();
      var htmlDoc = htmlCtrl?.GetDocument();
      var sel = htmlDoc?.selection;

      if (!(sel?.createRange() is IHTMLTxtRange textSel))
        return null;

      return textSel;

    }

    /// <summary>
    /// Get the IHTMLDocument2 object representing the focused html control of the current element.
    /// </summary>
    /// <returns>IHTMLDocument2 object or null</returns>
    public static IHTMLDocument2 GetFocusedHtmlDocument()
    {

      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      var htmlCtrl = ctrlGroup?.FocusedControl?.AsHtml();
      return htmlCtrl?.GetDocument();

    }

    public static IControlHtml GetFocusedHtmlControl()
    {

      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      return ctrlGroup?.FocusedControl?.AsHtml();

    }

    /// <summary>
    /// Get the IHTMLDocument2 object corresponding to the ControlIdx
    /// </summary>
    /// <param name="ControlIdx"></param>
    /// <returns>IHTMLDocument2 or null</returns>
    public static IHTMLDocument2 GetHtmlDocByIndex(int ControlIdx)
    {

      var ctrlGroup = Svc.SM.UI.ElementWdw.ControlGroup;
      if (ctrlGroup.Count() >= ControlIdx + 1)
        return null;

      return ctrlGroup[ControlIdx]
        ?.AsHtml()
        ?.GetDocument();

    }
  }
}