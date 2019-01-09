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
 * The CompoundObjectViewModel encompass the CompoundObject (incl its states, and behaviour) 
 * as well as all the ChildObjects of all states
 */


namespace LeapfrogEditor
{
   public class CompoundObjectViewModel : ConditionalSelectTreeViewViewModel, IPositionInterface
   {
      #region Declarations

      private CompoundObject _modelObject;
      private ChildObjectStatePropertiesViewModel _objectPropertiesVm;


      private CoBehaviourViewModel _behaviour;
      private int _selectedBehaviourIndex = 0;

      private StateShapeCollectionViewModel _shapes;
      private StateJointCollectionViewModel _joints;
      private StateSystemCollectionViewModel _systems;

      // There are two viewmodels structures for the ChildObjects
      // One is sorted on each individual ChildObject with each states under it (_childObjectsWithStates).
      // This is used for the tree view where all ChildObjects are shown (regardless of what 
      // state is displayed in the graphics window), used for creating new state properties of them
      // and to be able to edit ChildObjects properties even if they are not shown.
      // The other (_statesWithChildObjects) is sorted on the states and each state only contains the ChildObjects
      // of that state. This is for showing in the graphics view. Perhaps this could be handled
      // by a ValueConverter that sets the visibility of the object in the graphics window.


      private StateChildCollectionViewModel _childObjectsWithStates;
      private ObservableCollection<ObservableCollection<CompoundObjectViewModel>> _statesWithChildObjects = new ObservableCollection<ObservableCollection<CompoundObjectViewModel>>();

      private ObservableCollection<StateCollectionViewModelBase> _treeCollection = new ObservableCollection<StateCollectionViewModelBase>();
      #endregion

      #region Constructors

      public CompoundObjectViewModel(
         TreeViewViewModel treeParent,
         CompoundObjectViewModel parentVm,
         MainViewModel mainVm, 
         CompoundObject modelObject ) :
         base(treeParent, parentVm, mainVm)
      {
         ModelObject = modelObject;

         if (TreeParent is ChildObjectStatePropertiesViewModel)
         {
            _objectPropertiesVm = TreeParent as ChildObjectStatePropertiesViewModel;
         }
         else
         {
            _objectPropertiesVm = null;
         }

         Behaviour = new CoBehaviourViewModel(TreeParent, this, MainVm, ModelObject.Behaviour);

         SelectedBehaviourIndex = Behaviours.IndexOf(ModelObject.Behaviour.Type);

         ChildObjectsWithStates = new StateChildCollectionViewModel(treeParent, parentVm, MainVm);
      }

      #endregion

      #region Properties

/*
 * 
 * We need some new properties
 * 
 * Name -            this is the name of the ChildObject (if this is a child) or the 
 *                   file name (if this is a FileCOViewModel)
 * 
 * IsFileReference - true if this a ChidlObject that has a file reference. 
 *                   false, otherwise.
 *                   
 * FileName        - is the file reference of the ChildObject.stateProperties.file if
 *                   this is a ChildObject that has a file reference. If not, this is ""
 *                   
 */


      //public ChildObject ChildObjectOfParent
      //{
      //   get { return _childObjectOfParent; }
      //   set
      //   {
      //      _childObjectOfParent = value;
      //      OnPropertyChanged("");
      //   }
      //}

      public CompoundObject ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public ChildObjectStatePropertiesViewModel ObjectPropertiesVm
      {
         get { return _objectPropertiesVm; }
         set
         {
            _objectPropertiesVm = value;
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
         get
         {
            if (this is FileCOViewModel)
            {
               FileCOViewModel fcvm = this as FileCOViewModel;

               string fileName = System.IO.Path.GetFileName(fcvm.FileName);

               return fileName;
            }

            if ((TreeParent != null) && (TreeParent.TreeParent != null) && (TreeParent.TreeParent is ChildObjectViewModel))
            {
               ChildObjectViewModel covm = TreeParent.TreeParent as ChildObjectViewModel;

               return covm.Name;
            }

            return "Error: could not resolve name";
         }
         set
         {
            if ((TreeParent != null) && (TreeParent.TreeParent != null) && (TreeParent.TreeParent is ChildObjectViewModel))
            {
               ChildObjectViewModel covm = TreeParent.TreeParent as ChildObjectViewModel;

               covm.Name = value;
               OnPropertyChanged("Name");
               covm.OnPropertyChanged("Name");
            }
         }
      }

      public bool IsFileReferenceChild
      {
         get
         {
            if ((TreeParent != null) && (TreeParent is ChildObjectStatePropertiesViewModel))
            {
               ChildObjectStatePropertiesViewModel covm = TreeParent as ChildObjectStatePropertiesViewModel;

               if ((covm.File == "") || (covm.File == "undef_file.xml"))
               {
                  return true;
               }
            }

            return false;
         }
      }

      public string ReferenceChildFileName
      {
         get
         {
            if ((TreeParent != null) && (TreeParent is ChildObjectStatePropertiesViewModel))
            {
               ChildObjectStatePropertiesViewModel covm = TreeParent as ChildObjectStatePropertiesViewModel;

               return covm.File;
            }

            return "";
         }
      }

      //public string RefName
      //{
      //   get
      //   {
      //      if ((ModelObjectProperties.File == "") || (ModelObjectProperties.File == "undef_file.xml"))
      //      {
      //         return Name;
      //      }

      //      return Name + " - " + ModelObjectProperties.File;
      //   }
      //   set
      //   {
      //   }
      //}

      public double PosX
      {
         get
         {
            if (ObjectPropertiesVm == null)
            {
               return 0;
            }

            return ObjectPropertiesVm.PosX;
         }
         set
         {
            if (ObjectPropertiesVm == null)
            {
               ObjectPropertiesVm.PosX = value;

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
      }

      public double PosY
      {
         get
         {
            if (ObjectPropertiesVm == null)
            {
               return 0;
            }

            return ObjectPropertiesVm.PosY;
         }
         set
         {
            if (ObjectPropertiesVm == null)
            {
               ObjectPropertiesVm.PosY = value;

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
          

      public StateChildCollectionViewModel ChildObjectsWithStates
      {
         get { return _childObjectsWithStates; }
         set { _childObjectsWithStates = value; }
      }

      public ObservableCollection<CompoundObjectViewModel> StateChildObjects
      {
         get
         {
            if ((Behaviour.SelectedStateIndex >= 0) && (_statesWithChildObjects.Count > Behaviour.SelectedStateIndex))
            {
               return _statesWithChildObjects[Behaviour.SelectedStateIndex];
            }
            return null;
         }
         set
         {
            if ((Behaviour.SelectedStateIndex >= 0) && (_statesWithChildObjects.Count > Behaviour.SelectedStateIndex))
            {
               _statesWithChildObjects[Behaviour.SelectedStateIndex] = value;
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
               "scene",
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

            if ((StateShapes.Shapes.Count == 0) && (StateChildObjects.Count == 0))
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

            if (StateChildObjects.Count > 0)
            {
               foreach (CompoundObjectViewModel child in StateChildObjects)
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

      #endregion

      #region Private Methods

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


      private CompoundObjectViewModel FindCompoundObjectViewModelInChildrenObjects(ChildObjectStateProperties findMe)
      {
         foreach (ChildObjectViewModel childvm in ChildObjectsWithStates.Children)
         {
            foreach (ChildObjectStatePropertiesViewModel chospvm in childvm.StateProperties)
            {
               if (chospvm.ModelObject.Properties == findMe)
               {
                  return chospvm.CompoundObjectChild;
               }
            }
         }

         return null;
      }

      // All children CompoundObjectViewModels are created here. The constructor of the ChildObjectViewModel
      // creates a ChildObjectStatePropertiesViewModel which's constructor creates the CompoundObjectViewModel.
      private StateChildCollectionViewModel SetChildren(CompoundObject ModelObject)
      {
         StateChildCollectionViewModel schcvm = new StateChildCollectionViewModel(TreeParent, ParentVm, MainVm);

         foreach (ChildObject cho in ModelObject.ChildObjects)
         {
            ChildObjectViewModel chovm = new ChildObjectViewModel(TreeParent, this, MainVm, cho);
            schcvm.Children.Add(chovm);
         }

         return schcvm;
      }

      private ObservableCollection<ObservableCollection<CompoundObjectViewModel>> SetStateChildren(CompoundObject co)
      // This method creates the collection of StateChildCollectionViewModels for each state of the
      // CompoundObject. The StateChildCollectionViewModel for each state will hold the ChildObject that
      // mathches the state. However, if there is no match of the state, and the ChildObject has another
      // state that is "default", this is included in the state. If the ChildObject does not have a 
      // default state, either, it is not included in the state.
      // The default state is compulsory for all CompoundObjects. 
      // This method does not create and CompoundObjectViewModels since we want the same instances of these
      // in both the ChildObject collections.
      // 
      {
         ObservableCollection<ObservableCollection<CompoundObjectViewModel>> stateChildrenCollection = new ObservableCollection<ObservableCollection<CompoundObjectViewModel>>();

         // We iterate the list of states first
         foreach (StateViewModel stateVm in Behaviour.States)
         {

            ObservableCollection<CompoundObjectViewModel> stateChildren = new ObservableCollection<CompoundObjectViewModel>();

            bool matchedState = false;

            //... then we iterate all ChildObjects of the CO and look if any of them
            // has a matching state. 
            foreach (ChildObject cho in co.ChildObjects)
            {
               // Now we iterate all state properties of this ChildObject and process it
               // if the state of the ChildObject matches the current stateStr
               foreach (TStateProperties<ChildObjectStateProperties> sp in cho.StateProperties)
               {
                  if (sp.State == stateVm.StateName)
                  {
                     // Now process this ChildObject and insert this ChildObject in this state
                     CompoundObjectViewModel covm = FindCompoundObjectViewModelInChildrenObjects(sp.Properties);
                     covm.BuildViewModel(cho);
                     stateChildren.Add(covm);

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
                        CompoundObjectViewModel covm = FindCompoundObjectViewModelInChildrenObjects(sp.Properties);
                        covm.BuildViewModel(cho);
                        stateChildren.Add(covm);
                     }
                  }
               }
            }

            stateChildrenCollection.Add(stateChildren);
         }

         return stateChildrenCollection;
      }

      #endregion

      #region Public Methods


      public void SetBehaviourPropertyInTreeView()
      {
         // TODO: Since the scene is now a behaviour we should add states as treeview children of 
         // the behaviour tree item.

         // Remove the current element (if any) in the TreeCollection that is of BehaviourViewModelBase
         foreach (StateCollectionViewModelBase s in TreeCollection)
         {
            if (s is BehaviourViewModelBase)
            {
               TreeCollection.Remove(s);
               break;
            }
         }

         if ((Behaviour != null) && 
            ((Behaviour.Type == "breakableObject") || (Behaviour.Type == "scene")))
         {
            TreeCollection.Insert(0, Behaviour.BehaviourProperties);
         }
      }

      public void BuildTreeViewCollection()
      {
         _treeCollection.Clear();

         // Behaviour child is not ready to be set, it will 
         // be set later.
         SetBehaviourPropertyInTreeView();

         _treeCollection.Add(StateShapes);
         _treeCollection.Add(StateJoints);
         _treeCollection.Add(StateSystems);
         _treeCollection.Add(ChildObjectsWithStates);
      }

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
         _statesWithChildObjects.Clear();
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

         _childObjectsWithStates = SetChildren(ModelObject);
         _statesWithChildObjects = SetStateChildren(ModelObject);

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
            foreach (ChildObjectViewModel child in ChildObjectsWithStates.Children)
            {
               child.DeselectAllChildren();
               child.IsSelected = false;
            }
         }
      }


      public bool ChildHasFileReference(string fileName)
         // TODO: What was the purpose of this method?
      {
         //foreach (ObservableCollection<CompoundObjectViewModel> sccvm in _statesWithChildObjects)
         //{
         //   foreach (CompoundObjectViewModel covm in sccvm)
         //   {
         //      if (covm.ModelObjectProperties.File == fileName)
         //      {
         //         return true;
         //      }
         //      else
         //      {
         //         if (covm.ChildHasFileReference(fileName))
         //         {
         //            return true;
         //         }
         //      }
         //   }
         //}

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

         foreach (ChildObjectViewModel covm in ChildObjectsWithStates.Children)
         {
            foreach (ChildObjectStatePropertiesViewModel propvm in covm.StateProperties)
            {
               propvm.CompoundObjectChild.GenerateTriangles();
            }
         }
      }

      #endregion

   }
}
