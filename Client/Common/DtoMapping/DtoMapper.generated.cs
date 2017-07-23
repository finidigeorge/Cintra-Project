
using System.ComponentModel;
using Shared.Dto;
using Common.Annotations;
using System.Runtime.CompilerServices;

namespace Common.DtoMapping
{
		
		public partial class CoachDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<CoachDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;

			private bool _isEditing = false;

			public CoachDtoUi()
			{
				_adapter = new EditableAdapter<CoachDto>(this);
			}

			public void BeginEdit()
			{
				_isEditing = true;
				_adapter.BeginEdit();
			}

			public void EndEdit()
			{
				_adapter.EndEdit();	
				if (ItemEndEdit != null && _isEditing)
				{
					_isEditing = false;
					ItemEndEdit(this);
				}
			}

			public void CancelEdit()
			{
				_isEditing = false;
				_adapter.CancelEdit();
			}	
			
			public virtual void OnPropertyChanged(string propertyName)
			{
				var propertyChanged = PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		
		public partial class HorseDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<HorseDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;

			private bool _isEditing = false;

			public HorseDtoUi()
			{
				_adapter = new EditableAdapter<HorseDto>(this);
			}

			public void BeginEdit()
			{
				_isEditing = true;
				_adapter.BeginEdit();
			}

			public void EndEdit()
			{
				_adapter.EndEdit();	
				if (ItemEndEdit != null && _isEditing)
				{
					_isEditing = false;
					ItemEndEdit(this);
				}
			}

			public void CancelEdit()
			{
				_isEditing = false;
				_adapter.CancelEdit();
			}	
			
			public virtual void OnPropertyChanged(string propertyName)
			{
				var propertyChanged = PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		
		public partial class ServiceDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<ServiceDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;

			private bool _isEditing = false;

			public ServiceDtoUi()
			{
				_adapter = new EditableAdapter<ServiceDto>(this);
			}

			public void BeginEdit()
			{
				_isEditing = true;
				_adapter.BeginEdit();
			}

			public void EndEdit()
			{
				_adapter.EndEdit();	
				if (ItemEndEdit != null && _isEditing)
				{
					_isEditing = false;
					ItemEndEdit(this);
				}
			}

			public void CancelEdit()
			{
				_isEditing = false;
				_adapter.CancelEdit();
			}	
			
			public virtual void OnPropertyChanged(string propertyName)
			{
				var propertyChanged = PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		
		public partial class UserDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<UserDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;

			private bool _isEditing = false;

			public UserDtoUi()
			{
				_adapter = new EditableAdapter<UserDto>(this);
			}

			public void BeginEdit()
			{
				_isEditing = true;
				_adapter.BeginEdit();
			}

			public void EndEdit()
			{
				_adapter.EndEdit();	
				if (ItemEndEdit != null && _isEditing)
				{
					_isEditing = false;
					ItemEndEdit(this);
				}
			}

			public void CancelEdit()
			{
				_isEditing = false;
				_adapter.CancelEdit();
			}	
			
			public virtual void OnPropertyChanged(string propertyName)
			{
				var propertyChanged = PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		
		public partial class UserRoleDtoUi : IAtomicEditableObject
		{
			private readonly EditableAdapter<UserRoleDto> _adapter;
			public event ItemEndEditEventHandler ItemEndEdit;

			private bool _isEditing = false;

			public UserRoleDtoUi()
			{
				_adapter = new EditableAdapter<UserRoleDto>(this);
			}

			public void BeginEdit()
			{
				_isEditing = true;
				_adapter.BeginEdit();
			}

			public void EndEdit()
			{
				_adapter.EndEdit();	
				if (ItemEndEdit != null && _isEditing)
				{
					_isEditing = false;
					ItemEndEdit(this);
				}
			}

			public void CancelEdit()
			{
				_isEditing = false;
				_adapter.CancelEdit();
			}	
			
			public virtual void OnPropertyChanged(string propertyName)
			{
				var propertyChanged = PropertyChanged;
				if (propertyChanged != null)
				{
					propertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}


}