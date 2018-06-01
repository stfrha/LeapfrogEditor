using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapfrogEditor
{
    class DragablePointViewModel : MicroMvvm.ViewModelBase, IPositionInterface
   {
      #region Declarations

      private MainViewModel _mainVm;
      private DragablePoint _modelObject;
      private EditablePolygonViewModel _parent;

      private bool _isSelected;


      #endregion

      #region Constructors

      public DragablePointViewModel(MainViewModel mainVm, EditablePolygonViewModel parent, DragablePoint modelObject)
      {
         MainVm = mainVm;
         Parent = parent;
         ModelObject = modelObject;
         IsSelected = false;
      }

      #endregion

      #region Properties

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public DragablePoint ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public EditablePolygonViewModel Parent
      {
         get { return _parent; }
         set
         {
            _parent = value;
            OnPropertyChanged("");
         }
      }

      public double PosX
      {
         get
         {
            return _modelObject.PosX;
         }
         set
         {
            _modelObject.PosX = value;

            if (Parent != null)
            {
               Parent.OnPropertyChanged("Points");
               Parent.OnPropertyChanged("ClosedPointVms");
               Parent.OnPropertyChanged("BoundingBox");

               CompoundObjectViewModel p = Parent.Parent;

               while (p != null)
               {
                  p.OnPropertyChanged("BoundingBox");
                  p = p.Parent;
               }
            }

            OnPropertyChanged("PosX");
         }
      }


      public double PosY
      {
         get
         {
            return _modelObject.PosY;
         }
         set
         {
            _modelObject.PosY = value;

            if (Parent != null)
            {
               Parent.OnPropertyChanged("Points");
               Parent.OnPropertyChanged("ClosedPointVms");
               Parent.OnPropertyChanged("BoundingBox");

               CompoundObjectViewModel p = Parent.Parent;

               while (p != null)
               {
                  p.OnPropertyChanged("BoundingBox");
                  p = p.Parent;
               }
            }

            OnPropertyChanged("PosY");
         }
      }

      public bool IsSelected
      {
         get { return _isSelected; }
         set
         {
            _isSelected = value;
            OnPropertyChanged("IsSelected");
         }
      }

      #endregion
   }
}
