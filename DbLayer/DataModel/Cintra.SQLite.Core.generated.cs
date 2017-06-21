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
		public ITable<DbUpdatesLog>     DbUpdatesLog     { get { return this.GetTable<DbUpdatesLog>(); } }
		public ITable<Hors>             Horses           { get { return this.GetTable<Hors>(); } }
		public ITable<Service>          Services         { get { return this.GetTable<Service>(); } }
		public ITable<Trainer>          Trainers         { get { return this.GetTable<Trainer>(); } }
		public ITable<TrainerSchedules> TrainerSchedules { get { return this.GetTable<TrainerSchedules>(); } }
		public ITable<User>             Users            { get { return this.GetTable<User>(); } }
		public ITable<UserRoles>        UserRoles        { get { return this.GetTable<UserRoles>(); } }

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
		[Column("id"),       PrimaryKey, NotNull] public long   Id       { get; set; } // integer
		[Column("nickname"),             NotNull] public string Nickname { get; set; } // varchar(255)
	}

	[Table("services")]
	public partial class Service
	{
		[Column("id"),   PrimaryKey, NotNull] public long   Id   { get; set; } // integer
		[Column("name"),             NotNull] public string Name { get; set; } // varchar(255)
	}

	[Table("trainers")]
	public partial class Trainer
	{
		[Column("id"),    PrimaryKey,  Identity] public long   Id    { get; set; } // integer
		[Column("name"),  NotNull              ] public string Name  { get; set; } // varchar(128)
		[Column("email"),    Nullable          ] public string Email { get; set; } // varchar(50)
		[Column("phone"),    Nullable          ] public string Phone { get; set; } // varchar(50)

		#region Associations

		/// <summary>
		/// FK_trainer_schedules_0_0_BackReference
		/// </summary>
		[Association(ThisKey="Id", OtherKey="TrainerId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<TrainerSchedules> trainerschedules { get; set; }

		#endregion
	}

	[Table("trainer_schedules")]
	public partial class TrainerSchedules
	{
		[Column("id"),                  PrimaryKey, Identity] public long     Id                 { get; set; } // integer
		[Column("trainer_id"),          NotNull             ] public long     TrainerId          { get; set; } // integer
		[Column("is_avialable"),        NotNull             ] public bool     IsAvialable        { get; set; } // boolean
		[Column("availability_status"), NotNull             ] public string   AvailabilityStatus { get; set; } // varchar(50)
		[Column("begin_time"),          NotNull             ] public DateTime BeginTime          { get; set; } // time
		[Column("end_time"),            NotNull             ] public DateTime EndTime            { get; set; } // time

		#region Associations

		/// <summary>
		/// FK_trainer_schedules_0_0
		/// </summary>
		[Association(ThisKey="TrainerId", OtherKey="Id", CanBeNull=false, Relationship=Relationship.ManyToOne, KeyName="FK_trainer_schedules_0_0", BackReferenceName="trainerschedules")]
		public Trainer trainerschedule { get; set; }

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

		public static Service Find(this ITable<Service> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Trainer Find(this ITable<Trainer> table, long Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static TrainerSchedules Find(this ITable<TrainerSchedules> table, long Id)
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
