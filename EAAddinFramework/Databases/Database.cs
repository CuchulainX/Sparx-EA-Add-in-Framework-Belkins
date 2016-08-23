﻿
using System;
using System.Collections.Generic;
using DB=DatabaseFramework;
using TSF.UmlToolingFramework.Wrappers.EA;
using System.Linq;
using UML=TSF.UmlToolingFramework.UML;

namespace EAAddinFramework.Databases
{
	/// <summary>
	/// Description of Database.
	/// </summary>
	public class Database:DB.Database
	{
		internal Package _wrappedPackage;
		internal DatabaseFactory _factory;
		internal List<Table> _tables;
		private string _name;
		public Database(Package package,DatabaseFactory factory)
		{
			this._wrappedPackage = package;
			this._factory = factory;
		}
		public Database(string name,DatabaseFactory factory)
		{
			this._name = name;
			this._factory = factory;
		}

		public void addTable(DB.Table table)
		{
			//initialize 
			int nbrOfTable = this.tables.Count;
			this._tables.Add(table as Table);
		}
		#region Database implementation

		public string name 
		{
			get 
			{
				if (this._wrappedPackage != null)
				{
					this._name = this._wrappedPackage.name;
				}
				return this._name;
			}
			set 
			{
				this._name = value;
				if (this._wrappedPackage != null)
				{
					this._wrappedPackage.name = _name;
				}
			}
		}

		public string itemType {
			get {return "Database";}
		}
		public string properties {
			get {return this._factory.type;}
		}
		public DB.DataBaseFactory factory 
		{
			get 
			{
				return this._factory;
			}
			set 
			{
				this._factory = (DatabaseFactory) value;
			}
		}
		public string type 
		{
			get 
			{
				return this._factory.type;
			}
			set 
			{
				var newFactory = DatabaseFactory.getFactory(value);
				if (newFactory != null)
				{
					this.factory = newFactory;
				}
				else 
				{
					throw new ArgumentException(string.Format("Database type {0} is not known", value));
				}
			}
		}

		public List<DB.Table> tables 
		{
			get 
			{
				if (this._tables == null)
				{
					this._tables = this.getTablesFromPackage(this._wrappedPackage);
				}
				return this._tables.Cast<DB.Table>().ToList();
			}
			set 
			{
				throw new NotImplementedException();
			}
		}
		private List<Table> getTablesFromPackage(Package package)
		{
			
			var foundTables = new List<Table>();
			if (package != null)
			{
				var ownedElements = package.ownedElements;
				foreach( Class tableElement in ownedElements
									.Where(x => x is Class
				                    && x.stereotypes.Any( y => y.name.Equals("table", StringComparison.CurrentCultureIgnoreCase))))
				{
					foundTables.Add(new Table(this, tableElement));
				}
				//also check subPackages
				foreach (Package ownedPackage in ownedElements.OfType<Package>()) 
				{
					foundTables.AddRange(getTablesFromPackage(ownedPackage));
				}
			}
			return foundTables;
		}
		#endregion

	}
}