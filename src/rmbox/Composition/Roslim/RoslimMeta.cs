using Ruminoid.Toolbox.Core;

namespace rmbox.Composition.Roslim
{
    public class RoslimMeta : IMeta
    {
        public RoslimMeta(
            string name,
            string description,
            string author)
        {
            Name = name;
            Description = description;
            Author = author;
        }

        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
    }
}
