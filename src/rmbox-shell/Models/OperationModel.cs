using System;
using JetBrains.Annotations;
using Ruminoid.Common2.Collections;
using Ruminoid.Common2.Utils.UserTypes;
using SearchSharp;

namespace Ruminoid.Toolbox.Shell.Models
{
    [UsedImplicitly]
    public record OperationModel
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public Rate Rate { get; init; }

        public string Category { get; init; }

        public string Author { get; init; }

        public Type Type { get; init; }

        public ObservableCollectionEx<OperationModel> Children { get; init; }

        public SearchStorage<OperationModel> SearchStorage;
    }
}
