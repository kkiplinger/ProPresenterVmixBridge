namespace TiagoViegas.ProPresenterVmixBridge.Entities
{
    public class ProPresenterInstance
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }

        public override string ToString()
        {
            return Name + " - " + Ip + ":" + Port;
        }
        
        public override bool Equals(object other)
        {
            var obj = (ProPresenterInstance) other;
            return obj != null && string.Equals(Name, obj.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
