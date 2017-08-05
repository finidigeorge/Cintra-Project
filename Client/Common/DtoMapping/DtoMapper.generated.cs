
using System.ComponentModel;
using Shared.Dto;
using Common.Annotations;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace Common.DtoMapping
{
		
		public partial class CoachDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<CoachDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsEditing { get; set; } = false;

			public CoachDtoUi()
			{
				_adapter = new EditableAdapter<CoachDto>(this);
			}

			public void BeginEdit()
			{
				if(!IsEditing) 
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
							error.Append((error.Length!=0  ? ", " : "") + propertyError);
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

		
		public partial class HorseDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<HorseDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsEditing { get; set; } = false;

			public HorseDtoUi()
			{
				_adapter = new EditableAdapter<HorseDto>(this);
			}

			public void BeginEdit()
			{
				if(!IsEditing) 
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
							error.Append((error.Length!=0  ? ", " : "") + propertyError);
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

		
		public partial class ServiceDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<ServiceDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsEditing { get; set; } = false;

			public ServiceDtoUi()
			{
				_adapter = new EditableAdapter<ServiceDto>(this);
			}

			public void BeginEdit()
			{
				if(!IsEditing) 
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
							error.Append((error.Length!=0  ? ", " : "") + propertyError);
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

		
		public partial class UserDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<UserDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsEditing { get; set; } = false;

			public UserDtoUi()
			{
				_adapter = new EditableAdapter<UserDto>(this);
			}

			public void BeginEdit()
			{
				if(!IsEditing) 
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

            [JsonIgnore]
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
							error.Append((error.Length!=0  ? ", " : "") + propertyError);
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

		
		public partial class UserRoleDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<UserRoleDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsEditing { get; set; } = false;

			public UserRoleDtoUi()
			{
				_adapter = new EditableAdapter<UserRoleDto>(this);
			}

			public void BeginEdit()
			{
				if(!IsEditing) 
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
							error.Append((error.Length!=0  ? ", " : "") + propertyError);
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