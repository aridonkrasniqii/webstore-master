using System.DirectoryServices.ActiveDirectory;

namespace WebStore.Config
{
    public class LdapConfig
    {
        public string Server { get; set; }
        public string BindUserName { get; set; }
        public string BindPassword { get; set; }
        public string Domain { get; set; }
    }
}
