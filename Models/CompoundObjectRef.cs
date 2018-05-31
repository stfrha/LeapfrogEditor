using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeapfrogEditor
{
   [Serializable]
   public class CompoundObjectRef
   {
      #region Declarations

      private string _name;
      private ObservableCollection<ObjectRefStateProperties> _stateProperties = new ObservableCollection<ObjectRefStateProperties>();

      #endregion

      #region Constructor

      public CompoundObjectRef()
      {

      }

      #endregion

      #region Properties

      [XmlAttribute("name")]
      public string Name
      {
         get { return _name; }
         set { _name = value; }
      }

      [XmlElement("stateProperties")]
      public ObservableCollection<ObjectRefStateProperties> StateProperties
      {
         get { return _stateProperties; }
         set { _stateProperties = value; }
      }
         
      #endregion
   }
}
