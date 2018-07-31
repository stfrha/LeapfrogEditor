using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   public enum LeftClickState
   {
      none,
      spriteBox,
      spritePolygon,
      staticBox,
      staticCircle,
      staticPolygon,
      staticBoxedSpritePolygon,
      dynamicBox,
      dynamicCircle,
      dynamicPolygon,
      dynamicBoxedSpritePolygon,
      weldJoint,
      revoluteJoint,
      prismaticJoint,
      addPoint,
      asteroidField
   }

   public enum MouseEventObjectType
   {
      none,                         // object is null
      shape,                        // object is the shape
      dragablePolygonBorder,        // border that a point can be added to,
                                    // object is the point (before or after?) the line
      dragableBorder,               // object is the point (before or after?) the line
      dragablePoint,                // object is the point
      compoundObjectBoundaryBox,    // object is the CompoundObject
      jointAnchorA,                 // object is the joint
      jointAnchorB,                 // object is the joint
      prismJointUpperLimit,         // object is the prismatic joint
      prismJointLowerLimit          // object is the prismatic joint
   }

   public class MainViewModel : MicroMvvm.ViewModelBase
   {

      #region Declarations

      private CompoundObjectRef _myCpRef = new CompoundObjectRef();
      private ObjectRefStateProperties _myStateProp = new ObjectRefStateProperties();
      private CompoundObject _myCP;
      private CompoundObjectViewModel _myCpVm;

      private CompoundObjectViewModel _selectedCompoundObject = null;
      private ObservableCollection<LfShapeViewModel> _selectedShapes = new ObservableCollection<LfShapeViewModel>();
      private ObservableCollection<WeldJointViewModel> _selectedJoints = new ObservableCollection<WeldJointViewModel>();
      private ObservableCollection<LfDragablePointViewModel> _selectedPoints = new ObservableCollection<LfDragablePointViewModel>();

      private ZLevels _zLevels = new ZLevels();
      private CollisionEntities _collEnts = new CollisionEntities();

      private LeftClickState _LeftClickState = LeftClickState.none;

      private bool _showJoints;
      private bool _showTriangles;

      private ObservableCollection<string> _textures = new ObservableCollection<string>();

      #endregion

      #region Constructor

      public MainViewModel()
      {
         string fileName = "z_levels.xml";
         string s = @"..\..\..\leapfrog\data\" + fileName;
         string fullPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
         string fullFileName = System.IO.Path.Combine(fullPath, s);

         MyZLevels = ZLevels.ReadFromFile(fullFileName);

         fileName = "collision_entities.xml";
         s = @"..\..\..\leapfrog\data\" + fileName;
         fullPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
         fullFileName = System.IO.Path.Combine(fullPath, s);

         CollEnts = CollisionEntities.ReadFromFile(fullFileName);

         //fileName = "landing_scene.xml";
         //s = @"..\..\..\leapfrog\data\" + fileName;
         //fullPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
         //fullFileName = System.IO.Path.Combine(fullPath, s);

         //MyStateProp.File = fullFileName;
         //MyCP = CompoundObject.ReadFromFile(fullFileName);

         //MyStateProp.CompObj = MyCP;
         //MyCpRef.StateProperties.Add(MyStateProp);

         //MyCpVm = new CompoundObjectViewModel(this, null, MyCpRef);
         //MyCpVm.BuildViewModel(MyCpRef);

         // Build collections of texture names
         // Process the list of files found in the directory.
         s = @"..\..\..\leapfrog\data\images";

         string[] fileEntries = Directory.GetFiles(s);
         foreach (string file in fileEntries)
         {
            string fname = System.IO.Path.GetFileNameWithoutExtension(file);
            string ext = System.IO.Path.GetExtension(file);

            if (ext == ".png")
            {
               Textures.Add(fname);
            }
         }

      }

      #endregion

      #region Properties

      public CompoundObjectRef MyCpRef
      {
         get { return _myCpRef; }
         set { _myCpRef = value; }
      }

      public ObjectRefStateProperties MyStateProp
      {
         get { return _myStateProp; }
         set { _myStateProp = value; }
      }

      public CompoundObject MyCP
      {
         get { return _myCP; }
         set { _myCP = value; }
      }

      public CompoundObjectViewModel MyCpVm
      {
         get { return _myCpVm; }
         set
         {
            _myCpVm = value;
            OnPropertyChanged("MyCpVm");
         }
      }

      public string WindowTitle
      {
         get
         {
            if (MyCpVm == null)
            {
               return "Leapfrog Editor - No file loaded";
            }

            string fileName = System.IO.Path.GetFileName(MyCpVm.File);

            return "Leapfrog Editor - " + fileName;
         }
      }

      public CompoundObjectViewModel SelectedCompoundObject
      {
         get { return _selectedCompoundObject; }
         set
         {
            _selectedCompoundObject = value;
            OnPropertyChanged("SelectedCompoundObject");
         }
      }

      public ObservableCollection<LfShapeViewModel> SelectedShapes
      {
         get { return _selectedShapes; }
         set { _selectedShapes = value; }
      }

      public ObservableCollection<LfDragablePointViewModel> SelectedPoints
      {
         get { return _selectedPoints; }
         set { _selectedPoints = value; }
      }

      public LeftClickState LeftClickState
      {
         get { return _LeftClickState; }
         set { _LeftClickState = value; }
      }

      public bool ShowJoints
      {
         get { return _showJoints; }
         set
         {
            _showJoints = value;
            OnPropertyChanged("ShowJoints");
         }
      }

      public bool ShowTriangles
      {
         get { return _showTriangles; }
         set
         {
            _showTriangles = value;
            OnPropertyChanged("ShowTriangles");
         }
      }

      public ZLevels MyZLevels
      {
         get { return _zLevels; }
         set
         {
            _zLevels = value;
            OnPropertyChanged("MyZLevels");
         }
      }

      public CollisionEntities CollEnts
      {
         get { return _collEnts; }
         set
         {
            _collEnts = value;
            OnPropertyChanged("CollEnts");
         }
      }

      public ObservableCollection<string> Textures
      {
         get { return _textures; }
         set { _textures = value; }
      }


      #endregion

      #region Commands

      void NewExecute(Object parameter)
      {
         if (MessageBox.Show("There is no check if there is unsaved data. Do you really want to open a new scene?", "Open Scene Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
         {
            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == true)
            {
               string fileName = System.IO.Path.GetFileName(sfd.FileName);
               string s = @"..\..\..\leapfrog\data\" + fileName;
               string fullPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
               string fullFileName = System.IO.Path.Combine(fullPath, s);

               MyStateProp = new ObjectRefStateProperties();
               MyStateProp.File = fullFileName;

               MyCP = new CompoundObject();

               MyStateProp.CompObj = MyCP;

               MyCpRef = new CompoundObjectRef();

               MyCpRef.StateProperties.Add(MyStateProp);

               MyCpVm = new CompoundObjectViewModel(this, null, MyCpRef);

               // To get a handle to the new CompoundObject we need a shape
               // to select. Lets place a default Sprite Box at coordinate 0,0
               LfStaticCircle defShape = new LfStaticCircle();
               MyCP.StaticCircles.Add(defShape);
               LfStaticCircleViewModel defShapeVM = new LfStaticCircleViewModel(this, null, defShape);
               MyCpVm.BuildViewModel(MyCpRef);
               MyCpVm.OnPropertyChanged("");
               OnPropertyChanged("");
            }
         }
      }

      bool CanNewExecute(Object parameter)
      {
         return true;
      }

      public ICommand New
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => NewExecute(parameter), parameter => CanNewExecute(parameter));
         }
      }

      void ReloadExecute(Object parameter)
      {
         if (MessageBox.Show("There is no check if there is unsaved data. Do you really want to open a new scene?", "Open Scene Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
         {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Scene files (*.xml)|*.xml|All files (*.*)|*.*";
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == true)
            {
               string fileName = System.IO.Path.GetFileName(ofd.FileName);
               string s = @"..\..\..\leapfrog\data\" + fileName;
               string fullPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
               string fullFileName = System.IO.Path.Combine(fullPath, s);

               MyStateProp.File = fullFileName;

               MyCP = CompoundObject.ReadFromFile(fullFileName);

               MyStateProp.CompObj = MyCP;
               MyCpRef.StateProperties.Add(MyStateProp);

               MyCpVm = new CompoundObjectViewModel(this, null, MyCpRef);
               MyCpVm.BuildViewModel(MyCpRef);
               MyCpVm.OnPropertyChanged("");
               OnPropertyChanged("");
            }
         }
      }

      bool CanReloadExecute(Object parameter)
      {
         return true;
      }

      public ICommand Reload
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => ReloadExecute(parameter), parameter => CanReloadExecute(parameter));
         }
      }

      void SaveExecute(Object parameter)
      {
         // Generate Triangles before saving
         MyCpVm.GenerateTriangles();

         MyCpVm.ModelObject.WriteToFile(MyStateProp.File);
      }

      bool CanSaveExecute(Object parameter)
      {
         return true;
      }

      public ICommand Save
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => SaveExecute(parameter), parameter => CanSaveExecute(parameter));
         }
      }

      void SaveAsExecute(Object parameter)
      {
         SaveFileDialog sfd = new SaveFileDialog();

         if (sfd.ShowDialog() == true)
         {
            MyCpVm.GenerateTriangles();

            MyStateProp.File = sfd.FileName;
            MyCpVm.ModelObject.WriteToFile(MyStateProp.File);
         }
      }

      bool CanSaveAsExecute(Object parameter)
      {
         return true;
      }

      public ICommand SaveAs
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => SaveAsExecute(parameter), parameter => CanSaveAsExecute(parameter));
         }
      }

      void ImportCompoundObjectExecute(Object parameter)
      {
         OpenFileDialog ofd = new OpenFileDialog();

         ofd.Filter = "Compound Object files (*.xml)|*.xml|All files (*.*)|*.*";
         ofd.CheckFileExists = true;

         if (ofd.ShowDialog() == true)
         {
            string fileName = System.IO.Path.GetFileName(ofd.FileName);
            string s = @"..\..\..\leapfrog\data\" + fileName;
            string fullPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string fullFileName = System.IO.Path.Combine(fullPath, s);

            // Fortsätt här!!!!!

            ObjectRefStateProperties newStateProp = new ObjectRefStateProperties();
            newStateProp.State = "default";
            newStateProp.File = fullFileName;

            CompoundObject newCp = CompoundObject.ReadFromFile(fullFileName);

            newStateProp.CompObj = newCp;

            CompoundObjectRef newCpRef = new CompoundObjectRef();

            newCpRef.StateProperties.Add(newStateProp);

            CompoundObjectViewModel newCpVm = new CompoundObjectViewModel(this, null, newCpRef);
            newCpVm.BuildViewModel(newCpRef);

            SelectedCompoundObject.ModelObject.ChildObjectRefs.Add(newCpRef);
            SelectedCompoundObject.ChildObjects.Add(newCpVm);

            MyCpVm.OnPropertyChanged("");
            OnPropertyChanged("");
         }
      }

      bool CanImportCompoundObjectExecute(Object parameter)
      {
         return (SelectedCompoundObject != null);
      }

      public ICommand ImportCompoundObject
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => ImportCompoundObjectExecute(parameter), parameter => CanImportCompoundObjectExecute(parameter));
         }
      }


      void SetShapeWidthExecute(Object parameter)
      {
         if (parameter is IWidthHeightInterface)
         {
            IWidthHeightInterface iwh = (IWidthHeightInterface)parameter;

            iwh.SetWidthToTextureAspectRatio();
         }
      }

      bool CanSetShapeWidthExecute(Object parameter)
      {
         return true;
      }

      public ICommand SetShapeWidth
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => SetShapeWidthExecute(parameter), parameter => CanSetShapeWidthExecute(parameter));
         }
      }

      void SetShapeHeightExecute(Object parameter)
      {
         if (parameter is IWidthHeightInterface)
         {
            IWidthHeightInterface iwh = (IWidthHeightInterface)parameter;

            iwh.SetHeightToTextureAspectRatio();
         }
      }

      bool CanSetShapeHeightExecute(Object parameter)
      {
         return true;
      }

      public ICommand SetShapeHeight
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => SetShapeHeightExecute(parameter), parameter => CanSetShapeHeightExecute(parameter));
         }
      }

      void CreateTrianglesExecute(Object parameter)
      {
         MyCpVm.GenerateTriangles();
      }

      bool CanCreateTrianglesExecute(Object parameter)
      {
         return true;
      }

      public ICommand CreateTriangles
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => CreateTrianglesExecute(parameter), parameter => CanCreateTrianglesExecute(parameter));
         }
      }

      void StateExecute(Object parameter)
      {
         CompoundObjectViewModel covm = MyCpVm.ChildObjects[0];

         if (covm.SelectedStateIndex == 0)
         {
            covm.SelectedStateIndex = 1;
         }
         else
         {
            covm.SelectedStateIndex = 0;
         }
      }

      bool CanStateExecute(Object parameter)
      {
         return true;
      }

      public ICommand State
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => StateExecute(parameter), parameter => CanStateExecute(parameter));
         }
      }

       

      //void NewStaticPolygonExecute(Object parameter)
      //{
      //   // Deselect all other shapes when generating a new polygon
      //   foreach (LfShapeViewModel shape in _selectedShapes)
      //   {
      //      shape.IsSelected = false;
      //   }

      //   _selectedShapes.Clear();

      //   _LeftClickState = LeftClickState.staticPolygon;
      //}

      //bool CanNewStaticPolygonExecute(Object parameter)
      //{
      //   return (SelectedCompoundObject != null);
      //}

      //public ICommand NewStaticPolygon
      //{
      //   get
      //   {
      //      return new MicroMvvm.RelayCommand<Object>(parameter => NewStaticPolygonExecute(parameter), parameter => CanNewStaticPolygonExecute(parameter));
      //   }
      //}

      //void NewDynamicPolygonExecute(Object parameter)
      //{
      //   // Deselect all other shapes when generating a new polygon
      //   foreach (LfShapeViewModel shape in _selectedShapes)
      //   {
      //      shape.IsSelected = false;
      //   }

      //   _selectedShapes.Clear();

      //   _LeftClickState = LeftClickState.dynamicPolygon;
      //}

      //bool CanNewDynamicPolygonExecute(Object parameter)
      //{
      //   return (SelectedCompoundObject != null);
      //}

      //public ICommand NewDynamicPolygon
      //{
      //   get
      //   {
      //      return new MicroMvvm.RelayCommand<Object>(parameter => NewDynamicPolygonExecute(parameter), parameter => CanNewDynamicPolygonExecute(parameter));
      //   }
      //}


      void NewShapeExecute(Object parameter)
      {
         // Deselect all other shapes when generating a new polygon
         foreach (LfShapeViewModel shape in _selectedShapes)
         {
            shape.IsSelected = false;
         }

         _selectedShapes.Clear();

         string param = parameter as string;

         if (!Enum.TryParse<LeftClickState>(param,  out _LeftClickState))
         {
            _LeftClickState = LeftClickState.none;
         }
      }

      bool CanNewShapeExecute(Object parameter)
      {
         return ((SelectedCompoundObject != null) && (parameter is string));
      }

      public ICommand NewShape
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => NewShapeExecute(parameter), parameter => CanNewShapeExecute(parameter));
         }
      }

      void NewJointExecute(Object parameter)
      {
         string param = parameter as string;

         if (!Enum.TryParse<LeftClickState>(param, out _LeftClickState))
         {
            _LeftClickState = LeftClickState.none;
         }
      }

      bool CanNewJointExecute(Object parameter)
      {
         return ((SelectedCompoundObject != null) && (_selectedShapes.Count == 2) && (parameter is string));
      }

      public ICommand NewJoint
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => NewJointExecute(parameter), parameter => CanNewJointExecute(parameter));
         }
      }

      void DeleteExecute(Object parameter)
      {
         if (_selectedPoints.Count > 0)
         {
            foreach (LfDragablePointViewModel dp in _selectedPoints)
            {
               LfPolygonViewModel polyVm = dp.Parent;

               // Is this the last point to be removed? If so, remove the shape
               // first so there is no problem with updating something with zero
               // points.
               if (polyVm.PointVms.Count == 1)
               {
                  // Polygon has no more points, delete the polygon Shape

                  polyVm.Parent.ModelObject.RemoveShape(polyVm.ModelObject);
                  polyVm.Parent.Shapes.Remove(polyVm);
               }

               // Before we remove the point
               polyVm.RemovePoint(dp);

            }
            _selectedPoints.Clear();
         }
         else if (_selectedJoints.Count > 0)
         {
            foreach (WeldJointViewModel jvm in _selectedJoints)
            {
               CompoundObjectViewModel covm = jvm.Parent;

               covm.ModelObject.RemoveJoint(jvm.ModelObject);
               covm.Joints.Remove(jvm);

            }
            _selectedJoints.Clear();

         }
         else if (_selectedShapes.Count > 0)
         {
            foreach (LfShapeViewModel svm in _selectedShapes)
            {
               CompoundObjectViewModel covm = svm.Parent;

               covm.ModelObject.RemoveShape(svm.ModelObject);
               covm.RemoveShape(svm);

            }
            _selectedShapes.Clear();
         }
      }

      bool CanDeleteExecute(Object parameter)
      {
         return ((SelectedCompoundObject != null) && 
            ((_selectedShapes.Count > 0) || (_selectedPoints.Count > 0)  || (_selectedJoints.Count > 0) ));
      }

      public ICommand Delete
      {
         get
         {
            return new MicroMvvm.RelayCommand<Object>(parameter => DeleteExecute(parameter), parameter => CanDeleteExecute(parameter));
         }
      }



      #endregion

      #region Public Methods

      // NOTE: FrameworkElement is used to communicate from the View. It can 
      // be used to obtain the ViewModel object from it's datacontext.
      // In some cases we need the type of FrameworkElement (to see if a 
      // Line or Rectangle).

      // All mouse handling function return true if the the data was used for some action
      // (if original event was handled)


      public bool MouseDown(
         MouseEventObjectType objectType, 
         object sender, 
         MouseButton button, 
         Point clickPoint, 
         int clickCount, 
         bool shift, 
         bool ctrl, 
         bool alt)
      {
         if (button == MouseButton.Left)
         {

            if (clickCount > 1)
            {
               if (_LeftClickState == LeftClickState.addPoint)
               {
                  TerminatePointAdding();
                  return true;
               }
            }

            // If we are creating shapes or adding points to a polygon
            // we want to be able to click anywhere (even in an existing shape).
            // Therefore, we return false for any of those cases and hopefully
            // the mouse event will bubble up to the background canvas
            // and thus be handeled in that mouse handler.
            if (_LeftClickState != LeftClickState.none)
            {
               return false;
            }

            // Decode target ViewModel and view object that was clicked
            switch (objectType)
            {
               case MouseEventObjectType.shape:
                  // So far we do not do anything here
                  //LfShapeViewModel shvm = (LfShapeViewModel)sender;
                  return false;

               case MouseEventObjectType.dragableBorder:
                  break;

               case MouseEventObjectType.dragablePoint:

                  LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

                  if (dpvm.IsSelected)
                  {
                     // Could be the beginning of dragging

                  }
                  else
                  {
                     if (!ctrl)
                     {
                        foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                        {
                           selpoint.IsSelected = false;
                        }
                        _selectedPoints.Clear();
                     }

                     _selectedPoints.Add(dpvm);
                     dpvm.IsSelected = true;
                  }

                  return true;
              
               case MouseEventObjectType.compoundObjectBoundaryBox:
                  // So far we do not do anything here
                  // CompoundObjectViewModel covm = (CompoundObjectViewModel)sender;
                  return false;

               case MouseEventObjectType.none:
                  BackgroundMouseDown(clickPoint, button, clickCount, shift, ctrl, alt);
                  return false;
            }
         }

         return false;
      }

      public bool MouseMove(
         MouseEventObjectType objectType,
         object sender,
         Vector dragVector,
         bool shift,
         bool ctrl,
         bool alt)
      {
         // If we are creating shapes or adding points to a polygon
         // we want to be able to click anywhere (even in an existing shape).
         // Therefore, we return false for any of those cases and hopefully
         // the mouse event will bubble up to the background canvas
         // and thus be handeled in that mouse handler.
         if (_LeftClickState != LeftClickState.none)
         {
            return false;
         }

         // Decode target ViewModel and view oobject that was clicked
         switch (objectType)
         {
            case MouseEventObjectType.shape:
               return false;

            case MouseEventObjectType.dragableBorder:
            case MouseEventObjectType.dragablePolygonBorder:

               IPositionInterface posvm = (IPositionInterface)sender;

               foreach (IPositionInterface shape in _selectedShapes)
               {
                  shape.PosX += dragVector.X;
                  shape.PosY += dragVector.Y;
               }

               return true;

            case MouseEventObjectType.dragablePoint:

               // Mouse move on rectangle of DragablePoint
               LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

               // Before moving all vertices, we rotate the vector to match the shape rotation.
               Point vectPoint = new Point(dragVector.X, dragVector.Y);
               Point rotPoint = dpvm.Parent.LocalPointFromRotated(vectPoint);
               Vector rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);
               
               foreach (LfDragablePointViewModel point in _selectedPoints)
               {
                  point.PosX += rotatedDragVector.X;
                  point.PosY += rotatedDragVector.Y;
               }

               return true;

            case MouseEventObjectType.compoundObjectBoundaryBox:

               CompoundObjectViewModel covm = (CompoundObjectViewModel)sender;

               covm.PosX += dragVector.X;
               covm.PosY += dragVector.Y;

               return true;

            case MouseEventObjectType.jointAnchorA:

               WeldJointViewModel wjvm = (WeldJointViewModel)sender;

               // Before moving, we rotate the vector to match the shape rotation.
               vectPoint = new Point(dragVector.X, dragVector.Y);
               rotPoint = wjvm.AShapeObject.LocalPointFromRotated(vectPoint);
               rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);

               // Find new point
               wjvm.AAnchorX += rotatedDragVector.X;
               wjvm.AAnchorY += rotatedDragVector.Y;

               wjvm.OnPropertyChanged("");

               return true;

            case MouseEventObjectType.jointAnchorB:

               wjvm = (WeldJointViewModel)sender;

               // Before moving, we rotate the vector to match the shape rotation.
               vectPoint = new Point(dragVector.X, dragVector.Y);
               rotPoint = wjvm.BShapeObject.LocalPointFromRotated(vectPoint);
               rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);

               // Find new point
               wjvm.BAnchorX += rotatedDragVector.X;
               wjvm.BAnchorY += rotatedDragVector.Y;

               wjvm.OnPropertyChanged("");

               return true;

            case MouseEventObjectType.prismJointUpperLimit:

               PrismaticJointViewModel pjvm = (PrismaticJointViewModel)sender;

               // Before moving, we rotate the vector to match the shape rotation.
               vectPoint = new Point(dragVector.X, dragVector.Y);
               rotPoint = pjvm.AShapeObject.LocalPointFromRotated(vectPoint);
               rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);

               // Find new point
               double newPosX = pjvm.UpperLimitPosX + rotatedDragVector.X;
               double newPosY = pjvm.UpperLimitPosY + rotatedDragVector.Y;

               // Now calculate direction vector and upper limit from point
               Vector v = new Vector(newPosX, newPosY);
               Vector f = new Vector(pjvm.AAnchorX, pjvm.AAnchorY);
               v = v - f;
               pjvm.UpperLimit = v.Length;

               if (pjvm.LowerLimit > pjvm.UpperLimit)
               {
                  pjvm.LowerLimit = pjvm.UpperLimit;
               }

               v.Normalize();
               pjvm.AAxisX = v.X;
               pjvm.AAxisY = v.Y;
               
               pjvm.OnPropertyChanged("");

               return true;

            case MouseEventObjectType.prismJointLowerLimit:

               pjvm = (PrismaticJointViewModel)sender;

               // Before moving, we rotate the vector to match the shape rotation.
               vectPoint = new Point(dragVector.X, dragVector.Y);
               rotPoint = pjvm.AShapeObject.LocalPointFromRotated(vectPoint);
               rotatedDragVector = new Vector(rotPoint.X, rotPoint.Y);

               // Find new point
               newPosX = pjvm.LowerLimitPosX + rotatedDragVector.X;
               newPosY = pjvm.LowerLimitPosY + rotatedDragVector.Y;

               // Now calculate direction vector and upper limit from point
               f = new Vector(pjvm.AAnchorX, pjvm.AAnchorY);
               Vector upperVector = new Vector(pjvm.UpperLimitPosX, pjvm.UpperLimitPosY) - f;
               v = new Vector(newPosX, newPosY) - f;

               upperVector.Normalize();

               pjvm.LowerLimit = Vector.Multiply(upperVector, v);

               if (pjvm.LowerLimit > pjvm.UpperLimit)
               {
                  pjvm.LowerLimit = pjvm.UpperLimit;
               }
               //v.Normalize();
               //pjvm.AAxisX = v.X;
               //pjvm.AAxisY = v.Y;

               pjvm.OnPropertyChanged("");

               return true;

            case MouseEventObjectType.none:
               break;
         }

         return false;
      }

      // clickPoint will be in coordinates of the parent CompoundObject
      public bool MouseUp(
         MouseEventObjectType objectType,
         object sender,
         MouseButton button,
         Point clickPoint,
         int clickCount,
         bool shift,
         bool ctrl,
         bool alt)
      {
         if (button == MouseButton.Left)
         {
            // If we are creating shapes or adding points to a polygon
            // we want to be able to click anywhere (even in an existing shape).
            // Therefore, we return false for any of those cases and hopefully
            // the mouse event will bubble up to the background canvas
            // and thus be handeled in that mouse handler.
            if (_LeftClickState != LeftClickState.none)
            {
               return false;
            }

            // Decode target ViewModel and view oobject that was clicked
            switch (objectType)
            {
               case MouseEventObjectType.shape:

                  // Mouse up on Shape
                  LfShapeViewModel shvm = (LfShapeViewModel)sender;

                  if (shvm.Parent.IsSelected)
                  {
                     if (!ctrl)
                     {
                        foreach (LfShapeViewModel selshape in _selectedShapes)
                        {
                           selshape.IsSelected = false;
                        }
                        _selectedShapes.Clear();

                        foreach (WeldJointViewModel seljoint in _selectedJoints)
                        {
                           seljoint.IsSelected = false;
                        }
                        _selectedJoints.Clear();
                     }

                     _selectedShapes.Add(shvm);
                     shvm.IsSelected = true;
                  }
                  else
                  {
                     if (SelectedCompoundObject != null)
                     {
                        SelectedCompoundObject.DeselectAllChildren();
                        _selectedShapes.Clear();
                        _selectedJoints.Clear();
                        SelectedCompoundObject.IsSelected = false;
                     }
                     SelectedCompoundObject = shvm.Parent;
                  }

                  if (!shvm.IsSelected)
                  {
                     shvm.Parent.IsSelected = true;
                     //_selectedShapes.Add(shvm);
                     SelectedCompoundObject = shvm.Parent;

                  }

                  return true;

               case MouseEventObjectType.jointAnchorA:
               case MouseEventObjectType.jointAnchorB:

                  WeldJointViewModel jvm = (WeldJointViewModel)sender;

                  if (jvm.Parent.IsSelected)
                  {
                     if (!ctrl)
                     {
                        foreach (WeldJointViewModel selJoint in _selectedJoints)
                        {
                           selJoint.IsSelected = false;
                        }
                        _selectedJoints.Clear();

                        foreach (WeldJointViewModel seljoint in _selectedJoints)
                        {
                           seljoint.IsSelected = false;
                        }
                        _selectedJoints.Clear();

                     }

                     _selectedJoints.Add(jvm);
                     jvm.IsSelected = true;
                  }
                  else
                  {
                     if (SelectedCompoundObject != null)
                     {
                        SelectedCompoundObject.DeselectAllChildren();
                        _selectedShapes.Clear();
                        _selectedJoints.Clear();
                        SelectedCompoundObject.IsSelected = false;
                     }
                     SelectedCompoundObject = jvm.Parent;
                  }

                  if (!jvm.IsSelected)
                  {
                     jvm.Parent.IsSelected = true;
                     //_selectedShapes.Add(shvm);
                     SelectedCompoundObject = jvm.Parent;

                  }

                  return true;


               case MouseEventObjectType.dragablePolygonBorder:

                  if (ctrl)
                  {
                     LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

                     // What is the click-point in this case?
                     // It is said to be the closesed parenting canvas which
                     // should be in CompoundObject coordinates. Lets try to convert
                     // this into the rotated shape coordinates. 

                     Point coPoint = clickPoint;

                     Point unrotatedShapePoint = dpvm.Parent.Parent.CoPointInShape(clickPoint, dpvm.Parent);

                     Point rotShapePoint = dpvm.Parent.LocalPointFromRotated(unrotatedShapePoint);

                     LfDragablePointViewModel newPoint = dpvm.Parent.InsertPoint(rotShapePoint, dpvm);
                     foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                     {
                        selpoint.IsSelected = false;
                     }
                     _selectedPoints.Clear();

                     _selectedPoints.Add(newPoint);
                     newPoint.IsSelected = true;
                  }

                  return true;

               case MouseEventObjectType.dragableBorder:

                  return true;

               case MouseEventObjectType.dragablePoint:
                  // So far nothing is done here
                  // LfDragablePointViewModel dpvm = (LfDragablePointViewModel)sender;

                  return true;

               case MouseEventObjectType.compoundObjectBoundaryBox:
                  // So far nothing is done here
                  // CompoundObjectViewModel covm = (CompoundObjectViewModel)sender;

                  return true;

               case MouseEventObjectType.none:
                  return BackgroundMouseUp(clickPoint, button, shift, ctrl, alt);
            }
         }

         return false;
      }

      public bool BackgroundMouseUp(Point clickPoint, MouseButton button, bool shift, bool ctrl, bool alt)
      {
         if (MyCpVm == null) return false;

         if (button == MouseButton.Left)
         {
            //Debug.WriteLine("Clicked on background");
            if (_LeftClickState == LeftClickState.none)
            {
               MyCpVm.DeselectAllChildren();
               MyCpVm.IsSelected = false;
               SelectedCompoundObject = null;
               _selectedShapes.Clear();
               _selectedJoints.Clear();
               _selectedPoints.Clear();
            }
            else if ((_LeftClickState == LeftClickState.staticBox) ||
               (_LeftClickState == LeftClickState.dynamicBox) ||
               (_LeftClickState == LeftClickState.dynamicCircle) ||
               (_LeftClickState == LeftClickState.staticCircle) ||
               (_LeftClickState == LeftClickState.spriteBox))
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(SelectedCompoundObject.PosX, SelectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               LfShape newShape = null;
               LfShapeViewModel newShapeVm = null;

               if (_LeftClickState == LeftClickState.staticBox)
               {
                  newShape = new LfStaticBox();
                  newShapeVm = new LfStaticBoxViewModel(this, SelectedCompoundObject, (LfStaticBox)newShape);
                  SelectedCompoundObject.ModelObject.StaticBoxes.Add((LfStaticBox)newShape);
               }
               else if (_LeftClickState == LeftClickState.dynamicBox)
               {
                  newShape = new LfDynamicBox();
                  newShapeVm = new LfDynamicBoxViewModel(this, SelectedCompoundObject, (LfDynamicBox)newShape);
                  SelectedCompoundObject.ModelObject.DynamicBoxes.Add((LfDynamicBox)newShape);
               }
               else if (_LeftClickState == LeftClickState.staticCircle)
               {
                  newShape = new LfStaticCircle();
                  newShapeVm = new LfStaticCircleViewModel(this, SelectedCompoundObject, (LfStaticCircle)newShape);
                  SelectedCompoundObject.ModelObject.StaticCircles.Add((LfStaticCircle)newShape);
               }
               else if (_LeftClickState == LeftClickState.dynamicCircle)
               {
                  newShape = new LfDynamicCircle();
                  newShapeVm = new LfDynamicCircleViewModel(this, SelectedCompoundObject, (LfDynamicCircle)newShape);
                  SelectedCompoundObject.ModelObject.DynamicCircles.Add((LfDynamicCircle)newShape);
               }
               else if (_LeftClickState == LeftClickState.spriteBox)
               {
                  newShape = new LfSpriteBox();
                  newShapeVm = new LfSpriteBoxViewModel(this, SelectedCompoundObject, (LfSpriteBox)newShape);
                  SelectedCompoundObject.ModelObject.SpriteBoxes.Add((LfSpriteBox)newShape);
               }

               if ((newShape != null) && (newShapeVm != null))
               {
                  newShape.PosX = localClickPoint.X;
                  newShape.PosY = localClickPoint.Y;

                  SelectedCompoundObject.Shapes.Add(newShapeVm);

                  _selectedShapes.Add(newShapeVm);
                  newShapeVm.IsSelected = true;

                  foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                  {
                     selpoint.IsSelected = false;
                  }
                  _selectedPoints.Clear();
               }

               _LeftClickState = LeftClickState.none;

            }
            else if ((_LeftClickState == LeftClickState.staticPolygon) ||
               (_LeftClickState == LeftClickState.dynamicPolygon) ||
               (_LeftClickState == LeftClickState.spritePolygon))
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(SelectedCompoundObject.PosX, SelectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               LfScalableTexturePolygon newPolygon;
               LfScalableTexturePolygonViewModel newPolygonVm;

               if (_LeftClickState == LeftClickState.staticPolygon)
               {
                  newPolygon = new LfStaticPolygon();
                  newPolygonVm = new LfStaticPolygonViewModel(this, SelectedCompoundObject, (LfStaticPolygon)newPolygon);
                  SelectedCompoundObject.ModelObject.StaticPolygons.Add((LfStaticPolygon)newPolygon);
               }
               else if (_LeftClickState == LeftClickState.dynamicPolygon)
               {
                  newPolygon = new LfDynamicPolygon();
                  newPolygonVm = new LfDynamicPolygonViewModel(this, SelectedCompoundObject, (LfDynamicPolygon)newPolygon);
                  SelectedCompoundObject.ModelObject.DynamicPolygons.Add((LfDynamicPolygon)newPolygon);
               }
               else
               {
                  newPolygon = new LfSpritePolygon();
                  newPolygonVm = new LfSpritePolygonViewModel(this, SelectedCompoundObject, (LfSpritePolygon)newPolygon);
                  SelectedCompoundObject.ModelObject.SpritePolygons.Add((LfSpritePolygon)newPolygon);
               }

               newPolygon.PosX = localClickPoint.X;
               newPolygon.PosY = localClickPoint.Y;

               SelectedCompoundObject.Shapes.Add(newPolygonVm);

               _selectedShapes.Add(newPolygonVm);
               newPolygonVm.IsSelected = true;

               LfDragablePointViewModel newPoint = newPolygonVm.InsertPoint(new Point(0, 0), null);

               foreach (LfDragablePointViewModel selpoint in _selectedPoints)
               {
                  selpoint.IsSelected = false;
               }
               _selectedPoints.Clear();

               _selectedPoints.Add(newPoint);
               newPoint.IsSelected = true;

               _LeftClickState = LeftClickState.addPoint;

            }
            else if ((_LeftClickState == LeftClickState.staticBoxedSpritePolygon) ||
               (_LeftClickState == LeftClickState.dynamicBoxedSpritePolygon))
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(SelectedCompoundObject.PosX, SelectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               LfStaticBoxedSpritePolygon newPolygon = null;
               LfStaticBoxedSpritePolygonViewModel newPolygonVm = null;

               if (_LeftClickState == LeftClickState.staticBoxedSpritePolygon)
               {
                  newPolygon = new LfStaticBoxedSpritePolygon();
                  newPolygonVm = new LfStaticBoxedSpritePolygonViewModel(this, SelectedCompoundObject, (LfStaticBoxedSpritePolygon)newPolygon);
                  SelectedCompoundObject.ModelObject.StaticBoxedSpritePolygons.Add((LfStaticBoxedSpritePolygon)newPolygon);
               }
               else
               {
                  newPolygon = new LfDynamicBoxedSpritePolygon();
                  newPolygonVm = new LfDynamicBoxedSpritePolygonViewModel(this, SelectedCompoundObject, (LfDynamicBoxedSpritePolygon)newPolygon);
                  SelectedCompoundObject.ModelObject.DynamicBoxedSpritePolygons.Add((LfDynamicBoxedSpritePolygon)newPolygon);
               }

               newPolygon.PosX = localClickPoint.X;
               newPolygon.PosY = localClickPoint.Y;

               SelectedCompoundObject.Shapes.Add(newPolygonVm);

               _selectedShapes.Add(newPolygonVm);
               newPolygonVm.IsSelected = true;

               LfDragablePointViewModel newPoint = newPolygonVm.InsertPoint(new Point(0, 0), null);

               foreach (LfDragablePointViewModel selpoint in _selectedPoints)
               {
                  selpoint.IsSelected = false;
               }
               _selectedPoints.Clear();

               _selectedPoints.Add(newPoint);
               newPoint.IsSelected = true;

               _LeftClickState = LeftClickState.addPoint;

            }
            else if ((_LeftClickState == LeftClickState.weldJoint) ||
               (_LeftClickState == LeftClickState.revoluteJoint) ||
               (_LeftClickState == LeftClickState.prismaticJoint))
            {
               WeldJoint wj = null;

               if (_LeftClickState == LeftClickState.weldJoint)
               {
                  wj = new WeldJoint();
               }
               else if (_LeftClickState == LeftClickState.revoluteJoint)
               {
                  wj = new RevoluteJoint();
               }
               else
               {
                  wj = new PrismaticJoint();
               }

               wj.AName = _selectedShapes[0].Name;
               wj.BName = _selectedShapes[1].Name;

               WeldJointViewModel wjvm = null;

               if (_LeftClickState == LeftClickState.weldJoint)
               {
                  wjvm = new WeldJointViewModel(this, SelectedCompoundObject, wj);
               }
               else if (_LeftClickState == LeftClickState.revoluteJoint)
               {
                  wjvm = new RevoluteJointViewModel(this, SelectedCompoundObject, (RevoluteJoint)wj);
               }
               else
               {
                  wjvm = new PrismaticJointViewModel(this, SelectedCompoundObject, (PrismaticJoint)wj);
               }

               Point parentObjectOrigo = new Point(SelectedCompoundObject.PosX, SelectedCompoundObject.PosY);

               // Shape A point
               Point shapeAOrigo = new Point(wjvm.AShapeObject.PosX, wjvm.AShapeObject.PosY);
               shapeAOrigo.Offset(parentObjectOrigo.X, parentObjectOrigo.Y);
               Point localAClickPoint = new Point();
               localAClickPoint = (Point)(clickPoint - shapeAOrigo);

               // Rotate point to shape rotation
               Point rotatedAClickPoint = wjvm.AShapeObject.RotatedPointFromLocal(localAClickPoint);

               wjvm.AAnchorX = rotatedAClickPoint.X;
               wjvm.AAnchorY = rotatedAClickPoint.Y;

               // Shape A point
               Point shapeBOrigo = new Point(wjvm.BShapeObject.PosX, wjvm.BShapeObject.PosY);
               shapeBOrigo.Offset(parentObjectOrigo.X, parentObjectOrigo.Y);
               Point localBClickPoint = new Point();
               localBClickPoint = (Point)(clickPoint - shapeBOrigo);

               // Rotate point to shape rotation
               Point rotatedBClickPoint = wjvm.BShapeObject.RotatedPointFromLocal(localBClickPoint);

               wjvm.BAnchorX = rotatedBClickPoint.X;
               wjvm.BAnchorY = rotatedBClickPoint.Y;

               SelectedCompoundObject.Joints.Add(wjvm);
               SelectedCompoundObject.ModelObject.WeldJoints.Add(wj);

               _LeftClickState = LeftClickState.none;

            }
            else if (_LeftClickState == LeftClickState.asteroidField)
            {
               // The first point of this polygon will be the PosX and PosY of the 
               // new shape, and thus, the first polygon vertex should be at 0,0.
               Point parentOrigo = new Point(SelectedCompoundObject.PosX, SelectedCompoundObject.PosY);
               Point localClickPoint = new Point();
               localClickPoint = (Point)(clickPoint - parentOrigo);

               AsteroidFieldRef afr = new AsteroidFieldRef();

               TStateProperties<AsteroidFieldProperties> sp = new TStateProperties<AsteroidFieldProperties>();

               afr.StateProperties.Add(sp);

               AsteroidFieldProperties afp = new AsteroidFieldProperties();
               afp.PosX = localClickPoint.X;
               afp.PosY = localClickPoint.Y;

               sp.Properties = afp;

               AsteroidFieldViewModel afvm = new AsteroidFieldViewModel(this, SelectedCompoundObject, afr);

               SelectedCompoundObject.ModelObject.AsteroidFields.Add(afr);

               SelectedCompoundObject.Shapes.Add(afvm);

               _selectedShapes.Add(afvm);
               afvm.IsSelected = true;

               _LeftClickState = LeftClickState.none;

            }
            else if (_LeftClickState == LeftClickState.addPoint)
            {
               // When adding points to new polygon we require that the
               // shape is the only one selected
               if ((_selectedShapes.Count == 1) && (_selectedShapes[0] is LfPolygonViewModel))
               {
                  LfPolygonViewModel newPolygon = (LfPolygonViewModel)_selectedShapes[0];
                  Point parentObjectOrigo = new Point(newPolygon.Parent.PosX, newPolygon.Parent.PosY);
                  Point shapeOrigo = new Point(newPolygon.PosX, newPolygon.PosY);
                  shapeOrigo.Offset(parentObjectOrigo.X, parentObjectOrigo.Y);
                  Point localClickPoint = new Point();
                  localClickPoint = (Point)(clickPoint - shapeOrigo);

                  LfDragablePointViewModel newPoint = newPolygon.AddPoint(localClickPoint);
                  foreach (LfDragablePointViewModel selpoint in _selectedPoints)
                  {
                     selpoint.IsSelected = false;
                  }
                  _selectedPoints.Clear();

                  _selectedPoints.Add(newPoint);
                  newPoint.IsSelected = true;
               }

            }

            return true;
         }

         return false;
      }

      public bool BackgroundMouseDown(Point clickPoint, MouseButton button, int clickCount, bool shift, bool ctrl, bool alt)
      {
         if (clickCount > 1)
         {

            if (button == MouseButton.Left)
            {
               if (_LeftClickState == LeftClickState.addPoint)
               {
                  _LeftClickState = LeftClickState.none;
                  return true;
               }
            }
         }

         return false;
      }

      public bool KeyDownHandler(KeyEventArgs e)
      {
         if (e.Key == Key.Delete)
         {
            if (CanDeleteExecute(null))
            {
               DeleteExecute(null);
            }
            //if (_selectedPoints.Count > 0)
            //{
            //   foreach (LfDragablePointViewModel dp in _selectedPoints)
            //   {
            //      LfPolygonViewModel polyVm = dp.Parent;

            //      // Is this the last point to be removed? If so, remove the shape
            //      // first so there is no problem with updating something with zero
            //      // points.
            //      if (polyVm.PointVms.Count == 1)
            //      {
            //         // Polygon has no more points, delete the polygon Shape

            //         polyVm.Parent.ModelObject.RemoveShape(polyVm.ModelObject);
            //         polyVm.Parent.Shapes.Remove(polyVm);
            //      }

            //      // Before we remove the point
            //      polyVm.RemovePoint(dp);

            //   }
            //   _selectedPoints.Clear();
            //}
         }

         return false;
      }

      public void RotateSelectedShape(int delta, bool fine)
      {
         foreach (LfShapeViewModel svm in _selectedShapes)
         {
            svm.RotateShape(delta, fine);
         }
      }


      
      #endregion

      #region private Methods

      private void TerminatePointAdding()
      {
         SystemSounds.Beep.Play();

         _LeftClickState = LeftClickState.none;

         // Lets do a brute force invalidate of all points of the polygon
         if ((_selectedShapes.Count() == 1) && (_selectedShapes[0] is LfPolygonViewModel))
         {
            LfPolygonViewModel pvm = (LfPolygonViewModel)_selectedShapes[0];

            foreach (LfDragablePointViewModel dpvm in pvm.PointVms)
            {
               dpvm.OnPropertyChanged("");
            }
         }

      }

      #endregion
   }
}
