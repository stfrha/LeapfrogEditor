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
      private CompoundObjectViewModel _parent;

      private int _selectedStateIndex = 0;

      private CoBehaviourViewModel _behaviour;

      // Collections below is two dimensional to accomondate for all State properties
      // The outher collection is all StateChildObjects and the inner collection
      // is the states of that child
      private ObservableCollection<StateShapeCollectionViewModel> _shapes = new ObservableCollection<StateShapeCollectionViewModel>();
      private ObservableCollection<StateJointCollectionViewModel> _joints = new ObservableCollection<StateJointCollectionViewModel>();
      private ObservableCollection<StateSystemCollectionViewModel> _systems = new ObservableCollection<StateSystemCollectionViewModel>();
      private ObservableCollection<StateChildCollectionViewModel> _childObjects = new ObservableCollection<StateChildCollectionViewModel>();

      private ObservableCollection<StateCollectionViewModelBase> _treeCollection = new ObservableCollection<StateCollectionViewModelBase>();
      #endregion

      #region Constructors

      public CompoundObjectViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, ChildObject childObjectOfParent)
      {
         MainVm = mainVm;
         Parent = parent;
         ChildObjectOfParent = childObjectOfParent;
         SelectedStateIndex = 0;
         BuildTreeViewCollection();
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
         get
         {
            if (_childObjectOfParent != null)
            {
               if (_childObjectOfParent.StateProperties.Count > 0)
               {
                  return _childObjectOfParent.StateProperties[_selectedStateIndex].Properties.CompObj;
               }
            }
            return null;
         }
         set { }
      }

      public CompoundObjectViewModel Parent
      {
         get { return _parent; }
         set
         {
            _parent = value;
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

      public double PosX
      {
         get
         {
            if (_childObjectOfParent != null)
            {
               if ((_selectedStateIndex >= 0) && (_childObjectOfParent.StateProperties.Count > 0))
               {
                  return _childObjectOfParent.StateProperties[_selectedStateIndex].Properties.PosX;
               }
            }
            return 0;
         }
         set
         {
            if (_childObjectOfParent != null)
            {
               if ((_selectedStateIndex >= 0) && (_childObjectOfParent.StateProperties.Count > 0))
               {
                  _childObjectOfParent.StateProperties[_selectedStateIndex].Properties.PosX = value;

                  OnPropertyChanged("PosX");
                  OnPropertyChanged("BoundingBox");

                  CompoundObjectViewModel p = Parent;

                  while (p != null)
                  {
                     p.OnPropertyChanged("BoundingBox");
                     p = p.Parent;
                  }
               }
            }
         }
      }

      public double PosY
      {
         get
         {
            if (_childObjectOfParent != null)
            {
               if ((_selectedStateIndex >= 0) && (_childObjectOfParent.StateProperties.Count > _selectedStateIndex))
               {
                  return _childObjectOfParent.StateProperties[_selectedStateIndex].Properties.PosY;
               }
            }
            return 0;
         }
         set
         {
            if (_childObjectOfParent != null)
            {
               if ((_selectedStateIndex >= 0) && (_childObjectOfParent.StateProperties.Count > 0))
               {
                  _childObjectOfParent.StateProperties[_selectedStateIndex].Properties.PosY = value;

                  OnPropertyChanged("PosY");
                  OnPropertyChanged("BoundingBox");

                  CompoundObjectViewModel p = Parent;

                  while (p != null)
                  {
                     p.OnPropertyChanged("BoundingBox");
                     p = p.Parent;
                  }
               }
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

            CompoundObjectViewModel p = Parent;

            while (p != null)
            {
               p.OnPropertyChanged("BoundingBox");
               p = p.Parent;
            }
         }
      }

      public ObservableCollection<string> States
      {
         get { return ModelObject.States; }
         set { }
      }

      // Old implementation, is this of any use?
      //public ObservableCollection<string> States
      //{
      //   get
      //   {
      //      ObservableCollection<string> s = new ObservableCollection<string>();

      //      foreach (TStateProperties<ChildObjectStateProperties> sp in _childObjectOfParent.StateProperties)
      //      {
      //         s.Add(sp.State);
      //      }

      //      return s;
      //   }
      //   set { }
      //}

      public StateShapeCollectionViewModel StateShapes
      {
         get
         {
            if ((_selectedStateIndex >= 0) && (_shapes.Count > _selectedStateIndex))
            {
               return _shapes[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_shapes.Count > _selectedStateIndex))
            {
               _shapes[_selectedStateIndex] = value;
            }
         }
      }

      public StateJointCollectionViewModel StateJoints
      {
         get
         {
            if ((_selectedStateIndex >= 0) && (_joints.Count > _selectedStateIndex))
            {
               return _joints[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_joints.Count > _selectedStateIndex))
            {
               _joints[_selectedStateIndex] = value;
            }
         }
      }

      public StateSystemCollectionViewModel StateSystems
      {
         get
         {
            if ((_selectedStateIndex >= 0) && (_shapes.Count > _selectedStateIndex))
            {
               return _systems[_selectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((_selectedStateIndex >= 0) && (_shapes.Count > _selectedStateIndex))
            {
               _systems[_selectedStateIndex] = value;
            }
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


      private StateChildCollectionViewModel SetChildren(CompoundObject co)
      {
         StateChildCollectionViewModel tempChildren = new StateChildCollectionViewModel(this);

         foreach (ChildObject cho in co.ChildObjects)
         {
            CompoundObjectViewModel covm = new CompoundObjectViewModel(MainVm, this, cho);
            covm.BuildViewModel(cho);
            tempChildren.Children.Add(covm);
         }

         return tempChildren;
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
            //Parent.StateChildObjects.Remove(this);
            //Parent.ModelObject.ChildObjectRefs(this.ChildObjectOfParent)
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
      {
         _shapes.Clear();
         _joints.Clear();
         _systems.Clear();
         _childObjects.Clear();
         _treeCollection.Clear();

         foreach (TStateProperties<ChildObjectStateProperties> sp in cor.StateProperties)
         {
            StateShapeCollectionViewModel sc = SetShapes(sp.Properties.CompObj);
            _shapes.Add(sc);

            StateJointCollectionViewModel jc = SetJoints(sp.Properties.CompObj);
            _joints.Add(jc);

            _systems.Add(SetSystems(sp.Properties.CompObj));

            _childObjects.Add(SetChildren(sp.Properties.CompObj));

            // Only now is the Joints property valid for this state
            foreach (object o in jc.Joints)
            {
               if (o is WeldJointViewModel)
               {
                  if (o is RopeViewModel)
                  {
                     RopeViewModel joint = (RopeViewModel)o;

                     joint.ConnectToShapes(sc);
                  }
                  else
                  {
                     WeldJointViewModel joint = (WeldJointViewModel)o;

                     joint.ConnectToShapes(sc);
                  }
               }
            }
         }

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
         if (Parent != null)
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
         if (Parent != null)
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
