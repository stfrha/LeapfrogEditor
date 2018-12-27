using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ModestyGE
{
    [Serializable()]
    public class ViewModelBase : INotifyPropertyChanged
    {
       [field: NonSerialized]
       private bool _isDirty;

       [XmlIgnore]
       public bool IsDirty
       {
          get { return _isDirty; }
          set { _isDirty = value; }
       }
       

       // NonSerialized is needed to be able to do a DeepCopy of an object that inherits from ViewModelBase
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propname, bool setDirty = true)
        {
           if (setDirty)
           {
              IsDirty = true;
           }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }
    }
}
