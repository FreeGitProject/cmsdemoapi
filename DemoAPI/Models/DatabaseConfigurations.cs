namespace DemoAPI.Models
{
    public class DatabaseConfigurations
    {
        public string ConnectionString { get; set; }
        public string ChannelMasterDatabaseName { get; set; }
        public string ChannelDomainDatabasePrefix { get; set; }
        public string OmnicxMasterConnectionString { get; set; }
        //public MasterCollections MasterCollections { get; set; }
        //public DomainCollections DomainCollections { get; set; }
    }
}
