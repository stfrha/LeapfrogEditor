using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeapfrogEditor
{
    class LfPointViewModel : MicroMvvm.ViewModelBase, IPositionInterface
   {
      #region Declarations

      private double _posX;
      private double _posY;

      private MainViewModel _mainVm;
      private IBoxPointsInterface _parent;

      private bool _isSelected;


      #endregion

      #region Constructors

      public LfPointViewModel(MainViewModel mainVm, IBoxPointsInterface parent, Point p)
      {
         PosX = p.X;
         PosY = p.Y;
         MainVm = mainVm;
         Parent = parent;
         IsSelected = false;
      }

      #endregion

      #region Properties

      public MainViewModel MainVm
      {
         get { return _mainVm; }
         set { _mainVm = value; }
      }

      public IBoxPointsInterface Parent
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
            return _posX;
         }
         set
         {
            _posX = value;
            OnPropertyChanged("PosX");
         }
      }


      public double PosY
      {
         get
         {
            return _posY;
         }
         set
         {
            _posY = value;
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
