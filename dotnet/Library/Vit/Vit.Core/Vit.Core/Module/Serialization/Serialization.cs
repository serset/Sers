 


namespace Vit.Core.Module.Serialization
{
    public  abstract class Serialization
    {

        public static ISerialization Instance { get; set; } = Serialization_Newtonsoft.Instance;

    }
}
