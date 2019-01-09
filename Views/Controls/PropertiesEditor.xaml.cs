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



      public SpawnObjectViewModel EditableSpawnObject
      {
         get { return (SpawnObjectViewModel)GetValue(EditableSpawnObjectProperty); }
         set { SetValue(EditableSpawnObjectProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableSpawnObject.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableSpawnObjectProperty =
          DependencyProperty.Register("EditableSpawnObject", typeof(SpawnObjectViewModel), typeof(PropertiesEditor), new PropertyMetadata(default(SpawnObjectViewModel)));




      public ObservableCollection<CompoundObjectViewModel> SelectedChildren
      {
         get { return (ObservableCollection<CompoundObjectViewModel>)GetValue(SelectedChildrenProperty); }
         set { SetValue(SelectedChildrenProperty, value); }
      }

      // Using a DependencyProperty as the backing store for SelectedChildren.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty SelectedChildrenProperty =
          DependencyProperty.Register("SelectedChildren", typeof(ObservableCollection<CompoundObjectViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<CompoundObjectViewModel>)));


      // Fortsätt här någonstans!!!!!


      public ObservableCollection<LfShapeViewModel> EditableShapes
      {
         get { return (ObservableCollection<LfShapeViewModel>)GetValue(EditableShapesProperty); }
         set { SetValue(EditableShapesProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableShapes.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableShapesProperty =
          DependencyProperty.Register("EditableShapes", typeof(ObservableCollection<LfShapeViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<LfShapeViewModel>)));




      public ObservableCollection<WeldJointViewModel> EditableJoints
      {
         get { return (ObservableCollection<WeldJointViewModel>)GetValue(EditableJointsProperty); }
         set { SetValue(EditableJointsProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableJoints.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableJointsProperty =
          DependencyProperty.Register("EditableJoints", typeof(ObservableCollection<WeldJointViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<WeldJointViewModel>)));



      public ObservableCollection<CoSystemViewModel> EditableSystems
      {
         get { return (ObservableCollection<CoSystemViewModel>)GetValue(EditableSystemsProperty); }
         set { SetValue(EditableSystemsProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableSystems.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableSystemsProperty =
          DependencyProperty.Register("EditableSystems", typeof(ObservableCollection<CoSystemViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<CoSystemViewModel>)));

   }
}
