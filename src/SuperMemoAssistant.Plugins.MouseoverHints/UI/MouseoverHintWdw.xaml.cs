using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperMemoAssistant.Plugins.MouseoverHints.UI
{
  /// <summary>
  /// Interaction logic for MouseoverHintWdw.xaml
  /// </summary>
  public partial class MouseoverHintWdw : Window
  {
    public MouseoverHintWdw()
    {
      InitializeComponent();
    }

    private void RemoveHintBtn_Click(object sender, RoutedEventArgs e)
    {

      var htmlCtrl = ContentUtils.GetFocusedHtmlControl();
      if (htmlCtrl.IsNull() || htmlCtrl.Text.IsNullOrEmpty())
        return;

      var doc = new HtmlDocument();
      doc.LoadHtml(htmlCtrl.Text);

      // TODO:
      doc.DocumentNode.SelectNodes("//span[@class='']");

    }

    private void DeleteHintBtn_Click(object sender, RoutedEventArgs e)
    {

    }
  }
}
