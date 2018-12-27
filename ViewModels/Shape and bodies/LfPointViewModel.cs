using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeapfrogEditor
{
    class LfPointViewModel : TreeViewViewModel, IPositionInterface
   {
      #region Declarations

      private double _posX;
      private double _posY;

      private MainViewModel _mainVm;
      private IBoxPointsInterface _parent;

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

      #endregion
   }
}
