using System;

namespace Ruminoid.Toolbox.Shell.Models
{
    public record OperationModel
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public Type Type { get; set; }
    }
}
