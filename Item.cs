namespace DropManager
{
    public class Item
    {
        public string name;
        public ushort id;
        public int min;
        public int max;

        public Item(string name, ushort id, int min, int max)
        {
            this.name = name;
            this.id = id;
            this.min = min;
            this.max = max;
        }

        public Item() { }
    }
}
