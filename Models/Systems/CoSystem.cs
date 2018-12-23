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
   public class CoSystem
   {
      #region Declarations

      private string _name;
      private string _type;
      private ObservableCollection<ObjectFactoryProperties> _objFactStateProperties = new ObservableCollection<ObjectFactoryProperties>();
      private ObservableCollection<GunProperties> _gunStateProperties = new ObservableCollection<GunProperties>();
      private ObservableCollection<ShieldProperties> _shieldStateProperties = new ObservableCollection<ShieldProperties>();
      private ObservableCollection<FlameEmitterProperties> _flameEmitterStateProperties = new ObservableCollection<FlameEmitterProperties>();

      #endregion

      #region Constructors

      public CoSystem()
      {
         _name = "notDefined";
         _type = "notDefined";
      }

      #endregion

      #region Properties

      [XmlAttribute("name")]
      public string Name
      {
         get { return _name; }
         set { _name = value; }
      }

      [XmlAttribute("type")]
      public string Type
      {
         get { return _type; }
         set { _type = value; }
      }

      [XmlElement("stateProperties")]
      public ObservableCollection<ObjectFactoryProperties> ObjFactStateProperties
      {
         get { return _objFactStateProperties; }
         set { _objFactStateProperties = value; }
      }

      // TODO: Serialize above on condition that _type == "objectFactory"

      [XmlElement("stateProperties")]
      public ObservableCollection<GunProperties> GunStateProperties
      {
         get { return _gunStateProperties; }
         set { _gunStateProperties = value; }
      }

      // TODO: Serialize above on condition that _type == "gun"

      [XmlElement("stateProperties")]
      public ObservableCollection<ShieldProperties> ShieldStateProperties
      {
         get { return _shieldStateProperties; }
         set { _shieldStateProperties = value; }
      }

      // TODO: Serialize above on condition that _type == "shield"

      [XmlElement("stateProperties")]
      public ObservableCollection<FlameEmitterProperties> FlameEmitterStateProperties
      {
         get { return _flameEmitterStateProperties; }
         set { _flameEmitterStateProperties = value; }
      }

      // TODO: Serialize above on condition that _type == "flameEmitter"


      #endregion

   }
}
