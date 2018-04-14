using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Shared.Attributes;

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
                if (p.GetSetMethod() != null)
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


        public string HandleMetadataValiadations(string propertyName)
        {
            var property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == propertyName);

            if (property != null)
            {
                //get Meta attr
                var metaAttr = property.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(VmMetaAttribute));
                var isNullable = metaAttr?.NamedArguments?.FirstOrDefault(x => x.MemberName == "IsNullable");

                //do checks
                if (isNullable != null && !(bool)isNullable.Value.TypedValue.Value)
                {

                    if (property.GetValue(_observableVm) == null || string.IsNullOrEmpty(property.GetValue(_observableVm).ToString()))
                        return $"{propertyName} cannot be null or empty";

                    //if collection
                    if (typeof(IEnumerable).IsAssignableFrom(property.DeclaringType)) {
                        var v = (property.GetValue(_observableVm) as IEnumerable).GetEnumerator();

                        if (!v.MoveNext())
                        {
                            return $"{propertyName} cannot be null or empty";
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}
