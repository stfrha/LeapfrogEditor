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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeapfrogEditor
{
   /// <summary>
   /// Interaction logic for TrianglesControl.xaml
   /// </summary>
   public partial class TrianglesControl : UserControl
   {
      public TrianglesControl()
      {
         InitializeComponent();
      }

      private void Shape_MouseDown(object sender, MouseButtonEventArgs e)
      {
         CompoundObjectUserControl parentControl = ParentalFinder.FindParent<CompoundObjectUserControl>(this);
         parentControl.GeneralMouse<LfShapeViewModel>(true, MouseEventObjectType.shape, sender, e);
      }

      private void Shape_MouseUp(object sender, MouseButtonEventArgs e)
      {
         CompoundObjectUserControl parentControl = ParentalFinder.FindParent<CompoundObjectUserControl>(this);
         parentControl.GeneralMouse<LfShapeViewModel>(false, MouseEventObjectType.shape, sender, e);
      }

      private void ShapeBorder_MouseDown(object sender, MouseButtonEventArgs e)
      {
         CompoundObjectUserControl parentControl = ParentalFinder.FindParent<CompoundObjectUserControl>(this);
         parentControl.GeneralMouse<LfPointViewModel>(true, MouseEventObjectType.dragableBorder, sender, e);
      }

      private void ShapeBorder_MouseUp(object sender, MouseButtonEventArgs e)
      {
         CompoundObjectUserControl parentControl = ParentalFinder.FindParent<CompoundObjectUserControl>(this);
         parentControl.GeneralMouse<LfPointViewModel>(false, MouseEventObjectType.dragableBorder, sender, e);
      }

      private void ShapeBorder_MouseMove(object sender, MouseEventArgs e)
      {
         CompoundObjectUserControl parentControl = ParentalFinder.FindParent<CompoundObjectUserControl>(this);
         parentControl.GeneralMouseMove<IPositionInterface>(MouseEventObjectType.dragableBorder, sender, e);
      }

      private void LineCursorKeyDown(object sender, KeyEventArgs e)
      {
         if (sender is FrameworkElement)
         {
            FrameworkElement el = (FrameworkElement)sender;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
               el.Cursor = Cursors.Pen;

            e.Handled = true;
         }
      }

      private void LineCursorKeyUp(object sender, KeyEventArgs e)
      {
         if (sender is FrameworkElement)
         {
            FrameworkElement el = (FrameworkElement)sender;

            el.Cursor = null;

            e.Handled = true;
         }
      }

   }
}
