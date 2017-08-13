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
		public ITable<Coach>             Coaches           { get { return this.GetTable<Coach>(); } }
		public ITable<DbUpdatesLog>      DbUpdatesLog      { get { return this.GetTable<DbUpdatesLog>(); } }
		public ITable<Hors>              Horses            { get { return this.GetTable<Hors>(); } }
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

	[Table("coaches")]
	public partial class Coach
	{
		[Column("id"),    PrimaryKey,  Identity] public long   Id    { get; set; } // integer
		[Column("name"),  NotNull              ] public string Name  { get; set; } // varchar(128)
		[Column("email"),    Nullable          ] public string Email { get; set; } // varchar(50)
		[Column("phone"),    Nullable          ] public string Phone { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// FK_schedules_2_0_BackReference
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
		[Column("id"),       PrimaryKey, Identity] public long   Id       { get; set; } // integer
		[Column("nickname"), NotNull             ] public string Nickname { get; set; } // varchar(255)

		#region Associations

		/// <summary>
		/// FK_schedules_1_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="HorseId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Schedule> schedules { get; set; }

		#endregion
	}

	[Table("schedules")]
	public partial class Schedule
	{
		[Column("id"),          PrimaryKey,  Identity] public long   Id         { get; set; } // integer
		[Column("coach_id"),       Nullable          ] public long?  CoachId    { get; set; } // integer
		[Column("horse_id"),       Nullable          ] public long?  HorseId    { get; set; } // integer
		[Column("interval_id"), NotNull              ] public long   IntervalId { get; set; } // integer
		[Column("name"),        NotNull              ] public string Name       { get; set; } // varchar(50)
		[Column("is_active"),   NotNull              ] public bool   IsActive   { get; set; } // boolean

		#region Associations

		/// <summary>
		/// FK_schedules_2_0
		/// </summary>
		[Association(ThisKey="CoachId", OtherKey="Id", CanBeNull=true, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_2_0", BackReferenceName="schedules")]
		public Coach coach { get; set; }

		/// <summary>
		/// FK_schedules_data_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="ScheduleId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<SchedulesData> data { get; set; }

		/// <summary>
		/// FK_schedules_1_0
		/// </summary>
		[Association(ThisKey="HorseId", OtherKey="Id", CanBeNull=true, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_1_0", BackReferenceName="schedules")]
		public Hors hors { get; set; }

		/// <summary>
		/// FK_schedules_0_0
		/// </summary>
		[Association(ThisKey="IntervalId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_0_0", BackReferenceName="schedules")]
		public SchedulesInterval schedules_interval { get; set; }

		#endregion
	}

	[Table("schedules_data")]
	public partial class SchedulesData
	{
		[Column("id"),                       PrimaryKey,  Identity] public long      Id                      { get; set; } // integer
		[Column("schedule_id"),              NotNull              ] public long      ScheduleId              { get; set; } // integer
		[Column("is_avialable"),             NotNull              ] public bool      IsAvialable             { get; set; } // boolean
		[Column("availability_description"), NotNull              ] public string    AvailabilityDescription { get; set; } // varchar(50)
		[Column("day_number"),                  Nullable          ] public long?     DayNumber               { get; set; } // integer
		[Column("date_on"),                     Nullable          ] public DateTime? DateOn                  { get; set; } // date
		[Column("begin_time"),               NotNull              ] public DateTime  BeginTime               { get; set; } // time
		[Column("end_time"),                 NotNull              ] public DateTime  EndTime                 { get; set; } // time

		#region Associations

		/// <summary>
		/// FK_schedules_data_0_0
		/// </summary>
		[Association(ThisKey="ScheduleId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_schedules_data_0_0", BackReferenceName="data")]
		public Schedule schedulesdata { get; set; }

		#endregion
	}

	[Table("schedules_interval")]
	public partial class SchedulesInterval
	{
		[Column("id"),   PrimaryKey, NotNull] public long   Id   { get; set; } // integer
		[Column("name"),             NotNull] public string Name { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// FK_schedules_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="IntervalId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Schedule> schedules { get; set; }

		#endregion
	}

	[Table("services")]
	public partial class Service
	{
		[Column("id"),   PrimaryKey, Identity] public long   Id   { get; set; } // integer
		[Column("name"), NotNull             ] public string Name { get; set; } // varchar(255)
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
		[Column("id"),   PrimaryKey,  NotNull] public long   Id   { get; set; } // integer
		[Column("name"),    Nullable         ] public string Name { get; set; } // varchar(255)

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
