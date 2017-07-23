using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.DtoMapping
{
    //adapted from https://www.codeproject.com/articles/16408/discard-changes-in-business-objects
    public class EditableAdapter<T>
    {
        private readonly T _observableVm;
        public EditableAdapter(T observableVm)
        {
            _observableVm = observableVm;
        }

        Dictionary<string, dynamic> _props;
        public void BeginEdit()
        {
            //exit if in Edit mode
            //uncomment if  CancelEdit discards changes since the 
            //LAST BeginEdit call is desired action
            //otherwise CancelEdit discards changes since the 
            //FIRST BeginEdit call is desired action
            //if (null != props) return;
            
            _props = new Dictionary<string, dynamic>();

            foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {                                        
                //check if there is set accessor
                if (null != p.GetSetMethod())
                {
                    _props[p.Name] = p.GetValue(_observableVm);
                }
            }
        }

        public void CancelEdit()
        {
            //check for inappropriate call sequence
            if (null == _props) return;

            //restore old values
            foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                //check if there is set accessor
                if (null != p.GetSetMethod())
                {
                    p.SetValue(_observableVm, _props[p.Name]);
                }
            }

            //delete current values
            _props = null;
        }

        public void EndEdit()
        {
            //delete current values
            _props = null;
        }
    }
}
