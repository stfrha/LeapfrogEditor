using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;



/*
 *  How about a new set of view model classes: StateShapes, StateJoints, StateSystems and StateChildren
 * 
 * This class would have one ObservableCollection for each state and for each such class
 * Each such class would have one CompositeCollection (or ObservableCollection) for each 
 * object of each class.
 * There can be a HierarchicalDataTemplate for each State-class. The CompoundObjectViewModel
 * will have one CompositeCollection of all State-classes which will group each object type
 * into one level of the TreeView. 
 * 
 * Lets try it
 * 
 * 
 * 
 */





namespace LeapfrogEditor
{
   public class CompoundObjectViewModel : TreeViewViewModel, IPositionInterface
   {
      #region Declarations

      private ChildObject _childObjectOfParent;
      private CompoundObject _modelObject;
      private ChildObjectStateProperties _modelObjectProperties;
      private CompoundObjectViewModel _parentVm;

      private int _selectedStateIndex = 0;

      private CoBehaviourViewModel _behaviour;
      private int _selectedBehaviourIndex = 0;

      private StateShapeCollectionViewModel _shapes;
      private StateJointCollectionViewModel _joints;
      private StateSystemCollectionViewModel _systems;

      // Collection below is two dimensional to accomondate for all State properties
      // The outer collection is all the states of the CompoundObject and the inner collection
      // is the ChildObjects of each state
      private ObservableCollection<StateChildCollectionViewModel> _childObjects = new ObservableCollection<StateChildCollectionViewModel>();

      private ObservableCollection<StateCollectionViewModelBase> _treeCollection = new ObservableCollection<StateCollectionViewModelBase>();
      #endregion

      #region Constructors

      public CompoundObjectViewModel(
         MainViewModel mainVm, 
         CompoundObject modelObject, 
         ChildObjectStateProperties modelObjectProperties, 
         CompoundObjectViewModel parentVm, 
         ChildObject childObjectOfParent)
      {
         MainVm = mainVm;
         ModelObject = modelObject;
         ModelObjectProperties = modelObjectProperties;
         ParentVm = parentVm;
         ChildObjectOfParent = childObjectOfParent;
         SelectedStateIndex = 0;
         Behaviour = new CoBehaviourViewModel(mainVm, this, ModelObject.Behaviour);

         BuildTreeViewCollection();

         SelectedBehaviourIndex = Behaviours.IndexOf(ModelObject.Behaviour.Type);
      }

      #endregion

      #region Properties

      public ChildObject ChildObjectOfParent
      {
         get { return _childObjectOfParent; }
         set
         {
            _childObjectOfParent = value;
            OnPropertyChanged("");
         }
      }

      public CompoundObject ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public ChildObjectStateProperties ModelObjectProperties
      {
         get { return _modelObjectProperties; }
         set
         {
            _modelObjectProperties = value;
            OnPropertyChanged("");
         }
      }


      public CompoundObjectViewModel ParentVm
      {
         get { return _parentVm; }
         set
         {
            _parentVm = value;
            OnPropertyChanged("");
         }
      }
      
      public CoBehaviourViewModel Behaviour
      {
         get { return _behaviour; }
         set
         {
            _behaviour = value;
            OnPropertyChanged("Behaviour");
         }
      }

      public string Name
      {
         get { return _childObjectOfParent.Name; }
         set
         {
            _childObjectOfParent.Name = value;
            OnPropertyChanged("Name");
         }
      }

      public string RefName
      {
         get
         {
            if ((ModelObjectProperties.File == "") || (ModelObjectProperties.File == "undef_file.xml"))
            {
               return Name;
            }

            return Name + " - " + ModelObjectProperties.File;
         }
         set
         {
         }
      }

      public double PosX
      {
         get { return _modelObjectProperties.PosX; }
         set
         {
            _modelObjectProperties.PosX = value;

            OnPropertyChanged("PosX");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }
         }
      }

      public double PosY
      {
         get { return _modelObjectProperties.PosY; }
         set
         {
            _modelObjectProperties.PosY = value;

            OnPropertyChanged("PosY");
            OnPropertyChanged("BoundingBox");

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }
         }
      }

      public int SelectedStateIndex
      {
         get
         {
            return _selectedStateIndex;
         }
         set
         {
            if (value == -1)
            {
               _selectedStateIndex = 0;
            }
            else
            {
               _selectedStateIndex = value;
            }

            DeselectAllChildren();
            OnPropertyChanged("");
            BuildTreeViewCollection();

            CompoundObjectViewModel p = ParentVm;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.ParentVm;
            }
         }
      }

      public ObservableCollection<string> States
      {
         get { return ModelObject.States; }
         set { }
      }

      public StateShapeCollectionViewModel StateShapes
      {
         get { return _shapes; }
         set
         {
            _shapes = value;
            OnPropertyChanged("StateShapes");
         }
      }

      public StateJointCollectionViewModel StateJoints
      {
         get { return _joints; }
         set
         {
            _joints = value;
            OnPropertyChanged("StateJoints");
         }
      }

      public StateSystemCollectionViewModel StateSystems
      {
         get { return _systems; }
         set
         {
            _systems = value;
            OnPropertyChanged("StateSystems");
         }
      }

      public StateChildCollectionViewModel StateChildObjects
      {
         get
         {
            if ((_selectedStateIndex >= 0) && (_childObjects.Count > _selectedStateIndex))
            {
               return _childObjects[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_childObjects.Count > _selectedStateIndex))
            {
               _childObjects[_selectedStateIndex] = value;
            }
         }
      }

      public List<string> Behaviours
      {
         get
         {
            return new List<string>()
            {
               "notApplicable",
               "leapfrog",
               "launchSite",
               "landingPad",
               "breakableObject",
               "steerableObject"
            };
         }
      }

      public int SelectedBehaviourIndex
      {
         get
         {
            return _selectedBehaviourIndex;
         }
         set
         {
            if (value == -1)
            {
               _selectedBehaviourIndex = 0;
            }
            else
            {
               _selectedBehaviourIndex = value;
            }
            OnPropertyChanged("SelectedBehaviourIndex");

            Behaviour.Type = Behaviours[SelectedBehaviourIndex];
         }
      }



      public ObservableCollection<StateCollectionViewModelBase> TreeCollection
      {
         get { return _treeCollection; }
         set { _treeCollection = value; }
      }

      public Rect BoundingBox
      {
         get
         {
            if (StateShapes == null) return new Rect(0, 0, 0, 0);

            if ((StateShapes.Shapes.Count == 0) && (StateChildObjects.Children.Count == 0))
            {
               return new Rect(0,0,0,0);
            }

            BoundingBoxRect bbr = new BoundingBoxRect();

            if (StateShapes.Shapes.Count > 0)
            {
               foreach (object o in StateShapes.Shapes)
               {
                  if (o is LfShapeViewModel)
                  {
                     LfShapeViewModel shape = (LfShapeViewModel)o;

                     Rect cb = shape.BoundingBox;
                     cb.Offset(new Vector(shape.PosX, shape.PosY));
                     bbr.AddRect(cb);
                  }
               }
            }

            if (StateChildObjects.Children.Count > 0)
            {
               foreach (CompoundObjectViewModel child in StateChildObjects.Children)
               {
                  Rect cb = child.BoundingBox;
                  cb.Offset(new Vector(child.PosX, child.PosY));
                  bbr.AddRect(cb);
               }
            }

            return bbr.BoundingBox;
         }
         set
         { }
      }

      // We override the IsSelected since we want to unselect all 
      // if this is a newly selected CO
      public new bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            _isSelected = value;

            if (!_isSelected)
            {
               DeselectAllChildren();
            }

            OnPropertyChanged("IsSelected");
         }
      }

      #endregion

      #region Private Methods

      private void BuildTreeViewCollection()
      {
         _treeCollection.Clear();
         _treeCollection.Add(StateShapes);
         _treeCollection.Add(StateJoints);
         _treeCollection.Add(StateSystems);
         _treeCollection.Add(StateChildObjects);
      }

      private StateShapeCollectionViewModel SetShapes(CompoundObject co)
      {
         StateShapeCollectionViewModel shapes = new StateShapeCollectionViewModel(this);

         foreach (LfSpriteBox sb in co.SpriteBoxes)
         {
            LfSpriteBoxViewModel shapevm = new LfSpriteBoxViewModel(MainVm, this, sb);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfSpritePolygon sp in co.SpritePolygons)
         {
            LfSpritePolygonViewModel shapevm = new LfSpritePolygonViewModel(MainVm, this, sp);

            foreach (LfDragablePoint dragPoint in sp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticBox sb in co.StaticBoxes)
         {
            LfStaticBoxViewModel shapevm = new LfStaticBoxViewModel(MainVm, this, sb);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticCircle sb in co.StaticCircles)
         {
            LfStaticCircleViewModel shapevm = new LfStaticCircleViewModel(MainVm, this, sb);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticPolygon sp in co.StaticPolygons)
         {
            LfStaticPolygonViewModel shapevm = new LfStaticPolygonViewModel(MainVm, this, sp);

            foreach (LfDragablePoint dragPoint in sp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicBox db in co.DynamicBoxes)
         {
            LfDynamicBoxViewModel shapevm = new LfDynamicBoxViewModel(MainVm, this, db);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicCircle db in co.DynamicCircles)
         {
            LfDynamicCircleViewModel shapevm = new LfDynamicCircleViewModel(MainVm, this, db);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicPolygon dp in co.DynamicPolygons)
         {
            LfDynamicPolygonViewModel shapevm = new LfDynamicPolygonViewModel(MainVm, this, dp);

            foreach (LfDragablePoint dragPoint in dp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticBoxedSpritePolygon bsp in co.StaticBoxedSpritePolygons)
         {
            LfStaticBoxedSpritePolygonViewModel shapevm = new LfStaticBoxedSpritePolygonViewModel(MainVm, this, bsp);

            foreach (LfDragablePoint dragPoint in bsp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicBoxedSpritePolygon bsp in co.DynamicBoxedSpritePolygons)
         {
            LfDynamicBoxedSpritePolygonViewModel shapevm = new LfDynamicBoxedSpritePolygonViewModel(MainVm, this, bsp);

            foreach (LfDragablePoint dragPoint in bsp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         return shapes;

      }

      private StateJointCollectionViewModel SetJoints(CompoundObject co)
      {
         StateJointCollectionViewModel joints = new StateJointCollectionViewModel(this);

         foreach (WeldJoint wj in co.WeldJoints)
         {
            WeldJointViewModel wjvm = new WeldJointViewModel(MainVm, this, wj);
            joints.Joints.Add(wjvm);
         }

         foreach (RevoluteJoint rj in co.RevoluteJoints)
         {
            RevoluteJointViewModel rjvm = new RevoluteJointViewModel(MainVm, this, rj);
            joints.Joints.Add(rjvm);
         }

         foreach (PrismaticJoint pj in co.PrismaticJoints)
         {
            PrismaticJointViewModel pjvm = new PrismaticJointViewModel(MainVm, this, pj);
            joints.Joints.Add(pjvm);
         }

         foreach (Rope r in co.Ropes)
         {
            RopeViewModel rvm = new RopeViewModel(MainVm, this, r);
            joints.Joints.Add(rvm);
         }

         return joints;
      }

      private StateSystemCollectionViewModel SetSystems(CompoundObject co)
      {
         StateSystemCollectionViewModel systems = new StateSystemCollectionViewModel(this);

         foreach (CoSystem s in co.Systems)
         {
            CoSystemViewModel svm = new CoSystemViewModel(MainVm, this, s);
            systems.Systems.Add(svm);
         }

         return systems;
      }


      private ObservableCollection<StateChildCollectionViewModel> SetChildren(CompoundObject co)
      // This method creates the collection of StateChildCollectionViewModels for each state of the
      // CompoundObject. The StateChildCollectionViewModel for each state will hold the ChildObject that
      // mathches the state. However, if there is no match of the state, and the ChildObject has another
      // state that is "default", this is included in the state. If the ChildObject does not have a 
      // default state, either, it is not included in the state.
      // The default state is compulsory for all CompoundObjects. 
      // 
      {
         ObservableCollection<StateChildCollectionViewModel> stateChildren = new ObservableCollection<StateChildCollectionViewModel>();

         // We iterate the list of states first
         foreach (string stateStr in States)
         {

            StateChildCollectionViewModel stateChild = new StateChildCollectionViewModel(this);

            bool matchedState = false;

            //... then we iterate all ChildObjects of the CO and look if any of them
            // has a matching state. 
            foreach (ChildObject cho in co.ChildObjects)
            {
               // Now we iterate all state properties of this ChildObject and process it
               // if the state of the ChildObject matches the current stateStr
               foreach (TStateProperties<ChildObjectStateProperties> sp in cho.StateProperties)
               {
                  if (sp.State == stateStr)
                  {
                     // Now process this ChildObject and insert this ChildObject in this state
                     CompoundObjectViewModel covm = new CompoundObjectViewModel(MainVm, sp.Properties.CompObj, sp.Properties, this, cho);
                     covm.BuildViewModel(cho);
                     stateChild.Children.Add(covm);

                     matchedState = true;
                  }
               }

               if (!matchedState)
               {
                  // There was no match, now we iterate all state properties again and process it
                  // if the state of the ChildObject is "default"
                  foreach (TStateProperties<ChildObjectStateProperties> sp in cho.StateProperties)
                  {
                     if (sp.State == "default")
                     {
                        // Now process this ChildObject and insert this ChildObject in this state
                        CompoundObjectViewModel covm = new CompoundObjectViewModel(MainVm, sp.Properties.CompObj, sp.Properties, this, cho);
                        covm.BuildViewModel(cho);
                        stateChild.Children.Add(covm);
                     }
                  }
               }
            }

            stateChildren.Add(stateChild);
         }

         return stateChildren;
      }

      #endregion

      #region Public Methods

      public void RemoveShape(LfShapeViewModel svm)
      {
         // Check if there are any joints connected to this svm, if so, removed them
         // We may remove joints so we need a for loop here:
         for (int i = StateJoints.Joints.Count - 1; i >= 0; i--)
         {
            // Below will take care of all joints since they
            // all inherit from WeldJoint
            if (StateJoints.Joints[i] is WeldJointViewModel)
            {
               WeldJointViewModel joint = (WeldJointViewModel)StateJoints.Joints[i];

               if ((joint.AName == svm.Name) || (joint.BName == svm.Name))
               {
                  // Remove the joint
                  ModelObject.RemoveJoint(joint.ModelObject);
                  StateJoints.Joints.RemoveAt(i);
               }
            }
         }

         // Remove the shape model
         ModelObject.RemoveShape(svm.ModelObject);

         // Remove the shape viewmodel from this
         StateShapes.Shapes.Remove(svm);

         // If there are no more shapes in the CO, remove the CO
         if (StateShapes.Shapes.Count == 0)
         {
            //ParentVm.StateChildObjects.Remove(this);
            //ParentVm.ModelObject.ChildObjectRefs(this.ChildObjectOfParent)
         }

         OnPropertyChanged("");
      }

      public void InvalidateJoints()
      {
         foreach (object o in StateJoints.Joints)
         {
            // Below will take care of all joints since they
            // all inherit from WeldJoint
            if (o is WeldJointViewModel)
            {
               WeldJointViewModel joint = (WeldJointViewModel)o;

               joint.OnPropertyChanged("");
            }
         }
      }

      public LfShapeViewModel FindShape(string name, StateShapeCollectionViewModel shapes)
      {
         foreach (object o in shapes.Shapes)
         {
            if (o is LfShapeViewModel)
            {
               LfShapeViewModel shape = (LfShapeViewModel)o;

               if (shape.Name == name)
               {
                  return shape;
               }
            }
         }

         return null;
      }

      public void BuildViewModel(ChildObject cor)
      // This method connects references in the COVM. At the creation of the COVM
      // the ModelObject, ModelObjectProperties, ParentVm and ChildObjectOfParent is defined.
      // Here we build the shape, joint and system collections and connect those objects 
      // with the necessary references. The ChildObjects are also added to the corresponding
      // state index in the StateChildObjects collection
      {
         _childObjects.Clear();
         _treeCollection.Clear();

         _shapes = SetShapes(ModelObject);
         _joints = SetJoints(ModelObject);
         _systems = SetSystems(ModelObject);

         // Only now is the Joints property valid for this state
         foreach (object o in _joints.Joints)
         {
            if (o is WeldJointViewModel)
            {
               if (o is RopeViewModel)
               {
                  RopeViewModel joint = (RopeViewModel)o;

                  joint.ConnectToShapes(_shapes);
               }
               else
               {
                  WeldJointViewModel joint = (WeldJointViewModel)o;

                  joint.ConnectToShapes(_shapes);
               }
            }
         }

         _childObjects = SetChildren(ModelObject);

         BuildTreeViewCollection();
      }

      public void DeselectAllChildren()
      {
         if ((StateShapes != null) && (StateShapes.Shapes != null))
         {
            foreach (object o in StateShapes.Shapes)
            {
               if (o is LfShapeViewModel)
               {
                  LfShapeViewModel shape = (LfShapeViewModel)o;

                  shape.IsSelected = false;
               }
            }
         }

         if (StateJoints != null)
         {
            foreach (object o in StateJoints.Joints)
            {
               if (o is WeldJointViewModel)
               {
                  WeldJointViewModel joint = (WeldJointViewModel)o;

                  joint.IsSelected = false;
               }
            }
         }

         if (StateChildObjects != null)
         {
            foreach (CompoundObjectViewModel child in StateChildObjects.Children)
            {
               // child.DeselectAllChildren();
               child.IsSelected = false;
            }
         }
      }


      public bool ChildHasFileReference(string fileName)
      {
         foreach (StateChildCollectionViewModel sccvm in _childObjects)
         {
            foreach (CompoundObjectViewModel covm in sccvm.Children)
            {
               if (covm.ModelObjectProperties.File == fileName)
               {
                  return true;
               }
               else
               {
                  if (covm.ChildHasFileReference(fileName))
                  {
                     return true;
                  }
               }
            }
         }

         return false;
      }



      // This method returns with the point of the shape's coordinate system
      // in this CompoundObject's coordinate system. 
      // The position of the shape is added to the shape point. The shape point
      // can be either rotated or not.
      public Point ShapePointInCo(Point shapePoint, LfShapeViewModel shape)
      {
         if (shape == null) return new Point(0, 0);

         Point coPoint = shapePoint;

         coPoint.Offset(shape.PosX, shape.PosY);

         return coPoint;
      }

      // This method returns with the point of the CO's coordinate system
      // in this Shape's coordinate system. 
      // The position of the shape is subtracted from the CO point. 
      public Point CoPointInShape(Point coPoint, LfShapeViewModel shape)
      {
         if (shape == null) return new Point(0, 0);

         Point shapePoint = coPoint;

         shapePoint.Offset(-shape.PosX, -shape.PosY);

         return shapePoint;
      }

      // This method returns with the supplied point (expressed in this CompoundObject's
      // coordinate system) converted to the parent CompoundObjects coordinate system.
      // This is done by adding the Position of this CompoundObject.
      public Point ParentCoPoint(Point coPoint)
      {
         Point parentPoint = coPoint;

         parentPoint.Offset(PosX, PosY);

         return parentPoint;
      }

      // This method returns with the supplied point (expressed in the parent of this 
      // CompoundObject's coordinate system) converted to this CompoundObjects coordinate system.
      // This is done by subtracting the parentPoint with the position of this CompoundObject.
      public Point CoPointFromParent(Point parentPoint)
      {
         Point coPoint = parentPoint;

         coPoint.Offset(-PosX, -PosY);

         return coPoint;
      }

      // This method returns with the supplied point in the scene's coordinate system
      // (Equal to the top level CompoundObject's coordinate system). 
      // The point is converted to the parent's coordinate system and the method is then
      // recursively called for the parent until the parent is null. In this case we are 
      // at the top level CompoundObject which is the scene. Then we returns the point
      public Point GetScenePointFromCoPoint(Point coPoint)
      {
         if (ParentVm != null)
         {
            Point parentPoint = ParentCoPoint(coPoint);
            return GetScenePointFromCoPoint(parentPoint);
         }
         else
         {
            return coPoint;
         }
      }

      public Point GetCoPointFromScenePoint(Point scenePoint)
      {
         if (ParentVm != null)
         {
            Point parentPoint = CoPointFromParent(scenePoint);
            return GetCoPointFromScenePoint(parentPoint);
         }
         else
         {
            return scenePoint;
         }
      }

      public void GenerateTriangles()
      {
         foreach (object o in StateShapes.Shapes)
         {
            if (o is LfPolygonViewModel)
            {
               LfPolygonViewModel pvm = (LfPolygonViewModel)o;

               pvm.GenerateTriangles();
            }
         }

         foreach (CompoundObjectViewModel covm in StateChildObjects.Children)
         {
            covm.GenerateTriangles();
         }
      }

      #endregion

   }
}
