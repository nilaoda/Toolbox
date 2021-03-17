using System;

namespace Ruminoid.Toolbox.Core
{
    [Serializable]
    public class OperationException : Exception
    {
        #region Constructor

        public OperationException(
            string message,
            Type operationType,
            Exception inner = null) : base(message, inner)
        {
            OperationType = operationType;
        }

        #endregion

        public Type OperationType { get; }

        public override string ToString()
        {
            return $"操作 {OperationType.Name} 引发了异常：\n{base.ToString()}";
        }
    }
}
