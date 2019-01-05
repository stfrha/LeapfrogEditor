using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LeapfrogEditor
{
   public class CoBehaviourViewModel : MicroMvvm.ViewModelBase
   {
      #region Declarations

      private CoBehaviour ModelObject;
      private SteerableObjectPropertiesViewModel _steerableObjProperties;
      private BreakableObjectPropertiesViewModel _breakableObjProperties;

      #endregion

      #region Constructors

      public CoBehaviourViewModel(MainViewModel mainVm, CompoundObjectViewModel parent, CoBehaviour modelObject) 
      {
         ModelObject = modelObject;

         _steerableObjProperties = new SteerableObjectPropertiesViewModel(mainVm, parent, ModelObject.SteerableObjProps);
         _breakableObjProperties = new BreakableObjectPropertiesViewModel(mainVm, parent, ModelObject.BreakableObjProps);
      }

      #endregion

      #region Properties

      public CoBehaviour LocalModelObject
      {
         get { return ModelObject; }
      }

      public string Type
      {
         get { return LocalModelObject.Type; }
         set
         {
            LocalModelObject.Type = value;
            OnPropertyChanged("Type");
            OnPropertyChanged("BehaviourProperties");
         }
      }

      public BehaviourViewModelBase BehaviourProperties
      {
         get
         {
            if (Type == "steerableObject")
            {
               return _steerableObjProperties;
            }
            else if (Type == "breakableObject")
            {
               return _breakableObjProperties;
            }

            return null;
         }
         //set
         //{
         //   if (Type == "steerableObject")
         //   {
         //      if (value is SteerableObjectPropertiesViewModel)
         //      {
         //         _steerableObjProperties = (SteerableObjectPropertiesViewModel)value;
         //         LocalModelObject.SteerableObjProps = ((SteerableObjectPropertiesViewModel)value).LocalModelObject;
         //         OnPropertyChanged("BehaviourProperties");
         //      }
         //   }
         //   else if (Type == "breakableObject")
         //   {
         //      if (value is BreakableObjectPropertiesViewModel)
         //      {
         //         _breakableObjProperties = (BreakableObjectPropertiesViewModel)value;
         //         LocalModelObject.BreakableObjProps = ((BreakableObjectPropertiesViewModel)value).LocalModelObject;
         //         OnPropertyChanged("BehaviourProperties");
         //      }
         //   }
         //}
      }

      #endregion

      #region private Methods

      #endregion

      #region protected Methods

      #endregion

      #region public Methods

      #endregion
   }
}
