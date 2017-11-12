

using System;
using System.ComponentModel;
using Shared;
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

				
			public new Int64 Id { get; set; }
				
			public new String Name { get; set; }
				
			public new String Email { get; set; }
				
			public new String Phone { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		
		public partial class ScheduleDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<ScheduleDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

				
			public new Int64 Id { get; set; }
				
			public new String Name { get; set; }
				
			public new Boolean IsActive { get; set; }
				
			public new Int64 CoachId { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public ScheduleDtoUi()
			{
				_adapter = new EditableAdapter<ScheduleDto>(this);
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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		
		public partial class ScheduleDataDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<ScheduleDataDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

				
			public new Int64 Id { get; set; }
				
			public new Int64 ScheduleId { get; set; }
				
			public new ScheduleIntervalEnum IntervalId { get; set; }
				
			public new Boolean IsAvialable { get; set; }
				
			public new String AvailabilityDescription { get; set; }
					
			public new Int64 DayNumber { get; set; }
					
			public new DateTime DateOn { get; set; }
				
			public new DateTime BeginTime { get; set; }
				
			public new DateTime EndTime { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public ScheduleDataDtoUi()
			{
				_adapter = new EditableAdapter<ScheduleDataDto>(this);
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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		
		public partial class HorseDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<HorseDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

				
			public new Int64 Id { get; set; }
				
			public new String NickName { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		
		public partial class ServiceDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<ServiceDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

				
			public new Int64 Id { get; set; }
				
			public new String Name { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		
		public partial class UserDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<UserDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

				
			public new Int64 Id { get; set; }
				
			public new String Login { get; set; }
				
			public new String Password { get; set; }
				
			public new String Name { get; set; }
				
			public new String Email { get; set; }
				
			public new String Phone { get; set; }
				
			public new Boolean NewPasswordOnLogin { get; set; }
				
			public new UserRoleDto UserRole { get; set; }
				
			public new Boolean IsLocked { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

		
		public partial class UserRoleDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<UserRoleDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

				
			public new Int64 Id { get; set; }
				
			public new String Name { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

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
			
			[NotifyPropertyChangedInvocator]
			public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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