using System;
using System.Collections.Generic;
using System.Diagnostics;
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


//      private void General_MouseDown(object sender, MouseButtonEventArgs e)
//      {
//         UIElement parentCanvas = FindParent<Canvas>(content);
//         parentCanvas.Focus();
//         Keyboard.Focus(parentCanvas);

//         mouseButtonDown = e.ChangedButton;

//         if (mouseButtonDown == MouseButton.Left)
//         {
//            mouseHandlingMode = ZoomAndPan.MouseHandlingMode.DraggingObjects;
//            origContentMouseDownPoint = e.GetPosition(parentCanvas);

//            UIElement dragObject = (UIElement)sender;
//            dragObject.CaptureMouse();

////          e.Handled = true;
//         }
         
//         bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
//         bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
//         bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

//         if (sender is FrameworkElement)
//         {
//            FrameworkElement fwe = (FrameworkElement)sender;

//            if (fwe.DataContext is IPositionInterface)
//            {
//               IPositionInterface obj = (IPositionInterface)fwe.DataContext;
//               if (obj.MainVm.MouseDown(Type,  fwe, e.ChangedButton, e.GetPosition(content), e.ClickCount, shift, ctrl, alt))
//               {
//                  e.Handled = true;
//               }

//            }
//         }
//      }

//      private void General_MouseUp(object sender, MouseButtonEventArgs e)
//      {
//         UIElement parentCanvas = FindParent<Canvas>(content);
//         parentCanvas.Focus();
//         Keyboard.Focus(parentCanvas);

//         if (mouseHandlingMode == ZoomAndPan.MouseHandlingMode.DraggingObjects)
//         {
//            mouseButtonDown = e.ChangedButton;

//            if (mouseButtonDown == MouseButton.Left)
//            {
//               mouseHandlingMode = ZoomAndPan.MouseHandlingMode.None;

//               UIElement rectangle = (UIElement)sender;
//               rectangle.ReleaseMouseCapture();

////             e.Handled = true;
//            }
//         }

//         bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
//         bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
//         bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

//         if (sender is FrameworkElement)
//         {
//            FrameworkElement fwe = (FrameworkElement)sender;

//            if (fwe.DataContext is IPositionInterface)
//            {
//               IPositionInterface obj = (IPositionInterface)fwe.DataContext;

//               if (obj.MainVm.MouseUp(fwe, e.ChangedButton, e.GetPosition(content), e.ClickCount, shift, ctrl, alt))
//               {
//                  e.Handled = true;
//               }
//            }
//         }
//      }

//      private void General_MouseMove(object sender, MouseEventArgs e)
//      {
//         if (mouseHandlingMode == ZoomAndPan.MouseHandlingMode.DraggingObjects)
//         {
//            if (mouseButtonDown == MouseButton.Left)
//            {
//               UIElement parentCanvas = FindParent<Canvas>(content);

//               Point curContentPoint = e.GetPosition(parentCanvas);
//               Vector rectangleDragVector = curContentPoint - origContentMouseDownPoint;

//               origContentMouseDownPoint = curContentPoint;

//               bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
//               bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
//               bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

//               if (sender is FrameworkElement)
//               {
//                  FrameworkElement fwe = (FrameworkElement)sender;

//                  if (fwe.DataContext is IPositionInterface)
//                  {
//                     IPositionInterface obj = (IPositionInterface)fwe.DataContext;

//                     obj.MainVm.MouseMove(fwe, rectangleDragVector, shift, ctrl, alt);
//                  }
//               }

//               e.Handled = true;

//            }
//         }
//      }

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

               bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);

               if (ctrl)
               {
                  svm.RotateShape(e.Delta);
                  e.Handled = true;
               }
            }
         }
      }

      private void GeneralMouse<T>(bool down, MouseEventObjectType type, object sender, MouseButtonEventArgs e)
      {
         UIElement parentCanvas = FindParent<Canvas>(content);
         parentCanvas.Focus();
         Keyboard.Focus(parentCanvas);

         mouseButtonDown = e.ChangedButton;

         bool shift = ((Keyboard.Modifiers & ModifierKeys.Shift) != 0);
         bool ctrl = ((Keyboard.Modifiers & ModifierKeys.Control) != 0);
         bool alt = ((Keyboard.Modifiers & ModifierKeys.Alt) != 0);

         // If ctrl is held down, we never intent to drag things
         if (!ctrl && (mouseButtonDown == MouseButton.Left))
         {
            if (down)
            {
               mouseHandlingMode = ZoomAndPan.MouseHandlingMode.DraggingObjects;
               origContentMouseDownPoint = e.GetPosition(parentCanvas);

               UIElement dragObject = (UIElement)sender;
               dragObject.CaptureMouse();

               e.Handled = true;
            }
            else
            {
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
            }
         }

         if (sender is FrameworkElement)
         {
            FrameworkElement fwe = (FrameworkElement)sender;

            if (fwe.DataContext is T)
            {
               IMainVmInterface obj = (IMainVmInterface)fwe.DataContext;

               if (down)
               {
                  if (obj.MainVm.MouseDown(type, obj, e.ChangedButton, e.GetPosition(content), e.ClickCount, shift, ctrl, alt))
                  {
                     e.Handled = true;
                  }
               }
               else
               {
                  if (obj.MainVm.MouseUp(type, obj, e.ChangedButton, e.GetPosition(content), e.ClickCount, shift, ctrl, alt))
                  {
                     e.Handled = true;
                  }
               }
            }
         }
      }

      private void GeneralMouseMove<T>(MouseEventObjectType type, object sender, MouseEventArgs e)
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

                  if (fwe.DataContext is IMainVmInterface)
                  {
                     IMainVmInterface mvm = (IMainVmInterface)fwe.DataContext;

                     T obj = (T)fwe.DataContext;

                     mvm.MainVm.MouseMove(type, obj, rectangleDragVector, shift, ctrl, alt);
                  }
               }

               e.Handled = true;

            }
         }
      }

      private void Shape_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<LfShapeViewModel>(true, MouseEventObjectType.shape, sender, e);
      }

      private void Shape_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<LfShapeViewModel>(false, MouseEventObjectType.shape, sender, e);
      }

      private void ShapeBorder_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<LfDragablePointViewModel>(true, MouseEventObjectType.dragableBorder, sender, e);
      }

      private void ShapeBorder_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<LfDragablePointViewModel>(false, MouseEventObjectType.dragableBorder, sender, e);
      }

      private void ShapeBorder_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<IPositionInterface>(MouseEventObjectType.dragableBorder, sender, e);
      }

      private void DragablePoint_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<LfDragablePointViewModel>(true, MouseEventObjectType.dragablePoint, sender, e);
      }

      private void DragablePoint_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<LfDragablePointViewModel>(false, MouseEventObjectType.dragablePoint, sender, e);
      }

      private void DragablePoint_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<IPositionInterface>(MouseEventObjectType.dragablePoint, sender, e);
      }

      private void CompoundObject_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<CompoundObjectViewModel>(true, MouseEventObjectType.compoundObjectBoundaryBox, sender, e);
      }

      private void CompoundObject_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<CompoundObjectViewModel>(false, MouseEventObjectType.compoundObjectBoundaryBox, sender, e);
      }

      private void CompoundObject_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<IPositionInterface>(MouseEventObjectType.compoundObjectBoundaryBox, sender, e);
      }

      private void AnchorA_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<WeldJointViewModel>(true, MouseEventObjectType.jointAnchorA, sender, e);
      }

      private void AnchorA_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<WeldJointViewModel>(false, MouseEventObjectType.jointAnchorA, sender, e);
      }

      private void AnchorA_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<WeldJointViewModel>(MouseEventObjectType.jointAnchorA, sender, e);
      }

      private void AnchorB_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<WeldJointViewModel>(true, MouseEventObjectType.jointAnchorB, sender, e);
      }

      private void AnchorB_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<WeldJointViewModel>(false, MouseEventObjectType.jointAnchorB, sender, e);
      }

      private void AnchorB_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<WeldJointViewModel>(MouseEventObjectType.jointAnchorB, sender, e);
      }

      private void UpperLimit_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<PrismaticJointViewModel>(true, MouseEventObjectType.prismJointUpperLimit, sender, e);
      }

      private void UpperLimit_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<PrismaticJointViewModel>(false, MouseEventObjectType.prismJointUpperLimit, sender, e);
      }

      private void UpperLimit_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<PrismaticJointViewModel>(MouseEventObjectType.prismJointUpperLimit, sender, e);
      }

      private void LowerLimit_MouseDown(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<PrismaticJointViewModel>(true, MouseEventObjectType.prismJointLowerLimit, sender, e);
      }

      private void LowerLimit_MouseUp(object sender, MouseButtonEventArgs e)
      {
         GeneralMouse<PrismaticJointViewModel>(false, MouseEventObjectType.prismJointLowerLimit, sender, e);
      }

      private void LowerLimit_MouseMove(object sender, MouseEventArgs e)
      {
         GeneralMouseMove<PrismaticJointViewModel>(MouseEventObjectType.prismJointLowerLimit, sender, e);
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
