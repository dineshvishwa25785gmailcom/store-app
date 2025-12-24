using System.ComponentModel.DataAnnotations.Schema;

namespace store_app_apis.Modal
{
    public class MenuPermission
    {
        public string Menucode { get; set; }
        public string Menuname { get; set; }
        public bool Haveview { get; set; }

       
        public bool Haveadd { get; set; }

         
        public bool Haveedit { get; set; }

        
        public bool Havedelete { get; set; }
    }
}
