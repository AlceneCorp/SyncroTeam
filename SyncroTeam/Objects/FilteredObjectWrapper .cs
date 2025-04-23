using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.UI.Debug
{
    public class FilteredObjectWrapper : ICustomTypeDescriptor
    {
        private object _target;
        private string _filter;

        public FilteredObjectWrapper(object target, string filter)
        {
            _target = target;
            _filter = filter?.ToLower() ?? "";
        }

        public PropertyDescriptorCollection GetProperties()
        {
            var allProps = TypeDescriptor.GetProperties(_target);
            var filtered = allProps.Cast<PropertyDescriptor>()
                .Where(p => p.DisplayName.ToLower().Contains(_filter) || p.Name.ToLower().Contains(_filter))
                .ToArray();

            return new PropertyDescriptorCollection(filtered);
        }

        // Les autres méthodes renvoient l'objet original
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(_target);
        public string GetClassName() => TypeDescriptor.GetClassName(_target);
        public string GetComponentName() => TypeDescriptor.GetComponentName(_target);
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(_target);
        public EventDescriptor GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(_target);
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(_target);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(_target, editorBaseType);
        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(_target);
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(_target, attributes);
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();
        public object GetPropertyOwner(PropertyDescriptor pd) => _target;
    }
}
