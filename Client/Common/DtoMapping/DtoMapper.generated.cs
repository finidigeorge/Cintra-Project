﻿

using System;
using System.ComponentModel;
using Shared;
using Shared.Dto;
using Common.Annotations;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;

namespace Common.DtoMapping
{

		
		public partial class BookingDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<BookingDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override Guid EventGuid { get; set; }
				
			public override DateTime DateOn { get; set; }
				
			public override DateTime BeginTime { get; set; }
				
			public override DateTime EndTime { get; set; }
				
			public override Int32 DayOfWeek { get; set; }
				
			public override ServiceDto Service { get; set; }
				
			public override BookingPaymentDto BookingPayment { get; set; }
				
			public override BookingTemplateMetadataDto BookingTemplateMetadata { get; set; }
				
			public override String ValidationErrors { get; set; }
				
			public override String ValidationWarnings { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public BookingDtoUi()
			{
				_adapter = new EditableAdapter<BookingDto>(this);
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

		
		public partial class BookingPaymentDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<BookingPaymentDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override Int64 BookingId { get; set; }
				
			public override PaymentTypeDto PaymentType { get; set; }
				
			public override Boolean IsPaid { get; set; }
				
			public override String PaymentOptions { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public BookingPaymentDtoUi()
			{
				_adapter = new EditableAdapter<BookingPaymentDto>(this);
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

		
		public partial class CoachDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<CoachDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Name { get; set; }
				
			public override String Email { get; set; }
				
			public override String Phone { get; set; }

            public override String Notes { get; set; }

            public override CoachRolesEnum? CoachRole { get; set; }
				
			public override Boolean ShowOnlyAssignedCoaches { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

		
		public partial class ClientDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<ClientDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Name { get; set; }
				
			public override String Email { get; set; }
				
			public override String Phone { get; set; }
				
			public override String Age { get; set; }
				
			public override String Weight { get; set; }
				
			public override String Height { get; set; }
				
			public override String ContactDetails { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public ClientDtoUi()
			{
				_adapter = new EditableAdapter<ClientDto>(this);
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Name { get; set; }
				
			public override Boolean IsActive { get; set; }
				
			public override Int64 CoachId { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override Int64 ScheduleId { get; set; }
				
			public override ScheduleIntervalEnum IntervalId { get; set; }
				
			public override Guid EventGuid { get; set; }
				
			public override Boolean IsAvialable { get; set; }
				
			public override String AvailabilityDescription { get; set; }
					
			public override Int64? DayNumber { get; set; }
					
			public override DateTime? DateOn { get; set; }
				
			public override DateTime BeginTime { get; set; }
				
			public override DateTime EndTime { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String NickName { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Name { get; set; }
					
			public override Int64? LengthMinutes { get; set; }
				
			public override Boolean NoHorseRequired { get; set; }

            public override int MaxClients { get; set; }
            public override int MaxHorses { get; set; }
            public override int MaxCoaches { get; set; }

            public override DateTime? BeginTime { get; set; }
					
			public override DateTime? EndTime { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Login { get; set; }
				
			public override String Password { get; set; }
				
			public override String Name { get; set; }
				
			public override String Email { get; set; }
				
			public override String Phone { get; set; }
				
			public override Boolean NewPasswordOnLogin { get; set; }
				
			public override UserRoleDto UserRole { get; set; }
				
			public override Boolean IsLocked { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Name { get; set; }
			
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

		
		public partial class PaymentTypeDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<PaymentTypeDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override String Name { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public PaymentTypeDtoUi()
			{
				_adapter = new EditableAdapter<PaymentTypeDto>(this);
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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

		
		public partial class HorseScheduleDataDtoUi : IAtomicEditableObject, ICustomDataErrorInfo
		{
			private readonly EditableAdapter<HorseScheduleDataDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;
			public event ItemEndCancelEventHandler ItemCancelEdit;

			public bool IsValid => String.IsNullOrEmpty(Error);

				
			public override Int64 Id { get; set; }
				
			public override Int64 HorseId { get; set; }
				
			public override HorsesUnavailabilityEnum UnavailabilityType { get; set; }
					
			public override Int64? DayOfWeek { get; set; }
					
			public override DateTime? StartDate { get; set; }
					
			public override DateTime? EndDate { get; set; }
			
			public bool IsEditing { get; set; } = false;
			public string this[string propertyName] => _adapter.HandleMetadataValiadations(propertyName);

			public HorseScheduleDataDtoUi()
			{
				_adapter = new EditableAdapter<HorseScheduleDataDto>(this);
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
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
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