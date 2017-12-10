//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : Cintra
	/// Data Source    : Cintra
	/// Server Version : 3.14.2
	/// </summary>
	public partial class CintraDB : LinqToDB.Data.DataConnection
	{
		public ITable<Booking>           Bookings          { get { return this.GetTable<Booking>(); } }
		public ITable<BookingPayments>   BookingPayments   { get { return this.GetTable<BookingPayments>(); } }
		public ITable<Client>            Clients           { get { return this.GetTable<Client>(); } }
		public ITable<Coach>             Coaches           { get { return this.GetTable<Coach>(); } }
		public ITable<DbUpdatesLog>      DbUpdatesLog      { get { return this.GetTable<DbUpdatesLog>(); } }
		public ITable<Hors>              Horses            { get { return this.GetTable<Hors>(); } }
		public ITable<PaymentType>      PaymentTypes      { get { return this.GetTable<PaymentType>(); } }
		public ITable<Schedule>          Schedules         { get { return this.GetTable<Schedule>(); } }
		public ITable<SchedulesData>     SchedulesData     { get { return this.GetTable<SchedulesData>(); } }
		public ITable<SchedulesInterval> SchedulesInterval { get { return this.GetTable<SchedulesInterval>(); } }
		public ITable<Service>           Services          { get { return this.GetTable<Service>(); } }
		public ITable<User>              Users             { get { return this.GetTable<User>(); } }
		public ITable<UserRoles>         UserRoles         { get { return this.GetTable<UserRoles>(); } }

		public CintraDB()
		{
			InitDataContext();
		}

		public CintraDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table("bookings")]
	public partial class Booking
	{
		[Column("id"),         PrimaryKey, Identity] public long     Id        { get; set; } // integer
		[Column("event_guid"), NotNull             ] public Guid     EventGuid { get; set; } // guid
		[Column("horse_id"),   NotNull             ] public long     HorseId   { get; set; } // integer
		[Column("coach_id"),   NotNull             ] public long     CoachId   { get; set; } // integer
		[Column("client_id"),  NotNull             ] public long     ClientId  { get; set; } // integer
		[Column("service_id"), NotNull             ] public long     ServiceId { get; set; } // integer
		[Column("is_deleted"), NotNull             ] public bool     IsDeleted { get; set; } // boolean
		[Column("date_on"),    NotNull             ] public DateTime DateOn    { get; set; } // date
		[Column("begin_time"), NotNull             ] public DateTime BeginTime { get; set; } // time
		[Column("end_time"),   NotNull             ] public DateTime EndTime   { get; set; } // time

		#region Associations

		/// <summary>
		/// FK_booking_payments_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="BookingId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<BookingPayments> bookingpayments { get; set; }

		/// <summary>
		/// FK_bookings_3_0
		/// </summary>
		[Association(ThisKey="ClientId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_bookings_3_0", BackReferenceName="bookings")]
		public Client client { get; set; }

		/// <summary>
		/// FK_bookings_2_0
		/// </summary>
		[Association(ThisKey="CoachId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_bookings_2_0", BackReferenceName="bookings")]
		public Coach coach { get; set; }

		/// <summary>
		/// FK_bookings_1_0
		/// </summary>
		[Association(ThisKey="HorseId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_bookings_1_0", BackReferenceName="bookings")]
		public Hors hors { get; set; }

		/// <summary>
		/// FK_bookings_0_0
		/// </summary>
		[Association(ThisKey="ServiceId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_bookings_0_0", BackReferenceName="bookings")]
		public Service service { get; set; }

		#endregion
	}

	[Table("booking_payments")]
	public partial class BookingPayments
	{
		[Column("id"),              PrimaryKey,  Identity] public long   Id             { get; set; } // integer
		[Column("booking_id"),      NotNull              ] public long   BookingId      { get; set; } // integer
		[Column("payment_type_id"), NotNull              ] public long   PaymentTypeId  { get; set; } // integer
		[Column("isPaid"),          NotNull              ] public bool   IsPaid         { get; set; } // boolean
		[Column("paymentOptions"),     Nullable          ] public string PaymentOptions { get; set; } // varchar(200)
		[Column("is_deleted"),      NotNull              ] public bool   IsDeleted      { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_booking_payments_0_0
		/// </summary>
		[Association(ThisKey="BookingId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_booking_payments_0_0", BackReferenceName="bookingpayments")]
		public Booking bookingpayment { get; set; }

		/// <summary>
		/// FK_booking_payments_1_0
		/// </summary>
		[Association(ThisKey="PaymentTypeId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_booking_payments_1_0", BackReferenceName="bookingpayments")]
		public PaymentType FK_booking_payments_1_0 { get; set; }

		#endregion
	}

	[Table("clients")]
	public partial class Client
	{
		[Column("id"),              PrimaryKey,  Identity] public long   Id             { get; set; } // integer
		[Column("name"),            NotNull              ] public string Name           { get; set; } // varchar(50)
		[Column("email"),              Nullable          ] public string Email          { get; set; } // varchar(50)
		[Column("phone"),              Nullable          ] public string Phone          { get; set; } // varchar(50)
		[Column("age"),             NotNull              ] public object Age            { get; set; } // number
		[Column("weight"),             Nullable          ] public string Weight         { get; set; } // varchar(10)
		[Column("height"),             Nullable          ] public string Height         { get; set; } // varchar(10)
		[Column("contact_details"),    Nullable          ] public string ContactDetails { get; set; } // varchar(200)
		[Column("is_deleted"),      NotNull              ] public bool   IsDeleted      { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_bookings_3_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ClientId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Booking> bookings { get; set; }

		#endregion
	}

	[Table("coaches")]
	public partial class Coach
	{
		[Column("id"),         PrimaryKey,  Identity] public long   Id        { get; set; } // integer
		[Column("name"),       NotNull              ] public string Name      { get; set; } // varchar(128)
		[Column("email"),         Nullable          ] public string Email     { get; set; } // varchar(50)
		[Column("phone"),         Nullable          ] public string Phone     { get; set; } // varchar(50)
		[Column("is_deleted"), NotNull              ] public bool   IsDeleted { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_bookings_2_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="CoachId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Booking> bookings { get; set; }

		/// <summary>
		/// FK_schedules_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="CoachId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Schedule> schedules { get; set; }

		#endregion
	}

	[Table("db_updates_log")]
	public partial class DbUpdatesLog
	{
		[PrimaryKey, Identity] public long     SchemaVersionID { get; set; } // integer
		[Column,     NotNull ] public string   ScriptName      { get; set; } // text(max)
		[Column,     NotNull ] public DateTime Applied         { get; set; } // datetime
	}

	[Table("horses")]
	public partial class Hors
	{
		[Column("id"),         PrimaryKey, Identity] public long   Id        { get; set; } // integer
		[Column("nickname"),   NotNull             ] public string Nickname  { get; set; } // varchar(255)
		[Column("is_deleted"), NotNull             ] public bool   IsDeleted { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_bookings_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="HorseId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Booking> bookings { get; set; }

		#endregion
	}

	[Table("payment_types")]
	public partial class PaymentType
	{
		[Column("id"),         PrimaryKey, Identity] public long   Id        { get; set; } // integer
		[Column("name"),       NotNull             ] public string Name      { get; set; } // varchar(50)
		[Column("is_deleted"), NotNull             ] public bool   IsDeleted { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_booking_payments_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="PaymentTypeId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<BookingPayments> bookingpayments { get; set; }

		#endregion
	}

	[Table("schedules")]
	public partial class Schedule
	{
		[Column("id"),         PrimaryKey, Identity] public long   Id        { get; set; } // integer
		[Column("coach_id"),   NotNull             ] public long   CoachId   { get; set; } // integer
		[Column("name"),       NotNull             ] public string Name      { get; set; } // varchar(50)
		[Column("is_active"),  NotNull             ] public bool   IsActive  { get; set; } // boolean
		[Column("is_deleted"), NotNull             ] public bool   IsDeleted { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_schedules_0_0
		/// </summary>
		[Association(ThisKey="CoachId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_0_0", BackReferenceName="schedules")]
		public Coach coach { get; set; }

		/// <summary>
		/// FK_schedules_data_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ScheduleId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<SchedulesData> data { get; set; }

		#endregion
	}

	[Table("schedules_data")]
	public partial class SchedulesData
	{
		[Column("id"),                       PrimaryKey,  Identity] public long      Id                      { get; set; } // integer
		[Column("schedule_id"),              NotNull              ] public long      ScheduleId              { get; set; } // integer
		[Column("interval_id"),              NotNull              ] public long      IntervalId              { get; set; } // integer
		[Column("event_guid"),               NotNull              ] public Guid      EventGuid               { get; set; } // guid
		[Column("is_avialable"),             NotNull              ] public bool      IsAvialable             { get; set; } // boolean
		[Column("availability_description"), NotNull              ] public string    AvailabilityDescription { get; set; } // varchar(50)
		[Column("day_number"),                  Nullable          ] public long?     DayNumber               { get; set; } // integer
		[Column("date_on"),                     Nullable          ] public DateTime? DateOn                  { get; set; } // date
		[Column("begin_time"),               NotNull              ] public DateTime  BeginTime               { get; set; } // time
		[Column("end_time"),                 NotNull              ] public DateTime  EndTime                 { get; set; } // time
		[Column("is_deleted"),               NotNull              ] public bool      IsDeleted               { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_schedules_data_1_0
		/// </summary>
		[Association(ThisKey="ScheduleId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_data_1_0", BackReferenceName="data")]
		public Schedule FK_schedules_data_1_0 { get; set; }

		/// <summary>
		/// FK_schedules_data_0_0
		/// </summary>
		[Association(ThisKey="IntervalId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_data_0_0", BackReferenceName="schedulesdatas")]
		public SchedulesInterval schedulesdata { get; set; }

		#endregion
	}

	[Table("schedules_interval")]
	public partial class SchedulesInterval
	{
		[Column("id"),   PrimaryKey, NotNull] public long   Id   { get; set; } // integer
		[Column("name"),             NotNull] public string Name { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// FK_schedules_data_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="IntervalId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<SchedulesData> schedulesdatas { get; set; }

		#endregion
	}

	[Table("services")]
	public partial class Service
	{
		[Column("id"),         PrimaryKey, Identity] public long   Id        { get; set; } // integer
		[Column("name"),       NotNull             ] public string Name      { get; set; } // varchar(255)
		[Column("is_deleted"), NotNull             ] public bool   IsDeleted { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_bookings_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ServiceId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Booking> bookings { get; set; }

		#endregion
	}

	[Table("users")]
	public partial class User
	{
		[Column("id"),                    PrimaryKey,  Identity] public long   Id                 { get; set; } // integer
		[Column("login"),                 NotNull              ] public string Login              { get; set; } // varchar(32)
		[Column("name"),                  NotNull              ] public string Name               { get; set; } // varchar(128)
		[Column("password"),              NotNull              ] public string Password           { get; set; } // varchar(255)
		[Column("new_password_on_login"),    Nullable          ] public bool?  NewPasswordOnLogin { get; set; } // boolean
		[Column("role_id"),               NotNull              ] public long   RoleId             { get; set; } // integer
		[Column("email"),                    Nullable          ] public string Email              { get; set; } // varchar(50)
		[Column("phone"),                    Nullable          ] public string Phone              { get; set; } // varchar(50)
		[Column("is_locked"),             NotNull              ] public bool   IsLocked           { get; set; } // boolean
		[Column("is_deleted"),            NotNull              ] public bool   IsDeleted          { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_users_0_0
		/// </summary>
		[Association(ThisKey="RoleId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_users_0_0", BackReferenceName="users")]
		public UserRoles user_roles { get; set; }

		#endregion
	}

	[Table("user_roles")]
	public partial class UserRoles
	{
		[Column("id"),         PrimaryKey,  NotNull] public long   Id        { get; set; } // integer
		[Column("name"),          Nullable         ] public string Name      { get; set; } // varchar(255)
		[Column("is_deleted"),              NotNull] public bool   IsDeleted { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_users_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="RoleId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<User> users { get; set; }

		#endregion
	}

	public static partial class TableExtensions
	{
		public static Booking Find(this ITable<Booking> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static BookingPayments Find(this ITable<BookingPayments> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Client Find(this ITable<Client> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Coach Find(this ITable<Coach> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static DbUpdatesLog Find(this ITable<DbUpdatesLog> table, long SchemaVersionID)
		{
			return table.FirstOrDefault(t =>
				t.SchemaVersionID == SchemaVersionID);
		}

		public static Hors Find(this ITable<Hors> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static PaymentType Find(this ITable<PaymentType> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Schedule Find(this ITable<Schedule> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static SchedulesData Find(this ITable<SchedulesData> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static SchedulesInterval Find(this ITable<SchedulesInterval> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Service Find(this ITable<Service> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static User Find(this ITable<User> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static UserRoles Find(this ITable<UserRoles> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}
