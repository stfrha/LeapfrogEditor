using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapfrogEditor
{
    class TriangleViewModel : MicroMvvm.ViewModelBase
   {
      #region Declarations

      private Triangle _modelObject;

      #endregion

      #region Constructors

      public TriangleViewModel()
      {
         ModelObject = new Triangle();
      }

      #endregion

      #region Properties

      public Triangle ModelObject
      {
         get { return _modelObject; }
         set
         {
            _modelObject = value;
            OnPropertyChanged("");
         }
      }

      public uint Id
      {
         get { return _modelObject.Id; }
         set
         {
            _modelObject.Id = value;
            OnPropertyChanged("Id");
         }
      }

      public uint V1
      {
         get { return _modelObject.V1; }
         set
         {
            _modelObject.V1 = value;
            OnPropertyChanged("V1");
         }
      }

      public uint V2
      {
         get { return _modelObject.V2; }
         set
         {
            _modelObject.V2 = value;
            OnPropertyChanged("V2");
         }
      }

      public uint V3
      {
         get { return _modelObject.V3; }
         set
         {
            _modelObject.V3 = value;
            OnPropertyChanged("V3");
         }
      }

      #endregion
   }
}
