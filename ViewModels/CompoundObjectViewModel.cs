﻿using System;
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
   public class CompoundObjectViewModel : ConditionalSelectTreeViewViewModel, IPositionInterface
   {
      #region Declarations

      private ChildObject _childObjectOfParent;
      private CompoundObject _modelObject;
      private ChildObjectStateProperties _modelObjectProperties;

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
         TreeViewViewModel treeParent,
         CompoundObjectViewModel parentVm,
         MainViewModel mainVm, 
         CompoundObject modelObject, 
         ChildObjectStateProperties modelObjectProperties, 
         ChildObject childObjectOfParent) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;
         ModelObjectProperties = modelObjectProperties;
         ChildObjectOfParent = childObjectOfParent;

         Behaviour = new CoBehaviourViewModel(TreeParent, this, MainVm, ModelObject.Behaviour);

         SelectedStateIndex = 0;
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

            SetBehaviourPropertyInTreeView();

            // TODO: Except for updating the Type property 
            // which will expose a new BehaviourProperty,
            // we must also update the child of the Tree View
            // and this must be done here.
            // This ViewModel object is the Behaviour property of
            // the CompoundObjectViewModel. The COVM also has a TreeCollection
            // property which holds folders for all shapes, joints, systems, child
            // objects and Behaviour property. The Behaviour property should be the
            // set to either _steerableObjProperties or _breakableObjProperties
            // depending on the Type property. Both these types are derived of
            // BehaviourViewModelBase which is derived of TreeViewViewModel.
            // By letting BehaviourViewModelBase be derived from a new class that
            // is derived from StateCollectionViewModelBase, which basically is empty,
            // and derived from TreeViewViewModel it can be inserted into the TreeCollection
            // and thus will be displayed in the tree. 
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

      //// We override the IsSelected since we want to unselect all 
      //// if this is a newly selected CO
      //public new bool IsSelected
      //{
      //   get { return _isSelected; }
      //   set
      //   {
      //      _isSelected = value;

      //      if (!_isSelected)
      //      {
      //         DeselectAllChildren();
      //      }

      //      OnPropertyChanged("IsSelected");
      //   }
      //}

      public void SetBehaviourPropertyInTreeView()
      {
         // Remove the current element (if any) in the TreeCollection that is of BehaviourViewModelBase
         foreach (StateCollectionViewModelBase s in TreeCollection)
         {
            if (s is BehaviourViewModelBase)
            {
               TreeCollection.Remove(s);
               break;
            }
         }

         TreeCollection.Insert(0, Behaviour.BehaviourProperties);
      }

      #endregion

      #region Private Methods

      private void BuildTreeViewCollection()
      {
         _treeCollection.Clear();

         // Behaviour child is not ready to be set, it will 
         // be set later.
         _treeCollection.Add(Behaviour.BehaviourProperties);

         _treeCollection.Add(StateShapes);
         _treeCollection.Add(StateJoints);
         _treeCollection.Add(StateSystems);
         _treeCollection.Add(StateChildObjects);
      }

      private StateShapeCollectionViewModel SetShapes(CompoundObject co)
      {
         StateShapeCollectionViewModel shapes = new StateShapeCollectionViewModel(this, this, MainVm);

         foreach (LfSpriteBox sb in co.SpriteBoxes)
         {
            LfSpriteBoxViewModel shapevm = new LfSpriteBoxViewModel(shapes, this, MainVm, sb);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfSpritePolygon sp in co.SpritePolygons)
         {
            LfSpritePolygonViewModel shapevm = new LfSpritePolygonViewModel(shapes, this, MainVm, sp);

            foreach (LfDragablePoint dragPoint in sp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticBox sb in co.StaticBoxes)
         {
            LfStaticBoxViewModel shapevm = new LfStaticBoxViewModel(shapes, this, MainVm, sb);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticCircle sb in co.StaticCircles)
         {
            LfStaticCircleViewModel shapevm = new LfStaticCircleViewModel(shapes, this, MainVm, sb);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticPolygon sp in co.StaticPolygons)
         {
            LfStaticPolygonViewModel shapevm = new LfStaticPolygonViewModel(shapes, this, MainVm, sp);

            foreach (LfDragablePoint dragPoint in sp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicBox db in co.DynamicBoxes)
         {
            LfDynamicBoxViewModel shapevm = new LfDynamicBoxViewModel(shapes, this, MainVm, db);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicCircle db in co.DynamicCircles)
         {
            LfDynamicCircleViewModel shapevm = new LfDynamicCircleViewModel(shapes, this, MainVm, db);
            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicPolygon dp in co.DynamicPolygons)
         {
            LfDynamicPolygonViewModel shapevm = new LfDynamicPolygonViewModel(shapes, this, MainVm, dp);

            foreach (LfDragablePoint dragPoint in dp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfStaticBoxedSpritePolygon bsp in co.StaticBoxedSpritePolygons)
         {
            LfStaticBoxedSpritePolygonViewModel shapevm = new LfStaticBoxedSpritePolygonViewModel(shapes, this, MainVm, bsp);

            foreach (LfDragablePoint dragPoint in bsp.Points)
            {
               LfDragablePointViewModel dragPointVm = new LfDragablePointViewModel(MainVm, shapevm, dragPoint);
               shapevm.PointVms.Add(dragPointVm);
            }

            shapes.Shapes.Add(shapevm);
         }

         foreach (LfDynamicBoxedSpritePolygon bsp in co.DynamicBoxedSpritePolygons)
         {
            LfDynamicBoxedSpritePolygonViewModel shapevm = new LfDynamicBoxedSpritePolygonViewModel(shapes, this, MainVm, bsp);

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
         StateJointCollectionViewModel joints = new StateJointCollectionViewModel(this, this, MainVm);

         foreach (WeldJoint wj in co.WeldJoints)
         {
            WeldJointViewModel wjvm = new WeldJointViewModel(joints, this, MainVm, wj);
            joints.Joints.Add(wjvm);
         }

         foreach (RevoluteJoint rj in co.RevoluteJoints)
         {
            RevoluteJointViewModel rjvm = new RevoluteJointViewModel(joints, this, MainVm, rj);
            joints.Joints.Add(rjvm);
         }

         foreach (PrismaticJoint pj in co.PrismaticJoints)
         {
            PrismaticJointViewModel pjvm = new PrismaticJointViewModel(joints, this, MainVm, pj);
            joints.Joints.Add(pjvm);
         }

         foreach (Rope r in co.Ropes)
         {
            RopeViewModel rvm = new RopeViewModel(joints, this, MainVm, r);
            joints.Joints.Add(rvm);
         }

         return joints;
      }

      private StateSystemCollectionViewModel SetSystems(CompoundObject co)
      {
         StateSystemCollectionViewModel systems = new StateSystemCollectionViewModel(this, this, MainVm);

         foreach (CoSystem s in co.Systems)
         {
            CoSystemViewModel svm = new CoSystemViewModel(systems, this, MainVm, s);
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

            StateChildCollectionViewModel stateChild = new StateChildCollectionViewModel(this, this, MainVm);

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
                     CompoundObjectViewModel covm = new CompoundObjectViewModel(stateChild, this, MainVm, sp.Properties.CompObj, sp.Properties, cho);
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
                        CompoundObjectViewModel covm = new CompoundObjectViewModel(stateChild, this, MainVm, sp.Properties.CompObj, sp.Properties, cho);
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
               child.DeselectAllChildren();
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
