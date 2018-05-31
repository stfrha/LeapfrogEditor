using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   class MainViewModel : MicroMvvm.ViewModelBase
   {

      #region Declarations

      private CompoundObjectRef _myCpRef = new CompoundObjectRef();
      private ObjectRefStateProperties _myStateProp = new ObjectRefStateProperties();
      private CompoundObject _myCP = null;
      private CompoundObjectViewModel _myCpVm = new CompoundObjectViewModel();

      #endregion

      #region Constructor

      public MainViewModel()
      {
         MyCP = CompoundObject.ReadFromFile("landing_scene.xml");
         MyStateProp.CompObj = MyCP;
         MyCpRef.StateProperties.Add(MyStateProp);

         MyCpVm.BuildViewModel(MyCpRef);
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
         set { _myCpVm = value; }
      }

      #endregion

      #region Commands

      void ReloadExecute(Object parameter)
      {
         MyCP = CompoundObject.ReadFromFile("landing_scene.xml");
         MyStateProp.CompObj = MyCP;
         MyCpRef.StateProperties.Add(MyStateProp);

         MyCpVm.BuildViewModel(MyCpRef);

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
         MyCpVm.ModelObject.WriteToFile();
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


      #endregion
   }
}
