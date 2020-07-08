#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   7/7/2020 6:21:50 PM
// Modified By:  james

#endregion




namespace SuperMemoAssistant.Plugins.MouseoverHints
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Windows.Input;
  using Anotar.Serilog;
  using mshtml;
  using SuperMemoAssistant.Plugins.MouseoverHints.Interop;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.Keyboard;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Sys.IO.Devices;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class MouseoverHintsPlugin : SentrySMAPluginBase<MouseoverHintsPlugin>
  {
    #region Constructors

    /// <inheritdoc />
    public MouseoverHintsPlugin() : base("Enter your Sentry.io api key (strongly recommended)") { }

    #endregion


    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "MouseoverHints";

    /// <inheritdoc />
    public override bool HasSettings => false;
    public MouseoverHintsCfg Config;
    private MouseoverHintSvc mouseoverHintSvc = new MouseoverHintSvc();
    private HTMLControlEvents HtmlDocEvents { get; set; }

    //
    // Html tags

    // Single
    private const string SingleHintClass = "mouseover-hint-single";
    private readonly string SingleHintTagOpen = $"<span class='{SingleHintClass}' style='background-color: IndianRed; color: IndianRed'>";
    private const string SingleHintTagClose = "</span>";

    // Group
    private const string GroupHintClass = "mouseover-hint-group";
    private readonly string GroupHintTagOpen = $"<span class='{GroupHintClass}' style='background-color: IndianRed; color: IndianRed'>";
    private const string GroupHintTagClose = "</span>";

    #endregion

    #region Methods Impl

    private void LoadConfig()
    {
      Config = Svc.Configuration.Load<MouseoverHintsCfg>() ?? new MouseoverHintsCfg();
    }

    /// <inheritdoc />
    protected override void PluginInit()
    {

      LoadConfig();

      Svc.HotKeyManager
      .RegisterGlobal(
        "CreateSingleHint",
        "Create a single mouse over hint on the selected text",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.H, KeyModifiers.CtrlAlt),
        SingleHint
      )

      .RegisterGlobal(
        "CreateHintGroup",
        "Create a hint that is part of a hint group",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.H, KeyModifiers.CtrlAltShift),
        GroupHint
      )

      .RegisterGlobal(
        "HideContext",
        "Wrap group hints around the selected text",
        HotKeyScopes.SMBrowser,
        new HotKey(Key.H, KeyModifiers.CtrlShift),
        HideContext1
      );

      SubscribeToHtmlEvents();

      PublishService<IMouseoverHintSvc, MouseoverHintSvc>(mouseoverHintSvc);

    }

    private void HideContext1()
    {
      var selObj = ContentUtils.GetSelectionObject();
      HideContext(selObj);
    }

    private bool IsMouseoverHintElement(IHTMLElement2 x)
    {
      var element = x as IHTMLElement;
      if (element.IsNull())
        return false;

      if (element.tagName.ToLower() != "span")
        return false;

      if (element.className.IsNullOrEmpty())
        return false;

      if (!element.className.Contains("mouseover-hint"))
        return false;

      return true;
    }

    private void SubscribeToHtmlEvents()
    {

      var events = new List<EventInitOptions>
      {
        new EventInitOptions(EventType.onmouseenter, _ => true, IsMouseoverHintElement),
        new EventInitOptions(EventType.onmouseleave, _ => true, IsMouseoverHintElement)
      };

      HtmlDocEvents = new HTMLControlEvents(events);

      HtmlDocEvents.OnMouseEnterEvent += HtmlDocEvents_OnMouseEnterEvent;
      HtmlDocEvents.OnMouseLeaveEvent += HtmlDocEvents_OnMouseLeaveEvent;

    }

    private void HtmlDocEvents_OnMouseLeaveEvent(object sender, IHTMLControlEventArgs e)
    {
      var element = e?.EventObj?.srcElement;
      if (element.IsNull())
        return;

      if (element.className == GroupHintClass)
        HideGroupHint();
      else
        HideSingleHint(element);
    }

    private void HideSingleHint(IHTMLElement element)
    {

      if (element.IsNull())
        return;

      element.style.backgroundColor = "IndianRed";
      element.style.color = "IndianRed";

    }

    private void ShowSingleHint(IHTMLElement element)
    {

      if (element.IsNull())
        return;

      element.style.backgroundColor = "white";
      element.style.color = "black";

    }

    private void HideGroupHint()
    {
      var document = ContentUtils.GetFocusedHtmlDocument();
      if (document.IsNull())
        return;

      var elements = document.all
        ?.Cast<IHTMLElement>()
        ?.Where(x => IsMouseoverHintElement((IHTMLElement2)x));

      if (elements.IsNull() || !elements.Any())
        return;

      foreach (var element in elements)
      {
        HideSingleHint(element);
      }
    }

    private void ShowGroupHint()
    {
      var document = ContentUtils.GetFocusedHtmlDocument();
      if (document.IsNull())
        return;

      var elements = document.all
        ?.Cast<IHTMLElement>()
        ?.Where(x => IsMouseoverHintElement((IHTMLElement2)x));

      if (elements.IsNull() || !elements.Any())
        return;

      foreach (var element in elements)
      {
        ShowSingleHint(element);
      }
    }

    private void HtmlDocEvents_OnMouseEnterEvent(object sender, IHTMLControlEventArgs e)
    {
  
      var element = e?.EventObj?.srcElement;
      if (element.IsNull())
        return;

      if (element.className == GroupHintClass)
        ShowGroupHint();
      else
        ShowSingleHint(element);

    }

    private void GroupHint()
    {
      var selObj = ContentUtils.GetSelectionObject();
      CreateGroupHint(selObj);
    }

    private void SingleHint()
    {
      var selObj = ContentUtils.GetSelectionObject();
      CreateSingleHint(selObj);
    }

    public bool CreateSingleHint(IHTMLTxtRange selObj)
    {

      var html = selObj?.htmlText;
      if (selObj.IsNull() || html.IsNullOrEmpty())
        return false;

      string hinted = SingleHintTagOpen + html + SingleHintTagClose;
      selObj.pasteHTML(hinted);
      LogTo.Debug("Successfully created single hint");
      return true;

    }

    public bool CreateGroupHint(IHTMLTxtRange selObj)
    {

      var html = selObj?.htmlText;
      if (selObj.IsNull() || html.IsNullOrEmpty())
        return false;

      string hinted = GroupHintTagOpen + html + GroupHintTagClose;
      selObj.pasteHTML(hinted);
      LogTo.Debug("Successfully created group hint");
      return true;

    }

    public bool HideContext(IHTMLTxtRange selObj)
    {

      if (selObj.IsNull())
        return false;

      int MaxTextLength = 2000000000;

      // Wrap the section before the visible html
      var pre = selObj.duplicate();
      pre.collapse();
      pre.moveStart("character", -MaxTextLength);
      string preHtml = pre.htmlText;
      pre.pasteHTML(GroupHintTagOpen + preHtml + GroupHintTagClose);

      // Wrap the section after the visible html
      var post = selObj.duplicate();
      post.collapse(false);
      post.moveEnd("character", MaxTextLength);
      string postHtml = post.htmlText;
      post.pasteHTML(GroupHintTagOpen + postHtml + GroupHintTagClose);

      return true;

    }

    /// <inheritdoc />
    public override void ShowSettings()
    {
    }

    #endregion

    #region Methods

    #endregion
  }
}
