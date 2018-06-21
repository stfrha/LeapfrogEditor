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
   /// Interaction logic for CompoundObjectUserControl.xaml
   /// </summary>
   public partial class CompoundObjectUserControl : UserControl
   {
      /// <summary>
      /// Specifies the current state of the mouse handling logic.
      /// </summary>
      private ZoomAndPan.MouseHandlingMode mouseHandlingMode = ZoomAndPan.MouseHandlingMode.None;

      /// <summary>
      /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
      /// </summary>
      private Point origContentMouseDownPoint;

      /// <summary>
      /// Records which mouse button clicked during mouse dragging.
      /// </summary>
      private MouseButton mouseButtonDown;



      public CompoundObjectUserControl()
      {
         InitializeComponent();
      }


      private void General_MouseDown(object sender, MouseButtonEventArgs e)
      {
         UIElement parentCanvas = FindParent<Canvas>(content);
         parentCanvas.Focus();
         Keyboard.Focus(parentCanvas);

         mouseButtonDown = e.ChangedButton;

         if (mouseButtonDown == MouseButton.Left)
         {
            mouseHandlingMode = ZoomAndPan.MouseHandlingMode.DraggingObjects;
            origContentMouseDownPoint = e.GetPosition(parentCanvas);

            UIElement dragObject = (UIElement)sender;
            dragObject.CaptureMouse();

            e.Handled = true;
         }
         
         bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
         bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
         bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

         if (sender is FrameworkElement)
         {
            FrameworkElement fwe = (FrameworkElement)sender;

            if (fwe.DataContext is IPositionInterface)
            {
               IPositionInterface obj = (IPositionInterface)fwe.DataContext;
               if (obj.MainVm.MouseDown(fwe, e.ChangedButton, e.GetPosition(content), e.ClickCount, shift, ctrl, alt))
               {
                  e.Handled = true;
               }

            }
         }
      }

      private void General_MouseUp(object sender, MouseButtonEventArgs e)
      {
         UIElement parentCanvas = FindParent<Canvas>(content);
         parentCanvas.Focus();
         Keyboard.Focus(parentCanvas);

         if (mouseHandlingMode == ZoomAndPan.MouseHandlingMode.DraggingObjects)
         {
            mouseButtonDown = e.ChangedButton;

            if (mouseButtonDown == MouseButton.Left)
            {
               mouseHandlingMode = ZoomAndPan.MouseHandlingMode.None;

               UIElement rectangle = (UIElement)sender;
               rectangle.ReleaseMouseCapture();

               e.Handled = true;
            }
         }

         bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
         bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
         bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

         if (sender is FrameworkElement)
         {
            FrameworkElement fwe = (FrameworkElement)sender;

            if (fwe.DataContext is IPositionInterface)
            {
               IPositionInterface obj = (IPositionInterface)fwe.DataContext;

               if (obj.MainVm.MouseDown(fwe, e.ChangedButton, e.GetPosition(content), e.ClickCount, shift, ctrl, alt))
               {
                  e.Handled = true;
               }
            }
         }
      }

      private void General_MouseMove(object sender, MouseEventArgs e)
      {
         if (mouseHandlingMode == ZoomAndPan.MouseHandlingMode.DraggingObjects)
         {
            if (mouseButtonDown == MouseButton.Left)
            {
               UIElement parentCanvas = FindParent<Canvas>(content);

               Point curContentPoint = e.GetPosition(parentCanvas);
               Vector rectangleDragVector = curContentPoint - origContentMouseDownPoint;

               origContentMouseDownPoint = curContentPoint;

               bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
               bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
               bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

               if (sender is FrameworkElement)
               {
                  FrameworkElement fwe = (FrameworkElement)sender;

                  if (fwe.DataContext is IPositionInterface)
                  {
                     IPositionInterface obj = (IPositionInterface)fwe.DataContext;

                     obj.MainVm.MouseMove(fwe, rectangleDragVector, shift, ctrl, alt);
                  }
               }

               e.Handled = true;

            }
         }
      }

      //private void Shape_MouseUp(object sender, MouseButtonEventArgs e)
      //{
      //   Keyboard.Focus(content);

      //   if (sender is FrameworkElement)
      //   {
      //      FrameworkElement fwe = (FrameworkElement)sender;

      //      if (fwe.DataContext is IShapeInterface)
      //      {
      //         // Code in here should probably be handled by some Shape base class

      //         IShapeInterface obj = (IShapeInterface)fwe.DataContext;

      //         if (obj.Parent.IsSelected)
      //         {
      //            // Check if ctrl is pressed, in which case 
      //            // we leave old selection as is
      //            // If not, all other shapes that were selected
      //            // becomes unselected
      //            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
      //            {
      //               obj.Parent.DeselectAllChildren();
      //            }

      //            obj.IsSelected = true;
      //         }
      //         else
      //         {
      //            obj.Parent.IsSelected = true;
      //         }
      //      }

      //   }
      //   e.Handled = true;
      //}

      //private void PolyLine_MouseUp(object sender, MouseButtonEventArgs e)
      //{
      //   Keyboard.Focus(content);

      //   if (sender is FrameworkElement)
      //   {
      //      FrameworkElement fwe = (FrameworkElement)sender;

      //      if (fwe.DataContext is DragablePointViewModel)
      //      {
      //         // Code in here should probably be handled by DragablePointViewModel class

      //         if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
      //         {
      //            DragablePointViewModel obj = (DragablePointViewModel)fwe.DataContext;
      //            DragablePointViewModel newPoint = obj.Parent.InsertPoint(e.GetPosition(content), obj);
      //            newPoint.Parent.DeselectAllPoints();
      //            newPoint.IsSelected = true;
      //         }
      //      }

      //   }
      //   e.Handled = true;
      //}

      //private void PolyPoint_MouseUp(object sender, MouseButtonEventArgs e)
      //{
      //   Keyboard.Focus(content);

      //   if (sender is FrameworkElement)
      //   {
      //      FrameworkElement fwe = (FrameworkElement)sender;

      //      if (fwe.DataContext is DragablePointViewModel)
      //      {
      //         // Code in here should probably be handled by DragablePointViewModel class
      //         DragablePointViewModel obj = (DragablePointViewModel)fwe.DataContext;

      //         // Check if ctrl is pressed, in which case 
      //         // we leave old selection as is
      //         // If not, all other shapes that were selected
      //         // becomes unselected
      //         if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
      //         {
      //            obj.Parent.DeselectAllPoints();
      //         }

      //         obj.IsSelected = true;
      //      }

      //   }
      //   e.Handled = true;
      //}


      //private void DragableRect_MouseDown(object sender, MouseButtonEventArgs e)
      //{
      //   content.Focus();
      //   Keyboard.Focus(content);

      //   if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
      //   {
      //      //
      //      // When the shift key is held down special zooming logic is executed in content_MouseDown,
      //      // so don't handle mouse input here.
      //      //
      //      return;
      //   }

      //   if (mouseHandlingMode != ZoomAndPan.MouseHandlingMode.None)
      //   {
      //      //
      //      // We are in some other mouse handling mode, don't do anything.
      //      return;
      //   }

      //   mouseButtonDown = e.ChangedButton;

      //   if (mouseButtonDown == MouseButton.Left)
      //   {
      //      mouseHandlingMode = ZoomAndPan.MouseHandlingMode.DraggingRectangles;
      //      origContentMouseDownPoint = e.GetPosition(content);

      //      UIElement rectangle = (UIElement)sender;
      //      rectangle.CaptureMouse();

      //      if (sender is FrameworkElement)
      //      {
      //         FrameworkElement fwe = (FrameworkElement)sender;

      //         if (fwe.DataContext is IPositionInterface)
      //         {
      //               // Code in here should probably be handled by DragablePointViewModel class
      //               IPositionInterface obj = (IPositionInterface)fwe.DataContext;

      //               obj.M
      //            // Check if ctrl is pressed, in which case 
      //            // we leave old selection as is
      //            // If not, all other shapes that were selected
      //            // becomes unselected
      //            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
      //            {
      //               obj.Parent.DeselectAllPoints();
      //            }

      //            obj.IsSelected = true;
      //         }
      //      }

      //      e.Handled = true;
      //   }
      //}

      //private void DragableRect_MouseUp(object sender, MouseButtonEventArgs e)
      //{
      //   if (mouseHandlingMode != ZoomAndPan.MouseHandlingMode.DraggingRectangles)
      //   {
      //      //
      //      // We are not in rectangle dragging mode.
      //      //
      //      return;
      //   }

      //   mouseButtonDown = e.ChangedButton;

      //   if (mouseButtonDown == MouseButton.Left)
      //   {
      //      mouseHandlingMode = ZoomAndPan.MouseHandlingMode.None;

      //      UIElement rectangle = (UIElement)sender;
      //      rectangle.ReleaseMouseCapture();

      //      e.Handled = true;
      //   }
      //}

      //private void DragableRect_MouseMove(object sender, MouseEventArgs e)
      //{
      //   if (mouseHandlingMode != ZoomAndPan.MouseHandlingMode.DraggingRectangles)
      //   {
      //      //
      //      // We are not in rectangle dragging mode, so don't do anything.
      //      //
      //      return;
      //   }

      //   if (mouseButtonDown == MouseButton.Left)
      //   {
      //      Point curContentPoint = e.GetPosition(content);
      //      Vector rectangleDragVector = curContentPoint - origContentMouseDownPoint;

      //      //
      //      // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
      //      //

      //      origContentMouseDownPoint = curContentPoint;

      //      if (sender is FrameworkElement)
      //      {
      //         FrameworkElement fwe = (FrameworkElement)sender;

      //         if (fwe.DataContext is IPositionInterface)
      //         {
      //            IPositionInterface obj = (IPositionInterface)fwe.DataContext;

      //            obj.PosX += rectangleDragVector.X;
      //            obj.PosY += rectangleDragVector.Y;
      //         }

      //      }



      //      //UIElement rectangle = (UIElement)sender;
      //      //Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) + rectangleDragVector.X);
      //      //Canvas.SetTop(rectangle, Canvas.GetTop(rectangle) + rectangleDragVector.Y);

      //      e.Handled = true;
      //   }
      //}

      //private void Co_MouseDown(object sender, MouseButtonEventArgs e)
      //{
      //   UIElement parentCanvas = FindParent<Canvas>(content);
      //   parentCanvas.Focus();
      //   Keyboard.Focus(parentCanvas);

      //   if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
      //   {
      //      //
      //      // When the shift key is held down special zooming logic is executed in content_MouseDown,
      //      // so don't handle mouse input here.
      //      //
      //      return;
      //   }

      //   if (mouseHandlingMode != ZoomAndPan.MouseHandlingMode.None)
      //   {
      //      //
      //      // We are in some other mouse handling mode, don't do anything.
      //      return;
      //   }

      //   mouseButtonDown = e.ChangedButton;

      //   if (mouseButtonDown == MouseButton.Left)
      //   {
      //      mouseHandlingMode = ZoomAndPan.MouseHandlingMode.DraggingRectangles;
      //      origContentMouseDownPoint = e.GetPosition(parentCanvas);

      //      UIElement rectangle = (UIElement)sender;
      //      rectangle.CaptureMouse();

      //      e.Handled = true;
      //   }
      //}

      //private void Co_MouseUp(object sender, MouseButtonEventArgs e)
      //{
      //   if (mouseHandlingMode != ZoomAndPan.MouseHandlingMode.DraggingRectangles)
      //   {
      //      //
      //      // We are not in rectangle dragging mode.
      //      //
      //      return;
      //   }

      //   mouseButtonDown = e.ChangedButton;

      //   if (mouseButtonDown == MouseButton.Left)
      //   {
      //      mouseHandlingMode = ZoomAndPan.MouseHandlingMode.None;

      //      UIElement rectangle = (UIElement)sender;
      //      rectangle.ReleaseMouseCapture();

      //      e.Handled = true;
      //   }
      //}

      //private void Co_MouseMove(object sender, MouseEventArgs e)
      //{
      //   if (mouseHandlingMode != ZoomAndPan.MouseHandlingMode.DraggingRectangles)
      //   {
      //      //
      //      // We are not in rectangle dragging mode, so don't do anything.
      //      //
      //      return;
      //   }

      //   if (mouseButtonDown == MouseButton.Left)
      //   {
      //      UIElement parentCanvas = FindParent<Canvas>(content);

      //      Point curContentPoint = e.GetPosition(parentCanvas);
      //      Vector rectangleDragVector = curContentPoint - origContentMouseDownPoint;

      //      //
      //      // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
      //      //

      //      origContentMouseDownPoint = curContentPoint;

      //      if (sender is FrameworkElement)
      //      {
      //         FrameworkElement fwe = (FrameworkElement)sender;

      //         if (fwe.DataContext is IPositionInterface)
      //         {
      //            IPositionInterface obj = (IPositionInterface)fwe.DataContext;

      //            obj.PosX += rectangleDragVector.X;
      //            obj.PosY += rectangleDragVector.Y;
      //         }

      //      }
      //      e.Handled = true;
      //   }
      //}

      public static T FindParent<T>(DependencyObject child) where T : DependencyObject
         // From: https://www.infragistics.com/community/blogs/b/blagunas/posts/find-the-parent-control-of-a-specific-type-in-wpf-and-silverlight
      {
         //get parent item
         DependencyObject parentObject = VisualTreeHelper.GetParent(child);

         //we've reached the end of the tree
         if (parentObject == null) return null;

         //check if the parent matches the type we're looking for
         T parent = parentObject as T;
         if (parent != null)
            return parent;
         else
            return FindParent<T>(parentObject);
      }

      private void Shape_Rotate(object sender, MouseWheelEventArgs e)
      {
         if (sender is FrameworkElement)
         {
            FrameworkElement fwe = (FrameworkElement)sender;

            if (fwe.DataContext is LfShapeViewModel)
            {
               LfShapeViewModel svm = (LfShapeViewModel)fwe.DataContext;

               if (svm.IsSelected)
               {
                  svm.Angle += (double)e.Delta / 120 * 5 / 180 * Math.PI;
                  svm.InvalidateAll();
                  e.Handled = true;
               }
            }
         }
      }
   }
}
