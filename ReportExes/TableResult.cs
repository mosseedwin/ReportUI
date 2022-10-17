namespace Report
{
    internal class TableResult
    {
        public string Name { get; }

        public int Wave { get; }

        public string ServerNumber { get; }

        public int Events { get; set; }

        public int Incidents { get; set; }

        public int Changes { get; set; }

        public double Availability { get; set; }

        public TableResult(Country country)
        {
            Name = country.Name;
            Wave = country.Wave;
            ServerNumber = country.Servers == country.Maximum ? (country.Servers + "") : (country.Servers + "/" + country.Maximum);
        }
    }
}
