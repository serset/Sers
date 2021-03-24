 


namespace Vit.Core.Module.Serialization
{
    public  abstract class Serialization
    {

        public static ISerialization Instance { get; set; } = Serialization_Newtonsoft.Instance;

        //public static readonly Serialization_Newtonsoft Newtonsoft = Serialization_Newtonsoft.Instance;
        //public static readonly Serialization_Text Text = Serialization_Text.Instance;
        //public static readonly Serialization_MessagePack MessagePack = Serialization_MessagePack.Instance;










    }
}
