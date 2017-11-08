

using System.ComponentModel;
using Shared.Dto;
using Common.Annotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common.DtoMapping
{

    public partial class CoachDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<CoachDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public CoachDtoUi()
        {
            _adapter = new EditableAdapter<CoachDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


    public partial class ScheduleDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<ScheduleDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public ScheduleDtoUi()
        {
            _adapter = new EditableAdapter<ScheduleDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


    public partial class ScheduleDataDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<ScheduleDataDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public ScheduleDataDtoUi()
        {
            _adapter = new EditableAdapter<ScheduleDataDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


    public partial class HorseDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<HorseDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public HorseDtoUi()
        {
            _adapter = new EditableAdapter<HorseDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


    public partial class ServiceDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<ServiceDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public ServiceDtoUi()
        {
            _adapter = new EditableAdapter<ServiceDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


    public partial class UserDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<UserDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public UserDtoUi()
        {
            _adapter = new EditableAdapter<UserDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


    public partial class UserRoleDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
    {
        private readonly EditableAdapter<UserRoleDto> _adapter;
        public event ItemEndEditEventHandler ItemEndEdit;
        public event ItemEndCancelEventHandler ItemCancelEdit;

        public bool IsEditing { get; set; } = false;
        public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

        public UserRoleDtoUi()
        {
            _adapter = new EditableAdapter<UserRoleDto>(this);
        }

        public void BeginEdit()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                _adapter.BeginEdit();
            }
        }

        public void EndEdit()
        {
            _adapter.EndEdit();
            if (ItemEndEdit != null && IsEditing)
            {
                IsEditing = false;
                ItemEndEdit(this);
            }
        }

        public void CancelEdit()
        {
            IsEditing = false;
            _adapter.CancelEdit();
            if (ItemCancelEdit != null)
                ItemCancelEdit(this);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get
            {
                StringBuilder error = new StringBuilder();

                // iterate over all of the properties
                // of this object - aggregating any validation errors
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
                foreach (PropertyDescriptor prop in props)
                {
                    string propertyError = this[prop.Name];
                    if (!string.IsNullOrEmpty(propertyError))
                    {
                        error.Append((error.Length != 0 ? ", " : "") + propertyError);
                    }
                }

                // apply object level validation rules
                var objectError = ApplyObjectLevelValidations();
                if (!string.IsNullOrEmpty(objectError))
                    error.Append((error.Length != 0 ? ", " : "") + objectError);

                return error.ToString();
            }
        }

    }


}