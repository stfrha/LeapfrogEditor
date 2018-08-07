using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
   /// Interaction logic for PropertiesEditor.xaml
   /// </summary>
   public partial class PropertiesEditor : UserControl
   {
      public PropertiesEditor()
      {
         InitializeComponent();
      }

      public CompoundObjectViewModel EditableCompoundObject
      {
         get { return (CompoundObjectViewModel)GetValue(EditableCompoundObjectProperty); }
         set { SetValue(EditableCompoundObjectProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableCompoundObject.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableCompoundObjectProperty =
          DependencyProperty.Register("EditableCompoundObject", typeof(CompoundObjectViewModel), typeof(PropertiesEditor), new PropertyMetadata(default(CompoundObjectViewModel)));



      public ObservableCollection<LfShapeViewModel> EditableShapes
      {
         get { return (ObservableCollection<LfShapeViewModel>)GetValue(observableCollectionProperty); }
         set { SetValue(observableCollectionProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableShapes.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty observableCollectionProperty =
          DependencyProperty.Register("EditableShapes", typeof(ObservableCollection<LfShapeViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<LfShapeViewModel>)));




      public ObservableCollection<WeldJointViewModel> EditableJoints
      {
         get { return (ObservableCollection<WeldJointViewModel>)GetValue(EditableJointsProperty); }
         set { SetValue(EditableJointsProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableJoints.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableJointsProperty =
          DependencyProperty.Register("EditableJoints", typeof(ObservableCollection<WeldJointViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<WeldJointViewModel>)));

   }
}
