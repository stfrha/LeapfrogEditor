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



      public ObservableCollection<ChildObjectViewModel> EditableChildren
      {
         get { return (ObservableCollection<ChildObjectViewModel>)GetValue(EditableChildrenProperty); }
         set { SetValue(EditableChildrenProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableChildren.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableChildrenProperty =
          DependencyProperty.Register("EditableChildren", typeof(ObservableCollection<ChildObjectViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<ChildObjectViewModel>)));


      public ObservableCollection<StateViewModel> EditableStates
      {
         get { return (ObservableCollection<StateViewModel>)GetValue(EditableStatesProperty); }
         set { SetValue(EditableStatesProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableStates.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableStatesProperty =
          DependencyProperty.Register("EditableStates", typeof(ObservableCollection<StateViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<StateViewModel>)));



      public ObservableCollection<ChildCOViewModel> ChildObjectStateProperties
      {
         get { return (ObservableCollection<ChildCOViewModel>)GetValue(ChildObjectStatePropertiesProperty); }
         set { SetValue(ChildObjectStatePropertiesProperty, value); }
      }

      // Using a DependencyProperty as the backing store for ChildObjectStateProperties.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty ChildObjectStatePropertiesProperty =
          DependencyProperty.Register("ChildObjectStateProperties", typeof(ObservableCollection<ChildCOViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<ChildCOViewModel>)));



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



      public ObservableCollection<SpawnObjectViewModel> EditableSpawnObjects
      {
         get { return (ObservableCollection<SpawnObjectViewModel>)GetValue(EditableSpawnObjectsProperty); }
         set { SetValue(EditableSpawnObjectsProperty, value); }
      }

      // Using a DependencyProperty as the backing store for EditableSpawnObjects.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty EditableSpawnObjectsProperty =
          DependencyProperty.Register("EditableSpawnObjects", typeof(ObservableCollection<SpawnObjectViewModel>), typeof(PropertiesEditor), new PropertyMetadata(default(ObservableCollection<SpawnObjectViewModel>)));


   }
}
