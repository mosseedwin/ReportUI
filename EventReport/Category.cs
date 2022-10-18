using System.Collections.Generic;

namespace Report
{
    public class Category
	{
		private string _Key = string.Empty;

		public string Key
		{
			get { return _Key; }
			set { _Key = value ?? string.Empty; }
		}

		private List<string> _Prefixes = new List<string>();

		public List<string> Prefixes
		{
			get { return _Prefixes; }
			set { _Prefixes = value ?? new List<string>(); }
		}
	}
}
